namespace Weezlabs.Storgage.AbuseService
{

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net.Http;

    using DataLayer;
    using DataLayer.Abuse;
    using DataLayer.Dictionaries;
    using DataTransferObjects.Abuse;
    using DataTransferObjects.Misc;
    using Model;
    using Model.Exceptions;
    using Model.ModelExtension;
    using PhotoService;
    using UtilService;

    /// <summary>
    /// Methods for Abuse and Contact Us
    /// </summary>
    public class AbuseProvider : IAbuseProvider
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAbuseRepository abuseRepository;
        private readonly IDictionaryProvider dictionaryProvider;
        private readonly IPhotoProvider photoProvider;
        private readonly IAppSettings appSettings;

        /// <summary>
        /// Abuse Provider default counstructor with injected parameters
        /// </summary>
        /// <param name="unitOfWork">Unit Of Work </param>
        /// <param name="abuseRepository">Abuse Repository</param>
        /// <param name="dictionaryProvider">Dictionary Provider</param>
        /// <param name="photoProvider">Photo Provider</param>
        /// <param name="appSettings">Application Settings</param>
        public AbuseProvider(
            IUnitOfWork unitOfWork,
            IAbuseRepository abuseRepository,
            IDictionaryProvider dictionaryProvider,
            IPhotoProvider photoProvider,
            IAppSettings appSettings
            )
        {
            Contract.Requires(unitOfWork != null);
            Contract.Requires(abuseRepository != null);
            Contract.Requires(dictionaryProvider != null);
            Contract.Requires(photoProvider != null);
            Contract.Requires(appSettings != null);

            this.unitOfWork = unitOfWork;
            this.abuseRepository = abuseRepository;
            this.dictionaryProvider = dictionaryProvider;
            this.photoProvider = photoProvider;
            this.appSettings = appSettings;
        }

        /// <summary>
        /// Posts Abuse.
        /// </summary>
        /// <param name="request">Post Abuse Request.</param>
        /// <returns>Created rating.</returns>
        public AbuseInfo PostAbuse(AbuseInternalRequest request)
        {
            if (request.RatingId == null && request.SpaceId == null && request.ContactUsType == null)
            {
                throw new BadRequestException();
            }

            var abuse = new Abuse()
            {
                Message = request.Message,
                ReporterId = request.ReporterId
            };


            if (request.SpaceId != null)
            {
                abuse.AbuseSpaces.Add(
                    new AbuseSpace()
                    {
                        SpaceId = (Guid)request.SpaceId
                    }
                    );
            }

            if (request.RatingId != null)
            {
                abuse.AbuseRatings.Add(
                    new AbuseRating()
                    {
                        RatingId = (Guid)request.RatingId
                    }
                    );
            }


            if (request.AbuseType != null)
            {
                IEnumerable<Guid> abuseTypeIDs = dictionaryProvider.AbuseTypeDictionary.Join(request.AbuseType, t => t.ToEnumAbuse(), et => et, (t, et) => t.Id);

                abuse.AbuseTypes = abuseTypeIDs.Select(id => new AbuseType { TypeId = id }).ToArray();
            }

            if (request.ContactUsType != null)
            {
                var contactUsType = new AbuseType { TypeId = dictionaryProvider.AbuseTypeDictionary.Single(x => x.ToEnumContactUs() == (Model.Enums.ContactUsDictionary)request.ContactUsType).Id };

                abuse.AbuseTypes = (new AbuseType[] { contactUsType }).Concat(abuse.AbuseTypes).ToArray();
            }

            abuseRepository.Add(abuse);

            try
            {
                unitOfWork.CommitChanges();
            }

            catch (DataException ex)
            {
                if (ex.ToString().Contains("fk_AbuseSpace_SpaceId"))
                {
                    throw new NotFoundException(String.Format(Resources.Messages.SpaceDoesntExist, request.SpaceId));
                }
                else if (ex.ToString().Contains("fk_AbuseRating_RatingId"))
                {
                    throw new NotFoundException(String.Format(Resources.Messages.RatingDoesntExist, request.RatingId));
                }
                else if (ex.ToString().Contains("idx_AbuseType_AbuseId_TypeId"))
                {
                    throw new BadRequestException(Resources.Messages.AbuseTypeUnique);
                };

                //fk_AbuseType_AbuseId

                throw ex;
            }

            var dbAbuse = abuseRepository.GetAll()
                .Where( a => a.Id == abuse.Id)
                .Include(a => a.AbuseTypes.Select(at => at.AbuseTypeDictionary))
                .SingleOrDefault();
            return new AbuseInfo(dbAbuse);

            //return new AbuseInfo(abuse) { AbuseType = request.AbuseType };
        }

        public AbuseInfo GetAbuse(Guid abuseId)
        {
            var abuse = abuseRepository
                .GetAll()
                .Where(a => a.Id == abuseId)
                .AttachIncludes()
                .SingleOrDefault();

            if (abuse == null)
            {
                throw new NotFoundException(String.Format(Resources.Messages.AbuseDoesntExist, abuseId));
            }

            return new AbuseInfo(abuse);
        }

        /// <summary>
        /// Upload file for abuse.
        /// </summary>
        /// <param name="abuseId">Abuse identifier.</param>
        /// <param name="userId">autorised User Id.</param>
        /// <param name="formData">IEnumerable of files.</param>
        public UploadedFile UploadFile(Guid abuseId, Guid userId, IEnumerable<HttpContent> formData)
        {
            //In this case we must check BEFORE inserting(actually uploading) file

            //abuseRepository.GetById(abuseId)
            //prevent redundand call
            if (!(abuseRepository.GetAll().Where(a => (a.Id == abuseId) && (a.ReporterId == userId)).Any()))
            {
                //User doesn't have access to the Abuse/ContactUd
                throw new AccessDeniedException();
            }

            List<HttpContent> files = formData.ToList();

            Int32 countOfNewFiles = files.Count();

            if (countOfNewFiles == 0 || countOfNewFiles > 1)
            {
                throw new BadRequestException(Resources.Messages.SingleFileMustBeAttached);
            };

            //Will use the same setting for now because Image can be sent and I think that 1Mb is good value for now
            Int32 maxFileLength = appSettings.GetSetting<Int32>("maxLengthOfImage");
            files.CheckFilesLength(maxFileLength);

            HttpContent fileContent = files.Single();

            Stream stream = fileContent.ReadAsStreamAsync().Result;
            String newFileName = fileContent.GenerateFileNameWithExt();
            String pathToOriginal = String.Format("{0}/", appSettings.GetS3ContactUsOriginal());


            var filePath = photoProvider.UploadFile
                (
                appSettings.GetS3ContactUsBucket(),
                pathToOriginal,
                newFileName,
                stream
                );

            abuseRepository.UpdateFile(abuseId, newFileName);
            unitOfWork.CommitChanges();

            return new UploadedFile(newFileName, appSettings.GetS3ContactUsFileUrl() );
        }

    }
}
