using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationAPI_EPOSHBOOKING.Model
{
    [Table("Room")]
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomID { get; set; }
        [MaxLength(255)]
        public String TypeOfRoom { get;set; }
        public int NumberCapacity { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int SizeOfRoom { get; set; }
        [MaxLength (255)]
        public String TypeOfBed { get; set; }

        [ForeignKey("HotelID")]
        public Hotel Hotel { get; set; }

        public ICollection<RoomImage> RoomImages { get; set; }
        public ICollection<RoomAmenities> RoomAmenities { get; set; }
        public ICollection<RoomService> RoomService { get; set; }
    }
}
