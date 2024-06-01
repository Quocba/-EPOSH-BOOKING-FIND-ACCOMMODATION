using GraduationAPI_EPOSHBOOKING.Model;

namespace GraduationAPI_EPOSHBOOKING.IRepository
{
    public interface IRoomRepository
    {
        ResponseMessage GetRoomDetail(int roomID);
        ResponseMessage GetAllRoom();
        ResponseMessage DeleteRoom(int roomID);
        ResponseMessage AddRoom(int hotelID,Room room,List<SpecialPrice>specialPrices, List<IFormFile>images,List<ServiceType>services);
        ResponseMessage UpdateRoom(int roomID,Room room,List<SpecialPrice> specialPrices, List<IFormFile>image,List<ServiceType>services);
    }
}
