
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

﻿using GraduationAPI_EPOSHBOOKING.IRepository;
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
            public class HotelRegistrationModel
            {
                public string HotelName { get; set; }
                public int OpenedIn { get; set; }
                public string Description { get; set; }
                public int HotelStandard { get; set; }
                public string HotelAddress { get; set; }
                public string City { get; set; }
                public double Latitude { get; set; }
                public double Longitude { get; set; }
                public List<IFormFile> Images { get; set; }
                public IFormFile MainImage { get; set; }
                public int AccountID { get; set; }
                public List<ServiceType> Services { get; set; }
            }
            [HttpPost("hotel-registration")]
            public IActionResult RegisterHotel([FromForm] HotelRegistrationModel registrationModel)
            {
                var response = repository.HotelRegistration(
                    registrationModel.HotelName,
                    registrationModel.OpenedIn,
                    registrationModel.Description,
                    registrationModel.HotelStandard,
                    registrationModel.HotelAddress,
                    registrationModel.City,
                    registrationModel.Latitude,
                    registrationModel.Longitude,
                    registrationModel.Images,
                    registrationModel.MainImage,
                    registrationModel.AccountID,
                    registrationModel.Services
                );

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
            var response = repository.UpdateBasicInfomation(hotelID, hotelName, openedIn, description, hotelAddress, city, latitude, longitude,mainImage);
            return StatusCode(response.StatusCode, response);
        }
       
        [HttpPut("update-service")]
        public IActionResult UpdateHotelService([FromBody] UpdateService update)
        {
            var response = repository.UpdateHotelService(update.HotelID, update.Services);
            return StatusCode(response.StatusCode, response);
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
 