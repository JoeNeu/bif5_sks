using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace TeamC.SKS.BusinessLogic.Entities
{ 
    public class Parcel
    {
        public Parcel()
        {
            VisitedHops = new List<HopArrival>();
            FutureHops = new List<HopArrival>();
        }
        /// <summary>
        /// Gets or Sets Weight
        /// </summary>
        public float? Weight { get; set; }

        /// <summary>
        /// Gets or Sets Receipient
        /// </summary>
        public Receipient Receipient { get; set; }

        //public string ReceipientName { get; set; }
        //public string ReceipientStreet { get; set; }
        //public string ReceipientPostalCode { get; set; }
        //public string ReceipientCity { get; set; }
        //public string ReceipientCountry { get; set; }

        public Receipient Sender { get; set; }

        //public string SenderName { get; set; }
        //public string SenderStreet { get; set; }
        //public string SenderPostalCode { get; set; }
        //public string SenderCity { get; set; }
        //public string SenderCountry { get; set; }

        /// <summary>
        /// The tracking ID of the parcel. 
        /// </summary>
        /// <value>The tracking ID of the parcel. </value>
        public string TrackingId { get; set; }

        /// <summary>
        /// The tracking ID from the LogisticPartner
        /// </summary>
        /// <value>The tracking ID from the LogisticPartner </value>
        public string OldTrackingId { get; set; }

        /// <summary>
        /// State of the parcel.
        /// </summary>
        /// <value>State of the parcel.</value>
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public enum StateEnum
        {
            /// <summary>
            /// Enum InTransportEnum for InTransport
            /// </summary>
            [EnumMember(Value = "InTransport")]
            InTransportEnum = 0,
            /// <summary>
            /// Enum InTruckDeliveryEnum for InTruckDelivery
            /// </summary>
            [EnumMember(Value = "InTruckDelivery")]
            InTruckDeliveryEnum = 1,
            /// <summary>
            /// Enum DeliveredEnum for Delivered
            /// </summary>
            [EnumMember(Value = "Delivered")]
            DeliveredEnum = 2
        }

        /// <summary>
        /// State of the parcel.
        /// </summary>
        /// <value>State of the parcel.</value>
        public StateEnum? State { get; set; }

        /// <summary>
        /// Hops visited in the past.
        /// </summary>
        /// <value>Hops visited in the past.</value>
        public List<HopArrival> VisitedHops { get; set; }

        /// <summary>
        /// Hops coming up in the future - their times are estimations.
        /// </summary>
        /// <value>Hops coming up in the future - their times are estimations.</value>
        public List<HopArrival> FutureHops { get; set; }
    }
}
