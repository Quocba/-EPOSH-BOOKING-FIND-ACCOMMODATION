using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationAPI_EPOSHBOOKING.Model
{
    [Table("RoomService")]
    public class RoomService
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomServiceID { get; set; }
        [MaxLength(255)]
        public String Type { get; set; }
    }
}   
