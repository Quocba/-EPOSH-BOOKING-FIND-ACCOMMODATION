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
        public IActionResult GetBookingByAccount([FromQuery] int accountID)
        {
            var response = repository.GetBookingByAccount(accountID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("cancle-booking")]
        public IActionResult CancleBooking([FromForm] int bookingID, [FromForm] String Reason)
        {
            var response = repository.CancleBooking(bookingID, Reason);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("create-booking")]
        public IActionResult CreateBooking([FromForm] int accountID, [FromForm] int voucherID, [FromForm] int roomID, [FromForm] Booking? booking)
        {
            var response = repository.CreateBooking(accountID, voucherID, roomID, booking);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("get-all-bookings")]
        public IActionResult GetAllBookings()
        {
            var response = repository.GetAllBooking();
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("change-wait-for-payment")]
        public IActionResult ChangeStatusWaitForPayment([FromForm] int bookingID)
        {
            var response = repository.ChangeStatusWaitForPayment(bookingID);
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("export-bookings-by-accountID")]
        public IActionResult ExportBookingsByAccountID([FromQuery] int accountID)
        {
            var response = repository.ExportBookingsByAccountID(accountID);

            if (response.Success)
            {
                return File((byte[])response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Bookings_{accountID}.xlsx");
            }
            else
            {
                return StatusCode(response.StatusCode, response);
            }

        }

        [HttpPut("change-complete")]
        public IActionResult ChangeStatusComplete([FromForm]int bookingID)
        {
            var response = repository.ChangeStatusComplete(bookingID);
            return StatusCode(response.StatusCode, response);
        }

        }

    }

