namespace Weezlabs.Storgage.RestApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Results;

    using DataTransferObjects.Filter;
    using DataTransferObjects.Misc;
    using DataTransferObjects.Space;    
    using FilterService;
    using Helpers;
    using Model.Exceptions;
    using SpaceService;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Weezlabs.Storgage.Model.Enums;

    /// <summary>
    /// Provides ability to make listing of spaces (storgages).
    /// </summary>
    [RoutePrefix("api/spaces")]
    public class SpacesController : ApiController
    {
        private readonly ISpaceSearcher spaceSearcher;
        private readonly ISpaceProvider spaceProvider;
        private readonly IFilterProvider filterProvider;

        /// <summary>
        /// Create controller.
        /// </summary>
        /// <param name="spaceSearcher">Space searcher</param>
        /// <param name="spaceProvider">Space provider</param>
        /// <param name="filterProvider">Filter provider</param>
        public SpacesController(ISpaceSearcher spaceSearcher,
            ISpaceProvider spaceProvider, IFilterProvider filterProvider)
        {
            Contract.Requires(filterProvider != null);
            Contract.Requires(spaceSearcher != null);
            Contract.Requires(spaceProvider != null);

            this.filterProvider = filterProvider;
            this.spaceSearcher = spaceSearcher;
            this.spaceProvider = spaceProvider;
        }


        /// <summary>
        /// Searches spaces by:
        /// </summary>
        /// <param name="longitude">Longitude</param>
        /// <param name="latitude">Latitude</param> 
        /// <param name="maxDistance">Max. distance in miles to space.</param>     
        /// <param name="sizes">Size: small, medium, large, x-large.</param>
        /// <param name="accessTypes">Access type.</param>
        /// <param name="types">Type of space: indoor, outdoor.</param>
        /// <param name="minRate">Min. rate.</param>
        /// <param name="maxRate">Max. rate.</param>
        /// <param name="offset">Offset</param>
        /// <param name="limit">Limitation</param>
        /// <returns>Filtered spaces.</returns>     
        /// <response code="200">Ok.</response>
        /// <response code="400">Bad request. See description to get information about details.</response>
        [HttpGet]
        [Route("searchbydistance")]
        [ResponseType(typeof(GetSpaceWithDistanceResponse))]
        [AllowAnonymous]
        public IHttpActionResult SearchSpaces(Double longitude, Double latitude, Double maxDistance,
            [FromUri] Model.Enums.SizeType[] sizes = null, [FromUri] Model.Enums.SpaceAccessType[] accessTypes = null,
            [FromUri] Model.Enums.SpaceType[] types = null, Decimal? minRate = null, Decimal? maxRate = null,
            Int32 offset = 0, Int32 limit = 0)
        {
            IEnumerable<GetSpaceWithDistanceResponse> response;

            FilterInfo filterInfo =
                new FilterInfo(new GeoPoint { Longitude = longitude, Latitude = latitude },
                    maxDistance, null, sizes, accessTypes, types, minRate, maxRate);

            try
            {
                filterInfo.Verify();
                response = spaceSearcher.SearchByDistance(filterInfo, offset, limit);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok<IEnumerable<GetSpaceWithDistanceResponse>>(response);
        }


        /// <summary>
        /// Searches spaces by:
        /// </summary>     
        /// <param name="westLongitude">West point of bbox.</param>
        /// <param name="northLatitude">North point of bbox.</param>
        /// <param name="eastLongitude">East point of bbox.</param>
        /// <param name="sourthLatitude">Sourth point of bbox.</param>
        /// <param name="sizes">Size: small, medium, large, x-large.</param>
        /// <param name="accessTypes">Access type.</param>
        /// <param name="types">Type of space: indoor, outdoor.</param>
        /// <param name="minRate">Min. rate.</param>
        /// <param name="maxRate">Max. rate.</param>
        /// <param name="offset">Offset</param>
        /// <param name="limit">Limitation</param>
        /// <returns>Filtered spaces.</returns>
        /// <response code="200">Ok.</response>
        /// <response code="400">Bad request. See description to get information about details.</response>
        [HttpGet]
        [Route("searchbybbox")]
        [ResponseType(typeof(GetSpaceResponse))]
        [AllowAnonymous]
        public IHttpActionResult SearchSpaces(Double westLongitude, Double northLatitude, Double eastLongitude, Double sourthLatitude,
            [FromUri] Model.Enums.SizeType[] sizes = null, [FromUri] Model.Enums.SpaceAccessType[] accessTypes = null,
            [FromUri] Model.Enums.SpaceType[] types = null, Decimal? minRate = null, Decimal? maxRate = null, Int32 offset = 0, Int32 limit = 0)
        {
            IEnumerable<GetSpaceResponse> response;

            FilterInfo filterInfo =
                new FilterInfo(null, null,
                    new BoundingBox
                    {
                        TopLeftPoint = new GeoPoint { Longitude = westLongitude, Latitude = northLatitude },
                        BottomRightPoint = new GeoPoint { Longitude = eastLongitude, Latitude = sourthLatitude }
                    },
                    sizes, accessTypes, types, minRate, maxRate);

            try
            {
                filterInfo.Verify();
                response = spaceSearcher.SearchByBBox(filterInfo, offset, limit);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok<IEnumerable<GetSpaceResponse>>(response);
        }


        /// <summary>
        /// Search spaces by filter identifier
        /// </summary>
        /// <param name="filterId">filter identifier</param>
        /// <param name="offset">offset</param>
        /// <param name="limit">records limit</param>
        /// <returns>Instance of the new filter</returns>
        /// <response code="200">A collection of spaces has been returned</response>
        /// <response code="400">Bad request parameters</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("searchbyfilter/{filterId}")]
        [ResponseType(typeof(IEnumerable<GetSpaceResponse>))]
        [Authorize]
        public IHttpActionResult SearchSpaces(Guid filterId, Int32 offset = 0, Int32 limit = 0)
        {
            Guid userID = RequestContext.Principal.GetUserIdFromClaim();
            SavedFilterInfo filter = null;
            IEnumerable<GetSpaceResponse> response;

            try
            {
                filter = filterProvider.Get(filterId, userID);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (AccessDeniedException)
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }

            FilterInfo filterInfo = new FilterInfo(filter.Id, null, null, filter.BBox,
                filter.Sizes, filter.AccessTypes, filter.SpaceTypes, filter.MinRate, filter.MaxRate)
            {
                AvailableSince = filter.RentStartDate
            };

            try
            {
                filterInfo.Verify();
                response = spaceSearcher.SearchByBBox(filterInfo, offset, limit);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok<IEnumerable<GetSpaceResponse>>(response);
        }

        /// <summary>
        /// Updates space by its identifier.
        /// </summary>
        /// <param name="spaceId">Space identifier.</param>
        /// <param name="spaceToUpdate">Space.</param>
        /// <returns>Space after update.</returns>
        /// <response code="200">Ok. Space was updated.</response>
        /// <response code="400">Bad request. See description to get information about details.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden.</response>
        /// <response code="404">Not found. Space to update was not found.</response>
        [HttpPut]
        [Route("{spaceId}")]
        [ResponseType(typeof(GetSpaceResponseForOwner))]
        [Authorize]
        public IHttpActionResult UpdateSpace([FromUri] Guid spaceId, [FromBody] EditSpaceRequest spaceToUpdate)
        {
            try
            {
                var actorId = RequestContext.Principal.GetUserIdFromClaim();
                var result = spaceProvider.Update(spaceId, spaceToUpdate, actorId);
                return Ok<GetSpaceResponseForOwner>(result);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (AccessDeniedException)
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }
            catch (BadRequestException ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete space by its identifier.
        /// </summary>
        /// <param name="spaceId">Identifier of space to delete.</param>
        /// <returns></returns>
        /// <response code="200">Ok. Space was deleted.</response>        
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden.</response>
        /// <response code="404">Not found. Space to delete was not found.</response>
        [HttpDelete]
        [Route("{spaceId}")]
        [Authorize]
        public IHttpActionResult DeleteSpace(Guid spaceId)
        {
            try
            {
                var actorId = RequestContext.Principal.GetUserIdFromClaim();
                spaceProvider.Delete(spaceId, actorId);

                return Ok();
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (AccessDeniedException)
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }
        }

        /// <summary>
        /// Posts photo for space (you can upload few photo at time).
        /// </summary>
        /// <param name="spaceId">Identifier of space</param>
        /// <returns>IEnumerable of uploaded photos.</returns>
        /// <response code="200">Ok. Photos was uploaded.</response>        
        /// <response code="400">Bad request.</response>     
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden</response>
        /// <response code="404">Not found. Space was not found.</response>
        /// <response code="415">Unsupported media type.</response>
        /// <response code="503">Service Unavailable. Problems with uploading files to Amazon S3 bucket.</response>
        [HttpPost]
        [Route("{spaceId}/photo")]
        [ResponseType(typeof(IEnumerable<Photo>))]
        [Authorize]
        public async Task<IHttpActionResult> UploadPhoto(Guid spaceId)
        {
            HttpRequestMessage request = this.Request;
            if (!request.Content.IsMimeMultipartContent())
            {
                var message = new HttpRequestMessage()
                {
                    Content = new StringContent(Resources.Messages.UnsupportedMediaType)
                };
                return new StatusCodeResult(HttpStatusCode.UnsupportedMediaType, message);
            }

            IEnumerable<Photo> uploadedFiles = null;

            try
            {
                var actorId = RequestContext.Principal.GetUserIdFromClaim();

                await Request.Content.ReadAsMultipartAsync<MultipartMemoryStreamProvider>(
                    new MultipartMemoryStreamProvider()).ContinueWith((task) =>
                    {
                        MultipartMemoryStreamProvider provider = task.Result;
                        uploadedFiles = spaceProvider.UploadPhotos(spaceId, actorId, provider.Contents.AsEnumerable());
                    });
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException.GetType() == typeof(IOException))
                    return this.BadRequest(ex.InnerException.Message);
            }
            catch (FileLoadException ex)
            {
                var message = new HttpRequestMessage()
                {
                    Content = new StringContent(ex.Message)
                };
                return new StatusCodeResult(HttpStatusCode.ServiceUnavailable, message);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (AccessDeniedException)
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }
            catch (BadImageFormatException ex)
            {
                return this.BadRequest(ex.Message);
            }
            catch (ImagesUploadOverflowException ex)
            {
                return this.BadRequest(ex.Message);
            }

            return Ok<IEnumerable<Photo>>(uploadedFiles);
        }

        /// <summary>
        /// Delete photos by its identifiers.
        /// </summary>
        /// <param name="spaceId"></param>
        /// <param name="deletePhotoRequest">Photo identifiers for delete.</param>
        /// <returns></returns>
        /// <response code="200">Ok. Photos was uploaded.</response>          
        /// <response code="400">Bad request.</response>        
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden</response>
        /// <response code="404">Not found. Space was not found.</response>
        /// <response code="503">Service Unavailable. Problems with deleting files from Amazon S3 bucket.</response>
        [HttpDelete]
        [Route("{spaceId}/photo")]
        [Authorize]
        public async Task<IHttpActionResult> DeletePhoto(Guid spaceId, [FromBody] DeletePhotoRequest deletePhotoRequest)
        {

            if (deletePhotoRequest == null || !deletePhotoRequest.PhotoIds.Any())
            {
                return this.BadRequest(Resources.Messages.InvalidListOfPhotoForDelete);
            }

            try
            {
                Guid actorId = RequestContext.Principal.GetUserIdFromClaim();
                await Task.Run(() => spaceProvider.DeletePhotos(spaceId, actorId, deletePhotoRequest.PhotoIds.ToList()));
            }
            catch (BadRequestException ex)
            {
                return this.BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (AccessDeniedException)
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }
            catch (DeleteFileException ex)
            {
                var message = new HttpRequestMessage()
                {
                    Content = new StringContent(ex.Message)
                };
                return new StatusCodeResult(HttpStatusCode.ServiceUnavailable, message);
            }

            return Ok();
        }

        /// <summary>
        /// Returns IEnumerable of links to photo for space.
        /// </summary>
        /// <param name="userId">Identifier of User</param>
        /// <returns>IEnumerable of spaces.</returns>
        /// <response code="200">Space list was returned</response>
        /// <response code="401">User is unauthorized.</response>
        /// <response code="403">Access forbidden.</response>
        /// <response code="404">User was not found</response>
        [HttpGet]
        [Route("for/{userId}")]
        [ResponseType(typeof(IEnumerable<GetSpaceResponseForOwner>))]
        [Authorize]
        public IHttpActionResult GetSpacesByUser(Guid userId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }

            var result = spaceProvider.GetSpaces(userId);

            if (result.Count == 0)
            {
                return NotFound();
            }

            if (result.Count == 1 && result[0] == null)
            {
                return Ok<IEnumerable<GetSpaceResponseForOwner>>(new GetSpaceResponseForOwner[0]);
            }

            return Ok<IEnumerable<GetSpaceResponseForOwner>>(result);
        }


        /// <summary>
        /// Post new space
        /// </summary>
        /// <param name="userId">Identifier of user</param>
        /// <param name="space">Space with that should be created</param>
        /// <returns>GUID identifier of created Space instance</returns>
        /// <response code="201">Space with related Address and AdSpace were created.</response>
        /// <response code="400">Bad request. See description to get detailed information.</response>
        /// <response code="401">User is unauthorized.</response>
        /// <response code="403">Access forbidden.</response>
        /// <response code="404">User not found.</response>
        [HttpPost]

        [Route("for/{userId}")]
        [ResponseType(typeof(GetSpaceResponseForOwner))]

        [Authorize]
        public IHttpActionResult PostSpaceForUser(Guid userId, PublishSpaceRequest space)
        {
            if (space == null)
            {
                return BadRequest(Resources.Messages.PostBodyCannotBeNull);
            }

            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }

            try
            {
                var result = spaceProvider.PostSpace(userId, space);
                var location = new Uri(
                    new Uri(Request.RequestUri, RequestContext.VirtualPathRoot),
                    String.Format("api/spaces/{0}", result));

                return Created<GetSpaceResponseForOwner>(location, result);
            }

            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Returns space for owner by identifier.
        /// </summary>
        /// <param name="spaceId">Space identifier.</param>
        /// <returns>Space info.</returns>
        /// <response code="200">Ok.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Fobbiden.</response>
        /// <response code="404">Not found.</response>
        [HttpGet]
        [Route("{spaceId}")]
        [ResponseType(typeof(GetSpaceResponseForOwner))]
        [Authorize]
        public IHttpActionResult GetSpace(Guid spaceId)
        {
            try
            {
                var actorId = RequestContext.Principal.GetUserIdFromClaim();
                var result = spaceProvider.GetSpace(spaceId, actorId);
                return Ok<GetSpaceResponseForOwner>(result);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (AccessDeniedException)
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }
        }

        /// <summary>
        /// Returns public space information by identifier.
        /// </summary>
        /// <param name="spaceId">Space identifier.</param>
        /// <returns>Space info.</returns>
        /// <response code="200">Ok.</response>
        /// <response code="404">Not found.</response>
        [HttpGet]
        [Route("{spaceId}/public")]
        [ResponseType(typeof(GetSpaceResponse))]
        [AllowAnonymous]
        public IHttpActionResult GetSpacePublic(Guid spaceId)
        {
            try
            {
                var result = spaceProvider.GetSpace(spaceId);
                return Ok<GetSpaceResponse>(result);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
        }
        /// <summary>
        /// Returns forecaseted rate or 0 if there is no enoght data according to provided conditions
        /// </summary>
        /// <param name="sizeType">Size type</param>
        /// <param name="zipCode">Zip code string value</param>
        /// <returns></returns>
        [HttpGet]
        [Route("forecast-rate")]
        [ResponseType(typeof(DecimalValue))]
        [AllowAnonymous]
        public IHttpActionResult GetForecastedRate([FromUri] [JsonConverter(typeof(StringEnumConverter))] SizeType? sizeType = null, [FromUri] String zipCode = null)
        {
            if (sizeType == null)
            {
                return this.BadRequest(String.Format(Resources.Messages.MissedQueryParameter, "sizeType", "forecast-rate"));
            }

            if (zipCode == null)
            {
                return this.BadRequest(String.Format(Resources.Messages.MissedQueryParameter, "zipCode", "forecast-rate"));
            }

            var result = spaceProvider.GetForecastedRate((SizeType)sizeType, zipCode);
            return Ok<DecimalValue>( new DecimalValue(result));
        }

    }
}
