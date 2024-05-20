﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationAPI_EPOSHBOOKING.Model
{
    [Table("HotelAddress")]
    public class HotelAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressID { get; set; }
        [MaxLength(255)]
        public String Address { get; set; }
        [MaxLength (255)]
        public String City { get; set; }
        public double lat { get; set; }
        public double lon { get;set; }
    }
}
