using System;
using Newtonsoft.Json.Linq;
using TeamC.SKS.Package.Services.DTOs.Models;

namespace TeamC.SKS.Package.Services.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class HopJsonConverter : JsonCreationConverter<Hop>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="jObject"></param>
        /// <returns></returns>
        protected override Hop Create(Type objectType, JObject jObject)
        {
            if (jObject == null) throw new ArgumentNullException("jObject");

            if (jObject["nextHops"] != null)
                return new Warehouse();
            else if (jObject["numberPlate"] != null)
                return new Truck();
            else if (jObject["logisticsPartnerUrl"] != null)
                return new Transferwarehouse();
            else
                throw new Newtonsoft.Json.JsonSerializationException("Not a valid subclass of abstract class Hop!");
        }
    }
}