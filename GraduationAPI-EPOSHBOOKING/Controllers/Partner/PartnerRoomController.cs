using GraduationAPI_EPOSHBOOKING.DTO;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;
#pragma warning disable // tắt cảnh báo để code sạch hơn


namespace GraduationAPI_EPOSHBOOKING.Controllers.Partner
{
    public class PartnerRoomController : Controller
    {
        private readonly IRoomRepository reponsitory;
        public PartnerRoomController(IRoomRepository roomRepository)
        {
            this.reponsitory = roomRepository;
        }



        [HttpDelete("delete-room")]
        public IActionResult DeleteRoom([FromQuery] int roomID)
        {
            var response = reponsitory.DeleteRoom(roomID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("add-room")]
        public IActionResult AddRoom([FromForm] AddRoomDTO addRoomModel)
        {
            var services = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ServiceTypeDTO>>(addRoomModel.Services);
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
        public IActionResult UpdateRoom([FromForm] UpdateRoomDTO updateRoomModel)
        {
            var services = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ServiceTypeDTO>>(updateRoomModel.Services);
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
