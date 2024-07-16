
using GraduationAPI_EPOSHBOOKING.DataAccess;
using Microsoft.Extensions.FileProviders;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using GraduationAPI_EPOSHBOOKING.DataAccess;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using GraduationAPI_EPOSHBOOKING.Repository;
using GraduationAPI_EPOSHBOOKING.Ultils;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using static GraduationAPI_EPOSHBOOKING.Ultils.Utils;
using Microsoft.AspNetCore.Http.Features;
using GraduationAPI_EPOSHBOOKING.BackgroundService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IFeedbackRepository,FeedBackRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IReportFeedbackRepository, ReportFeedbackRepository>();
builder.Services.AddScoped<Utils>();
builder.Services.AddDbContext<DBContext>();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1024 * 1024 * 50; // 50 MB
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = long.MaxValue; // No limit
});
builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAllOrigins",
            builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
    });
builder.Services.AddLogging();
builder.Services.AddHostedService<BookingStatusServcie>();
//builder.Services.AddHostedService<ClearDataBookingService>();
var app = builder.Build();
app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        });

app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "images")),
        RequestPath = "/images"
    });

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
