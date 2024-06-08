using GraduationAPI_EPOSHBOOKING.DataAccess;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Utils.Windows;
using System.Net;
using GraduationAPI_EPOSHBOOKING.Ultils;
using Microsoft.IdentityModel.Tokens;
using Azure;
using static System.Net.Mime.MediaTypeNames;
using GraduationAPI_EPOSHBOOKING.DTO;
using DocumentFormat.OpenXml.Office2013.Excel;
using DocumentFormat.OpenXml.Drawing.Diagrams;


#pragma warning disable // tắt cảnh báo để code sạch hơn
namespace GraduationAPI_EPOSHBOOKING.Repository
{
    public class HotelRepository : IHotelRepository
    {
        private readonly DBContext db;
        private readonly Utils ultils;
        private readonly IWebHostEnvironment environment;
        public HotelRepository(DBContext _db, Utils _ultils, IWebHostEnvironment environment)
        {
            this.db = _db;
            this.ultils = _ultils;
            this.environment = environment;
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
                    HotelID = hotel.HotelID,
                    MainImage = hotel.MainImage,
                    OpenedIn = hotel.OpenedIn,
                    Description = hotel.Description,
                    HotelStandar = hotel.HotelStandar,
                    IsRegister = hotel.isRegister,
                    Status = hotel.Status,
                    HotelAddress = new
                    {
                        hotel.HotelAddress.AddressID,
                        hotel.HotelAddress.Address,
                        hotel.HotelAddress.City,
                        hotel.HotelAddress.latitude,
                        hotel.HotelAddress.longitude
                    },
                    Rooms = hotel.rooms
                        .Where(room => room != null) // Lọc giá trị null trong rooms
                        .Select(room =>
                        {
                            var currentDate = DateTime.Now;
                            var specialPrice = room.SpecialPrice
                                .FirstOrDefault(sp => currentDate >= sp.StartDate && currentDate <= sp.EndDate);
                            if (specialPrice != null)
                            {
                                room.Price = specialPrice.Price;
                            }
                            return new
                            {
                                RoomID = room.RoomID,
                                TypeOfRoom = room.TypeOfRoom,
                                NumberCapacity = room.NumberCapacity,
                                Price = room.Price,
                                Quantity = room.Quantity,
                                SizeOfRoom = room.SizeOfRoom,
                                TypeOfBed = room.TypeOfBed,
                                SpecialPrice = room.SpecialPrice.Select(sp => new
                                {
                                    SpecialPriceID = sp.SpecialPriceID,
                                    StartDate = sp.StartDate,
                                    EndDate = sp.EndDate,
                                    Price = sp.Price
                                }).ToList(),
                            };
                        }).ToList(),
                    FeedBack = hotel.feedBacks.Select(feedback => new
                    {
                        FeedBackID = feedback.FeedBackID,
                        Rating = feedback.Rating,
                        Image = feedback.Image,
                        Description = feedback.Description,
                        isDelete = feedback.isDeleted,
                        Account = feedback.Account
                    }).ToList(),
                    AvgRating = hotel.feedBacks.Any() ? Math.Round(hotel.feedBacks.Average(feedback => feedback.Rating), 2) : 0,
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
                    Avgrating = hotel.feedBacks.Any() ? hotel.feedBacks.Average(feedBack => feedBack.Rating) : 0,
                    Room = hotel.rooms.Select(room =>
                    {
                        var currentDate = DateTime.Now;
                        var specialPrice = room.SpecialPrice
                                               .FirstOrDefault(sp => currentDate >= sp.StartDate && currentDate <= sp.EndDate);
                        if (specialPrice != null)
                        {
                            room.Price = specialPrice.Price;
                        }
                        return new
                        {
                            RoomID = room.RoomID,
                            TypeOfRoom = room.TypeOfRoom,
                            NumberCapacity = room.NumberCapacity,
                            Price = room.Price,
                            Quantity = room.Quantity,
                            SizeOfRoom = room.SizeOfRoom,
                            TypeOfBed = room.TypeOfBed,
                            SpecialPrice = room.SpecialPrice.Select(sp => new
                            {
                                SpecialPriceID = sp.SpecialPriceID,
                                StartDate = sp.StartDate,
                                EndDate = sp.EndDate,
                                Price = sp.Price
                            }).ToList()
                        };
                    }).ToList()
                }).Select(hotel => new
                {
                    HotelID = hotel.Hotel.HotelID,
                    MainImage = hotel.Hotel.MainImage,
                    OpenedIn = hotel.Hotel.OpenedIn,
                    Description = hotel.Hotel.Description,
                    HotelStandar = hotel.Hotel.HotelStandar,
                    IsRegister = hotel.Hotel.isRegister,
                    Status = hotel.Hotel.Status,
                    HotelAddress = hotel.Hotel.HotelAddress,
                    FeedBack = hotel.Hotel.feedBacks.Select(feedback => new
                    {
                        FeedBackID = feedback.FeedBackID,
                        Rating = feedback.Rating,
                        Image = feedback.Image,
                        Description = feedback.Description,
                        isDelete = feedback.isDeleted,
                        Account = feedback.Account
                    }),
                    Rooms = hotel.Room,
                    AvgRating = hotel.Avgrating
                }).ToList();
                return new ResponseMessage { Success = true, Data = listHotelWithAvgRating, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
            else
            {
                return new ResponseMessage { Success = false, Data = getListHotel, Message = "No Data", StatusCode = (int)HttpStatusCode.NotFound };
            }
        }

