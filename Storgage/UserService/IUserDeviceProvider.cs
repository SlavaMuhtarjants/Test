namespace Weezlabs.Storgage.UserService
{
    using System;
    using System.Collections.Generic;

    using DataTransferObjects.User;
    using Model;

    /// <summary>
    /// Interface for user device tokens.
    /// </summary>
    public interface IUserDeviceProvider
    {
        /// <summary>
        /// Adds new device token for user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="userDeviceInfo">User device info.</param>
        void AddDeviceToken(Guid userId, UserDeviceInfo userDeviceInfo);

        /// <summary>
        /// Returns Enumerable of device tokens for user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>Enumrable of device tokens.</returns>
        IEnumerable<UserDevice> GetUserDevices(Guid userId);

        /// <summary>
        /// Removes device token.
        /// </summary>                
        /// <param name="deviceToken">Device token</param>
        /// <param name="userId">User identifier.</param>
        /// <returns>True if device token was removed.</returns>
        void RemoveDeviceToken(String deviceToken, Guid userId);

        /// <summary>
        /// Enables device token for push notifications.
        /// </summary>
        /// <param name="deviceToken">Device token.</param>
        /// <param name="userId">User identifier.</param>
        /// <param name="isEnable">True if push notification is enabled.</param>
        void EnablePushNotification(String deviceToken, Guid userId, Boolean isEnable);

        /// <summary>
        /// Invalidates mobile endpoints.
        /// </summary>
        /// <param name="userDevices">User devices to invalidate.</param>
        void InvalidateUserDevices(IEnumerable<Guid> userDevices);
    }
}
