using GraduationAPI_EPOSHBOOKING.Model;

namespace GraduationAPI_EPOSHBOOKING.IRepository
{
    public interface IHotelRepository
    {
        public ResponseMessage GetAllHotel();
        public ResponseMessage GetHotelByCity(String city);
        public ResponseMessage GetHotelByID (int hotelID);
        public ResponseMessage GetHotelByPrice(double minPrice, double maxPrice);
        public ResponseMessage GetByRating(int rating);
        public ResponseMessage GetByService(List<String> services);
        public ResponseMessage GetServiceByHotelID(int hotelID);
        public ResponseMessage GetGalleriesByHotelID(int hotelID);
        public ResponseMessage SearchHotel(String? city,DateTime? checkInDate, DateTime? checkOutDate,int? numberCapacity,int? quantity);
        public ResponseMessage AddHotelImage(int hotelId, List<IFormFile> images);
        public ResponseMessage DeleteHotelImages(int hotelId);
    }
}
