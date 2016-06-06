namespace Weezlabs.Storgage.MessagePublisher.Sns
{
    using System;
    using System.Diagnostics.Contracts;

    using Model.Exceptions;
    using UtilService;

    using Amazon.SimpleNotificationService;
    using Amazon.SimpleNotificationService.Model;

    /// <summary>
    /// SNS Message publisher.
    /// </summary>
    public class SnsMessagePublisher : IMessagePublisher
    {         
        private readonly IAmazonSimpleNotificationService snsPublisher;

        /// <summary>
        /// Creates instance of message pusblisher.
        /// </summary>
        /// <param name="appSettings">Application settings provider.</param>
        public SnsMessagePublisher(IAppSettings appSettings)
        {
            Contract.Requires(appSettings != null);

            snsPublisher = appSettings.CreateSnsPublisher();            
        }

        /// <summary>
        /// Sends message to publisher.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="publisherEndpoint">Publisher endpoint.</param>
        public void SendMessage(String message, String publisherEndpoint)
        {
            try
            {
                var request = new PublishRequest { Message = message, TopicArn = publisherEndpoint, MessageStructure = "json" };
                var response = snsPublisher.Publish(request);
                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    var errorMessage = String.Format(Resources.Messages.SendMessageError, response.HttpStatusCode);
                    throw new CommunicationException(errorMessage);
                }
            }         
            catch (Exception ex)
            {
                throw new CommunicationException(Resources.Messages.SendMessageException, ex);
            }
        }
    }
}
