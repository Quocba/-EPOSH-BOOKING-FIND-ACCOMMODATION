using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;
#pragma warning disable // tắt cảnh báo để code sạch hơn

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
        public class AddRoomModel
        {
            public int HotelID { get; set; }
            public Room Room { get; set; }
            public string SpecialPrices { get; set; }
            public List<IFormFile> Images { get; set; }
            public string Services { get; set; }
        };
        public class UpdateRoomModel
        {
            public int RoomID { get; set; }
            public Room Room { get; set; }
            public String specialPrice { get; set; }
            public List<IFormFile> Images { get; set; }
            public string Services { get; set; }
        };
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
        public IActionResult AddRoom([FromForm] AddRoomModel addRoomModel)
        {
            var services = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ServiceType>>(addRoomModel.Services);
            var specialPrices = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SpecialPrice>>(addRoomModel.SpecialPrices);
            var response = reponsitory.AddRoom(
                addRoomModel.HotelID,
                addRoomModel.Room,
                specialPrices,
                addRoomModel.Images,
                services
            );

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("update-room")]
        public IActionResult UpdateRoom([FromForm] UpdateRoomModel updateRoomModel)
        {
            var services = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ServiceType>>(updateRoomModel.Services);
            var specialPrice = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SpecialPrice>>(updateRoomModel.specialPrice);
            var response = reponsitory.UpdateRoom(
                updateRoomModel.RoomID,
                updateRoomModel.Room,
                specialPrice,
                updateRoomModel.Images,
                services
            );
            return StatusCode(response.StatusCode, response);
        }


    }
}
