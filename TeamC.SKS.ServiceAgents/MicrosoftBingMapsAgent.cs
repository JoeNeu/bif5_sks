using Geocoding;
using Geocoding.Microsoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamC.SKS.ServiceAgents.Interfaces;

namespace TeamC.SKS.ServiceAgents
{
    public class MicrosoftBingMapsAgent : IGeocoderAgent
    {
        private readonly string ApiKey = ""; //Insert Key
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        IGeocoder Geocoder { get; set; }


        public MicrosoftBingMapsAgent()
        {
            Geocoder = new BingMapsGeocoder(ApiKey);
        }

        public Location EncodeGeocodeAsync(string address)
        {
            var addresses = Geocoder.GeocodeAsync(address);
            addresses.Wait();
            Location output = addresses.Result.First().Coordinates;
            
            return output;
        }

        public async Task<Address> DecodeGeocodeAsync(Location location)
        {
            IEnumerable<Address> locations = await Geocoder.ReverseGeocodeAsync(location);
            return locations.FirstOrDefault();
        }
    }
}
