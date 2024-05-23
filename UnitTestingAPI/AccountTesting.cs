using GraduationAPI_EPOSHBOOKING.Controllers.Auth;
using GraduationAPI_EPOSHBOOKING.IRepository;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable // tắt cảnh báo để code sạch hơn

namespace UnitTestingAPI
{
    public class AccountTesting
    {
        private AuthController authController;
        private Mock<IAccountRepository> repository;
        public void Setup()
        {
            
        }
    }
}
