using System;
using System.Threading.Tasks;
using Android.Content.Res;
using System.Net.Http;
using Android.Gms.Maps.Model;
using Newtonsoft.Json;
using EthoUber.Helpers;
using ufinix.Helpers;
using Com.Google.Maps.Android;
using System.Collections;
using Android.Graphics;
using Android.Gms.Maps;

namespace EthoUber.Helpers
{
    public class MapFunctionHelper
    {
        string apikey;
        GoogleMap map;
        public MapFunctionHelper(string apiKey, GoogleMap mmap)
        {
            apikey = apiKey;
            map = mmap;
        }

        public string GetGeoCodeUrl(double lat, double lng)
        {
            string url =
            "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + lat +
            "," + lng + "&key=" + apikey;
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

        public async Task<string> GetDirectionJsonAsync(LatLng location, LatLng destination)
        {
            string str_origin = "origin=" + location.Latitude + "," + location.Longitude;
            string str_destination = "destination=" + destination.Latitude + "," + destination.Longitude;
            string mode = "mode=driving";
            string param = str_origin + "&" + str_destination + "&" + mode;
            string key = apikey;
            string output = "json";
            string url = "https://maps.googleapis.com/maps/api/directions/" + output + "?" + param + key;
            string json = "";
            json = await GetGeoJsonAsync(url);
            return json;
        }

        public void DrawTriponMap(string json)
        {
            var directionData = JsonConvert.DeserializeObject<DirectionParser>(json);
            var points = directionData.routes[0].overview_polyline.points;
            var line = PolyUtil.Decode(points);

            ArrayList routeList = new ArrayList();
            foreach (LatLng item in line)
            {
                routeList.Add(item);
            }

            PolylineOptions polylineOptions = new PolylineOptions()
                .AddAll((Java.Lang.IIterable)routeList)
                .InvokeWidth(10)
                .InvokeColor(Color.Teal)
                .InvokeStartCap(new SquareCap())
                .InvokeJointType(JointType.Round)
                .Geodesic(true);
            Android.Gms.Maps.Model.Polyline mPolyline = map.AddPolyline(polylineOptions);

            LatLng firstpoint = line[0];
            LatLng lastpoint = line[line.Count - 1];

            MarkerOptions pickupMarkerOptions = new MarkerOptions();
            pickupMarkerOptions.SetPosition(firstpoint);
            pickupMarkerOptions.SetTitle("PickUp Location");
            pickupMarkerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue));

            MarkerOptions destinationMarkerOptions = new MarkerOptions();

            destinationMarkerOptions.SetPosition(lastpoint);
            destinationMarkerOptions.SetTitle("Destination");
            destinationMarkerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));

            Marker pickupMarker = map.AddMarker(pickupMarkerOptions);
            Marker destinationMarker = map.AddMarker(destinationMarkerOptions);

            //trip bounds
            double southlng = directionData.routes[0].bounds.southwest.lng;
            double southlat = directionData.routes[0].bounds.southwest.lat;

            double northlng = directionData.routes[0].bounds.northeast.lng;
            double northlat = directionData.routes[0].bounds.northeast.lat;

            LatLng southwest = new LatLng(southlat, southlng);
            LatLng northeast = new LatLng(northlat, northlng);

            LatLngBounds tripBound = new LatLngBounds(southwest,northeast);
            map.AnimateCamera(CameraUpdateFactory.NewLatLngBounds(tripBound,470));
            map.SetPadding(40,70,40,70);
            pickupMarker.ShowInfoWindow();

        }


    } 
}
