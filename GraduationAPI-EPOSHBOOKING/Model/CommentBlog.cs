using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationAPI_EPOSHBOOKING.Model
{
    [Table("CommentBlog")]
    public class CommentBlog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentID { get; set; }
        [MaxLength(255)]
        public String Desciption { get; set; }

        [ForeignKey("BlogID")]
        public Blog Blog { get; set; }
    }
}
