namespace Weezlabs.Storgage.SpaceService
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Spatial;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net.Http;

    using DataLayer;
    using DataLayer.ChatAndMessages;
    using DataLayer.Dictionaries;
    using DataLayer.Photo;
    using DataLayer.Spaces;
    using DataLayer.Users;
    using DataTransferObjects.Space;
    using Model;
    using Model.Exceptions;
    using Model.ModelExtension;
    using PhotoService;
    using UtilService;

    public class SpaceProvider : ISpaceProvider
    {
        private readonly ISpaceRepository spaceRepository;
        private readonly IUserReadonlyRepository userRepository;
        private readonly IPhotoRepository photoRepository;
        private readonly IPhotoProvider photoProvider;
        private readonly IUnitOfWork unitOfWork;
        private readonly IAppSettings appSettings;
        private readonly IDictionaryProvider dictionaryProvider;
        private readonly IZipRepository zipRepository;
        private readonly IChatRepository chatRepository;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="spaceRepository">Space repository.</param>
        /// <param name="userRepository">User repository.</param>
        /// <param name="photoRepository">Photo repository.</param>
        /// <param name="photoProvider">Photo provider.</param>
        /// <param name="unitOfWork">Unit of work.</param>
        /// <param name="appSettings">Application settings.</param>
        /// <param name="spaceAddressRepository">Space address repository.</param>
        /// <param name="dictionaryProvider">Dictionary provider.</param>
        /// <param name="zipReadonlyRepository">Zip repository.</param>
        /// <param name="adSpaceRepository">Ad space repository.</param>
        public SpaceProvider(
            ISpaceRepository spaceRepository,
            IUserReadonlyRepository userRepository,
            IPhotoRepository photoRepository,
            IPhotoProvider photoProvider,
            IUnitOfWork unitOfWork,
            IAppSettings appSettings,
            IDictionaryProvider dictionaryProvider,
            IZipRepository zipRepository,
            IChatRepository chatRepository
            )
        {
            Contract.Requires(spaceRepository != null);
            Contract.Requires(userRepository != null);
            Contract.Requires(photoRepository != null);
            Contract.Requires(photoProvider != null);
            Contract.Requires(userRepository != null);
            Contract.Requires(dictionaryProvider != null);
            Contract.Requires(unitOfWork != null);
            Contract.Requires(appSettings != null);
            Contract.Requires(zipRepository != null);
            Contract.Requires(chatRepository != null);

            this.spaceRepository = spaceRepository;
            this.userRepository = userRepository;
            this.photoRepository = photoRepository;
            this.photoProvider = photoProvider;
            this.unitOfWork = unitOfWork;
            this.appSettings = appSettings;
            this.dictionaryProvider = dictionaryProvider;
            this.unitOfWork = unitOfWork;
            this.zipRepository = zipRepository;
            this.chatRepository = chatRepository;
        }

        private IQueryable<Space> GetSpacesWithInclude()
        {
            return spaceRepository.GetAll()
                .Include(x => x.PhotoLibraries)
                .Include(x => x.SizeType)
                .Include(x => x.SpaceAccessType)
                .Include(x => x.SpaceBusies)
                .Include(x => x.SpaceType)
                .Include(x => x.User)
                .Include(x => x.Zip);
        }

        public List<GetSpaceResponseForOwner> GetSpaces(Guid userId)
        {
            var spaceQuery = GetSpacesWithInclude()
                .Where(x => x.UserId == userId && !x.IsDeleted);

            var resultQuery =
                from u
                  in userRepository.GetAll()
                join ur in spaceQuery
                  on u.Id equals ur.UserId
                into outer
                from item
                  in outer.DefaultIfEmpty()
                where u.Id == userId
                select item;

            var firstList = resultQuery.ToList();

            if (firstList.Count == 1 && firstList[0] == null)
            {
                return new List<GetSpaceResponseForOwner>(new GetSpaceResponseForOwner[] { null });
            }

            return firstList.Select(x => new GetSpaceResponseForOwner(x)).ToList();
        }

        /// <summary>
        /// Returns info about space.
        /// </summary>
        /// <param name="spaceId">Space identifier.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <returns>Space info.</returns>
        public GetSpaceResponseForOwner GetSpace(Guid spaceId, Guid actorId)
        {
            var fromDb = spaceRepository.GetAll()
                .Include(x => x.User)
                .Include(x => x.PhotoLibraries)
                .SingleOrDefault(x => x.Id == spaceId);
            if (fromDb == null)
            {
                throw new NotFoundException(String.Format(Resources.Messages.SpaceNotFound, spaceId));
            }

            if (fromDb.UserId != actorId)
            {
                throw new AccessDeniedException();
            }

            var result = new GetSpaceResponseForOwner(fromDb);

            return result;
        }

        /// <summary>
        /// Returns public info about space.
        /// </summary>
        /// <param name="spaceId">Space identifier.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <returns>Space  info.</returns>
        public GetSpaceResponse GetSpace(Guid spaceId)
        {
            var fromDb = spaceRepository.GetAll()
                .Include(x => x.PhotoLibraries)
                .SingleOrDefault(x => x.Id == spaceId);

            if (fromDb == null)
            {
                throw new NotFoundException(String.Format(Resources.Messages.SpaceNotFound, spaceId));
            }

            var result = new GetSpaceResponse(fromDb);

            return result;
        }

        /// <summary>
        /// Upload photos
        /// </summary>
        /// <param name="spaceId">Space identifier.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <param name="formData">IEnumerable of HttpContent contains files.</param>
        /// <returns>Ienumerable of uploaded photos.</returns>
        public IEnumerable<Photo> UploadPhotos(Guid spaceId, Guid actorId, IEnumerable<HttpContent> formData)
        {
            Contract.Requires(formData != null);

            List<HttpContent> files = formData.ToList();

            Int32 maxFileLength = appSettings.GetSetting<Int32>("maxLengthOfImage");
            files.CheckFilesLength(maxFileLength);

            Space space = GetMySpaceById(spaceId, actorId);

            Int32 countOfUploadedPhotos = photoRepository.GetAll().Count(x => x.SpaceId == spaceId);
            Int32 countOfNewPhotos = files.Count();
            Int32 maxPhotosForStorgage = appSettings.GetSetting<Int32>("MaximumPhotoCountForStorgage");

            if (countOfUploadedPhotos + countOfNewPhotos > maxPhotosForStorgage)
            {
                throw new ImagesUploadOverflowException(String.Format(Resources.Messages.MaximumPhotoForStorage,
                    maxPhotosForStorgage,
                    countOfUploadedPhotos, countOfNewPhotos));
            }

            var photos = new List<PhotoLibrary>();

            foreach (HttpContent content in files)
            {
                Stream stream = content.ReadAsStreamAsync().Result;
                String newFileName = content.GenerateFileNameWithExt();

                String file = photoProvider.UploadFileWithThumbnails(appSettings.GetS3SpacesBucket(),
                    appSettings.GetS3SpacesOriginal(),
                    appSettings.GetS3SpacesThumbnails(), newFileName, stream);

                if (!String.IsNullOrWhiteSpace(file))
                {
                    var photoToPost = new PhotoLibrary()
                    {
                        Link = file,
                        SpaceId = spaceId
                    };

                    photos.Add(photoToPost);
                    photoRepository.Add(photos.Last());
                }
            }

            unitOfWork.CommitChanges();
            List<Photo> result = photos.OrderBy(x => x.Id).Select(x => new Photo(x)).ToList();

            // set default first photo if it's first upload
            if (countOfUploadedPhotos == 0 && result.Any())
            {
                space.DefaultPhotoID = result.First().Id;
                result.First().IsDefault = true; //Duplicate logic
            }
            unitOfWork.CommitChanges();

            return result.AsEnumerable();
        }

        /// <summary>
        /// Delete photos by identifiers
        /// </summary>
        /// <param name="spaceId">Space identifier.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <param name="photoIds">List of photo identifiers to delete.</param>
        public void DeletePhotos(Guid spaceId, Guid actorId, List<Guid> photoIds)
        {
            Space space = GetMySpaceById(spaceId, actorId);

            IQueryable<PhotoLibrary> photosToDelete =
                photoRepository.GetAll().Join(photoIds, x => x.Id, id => id, (x, id) => x);
            IEnumerable<String> filesToDelete = photosToDelete.Select(x => x.Link).ToList().AsEnumerable();

            // set new default photo if deleting current default
            if (space.DefaultPhotoID.HasValue && photoIds.Contains(space.DefaultPhotoID.Value))
            {
                Guid? defaultPhotoId = photoRepository.GetAll().Where(x => x.SpaceId == spaceId)
                    .GroupJoin(photoIds, x => x.Id, id => id, (x, id) => new { x, id })
                    .SelectMany(x => x.id.DefaultIfEmpty(), (x, id) => new { x, id })
                    .Where(x => x.id == default(Guid))
                    .OrderBy(x => x.x.x.Id)
                    .Select(x => x.x.x.Id).FirstOrDefault();

                space.DefaultPhotoID = defaultPhotoId != default(Guid) ? defaultPhotoId : null;
                unitOfWork.CommitChanges();
            }
            photoRepository.DeleteRange(photosToDelete);

            photoProvider.DeleteFiles(appSettings.GetS3SpacesBucket(), appSettings.GetS3SpacesOriginal(),
                appSettings.GetS3SpacesThumbnails(), filesToDelete);
        }

        /// <summary>
        /// Method creates or return existing ZipId by ZipCode
        /// Maybe it should be method in ZipRepository but for now it contains logic related to Space creating/editing
        /// </summary>
        /// <param name="zipCode">Zip Code value</param>
        /// <returns></returns>
        private Guid? GetOrCreateZipIdByCode(String zipCode)
        {
            var zipCodeRecord = zipRepository.GetAll().Where(x => x.ZipCode == zipCode).SingleOrDefault(); //Single().Id;

            /*
             // Now we can to create spaces without ZIP codes
            if (space.ZipCode == null)
            {
                throw new NotFoundException(String.Format(Resources.Messages.InvalidZip, space.ZipCode));
            }
            */

            Guid? zipCodeId = zipCodeRecord != null ? zipCodeRecord.Id : (Guid?)null;

            if (zipCodeRecord == null && zipCode != null)
            {
                var newZip = new Zip { ZipCode = zipCode, Rank = 0 };
                zipRepository.Add(newZip);
                unitOfWork.CommitChanges();
                zipCodeId = newZip.Id;

            }

            return zipCodeId;
        }

        /// <summary>
        /// Post Space with one AdSpace and Space Address
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="space">Object PublishSpaceRequest that contains Space/AdSpace/ attributes</param>
        /// <returns>Created space with User/AdSpace</returns>
        public GetSpaceResponseForOwner PostSpace(Guid userId, PublishSpaceRequest space)
        {
            Contract.Requires(space != null);
            Contract.Requires(space.LocationOnMap != null);

            DbGeography point = space.LocationOnMap.GetPoint();
            var newSpace = new Space
            {
                Decription = space.Description,
                Title = space.Title,
                UserId = userId,
                IsListed = space.IsListed,

                Location = point,
                Latitude = point.Latitude.Value,        // geography point must have latitude
                Longitude = point.Longitude.Value,      // geography point must have longitude
                SpaceTypeId = dictionaryProvider.SpaceTypes.Single(x => x.ToEnum() == space.Type).Id,
                SizeTypeId = dictionaryProvider.SizeTypes.Single(x => x.ToEnum() == space.Size).Id,
                SpaceAccessTypeId = dictionaryProvider.SpaceAccessTypes.Single(x => x.ToEnum() == space.Access).Id,

                Rate = (Decimal)space.Rate, //I don't understand why we are using different datatypes, actually it's Money (baced on decimal) datatype.

                ZipId = GetOrCreateZipIdByCode(space.ZipCode), //required attribute

                FullAddress = space.FullAddress,
                ShortAddress = space.ShortAddress,
                AvailableSince = DateTimeOffset.Now
            };
            //May be it would be better to post Space with photos and set default photo but they must be loaded before and we must have ID

            spaceRepository.Add(newSpace);

            try
            {
                unitOfWork.CommitChanges();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("fk_Space_UserId"))
                {
                    throw new NotFoundException(String.Format(Resources.Messages.UserNotFound, userId));
                }
            }

            /*
             * We can do something like this but it's "way to bug"
             * this is why we reload entity tree compleatly
             * 
            newSpace.User = userRepository.GetById(userId);
            newSpace.SizeType = dictionaryProvider.SizeTypes.Single(x => x.ToEnum() == space.Size);
            newSpace.SpaceType = dictionaryProvider.SpaceTypes.Single(x => x.ToEnum() == space.Type);
            newSpace.SpaceAccessType = dictionaryProvider.SpaceAccessTypes.Single(x => x.ToEnum() == space.Access);
            */

            newSpace = GetSpacesWithInclude()
                .Where(x => x.Id == newSpace.Id).Single();

            var result = new GetSpaceResponseForOwner(newSpace);

            //retrn ID of space that was created by PublishSpaceRequest model
            //return newSpace.Id;
            return result;

        }

        /// <summary>
        /// Delete Space by Id that means "logical" deletion and related entities will not be removed but this Space can't be used in future fornew AdSpaces and etc.
        /// all related sata (AdSpaces, chats, offeres, paymets) will be saved and availavle
        /// </summary>
        /// <param name="id">Space identifier that should be disabled/deleted</param>
        /// <param name="actorId">Actor identifier.</param>
        public void Delete(Guid id, Guid actorId)
        {
            var fromDb = GetMySpaceById(id, actorId);

            spaceRepository.Delete(fromDb);

            unitOfWork.CommitChanges();
        }

        /// <summary>
        /// Full Space update
        /// </summary>
        /// <param name="spaceId">Space identifier</param>
        /// <param name="spaceToUpdate">New Space attributes that will be set</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <returns>Curret space with new attributes</returns>
        public GetSpaceResponseForOwner Update(Guid spaceId, EditSpaceRequest spaceToUpdate, Guid actorId)
        {
            var space = GetMySpaceById(spaceId, actorId);

            if (space.IsOccupied == true && spaceToUpdate.IsListed == true &&
                space.IsListed != true //We ignore case when IsListed already has value "1", we can't just SET "1" and, "Yes, I mean space.IsListed = false"
                )
            {
                throw new BadRequestException(Resources.Messages.SpaceIsOccupied);
            }

            space.SpaceAccessTypeId = dictionaryProvider.SpaceAccessTypes.Single(x => x.ToEnum() == spaceToUpdate.Access).Id;

            space.Decription = spaceToUpdate.Description;
            // Will it separated method? space.DefaultPhotoID

            //may be it should be AdSpace attrbute?
            space.IsListed = spaceToUpdate.IsListed;
            space.Location = spaceToUpdate.LocationOnMap.GetPoint();
            //Whe it's in DTO?
            //space.Rate = spaceToUpdate.Rate;
            space.SizeTypeId = dictionaryProvider.SizeTypes.Single(x => x.ToEnum() == spaceToUpdate.Size).Id;
            space.Title = spaceToUpdate.Title;
            space.SpaceTypeId = dictionaryProvider.SpaceTypes.Single(x => x.ToEnum() == spaceToUpdate.Type).Id;

            //Setup Address' attributes 
            space.FullAddress = spaceToUpdate.FullAddress;
            space.ShortAddress = spaceToUpdate.ShortAddress;
            space.ZipId = GetOrCreateZipIdByCode(spaceToUpdate.ZipCode);

            //AdSpace attributes
            space.Rate = (decimal)spaceToUpdate.Rate;

            spaceRepository.Update(space);
            unitOfWork.CommitChanges();

            return new GetSpaceResponseForOwner(space);
        }

        /// <summary>
        /// Retuns forecasted rate by size type and zipCode
        /// </summary>
        /// <param name="sizeType">space size type</param>
        /// <param name="zipCode">space zip code</param>
        /// <returns>forecased rate value</returns>
        public decimal GetForecastedRate(Model.Enums.SizeType sizeType, String zipCode)
        {
            var sizeTypeId = dictionaryProvider.SizeTypes.Single(x => x.ToEnum() == sizeType).Id;

            var result = spaceRepository.GetForecastedRate(sizeTypeId, zipCode);

            return result;

        }

        /// <summary>
        /// Get Space by id with checking access.
        /// </summary>
        /// <param name="spaceId">Identifier of space.</param>
        /// <param name="actorId">Identifier of actor.</param>
        /// <returns>Spaсe if actor is owner of space.</returns>
        private Space GetMySpaceById(Guid spaceId, Guid actorId)
        {
            Space space = spaceRepository.GetById(spaceId);
            if (space == null)
            {
                throw new NotFoundException(String.Format(Resources.Messages.SpaceNotFound, spaceId));
            }

            if (space.UserId != actorId)
            {
                throw new AccessDeniedException();
            }

            return space;
        }
    }
}
