using GraduationAPI_EPOSHBOOKING.DataAccess;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using GraduationAPI_EPOSHBOOKING.Ultils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Security.Principal;
using System.Text.RegularExpressions;
#pragma warning disable // tắt cảnh báo để code sạch hơn

namespace GraduationAPI_EPOSHBOOKING.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DBContext db;
        public AccountRepository(DBContext _db)
        {
            this.db = _db;
        }

        public ResponseMessage RegisterPartnerAccount(Account account, String fullName)
        {
            String role = "Partner";
            var addRole = db.roles.FirstOrDefault(x => x.Name.Equals(role));
            var checkEmailAlready = db.accounts.FirstOrDefault(email => email.Email.Equals(account.Email));
            if (checkEmailAlready != null)
            {
                return new ResponseMessage { Success = false, Data = checkEmailAlready.Email, Message = "Email Already Exist", StatusCode = (int)HttpStatusCode.AlreadyReported };
            }
            else
            {
                if (account != null && !fullName.IsNullOrEmpty())
                {
                    Profile addProfile = new Profile
                    {
                        fullName = fullName,
                    };
                    db.profiles.Add(addProfile);
                    Account addAccount = new Account
                    {
                        Email = account.Email,
                        Password = Ultils.Utils.HashPassword(account.Password),
                        Phone = account.Phone,
                        Role = addRole,
                        Profile = addProfile,
                        IsActive = false
                    };
                    db.accounts.Add(addAccount);
                    db.SaveChanges();
                    Ultils.Utils.sendMail(account.Email);
                    return new ResponseMessage { Success = true, Data = addAccount, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
                }
                return new ResponseMessage { Success = false, Data = account, Message = "Register Fail", StatusCode = (int)HttpStatusCode.BadRequest };
            }

        }

        public ResponseMessage ActiveAccount(String email)
        {
            String emailParten = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
            Regex regex = new Regex(emailParten);
            if (regex.IsMatch(email))
            {
                var checkEmail = db.accounts.FirstOrDefault(x => x.Email.Equals(email));
                if (checkEmail != null)
                {
                    checkEmail.IsActive = true;
                    db.accounts.Update(checkEmail);
                    db.SaveChanges();
                    return new ResponseMessage { Success = true, Data = checkEmail, Message = "Your account has been activated", StatusCode = (int)HttpStatusCode.OK };
                }
                else
                {
                    return new ResponseMessage { Success = false, Data = checkEmail, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound };
                }
            }
            return new ResponseMessage { Success = false, Data = email, Message = "Email is not in correct format. Please re-enter for example: Eposh@eposh.com" };
        }

        public ResponseMessage LoginWithNumberPhone(String phone)
        {
            String phoneRegex = @"^(?:\+84|0)([3|5|7|8|9])+([0-9]{8})$";
            Regex regex = new Regex(phoneRegex);
            if (regex.IsMatch(phone))
            {
                var checkPhone = db.accounts.FirstOrDefault(x => x.Phone.Equals(phone));
                if (checkPhone != null)
                {
                    return new ResponseMessage { Success = true, Data = phone, Message = "Successfully", StatusCode = (int)HttpStatusCode.AlreadyReported };
                }
                else
                {
                    Profile addProfile = new Profile
                    {
                        fullName = Ultils.Utils.GenerateRandomString()
                    };
                    db.profiles.Add(addProfile);
                    Account addAccount = new Account
                    {
                        Phone = phone,
                        Profile = addProfile
                    };
                    db.accounts.Add(addAccount);
                    db.SaveChanges();
                    return new ResponseMessage { Success = true, Data = addAccount, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
                }
            }
            return new ResponseMessage { Success = false, Data = phone, Message = "Phone is not in correct format. Please re-enter for example: 0123456789", StatusCode = (int)HttpStatusCode.BadRequest };
        }
        public ResponseMessage Register(string email, string password, string fullName, string phone)
        {
            if (db.accounts.Any(a => a.Email == email))
            {
                return new ResponseMessage
                {
                    Success = false,
                    Message = "Email is already registered",
                    StatusCode = 400
                };
            }

            string hashedPassword = Utils.HashPassword(password);
            var account = new Account
            {
                Email = email,
                Password = hashedPassword,
                IsActive = true,
                Phone = phone,
                Role = db.roles.FirstOrDefault(r => r.Name == "Customer")
            };
            var profile = new Profile
            {
                fullName = fullName
            };

            account.Profile = profile;
            db.accounts.Add(account);
            db.SaveChanges();

            return new ResponseMessage
            {
                Success = true,
                Message = "Registration Successfully",
                StatusCode = 201,
                Data = new { account.AccountID, account.Email, profile.fullName }
            };
        }

        public ResponseMessage UpdateNewPassword(string email, string newPassword)
        {
            var getAccount = db.accounts.FirstOrDefault(account => account.Email.Equals(email));
            if(getAccount != null)
            {
                getAccount.Password = Ultils.Utils.HashPassword(newPassword);
                db.accounts.Update(getAccount);
                db.SaveChanges();
                return new ResponseMessage { Success = true, Data = getAccount, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
                return new ResponseMessage { Success = false, Data = email, Message = "Email Does not exitst", StatusCode = (int)HttpStatusCode.NotFound };
           
        }

        public ResponseMessage UpdateProfileByAccount(int accountID, Profile profile,IFormFile avatar)
        {
            try
            {
                var getAccount = db.accounts.Include(profile => profile.Profile).FirstOrDefault(account => account.AccountID == accountID);
                if (getAccount != null)
                {
                    getAccount.Profile.fullName = profile.fullName;
                    getAccount.Profile.BirthDay = profile.BirthDay;
                    getAccount.Profile.Gender = profile.Gender;
                    getAccount.Profile.Address = profile.Address;
                    getAccount.Profile.Avatar = Ultils.Utils.ConvertIFormFileToByteArray(avatar);
                    db.accounts.Update(getAccount);
                    db.SaveChanges();
                    return new ResponseMessage { Success = true, Data = getAccount, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
                }
                    return new ResponseMessage { Success = false,Data = getAccount, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound };
            }
            catch (Exception ex)
            {
                return new ResponseMessage { Success = false, Data = ex, Message = "Internal Server Error", StatusCode = (int)HttpStatusCode.InternalServerError };

            }

        }


    }
}
