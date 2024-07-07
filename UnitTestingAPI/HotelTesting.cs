using GraduationAPI_EPOSHBOOKING.Controllers.Guest;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
        private Mock<IHotelRepository> repository;
        [SetUp]
        public void SetUp()
        {
            repository = new Mock<IHotelRepository>();
            controller = new GeneralHotelController(repository.Object);
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
            repository.Setup(repository => repository.GetHotelByPrice(address,minPrice, maxPrice))
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
            var result = controller.GetByHotelStandar(rating) as ObjectResult;
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
            var result = controller.GetByHotelStandar(rating) as ObjectResult;
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
                new FeedBack { FeedBackID = 1, Rating = 5, Description = "Excellent", Status = "Normal" },
                new FeedBack { FeedBackID = 2, Rating = 5, Description = "Very Good", Status = "Normal" }
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
                new FeedBack { FeedBackID = 3, Rating = 3, Description = "Average", Status = "Normal" },
                new FeedBack { FeedBackID = 4, Rating = 2, Description = "Poor", Status = "Normal" }
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
                new FeedBack { FeedBackID = 5, Rating = 4, Description = "Good", Status = "Normal" },
                new FeedBack { FeedBackID = 6, Rating = 5, Description = "Excellent service", Status = "Normal" }
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