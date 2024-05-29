using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;

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

    }
}
