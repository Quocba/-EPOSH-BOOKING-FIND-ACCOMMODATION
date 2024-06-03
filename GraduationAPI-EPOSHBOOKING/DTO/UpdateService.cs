using GraduationAPI_EPOSHBOOKING.Model;

namespace GraduationAPI_EPOSHBOOKING.DTO
{
    public class UpdateService
    {
        public int HotelID { get; set; }
        public List<ServiceType> Services { get; set; }
    }
}
