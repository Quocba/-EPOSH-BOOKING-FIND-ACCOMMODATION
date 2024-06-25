using GraduationAPI_EPOSHBOOKING.Controllers.Admin;
using GraduationAPI_EPOSHBOOKING.Controllers.Guest;
using GraduationAPI_EPOSHBOOKING.Controllers.Partner;
using GraduationAPI_EPOSHBOOKING.DTO;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable // tắt cảnh báo để code sạch hơn

namespace UnitTestingAPI
{

    [TestFixture]
    public class HotelTesting
    {
        public GeneralHotelController controller { get; set; }
        public PartnerHotelController partnerHotelController { get; set; }
        public AdminHotelController adminHotelController { get; set; }
        private Mock<IHotelRepository> repository;
        [SetUp]
        public void SetUp()
        {
            repository = new Mock<IHotelRepository>();
            controller = new GeneralHotelController(repository.Object);
            partnerHotelController = new PartnerHotelController(repository.Object);
            adminHotelController = new AdminHotelController(repository.Object);
        }

        [Test]
        public void GetAllHotelSuccessfully()
        {
            var fakeHotels = GetFakeHotels();
            repository.Setup(repository => repository.GetAllHotel())
                .Returns(new ResponseMessage { Success = true, Data = fakeHotels, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });
            var result = controller.GetAllHotel() as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void GetAllHotelNoData()
        {
            repository.Setup(repository => repository.GetAllHotel())
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "No Data", StatusCode = (int)HttpStatusCode.NotFound });
            var result = controller.GetAllHotel() as ObjectResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void GetHotelByCitySuccess()
        {
            String city = "Can Tho";
            var fakeHotel = GetFakeHotels();
            repository.Setup(repository => repository.GetHotelByCity(city))
                .Returns(new ResponseMessage { Success = true, Data = fakeHotel, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });
            var result = controller.GetHotelByCity(city) as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);

        }

        [Test]
        public void GetHotelByCityNotFound()
        {
            String city = "Can Tho";
            var fakeHotel = GetFakeHotels();
            var expectedHotel = fakeHotel.FirstOrDefault(hotel => hotel.HotelAddress.City.Equals(city));

            repository.Setup(repository => repository.GetHotelByCity(city))
                .Returns(new ResponseMessage { Success = true, Data = expectedHotel, Message = "Successfully", StatusCode = (int)HttpStatusCode.NotFound });
            var result = controller.GetHotelByCity(city) as ObjectResult;
            Assert.AreEqual(404, result.StatusCode);

        }

