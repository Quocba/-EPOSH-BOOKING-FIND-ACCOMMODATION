using GraduationAPI_EPOSHBOOKING.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Partner
{
    [ApiController]
    [Route("api/v1/partner/booking")]
    public class PartnerBookingController : Controller
    {
        private readonly IBookingRepository repository;
        public PartnerBookingController(IBookingRepository repository)
        {
            this.repository = repository;
        }

        [HttpPut("change-wait-for-payment")]
        public IActionResult ChangeStatusWaitForPayment([FromForm] int bookingID)
        {
            var response = repository.ChangeStatusWaitForPayment(bookingID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("analysis-revenue-hotel")]
        public IActionResult AnalysisRevenueBookingHotel([FromQuery] int hotelID)
        {
            var response = repository.AnalysisRevenueBookingHotel(hotelID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("change-complete")]
        public IActionResult ChangeStatusComplete([FromForm] int bookingID)
        {
            var response = repository.ChangeStatusComplete(bookingID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("count-booking-hotel")]
        public IActionResult CountBookingHotel([FromQuery] int hotelID)
        {
            var response = repository.CountBookingHotel(hotelID);
            return StatusCode(response.StatusCode, response);
        }
        //Có vẽ rồi
        [HttpGet("get-booking-hotel")]
        public IActionResult GetBookingByHotel([FromQuery] int hotelID)
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
