using GraduationAPI_EPOSHBOOKING.Model;

namespace GraduationAPI_EPOSHBOOKING.DTO
{
    public class Top5BookingDTO
    {
        public byte[] avatar { get; set; }
        public String fullName { get; set; }
        public int TotalBooking { get; set; }
        public double Spending { get; set; }

    }
}
