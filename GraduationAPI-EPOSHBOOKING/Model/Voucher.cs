using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationAPI_EPOSHBOOKING.Model
{
    [Table("Voucher")]
    public class Voucher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VoucherID { get; set; }
        public byte[] VoucherImage { get; set; }
        [MaxLength(255)]
        public string VoucherName { get; set;}
        [MaxLength(6)]
        public String Code { get;set;}
        public int Quantity { get; set;}
        public double Discount { get; set;}
        [MaxLength(255)]
        public string Description { get; set;}
    }
}
