namespace Weezlabs.Storgage.RestApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Description;

    using DataTransferObjects.Filter;
    using DataTransferObjects.Space;
    using FilterService;
    using Helpers;
    using Model;
    using Model.Exceptions;
    using SpaceService;

    /// <summary>
    /// Provides methods of space filtering functionality
    /// </summary>
    [RoutePrefix("api/filters")]
    public class FiltersController : ApiController
    {
        private readonly IFilterProvider filterProvider;
        private readonly IZipCodeProvider zipCodeProvider;
        private readonly ISpaceSearcher spaceSearcher;

        /// <summary>
        /// Create controller.
        /// </summary>
        /// <param name="spaceSearcher">Space Searcher</param>
        /// <param name="zipCodeProvider">Zip code provider</param>
        /// <param name="filterProvider">Filter provider</param>
        public FiltersController(ISpaceSearcher spaceSearcher, IFilterProvider filterProvider, 
            IZipCodeProvider zipCodeProvider)
        {
            Contract.Requires(filterProvider != null);
            Contract.Requires(zipCodeProvider != null);
            Contract.Requires(spaceSearcher != null);

            this.filterProvider = filterProvider;
            this.zipCodeProvider = zipCodeProvider;
            this.spaceSearcher = spaceSearcher;
        }

        /// <summary>
        /// Provides a report of spaces quantity per each user saved filter
        /// </summary>
        /// <param name="userId">user identifer</param>
        /// <returns>Number of spacer per each filter</returns>
        /// <response code="200">Ok. Report is built</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Authorize]
        [Route("~/api/users/{userId}/saved-filters")]
        [ResponseType(typeof(IEnumerable<GetSpacesReportByFilters>))]
        public IHttpActionResult CalculateSpacesByFilters(Guid userId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }

            IEnumerable<GetSpacesReportByFilters> result = spaceSearcher.CalculateSpacesByFilters(userId);

            return Ok<IEnumerable<GetSpacesReportByFilters>>(result);
        }

        /// <summary>
        /// Creates new user filter for searching spaces
        /// </summary>
        /// <param name="request">instance of CreateFilterRequest</param> 
        /// Can be null or must contain coordinates of the top left and bottom right point</param>
        /// <returns>Instance of the new filter</returns>
        /// <response code="200">Filter has been created</response>
        /// <response code="400">Bad request parameters</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Route("")]
        [Authorize]
        [ResponseType(typeof(SavedFilterInfo))]
        public IHttpActionResult Create(CreateFilterRequest request)
        {
            if (request == null)
            {
                return this.BadRequest(Resources.Messages.IncomingParameterRequired);
            }

            Guid userID = RequestContext.Principal.GetUserIdFromClaim();
            BoundingBox boundingBox = request.BoundingBox;
            IEnumerable<Model.Enums.SizeType> sizeTypes = request.SizeTypes;
            IEnumerable<Model.Enums.SpaceAccessType> accessTypes = request.AccessTypes;
            IEnumerable<Model.Enums.SpaceType> types = request.Types;
            Decimal? minPrice = request.MinPrice;
            Decimal? maxPrice = request.MaxPrice;
            DateTimeOffset rentStartDate = request.RentStartDate;
            String location = request.Location;
            String zipCode = String.IsNullOrEmpty(request.ZipCode) ? null : request.ZipCode;

            SavedFilterInfo filter = new SavedFilterInfo(boundingBox, sizeTypes, accessTypes, 
                types, minPrice, maxPrice, rentStartDate, location, zipCode);

            try
            {
                filter.Verify();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }

            Zip postalCode = zipCode == null ? null : zipCodeProvider.Get(zipCode);
            filter = filterProvider.Create(userID, accessTypes, types, sizeTypes, 
                minPrice, maxPrice, rentStartDate, boundingBox, location, 
                postalCode == null ? (Guid?)null : postalCode.Id);

            return Ok<SavedFilterInfo>(filter);
        }

        /// <summary>
        /// Deletes user saved filters
        /// </summary>
        /// <param name="request">collection of filter identifiers</param>
        /// <returns>Empty result if the filters have been successfully deleted</returns>
        /// <response code="200">Filters have been deleted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access is forbidden</response>
        [HttpDelete]
        [Authorize]
        [Route("")]
        public IHttpActionResult DeleteFilters(DeleteFiltersRequest request)
        {
            if (request == null || request.FilterIDs == null)
            {
                return this.BadRequest(Resources.Messages.IncomingParameterRequired);
            }

            try
            {
                Guid userID = RequestContext.Principal.GetUserIdFromClaim();
                filterProvider.Delete(userID, request.FilterIDs);

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
    }
}