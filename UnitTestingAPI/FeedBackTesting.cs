using GraduationAPI_EPOSHBOOKING.Controllers.Admin;
using GraduationAPI_EPOSHBOOKING.Controllers.Customer;
using GraduationAPI_EPOSHBOOKING.Controllers.Guest;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace UnitTestingAPI
{
    [TestFixture]
    public class FeedBackTesting
    {
        private CustomerFeedbackController CustomerFeedbackController;
        private GeneralFeedbackController GeneralFeedbackController;
        private Mock<IFeedbackRepository> _mockRepository;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IFeedbackRepository>();
            CustomerFeedbackController = new CustomerFeedbackController(_mockRepository.Object);
            GeneralFeedbackController = new GeneralFeedbackController(_mockRepository.Object);

        }

        private List<FeedBack> GetFakeFeedbacks()
        {
            return new List<FeedBack>
            {
                new FeedBack
                {
                    FeedBackID = 1,
                    Rating = 4,
                    Description = "Khách sạn tốt, dịch vụ chu đáo.",
                    isDeleted = false,
                    Account = new Account { AccountID = 1, Email = "user1@example.com" },
                    Booking = new Booking { BookingID = 1, CheckInDate = DateTime.Now.AddDays(-5), CheckOutDate = DateTime.Now.AddDays(-2) },
                    Hotel = new Hotel { HotelID = 1, Name = "Hotel A" }
                },
                new FeedBack
                {
                    FeedBackID = 2,
                    Rating = 5,
                    Description = "Rất hài lòng về khách sạn.",
                    isDeleted = false,
                    Account = new Account { AccountID = 2, Email = "user2@example.com" },
                    Booking = new Booking { BookingID = 2, CheckInDate = DateTime.Now.AddDays(-10), CheckOutDate = DateTime.Now.AddDays(-7) },
                    Hotel = new Hotel { HotelID = 2, Name = "Hotel A" }
                },
                new FeedBack
                {
                    FeedBackID = 3,
                    Rating = 3,
                    Description = "Khách sạn tạm được, giá hơi cao.",
                    isDeleted = false,
                    Account = new Account { AccountID = 3, Email = "user3@example.com" },
                    Booking = new Booking { BookingID = 3, CheckInDate = DateTime.Now.AddDays(-3), CheckOutDate = DateTime.Now },
                    Hotel = new Hotel { HotelID = 3, Name = "Hotel B" }
                }
            };
        }

        private Booking GetFakeBooking(int bookingID)
        {
            return new Booking
            {
                BookingID = bookingID,
                CheckInDate = DateTime.Now.AddDays(-5),
                CheckOutDate = DateTime.Now.AddDays(-2),
                Account = new Account { AccountID = 1, Email = "user1@example.com" },
                Room = new Room { RoomID = 1, Hotel = new Hotel { HotelID = 1, Name = "Hotel A" } }
            };
        }

        [Test]
        public void GetallFeedbacksByHotelID_Success()
        {
            // Arrange
            int hotelID = 1;
            var fakeFeedbacks = GetFakeFeedbacks();
            var expectedFeedbacks = fakeFeedbacks.Where(fb => fb.Hotel.HotelID == hotelID).ToList();

            _mockRepository.Setup(repo => repo.GetAllFeedbackHotel(hotelID))
                .Returns(new ResponseMessage { Success = true, Data = expectedFeedbacks, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = GeneralFeedbackController.GetFeedbackByHotel(hotelID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(responseMessage.Success);
            Assert.AreEqual("Successfully", responseMessage.Message);
            Assert.AreEqual(expectedFeedbacks, responseMessage.Data);
        }

        [Test]
        public void GetallFeedbacksByHotelID_NotFound()
        {
            // Arrange
            int hotelID = 999; // Non-existent Hotel ID
            var fakeFeedbacks = GetFakeFeedbacks();
            var expectedFeedbacks = fakeFeedbacks.Where(fb => fb.Hotel.HotelID == hotelID).ToList(); 

            _mockRepository.Setup(repo => repo.GetAllFeedbackHotel(hotelID))
                .Returns(new ResponseMessage { Success = false, Data = expectedFeedbacks, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = GeneralFeedbackController.GetFeedbackByHotel(hotelID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsFalse(responseMessage.Success);
            Assert.AreEqual("Data not found", responseMessage.Message);
        }

        [Test]
        public void CreateFeedBack_Success()
        {
            // Arrange
            int bookingID = 1;
            var fakeBooking = GetFakeBooking(bookingID);
            var feedBack = new FeedBack
            {
                Rating = 5,
                Description = "Feedback content",
                isDeleted = false
            };
            var mockImage = new Mock<IFormFile>();

            _mockRepository.Setup(repo => repo.CreateFeedBack(bookingID, feedBack, mockImage.Object))
                .Returns(new ResponseMessage { Success = true, Data = feedBack, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = CustomerFeedbackController.CreateFeedBack(bookingID, feedBack, mockImage.Object) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(responseMessage.Success);
            Assert.AreEqual("Successfully", responseMessage.Message);
            Assert.AreEqual(feedBack, responseMessage.Data);
        }

        [Test]
        public void CreateFeedBack_BookingNotFound()
        {
            // Arrange
            int bookingID = 999;
            var feedBack = new FeedBack {  };
            var mockImage = new Mock<IFormFile>();

            _mockRepository.Setup(repo => repo.CreateFeedBack(bookingID, feedBack, mockImage.Object))
                .Returns(new ResponseMessage { Success = false, Message = "Booking not found", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = CustomerFeedbackController.CreateFeedBack(bookingID, feedBack, mockImage.Object) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsFalse(responseMessage.Success);
            Assert.AreEqual("Booking not found", responseMessage.Message);
        }
    }
}