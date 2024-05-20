using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GraduationAPI_EPOSHBOOKING.Model
{
    [Table("HotelAmenities")]
    public class HotelAmenities
    {
        [Key]
        public int HotelID { get; set; }
        public Hotel hotel { get; set; }

        [Key]
        public int ServiceID { get; set; }
        public HotelService hotelService { get; set; }

    }
}
