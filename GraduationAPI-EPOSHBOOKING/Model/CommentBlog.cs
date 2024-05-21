using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationAPI_EPOSHBOOKING.Model
{
    [Table("CommentBlog")]
    public class CommentBlog
    {
        [Key]
        public int AccountID { get; set; }  
        public Account Account { get; set; }
        [Key]
        public int BlogID { get; set; }
        public Blog Blog { get; set; }
        public String Desciption { get; set; }
        public DateTime DateComment { get; set; }

       
    }
}
