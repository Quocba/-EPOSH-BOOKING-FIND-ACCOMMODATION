using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using GraduationAPI_EPOSHBOOKING.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
      public IActionResult GetHotelByCity([FromQuery]String city) { 
        
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

        [HttpGet("get-by-rating")]
        public IActionResult getHotelByRating([FromQuery]int rating)
        {
            var response = repository.GetByRating(rating);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("get-by-service")]
        public IActionResult GetHotelByService([FromForm]List<string> service)
        {
            var response = repository.GetByService(service);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("get-service-by-hotelID")]
        public IActionResult GetServiceByHotel([FromQuery]int hotelID)
        {
            var response = repository.GetServiceByHotelID(hotelID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("get-galleries-by-hotelID")]
        public IActionResult GetGalleriesByHotel([FromQuery]int hotelID)
        {
            var response = repository.GetGalleriesByHotelID(hotelID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("search")]
        public IActionResult SearchHotel([FromQuery]String city, [FromQuery] DateTime? checkInDate, [FromQuery] DateTime? checkOuDate, [FromQuery] int? numberCapacity, int? Quantity)
        {
            var resposne = repository.SearchHotel(city, checkInDate, checkOuDate, numberCapacity,Quantity);
            return StatusCode(resposne.StatusCode, resposne);
        }
        [HttpPost("add-hotel-image")]
        public IActionResult AddHotelImage([FromForm] int hotelId, [FromForm] List<IFormFile> images)
        {
            var response = repository.AddHotelImage(hotelId, images);
            return StatusCode(response.StatusCode, response);
        }
        [HttpDelete("delete-hotel-images")]
        public IActionResult DeleteHotelImages([FromQuery] int hotelId)
        {
            var response = repository.DeleteHotelImages(hotelId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
 