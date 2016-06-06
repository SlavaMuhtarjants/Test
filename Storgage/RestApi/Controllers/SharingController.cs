namespace Weezlabs.Storgage.RestApi.Controllers
{
    using System;
    using System.Web.Mvc;

    /// <summary>
    /// Responses for pages of the sharing functionality
    /// </summary>
    public class SharingController : Controller
    {
        /// <summary>
        /// Displays a page prompting the user to run the app or to install it
        /// </summary>
        /// <param name="spaceId">space identifier</param>
        /// <returns>HTML page</returns>
        [HttpGet]
        [Route("share/spaces/{spaceId:guid}")]
        public ActionResult ShareSpace(Guid spaceId)
        {
            return View(spaceId);
        }
    }
}