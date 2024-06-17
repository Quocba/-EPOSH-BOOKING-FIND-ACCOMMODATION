using Castle.Components.DictionaryAdapter.Xml;
using GraduationAPI_EPOSHBOOKING.Controllers.Auth;
using GraduationAPI_EPOSHBOOKING.DTO;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using static GraduationAPI_EPOSHBOOKING.Controllers.Auth.AuthController;
#pragma warning disable // tắt cảnh báo để code sạch hơn

namespace UnitTestingAPI
{
    public class AccountTesting
    {
        private AuthController authController;
        private Mock<IAccountRepository> _mockRepository;
        IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IAccountRepository>();
            authController = new AuthController(_mockRepository.Object,_configuration);
        }

        [Test]
        public void LoginwithNumberPhone()
        {
            
            String phone = "0923343536";
            _mockRepository.Setup(repo => repo.LoginWithNumberPhone(phone))
                .Returns(new ResponseMessage { Success = true, Data = phone, StatusCode = (int)HttpStatusCode.OK });

            
            var result = authController.LoginPhone(phone) as ObjectResult;

            
            Assert.AreEqual(200, result.StatusCode);

        }

        [Test]
        public void LoginwithNumberPhoneFailed()
        {
            
            String phone = "0923343536";
            String phone2 = "012345689";
            String phone3 = "anhyeuem";
            String phone4 = "$%$%";
            String phone5 = "";
            _mockRepository.Setup(repo => repo.LoginWithNumberPhone(phone))
                .Returns(new ResponseMessage { Success = false, Data = phone, StatusCode = (int)HttpStatusCode.BadRequest });
            _mockRepository.Setup(repo => repo.LoginWithNumberPhone(phone2))
            .Returns(new ResponseMessage { Success = false, Data = phone2, StatusCode = (int)HttpStatusCode.BadRequest });
            _mockRepository.Setup(repo => repo.LoginWithNumberPhone(phone3))
                .Returns(new ResponseMessage { Success = false, Data = phone3, StatusCode = (int)HttpStatusCode.BadRequest });
            _mockRepository.Setup(repo => repo.LoginWithNumberPhone(phone4))
                .Returns(new ResponseMessage { Success = false, Data = phone4, StatusCode = (int)HttpStatusCode.BadRequest });
            _mockRepository.Setup(repo => repo.LoginWithNumberPhone(phone5))
                .Returns(new ResponseMessage { Success = false, Data = phone5, StatusCode = (int)HttpStatusCode.BadRequest });
            
            var result = authController.LoginPhone(phone) as ObjectResult;
            var result2 = authController.LoginPhone(phone2) as ObjectResult;
            var result3 = authController.LoginPhone(phone3) as ObjectResult;
            var result4 = authController.LoginPhone(phone4) as ObjectResult;
            var result5 = authController.LoginPhone(phone5) as ObjectResult;
            
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual(400, result2.StatusCode);
            Assert.AreEqual(400, result3.StatusCode);
            Assert.AreEqual(400, result4.StatusCode);
            Assert.AreEqual(400, result5.StatusCode);

        }
        [Test]
        public void LoginwithNumberPhoneNotFound()
        {
            
            String phone = "0923343536";
            _mockRepository.Setup(repo => repo.LoginWithNumberPhone(phone))
                .Returns(new ResponseMessage { Success = false, Data = phone, StatusCode = (int)HttpStatusCode.NotFound });

            
            var result = authController.LoginPhone(phone) as ObjectResult;

            
            Assert.AreEqual(404, result.StatusCode);
        }
        [Test]
        public void LoginwithNumberPhoneExist()
        {
            
            String phone = "0923343536";
            _mockRepository.Setup(repo => repo.LoginWithNumberPhone(phone))
                .Returns(new ResponseMessage { Success = false, Data = phone, StatusCode = (int)HttpStatusCode.AlreadyReported });

            
            var result = authController.LoginPhone(phone) as ObjectResult;

            
            Assert.AreEqual(208, result.StatusCode);
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
    }
    
}
