using GraduationAPI_EPOSHBOOKING.DataAccess;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using GraduationAPI_EPOSHBOOKING.Repository;
using GraduationAPI_EPOSHBOOKING.Ultils;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

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
builder.Services.AddScoped<Utils>();
builder.Services.AddDbContext<DBContext>();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
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
    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {

        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        });
    }
    app.UseHttpsRedirection();
    // Enable CORS
    app.UseCors("AllowAllOrigins");

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
