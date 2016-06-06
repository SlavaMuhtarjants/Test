using Microsoft.Owin;

[assembly: OwinStartup(typeof(Weezlabs.Storgage.RestApi.Startup))]
namespace Weezlabs.Storgage.RestApi
{
    using System;

    using Authorization;
    using Helpers;
    using IoC;
    using SecurityService;

    using UtilService;
    
    using Microsoft.Owin.Security.OAuth;
    using Microsoft.Owin.Security.DataProtection;
    using Microsoft.Owin.Security.DataHandler;
    using Owin;

    /// <summary>
    /// Startup owin.
    /// </summary>
    public class Startup
    {
        internal static IDataProtectionProvider DataProtectionProvider { get; private set; }

        /// <summary>
        /// Makes configuration.
        /// </summary>
        /// <param name="app">App config.</param>
        public void Configuration(IAppBuilder app)
        {
            Int32 accessTokenExpiredTime = GetAppSetting().GetSetting<Int32>("accessTokenExpiredTime");
            
            OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString(AuthTokenOperationDocumentation.TokenEndpoint),
                AccessTokenExpireTimeSpan =
                    // if accessTokenExpiredTime not exists, then default token lifetime is 60 minutes
                    TimeSpan.FromMinutes(accessTokenExpiredTime != default(Int32) ? accessTokenExpiredTime : 60),
                Provider = new StorgageAuthorizationServerProvider(GetAuthProvider),
                RefreshTokenProvider = new StorgageRefreshTokenProvider(GetAuthProvider),
                AccessTokenFormat =
                    new TicketDataFormat(
                        app.CreateDataProtector(typeof (OAuthAuthorizationServerMiddleware).Namespace,
                            "Access_Token",
                            "v1")
                        )
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            DataProtectionProviderWrapper.DataProtectionProvider = app.GetDataProtectionProvider();
        }

        private static IAuthProvider GetAuthProvider()
        {
            return ContainerWrapper.Container.Resolve<IAuthProvider>();
        }

        private static IAppSettings GetAppSetting()
        {
            return ContainerWrapper.Container.Resolve<IAppSettings>();
        }

        /// <summary>
        /// OAuth serverr options.
        /// </summary>
        public static OAuthAuthorizationServerOptions OAuthServerOptions { get; private set; }

    }
}