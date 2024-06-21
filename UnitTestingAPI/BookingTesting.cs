using GraduationAPI_EPOSHBOOKING.Controllers.Admin;
using GraduationAPI_EPOSHBOOKING.Controllers.Customer;
using GraduationAPI_EPOSHBOOKING.Controllers.Guest;
using GraduationAPI_EPOSHBOOKING.Controllers.Partner;
using GraduationAPI_EPOSHBOOKING.DTO;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestingAPI
{
    public class BookingTesting
    {
        private AdminBookingController AdminBookingController;
        private CustomerBookingController CustomerBookingController;
        private PartnerBookingController PartnerBookingController;
        private Mock<IBookingRepository> _mockRepository;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IBookingRepository>();
            AdminBookingController = new AdminBookingController(_mockRepository.Object);
            CustomerBookingController = new CustomerBookingController(_mockRepository.Object);
            PartnerBookingController = new PartnerBookingController(_mockRepository.Object);
        }
        private List<Account> GetFakeAccounts()
        {
            return new List<Account>
            {
                new Account { AccountID = 1, Email = "user1@example.com" },
                new Account { AccountID = 2, Email = "user2@example.com" }
            };
        }
        private List<Hotel> GetFakeHotels()
        {
            return new List<Hotel>
            {
                new Hotel { HotelID = 1, Name = "Hotel A" },
                new Hotel { HotelID = 2, Name = "Hotel B" }
            };
        }
        private List<Voucher> GetFakeVouchers()
        {
                return new List<Voucher>
            {
                new Voucher { VoucherID = 1, VoucherName = "Voucher 1" },
                new Voucher { VoucherID = 2, VoucherName = "Voucher 2" }
            };
        }

        private List<Room> GetFakeRooms()
        {
            var hotels = GetFakeHotels(); 

            return new List<Room>
            {
                new Room { RoomID = 1, TypeOfRoom = "Deluxe", Hotel = hotels[0] }, 
                new Room { RoomID = 2, TypeOfRoom = "Standard", Hotel = hotels[1] }, 
                new Room { RoomID = 3, TypeOfRoom = "Suite", Hotel = hotels[0] }  
            };
        }

        private List<Booking> GetFakeBookings()
        {
            var accounts = GetFakeAccounts();
            var vouchers = GetFakeVouchers();
            var rooms = GetFakeRooms();

            return new List<Booking>
            {
                new Booking
                {
                    BookingID = 1,
                    Account = accounts[0], 
                    Voucher = vouchers[0], 
                    Room = rooms[0],       
                    CheckInDate = DateTime.Now.AddDays(-3),
                    CheckOutDate = DateTime.Now.AddDays(-1),
                    TotalPrice = 300.0,
                    UnitPrice = 150.0,
                    TaxesPrice = 10.0,
                    NumberGuest = 2,
                    NumberOfRoom = 1,
                    Status = "Complete"
                },
                new Booking
                {
                    BookingID = 2,
                    Account = accounts[1],
                    Voucher = null,
                    Room = rooms[1],
                    CheckInDate = DateTime.Now.AddDays(5),
                    CheckOutDate = DateTime.Now.AddDays(7),
                    TotalPrice = 200.0,
                    UnitPrice = 100.0,
                    TaxesPrice = 5.0,
                    NumberGuest = 1,
                    NumberOfRoom = 2,
                    Status = "Wait For Check-In"
                },
                new Booking
                {
                    BookingID = 3,
                    Account = accounts[0],
                    Voucher = vouchers[1],
                    Room = rooms[2],
                    CheckInDate = DateTime.Now.AddDays(-10),
                    CheckOutDate = DateTime.Now.AddDays(-8),
                    TotalPrice = 500.0,
                    UnitPrice = 250.0,
                    TaxesPrice = 15.0,
                    NumberGuest = 4,
                    NumberOfRoom = 1,
                    Status = "Complete"
                }
            };
        }
        #region GetBookingByAccount Tests

        [Test]
        public void GetBookingByAccount_ValidAccountID_ReturnsOk()
        {
            // Arrange
            int accountID = 1;
            var bookings = GetFakeBookings().Where(b => b.Account.AccountID == accountID).ToList();
            _mockRepository.Setup(repo => repo.GetBookingByAccount(accountID))
                .Returns(new ResponseMessage { Success = true, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = CustomerBookingController.GetBookingByAccount(accountID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
        }

        [Test]
        public void GetBookingByAccount_NoBookingsFound_ReturnsNotFound()
        {
            // Arrange
            int accountID = 999; // Non-existent account ID

            _mockRepository.Setup(repo => repo.GetBookingByAccount(accountID))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "No Booking", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = CustomerBookingController.GetBookingByAccount(accountID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("No Booking"));
        }

        #endregion
        #region CancleBooking Tests
        [Test]
        public void CancleBooking_ValidBookingID_ReturnsOk()
        {
            // Arrange
            int bookingID = 1;
            _mockRepository.Setup(repo => repo.CancleBooking(bookingID, "Reason"))
                .Returns(new ResponseMessage { Success = true, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = CustomerBookingController.CancleBooking(bookingID, "Reason") as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
        }
        [Test]
        public void CancleBooking_InvalidBookingID_ReturnsNotFound()
        {
            // Arrange
            int bookingID = 999; // Non-existent booking ID

            _mockRepository.Setup(repo => repo.CancleBooking(bookingID, "Reason"))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "No Booking", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = CustomerBookingController.CancleBooking(bookingID, "Reason") as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("No Booking"));
        }
        [Test]
        public void CancleBooking_InvalidReason_ReturnsBadRequest()
        {
            // Arrange
            int bookingID = 1;
            string reason = ""; // Empty reason

            _mockRepository.Setup(repo => repo.CancleBooking(bookingID, reason))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "Invalid Reason", StatusCode = (int)HttpStatusCode.BadRequest });

            // Act
            var result = CustomerBookingController.CancleBooking(bookingID, reason) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Invalid Reason"));
        }
        #endregion

        #region CreateBooking Tests
        [Test]
        public void CreateBooking_ValidBooking_ReturnsOk()
        {
            // Arrange
            var newBooking = new CreateBookingDTO
            {
                AccountID = 1,
                VoucherID = 1,
                RoomID = 1,
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(3),
                NumberOfGuest = 2,
                NumberOfRoom = 1
            };

            _mockRepository.Setup(repo => repo.CreateBooking(newBooking))
                .Returns(new ResponseMessage { Success = true, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = CustomerBookingController.CreateBooking(newBooking) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
        }
        [Test]
        public void CreateBooking_InvalidBooking_ReturnsBadRequest()
        {
            // Arrange
            var newBooking = new CreateBookingDTO
            {
                AccountID = 1,
                VoucherID = 1,
                RoomID = 1,
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(-1), // Invalid check-out date
                NumberOfGuest = 2,
                NumberOfRoom = 1
            };

            _mockRepository.Setup(repo => repo.CreateBooking(newBooking))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "Invalid Booking", StatusCode = (int)HttpStatusCode.BadRequest });

            // Act
            var result = CustomerBookingController.CreateBooking(newBooking) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Invalid Booking"));
        }
        [Test]
        public void CreateBooking_InvalidAccountID_ReturnsNotFound()
        {
            // Arrange
            var newBooking = new CreateBookingDTO
            {
                AccountID = 999, // Non-existent account ID
                VoucherID = 1,
                RoomID = 1,
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(3),
                NumberOfGuest = 2,
                NumberOfRoom = 1
            };

            _mockRepository.Setup(repo => repo.CreateBooking(newBooking))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "No Account", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = CustomerBookingController.CreateBooking(newBooking) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("No Account"));
        }
        #endregion
        #region ChangeStatusWaitForPayment Tests
        public void ChangeStatusWaitForPayment_ValidBookingID_ReturnsOk()
        {
            // Arrange
            int bookingID = 1;
            _mockRepository.Setup(repo => repo.ChangeStatusWaitForPayment(bookingID))
                .Returns(new ResponseMessage { Success = true, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = PartnerBookingController.ChangeStatusWaitForPayment(bookingID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
        }
        [Test]
        public void ChangeStatusWaitForPayment_InvalidBookingID_ReturnsNotFound()
        {
            // Arrange
            int bookingID = 999; // Non-existent booking ID

            _mockRepository.Setup(repo => repo.ChangeStatusWaitForPayment(bookingID))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "No Booking", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = PartnerBookingController.ChangeStatusWaitForPayment(bookingID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("No Booking"));
        }
        #endregion

        #region ChangeStatusComplete Tests
        public void ChangeStatusComplete_ValidBookingID_ReturnsOk()
        {
            // Arrange
            int bookingID = 1;
            _mockRepository.Setup(repo => repo.ChangeStatusComplete(bookingID))
                .Returns(new ResponseMessage { Success = true, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = PartnerBookingController.ChangeStatusComplete(bookingID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
        }
        [Test]
        public void ChangeStatusComplete_InvalidBookingID_ReturnsNotFound()
        {
            // Arrange
            int bookingID = 999; // Non-existent booking ID

            _mockRepository.Setup(repo => repo.ChangeStatusComplete(bookingID))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "No Booking", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = PartnerBookingController.ChangeStatusComplete(bookingID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("No Booking"));
        }
        #endregion

        #region AnalysisRevenueBookingSystem Tests
        [Test]
        public void AnalysisRevenueBookingSystem_ValidData_ReturnsOk()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.AnalysisRevenueBookingSystem())
                .Returns(new ResponseMessage { Success = true, Data = 1000.0, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = AdminBookingController.AnalysisRevenueBookingSystem() as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.That(responseMessage.Data, Is.EqualTo(1000.0));
        }
        [Test]
        public void AnalysisRevenueBookingSystem_NoDataFound_ReturnsNotFound()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.AnalysisRevenueBookingSystem())
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "No Data", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = AdminBookingController.AnalysisRevenueBookingSystem() as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("No Data"));
        }
        #endregion

        #region AnalysisRevenueBookingHotel Tests
        [Test]
        public void AnalysisRevenueBookingHotel_ValidHotelID_ReturnsOk()
        {
            // Arrange
            int hotelID = 1;
            _mockRepository.Setup(repo => repo.AnalysisRevenueBookingHotel(hotelID))
                .Returns(new ResponseMessage { Success = true, Data = 500.0, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = PartnerBookingController.AnalysisRevenueBookingHotel(hotelID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.That(responseMessage.Data, Is.EqualTo(500.0));
        }
        [Test]
        public void AnalysisRevenueBookingHotel_NoDataFound_ReturnsNotFound()
        {
            // Arrange
            int hotelID = 999; // Non-existent hotel ID

            _mockRepository.Setup(repo => repo.AnalysisRevenueBookingHotel(hotelID))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "No Data", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = PartnerBookingController.AnalysisRevenueBookingHotel(hotelID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("No Data"));
        }
        #endregion

        #region CountBookingSystem Tests
        [Test]
        public void CountBookingSystem_ValidData_ReturnsOk()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.CountBookingSystem())
                .Returns(new ResponseMessage { Success = true, Data = 10, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = AdminBookingController.CountBookingSystem() as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.That(responseMessage.Data, Is.EqualTo(10));
        }
        [Test]
        public void CountBookingSystem_NoDataFound_ReturnsNotFound()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.CountBookingSystem())
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "No Data", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = AdminBookingController.CountBookingSystem() as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("No Data"));
        }
        #endregion

        #region CountBookingHotel Tests
        [Test]
        public void CountBookingHotel_ValidHotelID_ReturnsOk()
        {
            // Arrange
            int hotelID = 1;
            _mockRepository.Setup(repo => repo.CountBookingHotel(hotelID))
                .Returns(new ResponseMessage { Success = true, Data = 5, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = PartnerBookingController.CountBookingHotel(hotelID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.That(responseMessage.Data, Is.EqualTo(5));
        }
        [Test]
        public void CountBookingHotel_NoDataFound_ReturnsNotFound()
        {
            // Arrange
            int hotelID = 999; // Non-existent hotel ID

            _mockRepository.Setup(repo => repo.CountBookingHotel(hotelID))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "No Data", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = PartnerBookingController.CountBookingHotel(hotelID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("No Data"));
        }
        #endregion

        #region Top5Booking Tests
        [Test]
        public void Top5Booking_ValidData_ReturnsOk()
        {
            // Arrange
            var bookings = GetFakeBookings().OrderByDescending(b => b.TotalPrice).Take(5).ToList();
            _mockRepository.Setup(repo => repo.Top5Booking())
                .Returns(new ResponseMessage { Success = true, Data = bookings, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = AdminBookingController.Top5Booking() as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.That(responseMessage.Data, Is.EqualTo(bookings));
        }
        [Test]
        public void Top5Booking_NoDataFound_ReturnsNotFound()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.Top5Booking())
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "No Data", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = AdminBookingController.Top5Booking() as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("No Data"));
        }
        #endregion

        #region GetBookingByHotel Tests
        [Test]
        public void GetBookingByHotel_ValidHotelID_ReturnsOk()
        {
            // Arrange
            int hotelID = 1;
            var bookings = GetFakeBookings().Where(b => b.Room.Hotel.HotelID == hotelID).ToList();
            _mockRepository.Setup(repo => repo.GetBookingByHotel(hotelID))
                .Returns(new ResponseMessage { Success = true, Data = bookings, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = PartnerBookingController.GetBookingByHotel(hotelID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.That(responseMessage.Data, Is.EqualTo(bookings));
        }
        [Test]
        public void GetBookingByHotel_NoBookingsFound_ReturnsNotFound()
        {
            // Arrange
            int hotelID = 999; // Non-existent hotel ID

            _mockRepository.Setup(repo => repo.GetBookingByHotel(hotelID))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "No Booking", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = PartnerBookingController.GetBookingByHotel(hotelID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("No Booking"));
        }
        #endregion


        #region CheckRoomPrice Tests

        [Test]
        public void CheckRoomPrice_ValidData_ReturnsOk()
        {
            // đéo test nữa
        }

        #endregion

    }
}
