using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationAPI_EPOSHBOOKING.Model
{
    [Table("BookingDetails")]
    public class BookingDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailsID { get; set; }

        public double UnitPrice { get; set; }
        public double TaxesPrice { get; set; }
        public int NumberOfRoom { get; set; }

    }
}
