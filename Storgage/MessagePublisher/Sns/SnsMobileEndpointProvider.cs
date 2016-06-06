namespace Weezlabs.Storgage.MessagePublisher.Sns
{
    using System;
    using System.Diagnostics.Contracts;

    using Model.Exceptions;
    using UtilService;

    using Amazon.SimpleNotificationService;
    using Amazon.SimpleNotificationService.Model;

    /// <summary>
    /// SNS mobile endpoint provider.
    /// </summary>
    public class SnsMobileEndpointProvider : IMobileEndpointProvider
    {
        private readonly IAmazonSimpleNotificationService snsPublisher;
        private readonly IAppSettings appSettings;

        /// <summary>
        /// Creates SNS mobile endpoint providers.
        /// </summary>
        /// <param name="appSettings">Application settings.</param>
        public SnsMobileEndpointProvider(IAppSettings appSettings)
        {
            Contract.Requires(appSettings != null);
            this.appSettings = appSettings;

            snsPublisher = appSettings.CreateSnsPublisher();
        }

        /// <summary>
        /// Creates mobile endpoint.
        /// </summary>
        /// <param name="deviceToken">Device token.</param>
        /// <param name="applicationEndpoint">Application endpoint.</param>
        /// <returns>Created mobile endpoint.</returns>
        public MobileEndpointInfo CreateMobileEndpoint(String deviceToken, String applicationEndpoint)
        {
            try
            {
                var createEnpointRequest = new CreatePlatformEndpointRequest
                {
                    PlatformApplicationArn = applicationEndpoint,
                    Token = deviceToken
                };

                var createEndpointResponse = snsPublisher.CreatePlatformEndpoint(createEnpointRequest);

                if (createEndpointResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    var errorMessage = 
                        String.Format(Resources.Messages.CreateMobileEndpointError, 
                            createEndpointResponse.HttpStatusCode);
                    throw new CommunicationException(errorMessage);
                }

                var topicName = SnsTopicNameProvider.GetName(appSettings, deviceToken);
                var createTopicResponse = snsPublisher.CreateTopic(topicName);

                if (createTopicResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    var errorMessage = String.Format(Resources.Messages.CreateTopicError,
                        createTopicResponse.HttpStatusCode);
                    throw new CommunicationException(errorMessage);
                }

                var subscribeMobileEndpointRequest = new SubscribeRequest
                {
                    Endpoint = createEndpointResponse.EndpointArn,
                    TopicArn = createTopicResponse.TopicArn,
                    Protocol = "application"
                };

                var subscribeMobileEndpointResponse = snsPublisher.Subscribe(subscribeMobileEndpointRequest);
                if (subscribeMobileEndpointResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    var errorMessage =
                        String.Format(Resources.Messages.SubscribeMobileEndpointError, subscribeMobileEndpointResponse.HttpStatusCode);
                    throw new CommunicationException(errorMessage);
                }

                return new MobileEndpointInfo
                {
                    MobileEndpoint = createEndpointResponse.EndpointArn,
                    TopicEndpoint = createTopicResponse.TopicArn,
                    SubscriptionEndpoint = subscribeMobileEndpointResponse.SubscriptionArn
                };
            }
            catch (Exception ex)
            {
                throw new CommunicationException(Resources.Messages.CreateMobileEndpointException, ex);
            }
        }

        /// <summary>
        /// Removes mobile endpoint.
        /// </summary>
        /// <param name="mobileEndpointInfo">Mobile  endpoint to remove.</param>
        public void RemoveMobileEndpoint(MobileEndpointInfo mobileEndpointInfo)
        {
            try
            {
                var deleteTopicResponse = snsPublisher.DeleteTopic(mobileEndpointInfo.TopicEndpoint);
                if (deleteTopicResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    var errorMessage = String.Format(Resources.Messages.RemoveTopicError,
                        deleteTopicResponse.HttpStatusCode);
                    throw new CommunicationException(errorMessage);
                }

                var removeSubscriptionResponse = snsPublisher.Unsubscribe(mobileEndpointInfo.SubscriptionEndpoint);
                if (removeSubscriptionResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    var errorMessage = String.Format(Resources.Messages.UnsubscribeMobileEndointError,
                        removeSubscriptionResponse.HttpStatusCode);
                    throw new CommunicationException(errorMessage);
                }

                var removeEndpointRequest = new DeleteEndpointRequest
                {
                    EndpointArn = mobileEndpointInfo.MobileEndpoint
                };

                var removeEndpointResponse = snsPublisher.DeleteEndpoint(removeEndpointRequest);
                if (removeEndpointResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    var errorMessage = 
                        String.Format(Resources.Messages.RemoveMobileEndpointError, 
                            removeEndpointResponse.HttpStatusCode);
                    throw new CommunicationException(errorMessage);
                }
            }
            catch (Exception ex)
            {
                throw new CommunicationException(Resources.Messages.RemoveMobileEnpointException, ex);
            }
        }

        /// <summary>
        /// Checks that mobile endpoint is enabled.
        /// </summary>
        /// <param name="mobileEndpoint">Mobile endpoint.</param>
        /// <returns>True if mobile endpoint is enabled.</returns>
        public Boolean IsMobileEndpointEnabled(String mobileEndpoint)
        {
            try
            {
                var request = new GetEndpointAttributesRequest { EndpointArn = mobileEndpoint };
                var response = snsPublisher.GetEndpointAttributes(request);
                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    var errorMessage =
                        String.Format(Resources.Messages.GetEndpointAttributesError,
                            response.HttpStatusCode);
                    throw new CommunicationException(errorMessage);
                }

                var isEnabled = response.Attributes["Enabled"] == "true";
                return isEnabled;
            }
            catch (Exception ex)
            {
                throw new CommunicationException(Resources.Messages.GetEndpointAttributesException, ex);
            }
        }
    }
}
