namespace Weezlabs.Storgage.RestApi.Controllers
{
    using AbuseService;
    using DataTransferObjects.Abuse;
    using Helpers;
    using Model.Exceptions;
    using Resources;
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Weezlabs.Storgage.DataTransferObjects.Misc;


    /// <summary>
    /// Provides possibility to work with Abuses
    /// </summary>
    [RoutePrefix("api")]
    public class AbuseController : ApiController
    {
        private readonly IAbuseProvider abuseProvider;

        /// <summary>
        /// Create controller.
        /// </summary>
        /// <param name="ratingProvider">Space Searcher.</param>
        public AbuseController(IAbuseProvider abuseProvider)
        {
            Contract.Requires(abuseProvider != null);
            this.abuseProvider = abuseProvider;
        }

        /// <summary>
        /// Post new abuse for Space.
        /// <param name="spaceId">Space identifier.</param>
        /// <param name="request">Request for posting new abuse.</param>
        /// </summary>
        /// <returns>Posted rating.</returns>
        /// <response code="201">Abuse was posted.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Space cannot be found.</response>
        [HttpPost]
        [Route("spaces/{spaceId}/report-abuse")]
        [Authorize]
        [ResponseType(typeof(AbuseInfo))]
        public IHttpActionResult PostSpaceAbuse(Guid spaceId, [FromBody] AbuseRequest request)
        {
            return PostAbuse(new AbuseInternalRequest(request) { SpaceId = spaceId });
        }

        /// <summary>
        /// Post abuse for rating .
        /// <param name="ratingId">Rating identifier.</param>
        /// <param name="request">Request for posting new abuse.</param>
        /// </summary>
        /// <returns>Posted rating.</returns>
        /// <response code="201">Abuse was posted.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Rating cannot be found.</response>
        [HttpPost]
        [Route("ratings/{ratingId}/report-abuse")]
        [Authorize]
        [ResponseType(typeof(AbuseInfo))]
        public IHttpActionResult PostRatingAbuse(Guid ratingId, [FromBody] AbuseRequest request)
        {
            return PostAbuse(new AbuseInternalRequest(request) { RatingId = ratingId });
        }

        /// <summary>
        /// Returns abuse info
        /// <param name="abuseId">Abuse identifier.</param>        
        /// </summary>
        /// <returns>Posted rating.</returns>
        /// <response code="200">Abuse.</response>
        /// <response code="404">Rating cannot be found.</response>
        [HttpGet]
        [Route("abuse/{abuseId}")]
        [ResponseType(typeof(AbuseInfo))]
        [AllowAnonymous]
        public IHttpActionResult GetAbuse(Guid abuseId)
        {
            try
            {
                return Ok<AbuseInfo>(abuseProvider.GetAbuse(abuseId));
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Contact Us
        /// <param name="request">Request for posting new abuse.</param>
        /// </summary>
        /// <returns>Posted rating.</returns>
        /// <response code="201">Abuse/ContactUs was posted.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Space cannot be found.</response>
        [HttpPost]
        [Route("contact-us")]
        [Authorize]
        [ResponseType(typeof(AbuseInfo))]
        public IHttpActionResult ContactUs([FromBody] AbuseRequest request)
        {
            return PostAbuse(new AbuseInternalRequest(request) { });
        }

        /// <summary>
        /// Contact Us
        /// <param name="request">Request for posting new abuse.</param>
        /// </summary>
        /// <returns>Posted rating.</returns>
        /// <response code="201">Abuse/ContactUs was posted.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpPost]
        [Route("contact-us/upload-file")]
        [Authorize]
        [ResponseType(typeof(UploadedFile))]
        public async Task<IHttpActionResult> ContactUsUploadFile([FromUri] Guid abuseId)
        {
            UploadedFile file = null;

            try
            {
                await Request.Content.ReadAsMultipartAsync<MultipartMemoryStreamProvider>(
                   new MultipartMemoryStreamProvider()).ContinueWith((task) =>
                   {
                       MultipartMemoryStreamProvider provider = task.Result;
                       file = abuseProvider.UploadFile(abuseId, RequestContext.Principal.GetUserIdFromClaim(), provider.Contents.AsEnumerable());
                   });
                return this.Ok(file);
            }
            catch (BadRequestException e)
            {
                return this.BadRequest(e.Message);
            }
            catch (AccessDeniedException e)
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }
            catch (FileLoadException e)
            {
                return this.ServiceUnavailable(e.Message);
            }

        }

        private IHttpActionResult PostAbuse(AbuseInternalRequest request)
        {
            //request.ReporterId = RequestContext.Principal.GetUserIdFromClaim(); //overwrite ReporterId by authorized user 
            //I think we need to use previous variant
            if (!RequestContext.Principal.IsValidUser(request.ReporterId))
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }

            if ((request.AbuseType == null || request.AbuseType.Count() == 0) && (request.SpaceId != null || request.RatingId != null))
            {
                return this.BadRequest(Messages.AbuseTypeRequired);
            }

            try
            {
                var postedAbuse = abuseProvider.PostAbuse(request);
                var location = new Uri(new Uri(Request.RequestUri, RequestContext.VirtualPathRoot),
                       String.Format("api/abuse/{0}", postedAbuse.Id));

                return Created<AbuseInfo>(location, postedAbuse);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return this.BadRequest(ex.Message);
            }

        }

    }
}
