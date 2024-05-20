using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationAPI_EPOSHBOOKING.Model
{
    [Table("ReportFeedBack")]
    public class ReportFeedBack
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReportID { get; set; }

        [MaxLength(255)]
        public String ReporterEmail { get; set; }

        [MaxLength(255)]
        public String ReasonReport { get;set; }

        [ForeignKey("FeedBackID")]
        public FeedBack FeedBack { get; set; }
    }
}
