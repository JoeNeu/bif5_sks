using Geocoding;
using Geocoding.Google;
using System;
using System.Linq;
using System.Threading.Tasks;
using TeamC.SKS.ServiceAgents.Interfaces;

namespace TeamC.SKS.ServiceAgents
{
    public class GoogleMapAgent : IGeocoderAgent
    {
        private readonly IGeocoder _geocoder;
        public GoogleMapAgent()
        {
            _geocoder = new GoogleGeocoder() { ApiKey = "I-dont-have-a-key" };
        }

        public Location EncodeGeocodeAsync(string RegionGeoJson)
        {
            var address = _geocoder.GeocodeAsync(RegionGeoJson);
            address.Wait();
            return address.Result.First().Coordinates;
        }
        public Task<Address> DecodeGeocodeAsync(Location location)
        {
            throw new NotImplementedException();
        }
    }
}
