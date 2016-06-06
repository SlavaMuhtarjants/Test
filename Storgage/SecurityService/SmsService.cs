namespace Weezlabs.Storgage.SecurityService
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    
    using Model.Exceptions;
    using UtilService;

    using Microsoft.AspNet.Identity;
    using Twilio;

    /// <summary>
    /// Sms service
    /// </summary>
    public class SmsService : IIdentityMessageService
    {
        private readonly IAppSettings appSetting;

        /// <summary>
        /// Create instance of Sms service
        /// </summary>
        /// <param name="appSetting"></param>
        public SmsService(IAppSettings appSetting)
        {
            Contract.Requires(appSetting != null);

            this.appSetting = appSetting;
        }

        /// <summary>
        /// Send sms async
        /// </summary>
        /// <param name="message">Message.</param>
        /// <returns>Task</returns>
        public Task SendAsync(IdentityMessage message)
        {
            String accountSid = appSetting.GetSetting<String>("TwilioAccountSID");
            String authToken = appSetting.GetSetting<String>("TwilioAuthToken");
            String twilioPhoneNumber = appSetting.GetSetting<String>("TwilioPhoneNumber");

            var twilio = new TwilioRestClient(accountSid, authToken);
            Message result = twilio.SendMessage(twilioPhoneNumber, message.Destination, message.Body);

            if (result.Body == null)
            {
                throw new PostMessageException(result.RestException.Message);
            }

            //note: throw error for testing:
            throw new BadRequestException(result.Body);

            return Task.FromResult(0);
        }
    }
}
