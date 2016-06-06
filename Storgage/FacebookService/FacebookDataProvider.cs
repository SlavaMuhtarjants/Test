namespace Weezlabs.Storgage.FacebookService
{
    using Facebook;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Web;

    public class FacebookDataProvider : IFacebookDataProvider
    {
        public FacebookDataProvider()
        {
            //before it contained code that is used in GetUserInfo(String facebookToken) now
        }

        /// <summary>
        /// Get basic info of current user
        /// </summary>
        /// <returns></returns>
        public IDictionary<String, Object> GetUserInfo(String facebookToken)
        {
            //Actually it should be in constructor, but injection and etc...
            Contract.Requires(!String.IsNullOrWhiteSpace(facebookToken));
            facebookClient = GetFacebookClient(facebookToken);

            return facebookClient.Get("me", new { fields = "name,id,first_name,last_name,email,picture.width(2400)" }) as IDictionary<String, Object>;
        }

        /// <summary>
        /// Create new facebook client for communication with facebook API
        /// </summary>
        /// <param name="facebookToken">Token, received after user's authentication</param>
        /// <returns>FacebookClient object</returns>
        private static FacebookClient GetFacebookClient(String facebookToken)
        {
            if (String.IsNullOrWhiteSpace(facebookToken))
            {
                throw new ArgumentException("Facebook token is not specified.");
            }

            return new FacebookClient(facebookToken);
        }

        private /*readonly*/ FacebookClient facebookClient;
    }
}