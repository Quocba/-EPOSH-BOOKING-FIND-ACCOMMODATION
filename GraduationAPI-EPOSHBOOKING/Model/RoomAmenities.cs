using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationAPI_EPOSHBOOKING.Model
{
    [Table("RoomAmenities")]
    public class RoomAmenities
    {
        [Key]
        public int RoomId { get; set; }
        public Room Room { get; set; }

        [Key]
        public int RoomServiceID { get; set; }
        public RoomService RoomService { get; set; }
    }
}
