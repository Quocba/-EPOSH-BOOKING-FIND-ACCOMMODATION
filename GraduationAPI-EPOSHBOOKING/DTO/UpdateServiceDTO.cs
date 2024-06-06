using GraduationAPI_EPOSHBOOKING.Model;

namespace GraduationAPI_EPOSHBOOKING.DTO
{
    public class UpdateServiceDTO
    {
        public int HotelID { get; set; }
        public List<ServiceType> Services { get; set; }
    }
}
