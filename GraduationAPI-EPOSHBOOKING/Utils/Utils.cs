using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;
using ClosedXML.Excel;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.Extensions.Configuration;

namespace GraduationAPI_EPOSHBOOKING.Ultils
{
    public class Utils
    {
        public static class AppSettings
        {
            public static string Token { get; } = "my top secret key";
        }

        public static IConfiguration configuration;

        public static string CreateToken(Account accounts, IConfiguration configuration)
        {
            if (accounts == null || string.IsNullOrWhiteSpace(accounts.Email))
            {
                throw new ArgumentException("Account or account email is null or empty.");
            }

            var tokenValue = configuration.GetSection("AppSettings:Token").Value;
            if (string.IsNullOrEmpty(tokenValue))
            {
                throw new InvalidOperationException("Token key is missing or invalid in configuration.");
            }

            List<Claim> claims = new List<Claim>();

            if (accounts.Role == null || string.IsNullOrWhiteSpace(accounts.Role.Name))
            {
                throw new ArgumentException("Account role is null or empty.");
            }

            claims.Add(new Claim(ClaimTypes.Role, accounts.Role.Name.ToLower()));
            claims.Add(new Claim(ClaimTypes.Email, accounts.Email));

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(tokenValue));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        public static Account GetUserInfoFromToken(string token, IConfiguration configuration)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);
                var roleClaim = claimsPrincipal.FindFirst(ClaimTypes.Role);
                var emailClaim = claimsPrincipal.FindFirst(ClaimTypes.Email);

                if (roleClaim == null || emailClaim == null)
                {
                    return null;
                }

