using GraduationAPI_EPOSHBOOKING.Model;

namespace GraduationAPI_EPOSHBOOKING.IRepository
{
    public interface IRoomRepository
    {
        ResponseMessage GetRoomDetail(int roomID);
        ResponseMessage GetAllRoom();
        ResponseMessage DeleteRoom(int roomID);
        ResponseMessage AddRoom(int hotelID,Room room,DateTime StartDate,
            DateTime EndDate,double specialPrice
            , List<IFormFile>images,List<String>type, List<String>subServiceName);

        ResponseMessage UpdateRoom(int roomID,Room room,DateTime StartDate,
            DateTime EndDate,Double SpecialPrice, List<IFormFile>image,List<string>type,List<String>subServiceName);
    }
}
