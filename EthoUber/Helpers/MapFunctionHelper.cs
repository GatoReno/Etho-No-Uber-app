using System;
using System.Threading.Tasks;
using Android.Content.Res;
using System.Net.Http;
using Android.Gms.Maps.Model;
using Newtonsoft.Json;
using EthoUber.Helpers;

namespace EthoUber.Helpers
{
    public class MapFunctionHelper
    {
        string apikey ;
        public MapFunctionHelper(string apiKey)
        {
            apikey = apiKey;
        }

        public string GetGeoCodeUrl(double lat, double lng)
        {
             string url =
             "https://maps.googleapis.com/maps/api/geocode/json?latlng="+lat+
             "," +lng+ "&key=" + apikey;
            return url;
        }

        public async Task<string> GetGeoJsonAsync(string url)
        {
            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            string res = await client.GetStringAsync(url);
            return res;
        }

        public async Task<string> FindCordinateAddress(LatLng pos)
        {
            string url = GetGeoCodeUrl(pos.Latitude, pos.Longitude);
            string json = "";
            string placesAdd = "";

            json = await GetGeoJsonAsync(url);
            if (!string.IsNullOrEmpty(json))
            {
                var geoCodeData = JsonConvert.DeserializeObject<GeocodingParser>(json);
                if (geoCodeData.results[0] != null)
                {
                    placesAdd = geoCodeData.results[0].formatted_address;
                }
            }

            return placesAdd;
        }
    } 
}
