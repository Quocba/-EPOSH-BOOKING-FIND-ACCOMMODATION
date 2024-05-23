using GraduationAPI_EPOSHBOOKING.Model;

namespace GraduationAPI_EPOSHBOOKING.IRepository
{
    public interface IRoomRepository
    {
        ResponseMessage GetRoomDetail(int roomID);
        
       
    }
}
