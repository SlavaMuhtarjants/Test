namespace Weezlabs.Storgage.SecurityService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DataLayer;
    using DataLayer.Dictionaries;   
    using DataLayer.Users;
    using DataTransferObjects.Security;
    using DataTransferObjects.User;
    using FacebookService;
    using Model;   
    using PhotoService;
    using UtilService;

    using Microsoft.AspNet.Identity;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Tests for AuthProvider.
    /// </summary>
    [TestFixture]
    public class TestAuthProvider
    {
        private Mock<IDictionaryProvider> dictionaryProviderMock;
        private Mock<IPhotoProvider> photoProviderMock;

        private Mock<IUserRepository> userRepositoryMock;
        private Mock<IRefreshTokenRepository> refreshTokenRepositoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private Mock<IAppSettings> appSettingsMock;
        private IList<User> usersList;
        private IList<RefreshToken> refreshTokenList; 

        private IUserStore<User, Guid> userStore;
        private UserManager<User, Guid> userManager;
        private IFacebookDataProvider facebookDataProvider;

        private IAuthProvider authProvider;

        private void Init()
        {
            dictionaryProviderMock = new Mock<IDictionaryProvider>();
            dictionaryProviderMock.Setup(x => x.PhoneVerificationStatuses)
                .Returns(new PhoneVerificationStatus[] 
                {
                    new PhoneVerificationStatus { Id = Guid.NewGuid(), Title = "Verified", Synonym = "Verified" },
                    new PhoneVerificationStatus { Id = Guid.NewGuid(), Title = "NotVerified", Synonym = "NotVerified" },
                    new PhoneVerificationStatus { Id = Guid.NewGuid(), Title = "MustVerified", Synonym = "MustVerified" }
                });

            //todo: create tests for uploading/deleting images
            photoProviderMock = new Mock<IPhotoProvider>();
            appSettingsMock = new Mock<IAppSettings>();

            usersList = new List<User>();

            userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(x => x.GetAll()).Returns(() => usersList.AsQueryable());
            userRepositoryMock.Setup(x => x.Add(It.IsAny<User>())).Callback<User>(x => usersList.Add(x));
            userRepositoryMock.Setup(x => x.Update(It.IsAny<User>())).Callback<User>(x =>
                {
                    var userFromRepository = usersList.SingleOrDefault(u => u.Id == x.Id);
                    if (userFromRepository != null)
                    {
                        usersList.Remove(userFromRepository);
                        usersList.Add(x);
                    }
                });

            refreshTokenList = new List<RefreshToken>();

            refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            refreshTokenRepositoryMock.Setup(x => x.GetAll()).Returns(() => refreshTokenList.AsQueryable());
            refreshTokenRepositoryMock.Setup(x => x.Add(It.IsAny<RefreshToken>())).Callback<RefreshToken>(x => refreshTokenList.Add(x));
            refreshTokenRepositoryMock.Setup(x => x.Update(It.IsAny<RefreshToken>())).Callback<RefreshToken>(x =>
            {
                var refreshTokenFromRepository = refreshTokenList.SingleOrDefault(u => u.Id == x.Id);
                if (refreshTokenFromRepository != null)
                {
                    refreshTokenList.Remove(refreshTokenFromRepository);
                    refreshTokenList.Add(x);
                }
            });

            unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(x => x.CommitChanges()).Callback(() => { });

            userStore = new UserIdentityStore(userRepositoryMock.Object, dictionaryProviderMock.Object, unitOfWorkMock.Object);
            ///IdentityFactoryOptions<CustomUserManager> options,
            //IOwinContext context, IUserRepository userRepository, IDictionaryProvider dictionaryProvider, IAppSettings appSettings
            userManager = new CustomUserManager(userStore);
            facebookDataProvider = new FacebookDataProvider();
            var userRepository = new Mock<IUserReadonlyRepository>();

            authProvider = new AuthProvider(userManager, appSettingsMock.Object, unitOfWorkMock.Object,
                photoProviderMock.Object, userRepository.Object, facebookDataProvider, refreshTokenRepositoryMock.Object, dictionaryProviderMock.Object);
        }

        /// <summary>
        /// Test sign up.
        /// </summary>
        [Test]
        public void TestSignUp()
        {
            Init();

            var signupInfo = new SignupInfo
            {
                Password = "1234567890",
                Contact = new UserContact { Email = "pupkin@notexisted.com", Phone = "+1234567890" },
                FullName = new UserFullName { Firstname = "Vasya", Lastname = "Pupkin" }

            };
            var result = authProvider.RegisterUser(signupInfo).Result;
            Assert.IsTrue(result.Succeeded);

            var user = authProvider.FindUser(signupInfo.Contact.Email, signupInfo.Password).Result;
            Assert.IsNotNull(user);
            Assert.AreEqual(signupInfo.Contact.Email, user.Email);
        }

        /// <summary>
        /// Test sign up using existed username.
        /// </summary>
        [Test]
        public void TestSignUpWithExistedUsername()
        {
            Init();

            var signupInfo = new SignupInfo
            {
                Password = "1234567890",
                Contact = new UserContact { Email = "pupkin@notexisted.com", Phone = "+1234567890" },
                FullName = new UserFullName { Firstname = "Vasya", Lastname = "Pupkin" }
            };
            var result = authProvider.RegisterUser(signupInfo).Result;
            Assert.IsTrue(result.Succeeded);

            result = authProvider.RegisterUser(signupInfo).Result;
            Assert.IsFalse(result.Succeeded);
        }

        /// <summary>
        /// Test sign up with empty password.
        /// </summary>
        [Test]
        public void TestSignUpWithEmptyPassword()
        {
            Init();

            var signupInfo = new SignupInfo
            {
                Password = String.Empty,
                Contact = new UserContact { Email = "pupkin@notexisted.com", Phone = "+1234567890" },
                FullName = new UserFullName { Firstname = "Vasya", Lastname = "Pupkin" }
            };
            var result = authProvider.RegisterUser(signupInfo).Result;
            Assert.IsFalse(result.Succeeded);
        }

        /// <summary>
        /// Test sign up with empty username.
        /// </summary>
        [Test]
        public void TestSignUpWithEmptyUsername()
        {
            Init();

            var signupInfo = new SignupInfo
            {
                Password = "1234567890",
                Contact = new UserContact { Email = "", Phone = "+1234567890" },
                FullName = new UserFullName { Firstname = "Vasya", Lastname = "Pupkin" }
            };
            var result = authProvider.RegisterUser(signupInfo).Result;
            Assert.IsFalse(result.Succeeded);
        }

        /// <summary>
        /// Sign in with valid and invalid user credentials.
        /// </summary>
        [Test]
        public void TestSignIn()
        {
            Init();

            var signupInfo = new SignupInfo
            {
                Password = "1234567890",
                FullName = new UserFullName { Firstname = "Vasya", Lastname = "Pupkin" },
                Contact = new UserContact { Email = "pupkin@notexisted.com", Phone = "+1234567890" }
            };
            var result = authProvider.RegisterUser(signupInfo).Result;

            var user = authProvider.FindUser(signupInfo.Contact.Email, signupInfo.Password).Result;
            Assert.IsNotNull(user);
            Assert.AreEqual(signupInfo.Contact.Email, user.UserName);

            var notExistedUser = authProvider.FindUser("notexisteduser@notexistedserver.com", "wrongpasword").Result;
            Assert.IsNull(notExistedUser);

            var userWithWrongPassword = authProvider.FindUser(signupInfo.Contact.Email, "worngpassword").Result;
            Assert.IsNull(userWithWrongPassword);
        }
    }
}
