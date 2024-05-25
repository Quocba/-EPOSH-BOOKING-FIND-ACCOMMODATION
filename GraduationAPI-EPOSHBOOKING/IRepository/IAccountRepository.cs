using GraduationAPI_EPOSHBOOKING.Model;

namespace GraduationAPI_EPOSHBOOKING.IRepository
{
    public interface IAccountRepository
    {
        ResponseMessage RegisterPartnerAccount(Account account,String fullName);
        ResponseMessage ActiveAccount(String email);
        ResponseMessage LoginWithNumberPhone(String phone);
        ResponseMessage Register(String email, String password, String fullName, String phone);
        ResponseMessage UpdateNewPassword(String email,String newPassword);
        ResponseMessage UpdateProfileByAccount(int accountID,Profile profile, IFormFile avatar);
    }
}
        
 

