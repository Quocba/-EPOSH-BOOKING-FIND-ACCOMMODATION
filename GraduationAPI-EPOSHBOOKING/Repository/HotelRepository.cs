using GraduationAPI_EPOSHBOOKING.DataAccess;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Utils.Windows;
using System.Net;
using GraduationAPI_EPOSHBOOKING.Ultils;
using Microsoft.IdentityModel.Tokens;

#pragma warning disable // tắt cảnh báo để code sạch hơn
namespace GraduationAPI_EPOSHBOOKING.Repository
{
    public class HotelRepository : IHotelRepository
    {
        private readonly DBContext db;
        private readonly Utils ultils;
        public HotelRepository(DBContext _db, Utils _ultils)
        {
            this.db = _db;
            this.ultils = _ultils;
        }

        public ResponseMessage GetAllHotel()
        {
            var listHotel = db.hotel.Include(x => x.HotelAddress).Include(x => x.feedBacks)
                .Include(room => room.rooms).ThenInclude(x => x.SpecialPrice).OrderByDescending(hotel => hotel.HotelStandar)
                .Where(hotel => hotel.Status == true && hotel.isRegister.Equals("Approved")).ToList();
            if (listHotel.Any())
            {
                var listHotelWithAvgRating = listHotel.Select(hotel => new
                {
                    Hotel = hotel,
                    AvgRating = hotel.feedBacks.Any() ? Math.Round(hotel.feedBacks.Average(feedback => feedback.Rating), 2) : 0

                }).ToList();


                return new ResponseMessage { Success = true, Data = listHotelWithAvgRating, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
            else
            {
                return new ResponseMessage { Success = true, Data = listHotel, Message = "No Data", StatusCode = (int)HttpStatusCode.NotFound };
            }
        }

        public ResponseMessage GetHotelByCity(String city)
        {
            var getListHotel = db.hotel.Include(x => x.HotelAddress).Include(x => x.feedBacks)
                .Include(room => room.rooms)
                .ThenInclude(specialPrice => specialPrice.SpecialPrice)
                .OrderByDescending(hotel => hotel.HotelStandar)
                .Where(x => x.HotelAddress.City.Equals(city) && x.Status == true && x.isRegister.Equals("Approved")).ToList();

            if (getListHotel.Any())
            {
                var listHotelWithAvgRating = getListHotel.Select(hotel => new
                {
                    Hotel = hotel,
                    Avgrating = hotel.feedBacks.Any() ? hotel.feedBacks.Average(feedBack => feedBack.Rating) : 0
                });

                return new ResponseMessage { Success = true, Data = listHotelWithAvgRating, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
            else
            {
                return new ResponseMessage { Success = false, Data = getListHotel, Message = "No Data", StatusCode = (int)HttpStatusCode.NotFound };
            }
        }

        public ResponseMessage GetHotelByID(int id)
        {
            var getHotel = db.hotel.Include(x => x.HotelImages)
                .Include(x => x.HotelAddress)
                .Include(x => x.HotelServices).ThenInclude(x => x.HotelSubServices).Include(x => x.feedBacks)
                .ThenInclude(booking => booking.Booking).ThenInclude(account => account.Account).ThenInclude(profile => profile.Profile)
                .FirstOrDefault(x => x.HotelID == id && x.Status == true && x.isRegister.Equals("Approved"));

            if (getHotel != null)
            {
                double avgRating = Math.Round(getHotel.feedBacks.Average(feedback => feedback.Rating), 2);
                return new ResponseMessage { Success = true, Data = new { hotel = getHotel, avgRating = avgRating }, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
            else
            {

                return new ResponseMessage { Success = false, Data = getHotel, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound };
            }
        }


        public ResponseMessage GetHotelByPrice(double minPrice, double maxPrice)
        {
            var currentDate = DateTime.Now;
            var getHotel = db.hotel.Include(x => x.HotelAddress).Include(x => x.feedBacks).Include(x => x.rooms)
                .ThenInclude(x => x.SpecialPrice).OrderByDescending(hotel => hotel.HotelStandar).Select(hotel => new Hotel
                {
                    HotelID = hotel.HotelID,
                    Name = hotel.Name,
                    Description = hotel.Description,
                    HotelAddress = hotel.HotelAddress,
                    HotelStandar = hotel.HotelStandar,
                    MainImage = hotel.MainImage,
                    OpenedIn = hotel.OpenedIn,
                    rooms = hotel.rooms.Where(room => room.Price >= minPrice && room.Price <= maxPrice || room.SpecialPrice.Any(sp => currentDate >= sp.StartDate && currentDate <= sp.EndDate
                    && sp.Price >= minPrice && sp.Price <= maxPrice)).ToList(),
                    feedBacks = hotel.feedBacks.ToList(),
                }).Where(hotel => hotel.Status == true && hotel.isRegister.Equals("Approved")).ToList();

            var filterHotel = getHotel.Where(hotel => hotel.rooms.Any());
            var listHotelWithRating = filterHotel.Select(hotel => new
            {
                Hotel = hotel,
                AvgRating = hotel.feedBacks.Any() ? Math.Round(hotel.feedBacks.Average(feedback => feedback.Rating), 2) : 0
            });

            if (listHotelWithRating.Any())
            {
                return new ResponseMessage { Success = true, Data = listHotelWithRating, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
            return new ResponseMessage { Success = false, Data = listHotelWithRating, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound };
        }

        public ResponseMessage GetByRating(int rating)
        {
            var listHotel = db.hotel.Include(x => x.HotelImages).Include(x => x.HotelAddress).Include(x => x.HotelServices).ThenInclude(x => x.HotelSubServices)
                .Include(x => x.feedBacks).Include(x => x.rooms).ThenInclude(x => x.SpecialPrice)
                .Where(hotel => hotel.Status == true && hotel.isRegister.Equals("Approved")).ToList();
            var filterHotelWithRating = listHotel.OrderByDescending(hotel => hotel.HotelStandar).Select(hotel => new
            {
                Hotel = hotel,
                AvgRating = hotel.feedBacks.Any() ? Math.Round(hotel.feedBacks.Average(fb => fb.Rating), 2) : 0
            });

            var getHotelWithRating = filterHotelWithRating.Where(rt => rt.AvgRating <= rating && rating <= 5).OrderByDescending(rt => rt.AvgRating).ToList();
            if (getHotelWithRating.Any())
            {
                return new ResponseMessage { Success = true, Data = getHotelWithRating, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
            else
            {
                return new ResponseMessage { Success = false, Data = getHotelWithRating, Message = "No Data", StatusCode = (int)HttpStatusCode.NotFound };
            }
        }

        public ResponseMessage GetByService(List<String> services)
        {
            var currentDate = DateTime.Now;
            if (services.Any())
            {
                var listHotel = db.hotel.Include(x => x.HotelImages).Include(x => x.HotelAddress).Include(x => x.HotelServices).ThenInclude(x => x.HotelSubServices)
                .Include(x => x.feedBacks).Include(x => x.rooms).ThenInclude(special => special.SpecialPrice)
                .Where(hotel => hotel.Status == true && hotel.isRegister.Equals("Approved")).ToList();

                var listHotelWithService = listHotel.Where(hotel => hotel.HotelServices.Any(service => services.Contains(service.Type))).OrderByDescending(hotel => hotel.HotelStandar).ToList();
                var result = listHotelWithService.Select(hotel => new
                {
                    HotelId = hotel,
                    RoomSpecialPrice = hotel.rooms
                    .Where(room => room.SpecialPrice.Any(sp => sp.StartDate <= currentDate && sp.EndDate >= currentDate))
                    .Select(room => new
                    {
                        room.RoomID,
                        Price = room.SpecialPrice
                        .Where(sp => currentDate >= sp.StartDate && currentDate <= sp.EndDate)
                        .Select(sp => sp.Price)
                        .FirstOrDefault()

                    }).ToList(),
                }).ToList();
                if (result.Any())
                {
                    return new ResponseMessage { Success = true, Data = result, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
                }
                else
                {
                    return new ResponseMessage { Success = false, Data = result, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound };
                }
            }

            return new ResponseMessage { Success = false, Data = null, Message = "Service cannot be empty", StatusCode = (int)HttpStatusCode.BadRequest };
        }


        public ResponseMessage GetServiceByHotelID(int hotelID)
        {
            var hotelService = db.hotel.Include(hotelService => hotelService.HotelServices).ThenInclude(subService => subService.HotelSubServices).Where(x => x.HotelID == hotelID)
                .Where(hotel => hotel.Status == true && hotel.isRegister.Equals("Approved")).ToList();
            if (hotelService.Any())
            {
                return new ResponseMessage { Success = true, Data = hotelService, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
            else
            {
                return new ResponseMessage { Success = false, Data = hotelService, Message = "No Data", StatusCode = (int)HttpStatusCode.NotFound };
            }
        }


        public ResponseMessage GetGalleriesByHotelID(int hotelID)
        {
            var getGalleries = db.hotel.Include(x => x.HotelImages).Where(hotel => hotel.HotelID == hotelID).ToList();
            if (getGalleries.Any())
            {
                return new ResponseMessage { Success = true, Data = getGalleries, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
            else
            {
                return new ResponseMessage { Success = false, Data = getGalleries, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound };
            }
        }


        //Chức năng chưa hoàn thiện
        public ResponseMessage SearchHotel(String city, DateTime? checkInDate, DateTime? checkOutDate, int? numberCapacity, int? quantity)
        {
            var listHotel = db.hotel.Include(address => address.HotelAddress)
                .Include(feedback => feedback.feedBacks)
                .Include(room => room.rooms).ThenInclude(specialPrice => specialPrice.SpecialPrice)
                .Where(hotel => hotel.Status == true && hotel.isRegister.Equals("Approved")).ToList();

            if (!city.IsNullOrEmpty() && checkInDate == null && checkOutDate == null && numberCapacity == null && quantity == null)
            {
                var searchCity = listHotel.Select(hotel => new
                {
                    Hotel = hotel,
                    AvgRating = hotel.feedBacks.Any() ? hotel.feedBacks.Average(rating => rating.Rating) : 0,
                    CountReview = hotel.feedBacks.Any() ? hotel.feedBacks.Count() : 0

                });
                return new ResponseMessage { Success = true, Data = searchCity, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }

            if (checkInDate != null && checkOutDate != null && city != null && quantity == null && numberCapacity == null)
            {
                var filterHotelCity = listHotel.Where(hotel => hotel.HotelAddress.City.Equals(city, StringComparison.OrdinalIgnoreCase))
                                            .ToList();

                var hotel = filterHotelCity.Select(hotel => new
                {
                    Hotel = hotel,
                    RoomSpecialPrice = hotel.rooms
                    .Where(room => room.SpecialPrice.Any(sp => sp.StartDate <= checkInDate && sp.EndDate >= checkOutDate))
                    .Select(room => new
                    {
                        room.RoomID,
                        Price = room.SpecialPrice
                        .Where(sp => sp.StartDate <= checkInDate && sp.EndDate >= checkOutDate)
                        .Select(sp => sp.Price)
                        .FirstOrDefault()

                    }).ToList(),
                    AvgRating = hotel.feedBacks.Any() ? hotel.feedBacks.Average(rating => rating.Rating) : 0,
                    CountReview = hotel.feedBacks.Any() ? hotel.feedBacks.Count() : 0,
                });

                return new ResponseMessage { Success = true, Data = hotel, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }

            if (city != null && numberCapacity != null && quantity != null && checkInDate == null && checkOutDate == null)
            {
                var filterHotelCity = listHotel.Where(hotel => hotel.HotelAddress.City.Equals(city, StringComparison.OrdinalIgnoreCase))
                                           .ToList();

                var filterWithRoom = filterHotelCity.
                    Where(hotel => hotel.rooms.Any(room => room.Quantity >= 1 && room.NumberCapacity >= numberCapacity)).ToList();

                var searchResult = filterWithRoom.Select(hotel => new
                {
                    hotel = hotel,
                    AvgRating = hotel.feedBacks.Any() ? hotel.feedBacks.Average(fb => fb.Rating) : 0,
                    CountReview = hotel.feedBacks.Count()
                }).ToList();

                return new ResponseMessage { Success = true, Data = searchResult, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
            else
            {
                var filterHotelCity = listHotel
                     .Where(hotel => hotel.HotelAddress.City.Equals(city, StringComparison.OrdinalIgnoreCase))
                     .ToList();

                var filterWithRoom = filterHotelCity
                    .Where(hotel => hotel.rooms.Any(room => room.Quantity >= quantity && room.NumberCapacity >= numberCapacity))
                    .ToList();

                var hotel = filterWithRoom.Select(hotel => new
                {
                    Hotel = hotel,
                    AvgRating = hotel.feedBacks.Any() ? hotel.feedBacks.Average(rating => rating.Rating) : 0,
                    CountReview = hotel.feedBacks.Any() ? hotel.feedBacks.Count() : 0,
                    RoomSpecialPrice = hotel.rooms
                    .Where(room => room.SpecialPrice.Any(sp => sp.StartDate <= checkInDate && sp.EndDate >= checkOutDate))
                    .Select(room => new
                    {
                        room.RoomID,
                        Price = room.SpecialPrice
                        .Where(sp => sp.StartDate <= checkInDate && sp.EndDate >= checkOutDate)
                        .Select(sp => sp.Price)
                        .FirstOrDefault()

                    }).ToList()
                }).ToList();
                return new ResponseMessage { Success = true, Data = hotel, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
        }

        public ResponseMessage HotelRegistration(string hotelName,
                                         int openedIn,
                                         string description,
                                         int hotelStandar,
                                         string hotelAddress,
                                         string city,
                                         double latitude,
                                         double longitude,
                                         List<IFormFile> images,
                                         IFormFile mainImage,
                                         int accountID,
                                         List<string> serviceTypes,
                                         List<List<string>> subServiceNames)
        {
            var account = db.accounts
                            .Include(profile => profile.Profile)
                            .FirstOrDefault(a => a.AccountID == accountID);

            var addAddress = new HotelAddress
            {
                Address = hotelAddress,
                City = city,
                latitude = latitude,
                longitude = longitude
            };

            db.hotelAddress.Add(addAddress);

            var addHotel = new Hotel
            {
                Name = hotelName,
                OpenedIn = openedIn,
                MainImage = Ultils.Utils.ConvertIFormFileToByteArray(mainImage),
                Description = description,
                HotelAddress = addAddress,
                Status = false,
                isRegister = "Wait for approved",
                HotelStandar = hotelStandar,
                Account = account
            };

            db.hotel.Add(addHotel);
            db.SaveChanges(); // Save changes to generate IDs for the hotel

            var hotelServices = new List<HotelService>();
            var hotelSubServices = new List<HotelSubService>();

            for (int i = 0; i < serviceTypes.Count; i++)
            {
                var addService = new HotelService
                {
                    Type = serviceTypes[i],
                    Hotel = addHotel // Make sure the HotelID is correctly set
                };

                db.hotelService.Add(addService);
                db.SaveChanges(); // Save changes to generate IDs for the service
                foreach (var subServiceName in subServiceNames[i])
                {
                    var addSubService = new HotelSubService
                    {
                        SubServiceName = subServiceName,
                        HotelService = addService // Use the generated ServiceID
                    };

                    db.hotelSubService.Add(addSubService);
                    hotelSubServices.Add(addSubService);
                }

                addService.HotelSubServices = hotelSubServices;
                hotelServices.Add(addService);
            }

            foreach (var img in images)
            {
                var addImage = new HotelImage
                {
                    ImageData = Ultils.Utils.ConvertIFormFileToByteArray(img),
                    Hotel = addHotel
                };

                db.hotelImage.Add(addImage);
            }

            db.SaveChanges(); // Save all changes at the end
            return new ResponseMessage
            {
                Success = true,
                Data = new
                {
                    Hotel = new
                    {
                        addHotel.HotelID,
                        addHotel.Name,
                        addHotel.OpenedIn,
                        addHotel.MainImage,
                        addHotel.Description,
                        HotelAddress = new
                        {
                            addHotel.HotelAddress.Address,
                            addHotel.HotelAddress.City,
                            addHotel.HotelAddress.latitude,
                            addHotel.HotelAddress.longitude
                        },
                        addHotel.Status,
                        addHotel.isRegister,
                        addHotel.HotelStandar,
                        Account = new
                        {
                            account.AccountID,
                            account.Profile
                            // Thêm thông tin tài khoản nếu cần
                        }
                    },
                    HotelServices = hotelServices.Select(s => new
                    {
                        s.ServiceID,
                        s.Type,
                        HotelSubServices = s.HotelSubServices.Select(sub => new
                        {
                            sub.SubServiceID,
                            sub.SubServiceName
                        }).ToList()
                    }).ToList()
                },
                Message = "Successfully registered hotel",
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}

