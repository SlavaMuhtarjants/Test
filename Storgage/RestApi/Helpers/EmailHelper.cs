namespace Weezlabs.Storgage.RestApi.Helpers
{
    using System;

    using DataTransferObjects.ViewModels.Account;

    /// <summary>
    /// Helper for creating email
    /// </summary>
    public static class EmailHelper
    {
        /// <summary>
        /// Create email for password reset.
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <param name="callbackUrl">Callback url.</param>
        /// <param name="email">Email.</param>
        /// <param name="expiredHours">Expired hours.</param>
        /// <returns>Html body.</returns>
        public static String CreateEmailBodyForPasswordReset(Guid userId, String callbackUrl, String email, Int32 expiredHours)
        {
            var emailModel = new ForgotPasswordEmailViewModel()
            {
                Email = email,
                Link = callbackUrl,
                Expired = DateTime.UtcNow.AddHours(expiredHours)
            };

            String html = RazorViewEngineHelper.RenderViewToString("Account",
                "~/Views/Templates/ResetPassword.cshtml", emailModel);

            return html;
        }

        /// <summary>
        /// Create email for email confirm.
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <param name="callbackUrl">Callback url.</param>
        /// <param name="email">Email.</param>
        /// <param name="expiredHours">Expired hours.</param>
        /// <returns>Html body.</returns>
        public static String CreateEmailBodyForEmailConfirm(Guid userId, String callbackUrl, String email, Int32 expiredHours)
        {
            var emailModel = new ConfirmEmailViewModel()
            {
                Email = email,
                Link = callbackUrl,
                Expired = DateTime.UtcNow.AddHours(expiredHours)
            };

            String html = RazorViewEngineHelper.RenderViewToString("Account",
                "~/Views/Templates/ConfirmEmail.cshtml", emailModel);

            return html;
        }
    }
}