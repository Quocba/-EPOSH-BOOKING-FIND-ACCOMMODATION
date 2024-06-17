using GraduationAPI_EPOSHBOOKING.Controllers.Guest;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
#pragma warning disable // tắt cảnh báo để code sạch hơn
namespace UnitTestingAPI
{
    [TestFixture]
    public class VoucherTesting
    {
        private GeneralVoucherController voucherController;
        private Mock<IVoucherRepository> _mockRepository;

        [SetUp] 
        public void Setup()
        {
            _mockRepository = new Mock<IVoucherRepository>();
            voucherController = new GeneralVoucherController(_mockRepository.Object);
        }

        [Test]
        public void GetAllVouchers()
        {
            // Arrange
            List<Voucher> vouchersList = new List<Voucher>
            {
                new Voucher { VoucherID = 1, Code = "DISCOUNT10", VoucherName = "Voucher 1", Discount = 0.2, QuantityUse = 1, Description = "10% off" },
                new Voucher { VoucherID = 2, Code = "DISCOUNT20", VoucherName = "Voucher 2", Discount = 0.3, QuantityUse = 2, Description = "20% off" }
            };

            _mockRepository.Setup(repo => repo.GetAllVouchers())
                .Returns(new ResponseMessage { Success = true, Data = vouchersList, StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = voucherController.GetAllVouchers() as ObjectResult;
            // Assert
            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void GetNullVoucher()
        {
            // Arrange
            List<Voucher> vouchersList = new List<Voucher>();

            _mockRepository.Setup(repo => repo.GetAllVouchers())
                .Returns(new ResponseMessage { Success = false, Data = vouchersList, StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = voucherController.GetAllVouchers() as ObjectResult;
            // Assert
            Assert.AreEqual(404, result.StatusCode);
        }
        [Test]
        public void GetException()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllVouchers())
                .Returns(new ResponseMessage { Success = false, Data = null, StatusCode = (int)HttpStatusCode.InternalServerError });

            // Act
            var result = voucherController.GetAllVouchers() as ObjectResult;
            // Assert
            Assert.AreEqual(500, result.StatusCode);
        }

    }
}
