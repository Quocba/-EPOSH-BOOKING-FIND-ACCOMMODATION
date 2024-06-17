﻿using GraduationAPI_EPOSHBOOKING.DTO;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Customer
{
    [ApiController]
    [Route("api/v1/customer/booking")]
    public class CustomerBookingController : Controller
    {
        private readonly IBookingRepository repository;
        public CustomerBookingController(IBookingRepository repository)
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

        [HttpPost("create-booking-fe")]
        public IActionResult CreateBooking([FromForm] CreateBookingDTO createBookingDTO)
        {
            var response = repository.CreateBooking(createBookingDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("check-room-price")]
        public IActionResult CheckRoomPrice([FromForm] int roomID, [FromForm] DateTime CheckInDate, [FromForm] DateTime CheckOutDate)
        {
            double roomPrice = repository.CheckRoomPrice(roomID, CheckInDate, CheckOutDate);
            return Ok(new { Price = roomPrice });
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
    }
}
