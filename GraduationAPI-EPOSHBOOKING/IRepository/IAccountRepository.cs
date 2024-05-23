using GraduationAPI_EPOSHBOOKING.Model;

namespace GraduationAPI_EPOSHBOOKING.IRepository
{
    public interface IAccountRepository
    {
        ResponseMessage RegisterPartnerAccount(Account account,String fullName);
        ResponseMessage ActiveAccount(String email);
        ResponseMessage LoginWithNumberPhone(String phone);
    }
}
