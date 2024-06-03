
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
                                                   List<ServiceType>services);
        public ResponseMessage UpdateBasicInfomation(int hotelID,string hotelName,
                                                     int openedIn,
                                                     string description,
                                                     string hotelAddress,
                                                     string city,
                                                     double latitude,
                                                     double longitude,
                                                     IFormFile mainImage);
        public ResponseMessage UpdateHotelService(int hotelID,List<ServiceType>services);
        public ResponseMessage AddHotelImage(int hotelId, List<IFormFile> images);
        public ResponseMessage DeleteHotelImages(int hotelId);
        public ResponseMessage GetAllHotelInfomation();
        public ResponseMessage FilterHotelByStatus(bool Status);
        public ResponseMessage BlockedHotel(int hotelID);
        public ResponseMessage ConfirmRegistration(int hotelID);
        public ResponseMessage RejectRegistration(int hotelID,String reasonReject);
        public ResponseMessage SearchHotelByName(String hotelName);
        public ResponseMessage GetAllHotelWaitForConfirm();
        public ResponseMessage AnalyzeHotelStandar();
    }

 
}
