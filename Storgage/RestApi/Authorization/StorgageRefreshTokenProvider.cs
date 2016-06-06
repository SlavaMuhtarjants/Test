namespace Weezlabs.Storgage.RestApi.Authorization
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    using Model;
    using SecurityService;

    using Microsoft.Owin.Security.Infrastructure;
    
    /// <summary>
    /// Refresh token provider.
    /// </summary>
    public class StorgageRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private readonly Func<IAuthProvider> authProvider;

        /// <summary>
        /// Create instance 
        /// </summary>
        /// <param name="authProvider">Auth provider.</param>        
        public StorgageRefreshTokenProvider(Func<IAuthProvider> authProvider)
        {
            Contract.Requires(authProvider != null);

            this.authProvider = authProvider;
        }

        /// <summary>
        /// Create new refresh token
        /// </summary>
        /// <param name="context">Authentication token receive context.</param>
        public void Create(AuthenticationTokenCreateContext context)
        {
            Task.Run(() => CreateAsync(context));
        }

        /// <summary>
        /// Create new refresh token
        /// </summary>
        /// <param name="context">Authentication token receive context.</param>
        /// <returns>Task</returns>
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            String userId = context.Ticket.Properties.Dictionary[Constants.ClaimsId];

            String refreshTokenId = Guid.NewGuid().ToString("n");

            var token = new RefreshToken()
            {
                Id = GetHash(refreshTokenId),
                UserId = new Guid(userId),
                Issued = DateTime.UtcNow,
                Expired = DateTime.UtcNow.AddYears(1) // 1 year lifetime for refresh token
            };

            context.Ticket.Properties.IssuedUtc = token.Issued;
            context.Ticket.Properties.ExpiresUtc = token.Expired;

            token.ProtectedTicket = context.SerializeTicket();

            Boolean result = await authProvider().AddRefreshToken(token);

            if (result)
            {
                context.SetToken(refreshTokenId);
            }
        }

        /// <summary>
        /// Receive refresh token
        /// </summary>
        /// <param name="context">Authentication token receive context.</param>
        public void Receive(AuthenticationTokenReceiveContext context)
        {
            Task.Run(() => ReceiveAsync(context));
        }

        /// <summary>
        /// Receive async
        /// </summary>
        /// <param name="context">Authentication token receive context.</param>
        /// <returns>Task</returns>
        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] {"*"});

            String hashedTokenId = GetHash(context.Token);

            RefreshToken refreshToken = await authProvider().FindRefreshToken(hashedTokenId);

            if (refreshToken != null)
            {
                //Get protectedTicket from refreshToken class
                context.DeserializeTicket(refreshToken.ProtectedTicket);
                await authProvider().RemoveRefreshToken(hashedTokenId);
            }
        }

        /// <summary>
        /// Get hash of refresh token.
        /// </summary>
        /// <param name="input">Input strign.</param>
        /// <returns>Hash of input string.</returns>
        public static String GetHash(String input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            Byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            Byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }
    }
}