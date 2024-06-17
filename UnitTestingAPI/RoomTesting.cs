using GraduationAPI_EPOSHBOOKING.Controllers.Guest;
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
#pragma warning disable // tắt cảnh báo để code sạch hơn

namespace UnitTestingAPI
{

    [TestFixture]
    public class RoomTesting
    {
        private  Mock<IRoomRepository> repository;
        public GeneralRoomController controller { get; set; }
        [SetUp]
        public void SetUp()
        {
            repository = new Mock<IRoomRepository>();
            controller = new GeneralRoomController(repository.Object);
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
