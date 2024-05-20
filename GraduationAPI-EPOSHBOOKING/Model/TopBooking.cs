using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationAPI_EPOSHBOOKING.Model
{
    [Table("TopBooking")]
    public class TopBooking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TopID { get; set; }
        [MaxLength(32)]
        public String FullName { get; set; }
        public byte[] Avatar { get; set; }
        public int TotalBooking { get; set; }
        [ForeignKey("BookingID")]
        public Booking Booking { get; set; }
    }
}
