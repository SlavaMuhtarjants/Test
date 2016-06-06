namespace Weezlabs.Storgage.RestApi.Helpers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;


    /// <summary>
    /// Claims principal extension.
    /// </summary>
    public static class ClaimsPrincipalHelper
    {
        /// <summary>
        /// Compare user identifier from token with user identifier from request.
        /// </summary>
        /// <param name="principal">Principal from request..</param>
        /// <param name="userId">User identifier from request.</param>
        /// <returns>True if user identifier from request equal to user identifier from token.</returns>
        public static Boolean IsValidUser(this IPrincipal principal, Guid userId)
        {
            var claimValue = GetUserIdFromClaim(principal, Constants.ClaimsId);
            if (String.IsNullOrWhiteSpace(claimValue))
            {
                return false;
            }

            var result = claimValue == userId.ToString();
            return result;
        }       

        /// <summary>
        /// Returns get user identifier from claim.
        /// </summary>
        /// <param name="principal">Principal.</param>
        /// <returns>User identifier.</returns>
        public static Guid GetUserIdFromClaim(this IPrincipal principal)
        {
            var claimValue = GetUserIdFromClaim(principal, Constants.ClaimsId);
            var result = !String.IsNullOrWhiteSpace(claimValue) ? new Guid(claimValue) : Guid.Empty;
            return result;
        }

        private static String GetUserIdFromClaim(IPrincipal principal, String claimType)
        {
            var claimsPrincipal = principal as ClaimsPrincipal;
            if (claimsPrincipal == null)
            {
                return null;
            }

            var claimsId = claimsPrincipal.Claims.SingleOrDefault(x => x.Type == claimType);
            if (claimsId == null)
            {
                return null;
            }
            return claimsId.Value;
        }
    }
}