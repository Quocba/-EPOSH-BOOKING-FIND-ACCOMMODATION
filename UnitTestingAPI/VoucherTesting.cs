using GraduationAPI_EPOSHBOOKING.Controllers.Admin;
using GraduationAPI_EPOSHBOOKING.Controllers.Customer;
using GraduationAPI_EPOSHBOOKING.Controllers.Guest;
using GraduationAPI_EPOSHBOOKING.DTO;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Http;
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
        private GeneralVoucherController generalVoucherController;
        private AdminVoucherController adminVoucherController;
        private CustomerVoucherController customerVoucherController;
        private Mock<IVoucherRepository> _mockRepository;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IVoucherRepository>();
            generalVoucherController = new GeneralVoucherController(_mockRepository.Object);
            adminVoucherController = new AdminVoucherController(_mockRepository.Object);
            customerVoucherController = new CustomerVoucherController(_mockRepository.Object);
        }

        #region GetAllVouchers Tests
        [Test]
        public void GetAllVouchers_Success_ReturnsOk()
        {
            // Arrange
            var vouchersList = new List<Voucher>
            {
                new Voucher { VoucherID = 1, Code = "DISCOUNT10", VoucherName = "Voucher 1", Discount = 0.2, QuantityUse = 1, Description = "10% off" },
                new Voucher { VoucherID = 2, Code = "DISCOUNT20", VoucherName = "Voucher 2", Discount = 0.3, QuantityUse = 2, Description = "20% off" }
            };

            _mockRepository.Setup(repo => repo.GetAllVouchers())
                .Returns(new ResponseMessage { Success = true, Data = vouchersList, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = generalVoucherController.GetAllVouchers() as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            var returnedVouchers = responseMessage.Data as List<Voucher>;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.AreEqual(vouchersList.Count, returnedVouchers.Count);

        }

        #endregion

        #region GetVoucherById Tests
        [Test]
        public void GetVoucherById_ExistingVoucher_ReturnsOk()
        {
            // Arrange
            int voucherId = 1;
            var voucher = new Voucher { VoucherID = voucherId, Code = "DISCOUNT10", VoucherName = "Voucher 1", Discount = 0.2, QuantityUse = 1, Description = "10% off" };

            _mockRepository.Setup(repo => repo.GetVoucherById(voucherId))
                .Returns(new ResponseMessage { Success = true, Data = voucher, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = generalVoucherController.GetVoucherById(voucherId) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            var returnedVoucher = responseMessage.Data as Voucher;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.AreEqual(voucher, returnedVoucher);
        }
        [Test]
        public void GetVoucherById_NonExistingVoucher_ReturnsNotFound()
        {
            // Arrange
            int voucherId = 999; // Giả sử voucherId không tồn tại
            Voucher voucher = null;

            _mockRepository.Setup(repo => repo.GetVoucherById(voucherId))
                .Returns(new ResponseMessage { Success = false, Data = voucher, Message = "Not Found", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = generalVoucherController.GetVoucherById(voucherId) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Not Found"));
            Assert.That(responseMessage.Data, Is.Null);
        }
        #endregion

        #region GetVouchersByAccountId Tests
        [Test]
        public void GetVouchersByAccountId_Success_ReturnsOk()
        {
            // Arrange
            int accountId = 1;
            var vouchersList = new List<Voucher>
            {
                new Voucher { VoucherID = 1, Code = "DISCOUNT10", VoucherName = "Voucher 1", Discount = 0.2, QuantityUse = 1, Description = "10% off" },
                new Voucher { VoucherID = 2, Code = "DISCOUNT20", VoucherName = "Voucher 2", Discount = 0.3, QuantityUse = 2, Description = "20% off" }
            };

            _mockRepository.Setup(repo => repo.GetVouchersByAccountId(accountId))
                .Returns(new ResponseMessage { Success = true, Data = vouchersList, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = customerVoucherController.GetVouchersByAccountId(accountId) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            var returnedVouchers = responseMessage.Data as List<Voucher>;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.AreEqual(vouchersList, returnedVouchers);
        }
        [Test]
        public void GetVouchersByAccountId_NoVouchers_ReturnsNotFound()
        {
            // Arrange
            int accountId = 1;
            var vouchersList = new List<Voucher>(); // Danh sách rỗng

            _mockRepository.Setup(repo => repo.GetVouchersByAccountId(accountId))
                .Returns(new ResponseMessage { Success = false, Data = vouchersList, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = customerVoucherController.GetVouchersByAccountId(accountId) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            var returnedVouchers = responseMessage.Data as List<Voucher>;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Data not found"));
            Assert.AreEqual(vouchersList, returnedVouchers); 
        }
        #endregion

        #region ReceiveVoucher Tests
        [Test]
        public void ReceiveVoucher_Success_ReturnsOk()
        {
            // Arrange
            int accountId = 1;
            int voucherId = 1;
            var voucherData = new
            {
                VoucherID = voucherId,
                VoucherName = "Voucher 1",
                Code = "DISCOUNT10",
                QuantityUsed = 1,
                Discount = 0.2,
                Description = "10% off"
            };

            _mockRepository.Setup(repo => repo.ReceiveVoucher(accountId, voucherId))
                .Returns(new ResponseMessage { Success = true, Data = voucherData, Message = "Received Voucher successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = customerVoucherController.ReceviceVoucher(accountId, voucherId) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            var returnedData = responseMessage.Data;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Received Voucher successfully"));
            Assert.AreEqual(voucherData, returnedData); // So sánh dữ liệu trả về 
        }

        [Test]
        public void ReceiveVoucher_AlreadyReceived_ReturnsOk()
        {
            // Arrange
            int accountId = 1;
            int voucherId = 1;
            var voucherData = new
            {
                VoucherID = voucherId,
                VoucherName = "Voucher 1",
                Code = "DISCOUNT10",
                QuantityUsed = 1,
                Discount = 0.2,
                Description = "10% off"
            };

            _mockRepository.Setup(repo => repo.ReceiveVoucher(accountId, voucherId))
                .Returns(new ResponseMessage { Success = true, Data = voucherData, Message = "You have already received this voucher", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = customerVoucherController.ReceviceVoucher(accountId, voucherId) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("You have already received this voucher"));
        }
        #endregion

        #region CreateVoucher Tests
        [Test]
        public void CreateVoucher_ValidData_ReturnsOk()
        {
            // Arrange
            var voucher = new Voucher
            {
                Code = "NEWCODE",
                Description = "New Voucher Description",
                Discount = 0.15,
                QuantityUse = 5,
                VoucherName = "New Voucher",
            };
            var mockFile = new Mock<IFormFile>();

            _mockRepository.Setup(repo => repo.CreateVoucher(voucher, mockFile.Object))
                .Returns(new ResponseMessage { Success = true, Data = voucher, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = adminVoucherController.CreateVoucher(voucher, mockFile.Object) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            var returnedVoucher = responseMessage.Data as Voucher;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.AreEqual(voucher, returnedVoucher); // So sánh voucher được tạo với voucher trả về
        }
        [Test]
        public void CreateVoucher_DuplicateCode_ReturnsBadRequest()
        {
            // Arrange
            var voucher = new Voucher
            {
                Code = "EXISTINGCODE", // Giả sử Code đã tồn tại
                Description = "New Voucher Description",
                Discount = 0.15,
                QuantityUse = 5,
                VoucherName = "New Voucher",
            };
            var mockFile = new Mock<IFormFile>();

            _mockRepository.Setup(repo => repo.CreateVoucher(voucher, mockFile.Object))
                .Returns(new ResponseMessage { Success = false, Data = voucher, Message = "Code already exists", StatusCode = (int)HttpStatusCode.BadRequest });

            // Act
            var result = adminVoucherController.CreateVoucher(voucher, mockFile.Object) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Code already exists"));
        }
        #endregion

        #region DeleteVoucher Tests
        [Test]
        public void DeleteVoucher_ExistingVoucher_ReturnsOk()
        {
            // Arrange
            int voucherId = 1;
            var voucher = new Voucher { VoucherID = voucherId, Code = "DISCOUNT10" }; // Chỉ cần ID và Code cho test case này

            _mockRepository.Setup(repo => repo.DeleteVoucher(voucherId))
                .Returns(new ResponseMessage { Success = true, Data = voucher, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = adminVoucherController.DeleteVoucher(voucherId) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
        }
        [Test]
        public void DeleteVoucher_NonExistingVoucher_ReturnsNotFound()
        {
            // Arrange
            int voucherId = 999; // Giả sử voucherId không tồn tại

            _mockRepository.Setup(repo => repo.DeleteVoucher(voucherId))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = adminVoucherController.DeleteVoucher(voucherId) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Data not found"));
        }
        #endregion

        #region UpdateVoucher Tests
        [Test]
        public void UpdateVoucher_ValidData_ReturnsOk()
        {
            // Arrange
            int voucherId = 1;
            var voucher = new Voucher
            {
                VoucherID = voucherId,
                Code = "UPDATEDCODE",
                Description = "Updated Voucher Description",
                Discount = 0.25,
                QuantityUse = 10,
                VoucherName = "Updated Voucher",
            };
            var mockFile = new Mock<IFormFile>();

            _mockRepository.Setup(repo => repo.UpdateVoucher(voucherId, voucher, mockFile.Object))
                .Returns(new ResponseMessage { Success = true, Data = voucher, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = adminVoucherController.UpdateVoucher(voucherId, voucher, mockFile.Object) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            var returnedVoucher = responseMessage.Data as Voucher;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.AreEqual(voucher, returnedVoucher);
        }
        [Test]
        public void UpdateVoucher_NonExistingVoucher_ReturnsNotFound()
        {
            // Arrange
            int voucherId = 999; // Giả sử voucherId không tồn tại
            var voucher = new Voucher
            {
                VoucherID = voucherId,
                Code = "UPDATEDCODE",
                Description = "Updated Voucher Description",
                Discount = 0.25,
                QuantityUse = 10,
                VoucherName = "Updated Voucher",
            };
            var mockFile = new Mock<IFormFile>();

            _mockRepository.Setup(repo => repo.UpdateVoucher(voucherId, voucher, mockFile.Object))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = adminVoucherController.UpdateVoucher(voucherId, voucher, mockFile.Object) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Data not found"));
        }

        [Test]
        public void UpdateVoucher_DuplicateCode_ReturnsBadRequest()
        {
            // Arrange
            int voucherId = 1;
            var voucher = new Voucher
            {
                VoucherID = voucherId,
                Code = "EXISTINGCODE", // Giả sử Code đã tồn tại cho voucher khác
                Description = "Updated Voucher Description",
                Discount = 0.25,
                QuantityUse = 10,
                VoucherName = "Updated Voucher",
            };
            var mockFile = new Mock<IFormFile>();

            _mockRepository.Setup(repo => repo.UpdateVoucher(voucherId, voucher, mockFile.Object))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "Code already exists", StatusCode = (int)HttpStatusCode.BadRequest });

            // Act
            var result = adminVoucherController.UpdateVoucher(voucherId, voucher, mockFile.Object) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Code already exists"));
        }
        #endregion

        #region SearchVoucherName Tests
        [Test]
        public void SearchVoucherName_Success_ReturnsOk()
        {
            // Arrange
            string voucherName = "Voucher";
            var vouchersList = new List<Voucher>
            {
                new Voucher { VoucherID = 1, Code = "DISCOUNT10", VoucherName = "Voucher 1", Discount = 0.2, QuantityUse = 1, Description = "10% off" },
                new Voucher { VoucherID = 2, Code = "DISCOUNT20", VoucherName = "Voucher 2", Discount = 0.3, QuantityUse = 2, Description = "20% off" }
            };

            _mockRepository.Setup(repo => repo.SearchVoucherName(voucherName))
                .Returns(new ResponseMessage { Success = true, Data = vouchersList, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = adminVoucherController.SearchVoucherName(voucherName) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            var returnedVouchers = responseMessage.Data as List<Voucher>;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.AreEqual(vouchersList, returnedVouchers);
        }
        [Test]
        public void SearchVoucherName_NotFound_ReturnsNotFound()
        {
            // Arrange
            string voucherName = "NonExistingVoucher";
            var vouchersList = new List<Voucher>(); // Danh sách rỗng

            _mockRepository.Setup(repo => repo.SearchVoucherName(voucherName))
                .Returns(new ResponseMessage { Success = false, Data = vouchersList, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = adminVoucherController.SearchVoucherName(voucherName) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            var returnedVouchers = responseMessage.Data as List<Voucher>;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Data not found"));
            Assert.AreEqual(vouchersList, returnedVouchers);
        }
        #endregion
    }
}