using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
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
        [HttpDelete("delete-room")]
        public IActionResult DeleteRoom([FromQuery]int roomID)
        {
            var response = reponsitory.DeleteRoom(roomID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("add-room")]
        public IActionResult AddRoom([FromForm] int hotelID, [FromForm] Room room,
                                 [FromForm] DateTime startDate, [FromForm] DateTime endDate, [FromForm] double specialPrice,
                                 [FromForm] List<IFormFile> images, [FromForm] List<string> type, [FromForm] List<string> subServiceNames)
        {
            var response = reponsitory.AddRoom(hotelID, room, startDate, endDate, specialPrice, images, type, subServiceNames);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("update-room")]
        public IActionResult UpdateRoom([FromForm]int roomID, [FromForm] Room room, 
                                        [FromForm] DateTime StartDate, [FromForm] DateTime EndDate, [FromForm] double specialPrice,
                                        [FromForm] List<IFormFile> images, [FromForm] List<string> type, [FromForm]List<string> subServiceNames)
        {
            var response = reponsitory.UpdateRoom(roomID, room, StartDate, EndDate, specialPrice, images, type, subServiceNames);
            return StatusCode(response.StatusCode, response);
        }


    }
}
