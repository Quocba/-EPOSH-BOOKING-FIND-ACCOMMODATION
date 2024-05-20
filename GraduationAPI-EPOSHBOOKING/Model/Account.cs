using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace GraduationAPI_EPOSHBOOKING.Model
{

    [Table("Account")]
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountID { get; set; }
        [MaxLength(255)]
        [AllowNull]
        
        public String Email { get; set; }
        [MaxLength (16)]
        [AllowNull]
        public String Password { get; set; }
        [MaxLength(255)]
        [AllowNull]
        public String Phone {  get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("RoleID")]
        public Role Role { get; set; }

        [ForeignKey("ProfileID")]
        public Profile Profile { get; set; }

        

    }
}
