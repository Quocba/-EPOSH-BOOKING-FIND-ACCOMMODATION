using GraduationAPI_EPOSHBOOKING.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/booking")]
    public class AdminBookingController : Controller
    {
        private readonly IBookingRepository repository;
        public AdminBookingController(IBookingRepository repository) { 
          
            this.repository = repository;
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


        [HttpGet("analysis-revenue")]
        public IActionResult AnalysisRevenueBookingSystem()
        {
            var response = repository.AnalysisRevenueBookingSystem();
            return StatusCode(response.StatusCode, response);
        }



        [HttpGet("count-booking-system")]
        public IActionResult CountBookingSystem()
        {
            var response = repository.CountBookingSystem();
            return StatusCode(response.StatusCode, response);
        }



        [HttpGet("get-top-booking")]
        public IActionResult Top5Booking()
        {
            var response = repository.Top5Booking();
            return StatusCode(response.StatusCode, response);
        }
    }
}
