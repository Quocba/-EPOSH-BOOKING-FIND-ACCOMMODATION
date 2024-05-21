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
            var respone = repository.GetAllHotel();
            return StatusCode(respone.StatusCode, respone);
        }
      [HttpGet("get-by-city")]
      public IActionResult GetHotel([FromQuery]String city) { 
        
            var reponse = repository.GetHotelByCity(city);
            return StatusCode(reponse.StatusCode, reponse);
        
        }

      [HttpGet("get-by-id")]
      public IActionResult GetHotelByID([FromQuery]int id)
        {
            var response = repository.GetHotelByID(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("get-by-price")]
        public IActionResult getHotelByPrice([FromForm]double minPrice, [FromForm]double maxPrice)
        {
            var response = repository.GetHotelByPrice(minPrice, maxPrice);
            return StatusCode(response.StatusCode, response);
        }

    }
}
 