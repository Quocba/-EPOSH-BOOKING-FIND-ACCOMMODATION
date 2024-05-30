using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Customer
{
    [ApiController]
    [Route("api/v1/booking")]
    public class BookingController : Controller
    {
        private readonly IBookingRepository repository;
        public BookingController(IBookingRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("get-by-accountID")]
        public IActionResult GetBookingByAccount([FromQuery]int accountID)
        {
            var response = repository.GetBookingByAccount(accountID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("cancle-booking")]
        public IActionResult CancleBooking([FromForm]int bookingID, [FromForm]String Reason)
        {
            var response = repository.CancleBooking(bookingID, Reason);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("create-booking")]
        public IActionResult CreateBooking([FromForm]int accountID, [FromForm]int voucherID, [FromForm]int roomID, [FromForm]Booking? booking)
        {
            var response = repository.CreateBooking(accountID, voucherID, roomID, booking);
            return StatusCode(response.StatusCode, response);
        }
        // code ham GetAllBookings
        [HttpGet("get-all-bookings")]
        public IActionResult GetAllBookings()
        {
            var response = repository.GetAllBookings();
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("export-by-accountID")]
        public IActionResult ExportBookingsByAccountID([FromQuery] int accountID)
        {
            var fileContent = repository.ExportBookingsByAccountID(accountID);
            if (fileContent == null)
            {
                return NotFound(new ResponseMessage { Success = false, Data = null, Message = "No bookings found", StatusCode = (int)HttpStatusCode.NotFound });
            }

            var fileName = $"Bookings_{accountID}.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(fileContent, contentType, fileName);
        }
    }
}
