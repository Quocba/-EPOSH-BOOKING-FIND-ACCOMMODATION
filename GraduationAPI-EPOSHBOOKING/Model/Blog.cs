using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationAPI_EPOSHBOOKING.Model
{
    [Table("Blogs")]
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BlogID { get; set; }
        [MaxLength(255)]
        public String Title { get; set; }
        [MaxLength(255)]
        public String Description { get; set; }
        [MaxLength (255)]
        public String Location { get; set; }

        [ForeignKey("AccountID")]
        public Account Account { get; set; }


    }
}
