namespace Weezlabs.Storgage.SecurityService
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using DataTransferObjects.Security;  
    using DataLayer;
    using DataLayer.Dictionaries;
    using DataLayer.Users;
    using DataTransferObjects.User;
    using Model;   
    using Model.Exceptions;
    using Model.ModelExtension;
    using PhotoService;
    using UtilService;

    using Microsoft.AspNet.Identity;
    using FacebookService;

    /// <summary>
    /// Authentication provider implementation.
    /// </summary>
    public class AuthProvider : IAuthProvider
    {
        private readonly UserManager<User, Guid> userManager;
        private readonly IAppSettings appSettings;
        private readonly IUnitOfWork unitOfWork;
        private readonly IPhotoProvider photoProvider;
        private readonly IUserReadonlyRepository userRepository;
        private readonly IFacebookDataProvider facebookDataProvider;
        private readonly IRefreshTokenRepository refreshTokenRepository;
        private readonly IDictionaryProvider dictionaryProvider;

        /// <summary>
        /// Creates authentication provider.
        /// </summary>
        /// <param name="userManager">User manager of ASP Identity.</param>
        /// <param name="appSettings">Application settings.</param>
        /// <param name="unitOfWork">Unit of work.</param>
        /// <param name="photoProvider">Photo provider.</param>   
        /// <param name="userRepository">User repository.</param>
        /// <param name="facebookDataProvider">Facebook data provider.</param>
        /// <param name="refreshTokenRepository">Refresh token repository.</param>        
        public AuthProvider(UserManager<User, Guid> userManager,
            IAppSettings appSettings,
            IUnitOfWork unitOfWork,
            IPhotoProvider photoProvider,
            IUserReadonlyRepository userRepository,
            IFacebookDataProvider facebookDataProvider,
            IRefreshTokenRepository refreshTokenRepository,            
            IDictionaryProvider dictionaryProvider)
        {
            Contract.Requires(userManager != null);
            Contract.Requires(appSettings != null);
            Contract.Requires(unitOfWork != null);
            Contract.Requires(photoProvider != null);
            Contract.Requires(userRepository != null);
            Contract.Requires(facebookDataProvider != null);
            Contract.Requires(refreshTokenRepository != null);
            Contract.Requires(dictionaryProvider != null);

            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
            this.appSettings = appSettings;
            this.photoProvider = photoProvider;
            this.userRepository = userRepository;
            this.facebookDataProvider = facebookDataProvider;
            this.refreshTokenRepository = refreshTokenRepository;
            this.dictionaryProvider = dictionaryProvider;
        }

        /// <summary>
        /// Get user by identifier.
        /// </summary>
        /// <param name="userId">Identifier of user.</param>
        /// <returns>User.</returns>
        public async Task<User> GetUser(Guid userId)
        {
            Contract.Requires(userId != null);

            var result = await userManager.FindByIdAsync(userId);
            return result;
        }

        /// <summary>
        /// Signs user up.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        /// <returns>Sign up result.</returns>
        public async Task<User> FindUser(String userName, String password)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(userName));
            Contract.Requires(!String.IsNullOrWhiteSpace(password));

            var result = await userManager.FindAsync(userName, password);

            return result;
        }

        public async Task<User> FindUserByFb(String facebookId)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(facebookId));

            var result = await userManager.FindAsync(new UserLoginInfo("facebook", facebookId));

            return result;
        }

        /// <summary>
        /// Finds user by username and password.
        /// </summary>
        /// <param name="signUpInfo">SignUp info.</param>
        /// <returns>User.</returns>
        public async Task<IdentityResult> RegisterUser(SignupInfo signUpInfo)
        {
            Contract.Requires(signUpInfo != null);

            var userIdentity = signUpInfo.ToModel();

            userIdentity.PhoneVerificationStatusID = Model.Enums.PhoneVerificationStatus.MustVerified.GetDictionaryId();
            userIdentity.EmailVerificationStatusID = Model.Enums.EmailVerificationStatus.MustVerified.GetDictionaryId();

            var result = await userManager.CreateAsync(userIdentity, signUpInfo.Password);
            unitOfWork.CommitChanges();

            return result;
        }

        private IDictionary<String, Object> GetFacebookInfo (String facebookToken)
        {
            try
            {
                return facebookDataProvider.GetUserInfo(facebookToken);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Resources.Messages.FacebookTokenError, ex);
            }
        }

        /// <summary>
        /// Registers user by facebookToken.
        /// </summary>
        /// <param name="facebookToken">SignUp info.</param>
        /// <returns>User.</returns>
        public async Task<IdentityResultFb> RegisterFacebookUser(String facebookToken)
        {
            //I prefere to call RegisterUser method but task was described as copy-past

            Contract.Requires(!String.IsNullOrWhiteSpace(facebookToken));

            var fbUserInfo = GetFacebookInfo(facebookToken);
            //Facebook.FacebookOAuthException
           
            var facebookId = fbUserInfo["id"].ToString();

            var foundUser = await FindUserByFb(facebookId);

            if (foundUser != null)
            {
                return new IdentityResultFb(IdentityResult.Success, foundUser, false);
            }

            var userIdentity = new User
            {
                Firstname = fbUserInfo["first_name"].ToString(),
                Lastname = fbUserInfo["last_name"].ToString(),
                Email = fbUserInfo.ContainsKey("email") ? fbUserInfo["email"].ToString() : null,
                FacebookID = facebookId,
                PhoneVerificationStatusID = Model.Enums.PhoneVerificationStatus.MustVerified.GetDictionaryId(),
                EmailVerificationStatusID = Model.Enums.EmailVerificationStatus.MustVerified.GetDictionaryId(),
                IsExternal = true
            };

            var result = await userManager.CreateAsync(userIdentity);
            if (!result.Succeeded)
            {
                throw new BadRequestException(String.Join("\n", result.Errors));
            }
            
            unitOfWork.CommitChanges();
            User createdUser = await FindUserByFb(facebookId);

            // upload photo after creating user
            String avatar = LoadAvatarFromFb(fbUserInfo);
            if (!String.IsNullOrWhiteSpace(avatar))
            {
                createdUser.AvatarLink = avatar;
                // One more commit for saving avatar
                unitOfWork.CommitChanges();
            }

            return new IdentityResultFb(result, createdUser, true);
        }

        /// <summary>
        /// Sets FacebookId for user ONLY. It doesn't copy email or etc from facebook to existing account (do not update/refresh)
        /// </summary>
        /// <param name="signUpInfo">SignUp info.</param>
        /// <returns>User.</returns>
        public async Task<Boolean> SetFacebookUser(String facebookToken, Guid userId)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(facebookToken));

            User user = userRepository.GetById(userId);

            user.FacebookID = GetFacebookInfo(facebookToken)["id"].ToString();

            await userManager.UpdateAsync(user);

            try
            {
                unitOfWork.CommitChanges();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("idx_User_FacebookID_incl"))
                {
                    throw new BadRequestException(Resources.Messages.UniqueFacebookIdError);
                }
            }

            return true;

        }
        
        /// <summary>
        /// Updates user's attributes Email, Phone, LastName, FirstName
        /// </summary>
        /// <param name="userId">User identifer</param>
        /// <param name="userToUpdate">Updated model but Email, Phone, LastName, FirstName attributes will be used only</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <returns>Current user with all attrubutes but Email, Phone, LastName, FirstName will be the same that were passed in the userToUpdate</returns>
        public UserFullInfo UpdateUser(Guid userId, AccountInfo userToUpdate, Guid actorId)
        {
            var user = GetUserWithCheckPermissions(userId, actorId);

            if (!user.IsExternal && (userToUpdate.Contact.Email == null || userToUpdate.Contact.Phone == null))
            {                
                throw new BadRequestException(Resources.Messages.InternalUserMustHaveEmailAndPhone);
            }

            if (!(user.Email ?? String.Empty).Equals(userToUpdate.Contact.Email ?? String.Empty))
            {
                user.Email = userToUpdate.Contact.Email;
                // note: we should use:
                // userManager.SetEmailAsync(userId, userToUpdate.Contact.Email).Wait();
                // but default implementation of userManager call SetEmailConfirmedAsync after set Email async
                // because by default we should confirm email before set it.
                user.EmailVerificationStatusID = Model.Enums.EmailVerificationStatus.MustVerified.GetDictionaryId();
            }
            if (!(user.Phone ?? String.Empty).Equals(userToUpdate.Contact.Phone ?? String.Empty))
            {
                // note: like set email
                user.Phone = userToUpdate.Contact.Phone;
                user.PhoneVerificationStatusID = Model.Enums.PhoneVerificationStatus.MustVerified.GetDictionaryId();
                //It doesn't work
                //user.PhoneVerificationStatus = dictionaryProvider.PhoneVerificationStatuses.Single(x => x.ToEnum() == Model.Enums.PhoneVerificationStatus.MustVerified);
            }

            user.Lastname = userToUpdate.FullName.Lastname;
            user.Firstname = userToUpdate.FullName.Firstname;

            userManager.UpdateAsync(user).Wait();

            try
            {
                unitOfWork.CommitChanges();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("idx_User_Email_unique_incl"))
                {
                    throw new BadRequestException(Resources.Messages.UniqueEmailError);
                }
            }

            return new UserFullInfo(user);
        }

        /// <summary>
        /// User change password
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        public void ChangePassword(Guid userId, ChangePasswordInfo request)
        {

            var changePassResult = userManager.ChangePasswordAsync(userId, request.OldPassword, request.NewPassword).Result;

            if (!changePassResult.Succeeded)
            {
                throw new BadRequestException(String.Join("\n", changePassResult.Errors));
            }

            unitOfWork.CommitChanges();
        }

        /// <summary>
        /// Reset password by email
        /// </summary>
        /// <param name="email">Email.</param>
        /// <param name="token">Token.</param>
        /// <param name="password">Password.</param>
        /// <returns>IdentityResult.</returns>
        public async Task<IdentityResult> ResetPassword(String email, String token, String password)
        {
            User user = userManager.FindByNameAsync(email).Result;
            if (user == null)
            {
                return new IdentityResult(Resources.Messages.UserNotFound);
            }

            IdentityResult result = await userManager.ResetPasswordAsync(user.Id, token, password);
            if (!result.Succeeded)
            {
                return result;
            }

            unitOfWork.CommitChanges();
            return result;
        }

        /// <summary>
        /// Confirm email
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="token">Token.</param>
        public async Task<IdentityResult> ConfirmEmail(Guid userId, String token)
        {
            var result = await userManager.ConfirmEmailAsync(userId, token);

            unitOfWork.CommitChanges();
            return result;
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email body.</param>
        /// <returns></returns>
        public async Task<IdentityResult> SendEmail(Guid userId, String subject, String body)
        {
            await userManager.SendEmailAsync(userId, subject, body);

            User user = GetUserWithCheckPermissions(userId);
            if (user.EmailVerificationStatus.ToEnum() == Model.Enums.EmailVerificationStatus.MustVerified)
            {
                user.EmailVerificationStatusID = Model.Enums.EmailVerificationStatus.NotVerified.GetDictionaryId();
            }

            unitOfWork.CommitChanges();
            return IdentityResult.Success;
        }

        /// <summary>
        /// Send phone confirmation message
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>IdentityResult</returns>
        public async Task<IdentityResult> SendPhoneConfirmation(Guid userId)
        {
            User user = GetUserWithCheckPermissions(userId);
            String code = await userManager.GenerateChangePhoneNumberTokenAsync(userId, user.Phone);
            
            var responceMessage = String.Empty; // NOTE!!!! temp implementation for checking code
            try
            {
                await userManager.SendSmsAsync(userId, String.Format(Resources.Messages.SmsVerificationMessage, code));
            }
            catch (PostMessageException ex)
            {
                return IdentityResult.Failed(ex.Message);
            }
            catch (BadRequestException ex)
            {
                // NOTE!!!! temp implementation for checking code
                responceMessage = ex.Message;
            }
            
            if (user.PhoneVerificationStatus.ToEnum() == Model.Enums.PhoneVerificationStatus.MustVerified)
            {
                user.PhoneVerificationStatusID = Model.Enums.PhoneVerificationStatus.NotVerified.GetDictionaryId();
            }

            unitOfWork.CommitChanges();

            // NOTE!!! temp implementation for checking code
            if (!String.IsNullOrWhiteSpace(responceMessage))
            {
                return IdentityResult.Failed(responceMessage);
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// Confirm phone
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="code">Token.</param>
        /// <returns>IdentityResult</returns>
        public async Task<IdentityResult> ConfirmPhone(Guid userId, String code)
        {
            User user = GetUserWithCheckPermissions(userId);

            Boolean verifyResult = await userManager.VerifyChangePhoneNumberTokenAsync(userId, code, user.Phone);
            if (verifyResult)
            {
                user.PhoneVerificationStatusID = Model.Enums.PhoneVerificationStatus.Verified.GetDictionaryId();

                unitOfWork.CommitChanges();

                return IdentityResult.Success;
            }

            return IdentityResult.Failed(Resources.Messages.PhoneVerificationCodeInvalid);
        }

        /// <summary>
        /// Upload avatar.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="formData">IEnumerable of files.</param>
        public UserAvatar UploadAvatar(Guid userId, IEnumerable<HttpContent> formData)
        {
            User user = GetUserWithCheckPermissions(userId);
            String oldAvatar = user.AvatarLink;

            List<HttpContent> files = formData.ToList();

            Int32 countOfNewPhotos = files.Count();
            if (countOfNewPhotos > 1)
            {
                throw new ImagesUploadOverflowException(String.Format(Resources.Messages.InvalidAvatarUpload,
                    countOfNewPhotos));
            }

            Int32 maxFileLength = appSettings.GetSetting<Int32>("maxLengthOfImage");
            files.CheckFilesLength(maxFileLength);

            HttpContent avatar = files.Single();

            Stream stream = avatar.ReadAsStreamAsync().Result;
            String newFileName = avatar.GenerateFileNameWithExt();

            String file = UploadUserAvatarToS3(newFileName, stream);

            if (!String.IsNullOrWhiteSpace(file))
            {
                user.AvatarLink = file;
            }

            unitOfWork.CommitChanges();

            if (!String.IsNullOrEmpty(oldAvatar))
            {
                DeleteUserAvatarFromS3(oldAvatar);
            }

            return new UserAvatar(user.AvatarLink);
        }

        /// <summary>
        /// Skip avatar.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        public void SkipAvatar(Guid userId)
        {
            User user = GetUserWithCheckPermissions(userId);
            String oldAvatar = user.AvatarLink;

            if (!String.IsNullOrWhiteSpace(oldAvatar))
            {
                user.AvatarLink = null;
                unitOfWork.CommitChanges();

                DeleteUserAvatarFromS3(oldAvatar);
            }
        }

        /// <summary>
        /// Get Space by id with checking permissions.
        /// </summary>
        /// <param name="userId">Identifier of user.</param>
        /// <param name="actorId">Identifier of actor or null if don't need checking access.</param>
        /// <returns>User if actor is user.</returns>
        private User GetUserWithCheckPermissions(Guid userId, Guid actorId = default(Guid))
        {
            User user = userRepository.GetById(userId);

            if (user == null)
            {
                throw new NotFoundException(String.Format(Resources.Messages.UserNotFound, userId));
            }

            if (actorId == default(Guid))
            {
                actorId = userId;
            }

            if (user.Id != actorId)
            {
                throw new AccessDeniedException();
            }

            return user;
        }

        /// <summary>
        /// Load avatar from Facebook
        /// </summary>
        /// <param name="fbUserInfo">Facebook </param>
        /// <returns></returns>
        private String LoadAvatarFromFb(IDictionary<String, Object> fbUserInfo)
        {
            String uploadedFile = string.Empty;
            try
            {
                // get avatar url from fbInfo
                String avatarUrl =
                    ((fbUserInfo["picture"] as IDictionary<String, Object>)["data"] as IDictionary<String, Object>)[
                        "url"]
                        .ToString();

                // generate new filename for avatar
                String fileName = null;
                var uri = new Uri(avatarUrl);
                fileName = Path.GetFileName(uri.AbsolutePath);
                String newFileName = FilesHelper.GenerateFileNameWithExt(fileName);

                // get avatar as Stream
                Byte[] imageData = null;
                using (var wc = new System.Net.WebClient())
                {
                    imageData = wc.DownloadData(avatarUrl);
                }

                // upload avatar with thumbnail to s3
                var stream = new MemoryStream(imageData);
                uploadedFile = UploadUserAvatarToS3(newFileName, stream);
            }
            catch
            {
                // ignored
            }

            return uploadedFile;
        }

        /// <summary>
        /// Upload avatar on s3 bucket.
        /// </summary>
        /// <param name="newFileName">New name of file</param>
        /// <param name="stream">Stream.</param>
        /// <returns>Name of uploaded file.</returns>
        private String UploadUserAvatarToS3(String newFileName, Stream stream)
        {
            String file = photoProvider.UploadFileWithThumbnails(appSettings.GetS3UsersBucket(),
                appSettings.GetS3UsersOriginal(),
                appSettings.GetS3UsersThumbnails(), newFileName, stream);

            return file;
        }

        /// <summary>
        /// Delete user avatar from s3 bucket
        /// </summary>
        /// <param name="oldAvatar">Filename of deleted avatar.</param>
        private void DeleteUserAvatarFromS3(String oldAvatar)
        {
            photoProvider.DeleteFiles(appSettings.GetS3UsersBucket(), appSettings.GetS3UsersOriginal(),
                appSettings.GetS3UsersThumbnails(), new String[] {oldAvatar});
        }

        /// <summary>
        /// Add refresh token
        /// </summary>
        /// <param name="token">Refresh token model.</param>
        /// <returns></returns>
        public async Task<Boolean> AddRefreshToken(RefreshToken token)
        {
            await Task.Run(() => refreshTokenRepository.Add(token));
            unitOfWork.CommitChanges();

            return true;
        }

        /// <summary>
        /// Remove refresh token.
        /// </summary>
        /// <param name="refreshTokenId">Refresh token identifier.</param>
        /// <param name="actor"></param>
        /// <returns></returns>
        public async Task<Boolean> RemoveRefreshToken(String refreshTokenId, Guid actor = default(Guid))
        {
            var refreshToken =
                await Task.Run(() => refreshTokenRepository.GetAll().SingleOrDefault(x => x.Id == refreshTokenId));

            if (refreshToken == null)
            {
                throw new NotFoundException();
            }

            if (actor != default(Guid) && refreshToken.UserId != actor)
            {
                throw new AccessDeniedException();
            }
            
            refreshTokenRepository.Delete(refreshToken);
            unitOfWork.CommitChanges();

            return true;
        }

        /// <summary>
        /// Find refresh token by id.
        /// </summary>
        /// <param name="refreshTokenId">Refresh token identifier.</param>
        /// <returns>RefreshToken</returns>
        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken =
                await Task.Run(() => refreshTokenRepository.GetAll().SingleOrDefault(x => x.Id == refreshTokenId));

            return refreshToken;
        }
    }
}
