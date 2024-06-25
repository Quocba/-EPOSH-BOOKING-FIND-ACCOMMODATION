using DocumentFormat.OpenXml.Spreadsheet;
using GraduationAPI_EPOSHBOOKING.Controllers.Admin;
using GraduationAPI_EPOSHBOOKING.Controllers.Auth;
using GraduationAPI_EPOSHBOOKING.DTO;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using static GraduationAPI_EPOSHBOOKING.Controllers.Auth.AuthController;
using static GraduationAPI_EPOSHBOOKING.Controllers.Admin.AdminAccountController;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using GraduationAPI_EPOSHBOOKING.Ultils;
#pragma warning disable // tắt cảnh báo để code sạch hơn
namespace UnitTestingAPI
{
    public class AccountTesting
    {
        private AuthController authController;
        private AdminAccountController adminAccountController;
        private Mock<IAccountRepository> _mockRepository;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IAccountRepository>();
            authController = new AuthController(_mockRepository.Object);
            adminAccountController = new AdminAccountController(_mockRepository.Object, null);
        }

        [Test]
        public void LoginWithNumberPhone()
        {
            // Arrange
            string phone = "0923343536";
            _mockRepository.Setup(repo => repo.LoginWithNumberPhone(phone))
                .Returns(new ResponseMessage { Success = true, Data = phone, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = authController.LoginPhone(phone) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.That(responseMessage.Data, Is.EqualTo(phone));
        }

        [Test]
        public void LoginWithNumberPhone_ReturnsBadRequest()
        {
            // Arrange
            string[] invalidPhoneNumbers = { "0923343536", "012345689", "anhyeuem", "$%$%", "" };

            foreach (string phone in invalidPhoneNumbers)
            {
                _mockRepository.Setup(repo => repo.LoginWithNumberPhone(phone))
                    .Returns(new ResponseMessage { Success = false, Data = phone, Message = "Phone is not in correct format.", StatusCode = (int)HttpStatusCode.BadRequest });

                // Act
                var result = authController.LoginPhone(phone) as ObjectResult;
                var responseMessage = result.Value as ResponseMessage;

                // Assert
                Assert.AreEqual(400, result.StatusCode);
                Assert.That(responseMessage.Success, Is.False);
                Assert.That(responseMessage.Message, Is.EqualTo("Phone is not in correct format."));
            }
        }

        [Test]
        public void LoginWithNumberPhone_ReturnsNotFound()
        {
            // Arrange
            string phone = "0923343536";
            _mockRepository.Setup(repo => repo.LoginWithNumberPhone(phone))
                .Returns(new ResponseMessage { Success = false, Data = phone, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = authController.LoginPhone(phone) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Data not found"));
        }

        [Test]
        public void LoginWithNumberPhone_ReturnsAlreadyReported()
        {
            // Arrange
            string phone = "0923343536";
            _mockRepository.Setup(repo => repo.LoginWithNumberPhone(phone))
                .Returns(new ResponseMessage { Success = false, Data = phone, Message = "Phone number already exists.", StatusCode = (int)HttpStatusCode.AlreadyReported });

            // Act
            var result = authController.LoginPhone(phone) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(208, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Phone number already exists."));
        }

        [Test]
        public void RegisterCustomer()
        {
            RegisterDTO register = new RegisterDTO
            {
                Email = "tao@gmail.com",
                Password = "123456",
                FullName = "Nguyen Van A",
                Phone = "0923343536"
            };

            _mockRepository.Setup(repo => repo.Register(register.Email, register.Password, register.FullName, register.Phone))
                    .Returns(new ResponseMessage { Success = true, Data = register, StatusCode = (int)HttpStatusCode.OK });
            var result = authController.Register(register) as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void RegisterCustomerFailed()
        {
            RegisterDTO register = new RegisterDTO
            {
                Email = "",
                Password = "",
                FullName = "",
                Phone = ""
            };

            _mockRepository.Setup(repo => repo.Register(register.Email, register.Password, register.FullName, register.Phone))
                    .Returns(new ResponseMessage { Success = false, Data = register, StatusCode = (int)HttpStatusCode.BadRequest });
            var result = authController.Register(register) as ObjectResult;
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public void RegisterCustomerExist()
        {
            RegisterDTO register = new RegisterDTO
            {
                Email = ""
            };

            _mockRepository.Setup(repo => repo.Register(register.Email, register.Password, register.FullName, register.Phone))
                    .Returns(new ResponseMessage { Success = false, Data = register, StatusCode = (int)HttpStatusCode.AlreadyReported });
            var result = authController.Register(register) as ObjectResult;
            Assert.AreEqual(208, result.StatusCode);

        }
        [Test]
        public void RegisterCustomerNotFound()
        {
            RegisterDTO register = new RegisterDTO
            {
                Email = ""
            };

            _mockRepository.Setup(repo => repo.Register(register.Email, register.Password, register.FullName, register.Phone))
                    .Returns(new ResponseMessage { Success = false, Data = register, StatusCode = (int)HttpStatusCode.NotFound });
            var result = authController.Register(register) as ObjectResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void RegisterPartnerAccount()
        {
            Account account = new Account
            {
                Email = "tao@gmail.com",
                Password = "123456789",
                Phone = "0977778822"


            };
            String fullName = "Nguyen Van A";
            _mockRepository.Setup(repo => repo.RegisterPartnerAccount(account, fullName))
                    .Returns(new ResponseMessage { Success = true, Data = account, StatusCode = (int)HttpStatusCode.OK });
            var result = authController.RegisterPartnerAccount(account, fullName) as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);

        }

        [Test]
        public void RegisterPartnerAccountFailed()
        {
            Account account = new Account
            {
                Email = "",
                Password = "",
                Phone = ""
            };
            String fullName = "";
            _mockRepository.Setup(repo => repo.RegisterPartnerAccount(account, fullName))
                    .Returns(new ResponseMessage { Success = false, Data = account, StatusCode = (int)HttpStatusCode.BadRequest });
            var result = authController.RegisterPartnerAccount(account, fullName) as ObjectResult;
            Assert.AreEqual(400, result.StatusCode);
        }
        [Test]
        public void RegisterPartnerAccountExist()
        {

            Account account = new Account
            {
                Email = ""
            };
            String fullName = "";
            _mockRepository.Setup(repo => repo.RegisterPartnerAccount(account, fullName))
                    .Returns(new ResponseMessage { Success = false, Data = account, StatusCode = (int)HttpStatusCode.AlreadyReported });
            var result = authController.RegisterPartnerAccount(account, fullName) as ObjectResult;
            Assert.AreEqual(208, result.StatusCode);
        }
        [Test]
        public void RegisterPartnerAccountNotFound()
        {
            Account account = new Account
            {
                Email = ""
            };
            String fullName = "";
            _mockRepository.Setup(repo => repo.RegisterPartnerAccount(account, fullName))
                    .Returns(new ResponseMessage { Success = false, Data = account, StatusCode = (int)HttpStatusCode.NotFound });
            var result = authController.RegisterPartnerAccount(account, fullName) as ObjectResult;
            Assert.AreEqual(404, result.StatusCode);
        }


        [Test]
        public void ActiveAccount()
        {
            Account account = new Account
            {
                Email = "tao@gmail.com",
                Password = "123456789",
                Phone = "123456789",
                IsActive = false
            };
            _mockRepository.Setup(repo => repo.ActiveAccount(account.Email))
                    .Returns(new ResponseMessage { Success = true, Data = account, StatusCode = (int)HttpStatusCode.OK });
            var result = authController.ActiveAccount(account.Email) as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void ActiveAccountFailed()
        {
            Account account = new Account
            {
                Email = "",
                Password = "",
                Phone = "",
                IsActive = false
            };
            _mockRepository.Setup(repo => repo.ActiveAccount(account.Email))
                    .Returns(new ResponseMessage { Success = false, Data = account, StatusCode = (int)HttpStatusCode.BadRequest });
            var result = authController.ActiveAccount(account.Email) as ObjectResult;
            Assert.AreEqual(400, result.StatusCode);

        }
        [Test]
        public void ActiveAccountNotFound()
        {
            Account account = new Account
            {
                Email = ""
            };
            _mockRepository.Setup(repo => repo.ActiveAccount(account.Email))
                    .Returns(new ResponseMessage { Success = false, Data = account, StatusCode = (int)HttpStatusCode.NotFound });
            var result = authController.ActiveAccount(account.Email) as ObjectResult;
            Assert.AreEqual(404, result.StatusCode);
        }
        [Test]
        public void ActiveAccountExist()
        {
            Account account = new Account
            {
                Email = ""
            };
            _mockRepository.Setup(repo => repo.ActiveAccount(account.Email))
                    .Returns(new ResponseMessage { Success = false, Data = account, StatusCode = (int)HttpStatusCode.AlreadyReported });
            var result = authController.ActiveAccount(account.Email) as ObjectResult;
            Assert.AreEqual(208, result.StatusCode);
        }

        [Test]
        public void GetAllAccount()
        {
            // Arrange
            List<Account> accounts = new List<Account>
            {
                new Account { AccountID = 1, Email = "abcxyz@gmail.com", Password = "1234567890", Phone = "0912345678"},
                new Account { AccountID = 2, Email = "abyyyz@gmail.com", Password = "0123456789", Phone = "0912345678"}
            };

            _mockRepository.Setup(repo => repo.GetAllAccount())
                   .Returns(new ResponseMessage { Success = true, Data = accounts, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = adminAccountController.GetAllAccount() as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(responseMessage);
            Assert.IsTrue(responseMessage.Success);
            Assert.AreEqual("Successfully", responseMessage.Message);
            Assert.AreEqual(accounts, responseMessage.Data);
        }
        [Test]
        public void GetAllAccountFailed()
        {
            // Arrange
            List<Account> accounts = new List<Account>();

            _mockRepository.Setup(repo => repo.GetAllAccount())
                   .Returns(new ResponseMessage { Success = false, Data = accounts, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = adminAccountController.GetAllAccount() as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNotNull(responseMessage);
            Assert.IsFalse(responseMessage.Success);
            Assert.AreEqual("Data not found", responseMessage.Message);
        }
        [Test]
        public void BlockedAccount()
        {
            int accountId = 1;
            String reasonBlock = "ban";
            _mockRepository.Setup(repo => repo.BlockedAccount(accountId, reasonBlock))
                    .Returns(new ResponseMessage { Success = true, Data = reasonBlock, StatusCode = (int)HttpStatusCode.OK });
            var result = adminAccountController.BlockedAccount(accountId, reasonBlock) as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void BlockedAccountFailed()
        {
            int accountId = 1;
            String reasonBlock = "";
            _mockRepository.Setup(repo => repo.BlockedAccount(accountId, reasonBlock))
                    .Returns(new ResponseMessage { Success = false, Data = reasonBlock, StatusCode = (int)HttpStatusCode.BadRequest });
            var result = adminAccountController.BlockedAccount(accountId, reasonBlock) as ObjectResult;
            Assert.AreEqual(400, result.StatusCode);
        }
        [Test]
        public void BlockedAccountNotFound()
        {
            int accountId = 1;
            String reasonBlock = "";
            _mockRepository.Setup(repo => repo.BlockedAccount(accountId, reasonBlock))
                    .Returns(new ResponseMessage { Success = false, Data = reasonBlock, StatusCode = (int)HttpStatusCode.NotFound });
            var result = adminAccountController.BlockedAccount(accountId, reasonBlock) as ObjectResult;
            Assert.AreEqual(404, result.StatusCode);
        }
        [Test]
        public void FilterAccountByStatus()
        {
            bool isActive = true;
            _mockRepository.Setup(repo => repo.FilterAccountByStatus(isActive))
                    .Returns(new ResponseMessage { Success = true, Data = isActive, StatusCode = (int)HttpStatusCode.OK });
            var result = adminAccountController.FilterAccountByStatus(isActive) as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void SearchAccountByName()
        {
            String name = "Nguyen Van A";
            _mockRepository.Setup(repo => repo.SearchAccountByName(name))
                    .Returns(new ResponseMessage { Success = true, Data = name, StatusCode = (int)HttpStatusCode.OK });
            var result = adminAccountController.SearchAccountByName(name) as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void SearchAccountByNameFailed()
        {
            String name = "";
            _mockRepository.Setup(repo => repo.SearchAccountByName(name))
                    .Returns(new ResponseMessage { Success = false, Data = name, StatusCode = (int)HttpStatusCode.BadRequest });
            var result = adminAccountController.SearchAccountByName(name) as ObjectResult;
            Assert.AreEqual(400, result.StatusCode);
        }
        [Test]
        public void ChangePassword()
        {
            int accountId = 1;
            string currentPassword = "0909092222";
            string newPassword = "123456789";
            _mockRepository.Setup(repo => repo.ChangePassword(accountId, currentPassword, newPassword))
                .Returns(new ResponseMessage { Success = true, Data = newPassword, StatusCode = (int)HttpStatusCode.OK });
            var result = authController.ChangePassword(new ChangePasswordRequest { AccountId = accountId, OldPassword = currentPassword, NewPassword = newPassword }) as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void ChangePasswordFailed()
        {
            int accountId = 1;
            string currentPassword = "";
            string newPassword = "";
            _mockRepository.Setup(repo => repo.ChangePassword(accountId, currentPassword, newPassword))
                .Returns(new ResponseMessage { Success = false, Data = newPassword, StatusCode = (int)HttpStatusCode.BadRequest });
            var result = authController.ChangePassword(new ChangePasswordRequest { AccountId = accountId, OldPassword = currentPassword, NewPassword = newPassword }) as ObjectResult;
            Assert.AreEqual(400, result.StatusCode);
        }
        [Test]
        public void UpdateNewPassword()
        {
            string email = "test@example.com";
            string newPassword = "newPassword123";

            var updatedAccount = new Account
            {
                AccountID = 1,
                Email = email,
                Password = Utils.HashPassword(newPassword),
                Phone = "0123456789",
                IsActive = true
            };
            _mockRepository.Setup(repo => repo.UpdateNewPassword(email, newPassword))
                .Returns(new ResponseMessage { Success = true, Data = updatedAccount, StatusCode = (int)HttpStatusCode.OK });
            var result = authController.UpdateNewPassword(newPassword, email) as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void UpdateNewPassword_EmailNotFound()
        {
            string email = "nonexistent@example.com";
            string newPassword = "newPassword123";

            _mockRepository.Setup(repo => repo.UpdateNewPassword(email, newPassword))
                .Returns(new ResponseMessage { Success = false, Data = email, Message = "Email Does not exitst", StatusCode = (int)HttpStatusCode.NotFound });
            var result = authController.UpdateNewPassword(newPassword, email) as ObjectResult;
            Assert.AreEqual(404, result.StatusCode);
        }
        [Test]
        public void GetProfileByAccountId_Success()
        {
            int accountId = 1;
            var profile = new Profile
            {
                fullName = "Toi yeu FPT",
                BirthDay = new DateTime(1990, 1, 1),
            };

            _mockRepository.Setup(repo => repo.GetProfileByAccountId(accountId))
                .Returns(new ResponseMessage
                {
                    Success = true,
                    Data = profile,
                    Message = "Successfully",
                    StatusCode = (int)HttpStatusCode.OK
                });
            var result = authController.GetProfileByAccountId(accountId) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void GetProfileByAccountId_Failed()
        {
            int accountId = 1;
            var profile = new Profile
            {
                fullName = "",
                BirthDay = new DateTime(1990, 1, 1),
            };

            _mockRepository.Setup(repo => repo.GetProfileByAccountId(accountId))
                .Returns(new ResponseMessage
                {
                    Success = false,
                    Data = profile,
                    Message = "Failed",
                    StatusCode = (int)HttpStatusCode.OK
                });
            var result = authController.GetProfileByAccountId(accountId) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            Assert.AreEqual(200, result.StatusCode);

        }
        [Test]
        public void UpdateProfileByAccount_Success()
        {
            int accountId = 1;
            var profile = new Profile
            {
                fullName = "Toi yeu FPT",
                BirthDay = new DateTime(1990, 1, 1),
                Gender = "Male",
                Address = "FPT Can Tho"
            };
            var mockFile = new Mock<IFormFile>();

            _mockRepository.Setup(repo => repo.UpdateProfileByAccount(accountId, profile, mockFile.Object))
                .Returns(new ResponseMessage
                {
                    Success = true,
                    Data = new { }, 
                    Message = "Successfully",
                    StatusCode = (int)HttpStatusCode.OK 
                });
            var result = authController.UpdateProfileByAccount(accountId, profile, mockFile.Object) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            Assert.AreEqual(200, result.StatusCode); 
            Assert.IsNotNull(responseMessage);
            Assert.IsTrue(responseMessage.Success);
            Assert.AreEqual("Successfully", responseMessage.Message);
        }

        [Test]
        public void UpdateProfileByAccount_Failed()
        {
            int accountId = 1;
            var profile = new Profile
            {
                fullName = "", 
                BirthDay = new DateTime(1990, 1, 1),
            };
            var mockFile = new Mock<IFormFile>();

            _mockRepository.Setup(repo => repo.UpdateProfileByAccount(accountId, profile, mockFile.Object))
                .Returns(new ResponseMessage
                {
                    Success = false, 
                    Data = new { },
                    Message = "Failed", 
                    StatusCode = (int)HttpStatusCode.OK 
                });
            var result = authController.UpdateProfileByAccount(accountId, profile, mockFile.Object) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            Assert.AreEqual(200, result.StatusCode); 
            Assert.IsNotNull(responseMessage);
            Assert.IsFalse(responseMessage.Success);
            Assert.AreEqual("Failed", responseMessage.Message);
        }
        [Test]
        public void Login_Success_ReturnsOk()
        {
            // Arrange
            string email = "test@example.com";
            string password = "password123";
            var account = new Account
            {
                AccountID = 1,
                Email = email,
                Password = Utils.HashPassword(password)
            };

            _mockRepository.Setup(repo => repo.Login(email, password))
                .Returns(new ResponseMessage { Success = true, Data = account, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = authController.Login(new LoginDTO { Email = email, Password = password }) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            var returnedAccount = responseMessage.Data as Account;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.AreEqual(account, returnedAccount); // Kiểm tra dữ liệu account trả về
        }

        [Test]
        public void Login_InvalidCredentials_ReturnsNotFound()
        {
            // Arrange
            string email = "test@example.com";
            string password = "wrongPassword";

            _mockRepository.Setup(repo => repo.Login(email, password))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "Login Fail.Account does not exist!", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = authController.Login(new LoginDTO { Email = email, Password = password }) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Login Fail.Account does not exist!"));
        }
        [Test]
        public void Login_IsBlock()
        {
            // Arrange
            string email = "test@example.com";
            string password = "password123";
            var account = new Account
            {
                AccountID = 1,
                Email = email,
                Password = Utils.HashPassword(password),
                IsActive = false
            };
            _mockRepository.Setup(repo => repo.Login(email, password))
                .Returns(new ResponseMessage { Success = false, Data = account, Message = "Account is blocked", StatusCode = (int)HttpStatusCode.Forbidden });
            var result = authController.Login(new LoginDTO { Email = email, Password = password }) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            Assert.AreEqual(403, result.StatusCode);
        }
        [Test]
        public void Login_PartnerApprovedAndActive_ReturnsOk()
        {
            // Arrange
            string email = "partner@example.com";
            string password = "password123";
            var account = new Account
            {
                AccountID = 1,
                Email = email,
                Password = Utils.HashPassword(password),
                IsActive = true,
                Role = new Role { Name = "Partner" },
                Hotel = new List<Hotel>
        {
            new Hotel { HotelID = 1, isRegister = "Approved", Status = true }
        }
            };

            _mockRepository.Setup(repo => repo.Login(email, password))
                .Returns(new ResponseMessage { Success = true, Data = account, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = authController.Login(new LoginDTO { Email = email, Password = password }) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            var returnedAccount = responseMessage.Data as Account;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.AreEqual(account, returnedAccount);
        }

        [Test]
        public void Login_PartnerNotApproved_ReturnsAccepted()
        {
            // Arrange
            string email = "partner@example.com";
            string password = "password123";
            var account = new Account
            {
                AccountID = 1,
                Email = email, // Bổ sung Email
                Password = Utils.HashPassword(password), // Bổ sung Password
                IsActive = true,
                Role = new Role { Name = "Partner" },
                Hotel = new List<Hotel>
        {
            new Hotel { HotelID = 1, isRegister = "Awaiting Approval", Status = true }
        }
            };
            _mockRepository.Setup(repo => repo.Login(email, password))
                .Returns(new ResponseMessage
                {
                    Success = true,
                    Data = account,
                    Message = "Your partner account is awaiting approval. Please wait for our response email.",
                    StatusCode = (int)HttpStatusCode.Accepted
                });

            // Act
            var result = authController.Login(new LoginDTO { Email = email, Password = password }) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(202, result.StatusCode); // Accepted
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Your partner account is awaiting approval. Please wait for our response email."));
        }
        [Test]
        public void Login_CustomerSuccess_ReturnsOk()
        {
            // Arrange
            string email = "customer@example.com";
            string password = "password123";
            var account = new Account
            {
                AccountID = 1,
                Email = email,
                Password = Utils.HashPassword(password),
                IsActive = true,
                Role = new Role { Name = "Customer" }
            };

            _mockRepository.Setup(repo => repo.Login(email, password))
                .Returns(new ResponseMessage { Success = true, Data = account, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = authController.Login(new LoginDTO { Email = email, Password = password }) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            var returnedAccount = responseMessage.Data as Account;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.AreEqual(account, returnedAccount);
        }
        [Test]
        public void Login_Customer_ReturnsNotFound()
        {
            // Arrange
            string email = "customer@example.com";
            string password = "wrongPassword";

            _mockRepository.Setup(repo => repo.Login(email, password))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "Login Fail.Account does not exist!", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = authController.Login(new LoginDTO { Email = email, Password = password }) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Login Fail.Account does not exist!"));
        }
        [Test]
        public void Login_CustomerIsBlocked_ReturnsForbidden()
        {
            // Arrange
            string email = "customer@example.com";
            string password = "password123";
            var account = new Account
            {
                AccountID = 1,
                Email = email,
                Password = Utils.HashPassword(password),
                IsActive = false,
                Role = new Role { Name = "Customer" }
            };
            _mockRepository.Setup(repo => repo.Login(email, password))
                .Returns(new ResponseMessage { Success = false, Data = account, Message = "Your account has been permanently blocked.", StatusCode = (int)HttpStatusCode.Forbidden });
            var result = authController.Login(new LoginDTO { Email = email, Password = password }) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            Assert.AreEqual(403, result.StatusCode);
        }
        [Test]
        public void Login_PartnerBlocked_ReturnsForbidden()
        {
            // Arrange
            string email = "partner@example.com";
            string password = "password123";
            var account = new Account
            {
                AccountID = 1,
                Email = email,
                Password = Utils.HashPassword(password),
                IsActive = false,
                Role = new Role { Name = "Partner" },
                Hotel = new List<Hotel>
                {
                    new Hotel { HotelID = 1, isRegister = "Blocked", Status = false }
                }
            };

            _mockRepository.Setup(repo => repo.Login(email, password))
                .Returns(new ResponseMessage
                {
                    Success = false,
                    Data = account,
                    Message = "Your account has been permanently blocked.",
                    StatusCode = (int)HttpStatusCode.Forbidden
                });

            // Act
            var result = authController.Login(new LoginDTO { Email = email, Password = password }) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(403, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Your account has been permanently blocked."));
        }
        [Test]
        public void Login_AdminSuccess_ReturnsOk()
        {
            // Arrange
            string email = "admin@example.com";
            string password = "adminPassword";
            var account = new Account
            {
                AccountID = 1,
                Email = email,
                Password = Utils.HashPassword(password),
                IsActive = true,
                Role = new Role { Name = "Admin" }
            };

            _mockRepository.Setup(repo => repo.Login(email, password))
                .Returns(new ResponseMessage { Success = true, Data = account, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = authController.Login(new LoginDTO { Email = email, Password = password }) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            var returnedAccount = responseMessage.Data as Account;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.AreEqual(account, returnedAccount);
        }
        [Test]
        public void Login_Admin_ReturnsNotFound()
        {
            // Arrange
            string email = "admin@example.com";
            string password = "adminPassword";
            var account = new Account
            {
                AccountID = 1,
                Email = email,
                Password = Utils.HashPassword(password),
                IsActive = true,
                Role = new Role { Name = "Admin" }
            };
            _mockRepository.Setup(repo => repo.Login(email, password))
                .Returns(new ResponseMessage { Success = false, Data = account, Message = "Login Fail.Account does not exist!", StatusCode = (int)HttpStatusCode.NotFound });
            var result = authController.Login(new LoginDTO { Email = email, Password = password }) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            Assert.AreEqual(404, result.StatusCode);
        }
    }

}
