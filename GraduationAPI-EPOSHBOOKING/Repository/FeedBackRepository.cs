using GraduationAPI_EPOSHBOOKING.DataAccess;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using GraduationAPI_EPOSHBOOKING.Ultils;
using Microsoft.EntityFrameworkCore;
using System.Net;
#pragma warning disable // tắt cảnh báo để code sạch hơn

namespace GraduationAPI_EPOSHBOOKING.Repository
{
    public class FeedBackRepository : IFeedbackRepository
    {
        private readonly DBContext db;
        public FeedBackRepository(DBContext _db)
        {
            this.db = _db;
        }
        public ResponseMessage CreateFeedBack(int BookingID, FeedBack feedBack, IFormFile Image)
        {
            try
            {

                byte[] feedBackImage = Ultils.Utils.ConvertIFormFileToByteArray(Image);
                var booking = db.booking.Include(room => room.Room).ThenInclude(hotel => hotel.Hotel).Include(account => account.Account)
                    .FirstOrDefault(booking => booking.BookingID == BookingID);
                FeedBack addFeedBack = new FeedBack
                {
                    Account = booking.Account,
                    Booking = booking,
                    Rating = feedBack.Rating,
                    Description = feedBack.Description,
                    Hotel = booking.Room.Hotel,
                    Image = feedBackImage,
                    IsBlocked = false
                };

                db.feedback.Add(addFeedBack);
                db.SaveChanges();
                var result = new
                {
                    addFeedBack.FeedBackID,
                    addFeedBack.Rating,
                    addFeedBack.Description,
                    addFeedBack.IsBlocked,
                    addFeedBack.Image,
                    Account = addFeedBack.Account != null ? new
                    {
                        addFeedBack.Account.AccountID,
                        addFeedBack.Account.Email
                    } : null,
                    Booking = addFeedBack.Booking != null ? new
                    {
                        addFeedBack.Booking.BookingID,
                        addFeedBack.Booking.CheckInDate,
                        addFeedBack.Booking.CheckOutDate
                    } : null,
                    Hotel = addFeedBack.Hotel != null ? new
                    {
                        addFeedBack.Hotel.HotelID,
                        addFeedBack.Hotel.Name,
                        addFeedBack.Hotel.Description,
                        addFeedBack.Hotel.HotelStandar,
                        addFeedBack.Hotel.MainImage,

                        rooms = addFeedBack.Hotel.rooms?.Where(r => r != null).Select(r => new
                        {
                            r.RoomID,
                            r.TypeOfRoom,
                            r.Price,
                            SpecialPrices = r.SpecialPrice?.Where(sp => sp != null).Select(sp => new
                            {
                                sp.SpecialPriceID,
                                sp.Price,
                                sp.StartDate,
                                sp.EndDate
                            }).ToList()
                        }).ToList()
                    } : null
                };

                return new ResponseMessage { Success = true, Data = result, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };


            }
            catch (Exception ex)
            {
                return new ResponseMessage { Success = false, Data = null, Message = "Internal Server Error", StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }

        public ResponseMessage GetAllReportFeedback()
        {
            var report = db.reportFeedBack
                .Include(feedback => feedback.FeedBack)
                .ToList();
            if (report.Any())
            {
                return new ResponseMessage { Success = true, Data = report, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
            return new ResponseMessage { Success = false, Data = report, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound };
        }

        public ResponseMessage ReportFeedback(int AccountID, int FeedBackID, string reason)
        {
            var account = db.accounts.Find(AccountID);
            if (account == null)
            {
                return new ResponseMessage
                {
                    Success = false,
                    Message = "Account not found",
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }
            var feedback = db.feedback.Find(FeedBackID);
            if (feedback == null)
            {
                return new ResponseMessage
                {
                    Success = false,
                    Message = "Feedback not found",
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }

            var report = new ReportFeedBack
            {
                FeedBack = feedback,
                ReporterEmail = db.accounts.Find(AccountID).Email,
                ReasonReport = reason
            };

            db.reportFeedBack.Add(report);
            db.SaveChanges();

            return new ResponseMessage
            {
                Success = true,
                Message = "Feedback reported successfully",
                StatusCode = (int)HttpStatusCode.OK
            };
        }
        public ResponseMessage ConfirmReportFeedback(int reportId)
        {
            try
            {
                var report = db.reportFeedBack.Include(r => r.FeedBack).FirstOrDefault(r => r.ReportID == reportId);
                if (report == null)
                {
                    return new ResponseMessage
                    {
                        Success = false,
                        Message = "Report not found",
                        StatusCode = (int)HttpStatusCode.NotFound
                    };
                }

                // Xóa Feedback
                db.feedback.Remove(report.FeedBack);

                // Xóa ReportFeedback
                db.reportFeedBack.Remove(report);

                db.SaveChanges();

                return new ResponseMessage
                {
                    Success = true,
                    Message = "Report confirmed and feedback deleted successfully",
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ResponseMessage
                {
                    Success = false,
                    Message = "Error confirming report: " + ex.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