                return new Account
                {
                    Role = new Role { Name = roleClaim.Value },
                    Email = emailClaim.Value
                };
            }
            catch (Exception)
            {
                return null;
            }
        }


        public static string HashPassword(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }


        public static string ConvertToBase64(IFormFile image)
        {
            using (var ms = new MemoryStream())
            {
                image.CopyTo(ms);
                var fileBytes = ms.ToArray();
                return Convert.ToBase64String(fileBytes);
            }
        }

        public static string SaveImage(IFormFile image, IWebHostEnvironment environment)
        {
            if (image == null || environment == null)
            {
                throw new ArgumentNullException("Invalid image or environment settings.");
            }

            string uploadsFolder = Path.Combine(environment.ContentRootPath, "images");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            return "/images/" + uniqueFileName;
        }

        public static String sendMail(String toEmail)
        {
            Random random = new Random();
            int otpNumber = random.Next(100000, 999999);
            // Tạo mã OTP ngẫu nhiên
            string otp = otpNumber.ToString();

            // Cấu hình thông tin SMTP
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587; // Thay đổi nếu cần
            string smtpUsername = "eposhhotel@gmail.com";
            string smtpPassword = "yqgorijrzzvpmwqa";

            // Tạo đối tượng SmtpClient
            using (SmtpClient client = new SmtpClient(smtpServer, smtpPort))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = true; // Sử dụng SSL để bảo vệ thông tin đăng nhập

                // Tạo đối tượng MailMessage
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(smtpUsername);
                    mailMessage.To.Add(toEmail);
                    mailMessage.Subject = "EPOSH-BOOKING OTP CODE";
                    mailMessage.Body = $"Your OTP code is: {otp}";

                    // Gửi email
                    client.Send(mailMessage);
                }

            }
            return otp;
        }
        public static byte[] ConvertIFormFileToByteArray(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            return null;
        }

        public static string ConvertIFormFileToBase64(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();
                    return Convert.ToBase64String(fileBytes);
                }
            }
            return null;
        }
        public static string GenerateRandomString()
        {
            int length = 32;
            const string prefix = "EPOSH";
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            char[] stringChars = new char[length - prefix.Length]; // Đặt độ dài chuỗi ngẫu nhiên để giảm đi độ dài của prefix

            for (int i = 0; i < length - prefix.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return prefix + new string(stringChars);
        }

       public static String GenerateStringVoucher(int length = 6)
        {
            Random random = new Random();
            const string randomString = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                int randomIndex = random.Next(randomString.Length);
                stringBuilder.Append(randomString[randomIndex]);
            }
            return stringBuilder.ToString();
        }


        public static String SendMailRegistration([FromHeader] string toEmail,String content)
        {
            // Cấu hình thông tin SMTP
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587; // Thay đổi nếu cần
            string smtpUsername = "eposhhotel@gmail.com";
            string smtpPassword = "yqgorijrzzvpmwqa";

            // Tạo đối tượng SmtpClient
            using (SmtpClient client = new SmtpClient(smtpServer, smtpPort))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = true; // Sử dụng SSL để bảo vệ thông tin đăng nhập

                // Tạo đối tượng MailMessage
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(smtpUsername);
                    mailMessage.To.Add(toEmail);
                    mailMessage.Subject = "[Eposh Notifycation]";
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body =
       mailMessage.Body =
    $@"<!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>EPOSH-HOTEL Invoice</title>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-image: url('https://i.pinimg.com/736x/5d/98/8c/5d988c043c885fd910e0aedadbcfd423.jpg'); /* Đường dẫn đến hình ảnh nền */
                    background-size: cover;
                    background-repeat: no-repeat;
                    background-position: center center;
                    margin: 0;
                    padding: 0;
                }}
                .invoice-container {{
                    max-width: 600px;
                    margin: 20px auto;
                    background-color: rgba(255, 255, 255, 0.9);
                    border-radius: 8px;
                    padding: 20px;
                    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                }}
                .header {{
                    background-image:linear-gradient(to bottom right, #330867, #30CFD0);
                    color: #00A5F5;
                    padding: 10px;
                    border-radius: 8px 8px 0 0;
                    text-align: center;
                    
                }}
                .content {{
                    padding: 20px;
                    background-color: #C3CFE2
                    
                }}
                .content p{{
                    font-size:19px;
                    color:black;
                 }}
                .footer {{
                    background-image:linear-gradient(to bottom right, #330867, #30CFD0);
                    color: #fff;
                    padding: 10px;
                    border-radius: 0 0 8px 8px;
                    text-align: center;
                    font-szie: 19px;
                    font-weight:bold;
                }}
                .property {{
                    font-weight: bold;
                }}
                ul {{
                    list-style: none;
                    padding: 0;
                    font-size:18px;
                }}
                li {{
                    margin-bottom: 10px;
                    color:black;
                }}
                .property-name {{
                    font-weight: bold;
                    color: #007bff;
                }}  
                .logo {{
                    max-width: 120px; /* Kích thước tối đa của logo */
                }}
            </style>
        </head>
        <body>
            <div class='invoice-container'>
                <div class='header'>
                    <img class='logo' src='https://i.imgur.com/FrtuWsK.png' alt='EPOSH-HOTEL Logo' >
                </div>
                <div class='content'>
                    <ul>
                         <li><span class='property-name'>Notifycation:</span> {content}</li>
                    </ul>
                </div>
                <div class='footer'>
                    <p>Best Regards,</p>
                    <p>EPOSH-HOTEL Team</p>
                </div>
            </div>
        </body>
        </html>";

                    // Gửi email
                    client.Send(mailMessage);
                }
            }

            return "Ok";
        }

        public static String SendMailBooking([FromHeader] string toEmail,Booking booking)
        {
            // Cấu hình thông tin SMTP
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587; // Thay đổi nếu cần
            string smtpUsername = "eposhhotel@gmail.com";
            string smtpPassword = "yqgorijrzzvpmwqa";
            // URL cơ sở của máy chủ
            string baseUrl = "https://localhost:7162";

            // Đường dẫn tương đối của hình ảnh từ cơ sở dữ liệu
            string relativeImageUrl = booking.Room.Hotel.MainImage;

            // Tạo đường dẫn đầy đủ của hình ảnh
            string imageUrl = $"{baseUrl}{relativeImageUrl}";
            // Tạo đối tượng SmtpClient
            using (SmtpClient client = new SmtpClient(smtpServer, smtpPort))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = true; // Sử dụng SSL để bảo vệ thông tin đăng nhập

                // Tạo đối tượng MailMessage
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(smtpUsername);
                    mailMessage.To.Add(toEmail);
                    mailMessage.Subject = "[Eposh Notifycation]";
                    mailMessage.IsBodyHtml = true;
                        mailMessage.Body =
       mailMessage.Body =
    $@"<!DOCTYPE html>
   <html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Booking Form</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f9f9f9;
            margin: 0;
            padding: 0;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
        }}

        .container {{
            background-color: #FFFAF0;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
            width: 600px;
        }}

        .hotel-info {{
            text-align: center;
            margin-bottom: 20px;
        }}

        .hotel-image {{
            width: 100%;
            height: auto;
            border-radius: 8px;
        }}

        .room-type {{
            font-size: 18px;
            font-weight: bold;
        }}

        .details, .cancellation {{
            font-size: 14px;
            color: #666;
        }}

        .cancellation span {{
            color: #d9534f;
        }}

        .booking-form {{
            margin-bottom: 20px;
        }}

        .form-group {{
            margin-bottom: 15px;
        }}

        .form-group label {{
            display: block;
            margin-bottom: 5px;
        }}

        .form-group input, .form-group select, .form-group button {{
            width: 100%;
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 4px;
        }}

        .use-button {{
            width: auto;
            display: inline-block;
            margin-left: 10px;
        }}

        .price-detail {{
            text-align: right;
            font-size: 14px;
            color: #666;
        }}

        .price-detail span {{
            font-weight: bold;
            color: #333;
        }}

        .booking-button {{
            background-color: #5cb85c;
            color: #fff;
            border: none;
            cursor: pointer;
        }}

        .booking-button:hover {{
            background-color: #4cae4c;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""hotel-info"">
            <img src=""{imageUrl}"" alt=""Hotel Image"" class=""hotel-image"">
            <h2>{booking.Room.Hotel.Name} - {booking.Room.Hotel.HotelAddress.Address}</h2>
            <p class=""room-type"">{booking.Room.TypeOfRoom}</p>
            <p class=""details"">{booking.NumberGuest} people &bull; {booking.Room.TypeOfBed} &bull; {booking.Room.SizeOfRoom} m²</p>
            <p class=""cancellation"">Reservations can be <span>canceled</span> before 24 hours from check-in date</p>
        </div>
        <form action=""sendmail.php"" method=""post"" class=""booking-form"">
            <div class=""form-group"">
                <label for=""checkin"">Check-in</label>
                <input type=""date"" id=""checkin"" name=""checkin"" value =""{booking.CheckInDate:yyyy-MM-dd}"" required>
            </div>
            <div class=""form-group"">
                <label for=""checkout"">Check-out</label>
                <input type=""date"" id=""checkout"" name=""checkout"" value =""{booking.CheckOutDate:yyyy-MM-dd}""required>
            </div>
            <div class=""form-group"">
                <label for=""fullname"">Fullname</label>
                <input type=""text"" id=""fullname"" name=""fullname"" value= ""{booking.Account.Profile.fullName}"" required>
            </div>
            <div class=""form-group"">
                <label for=""email"">Email</label>
                <input type=""email"" id=""email"" name=""email"" value = ""{booking.Account.Email}"" required>
            </div>
            <div class=""form-group"">
                <label for=""phone"">Phone</label>  
                <input type=""tel"" id=""phone"" name=""phone"" value = ""{booking.Account.Phone}"" required>
            </div>

        </form>
        <div class=""price-detail"">
            <p>Room: <span>VND {booking.Room.Price}</span></p>
            <p>Taxes: <span>VND {booking.TaxesPrice}</span></p>
            <p>Total: <span>VND {booking.TotalPrice}</span></p>
        </div>
    </div>
</body>
        </html>";

                    // Gửi email
                    client.Send(mailMessage);
                }
            }

            return "Ok";
        }
    }

}