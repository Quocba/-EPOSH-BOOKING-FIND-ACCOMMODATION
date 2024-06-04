using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationAPI_EPOSHBOOKING.Model
{
    [Table("HotelImage")]
    public class HotelImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageID { get; set; }
        public String Title { get; set; }
        public byte[]? ImageData { get; set; }
        
        [ForeignKey("HotelID")]
        public Hotel Hotel { get; set; }

    }
}
