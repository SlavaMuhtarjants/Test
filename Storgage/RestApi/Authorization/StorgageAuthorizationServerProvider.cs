namespace Weezlabs.Storgage.RestApi
{   
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;   
    using System.Threading.Tasks;
    using System.Security.Claims;

    using Model;
    using SecurityService;

    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.OAuth;
    using Weezlabs.Storgage.Model.Exceptions;

    /// <summary>
    /// Authorization server provider.
    /// </summary>
    public class StorgageAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly Func<IAuthProvider> authProviderFactory;
        
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="authProviderFactory">Auth provider delegate. Func IAuthProvider </param>
        public StorgageAuthorizationServerProvider(Func<IAuthProvider> authProviderFactory)
        {
            Contract.Requires(authProviderFactory != null);

            this.authProviderFactory = authProviderFactory;
        }

        /// <summary>
        /// Validates authentication context.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <returns></returns>
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            await Task.Run(() => context.Validated());
        }

        /// <summary>
        /// Checks client's credentials.
        /// </summary>
        /// <param name="context">Authentication context.</param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            User user;
            try
            {
                user = await authProviderFactory().FindUser(context.UserName, context.Password);
            }
            catch (AccessDeniedException e)
            {
                context.SetError("invalid_grant", e.Message);
                return;
            }

            if (user == null)
            {
                context.SetError("invalid_grant", Resources.Messages.InvalidUserCredentials);
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(Constants.ClaimsId, user.Id.ToString()));

            var props = new AuthenticationProperties(new Dictionary<String, String>
            {
                {
                    Constants.ClaimsId, user.Id.ToString()
                }
            });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);
        }

        /// <summary>
        /// Overrides grant refresh token.
        /// </summary>
        /// <param name="context">OAuth grant refresh token context.</param>
        /// <returns></returns>
        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<Object>(null);
        }

        /// <summary>
        /// Overrides output token. We want to add additional information to token.
        /// </summary>
        /// <param name="context">OAuth token endpoint context.</param>
        /// <returns></returns>
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            var userId = context.Properties.Dictionary[Constants.ClaimsId];

            if (userId != null)
            {
                context.AdditionalResponseParameters.Add("User Id", userId);
            }
            return Task.FromResult<Object>(null);
        }
    }
}