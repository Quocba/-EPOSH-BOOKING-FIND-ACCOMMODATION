using GraduationAPI_EPOSHBOOKING.DTO;
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
        private readonly IConfiguration configuration;
        public CustomerBookingController(IBookingRepository repository, IConfiguration configuration)
        {
            this.repository = repository;
            this.configuration = configuration;
        }

        [HttpGet("get-by-accountID")]
        public IActionResult GetBookingByAccount([FromQuery] int accountID)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ","");
            var user = Ultils.Utils.GetUserInfoFromToken(token, configuration);
            try
            {
                switch(user.Role.Name.ToLower())
                {
                    case "customer":
                        var response = repository.GetBookingByAccount(accountID);
                        return StatusCode(response.StatusCode, response);
                    default:
                        return Unauthorized();
                }
            }catch (Exception ex)
            {
                return Unauthorized();
            }
         
        }

        [HttpPut("cancle-booking")]
        public IActionResult CancleBooking([FromForm] int bookingID, [FromForm] string Reason)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var user = Ultils.Utils.GetUserInfoFromToken (token, configuration);
            try
            {
                switch (user.Role.Name.ToLower())
                {
                    case "customer":
                        var response = repository.CancleBooking(bookingID, Reason);
                        return StatusCode(response.StatusCode, response);
                    default:
                        return Unauthorized();
                }
            }catch (Exception ex)
            {
                return Unauthorized();
            }

        }

        [HttpPost("create-booking")]
        public IActionResult CreateBooking([FromForm] CreateBookingDTO createBookingDTO)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var user = Ultils.Utils.GetUserInfoFromToken(token, configuration);
            try
            {
                switch (user.Role.Name.ToLower())
                {
                    case "customer":
                        var response = repository.CreateBooking(createBookingDTO);
                        return StatusCode(response.StatusCode, response);
                    default:
                        return Unauthorized();
                }
            }catch (Exception ex)
            {
                return Unauthorized();
            }

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
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var user = Ultils.Utils.GetUserInfoFromToken(token, configuration);
            try
            {
                switch (user.Role.Name.ToLower())
                {
                    case "customer":
                        var response = repository.ExportBookingsByAccountID(accountID);

                        if (response.Success)
                        {
                            return File((byte[])response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Bookings_{accountID}.xlsx");
                        }
                        else
                        {
                            return StatusCode(response.StatusCode, response);
                        }
                    default:
                        return Unauthorized();
                }
            }catch (Exception ex)
            {
                return Unauthorized();
            }
        }
    }
}
