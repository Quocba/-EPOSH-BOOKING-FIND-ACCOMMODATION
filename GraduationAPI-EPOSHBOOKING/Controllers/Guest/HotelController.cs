using GraduationAPI_EPOSHBOOKING.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Guest
{
    [ApiController]
    [Route("api/v1/hotel")]
    public class HotelController : Controller
    {
        private readonly IHotelRepository repository;
        public HotelController(IHotelRepository hotelRepository)
        {
            this.repository = hotelRepository;
        }
        [HttpGet("get-all")]
        public IActionResult GetAllHotel()
        {
            var response = repository.GetAllHotel();
            return StatusCode(response.StatusCode, response);
        }
    }
}
