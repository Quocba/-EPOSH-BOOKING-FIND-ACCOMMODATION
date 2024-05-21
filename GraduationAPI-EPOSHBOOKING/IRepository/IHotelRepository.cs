using GraduationAPI_EPOSHBOOKING.Model;

namespace GraduationAPI_EPOSHBOOKING.IRepository
{
    public interface IHotelRepository
    {
        public ResponseMessage GetAllHotel();
        public ResponseMessage GetHotelByCity(String city);
        public ResponseMessage GetHotelByID (int hotelID);
        public ResponseMessage GetHotelByPrice(double minPrice, double maxPrice);
    }
}
