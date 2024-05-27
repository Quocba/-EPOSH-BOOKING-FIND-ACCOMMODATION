using GraduationAPI_EPOSHBOOKING.DataAccess;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;
#pragma warning disable // tắt cảnh báo để code sạch hơn

namespace GraduationAPI_EPOSHBOOKING.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly DBContext db;
        public BookingRepository(DBContext _db)
        {
            this.db = _db;
        }

        public ResponseMessage GetBookingByAccount(int accountID)
        {
            var getBooking = db.booking.Where(booking => booking.Account.AccountID == accountID).Include(room => room.Room).ThenInclude(hotel => hotel.Hotel)
                .ToList();
            if (getBooking.Any())
            {
                return new ResponseMessage { Success = true, Data = getBooking, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
                return new ResponseMessage { Success = false,Data = getBooking, Message = "No Booking",StatusCode = (int)HttpStatusCode.NotFound };
        }

        public ResponseMessage CancleBooking(int bookingID, String Reason)
        {
            var getBooking = db.booking.Include(account => account.Account).FirstOrDefault(booking => booking.BookingID == bookingID);
            if (getBooking != null)
            {
                if (CanCancelBooking(getBooking.CheckInDate))
                {
                    getBooking.Status = "Cancle";
                    getBooking.ReasonCancle = Reason;
                    db.booking.Update(getBooking);
                    db.SaveChanges();
                    return new ResponseMessage { Success = true, Data = getBooking, Message = "Cancle Success", StatusCode = (int)HttpStatusCode.OK };
                }
           
            }
            return new ResponseMessage { Success = false,Data = getBooking, Message = "Cancel failed. You must cancel 24 hours before check-in date.", 
                StatusCode = (int)HttpStatusCode.NotFound };
        }

        private bool CanCancelBooking(DateTime checkInDate)
        {
            DateTime currentDateTime = DateTime.Now;
            TimeSpan timeDifference = checkInDate - currentDateTime;

            // Check if the check-in date is more than 24 hours from now
            return timeDifference.TotalHours > 24;
        }
    }
}
