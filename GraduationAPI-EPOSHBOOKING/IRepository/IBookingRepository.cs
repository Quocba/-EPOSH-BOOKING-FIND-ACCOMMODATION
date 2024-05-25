using GraduationAPI_EPOSHBOOKING.Model;

namespace GraduationAPI_EPOSHBOOKING.IRepository
{
    public interface IBookingRepository
    {
        public ResponseMessage GetBookingByAccount(int AccountID);
    }
}
