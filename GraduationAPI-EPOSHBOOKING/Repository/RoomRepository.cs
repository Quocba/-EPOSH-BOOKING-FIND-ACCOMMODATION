using GraduationAPI_EPOSHBOOKING.DataAccess;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;
#pragma warning disable // tắt cảnh báo để code sạch hơn

namespace GraduationAPI_EPOSHBOOKING.Repository
{
    public class RoomRepository : IRoomRepository
    {
        private readonly DBContext db;
        public RoomRepository(DBContext _db)
        {
            this.db = _db;
        }

        public ResponseMessage GetRoomDetail(int roomID)
        {
            var getRoom = db.room.Include(x => x.RoomImages).Include(x => x.SpecialPrice).Include(x => x.RoomService).ThenInclude(x => x.RoomSubServices)
            .FirstOrDefault(room => room.RoomID == roomID);
            if (getRoom != null)
            {
                return new ResponseMessage { Success = true, Data = getRoom, Message = "Successfully",StatusCode = (int)HttpStatusCode.OK };
            }
                return new ResponseMessage { Success = false,Data = getRoom, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound };
        }


       
    }
}
