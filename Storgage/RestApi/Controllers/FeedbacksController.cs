namespace Weezlabs.Storgage.RestApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Results;

    using FileUploadService;
    using Resources;
    using Helpers;

    /// <summary>
    /// Provides methods for feedbacks . 
    /// </summary>
    [RoutePrefix("api/feedbacks")]
    public class FeedbacksController : ApiController
    {
        private readonly IFileUploadProvider fileUploadProvider;

        /// <summary>
        /// Create instance of AdminToolsController.
        /// </summary>
        /// <param name="fileUploadProvider">File upload provider.</param>
        public FeedbacksController(IFileUploadProvider fileUploadProvider)
        {
            Contract.Requires(fileUploadProvider != null);
            
            this.fileUploadProvider = fileUploadProvider;
        }

        /// <summary>
        /// Upload logs
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden.</response>
        /// <response code="415">Unsupported media type.</response>
        /// <returns></returns>
        [HttpPost]
        [Route("contact-us/{userId}/upload-file")]
        [Authorize]
        public async Task<IHttpActionResult> UploadLogs(Guid userId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Messages.AccessDenied);
            }

            HttpRequestMessage request = this.Request;
            if (!request.Content.IsMimeMultipartContent())
            {
                var message = new HttpRequestMessage()
                {
                    Content = new StringContent(Resources.Messages.UnsupportedMediaType)
                };
                return new StatusCodeResult(HttpStatusCode.UnsupportedMediaType, message);
            }

            String pathToServerFolder = String.Format("{0}{1}\\{2}\\", AppDomain.CurrentDomain.BaseDirectory, "MobileLogs", userId);
            Dictionary<String, String> uploadedFiles = null;

            await Request.Content.ReadAsMultipartAsync<MultipartMemoryStreamProvider>(
                new MultipartMemoryStreamProvider()).ContinueWith((task) =>
                {
                    MultipartMemoryStreamProvider provider = task.Result;
                    uploadedFiles = fileUploadProvider.UploadFiles(pathToServerFolder, provider.Contents.AsEnumerable());
                });

            return Json(uploadedFiles);
        }
    }
}
