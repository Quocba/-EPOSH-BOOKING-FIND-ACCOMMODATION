using GraduationAPI_EPOSHBOOKING.DTO;
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
                                                       List<string> serviceTypes, List<string> subServiceNames);
    }

     
}