        [Test]
        public void GetHotelByIDSuccess()
        {
            int hotelID = 1;
            var fakeHotel = GetFakeHotels();
            var expectedHotel = fakeHotel.FirstOrDefault(h => h.HotelID == hotelID);
            repository.Setup(repository => repository.GetHotelByID(hotelID))
                .Returns(new ResponseMessage { Success = true, Data = expectedHotel, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });
            var result = controller.GetHotelByID(hotelID) as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void GetHotelByIDNotFound()
        {
            int hotelID = 100;
            var fakeHotel = GetFakeHotels();
            var expectedHotel = fakeHotel.FirstOrDefault(h => h.HotelID == hotelID);
            repository.Setup(repository => repository.GetHotelByID(hotelID))
                .Returns(new ResponseMessage { Success = false, Data = expectedHotel, Message = "Successfully", StatusCode = (int)HttpStatusCode.NotFound });
            var result = controller.GetHotelByID(hotelID) as ObjectResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void GetHotelByPriceSuccess()
        {
            double minPrice = 100;
            double maxPrice = 150;
            String address = "Ho Chi Minh";
            var fakeHotel = GetFakeHotels();
            var expectedResult = fakeHotel.Select(hotel => new
            {
                Hotel = hotel,
                AvgRating = hotel.feedBacks.Any() ? Math.Round(hotel.feedBacks.Average(feedback => feedback.Rating), 2) : 0
            }).ToList();
            repository.Setup(repository => repository.GetHotelByPrice(address, minPrice, maxPrice))
                .Returns(new ResponseMessage { Success = true, Data = expectedResult, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });
            var result = controller.getHotelByPrice(address, minPrice, maxPrice) as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void GetHotelByPriceNotFound()
        {
            double minPrice = 200;
            double maxPrice = 500;
            String address = "Ho Chi Minh";

            repository.Setup(repository => repository.GetHotelByPrice(address, minPrice, maxPrice))
                      .Returns(new ResponseMessage { Success = false, Data = null, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound });

            var result = controller.getHotelByPrice(address, minPrice, maxPrice) as ObjectResult;
            Assert.AreEqual(404, result.StatusCode);
        }
        [Test]
        public void GetGalleriesByHotelID_Success()
        {
            // Arrange
            int hotelID = 1;
            var fakeHotels = GetFakeHotels();
            var expectedGalleries = fakeHotels.FirstOrDefault(h => h.HotelID == hotelID)?.HotelImages;

            repository.Setup(repo => repo.GetGalleriesByHotelID(hotelID))
                .Returns(new ResponseMessage
                {
                    Success = true,
                    Data = expectedGalleries,
                    Message = "Successfully",
                    StatusCode = (int)HttpStatusCode.OK
                });

            // Act
            var result = partnerHotelController.GetGalleriesByHotel(hotelID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            var returnedGalleries = responseMessage.Data as List<HotelImage>;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.AreEqual(expectedGalleries, returnedGalleries);
        }
        [Test]
        public void GetGalleriesByHotelID_NoData_ReturnsNotFound()
        {
            // Arrange
            int hotelID = 1;

            repository.Setup(repo => repo.GetGalleriesByHotelID(hotelID))
                .Returns(new ResponseMessage
                {
                    Success = false,
                    Data = null,
                    Message = "No data",
                    StatusCode = (int)HttpStatusCode.NotFound
                });

            // Act
            var result = partnerHotelController.GetGalleriesByHotel(hotelID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("No data"));
            Assert.That(responseMessage.Data, Is.Null);
        }
        [Test]
        public void GetHotelByRateSuccess()
        {
            int rating = 5;
            var fakeHotel = GetFakeHotels();
            var filterHotelRating = fakeHotel.OrderByDescending(hotel => hotel.HotelStandar).Select(hotel => new
            {
                Hotel = hotel,
                AvgRating = hotel.feedBacks.Any() ? Math.Round(hotel.feedBacks.Average(fb => fb.Rating), 2) : 0
            });

            var expected = filterHotelRating.Where(h => (int)h.AvgRating == rating).ToList();
            repository.Setup(repository => repository.GetByHotelStandar(rating))
                .Returns(new ResponseMessage { Success = true, Data = expected, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });
            var result = controller.getHotelByRating(rating) as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);

        }

        [Test]
        public void GetHotelByRateNotFound()
        {
            int rating = 6;
            var fakeHotel = GetFakeHotels();
            var filterHotelRating = fakeHotel.OrderByDescending(hotel => hotel.HotelStandar).Select(hotel => new
            {
                Hotel = hotel,
                AvgRating = hotel.feedBacks.Any() ? Math.Round(hotel.feedBacks.Average(fb => fb.Rating), 2) : 0
            });

            var expected = filterHotelRating.Where(h => (int)h.AvgRating == rating).ToList();
            repository.Setup(repository => repository.GetByHotelStandar(rating))
                .Returns(new ResponseMessage { Success = true, Data = expected, Message = "Data Not Found", StatusCode = (int)HttpStatusCode.NotFound });
            var result = controller.getHotelByRating(rating) as ObjectResult;
            Assert.AreEqual(404, result.StatusCode);

        }


        [Test]
        public void GetByServiceSuccess()
        {
            List<string> services = new List<string>();
            String serviceTyp1 = "Breakfast";
            String serviceTyp2 = "Gym";
            services.Add(serviceTyp1);
            services.Add(serviceTyp2);
            var fakeHotel = GetFakeHotels();
            var expected = fakeHotel.Where(hotel => hotel.HotelServices.Any(service => services.Contains(service.Type))).ToList();
            repository.Setup(repository => repository.GetByService(services))
                .Returns(new ResponseMessage { Success = true, Data = expected, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });
            var result = controller.GetHotelByService(services) as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void GetByServiceNotFound()
        {
            List<string> services = new List<string>();
            String serviceTyp1 = "ABC";
            String serviceTyp2 = "DGH";
            services.Add(serviceTyp1);
            services.Add(serviceTyp2);
            var fakeHotel = GetFakeHotels();
            var expected = fakeHotel.Where(hotel => hotel.HotelServices.Any(service => services.Contains(service.Type))).ToList();
            repository.Setup(repository => repository.GetByService(services))
                .Returns(new ResponseMessage { Success = true, Data = expected, Message = "Successfully", StatusCode = (int)HttpStatusCode.NotFound });
            var result = controller.GetHotelByService(services) as ObjectResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void GetServiceByHotelIDSuccess()
        {
            int hotelID = 1;
            var fakeHotel = GetFakeHotels();
            var expected = fakeHotel.Where(hotel => hotel.HotelServices.Any(service => service.Hotel.HotelID == hotelID))
                .ToList();
            repository.Setup(repository => repository.GetServiceByHotelID(hotelID))
                .Returns(new ResponseMessage { Success = true, Data = expected, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });
            var result = controller.GetServiceByHotel(hotelID) as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void GetServiceByHotelIDNotFound()
        {
            int hotelID = 5;
            var fakeHotel = GetFakeHotels();
            var expected = fakeHotel.Where(hotel => hotel.HotelServices.Any(service => service.Hotel.HotelID == hotelID))
                .ToList();
            repository.Setup(repository => repository.GetServiceByHotelID(hotelID))
                .Returns(new ResponseMessage { Success = true, Data = expected, Message = "Not Found", StatusCode = (int)HttpStatusCode.NotFound });
            var result = controller.GetServiceByHotel(hotelID) as ObjectResult;
            Assert.AreEqual(404, result.StatusCode);
        }


        [Test]
        public void SearchHotel_ByCityOnly_ReturnsCorrectResults()
        {

            string city = "CityA";
            var fakeHotels = GetFakeHotels();
            var expected = fakeHotels.Where(h => h.HotelAddress.City.Equals(city, StringComparison.OrdinalIgnoreCase)).ToList();

            repository.Setup(repo => repo.SearchHotel(city, null, null, null, null))
                          .Returns(new ResponseMessage { Success = true, Data = expected, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });


            var result = controller.SearchHotel(city, null, null, null, null) as ObjectResult;


            Assert.IsNotNull(result, "Result should not be null");
            Assert.AreEqual(200, result.StatusCode);
            var responseMessage = result.Value as ResponseMessage;
            Assert.IsNotNull(responseMessage, "Response message should not be null");
            Assert.IsTrue(responseMessage.Success);
            Assert.AreEqual("Successfully", responseMessage.Message);
            Assert.AreEqual(expected, responseMessage.Data);
        }


        public void SearchHotel_ByCityAndDates_ReturnsCorrectResults()
        {

            string city = "CityA";
            DateTime checkInDate = DateTime.Today;
            DateTime checkOutDate = DateTime.Today.AddDays(1);
            var fakeHotels = GetFakeHotels();
            var expected = fakeHotels
                            .Where(h => h.HotelAddress.City.Equals(city, StringComparison.OrdinalIgnoreCase) &&
                                        h.rooms.Any(r => r.SpecialPrice.Any(sp => sp.StartDate <= checkInDate && sp.EndDate >= checkOutDate)))
                            .Select(h => new
                            {
                                Hotel = h,
                                RoomSpecialPrice = h.rooms.Where(r => r.SpecialPrice.Any(sp => sp.StartDate <= checkInDate && sp.EndDate >= checkOutDate))
                                                          .Select(r => new { r.RoomID, Price = r.SpecialPrice.First().Price }).ToList(),
                                AvgRating = h.feedBacks.Any() ? h.feedBacks.Average(fb => fb.Rating) : 0,
                                CountReview = h.feedBacks.Count
                            }).ToList();

            repository.Setup(repo => repo.SearchHotel(city, checkInDate, checkOutDate, null, null))
                          .Returns(new ResponseMessage { Success = true, Data = expected, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });


            var result = controller.SearchHotel(city, checkInDate, checkOutDate, null, null) as ObjectResult;


            Assert.IsNotNull(result, "Result should not be null");
            Assert.AreEqual(200, result.StatusCode);
            var responseMessage = result.Value as ResponseMessage;
            Assert.IsNotNull(responseMessage, "Response message should not be null");
            Assert.IsTrue(responseMessage.Success);
            Assert.AreEqual("Successfully", responseMessage.Message);
            Assert.AreEqual(expected, responseMessage.Data);
        }


        [Test]
        public void SearchHotel_ByCityCapacityQuantity_ReturnsCorrectResults()
        {

            string city = "CityA";
            int numberCapacity = 2;
            int quantity = 1;
            var fakeHotels = GetFakeHotels();
            var expected = fakeHotels
                            .Where(h => h.HotelAddress.City.Equals(city, StringComparison.OrdinalIgnoreCase) &&
                                        h.rooms.Any(r => r.NumberCapacity >= numberCapacity && r.Quantity >= quantity))
                            .Select(h => new
                            {
                                Hotel = h,
                                AvgRating = h.feedBacks.Any() ? h.feedBacks.Average(fb => fb.Rating) : 0,
                                CountReview = h.feedBacks.Count
                            }).ToList();

            repository.Setup(repo => repo.SearchHotel(city, null, null, numberCapacity, quantity))
                          .Returns(new ResponseMessage { Success = true, Data = expected, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });


            var result = controller.SearchHotel(city, null, null, numberCapacity, quantity) as ObjectResult;


            Assert.IsNotNull(result, "Result should not be null");
            Assert.AreEqual(200, result.StatusCode);
            var responseMessage = result.Value as ResponseMessage;
            Assert.IsNotNull(responseMessage, "Response message should not be null");
            Assert.IsTrue(responseMessage.Success);
            Assert.AreEqual("Successfully", responseMessage.Message);
            Assert.AreEqual(expected, responseMessage.Data);
        }

        [Test]
        public void SearchHotel_ByAllParameters_ReturnsCorrectResults()

        {
            string city = "CityA";
            DateTime checkInDate = DateTime.Today;
            DateTime checkOutDate = DateTime.Today.AddDays(1);
            int numberCapacity = 2;
            int quantity = 1;
            var fakeHotels = GetFakeHotels();
            var expected = fakeHotels
                            .Where(h => h.HotelAddress.City.Equals(city, StringComparison.OrdinalIgnoreCase) &&
                                        h.rooms.Any(r => r.SpecialPrice.Any(sp => sp.StartDate <= checkInDate && sp.EndDate >= checkOutDate) &&
                                                         r.NumberCapacity >= numberCapacity &&
                                                         r.Quantity >= quantity))
                            .Select(h => new
                            {
                                Hotel = h,
                                AvgRating = h.feedBacks.Any() ? h.feedBacks.Average(fb => fb.Rating) : 0,
                                CountReview = h.feedBacks.Count,
                                RoomSpecialPrice = h.rooms.Where(r => r.SpecialPrice.Any(sp => sp.StartDate <= checkInDate && sp.EndDate >= checkOutDate))
                                                          .Select(r => new { r.RoomID, Price = r.SpecialPrice.First().Price }).ToList()
                            }).ToList();

            repository.Setup(repo => repo.SearchHotel(city, checkInDate, checkOutDate, numberCapacity, quantity))
                          .Returns(new ResponseMessage { Success = true, Data = expected, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });


            var result = controller.SearchHotel(city, checkInDate, checkOutDate, numberCapacity, quantity) as ObjectResult;


            Assert.IsNotNull(result, "Result should not be null");
            Assert.AreEqual(200, result.StatusCode);
            var responseMessage = result.Value as ResponseMessage;
            Assert.IsNotNull(responseMessage, "Response message should not be null");
            Assert.IsTrue(responseMessage.Success);
            Assert.AreEqual("Successfully", responseMessage.Message);
            Assert.AreEqual(expected, responseMessage.Data);
        }
        [Test]
        public void SearchHotel_NoData_ReturnsNotFound()
        {
            string city = "CityA";
            DateTime checkInDate = DateTime.Today;
            DateTime checkOutDate = DateTime.Today.AddDays(1);
            int numberCapacity = 2;
            int quantity = 1;

            repository.Setup(repo => repo.SearchHotel(city, checkInDate, checkOutDate, numberCapacity, quantity))
                          .Returns(new ResponseMessage { Success = false, Data = null, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound });


            var result = controller.SearchHotel(city, checkInDate, checkOutDate, numberCapacity, quantity) as ObjectResult;


            Assert.IsNotNull(result, "Result should not be null");
            Assert.AreEqual(404, result.StatusCode);
            var responseMessage = result.Value as ResponseMessage;
            Assert.IsNotNull(responseMessage, "Response message should not be null");
            Assert.IsFalse(responseMessage.Success);
            Assert.AreEqual("Data not found", responseMessage.Message);
        }

        [Test]
        public void HotelRegistration_ValidData_ReturnsOk()
        {
            // Arrange
            var registrationModel = new HotelRegistrationDTO
            {
                HotelName = "New Hotel",
                OpenedIn = 2021,
                Description = "New Description",
                HotelAddress = "New Address",
                City = "New City",
                Latitude = 12.34,
                Longitude = 56.78,
                MainImage = new Mock<IFormFile>().Object,
                Services = JsonConvert.SerializeObject(new List<ServiceTypeDTO>
                {
                    new ServiceTypeDTO { serviceType = "Restaurant", subServiceName = new List<string> { "Buffet", "Fine Dining" } },
                    new ServiceTypeDTO { serviceType = "Fitness Center", subServiceName = new List<string> { "Gym", "Yoga" } }
                })
            };

            repository.Setup(repo => repo.HotelRegistration(registrationModel, It.IsAny<List<ServiceTypeDTO>>()))
                .Returns(new ResponseMessage
                {
                    Success = true,
                    Data = It.IsAny<object>(), // Chỉ cần kiểm tra kiểu dữ liệu
                    Message = "Successfully",
                    StatusCode = (int)HttpStatusCode.OK
                });

            // Act
            var result = partnerHotelController.RegisterHotel(registrationModel) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
        }
        [Test]
        public void HotelRegistration_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var registrationModel = new HotelRegistrationDTO
            {
                HotelName = "New Hotel",
                OpenedIn = 2021,
                Description = "New Description",
                HotelAddress = "New Address",
                City = "New City",
                Latitude = 12.34,
                Longitude = 56.78,
                MainImage = new Mock<IFormFile>().Object,
                Services = JsonConvert.SerializeObject(new List<ServiceTypeDTO>
                {
                    new ServiceTypeDTO { serviceType = "Restaurant", subServiceName = new List<string> { "Buffet", "Fine Dining" } },
                    new ServiceTypeDTO { serviceType = "Fitness Center", subServiceName = new List<string> { "Gym", "Yoga" } }
                })
            };

            repository.Setup(repo => repo.HotelRegistration(registrationModel, It.IsAny<List<ServiceTypeDTO>>()))
                .Returns(new ResponseMessage
                {
                    Success = false,
                    Data = null,
                    Message = "Invalid data",
                    StatusCode = (int)HttpStatusCode.BadRequest
                });

            // Act
            var result = partnerHotelController.RegisterHotel(registrationModel) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Invalid data"));
            Assert.That(responseMessage.Data, Is.Null);
        }
        [Test]
        public void UpdateBasicInformation_ValidData_ReturnsOk()
        {
            // Arrange
            int hotelID = 1;
            string hotelName = "Updated Hotel Name";
            int openedIn = 2022;
            string description = "Updated Description";
            string hotelAddress = "Updated Address";
            string city = "Updated City";
            double latitude = 12.34;
            double longitude = 56.78;
            var mainImage = new Mock<IFormFile>().Object;

            repository.Setup(repo => repo.UpdateBasicInformation(hotelID, hotelName, openedIn, description,
                                                                 hotelAddress, city, latitude, longitude, mainImage))
                .Returns(new ResponseMessage
                {
                    Success = true,
                    Data = It.IsAny<object>(), // Chỉ cần kiểm tra kiểu dữ liệu
                    Message = "Successfully",
                    StatusCode = (int)HttpStatusCode.OK
                });

            // Act
            var result = partnerHotelController.UpdateBasicInfomation(hotelID, hotelName, openedIn, description,
                                                                     hotelAddress, city, latitude, longitude, mainImage) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Successfully", responseMessage.Message);
        }
        [Test]
        public void UpdateBasicInformation_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            int hotelID = 1;
            string hotelName = "Updated Hotel Name";
            int openedIn = 2022;
            string description = "Updated Description";
            string hotelAddress = "Updated Address";
            string city = "Updated City";
            double latitude = 12.34;
            double longitude = 56.78;
            var mainImage = new Mock<IFormFile>().Object;

            repository.Setup(repo => repo.UpdateBasicInformation(hotelID, hotelName, openedIn, description,
                                                                                hotelAddress, city, latitude, longitude, mainImage))
                .Returns(new ResponseMessage
                {
                    Success = false,
                    Data = null,
                    Message = "Invalid data",
                    StatusCode = (int)HttpStatusCode.BadRequest
                });

            // Act
            var result = partnerHotelController.UpdateBasicInfomation(hotelID, hotelName, openedIn, description,
                                                                                    hotelAddress, city, latitude, longitude, mainImage) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Invalid data", responseMessage.Message);
        }

        [Test]
        public void UpdateHotelService_ValidData_ReturnsOk()
        {
            // Arrange
            int hotelID = 1;
            var services = new List<ServiceTypeDTO>
            {
                new ServiceTypeDTO { serviceType = "Updated Restaurant", subServiceName = new List<string> { "New Buffet", "Fine Dining" } },
                new ServiceTypeDTO { serviceType = "Fitness Center", subServiceName = new List<string> { "Gym", "Yoga" } }
            };

            repository.Setup(repo => repo.UpdateHotelService(hotelID, services))
                .Returns(new ResponseMessage
                {
                    Success = true,
                    Data = It.IsAny<object>(), // Chỉ cần kiểm tra kiểu dữ liệu
                    Message = "Successfully updated hotel services",
                    StatusCode = (int)HttpStatusCode.OK
                });

            // Act
            var result = partnerHotelController.UpdateHotelService(new UpdateServiceDTO { HotelID = hotelID, Services = services }) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Successfully updated hotel services", responseMessage.Message);
        }

        [Test]
        public void UpdateHotelService_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            int hotelID = 1;
            var services = new List<ServiceTypeDTO>
            {
                new ServiceTypeDTO { serviceType = "Updated Restaurant", subServiceName = new List<string> { "New Buffet", "Fine Dining" } },
                new ServiceTypeDTO { serviceType = "Fitness Center", subServiceName = new List<string> { "Gym", "Yoga" } }
            };

            repository.Setup(repo => repo.UpdateHotelService(hotelID, services))
                .Returns(new ResponseMessage
                {
                    Success = false,
                    Data = null,
                    Message = "Invalid data",
                    StatusCode = (int)HttpStatusCode.BadRequest
                });

            // Act
            var result = partnerHotelController.UpdateHotelService(new UpdateServiceDTO { HotelID = hotelID, Services = services }) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Invalid data", responseMessage.Message);
        }

        [Test]
        public void AddHotelImage_ValidData_ReturnsOk()
        {
            // Arrange
            int hotelID = 1;
            string title = "New Hotel View";
            var image = new Mock<IFormFile>().Object;

            repository.Setup(repo => repo.AddHotelImage(hotelID, title, image))
                .Returns(new ResponseMessage
                {
                    Success = true,
                    Data = It.IsAny<object>(), // Chỉ cần kiểm tra kiểu dữ liệu
                    Message = "Successfully",
                    StatusCode = (int)HttpStatusCode.OK
                });

            // Act
            var result = partnerHotelController.AddHotelImage(hotelID, title, image) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
        }

        [Test]
        public void AddHotelImage_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            int hotelID = 1;
            string title = "New Hotel View";
            var image = new Mock<IFormFile>().Object;

            repository.Setup(repo => repo.AddHotelImage(hotelID, title, image))
                .Returns(new ResponseMessage
                {
                    Success = false,
                    Data = null,
                    Message = "Invalid data",
                    StatusCode = (int)HttpStatusCode.BadRequest
                });

            // Act
            var result = partnerHotelController.AddHotelImage(hotelID, title, image) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Invalid data"));
            Assert.That(responseMessage.Data, Is.Null);
        }
        [Test]
        public void DeleteHotelImages_ValidImageID_ReturnsOk()
        {
            // Arrange
            int imageID = 1;

            repository.Setup(repo => repo.DeleteHotelImages(imageID))
                .Returns(new ResponseMessage
                {
                    Success = true,
                    Data = It.IsAny<object>(), // Chỉ cần kiểm tra kiểu dữ liệu
                    Message = "Successfully",
                    StatusCode = (int)HttpStatusCode.OK
                });

            // Act
            var result = partnerHotelController.DeleteHotelImages(imageID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
        }
        [Test]
        public void DeleteHotelImages_InvalidImageID_ReturnsBadRequest()
        {
            // Arrange
            int imageID = 1;

            repository.Setup(repo => repo.DeleteHotelImages(imageID))
                .Returns(new ResponseMessage
                {
                    Success = false,
                    Data = null,
                    Message = "Invalid data",
                    StatusCode = (int)HttpStatusCode.BadRequest
                });

            // Act
            var result = partnerHotelController.DeleteHotelImages(imageID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Invalid data"));
            Assert.That(responseMessage.Data, Is.Null);
        }
        [Test]
        public void GetAllHotelInfomation_Success()
        {
            // Arrange
            var fakeHotels = GetFakeHotels();
            repository.Setup(repo => repo.GetAllHotelInfomation())
                .Returns(new ResponseMessage
                {
                    Success = true,
                    Data = fakeHotels,
                    Message = "Successfully",
                    StatusCode = (int)HttpStatusCode.OK
                });

            // Act
            var result = adminHotelController.GetAllHotelInfomation() as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.That(responseMessage.Data, Is.EqualTo(fakeHotels));
        }
        [Test]
        public void GetAllHotelInfomation_NoData_ReturnsNotFound()
        {
            // Arrange
            repository.Setup(repo => repo.GetAllHotelInfomation())
                .Returns(new ResponseMessage
                {
                    Success = false,
                    Data = null,
                    Message = "No data",
                    StatusCode = (int)HttpStatusCode.NotFound
                });

            // Act
            var result = adminHotelController.GetAllHotelInfomation() as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("No data"));
            Assert.That(responseMessage.Data, Is.Null);
        }
        [Test]
        public void FilterHotelByStatus_Success()
        {
            // Arrange
            bool status = true;
            var fakeHotels = GetFakeHotels();
            var expected = fakeHotels.Where(h => h.Status == status).ToList();

            repository.Setup(repo => repo.FilterHotelByStatus(status))
                .Returns(new ResponseMessage
                {
                    Success = true,
                    Data = expected,
                    Message = "Successfully",
                    StatusCode = (int)HttpStatusCode.OK
                });

        }
        [Test]
        public void BlockedHotel_Success()
        {
            // Arrange
            int hotelID = 1;
            string reasonBlock = "Blocked by admin";

            repository.Setup(repo => repo.BlockedHotel(hotelID, reasonBlock))
                .Returns(new ResponseMessage
                {
                    Success = true,
                    Data = It.IsAny<object>(), // Chỉ cần kiểm tra kiểu dữ liệu
                    Message = "Successfully",
                    StatusCode = (int)HttpStatusCode.OK
                });

            // Act
            var result = adminHotelController.BlockedHotel(hotelID, reasonBlock) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
        }
        [Test]
        public void BlockedHotel_Fail()
        {
            // Arrange
            int hotelID = 1;
            string reasonBlock = "Blocked by admin";

            repository.Setup(repo => repo.BlockedHotel(hotelID, reasonBlock))
                .Returns(new ResponseMessage
                {
                    Success = false,
                    Data = null,
                    Message = "Invalid data",
                    StatusCode = (int)HttpStatusCode.BadRequest
                });

            // Act
            var result = adminHotelController.BlockedHotel(hotelID, reasonBlock) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Invalid data"));
            Assert.That(responseMessage.Data, Is.Null);
        }
        [Test]
        public void ConfirmRegistration_Success()
        {
            // Arrange
            int hotelID = 1;

            repository.Setup(repo => repo.ConfirmRegistration(hotelID))
                .Returns(new ResponseMessage
                {
                    Success = true,
                    Data = It.IsAny<object>(), // Chỉ cần kiểm tra kiểu dữ liệu
                    Message = "Successfully",
                    StatusCode = (int)HttpStatusCode.OK
                });

            // Act
            var result = adminHotelController.ConfirmRegistration(hotelID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
        }
        [Test]
        public void ConfirmRegistration_Fail()
        {
            // Arrange
            int hotelID = 1;

            repository.Setup(repo => repo.ConfirmRegistration(hotelID))
                .Returns(new ResponseMessage
                {
                    Success = false,
                    Data = null,
                    Message = "Invalid data",
                    StatusCode = (int)HttpStatusCode.BadRequest
                });

            // Act
            var result = adminHotelController.ConfirmRegistration(hotelID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Invalid data"));
            Assert.That(responseMessage.Data, Is.Null);
        }
        [Test]
        public void RejectRegistration_Success()
        {
            // Arrange
            int hotelID = 1;
            string reasonReject = "Rejected by admin";

            repository.Setup(repo => repo.RejectRegistration(hotelID, reasonReject))
                .Returns(new ResponseMessage
                {
                    Success = true,
                    Data = It.IsAny<object>(), // Chỉ cần kiểm tra kiểu dữ liệu
                    Message = "Successfully",
                    StatusCode = (int)HttpStatusCode.OK
                });

            // Act
            var result = adminHotelController.RejectRegistration(hotelID, reasonReject) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
        }
        [Test]
        public void RejectRegistration_Fail()
        {
            // Arrange
            int hotelID = 1;
            string reasonReject = "Rejected by admin";

            repository.Setup(repo => repo.RejectRegistration(hotelID, reasonReject))
                .Returns(new ResponseMessage
                {
                    Success = false,
                    Data = null,
                    Message = "Invalid data",
                    StatusCode = (int)HttpStatusCode.BadRequest
                });

            // Act
            var result = adminHotelController.RejectRegistration(hotelID, reasonReject) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Invalid data"));
            Assert.That(responseMessage.Data, Is.Null);
        }
        [Test]
        public void SearchHotelByName_Success()
        {
            // Arrange
            string hotelName = "HotelA";
            var fakeHotels = GetFakeHotels();
            var expected = fakeHotels.Where(h => h.Name.Equals(hotelName, StringComparison.OrdinalIgnoreCase)).ToList();

            repository.Setup(repo => repo.SearchHotelByName(hotelName))
                .Returns(new ResponseMessage
                {
                    Success = true,
                    Data = expected,
                    Message = "Successfully",
                    StatusCode = (int)HttpStatusCode.OK
                });
        }
        [Test]
        public void GetAllHotelWaitForApproval_Success()
        {
            // Arrange
            var fakeHotels = GetFakeHotels();
            var expected = fakeHotels.Where(h => h.Status == false).ToList();

            repository.Setup(repo => repo.GetAllHotelWaitForApproval())
                .Returns(new ResponseMessage
                {
                    Success = true,
                    Data = expected,
                    Message = "Successfully",
                    StatusCode = (int)HttpStatusCode.OK
                });

            // Act
            var result = adminHotelController.GetAllHotelWaitForApproval() as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.That(responseMessage.Data, Is.EqualTo(expected));
        }
        [Test]
        public void GetAllHotelWaitForApproval_NoData_ReturnsNotFound()
        {
            // Arrange
            repository.Setup(repo => repo.GetAllHotelWaitForApproval())
                .Returns(new ResponseMessage
                {
                    Success = false,
                    Data = null,
                    Message = "No data",
                    StatusCode = (int)HttpStatusCode.NotFound
                });

            // Act
            var result = adminHotelController.GetAllHotelWaitForApproval() as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("No data"));
            Assert.That(responseMessage.Data, Is.Null);
        }
        [Test]
        public void AnalyzeHotelStandar_Success()
        {
            // Arrange
            var fakeHotels = GetFakeHotels();
            var expected = fakeHotels.GroupBy(h => h.HotelStandar)
                                     .Select(g => new
                                     {
                                         HotelStandar = g.Key,
                                         Count = g.Count()
                                     }).ToList();

            repository.Setup(repo => repo.AnalyzeHotelStandar())
                .Returns(new ResponseMessage
                {
                    Success = true,
                    Data = expected,
                    Message = "Successfully",
                    StatusCode = (int)HttpStatusCode.OK
                });

            // Act
            var result = adminHotelController.AnalyzeHotelStandar() as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.That(responseMessage.Data, Is.EqualTo(expected));
        }
        [Test]
        public void AnalyzeHotelStandar_NoData_ReturnsNotFound()
        {
            // Arrange
            repository.Setup(repo => repo.AnalyzeHotelStandar())
                .Returns(new ResponseMessage
                {
                    Success = false,
                    Data = null,
                    Message = "No data",
                    StatusCode = (int)HttpStatusCode.NotFound
                });

            // Act
            var result = adminHotelController.AnalyzeHotelStandar() as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("No data"));
            Assert.That(responseMessage.Data, Is.Null);
        }
        [Test]
        public void GetGuestReviewByHotel_Success()
        {
            // Arrange
            int hotelID = 1;
            var fakeHotels = GetFakeHotels();
            var expected = fakeHotels.FirstOrDefault(h => h.HotelID == hotelID).feedBacks.ToList();

            repository.Setup(repo => repo.GetGuestReviewByHotel(hotelID))
                .Returns(new ResponseMessage
                {
                    Success = true,
                    Data = expected,
                    Message = "Successfully",
                    StatusCode = (int)HttpStatusCode.OK
                });

            // Act
            var result = controller.GetGuestReviewByHotel(hotelID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;
            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Successfully", responseMessage.Message);
        }
        [Test]
        public void GetBasicInformation_Success()
        {
            // Arrange
            int hotelID = 1;
            var fakeHotels = GetFakeHotels();
            var expected = fakeHotels.FirstOrDefault(h => h.HotelID == hotelID);

            repository.Setup(repo => repo.GetBasicInformation(hotelID))
                .Returns(new ResponseMessage
                {
                    Success = true,
                    Data = expected,
                    Message = "Successfully",
                    StatusCode = (int)HttpStatusCode.OK
                });

            // Act
            var result = partnerHotelController.GetBasicInformation(hotelID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Successfully", responseMessage.Message);
        }
        [Test]
        public void GetBasicInformation_Fail()
        {
            // Arrange
            int hotelID = 1;

            repository.Setup(repo => repo.GetBasicInformation(hotelID))
                .Returns(new ResponseMessage
                {
                    Success = false,
                    Data = null,
                    Message = "Invalid data",
                    StatusCode = (int)HttpStatusCode.BadRequest
                });

            // Act
            var result = partnerHotelController.GetBasicInformation(hotelID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Invalid data", responseMessage.Message);
        }
        private List<Hotel> GetFakeHotels()
        {
            var hotels = new List<Hotel>
        {
        new Hotel
        {
            HotelID = 1,
            Name = "Baodev Hotel",
            OpenedIn = 2024,
            Description = "Tao là trùm",
            HotelStandar = 4,
            Status = true,
            HotelAddress = new HotelAddress
            {
                AddressID = 1,
                Address = "Baodev",
                City = "Ho Chi Minh",
                latitude = 10,
                longitude = 10
            },
            feedBacks = new List<FeedBack>
            {
                new FeedBack { FeedBackID = 1, Rating = 5, Description = "Excellent", isDeleted = false },
                new FeedBack { FeedBackID = 2, Rating = 5, Description = "Very Good", isDeleted = false }
            },
            rooms = new List<Room>
            {
                new Room
                {
                    RoomID = 1,
                    TypeOfRoom = "Deluxe",
                    NumberCapacity = 2,
                    Price = 150.0,
                    Quantity = 10,
                    SizeOfRoom = 25,
                    TypeOfBed = "King Size",
                    SpecialPrice = new List<SpecialPrice>
                    {
                        new SpecialPrice { SpecialPriceID = 1, StartDate = DateTime.Now.AddMonths(-1), EndDate = DateTime.Now.AddMonths(1), Price = 120.0 }
                    }
                }
            },
            HotelServices = new List<HotelService>
            {
                new HotelService { ServiceID = 1, Type = "Breakfast", Hotel = new Hotel { HotelID = 1 } },
                new HotelService { ServiceID = 2, Type = "Gym", Hotel = new Hotel { HotelID = 1 } }
            }
        },
            new Hotel
            {
                HotelID = 2,
                Name = "DatDev Hotel",
                OpenedIn = 2024,
                Description = "Tao Siêu Nhân",
                HotelStandar = 5,
                Status = true,
                HotelAddress = new HotelAddress
                {
                    AddressID = 2,
                    Address = "DatDev",
                    City = "Ho Chi Minh",
                    latitude = 10,
                    longitude = 10
                },
                feedBacks = new List<FeedBack>
                {
                    new FeedBack { FeedBackID = 3, Rating = 3, Description = "Average", isDeleted = false },
                    new FeedBack { FeedBackID = 4, Rating = 2, Description = "Poor", isDeleted = false }
                },
                rooms = new List<Room>
                {
                    new Room
                    {
                        RoomID = 2,
                        TypeOfRoom = "Single",
                        NumberCapacity = 1,
                        Price = 100.0,
                        Quantity = 20,
                        SizeOfRoom = 15,
                        TypeOfBed = "Single Bed",
                        SpecialPrice = new List<SpecialPrice>
                        {
                            new SpecialPrice { SpecialPriceID = 2, StartDate = DateTime.Now.AddMonths(-2), EndDate = DateTime.Now.AddMonths(2), Price = 80.0 }
                        }
                    }
                },
                HotelServices = new List<HotelService>
                {
                    new HotelService { ServiceID = 3, Type = "Airport Shuttle", Hotel = new Hotel { HotelID = 2 } },
                    new HotelService { ServiceID = 4, Type = "Swimming Pool", Hotel = new Hotel { HotelID = 2 } }
                }
            },
            new Hotel
            {
                HotelID = 3,
                Name = "Sunrise Hotel",
                OpenedIn = 2024,
                Description = "Experience luxury at Sunrise Hotel.",
                HotelStandar = 5,
                Status = true,
                HotelAddress = new HotelAddress
                {
                    AddressID = 3,
                    Address = "Sunrise Avenue",
                    City = "Ho Chi Minh",
                    latitude = 10,
                    longitude = 10
                },
                feedBacks = new List<FeedBack>
                {
                    new FeedBack { FeedBackID = 5, Rating = 4, Description = "Good", isDeleted = false },
                    new FeedBack { FeedBackID = 6, Rating = 5, Description = "Excellent service", isDeleted = false }
                },
                rooms = new List<Room>
                {
                    new Room
                    {
                        RoomID = 3,
                        TypeOfRoom = "Suite",
                        NumberCapacity = 2,
                        Price = 200.0,
                        Quantity = 5,
                        SizeOfRoom = 35,
                        TypeOfBed = "King Size",
                        SpecialPrice = new List<SpecialPrice>
                        {
                            new SpecialPrice { SpecialPriceID = 3, StartDate = DateTime.Now.AddMonths(-1), EndDate = DateTime.Now.AddMonths(1), Price = 180.0 }
                        }
                    }
                },
                HotelServices = new List<HotelService>
                {
                    new HotelService { ServiceID = 5, Type = "Spa", Hotel = new Hotel { HotelID = 3 } },
                    new HotelService { ServiceID = 6, Type = "Restaurant", Hotel = new Hotel { HotelID = 3 } }
                }
            }
            };

            return hotels;
        }

    }
}