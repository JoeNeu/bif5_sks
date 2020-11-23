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
    public class Parcel
    {
        public Parcel()
        {
            VisitedHops = new List<HopArrival>();
            FutureHops = new List<HopArrival>();
        }
        [Required]
        [Key]
        public int ID { get; set; }
        [Required]
        public float? Weight { get; set; }
        [Required]
        public Receipient Receipient { get; set; }
        public Receipient Sender { get; set; }
        [Required]
        public string TrackingId { get; set; }
        public string OldTrackingId { get; set; }
        public enum StateEnum
        {
            InTransportEnum = 0,
            InTruckDeliveryEnum = 1,
            DeliveredEnum = 2
        }
        [Required]
        public StateEnum? State { get; set; }
        public List<HopArrival> VisitedHops { get; set; }
        public List<HopArrival> FutureHops { get; set; }
    }
}
