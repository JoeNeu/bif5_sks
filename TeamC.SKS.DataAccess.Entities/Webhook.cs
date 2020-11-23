using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace TeamC.SKS.DataAccess.Entities
{ 
    public class Webhook
    {
        [Required]
        [Key]
        public long Id { get; set; }
        public string TrackingId { get; set; }
        public string Url { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
