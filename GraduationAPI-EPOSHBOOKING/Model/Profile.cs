using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace GraduationAPI_EPOSHBOOKING.Model
{
    [Table("Profile")]
    public class Profile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProfileID { get; set; }
        [MaxLength(255)]
        public String fullName { get; set; }
        [AllowNull]
        public DateTime BirthDay { get; set; }
        [AllowNull]
        [MaxLength(10)]
        public String Gender { get; set; }
        [AllowNull]
        [MaxLength(255)]
        public String Address { get; set;}

        [AllowNull]
        public byte[] Avatar { get; set; }

    }
}
