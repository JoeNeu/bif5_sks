using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace TeamC.SKS.DataAccess.Entities
{ 
    public class HopArrival
    {
        [Required]
        [Key]
        public int ID { get; set; }
        [Required]
        public string Code { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}