using GraduationAPI_EPOSHBOOKING.IRepository;
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

    }
}
