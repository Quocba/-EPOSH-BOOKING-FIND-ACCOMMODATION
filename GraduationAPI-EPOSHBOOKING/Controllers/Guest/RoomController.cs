using GraduationAPI_EPOSHBOOKING.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Guest
{
    [ApiController]
    [Route("api/v1/room")]
    public class RoomController : Controller
    {
        private readonly IRoomRepository reponsitory;
        public RoomController(IRoomRepository roomRepository) { 
        
            this.reponsitory = roomRepository;
        }

        [HttpGet("get-room-by-id")]
        public IActionResult GetRoomDetail([FromQuery]int roomID)
        {
            var response = reponsitory.GetRoomDetail(roomID);
            return StatusCode(response.StatusCode, response);   
        }

        [HttpGet("get-all-room")]
        public IActionResult GetAllRoom()
        {
            var resposne = reponsitory.GetAllRoom();
            return StatusCode(resposne.StatusCode, resposne);
        }
    }
}
