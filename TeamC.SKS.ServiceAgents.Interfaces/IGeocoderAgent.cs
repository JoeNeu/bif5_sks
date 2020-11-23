using Geocoding;
using System.Threading.Tasks;

namespace TeamC.SKS.ServiceAgents.Interfaces
{
    public interface IGeocoderAgent
    {
        Location EncodeGeocodeAsync(string address);

        Task<Address> DecodeGeocodeAsync(Location location);
    }
}
