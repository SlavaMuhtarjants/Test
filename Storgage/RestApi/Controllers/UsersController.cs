namespace Weezlabs.Storgage.RestApi.Controllers
{
    using DataTransferObjects.User;
    using Helpers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Web.Http;
    using System.Web.Http.Description;
    using UserService;
    using Weezlabs.Storgage.Bookings;
    using Weezlabs.Storgage.DataTransferObjects.Booking;    

    /// <summary>
    /// Provides ability to work with users.
    /// </summary>
    [RoutePrefix("api/users/{userId}")]
    public class UsersController : ApiController
    {

        private readonly IUserProvider<Guid> userProvider;
        private readonly IBookingProvider bookingProvider;

        /// <summary>
        /// Creates Users controller.
        /// </summary>
        /// <param name="userProvider">User Provider.</param>
        /// <param name="bookingProvider">Booking provider.</param>
        public UsersController(
            IUserProvider<Guid> userProvider,
            IBookingProvider bookingProvider)
        {
            Contract.Requires(userProvider != null);
            Contract.Requires(bookingProvider != null);

            this.userProvider = userProvider;
            this.bookingProvider = bookingProvider;
        }

        /// <summary>
        /// Returns info about user by identifier.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>Returns info about user.</returns>
        /// <response code="200">Ok.</response>                        
        /// <response code="404">Not found. User was not found.</response>
        [HttpGet]
        [Route("shortinfo")]
        [ResponseType(typeof(UserInfo))]
        [AllowAnonymous]
        public IHttpActionResult GetUserShortInfo(Guid userId)
        {
            var result = userProvider.GetUserShortInfo(userId);
            if (result == null)
            {
                return NotFound();
            }

            return Ok<UserInfo>(result);
        }

        /// <summary>
        /// Returns chats that have approved offers for specifc renter user.
        /// </summary>
        /// <param name="userId">Identifier of User, actually method ignore it and uses UserId from token but Swagger doesn't work normally without this param</param>
        /// <returns>IEnumerable of spaces.</returns>
        /// <response code="200">Space list was returned</response>
        /// <response code="401">User is unauthorized.</response>
        /// <response code="404">User was not found</response>
        [HttpGet]
        [Route("my-bookings")]
        [ResponseType(typeof(List<MyBookingResponse>))]
        [Authorize]
        public IHttpActionResult GetMyBookings(Guid userId)
        {
            //I don't want to check, I will use correct value
            //this row will ve removed if logic is changed
            userId = RequestContext.Principal.GetUserIdFromClaim();

            // actually I use UserId from Token but Swagger doesn't work when 
            // "[Route("get-my-bookings")]"
            // and method doesn't have input parameters
            var result = bookingProvider.GetMyBookings(userId);

            if (result == null)
            {
                //I don't think that it's possible with [Authorize] 
                return this.NotFound(String.Format(Resources.Messages.UserNotFound, userId));
            }

            return Ok<List<MyBookingResponse>>(result);
        }
    }
}
