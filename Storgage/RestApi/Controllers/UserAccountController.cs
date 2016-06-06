namespace Weezlabs.Storgage.RestApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    
    using DataTransferObjects.ViewModels.Account;
    using Model;
    using SecurityService;

    using Microsoft.AspNet.Identity;
    using Newtonsoft.Json;

    /// <summary>
    /// User account web controller.
    /// </summary>
    public class UserAccountController : Controller
    {
        private readonly IAuthProvider authProvider;
        private readonly UserManager<User, Guid> userManager;

        /// <summary>
        /// Create instance of user account controller.
        /// </summary>
        /// <param name="authProvider">Auth provider.</param>
        /// <param name="userManager">User manager.</param>
        public UserAccountController(IAuthProvider authProvider, UserManager<User, Guid> userManager)
        {
            Contract.Requires(authProvider != null);
            Contract.Requires(userManager != null);

            this.authProvider = authProvider;
            this.userManager = userManager;
        }

        /// <summary>
        /// Confirm email.
        /// </summary>
        /// <param name="userId">User identirier.</param>
        /// <param name="code">Token.</param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(Guid userId, String code)
        {
            if (userId == default(Guid) || code == null)
            {
                return View("Error");
            }
            User user = userManager.FindByIdAsync(userId).Result;
            if (user.EmailVerificationStatus.ToEnum() == Model.Enums.EmailVerificationStatus.Verified)
            {
                return View("ConfirmEmail", null, Resources.Messages.UserEmailActuallyConfirmed);
            }

            IdentityResult result = await authProvider.ConfirmEmail(userId, code);
            if (result.Succeeded)
            {
                return View("ConfirmEmail", null, Resources.Messages.EmailConfirmSuccess);
            }

            ModelState.AddModelError("", String.Join("\n", result.Errors));

            return View("Error");
        }

        /// <summary>
        /// Forgot password.
        /// </summary>
        /// <returns>View.</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Web form for sending email if forgot password.
        /// </summary>
        /// <param name="model">Model with email.</param>
        /// <returns>View.</returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            using (var client = new HttpClient())
            {
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                                 Request.ApplicationPath.TrimEnd('/') + "/";

                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                HttpResponseMessage response = client.PostAsJsonAsync("api/account/forgotpassword", model).Result;
                if (response.IsSuccessStatusCode)
                {
                    return View("ForgotPasswordConfirmation");
                }
                String responseResult = response.Content.ReadAsStringAsync().Result;
                var errors = JsonConvert.DeserializeObject<Dictionary<String, String>>(responseResult);
                foreach (var error in errors)
                {
                    ModelState.AddModelError("", error.Value);
                }
            }
            return View("ForgotPassword");
        }


        /// <summary>
        /// Reset password.
        /// </summary>
        /// <param name="model">Data model.</param>
        /// <returns>View</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            IdentityResult result = await authProvider.ResetPassword(model.Email, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "UserAccount");
            }

            AddErrors(result);
            return View();
        }

        /// <summary>
        /// Reset password confirmarion.
        /// </summary>
        /// <returns>View</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="code">Code.</param>
        /// <param name="email">Email.</param>
        /// <returns>View</returns>
        [AllowAnonymous]
        public ActionResult ResetPassword(String code, String email)
        {
            return code == null ? View("Error") : View();
        }

        /// <summary>
        /// Add errors to Model state
        /// </summary>
        /// <param name="result">Identity result</param>
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}