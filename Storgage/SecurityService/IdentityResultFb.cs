namespace Weezlabs.Storgage.SecurityService
{
    using Microsoft.AspNet.Identity;
    using System;
using Weezlabs.Storgage.Model;

    public class IdentityResultFb
    {
        public IdentityResultFb(IdentityResult identityResult, User foundUser, Boolean wasCreated)
        {
            IdentityResult = identityResult;
            FoundUser = foundUser;
            WasCreated = wasCreated;
            
        }
        public IdentityResult IdentityResult { get; set; }
        public User FoundUser { get; set; }
        public Boolean WasCreated { get; set; }
    }
}
