using GraduationAPI_EPOSHBOOKING.DataAccess;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GraduationAPI_EPOSHBOOKING.Repository
{
    public class HotelRepository : IHotelRepository
    {
        private readonly DBContext db;
        public HotelRepository(DBContext _db) { 
            this.db = _db;
        }

        public ResponseMessage GetAllHotel()
        {
            var listHotel = db.hotel.Include(x => x.HotelAddress).Include(x => x.HotelImages).Include(x => x.HotelAmenities).ThenInclude(x => x.hotelService).ThenInclude(x => x.HotelSubServices)
                .Include(x => x.Account).Include(x => x.Account.Profile).ToList();
           if(listHotel.Any()) {
                return new ResponseMessage { Success = true, Message = "Successfully", Data = listHotel, StatusCode = (int)HttpStatusCode.OK };
            }
            else
            {
                return new ResponseMessage { Success = false, Message = "Not Found", Data = listHotel, StatusCode = (int)HttpStatusCode.NotFound };
            }
        }
    }
}