        public ResponseMessage GetHotelByID(int id)
        {
            var getHotel = db.hotel
                .Include(x => x.HotelImages)
                .Include(x => x.HotelAddress)
                .Include(x => x.HotelServices).ThenInclude(x => x.HotelSubServices).Include(x => x.feedBacks)
                .ThenInclude(booking => booking.Booking).ThenInclude(account => account.Account).ThenInclude(profile => profile.Profile)
                .FirstOrDefault(x => x.HotelID == id && x.Status == true && x.isRegister.Equals("Approved"));

            if (getHotel != null)
            {
                double avgRating = getHotel.feedBacks.Any() ? Math.Round(getHotel.feedBacks.Average(feedBack => feedBack.Rating), 2) : 0;
            
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
            var getHotel = db.hotel
                             .Include(x => x.HotelAddress)
                             .Include(x => x.feedBacks)
                             .Include(x => x.rooms)
                             .ThenInclude(x => x.SpecialPrice)
                             .OrderByDescending(hotel => hotel.HotelStandar)
                             .ToList();
            var filterHotel = getHotel.Select(hotel => new
            {
                HotelID = hotel.HotelID,
                Name = hotel.Name,
                Description = hotel.Description,
                HotelStandar = hotel.HotelStandar,
                MainImage = hotel.MainImage,
                OpenedIn = hotel.OpenedIn,
                HotelAddress = new
                {
                    hotel.HotelAddress.AddressID,
                    hotel.HotelAddress.Address,
                    hotel.HotelAddress.City,
                    hotel.HotelAddress.latitude,
                    hotel.HotelAddress.longitude
                },
                Rooms = hotel.rooms.Select(room =>
                {
                    var finalPrice = room.Price;
                    var specialPrice = room.SpecialPrice
                        .FirstOrDefault(sp => currentDate >= sp.StartDate && currentDate <= sp.EndDate);
                    if (specialPrice != null)
                    {
                        finalPrice = specialPrice.Price;
                    }
                    return new
                    {
                        RoomID = room.RoomID,
                        TypeOfRoom = room.TypeOfRoom,
                        NumberCapacity = room.NumberCapacity,
                        Price = finalPrice,
                        Quantity = room.Quantity,
                        SizeOfRoom = room.SizeOfRoom,
                        TypeOfBed = room.TypeOfBed,
                        SpecialPrice = room.SpecialPrice.Select(sp => new
                        {
                            SpecialPriceID = sp.SpecialPriceID,
                            StartDate = sp.StartDate,
                            EndDate = sp.EndDate,
                            Price = sp.Price
                        }).ToList()
                    };
                }).Where(room => room.Price >= minPrice && room.Price <= maxPrice).ToList(),
                FeedBacks = hotel.feedBacks.Select(feedback => new
                {
                    FeedBackID = feedback.FeedBackID,
                    Rating = feedback.Rating,
                    Image = feedback.Image,
                    Description = feedback.Description,
                    isDelete = feedback.isDeleted,
                    Account = feedback.Account
                }).ToList()
            }).Where(hotel => hotel.Rooms.Any()).ToList();


            var listHotelWithRating = filterHotel.Select(hotel => new
            {
                Hotel = hotel,
                AvgRating = hotel.FeedBacks.Any() ? Math.Round(hotel.FeedBacks.Average(feedback => feedback.Rating), 2) : 0
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

        public class ServiceWithSubServices
        {
            public string Type { get; set; }
            public List<string> SubServiceNames { get; set; }
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


        public ResponseMessage HotelRegistration(string hotelName, int openedIn, string description, int hotelStandar, string hotelAddress, string city, double latitude, double longitude,
                                                List<IFormFile> images, IFormFile mainImage, int accountID, List<ServiceType> services)
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
                MainImage = Utils.SaveImage(mainImage,environment),
                Description = description,
                HotelAddress = addAddress,
                Status = false,
                isRegister = "Wait for approved",
                HotelStandar = hotelStandar,
                Account = account
            };

            db.hotel.Add(addHotel);
            db.SaveChanges(); // Save changes to generate IDs for the hotel


            foreach (var service in services)
            {
                var addService = new HotelService
                {
                    Type = service.Type,
                    Hotel = addHotel,
                    
                };
                db.hotelService.Add(addService);
                db.SaveChanges();
                var hotelSubService = new List<HotelSubService>();
                foreach (var subServiceName in service.SubServiceNames)
                {
                    var addSubService = new HotelSubService
                    {
                        SubServiceName = subServiceName,
                        HotelService = addService
                    };
                    db.hotelSubService.Add(addSubService);
                    hotelSubService.Add(addSubService);
                }
                addService.HotelSubServices = hotelSubService;

            }

            foreach (var img in images)
            {
                var addImage = new HotelImage
                {
                    Image = Ultils.Utils.SaveImage(img, environment),
                    Hotel = addHotel,
                    Title = "Hotel View"
                };

                db.hotelImage.Add(addImage);
            }

            db.SaveChanges(); // Save all changes at the end
            return new ResponseMessage
            {
                Success = true,
                Data = addHotel,
                Message = "Successfully registered hotel",
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        public ResponseMessage UpdateBasicInformation(int hotelID, string hotelName, int openedIn, string description,
            string hotelAddress, string city, double latitude, double longitude, IFormFile mainImage)
        {
            var getHotel = db.hotel.Include(address => address.HotelAddress).FirstOrDefault(hotel => hotel.HotelID == hotelID);
            if (getHotel != null)
            {
                getHotel.Name = hotelName;
                getHotel.OpenedIn = openedIn;
                getHotel.Description = description;
                getHotel.HotelStandar = getHotel.HotelStandar;
                getHotel.HotelAddress.Address = hotelAddress;
                getHotel.HotelAddress.latitude = latitude;
                getHotel.HotelAddress.longitude = longitude;
                getHotel.HotelAddress.City = city;
                getHotel.MainImage = Utils.SaveImage(mainImage, environment);
                db.hotel.Update(getHotel);
                db.SaveChanges();
                return new ResponseMessage { Success = true, Data = getHotel, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
            return new ResponseMessage { Success = false, Data = getHotel, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound };
        }

        public ResponseMessage UpdateHotelService(int hotelID, List<ServiceType> services)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Find the hotel
                    var hotel = db.hotel.Include(h => h.HotelServices)
                                        .ThenInclude(s => s.HotelSubServices)
                                        .FirstOrDefault(h => h.HotelID == hotelID);

                    if (hotel == null)
                    {
                        return new ResponseMessage
                        {
                            Success = false,
                            Message = "Hotel not found",
                            StatusCode = (int)HttpStatusCode.NotFound
                        };
                    }

                    // Remove existing services and sub-services
                    foreach (var service in hotel.HotelServices)
                    {
                        db.hotelSubService.RemoveRange(service.HotelSubServices);
                        db.hotelService.Remove(service);
                    }
                    db.SaveChanges();

                    // Add new services and sub-services
                    foreach (var serviceType in services)
                    {
                        var addService = new HotelService
                        {
                            Type = serviceType.Type,
                            Hotel = hotel // Associate the service with the hotel
                        };

                        db.hotelService.Add(addService);
                        db.SaveChanges(); // Save changes to generate IDs for the service

                        var hotelSubServices = new List<HotelSubService>();

                        foreach (var subServiceName in serviceType.SubServiceNames)
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
                    }

                    db.SaveChanges(); // Save all changes at the end
                    transaction.Commit();

                    return new ResponseMessage
                    {
                        Success = true,
                        Data = hotel,
                        Message = "Successfully updated hotel services",
                        StatusCode = (int)HttpStatusCode.OK
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error occurred: {ex.Message}");
                    return new ResponseMessage
                    {
                        Success = false,
                        Message = ex.Message,
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
                }
            }
        }

        public ResponseMessage AddHotelImage(int hotelId, String title ,IFormFile images)
        {
            var hotel = db.hotel.FirstOrDefault(hotel => hotel.HotelID == hotelId);
            if (hotel != null)
            {
                HotelImage addImage = new HotelImage
                {
                    Title = title,
                    Image = Ultils.Utils.SaveImage(images,environment)
                };
                db.hotelImage.Add(addImage);
                db.SaveChanges();
                return new ResponseMessage { Success = true, Data = hotel, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
            return new ResponseMessage { Success = false, Data = null, Message = "Hotel not found", StatusCode = (int)HttpStatusCode.NotFound };
        }
        public ResponseMessage DeleteHotelImages(int imageID)
        {
            var getImage = db.hotelImage.FirstOrDefault(image => image.ImageID == imageID);
            if (getImage != null)
            {
                db.hotelImage.Remove(getImage);
                db.SaveChanges();
                return new ResponseMessage { Success = true, Data = getImage, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
            return new ResponseMessage { Success = false, Data = null, Message = "Hotel not found", StatusCode = (int)HttpStatusCode.NotFound };
        }

        public ResponseMessage GetAllHotelInfomation()
        {
            var listHotel = db.hotel
                              .Include(hotelService => hotelService.HotelServices)
                              .ThenInclude(hotelSubService => hotelSubService.HotelSubServices)
                              .Include(address => address.HotelAddress)
                              .Include(room => room.rooms)
                              .ToList()
                              .Select(hotel => new
                              {
                                  Hotel = hotel,
                                  Room = hotel.rooms,
                                  TotalBooking = db.booking.Count(booking => booking.Room.Hotel.HotelID == hotel.HotelID),
                                  TotalRevenue = db.booking.Where(booking => booking.Room.Hotel.HotelID == hotel.HotelID).Sum(booking => booking.TotalPrice)
                              });
            return new ResponseMessage { Success = true, Data = listHotel, Message = "Succsessfully", StatusCode = (int)HttpStatusCode.OK };

        }

        public ResponseMessage BlockedHotel(int hotelID)
        {
            var getHotel = db.hotel
                             .Include(account => account.Account)
                             .FirstOrDefault(hotel => hotel.HotelID == hotelID);
            var getAccount = db.accounts.FirstOrDefault(account => account.AccountID == getHotel.Account.AccountID);
            if (getHotel != null && getAccount != null)
            {
                getHotel.isRegister = "Blocked";
                getHotel.Status = false;
                getAccount.IsActive = false;
                db.hotel.Update(getHotel);
                db.accounts.Update(getAccount);
                db.SaveChanges();
                return new ResponseMessage { Success = true, Data = getHotel, StatusCode = (int)HttpStatusCode.OK };
            }
            return new ResponseMessage { Success = true, Data = getHotel, Message = "Data not found", StatusCode = (int)HttpStatusCode.OK };
        }

        public ResponseMessage ConfirmRegistration(int hotelID)
        {
            var getHotel = db.hotel
                             .Include(account => account.Account)
                             .Include(hotelService => hotelService.HotelServices)
                             .ThenInclude(hotelSubService => hotelSubService.HotelSubServices)
                             .Include(address => address.HotelAddress)
                             .FirstOrDefault(hotel => hotel.HotelID == hotelID);
            var getAccount = db.accounts.FirstOrDefault(account => account.AccountID == getHotel.Account.AccountID);
            if (getHotel != null)
            {
                getHotel.isRegister = "Approved";
                getHotel.Status = true;
                getAccount.IsActive = true;
                db.hotel.Update(getHotel);
                db.SaveChanges();
                String mailContent = "Bạn đã được duyệt để trở thành đối tác của chúng tôi.Vui lòng đăng nhập vào hệ thống và thực hiện các hoạt động.";
                Ultils.Utils.SendMailRegistration(getAccount.Email,mailContent);
                return new ResponseMessage { Success = true, Data = getHotel, Message = "Sucessfully", StatusCode = (int)HttpStatusCode.OK };
            }
                return new ResponseMessage { Success = false, Data = getHotel, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound };
        }

        public ResponseMessage FilterHotelByStatus(bool Status)
        {
            var filterHotel = db.hotel.Where(hotel => hotel.Status == Status).ToList();
            return new ResponseMessage {Success = true, Data = filterHotel, Message ="Successfully",StatusCode= (int)HttpStatusCode.OK};
        }

        public ResponseMessage RejectRegistration(int hotelID, String reasonReject)
        {
            var getHotel = db.hotel
                             .Include(account => account.Account)
                             .FirstOrDefault(hotel => hotel.HotelID == hotelID);
            var getAccount = db.accounts
                               .Include(role => role.Role)
                               .FirstOrDefault(account => account.AccountID == getHotel.Account.AccountID);
            String Role = "Customer";
            var getRole = db.roles.FirstOrDefault(role => role.Name.Equals(Role));
            if(getHotel != null)
            {
                getHotel.isRegister = "Was rejected";
                getAccount.Role = getRole;
                db.hotel.Update(getHotel);
                db.accounts.Update(getAccount);
                db.SaveChanges();
                Ultils.Utils.SendMailRegistration(getAccount.Email, reasonReject);
                return new ResponseMessage { Success = true, Data = getHotel, Message = "Successfully", StatusCode= (int)HttpStatusCode.OK};
            }
            else
            {
                return new ResponseMessage { Success =false, Data = getHotel, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound};
            }
        }

        public ResponseMessage SearchHotelByName(string hotelName)
        {
            var searchResult = db.hotel.Where(hotel => hotel.Name.Contains(hotelName)).ToList();
            return new ResponseMessage { Success = true, Data = searchResult, Message = "Successfully", StatusCode=(int)HttpStatusCode.OK};
        }

        public ResponseMessage GetAllHotelWaitForConfirm()
        {
            var listHotel = db.hotel
                              .Include(hotelService => hotelService.HotelServices)
                              .ThenInclude(hotelSubService => hotelSubService.HotelSubServices) 
                              .Include(account => account.Account)
                              .Include(profile => profile.Account.Profile)
                              .Where(hotel => hotel.isRegister.Equals("Wait for approved") && hotel.Status == false)
                              .ToList();
            return new ResponseMessage { Success = true,Data = listHotel, Message = "Sucessfully",StatusCode =(int)HttpStatusCode.OK};
        }

        public ResponseMessage AnalyzeHotelStandar()
        {
            var listHotel = db.hotel.ToList();
            var hotelStandar = new Dictionary<string, int>();
            for (int star = 1; star <= 5; star++)
            {
                hotelStandar[$"{star} Star"] = 0;
            }

            foreach (var hotel in listHotel)
            {
                if (hotelStandar.ContainsKey($"{hotel.HotelStandar} Star"))
                {
                    hotelStandar[$"{hotel.HotelStandar} Star"]++;
                }
            }

            var hotelStarCounts = hotelStandar.Select(h => new HotelStandarDTO
            {
                Name = $"Hotel {h.Key}",
                Value = h.Value
            }).ToList();

            return new ResponseMessage { Success = true, Data = hotelStarCounts, Message = "Sucessfully", StatusCode = (int)HttpStatusCode.OK };

        }
    }
}



