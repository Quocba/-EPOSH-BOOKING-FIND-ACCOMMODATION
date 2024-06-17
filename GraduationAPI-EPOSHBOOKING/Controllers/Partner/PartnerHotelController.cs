﻿using DocumentFormat.OpenXml.Bibliography;
using GraduationAPI_EPOSHBOOKING.DTO;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Partner
{
    [ApiController]
    [Route("api/v1/partner/hotel")]
    public class PartnerHotelController : Controller
    {
        private readonly IHotelRepository repository;
        private readonly IConfiguration configuration;
        public PartnerHotelController(IHotelRepository repository, IConfiguration configuration)
        {
            this.repository = repository;
            this.configuration = configuration;
        }

        [HttpPost("hotel-registration")]
        public IActionResult RegisterHotel([FromForm] HotelRegistrationDTO registrationModel)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var user = Ultils.Utils.GetUserInfoFromToken(token, configuration);
            try
            {
                switch(user.Role.Name.ToLower())
                {
                    case "partner":
                        var services = JsonConvert.DeserializeObject<List<ServiceTypeDTO>>(registrationModel.Services);
                        var response = repository.HotelRegistration(registrationModel, services);
                        return StatusCode(response.StatusCode, response);
                    default:
                        return Unauthorized();
                }
            }catch (Exception ex)
            {
                return Unauthorized();
            }

        }
        [HttpPut("update-basic-information")]
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


            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var user = Ultils.Utils.GetUserInfoFromToken(token, configuration);
            try
            {
                switch (user.Role.Name.ToLower())
                {
                    case "partner":
                        var response = repository.UpdateBasicInformation(hotelID, hotelName, openedIn, description, hotelAddress, city, latitude, longitude, mainImage);
                        return StatusCode(response.StatusCode, response);
                    default:
                        return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
     
        }

        [HttpPut("update-service")]
        public IActionResult UpdateHotelService([FromBody] UpdateServiceDTO update)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var user = Ultils.Utils.GetUserInfoFromToken(token, configuration);
            try
            {
                switch (user.Role.Name.ToLower())
                {
                    case "partner":
                        var response = repository.UpdateHotelService(update.HotelID, update.Services);
                        return StatusCode(response.StatusCode, response);
                    default:
                        return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
   
        }


        [HttpPost("add-hotel-image")]
        public IActionResult AddHotelImage([FromForm] int hotelId, [FromForm] String title, [FromForm] IFormFile images)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var user = Ultils.Utils.GetUserInfoFromToken(token, configuration);
            try
            {
                switch (user.Role.Name.ToLower())
                {
                    case "partner":
                        var response = repository.AddHotelImage(hotelId, title, images);
                        return StatusCode(response.StatusCode, response);
                    default:
                        return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }

        }
        [HttpDelete("delete-hotel-images")]
        public IActionResult DeleteHotelImages([FromQuery] int imageID)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var user = Ultils.Utils.GetUserInfoFromToken(token, configuration);
            try
            {
                switch (user.Role.Name.ToLower())
                {
                    case "partner":
                        var response = repository.DeleteHotelImages(imageID);
                        return StatusCode(response.StatusCode, response);
                    default:
                        return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
   
        }

        [HttpGet("get-galleries-by-hotelID")]
        public IActionResult GetGalleriesByHotel([FromQuery] int hotelID)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var user = Ultils.Utils.GetUserInfoFromToken(token, configuration);
            try
            {
                switch (user.Role.Name.ToLower())
                {
                    case "partner":
                        var response = repository.GetGalleriesByHotelID(hotelID);
                        return StatusCode(response.StatusCode, response);
                    default:
                        return Unauthorized();
                }
            }catch (Exception ex)
            {
                return Unauthorized() ;
            }

        }
    }
}
