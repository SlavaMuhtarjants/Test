namespace Weezlabs.Storgage.RestApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;

    using Authorization;
    using DataTransferObjects.ViewModels.Account;
    using DataTransferObjects.User;
    using DataTransferObjects.Security;
    using Helpers;
    using Model;
    using Model.Exceptions;
    using Resources;
    using SecurityService;
    using UtilService;

    using Microsoft.AspNet.Identity;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.Infrastructure;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Provides ability to signup/signin, to use Facebook for signup/signin, to change password. 
    /// </summary>
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private readonly IAuthProvider authProvider;
        private readonly UserManager<User, Guid> userManager;
        private readonly IAppSettings appSettings;

        /// <summary>
        /// Create instance of account controller.
        /// </summary>
        /// <param name="authProvider">Authentication provider.</param>
        /// <param name="userManager">User manager of ASP Identity.</param>
        /// <param name="appSettings">Application setting service.</param>
        public AccountController(
            IAuthProvider authProvider,
            UserManager<User, Guid> userManager,
            IAppSettings appSettings
            )
        {
            Contract.Requires(authProvider != null);
            Contract.Requires(userManager != null);
            Contract.Requires(appSettings != null);

            this.authProvider = authProvider;
            this.userManager = userManager;
            this.appSettings = appSettings;
        }


        /// <summary>
        /// Registers new user.
        /// </summary>
        /// <param name="signupInfo">Sign up info.</param>
        /// <returns>Authorization token.</returns>
        /// <response code="201">Created, user was registered.</response>        
        /// <response code="400">Bad parameters. See description to get extra information.</response>              
        [AllowAnonymous]
        [HttpPost]
        [Route("signup")]
        [ResponseType(typeof(UserFullInfo))]
        public async Task<IHttpActionResult> Signup([FromBody] SignupInfo signupInfo)
        {
            if (signupInfo == null)
            {
                return BadRequest(Messages.PostBodyCannotBeNull);
            }

            if (signupInfo.Contact.Email == null || String.IsNullOrEmpty(signupInfo.Contact.Phone))
            {
                return BadRequest(Messages.InternalUserMustHaveEmailAndPhone);
            }

            //I hope that FacebookID will not be ereased if it was not passed (by FbToken) when we EDIT user attributes (not sign up)
            var result = await authProvider.RegisterUser(signupInfo);

            if (result.Succeeded)
            {
                var user = await authProvider.FindUser(signupInfo.Contact.Email, signupInfo.Password);

                var userDto = new UserFullInfo(user);
                return Created<UserFullInfo>(GetLocation(userDto.Id), userDto);
            }
            else
            {
                var errors = String.Join("\n", result.Errors);
                return BadRequest(errors);
            }
        }

        /// <summary>
        /// Registers new user by facebook token.
        /// </summary>
        /// <param name="request">Signup via FB request.</param>
        /// <returns>Authorization token.</returns>
        /// <response code="201">Created, user was registered.</response>         
        /// <response code="400">Bad parameters. See description to get extra information.</response>              
        [AllowAnonymous]
        [HttpPost]
        [Route("signup-fb")]
        [ResponseType(typeof(UserFullInfoFb))]
        public async Task<IHttpActionResult> FbSignup([FromBody] SignupFbRequest request)
        {
            if (request == null)
            {
                return BadRequest(Resources.Messages.PostBodyCannotBeNull);
            }

            //I hope that FacebookID will not be ereased if it was not passed (by FbToken) when we EDIT user attributes (not sign up)
            IdentityResultFb result;
            try
            {
                result = await authProvider.RegisterFacebookUser(request.FbToken);
            }
            catch (BadRequestException ex)
            {
                return this.BadRequest(ex.Message);
            }

            if (result.IdentityResult.Succeeded)
            {
                var userDto = new UserFullInfoFb(result.FoundUser);

                ClaimsIdentity oAuthIdentity = await userManager.CreateIdentityAsync(result.FoundUser,
                    Startup.OAuthServerOptions.AuthenticationType);
                oAuthIdentity.AddClaim(new Claim(RestApi.Constants.ClaimsId, userDto.Id.ToString()));

                var properties = new AuthenticationProperties(new Dictionary<String, String>
                {
                    {
                        RestApi.Constants.ClaimsId, userDto.Id.ToString()
                    }
                })
                {
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTime.UtcNow.Add(Startup.OAuthServerOptions.AccessTokenExpireTimeSpan)
                };

                var ticket = new AuthenticationTicket(oAuthIdentity, properties);
                
                userDto.AccessToken = Startup.OAuthServerOptions.AccessTokenFormat.Protect(ticket);
                userDto.AccessTokenType = ticket.Identity.AuthenticationType;
                userDto.ExpiresAt = ticket.Properties.ExpiresUtc.Value;

                // we should get refresh_token after getting access_token because if we will generate refresh_token at first,
                // we are getting error: access_token will be correct without restriction on AccessTokenExpireTimeSpan
                // because when we getting refresh_token, we set AuthenticationProperties from refresh_token settings
                // so: 1 ticket - 1 token (access or refresh)
                var authTokenCreateContext = new AuthenticationTokenCreateContext(Request.GetOwinContext(),
                                    Startup.OAuthServerOptions.RefreshTokenFormat, ticket);
                await Startup.OAuthServerOptions.RefreshTokenProvider.CreateAsync(authTokenCreateContext);

                userDto.RefreshToken = authTokenCreateContext.Token;

                if (result.WasCreated)
                {
                    return Created<UserFullInfoFb>(GetLocation(userDto.Id), userDto);
                }
                else
                {
                    return Content<UserFullInfoFb>(HttpStatusCode.Found, userDto);
                }
            }
            else
            {
                var errors = String.Join("\n", result.IdentityResult.Errors);
                return BadRequest(errors);
            }
        }

        /// <summary>
        /// Set FacebookId for existing user by facebook token.
        /// </summary>
        /// <param name="request">Signup via FB request.</param>
        /// <returns>Authorization token.</returns>
        /// <response code="200">Ok. User was updated.</response>         
        /// <response code="400">Bad parameters. See description to get extra information.</response>              
        [HttpPut]
        [Route("verify-fb")]
        [ResponseType(typeof(UserFullInfo))]
        [Authorize]
        public async Task<IHttpActionResult> FbSetForUser([FromBody] SignupFbRequest request)
        {
            if (request == null)
            {
                return BadRequest(Resources.Messages.PostBodyCannotBeNull);
            }

            try
            {
                var userId = RequestContext.Principal.GetUserIdFromClaim();
                if (await authProvider.SetFacebookUser(request.FbToken, userId))
                {
                    return await this.GetUserInfo(userId);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (BadRequestException ex)
            {
                return this.BadRequest(ex.Message);
            }

        }

        private String GetLocation(Guid Id)
        {
            return new Uri(new Uri(Request.RequestUri, RequestContext.VirtualPathRoot),
                        String.Format("api/account/{0}", Id)).AbsoluteUri;
        }

        /// <summary>
        /// Updates information about user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="user">User data.</param>
        /// <returns>Updated user.</returns>
        /// <response code="200">Ok. User was updated.</response>
        /// <response code="400">Bad request. See description to get information about details.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not found. User to update was not found.</response>
        [HttpPut]
        [Route("{userId}")]
        [ResponseType(typeof(AccountInfo))]
        [Authorize]
        public IHttpActionResult UpdateUser(Guid userId, [FromBody] AccountInfo user)
        {
            try
            {
                var actorId = RequestContext.Principal.GetUserIdFromClaim();
                var result = authProvider.UpdateUser(userId, user, actorId);
                return Ok<UserFullInfo>(result);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (AccessDeniedException)
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }
            catch (BadRequestException ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Returns info about user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>Returns full info about user.</returns>
        /// <response code="200">Ok.</response>      
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden.</response>                  
        /// <response code="404">Not found. User was not found.</response>
        [HttpGet]
        [Route("{userId}")]
        [ResponseType(typeof(UserFullInfo))]
        [Authorize]
        public async Task<IHttpActionResult> GetUserInfo(Guid userId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Messages.AccessDenied);
            }

            var user = await authProvider.GetUser(userId);

            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserFullInfo(user);

            return Ok<UserFullInfo>(userDto);
        }

        /// <summary>
        /// Uploads new avatar for user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>Http status.</returns>
        /// <response code="200">Ok. Avatar was uploaded.</response>            
        /// <response code="400">Bad request.</response>   
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden.</response>
        /// <response code="404">Not found. User was not found.</response>
        /// <response code="503">Service Unavailable. Problems with deleting or uploading files to Amazon S3 bucket.</response>
        [HttpPost]
        [Route("{userId}/avatar")]
        [Authorize]
        [ResponseType(typeof(UserAvatar))]
        public async Task<IHttpActionResult> UploadAvatar(Guid userId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Messages.AccessDenied);
            }

            UserAvatar avatar = null;

            try
            {
               
                await Request.Content.ReadAsMultipartAsync<MultipartMemoryStreamProvider>(
                    new MultipartMemoryStreamProvider()).ContinueWith((task) =>
                    {
                        MultipartMemoryStreamProvider provider = task.Result;
                        avatar = authProvider.UploadAvatar(userId, provider.Contents.AsEnumerable());
                    });
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException.GetType() == typeof(IOException))
                    return this.BadRequest(ex.InnerException.Message);
            }
            catch (FileLoadException ex)
            {
                return this.ServiceUnavailable(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (BadImageFormatException ex)
            {
                return this.BadRequest(ex.Message);
            }
            catch (ImagesUploadOverflowException ex)
            {
                return this.BadRequest(ex.Message);
            }
            catch (DeleteFileException ex)
            {
                return this.ServiceUnavailable(ex.Message);
            }

            return Ok<UserAvatar>(avatar);
        }

        /// <summary>
        /// Skips avatar for user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>Http status.</returns>
        /// <response code="200">Ok. Avatar was skipped.</response>        
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden.</response>
        /// <response code="404">Not found. User was not found.</response>
        /// <response code="503">Service Unavailable. Problems with deleting files from Amazon S3 bucket.</response>
        [HttpDelete]
        [Route("{userId}/avatar")]
        [Authorize]
        public async Task<IHttpActionResult> SkipAvatar(Guid userId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Messages.AccessDenied);
            }
            try
            {
                await Task.Run(() => authProvider.SkipAvatar(userId));
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (DeleteFileException ex)
            {
                this.ServiceUnavailable(ex.Message);
            }

            return Ok();
        }

        /// <summary>
        /// Changes password.
        /// </summary>
        /// <param name="userId">User identifier.</param>      
        /// <param name="request">Contains information about old and new user passwords.</param>
        /// <returns>New authorization token.</returns>
        /// <response code="200">Ok. Password was updated.</response>
        /// <response code="400">Bad request. See description to get information about details.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden.</response>
        [HttpPost]
        [Route("{userId}/changepassword")]
        [Authorize]
        public IHttpActionResult ChangePassword(Guid userId, [FromBody] ChangePasswordInfo request)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Messages.AccessDenied);
            }

            try
            {
                authProvider.ChangePassword(userId, request);
            }
            catch (BadRequestException ex)
            {
                return this.BadRequest(ex.Message);
            }

            return Ok();
        }

        /// <summary>
        /// Forgot password.
        /// </summary>
        /// <param name="model">ForgotPasswordViewModel.</param>
        /// <returns>New authorization token.</returns>
        /// <response code="200">Ok. Password was updated.</response>
        /// <response code="400">Bad request. See description to get information about details.</response>
        /// <response code="406">Not acceptable. Email wasn't confirmed.</response>
        [HttpPost]
        [Route("forgotpassword")]
        [AllowAnonymous]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            User user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return this.BadRequest(String.Format(Messages.UserNotFoundByEmail, model.Email));
            }
            if (user.IsExternal)
            {
                return this.BadRequest(Messages.ExternalUserCanNotSetPassword);
            }
            if (!(await userManager.IsEmailConfirmedAsync(user.Id)))
            {
                return this.NotAcceptable(Messages.UserEmailNotConfirmed);
            }            

            String code = await userManager.GeneratePasswordResetTokenAsync(user.Id);

            String callbackUrl = this.Url.Link("Default",
                new { Controller = "UserAccount", Action = "ResetPassword", user.Id, code, email = model.Email });

            var emailBody = EmailHelper.CreateEmailBodyForPasswordReset(user.Id, callbackUrl, model.Email,
                appSettings.GetSetting<Int32>("emailTokenExpiredTime"));

            await
                userManager.SendEmailAsync(user.Id, Messages.ResetPasswordEmailSubject, emailBody);

            return Ok();
        }

        /// <summary>
        /// Send email for confirm.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <response code="200">Ok. Email was send.</response>
        /// <response code="304">Email was verified and not modified.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden.</response>
        /// <response code="406">Not acceptable. User haven't email.</response>
        /// <returns>IHttpActionResult</returns>
        [Route("{userId}/confirm/email")]
        [Authorize]
        public async Task<IHttpActionResult> ConfirmEmail(Guid userId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Messages.AccessDenied);
            }

            User user = await userManager.FindByIdAsync(userId);
            if (String.IsNullOrWhiteSpace(user.Email))
            {
                return this.NotAcceptable(Messages.UserHaveNoEmail);
            }
            if (user.EmailVerificationStatus.ToEnum() == Model.Enums.EmailVerificationStatus.Verified)
            {
                return this.NotModified(Messages.UserEmailActuallyConfirmed);
            }

            String code = await userManager.GenerateEmailConfirmationTokenAsync(userId);

            String callbackUrl = this.Url.Link("Default",
                new { Controller = "UserAccount", Action = "ConfirmEmail", userId, code });

            var emailBody = EmailHelper.CreateEmailBodyForEmailConfirm(userId, callbackUrl, user.Email,
                appSettings.GetSetting<Int32>("emailTokenExpiredTime"));

            await authProvider.SendEmail(user.Id, Messages.ConfirmEmailSubject, emailBody);

            return Ok();
        }

        /// <summary>
        /// Send sms for confirm phone number.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns></returns>
        /// <response code="200">Ok. Sms was send.</response>
        /// <response code="304">Phone was verified and not modified.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden.</response>
        /// <response code="406">Not acceptable. User haven't phone.</response>
        /// <returns>IHttpActionResult</returns>
        [Route("{userId}/confirm/phone")]
        [Authorize]
        public async Task<IHttpActionResult> ConfirmPhone(Guid userId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Messages.AccessDenied);
            }

            User user = await userManager.FindByIdAsync(userId);
            if (String.IsNullOrWhiteSpace(user.Phone))
            {
                return this.NotAcceptable(Messages.UserHaveNoPhone);
            }
            if (user.PhoneVerificationStatus.ToEnum() == Model.Enums.PhoneVerificationStatus.Verified)
            {
                return this.NotModified(Messages.UserPhoneActuallyConfirmed);
            }

            IdentityResult sendResult = await authProvider.SendPhoneConfirmation(userId);

            if (!sendResult.Succeeded)
            {
                // temporary! will be delete when we will have prod twilio acc
                var code = new Regex(@"(?<=is: )\d{6}").Match(String.Join("\n", sendResult.Errors)).Value;
                if (!String.IsNullOrWhiteSpace(code))
                {
                    Dictionary<String, Object> error = new Dictionary<String, Object>
                    {
                        {"Message", String.Join("\n", sendResult.Errors)},
                        {"SmsCode", code}
                    };

                    return Json(error);
                }
                // end of temp solution

                return this.BadRequest(String.Join("\n", sendResult.Errors));
            }

            return Ok();
        }

        /// <summary>
        /// Verif
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="code">Token.</param>
        /// <response code="200">Ok. Sms was send.</response>
        /// <response code="304">Phone was verified and not modified.</response>
        /// <response code="400">Bad request. See description to get information about details.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden.</response>
        /// <response code="406">Not acceptable. User haven't phone.</response>
        /// <returns>IHttpActionResult</returns>
        [HttpPut]
        [Route("{userId}/confirm/phone")]
        [Authorize]
        public async Task<IHttpActionResult> ConfirmPhone(Guid userId, String code)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Messages.AccessDenied);
            }

            User user = await userManager.FindByIdAsync(userId);
            // user can delete phone before enter code
            if (String.IsNullOrWhiteSpace(user.Phone))
            {
                return this.NotAcceptable(Messages.UserHaveNoPhone);
            }
            if (user.PhoneVerificationStatus.ToEnum() == Model.Enums.PhoneVerificationStatus.Verified)
            {
                return this.NotModified(Messages.UserPhoneActuallyConfirmed);
            }

            IdentityResult result = await authProvider.ConfirmPhone(userId, code);
            if (result.Succeeded)
            {
                return Ok();
            }

            String errors = String.Join("\n", result.Errors);

            return BadRequest(errors);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">Form data collection.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("refreshtoken")]
        public async Task<IHttpActionResult> RefreshToken([FromBody]FormDataCollection data)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(new Uri(Request.RequestUri, RequestContext.VirtualPathRoot), "api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                var config = new HttpConfiguration();
                WebApiConfig.Register(config);
                var server = new HttpServer(config);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<String, String>("grant_type", "refresh_token"),
                    new KeyValuePair<String, String>("refresh_token", data["refresh_token"]),
                });

                HttpResponseMessage result = await client.PostAsync("account/signin", content);

                String resultContent = result.Content.ReadAsStringAsync().Result;

                if (result.StatusCode == HttpStatusCode.BadRequest)
                {
                    return this.BadRequest("Invalid grant");
                }

                return Ok(JObject.Parse(resultContent));
            }
        }

        /// <summary>
        /// Sign out.
        /// </summary>
        /// <param name="data">Form data collection.</param>
        /// <response code="200">Ok. Refresh token was removed.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden.</response>
        /// <response code="404">Not found. Refresh token was not found.</response>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("signout")]
        [Authorize]
        public async Task<IHttpActionResult> SignOut([FromBody]FormDataCollection data)
        {
            var actorId = RequestContext.Principal.GetUserIdFromClaim();

            try
            {
                await authProvider.RemoveRefreshToken(StorgageRefreshTokenProvider.GetHash(data["refresh_token"]), actorId);
            }
            catch (NotFoundException)
            {
                return this.NotFound(String.Format(Resources.Messages.RefreshTokenNotFound, data["refresh_token"]));
            }
            catch (AccessDeniedException)
            {
                return this.Forbidden(String.Format(Resources.Messages.InvalidRefreshTokenUser,
                    actorId, data["refresh_token"]));
            }

            return Ok();
        }
    }
}
