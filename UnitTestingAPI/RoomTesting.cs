using GraduationAPI_EPOSHBOOKING.Controllers.Guest;
using GraduationAPI_EPOSHBOOKING.Controllers.Partner;
using GraduationAPI_EPOSHBOOKING.DTO;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
    public class RoomTesting
    {
        private Mock<IRoomRepository> repository;
        public GeneralRoomController controller { get; set; }
        public PartnerRoomController partnerRoomController { get; set; }
        [SetUp]
        public void SetUp()
        {
            repository = new Mock<IRoomRepository>();
            controller = new GeneralRoomController(repository.Object);
            partnerRoomController = new PartnerRoomController(repository.Object);
        }

        [Test]
        public void GetRoomDetailsSuccess()
        {
            int roomID = 1;
            var fakeHotel = GetFakeHotels();
            var expectedRoom = fakeHotel.SelectMany(hotel => hotel.rooms)
                                             .FirstOrDefault(room => room.RoomID == roomID);
            repository.Setup(repo => repo.GetRoomDetail(roomID))
                  .Returns(new ResponseMessage
                  {
                      Success = true,
                      Data = expectedRoom,
                      Message = "Successfully",
                      StatusCode = (int)HttpStatusCode.OK
                  });
            var result = controller.GetRoomDetail(roomID) as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void GetRoomDetailNotFound()
        {
            int roomID = 100;
            var fakeHotel = GetFakeHotels();
            var expectedRoom = fakeHotel.SelectMany(hotel => hotel.rooms)
                                   .FirstOrDefault(room => room.RoomID == roomID);
            repository.Setup(repository => repository.GetRoomDetail(roomID))
                .Returns(new ResponseMessage { Success = false, Data = expectedRoom, Message = "Date not found", StatusCode = (int)HttpStatusCode.NotFound });
            var result = controller.GetRoomDetail(roomID) as ObjectResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void GetRoomByHotelSuccess()
        {
            int hotelID = 1;
            var fakeHotel = GetFakeHotels();
            var expectedRoom = fakeHotel.SelectMany(hotel => hotel.rooms)
                                   .Where(room => room.Hotel.HotelID == hotelID)
                                   .ToList();
            repository.Setup(repository => repository.GetRoomByHotel(hotelID))
                .Returns(new ResponseMessage { Success = true, Data = expectedRoom, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });
            var result = controller.GetRoomByHotel(hotelID) as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void GetRoomByHotelNotFound()
        {
            int hotelID = 100;
            var fakeHotel = GetFakeHotels();
            var expectedRoom = fakeHotel.SelectMany(hotel => hotel.rooms)
                                   .Where(room => room.Hotel.HotelID == hotelID)
                                   .ToList();
            repository.Setup(repository => repository.GetRoomByHotel(hotelID))
                .Returns(new ResponseMessage { Success = false, Data = expectedRoom, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound });
            var result = controller.GetRoomByHotel(hotelID) as ObjectResult;
            Assert.AreEqual(404, result.StatusCode);
        }
        [Test]
        public void GetAllRomm()
        {
            var fakeHotel = GetFakeHotels();
            var expectedRoom = fakeHotel.SelectMany(hotel => hotel.rooms).ToList();
            repository.Setup(repository => repository.GetAllRoom())
                .Returns(new ResponseMessage { Success = true, Data = expectedRoom, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });
            var result = controller.GetAllRoom() as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void GetAllRoomNotFound()
        {
            var fakeHotel = GetFakeHotels();
            var expectedRoom = fakeHotel.SelectMany(hotel => hotel.rooms).ToList();
            repository.Setup(repository => repository.GetAllRoom())
                .Returns(new ResponseMessage { Success = false, Data = expectedRoom, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound });
            var result = controller.GetAllRoom() as ObjectResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        #region AddRoom Tests
        [Test]
        public void AddRoom_Success()
        {
            // Arrange
            var addRoomModel = new AddRoomDTO
            {
                HotelID = 1,
                Room = new Room
                {
                    TypeOfRoom = "Deluxe",
                    NumberCapacity = 2,
                    Price = 150.00,
                    Quantity = 10,
                    SizeOfRoom = 35,
                    TypeOfBed = "King"
                },
                SpecialPrices = "[{\"StartDate\":\"2024-05-01T00:00:00\",\"EndDate\":\"2024-05-10T00:00:00\",\"Price\":120.00}]",
                Images = new List<IFormFile> { new Mock<IFormFile>().Object },
                Services = "[{\"serviceType\":\"Breakfast\",\"subServiceName\":[\"Continental\"]}]"
            };

            repository.Setup(repo => repo.AddRoom(addRoomModel.HotelID, addRoomModel.Room, It.IsAny<List<SpecialPrice>>(),
                                                 It.IsAny<List<IFormFile>>(), It.IsAny<List<ServiceTypeDTO>>()))
                .Returns(new ResponseMessage { Success = true, Data = addRoomModel.Room, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = partnerRoomController.AddRoom(addRoomModel) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
        }

        [Test]
        public void AddRoom_RoomDataInvalid_ReturnsBadRequest()
        {
            // Arrange
            var addRoomModel = new AddRoomDTO
            {
                HotelID = 1,
                Room = new Room { }, // Dữ liệu phòng không hợp lệ
                SpecialPrices = "[{\"StartDate\":\"2024-05-01T00:00:00\",\"EndDate\":\"2024-05-10T00:00:00\",\"Price\":120.00}]",
                Images = new List<IFormFile> { new Mock<IFormFile>().Object },
                Services = "[{\"serviceType\":\"Breakfast\",\"subServiceName\":[\"Continental\"]}]"
            };

            repository.Setup(repo => repo.AddRoom(addRoomModel.HotelID, addRoomModel.Room, It.IsAny<List<SpecialPrice>>(),
                                                 It.IsAny<List<IFormFile>>(), It.IsAny<List<ServiceTypeDTO>>()))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "Invalid room data", StatusCode = (int)HttpStatusCode.BadRequest });

            // Act
            var result = partnerRoomController.AddRoom(addRoomModel) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Invalid room data"));
        }

        #endregion

        #region UpdateRoom Tests
        [Test]
        public void UpdateRoom_Success()
        {
            // Arrange
            var updateRoomModel = new UpdateRoomDTO
            {
                RoomID = 1,
                Room = new Room
                {
                    TypeOfRoom = "Updated Deluxe",
                    NumberCapacity = 3,
                    Price = 180.00,
                    Quantity = 15,
                    SizeOfRoom = 40,
                    TypeOfBed = "Queen"
                },
                specialPrice = "[{\"StartDate\":\"2024-06-01T00:00:00\",\"EndDate\":\"2024-06-10T00:00:00\",\"Price\":150.00}]",
                Images = new List<IFormFile> { new Mock<IFormFile>().Object },
                Services = "[{\"serviceType\":\"Dinner\",\"subServiceName\":[\"Buffet\"]}]"
            };

            repository.Setup(repo => repo.UpdateRoom(updateRoomModel.RoomID, updateRoomModel.Room, It.IsAny<List<SpecialPrice>>(),
                                                     It.IsAny<List<IFormFile>>(), It.IsAny<List<ServiceTypeDTO>>()))
                .Returns(new ResponseMessage { Success = true, Data = updateRoomModel.Room, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = partnerRoomController.UpdateRoom(updateRoomModel) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
        }

        [Test]
        public void UpdateRoom_RoomNotFound_ReturnsNotFound()
        {
            // Arrange
            var updateRoomModel = new UpdateRoomDTO
            {
                RoomID = 999, // ID phòng không tồn tại
                Room = new Room
                {
                    TypeOfRoom = "Updated Deluxe",
                    NumberCapacity = 3,
                    Price = 180.00,
                    Quantity = 15,
                    SizeOfRoom = 40,
                    TypeOfBed = "Queen"
                },
                specialPrice = "[{\"StartDate\":\"2024-06-01T00:00:00\",\"EndDate\":\"2024-06-10T00:00:00\",\"Price\":150.00}]",
                Images = new List<IFormFile> { new Mock<IFormFile>().Object },
                Services = "[{\"serviceType\":\"Dinner\",\"subServiceName\":[\"Buffet\"]}]"
            };

            repository.Setup(repo => repo.UpdateRoom(updateRoomModel.RoomID, updateRoomModel.Room, It.IsAny<List<SpecialPrice>>(),
                                                     It.IsAny<List<IFormFile>>(), It.IsAny<List<ServiceTypeDTO>>()))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = partnerRoomController.UpdateRoom(updateRoomModel) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Data not found"));
        }

        #endregion

        #region DeleteRoom Tests
        [Test]
        public void DeleteRoom_Success()
        {
            // Arrange
            int roomID = 1;
            var expectedRoom = new Room { RoomID = roomID };

            repository.Setup(repo => repo.DeleteRoom(roomID))
                .Returns(new ResponseMessage { Success = true, Data = expectedRoom, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = partnerRoomController.DeleteRoom(roomID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
        }

        [Test]
        public void DeleteRoom_RoomNotFound_ReturnsNotFound()
        {
            // Arrange
            int roomID = 999; // ID phòng không tồn tại
            repository.Setup(repo => repo.DeleteRoom(roomID))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = partnerRoomController.DeleteRoom(roomID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Data not found"));
        }
        #endregion
    

    public List<Hotel> GetFakeHotels()
        {
            var room1 = new Room
            {
                RoomID = 1,
                TypeOfRoom = "Deluxe",
                NumberCapacity = 2,
                Price = 150.00,
                Quantity = 10,
                SizeOfRoom = 35,
                TypeOfBed = "King",
                RoomImages = new List<RoomImage>
        {
            new RoomImage { ImageID = 1, Image ="/images/1cd30da4-9dab-4f69-8edd-98e26fa01878_meme.jpg"}
        },
                RoomService = new List<RoomService>
        {
            new RoomService
            {
                RoomServiceID = 1,
                Type = "Breakfast",
                RoomSubServices = new List<RoomSubService>
                {
                    new RoomSubService { SubServiceID = 1, SubName = "Continental" }
                }
            }
        },
                SpecialPrice = new List<SpecialPrice>
        {
            new SpecialPrice { SpecialPriceID = 1, StartDate = DateTime.Now.AddDays(-10), EndDate = DateTime.Now.AddDays(10), Price = 130.00 }
        }
            };

            var room2 = new Room
            {
                RoomID = 2,
                TypeOfRoom = "Standard",
                NumberCapacity = 2,
                Price = 100.00,
                Quantity = 5,
                SizeOfRoom = 25,
                TypeOfBed = "Queen",
                RoomImages = new List<RoomImage>
        {
            new RoomImage { ImageID = 2, Image = "/images/1cd30da4-9dab-4f69-8edd-98e26fa01878_meme.jpg" }
        },
                RoomService = new List<RoomService>
        {
            new RoomService
            {
                RoomServiceID = 2,
                Type = "Dinner",
                RoomSubServices = new List<RoomSubService>
                {
                    new RoomSubService { SubServiceID = 2, SubName = "Buffet" }
                }
            }
        },
                SpecialPrice = new List<SpecialPrice>
        {
            new SpecialPrice { SpecialPriceID = 2, StartDate = DateTime.Now.AddDays(-5), EndDate = DateTime.Now.AddDays(5), Price = 90.00 }
        }
            };

            var fakeHotels = new List<Hotel>
    {
        new Hotel
        {
            HotelID = 1,
            Name = "Fake Hotel 1",
            rooms = new List<Room> { room1, room2 }
        },
        new Hotel
        {
            HotelID = 2,
            Name = "Fake Hotel 2",
            rooms = new List<Room>
            {
                new Room
                {
                    RoomID = 3,
                    TypeOfRoom = "Suite",
                    NumberCapacity = 4,
                    Price = 250.00,
                    Quantity = 2,
                    SizeOfRoom = 50,
                    TypeOfBed = "King",
                    RoomImages = new List<RoomImage>
                    {
                        new RoomImage { ImageID = 3, Image = "/images/1cd30da4-9dab-4f69-8edd-98e26fa01878_meme.jpg" }
                    },
                    RoomService = new List<RoomService>
                    {
                        new RoomService
                        {
                            RoomServiceID = 3,
                            Type = "Spa",
                            RoomSubServices = new List<RoomSubService>
                            {
                                new RoomSubService { SubServiceID = 3, SubName = "Massage" }
                            }
                        }
                    },
                    SpecialPrice = new List<SpecialPrice>
                    {
                        new SpecialPrice { SpecialPriceID = 3, StartDate = DateTime.Now.AddDays(-15), EndDate = DateTime.Now.AddDays(15), Price = 220.00 }
                    }
                }
            }
        }
    };

            // Set the Hotel reference in Room
            foreach (var hotel in fakeHotels)
            {
                foreach (var room in hotel.rooms)
                {
                    room.Hotel = hotel;
                }
            }

            return fakeHotels;
        }


    }



}
