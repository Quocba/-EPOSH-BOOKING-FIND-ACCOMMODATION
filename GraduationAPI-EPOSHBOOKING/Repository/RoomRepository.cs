using GraduationAPI_EPOSHBOOKING.DataAccess;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Net;
#pragma warning disable // tắt cảnh báo để code sạch hơn

namespace GraduationAPI_EPOSHBOOKING.Repository
{
    public class RoomRepository : IRoomRepository
    {
        private readonly DBContext db;
        public RoomRepository(DBContext _db)
        {
            this.db = _db;
        }

        public ResponseMessage GetRoomDetail(int roomID)
        {
            var getRoom = db.room.Include(x => x.RoomImages).Include(x => x.SpecialPrice).Include(x => x.RoomService).ThenInclude(x => x.RoomSubServices)
            .FirstOrDefault(room => room.RoomID == roomID);
            if (getRoom != null)
            {
                return new ResponseMessage { Success = true, Data = getRoom, Message = "Successfully",StatusCode = (int)HttpStatusCode.OK };
            }
                return new ResponseMessage { Success = false,Data = getRoom, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound };
        }

        public ResponseMessage GetAllRoom()
        {
            var listRoom = db.room.Include(service => service.RoomService)
                .ThenInclude(subService => subService.RoomSubServices)
                .Include(img => img.RoomImages).ToList();
            if (listRoom.Any())
            {
                return new ResponseMessage { Success = true, Data = listRoom, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
            return new ResponseMessage { Success = false, Data = listRoom, Message = "No Data", StatusCode = (int)HttpStatusCode.NotFound };
        }

        public ResponseMessage DeleteRoom(int roomID)
        {
            var getRoom = db.room.FirstOrDefault(room => room.RoomID == roomID);
            if (getRoom != null)
            {
                db.room.Remove(getRoom);
                db.SaveChanges();
                return new ResponseMessage { Success = true, Data = getRoom, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
                return new ResponseMessage { Success = false,Data = getRoom, Message = "Data not found",StatusCode = (int)HttpStatusCode.NotFound}; 
        }

        public ResponseMessage AddRoom(int hotelID, Room room, DateTime StartDate,
                                       DateTime EndDate,
                                       double specialPrice, List<IFormFile> images,List<ServiceType>services)
        {
            var getHotel = db.hotel.FirstOrDefault(hotel => hotel.HotelID == hotelID);
            Room createRoom = new Room
            {
                TypeOfBed = room.TypeOfBed,
                NumberCapacity = room.NumberCapacity,
                Price = room.Price,
                Quantity = room.Quantity,
                SizeOfRoom = room.SizeOfRoom,
                TypeOfRoom = room.TypeOfRoom,
                Hotel = getHotel
            };
            db.room.Add(createRoom);

            SpecialPrice addSpecialPrice = new SpecialPrice
            {
                StartDate = StartDate,
                EndDate = EndDate,
                Price = specialPrice,
                Room = createRoom
            };
            db.specialPrice.Add(addSpecialPrice);

            foreach (var image in images)
            {
                byte[] ImageData = Ultils.Utils.ConvertIFormFileToByteArray(image);
                RoomImage addImage = new RoomImage
                {
                    Image = ImageData,
                    Room = createRoom
                };
                db.roomImage.Add(addImage);

            }

            foreach (var service in services)
            {
                var addService = new RoomService
                {
                    Type = service.Type,
                    Room = createRoom
                };
                db.roomService.Add(addService);
                db.SaveChanges();
                var roomSubService = new List<RoomSubService>();
                foreach (var subServiceName in service.SubServiceNames)
                {
                    var addSubService = new RoomSubService
                    {
                        SubName = subServiceName,
                        RoomService = addService
                    };
                    db.roomSubService.Add(addSubService);
                    roomSubService.Add(addSubService);
                }
                addService.RoomSubServices = roomSubService;

            }
            db.SaveChanges();
            var totalQuantity = db.room.Where(hotel => hotel.Hotel.HotelID == getHotel.HotelID)
                   .Sum(room => room.Quantity);
            if (totalQuantity > 0 && totalQuantity <= 10)
            {
                getHotel.HotelStandar = 1;
                db.hotel.Update(getHotel);
                db.SaveChanges();
            }
            if (totalQuantity >= 20 && totalQuantity <= 49)
            {
                getHotel.HotelStandar = 2;
                db.hotel.Update(getHotel);
                db.SaveChanges();
            }

            if (totalQuantity >= 50 && totalQuantity <= 79)
            {
                getHotel.HotelStandar = 3;
                db.hotel.Update(getHotel);
                db.SaveChanges();
            }
            if (totalQuantity >= 80 && totalQuantity <= 99)
            {
                getHotel.HotelStandar = 4;
                db.hotel.Update(getHotel);
                db.SaveChanges();
            }
            if (totalQuantity >= 99){ 
                getHotel.HotelStandar = 5;
                db.hotel.Update(getHotel);
                db.SaveChanges();
            }
               
            return new ResponseMessage { Success = true, Data = createRoom, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
        }

        public ResponseMessage UpdateRoom(int roomID, Room room, DateTime StartDate, DateTime EndDate, double SpecialPrice, List<IFormFile> image, 
            List<ServiceType>services)
        {
            var getRoom = db.room
                  .Include(room => room.Hotel)
                  .Include(room => room.SpecialPrice)
                  .Include(room => room.RoomService)
                  .ThenInclude(roomService => roomService.RoomSubServices)
                  .FirstOrDefault(room => room.RoomID == roomID);
            if (getRoom == null)
            {
                return new ResponseMessage { Success = false, Data = getRoom, Message = "Data not found", StatusCode = (int)HttpStatusCode.OK };
            }
            else
            {
                 getRoom.TypeOfRoom = room.TypeOfRoom;
                 getRoom.NumberCapacity = room.NumberCapacity;
                 getRoom.Price = room.Price;
                 getRoom.Quantity = room.Quantity;
                 getRoom.SizeOfRoom = room.SizeOfRoom;
                 getRoom.TypeOfBed = room.TypeOfBed;
                 db.room.Update(getRoom);
                 
                var getSpecialPriceRoom = db.specialPrice.FirstOrDefault(sp => sp.Room.RoomID== roomID);
                if (getSpecialPriceRoom == null)
                {
                    SpecialPrice addSpecialPrice = new SpecialPrice
                    {
                        StartDate = StartDate,
                        EndDate = EndDate,
                        Price = SpecialPrice,
                        Room = getRoom
                    };
                    db.specialPrice.Add(addSpecialPrice);
                }
                else
                {
                    getSpecialPriceRoom.StartDate = StartDate;
                    getSpecialPriceRoom.EndDate = EndDate;
                    getSpecialPriceRoom.Price = SpecialPrice;
                    db.specialPrice.Update(getSpecialPriceRoom);
                }
                var existingImages = db.roomImage.Where(ri => ri.Room.RoomID == roomID).ToList();
                db.roomImage.RemoveRange(existingImages);
                foreach (var img in image)
                {
                    byte[]imgData = Ultils.Utils.ConvertIFormFileToByteArray(img);
                    RoomImage updateNewImage = new RoomImage
                    {
                        Image = imgData,
                        Room = getRoom
                    };
                    db.roomImage.Add(updateNewImage);
                   
                }

                var existingServices = db.roomService.Where(rs => rs.Room.RoomID == roomID).ToList();
                db.roomService.RemoveRange(existingServices);

                foreach (var service in services)
                {
                    var addService = new RoomService
                    {
                        Type = service.Type,
                        Room = getRoom
                    };
                    db.roomService.Add(addService);
                    db.SaveChanges();
                    var roomSubService = new List<RoomSubService>();
                    foreach (var subServiceName in service.SubServiceNames)
                    {   
                        var addSubService = new RoomSubService
                        {
                            SubName = subServiceName,
                            RoomService = addService
                        };
                        db.roomSubService.Add(addSubService);
                        roomSubService.Add(addSubService);
                    }
                    addService.RoomSubServices = roomSubService;

                }


                var HotelID = getRoom.Hotel.HotelID;
                var totalQuantity = db.room.Where(r => r.Hotel.HotelID == HotelID).Sum(r => r.Quantity);
                var getHotel = db.hotel.FirstOrDefault(hotel => hotel.HotelID == HotelID);


                if (totalQuantity > 0 && totalQuantity <= 10)
                {
                    getHotel.HotelStandar = 1;
                    db.hotel.Update(getHotel);
                    db.SaveChanges();
                }
                else if (totalQuantity >= 20 && totalQuantity <= 49)
                {
                    getHotel.HotelStandar = 2;
                    db.hotel.Update(getHotel);
                    db.SaveChanges();
                }
                else if (totalQuantity >= 50 && totalQuantity <= 79)
                {
                    getHotel.HotelStandar = 3;
                    db.hotel.Update(getHotel);
                    db.SaveChanges();
                }
                else if (totalQuantity >= 80 && totalQuantity <= 99)
                {
                    getHotel.HotelStandar = 4;
                    db.hotel.Update(getHotel);
                    db.SaveChanges();
                }
                else if (totalQuantity >= 100)
                {
                    getHotel.HotelStandar = 5;
                    db.hotel.Update(getHotel);
                    db.SaveChanges();
                }


                
                return new ResponseMessage {Success = true, Data = getRoom, Message  = "Successfully", StatusCode = (int)HttpStatusCode.OK};
 
            }
        }

     


    }
}
