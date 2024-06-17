using GraduationAPI_EPOSHBOOKING.DTO;
using GraduationAPI_EPOSHBOOKING.IRepository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Partner
{
    [ApiController]
    [Route("api/v1/partner/hotel")]
    public class PartnerHotelController : Controller
    {
        private readonly IHotelRepository repository;

        public PartnerHotelController(IHotelRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost("hotel-registration")]
        public IActionResult RegisterHotel([FromForm] HotelRegistrationDTO registrationModel)
        {
            var services = JsonConvert.DeserializeObject<List<ServiceTypeDTO>>(registrationModel.Services);
            var response = repository.HotelRegistration(registrationModel, services);
            return StatusCode(response.StatusCode, response);
        }
        [HttpPut("update-basic-infomation")]
        public IActionResult UpdateBasicInfomation([FromForm] int hotelID,
                                            [FromForm] string hotelName,
                                            [FromForm] int openedIn,
                                            [FromForm] string description,
                                            [FromForm] string hotelAddress,
                                            [FromForm] string city,
                                            [FromForm] double latitude,
                                            [FromForm] double longitude,
                                            [FromForm] IFormFile mainImage)
        {
            var response = repository.UpdateBasicInformation(hotelID, hotelName, openedIn, description, hotelAddress, city, latitude, longitude, mainImage);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("update-service")]
        public IActionResult UpdateHotelService([FromBody] UpdateServiceDTO update)
        {
            var response = repository.UpdateHotelService(update.HotelID, update.Services);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("add-hotel-image")]
        public IActionResult AddHotelImage([FromForm] int hotelId, [FromForm] String title, [FromForm] IFormFile images)
        {
            var response = repository.AddHotelImage(hotelId, title, images);
            return StatusCode(response.StatusCode, response);
        }
        [HttpDelete("delete-hotel-images")]
        public IActionResult DeleteHotelImages([FromQuery] int imageID)
        {
            var response = repository.DeleteHotelImages(imageID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("get-galleries-by-hotelID")]
        public IActionResult GetGalleriesByHotel([FromQuery] int hotelID)
        {
            var response = repository.GetGalleriesByHotelID(hotelID);
            return StatusCode(response.StatusCode, response);
        }
    }
}
