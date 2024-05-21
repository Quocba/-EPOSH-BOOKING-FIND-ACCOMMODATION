using GraduationAPI_EPOSHBOOKING.DataAccess;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Net;
#pragma warning disable // tắt cảnh báo để code sạch hơn
namespace GraduationAPI_EPOSHBOOKING.Repository
{
    public class HotelRepository : IHotelRepository
    {
        private readonly DBContext db;
        public HotelRepository(DBContext _db)
        {
            this.db = _db;
        }

        public ResponseMessage GetAllHotel()
        {
            var listHotel = db.hotel.Include(x => x.HotelAddress).Include(x => x.HotelImages).Include(x => x.HotelServices).ThenInclude(x => x.HotelSubServices).Include(x => x.feedBacks)
                .ToList();
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
            var getListHotel = db.hotel.Include(x => x.HotelAddress).Include(x => x.HotelImages).Include(x => x.HotelServices).ThenInclude(x => x.HotelSubServices).Include(x => x.feedBacks).
                Where(x => x.HotelAddress.City.Equals(city)).ToList();
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
            var getHotel = db.hotel.Include(x => x.HotelImages).Include(x => x.HotelAddress).Include(x => x.HotelServices).ThenInclude(x => x.HotelSubServices).Include(x => x.feedBacks).
                FirstOrDefault(x => x.HotelID == id);
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

            var getHotel = db.hotel.Include(x => x.HotelAddress).Include(x => x.HotelImages).Include(x => x.HotelServices).ThenInclude(x => x.HotelSubServices).Include(x => x.feedBacks).Include(x => x.rooms)
                .ThenInclude(x => x.SpecialPrice).Select(hotel => new Hotel
                {
                    HotelID = hotel.HotelID,
                    Name = hotel.Name,
                    Description = hotel.Description,
                    rooms = hotel.rooms.Where(room => room.Price >= minPrice && room.Price <= maxPrice || room.SpecialPrice.Any(sp => currentDate >= sp.StartDate && currentDate <= sp.EndDate
                    && sp.Price >= minPrice && sp.Price <= maxPrice)).ToList(),
                    feedBacks = hotel.feedBacks.ToList(),
                }).ToList();
            var listHotelWithRating = getHotel.Select(hotel => new
            {
                Hotel = hotel,
                AvgRating = hotel.feedBacks.Any() ? Math.Round(hotel.feedBacks.Average(feedback => feedback.Rating), 2) : 0
            }); ;

            if (getHotel.Any())
            {
                return new ResponseMessage { Success = true, Data = listHotelWithRating, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
            return new ResponseMessage { Success = false, Data = getHotel, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound };
        }

        public ResponseMessage GetByRating(int rating)
        {
            var listHotel = db.hotel.Include(x => x.HotelImages).Include(x => x.HotelAddress).Include(x => x.HotelServices).ThenInclude(x => x.HotelSubServices)
                .Include(x => x.feedBacks).Include(x => x.rooms).ThenInclude(x => x.SpecialPrice).ToList();
            var filterHotelWithRating = listHotel.Select(hotel => new
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

            if (services.Any())
            {
                var listHotel = db.hotel.Include(x => x.HotelImages).Include(x => x.HotelAddress).Include(x => x.HotelServices).ThenInclude(x => x.HotelSubServices)
                .Include(x => x.feedBacks).Include(x => x.rooms).ToList();

                var listHotelWithService = listHotel.Where(hotel => hotel.HotelServices.Any(service => services.Contains(service.Type))).ToList();
                if (listHotelWithService.Any())
                {
                    return new ResponseMessage { Success = true, Data = listHotelWithService, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
                }
                else
                {
                    return new ResponseMessage { Success = false, Data = listHotelWithService, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound };
                }
            }

            return new ResponseMessage { Success = false, Data = null, Message = "Service cannot be empty", StatusCode = (int)HttpStatusCode.BadRequest };
        }

        

    }
}
