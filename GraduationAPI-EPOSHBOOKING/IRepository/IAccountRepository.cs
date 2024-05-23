using GraduationAPI_EPOSHBOOKING.Model;

namespace GraduationAPI_EPOSHBOOKING.IRepository
{
    public interface IAccountRepository
    {
        public ResponseMessage Register(String email, String password, String fullName, String phone);
    }
}
 