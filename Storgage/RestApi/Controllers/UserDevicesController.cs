namespace Weezlabs.Storgage.RestApi.Controllers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Web.Http;
    using System.Web.Http.Description;

    using DataTransferObjects.User;
    using Helpers;
    using Model.Exceptions;
    using UserService;
    

    /// <summary>
    /// User devices controller.
    /// </summary>
    [RoutePrefix("api")]
    public class UserDevicesController : ApiController
    {
        private readonly IUserDeviceProvider userDeviceProvider;

        /// <summary>
        /// Creates user devices controller.
        /// </summary>
        /// <param name="userDeviceProvider">User devices controller.</param>
        public UserDevicesController(IUserDeviceProvider userDeviceProvider)
        {
            Contract.Requires(userDeviceProvider != null);

            this.userDeviceProvider = userDeviceProvider;
        }

        /// <summary>
        /// Adds new device token for user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="userDevice"></param>
        /// <returns>Created user device info.</returns>
        /// <response code="200">Device token was added.</response>
        /// <response code="401">Not authorized.</response>
        /// <response code="403">Forbidden.</response>
        /// <response code="404">User not found.</response>
        [HttpPost]
        [Authorize]
        [Route("users/{userId}/devices")]
        [ResponseType(typeof(UserDeviceInfo))]
        public IHttpActionResult AddDeviceToken(Guid userId, [FromBody] UserDeviceInfo userDevice)
        {
            if (userDevice == null)
            {
                return BadRequest(Resources.Messages.PostBodyCannotBeNull);
            }

            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }

            try
            {
                userDeviceProvider.AddDeviceToken(userId, userDevice);
                return Ok<UserDeviceInfo>(userDevice);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (NotUniqueException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Removes device token.
        /// </summary>
        /// <param name="deviceToken">Device token.</param>
        /// <returns>Http status.</returns>
        /// <response code="200">Ok. Device status was removed.</response>
        /// <response code="401">Not authorized.</response>
        /// <response code="403">Forbidden.</response>
        /// <response code="404">Device token not found.</response>
        [HttpDelete]
        [Route("devices/{deviceToken}")]
        [Authorize]
        public IHttpActionResult DeleteDeviceToken(String deviceToken)
        {
            if (String.IsNullOrWhiteSpace(deviceToken))
            {
                return BadRequest(Resources.Messages.DeviceTokenEmpty);
            }

            try
            {
                var userId = RequestContext.Principal.GetUserIdFromClaim();
                userDeviceProvider.RemoveDeviceToken(deviceToken, userId);
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
        /// Returns list of user device tokens.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>List of user's device tokens.</returns>
        /// <response code="200">Ok.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden.</response>
        /// <response code="404">User not found.</response>
        [HttpGet]
        [Route("users/{userId}/devices")]
        [Authorize]
        [ResponseType(typeof(IEnumerable<UserDeviceInfo>))]
        public IHttpActionResult GetDevices(Guid userId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }

            try
            {
                var result = userDeviceProvider.GetUserDevices(userId).Select(x =>
                new UserDeviceInfo
                {
                    DeviceToken = x.PushNotificationToken,
                    MobileEndpointType = Model.Enums.MobileEndpointType.Apns
                });
                return Ok<IEnumerable<UserDeviceInfo>>(result);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Enables push notification.
        /// </summary>
        /// <param name="deviceToken">Device token.</param>
        /// <returns>Device token info.</returns>
        /// <response code="200">Ok. Push notification was enabled.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Device token not found.</response>       
        [HttpPut]
        [Route("devices/{deviceToken}/enable-push-notifications")]
        [Authorize]
        [ResponseType(typeof(UserDeviceInfo))]
        public IHttpActionResult EnablePushNotification(String deviceToken)
        {
            try
            {
                var actorId = RequestContext.Principal.GetUserIdFromClaim();
                userDeviceProvider.EnablePushNotification(deviceToken, actorId, true);
                return Ok<UserDeviceInfo>(new UserDeviceInfo
                {
                    DeviceToken = deviceToken,
                    IsPushNotificationEnabled = true,
                    MobileEndpointType = Model.Enums.MobileEndpointType.Apns
                });
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
        /// Disables push notification.
        /// </summary>
        /// <param name="deviceToken">Device token.</param>
        /// <returns>Device token info.</returns>
        /// <response code="200">Ok. Push notification was enabled.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Device token not found.</response>       
        [HttpPut]
        [Route("devices/{deviceToken}/disable-push-notifications")]
        [Authorize]
        [ResponseType(typeof(UserDeviceInfo))]
        public IHttpActionResult DisablePushNotification(String deviceToken)
        {
            try
            {
                var actorId = RequestContext.Principal.GetUserIdFromClaim();
                userDeviceProvider.EnablePushNotification(deviceToken, actorId, false);
                return Ok<UserDeviceInfo>(new UserDeviceInfo
                {
                    DeviceToken = deviceToken,
                    IsPushNotificationEnabled = false,
                    MobileEndpointType = Model.Enums.MobileEndpointType.Apns
                });
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
