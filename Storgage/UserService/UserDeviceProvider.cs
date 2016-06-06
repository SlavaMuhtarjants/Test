namespace Weezlabs.Storgage.UserService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using DataLayer;
    using DataLayer.Users;
    using DataTransferObjects.User;
    using MessagePublisher;
    using Model;
    using Model.Exceptions;
    using UtilService;

    using EntityFramework.Extensions;

    /// <summary>
    /// User device repository.
    /// </summary>
    public class UserDeviceProvider : IUserDeviceProvider
    {
        private readonly IUserDeviceRepository userDeviceRepository;
        private readonly IUserReadonlyRepository userRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMobileEndpointProvider mobileEndpointProvider;
        private readonly IAppSettings appSettings;

        /// <summary>
        /// Creates instance of user device provider.
        /// </summary>
        /// <param name="userDeviceRepository">User device repository.</param>
        /// <param name="userRepository">User repository.</param>
        /// <param name="mobileEndpointProvider">Mobile endpoiint provider.</param>
        /// <param name="appSettings">Application settings.</param>
        /// <param name="unitOfWork">Unit of work.</param>
        public UserDeviceProvider(IUserDeviceRepository userDeviceRepository,
            IUserReadonlyRepository userRepository,
            IMobileEndpointProvider mobileEndpointProvider,
            IAppSettings appSettings,
            IUnitOfWork unitOfWork)
        {
            Contract.Requires(userDeviceRepository != null);
            Contract.Requires(userRepository != null);
            Contract.Requires(mobileEndpointProvider != null);
            Contract.Requires(appSettings != null);
            Contract.Requires(unitOfWork != null);

            this.userDeviceRepository = userDeviceRepository;
            this.userRepository = userRepository;
            this.mobileEndpointProvider = mobileEndpointProvider;
            this.appSettings = appSettings;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Adds new device token for user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="userDeviceInfo">User device.</param>
        public void AddDeviceToken(Guid userId, UserDeviceInfo userDeviceInfo)
        {
            var userDevice = GetUserDevice(userDeviceInfo.DeviceToken);
            if (userDevice != null)
            {
                throw new NotUniqueException(String.Format(Resources.Messages.DeviceTokenAlwaysExist, userDeviceInfo.DeviceToken));
            }

            try
            {
                var platformEndpoint = appSettings.GetSetting<String>(userDeviceInfo.MobileEndpointType.ToString());
                var mobileEnpointInfo = mobileEndpointProvider.CreateMobileEndpoint(userDeviceInfo.DeviceToken, platformEndpoint);
                
                var newUserDevice = new UserDevice
                {
                    PushNotificationToken = userDeviceInfo.DeviceToken,
                    UserId = userId,
                    MobileEndpoint = mobileEnpointInfo.MobileEndpoint,
                    SubscriptionEndpoint = mobileEnpointInfo.SubscriptionEndpoint,
                    TopicEndpoint = mobileEnpointInfo.TopicEndpoint,
                    IsPushNotificationEnabled = userDeviceInfo.IsPushNotificationEnabled,                    
                };
                userDeviceRepository.Add(newUserDevice);
                unitOfWork.CommitChanges();
            }
            catch(DataException ex)
            {
                if (ex.Message.Contains("fk_UserDevice_UserId"))
                {
                    throw new NotFoundException(String.Format(Resources.Messages.UserNotFound, userId));
                }
                throw;
            }
        }

        /// <summary>
        /// Returns Enumerable of device tokens for user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>Enumrable of device tokens.</returns>
        public IEnumerable<UserDevice> GetUserDevices(Guid userId)
        {
            var user = userRepository.GetAll().Include(x => x.UserDevices).SingleOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new NotFoundException(String.Format(Resources.Messages.UserNotFound, userId));
            }
            var result = user.UserDevices.ToList();
            return result;
        }

        /// <summary>
        /// Removes device token.
        /// </summary>       
        /// <param name="deviceToken">Device token</param>
        /// <param name="userId">User identifier.</param>
        /// <returns>True if device token was removed.</returns>
        public void RemoveDeviceToken(String deviceToken, Guid userId)
        {
            var userDevice = GetUserDevice(deviceToken);
            if (userDevice == null)
            {
                throw new NotFoundException(String.Format(Resources.Messages.DeviceTokenNotFound, deviceToken));
            }
            if (userId != userDevice.UserId)
            {
                throw new AccessDeniedException();
            }

            mobileEndpointProvider.RemoveMobileEndpoint(new MobileEndpointInfo
            {
                MobileEndpoint = userDevice.MobileEndpoint,
                TopicEndpoint = userDevice.TopicEndpoint,
                SubscriptionEndpoint = userDevice.SubscriptionEndpoint
            });

            userDeviceRepository.Delete(userDevice);
            unitOfWork.CommitChanges();            
        }

        /// <summary>
        /// Enables device token for push notifications.
        /// </summary>
        /// <param name="deviceToken">Device token.</param>
        /// <param name="userId">User identifier.</param>
        /// <param name="isEnable">True if push notification is enabled.</param>
        public void EnablePushNotification(String deviceToken, Guid userId, Boolean isEnable)
        {
            var userDevice = GetUserDevice(deviceToken);
            if (userDevice == null)
            {
                throw new NotFoundException(String.Format(Resources.Messages.DeviceTokenNotFound, deviceToken));
            }
            if (userId != userDevice.UserId)
            {
                throw new AccessDeniedException();
            }

            userDevice.IsPushNotificationEnabled = isEnable;
            userDeviceRepository.Update(userDevice);
            unitOfWork.CommitChanges();
        }

        /// <summary>
        /// Invalidates mobile endpoints.
        /// </summary>
        /// <param name="userDevices">User devices to invalidate.</param>
        public void InvalidateUserDevices(IEnumerable<Guid> userDevices)
        {
            if (userDevices.Count() > 0)
            {
                userDeviceRepository.GetAll().Where(x => userDevices.Contains(x.Id)).Update(x => new UserDevice { IsPushNotificationEnabled = false });
            }
        }

        /// <summary>
        /// Get user device
        /// </summary>
        /// <param name="deviceToken">Device token.</param>
        /// <returns>UserDevice.</returns>
        private UserDevice GetUserDevice(String deviceToken)
        {
            var userDevice = userDeviceRepository.GetAll().SingleOrDefault(x => x.PushNotificationToken == deviceToken);
            return userDevice;
        }
    }
}
