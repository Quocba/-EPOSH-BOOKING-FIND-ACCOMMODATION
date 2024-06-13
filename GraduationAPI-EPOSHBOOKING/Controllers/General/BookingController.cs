using GraduationAPI_EPOSHBOOKING.DTO;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Guest
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
        public IActionResult CancleBooking([FromForm] int bookingID, [FromForm] string Reason)
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
        [HttpPost("create-booking-fe")]
        public IActionResult CreateBookingFE([FromForm]CreateBookingDTO createBookingDTO)
        {
            var response = repository.CreateBookingFE(createBookingDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("check-room-price")]
        public IActionResult CheckRoomPrice([FromForm]int roomID, [FromForm]DateTime CheckInDate, [FromForm]DateTime CheckOutDate) { 
            double roomPrice = repository.CheckRoomPrice(roomID, CheckInDate, CheckOutDate);
            return Ok(new { Price = roomPrice });
        }
        [HttpGet("get-all-bookings")]
        public IActionResult GetAllBooking()
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
        [HttpGet("export-all-bookings-total-revenue")]
        public IActionResult ExportAllBookings()
        {
            var response = repository.ExportAllBookings();

            if (response.Success)
            {
                return File((byte[])response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"AllBookings.xlsx");
            }
            else
            {
                return StatusCode(response.StatusCode, response);
            }
        }
        [HttpGet("export-revenues")]
        public IActionResult ExportRevenues()
        {
            var response = repository.ExportRevenues();

            if (response.Success)
            {
                return File((byte[])response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Revenues.xlsx");
            }
            else
            {
                return StatusCode(response.StatusCode, response);
            }
        }
            [HttpPut("change-complete")]
        public IActionResult ChangeStatusComplete([FromForm] int bookingID)
        {
            var response = repository.ChangeStatusComplete(bookingID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("analysis-revenue")]
        public IActionResult AnalysisRevenueBookingSystem()
        {
            var response = repository.AnalysisRevenueBookingSystem();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("analysis-revenue-hotel")]
        public IActionResult AnalysisRevenueBookingHotel([FromQuery]int hotelID)
        {
            var response = repository.AnalysisRevenueBookingHotel(hotelID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("count-booking-system")]
        public IActionResult CountBookingSystem()
        {
            var response = repository.CountBookingSystem();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("count-booking-hotel")]
        public IActionResult CountBookingHotel([FromQuery]int hotelID)
        {
            var response = repository.CountBookingHotel(hotelID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("get-top-booking")]
        public IActionResult Top5Booking()
        {
            var response = repository.Top5Booking();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("get-booking-hotel")]
        public IActionResult GetBookingByHotel([FromQuery]int hotelID)
        {
            var response = repository.GetBookingByHotel(hotelID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("export-bookings-and-total-revenue-by-hotelID")]
        public IActionResult ExportBookingbyHotelID([FromQuery] int hotelID)
        {
            var response = repository.ExportBookingbyHotelID(hotelID);

            if (response.Success)
            {
                return File((byte[])response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Bookings_HotelID_{hotelID}.xlsx");
            }
            else
            {
                return StatusCode(response.StatusCode, response);
            }
        }
    }
    
}

