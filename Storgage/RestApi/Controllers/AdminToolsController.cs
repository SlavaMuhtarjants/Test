#if !PROD

namespace Weezlabs.Storgage.RestApi.Controllers
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using System.Web.Http;

    using DataTransferObjects.Misc;
    using UserService;

    /// <summary>
    /// Provides methods for testing . 
    /// </summary>
    [RoutePrefix("api/admin-tools")]
    public class AdminToolsController : ApiController
    {
        private readonly IUserProvider<Guid> userProvider;

        /// <summary>
        /// Create instance of AdminToolsController.
        /// </summary>
        /// <param name="userProvider">user provider.</param>
        public AdminToolsController(
            IUserProvider<Guid> userProvider             
            )
        {
            Contract.Requires(userProvider != null);
            
            this.userProvider = userProvider;
        }

        /// <summary>
        /// Deletes users PHYSICALLY.
        /// </summary>
        /// <param name="userXml">It is String that contains XML with parameters of users that must be deleted physically
        /// it may contain UserId, Email, FacebookId and has format
        /// You can check parameter format in te comments of stored procedure with name "tst.spUserDel" in the database
        /// <x>
        /// <i id="82E38645-B9EB-4C72-A2ED-F59F4A1D9543"/>
        /// <i id="00A23CCD-0B26-464E-9381-63D137CAC808"/>
        /// <i email="fmarshall2b@cpanel.net"/>
        /// <i facebookid="1599004680424822"/>
        ///</x>
        /// </param>
        /// <returns>Boolean</returns>
        /// <response code="200">Ok. User was deleted with child entities.</response>
        /// <response code="400">Bad request. See description to get information about details.</response>
        [HttpPost]
        [Route("deleteusers")]
        [AllowAnonymous]
        public IHttpActionResult DeleteUsers([FromBody] StringValue userXml)
        {
            try
            {
                return Ok<Boolean>(userProvider.UserDel(userXml.Value));
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }

            
        }
    }
}
#endif 