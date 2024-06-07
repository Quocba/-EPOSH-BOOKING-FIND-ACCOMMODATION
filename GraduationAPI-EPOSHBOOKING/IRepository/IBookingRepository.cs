using GraduationAPI_EPOSHBOOKING.Model;

namespace GraduationAPI_EPOSHBOOKING.IRepository
{
    public interface IBookingRepository
    {
        public ResponseMessage GetBookingByAccount(int AccountID);

        public ResponseMessage CancleBooking(int bookingID, String Reason);
        public ResponseMessage CreateBooking(int accountID, int voucherID,int RoomID,Booking? booking);

        public ResponseMessage GetAllBooking();
        public ResponseMessage ChangeStatusWaitForPayment(int bookingID);
        public ResponseMessage ChangeStatusComplete(int bookingID);

        public ResponseMessage GetAllBookings();
        public ResponseMessage ExportBookingsByAccountID(int accountID);
        public ResponseMessage ExportAllBookings();
        public ResponseMessage AnalysisRevenueBookingSystem();
        public ResponseMessage AnalysisRevenueBookingHotel(int hotelID);
        public ResponseMessage CountBookingSystem();
        public ResponseMessage CountBookingHotel(int hotelID);
        public ResponseMessage Top5Booking();
        public ResponseMessage GetBookingByHotel(int hotelID);
    }
}
