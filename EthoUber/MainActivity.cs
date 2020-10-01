using System;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Location;
using Android.Gms.Location.Places.UI;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using EthoUber.Helpers;
using Firebase;
using Firebase.Database;
using Google.Places;

namespace EthoUber
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback
    {

        #region firebase
        FirebaseDatabase database;
        readonly Button btnTestConnection;
        GoogleMap mainMap;
        #endregion permision

        #region Permisions
        readonly string[] permissionGroupLocation =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        const int requestedLocationID = 0;
        #endregion

        #region Location Request
        LocationRequest myLocRequest;
        FusedLocationProviderClient locApiClient;
        Android.Locations.Location myLastLoc;
        LocationCallbackHelper myLocationCallbackHelper;

        RelativeLayout zoomOutbtn;
        RelativeLayout mylocationbtn;
        TextView destinationupLbl;
        TextView pickupLbl;

        static int Update_Interval = 5; // seconds
        static int Fastest_Interval = 5; 
        static int Displacement = 3; // meters
        #endregion

        #region Helpers
        MapFunctionHelper mapFunctionHelper;
        //trip details
        LatLng pickupLocationLatLng;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Xamarin.Essentials.Platform.Init(this, savedInstanceState);
           
            this.SupportActionBar?.SetDisplayHomeAsUpEnabled(true);


            SetContentView(Resource.Layout.include_main);

            ConnectControl();

            
            var mapFragment = (SupportMapFragment) SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            zoomOutbtn = (RelativeLayout)FindViewById(Resource.Id.zoomOutLayout);
            zoomOutbtn.Click += ZoomOutMap;
            mylocationbtn = (RelativeLayout)FindViewById(Resource.Id.mylocationbtn);
            mylocationbtn.Click += GetMyLocationByBtn;


            //GetMyLocation
            CheckLocationPermission();
            CreateLocationReq();
            GetMyLocation();
            StartLocationUpdate();
            //FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            //fab.Click += FabOnClick;
            
        }

        private void GetMyLocationByBtn(object sender, EventArgs e)
        {
            GetMyLocation();
        }

        private void ZoomOutMap(object sender, EventArgs e)
        {           
            mainMap.AnimateCamera(CameraUpdateFactory.ZoomOut());
        }

        void StartLocationUpdate()
        {
            if (CheckLocationPermission())
            {
                locApiClient.RequestLocationUpdates(myLocRequest, myLocationCallbackHelper, null);
            }
        }

        void StopLocationUpdate()
        {
            if (locApiClient != null && myLocationCallbackHelper != null)
            {
                locApiClient.RemoveLocationUpdates(myLocationCallbackHelper);
            }
        }



        void CreateLocationReq()
        {
            myLocRequest = new LocationRequest();
            myLocRequest.SetInterval(Update_Interval);
            myLocRequest.SetFastestInterval(Fastest_Interval);
            myLocRequest.SetSmallestDisplacement(Displacement);
            myLocRequest.SetPriority(LocationRequest.PriorityBalancedPowerAccuracy);
            locApiClient = LocationServices.GetFusedLocationProviderClient(this);
            myLocationCallbackHelper = new LocationCallbackHelper();
            myLocationCallbackHelper.MyLocation += MLocationCallbackHelper_MyLocation;
        }

        private void MLocationCallbackHelper_MyLocation(object sender, LocationCallbackHelper.OnLocationCapturedEventArgs e)
        {
            myLastLoc = e.Location;
            LatLng mypos = new LatLng(myLastLoc.Latitude, myLastLoc.Longitude);
            mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(mypos,16));
        }

        async void GetMyLocation()
        {
            if (!CheckLocationPermission())
            {
                return;
            }
            myLastLoc = await locApiClient.GetLastLocationAsync();
            if (myLastLoc != null)
            {
                LatLng myposition = new LatLng(myLastLoc.Latitude,myLastLoc.Longitude);
                mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(myposition,18));
            }
        }

      
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {

            if (grantResults[0] == (int)Android.Content.PM.Permission.Granted)
            {

                Toast.MakeText(this, "Permision was granted", ToastLength.Short).Show();
            }
            else
            {
                StopLocationUpdate();
                Toast.MakeText(this, "Permision was denied", ToastLength.Short).Show();
            }
        }

        bool CheckLocationPermission()
        {
            bool permissionGranted = false;
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                permissionGranted = false;
                RequestPermissions(permissionGroupLocation, requestedLocationID);
            }
            else permissionGranted = true;

            return permissionGranted;
        }


        public void OnMapReady(GoogleMap googleMap)
        {
            bool success = googleMap.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(this, Resource.Raw.mapStyle));
            mainMap = googleMap;
            mainMap.CameraIdle += MainMap_CameraIdle;
            string mapKey = Resources.GetString(Resource.String.mapApiKey);
            mapFunctionHelper = new MapFunctionHelper(mapKey,mainMap);
        }

        private async void MainMap_CameraIdle(object sender, EventArgs e)
        {
            pickupLocationLatLng = mainMap.CameraPosition.Target;
            pickupLbl.Text =await mapFunctionHelper.FindCordinateAddress(pickupLocationLatLng);
        }

        void ConnectControl()
        {

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Mipmap.ic_menu_action_white);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

           

            pickupLbl = (TextView)FindViewById(Resource.Id.pickupLbl);
            destinationupLbl = (TextView)FindViewById(Resource.Id.destinationupLbl);

            RelativeLayout layoutPickUp;
            RelativeLayout layoutDestination;

            layoutPickUp = (RelativeLayout)FindViewById(Resource.Id.layoutPickUp);
            layoutDestination = (RelativeLayout)FindViewById(Resource.Id.layoutDestination);

            layoutPickUp.Click += layoutPickUp_Click;
            layoutDestination.Click += layoutDestination_Click;

            if (!PlacesApi.IsInitialized)
            {
                PlacesApi.Initialize(this, Resources.GetString(Resource.String.mapApiKey));
            }


        }

        private void layoutDestination_Click(object sender, EventArgs e)
        {
            List<Place.Field> fields = new List<Place.Field>();
            fields.Add(Place.Field.Id);
            fields.Add(Place.Field.Name);
            fields.Add(Place.Field.LatLng);
            fields.Add(Place.Field.Address);
            Intent intent = new Autocomplete.IntentBuilder(AutocompleteActivityMode.Overlay, fields)
                .SetCountry("Us")
                .Build(this);
            StartActivityForResult(intent, 2);
        }

        private void layoutPickUp_Click(object sender, EventArgs e)
        {
            List<Place.Field> fields = new List<Place.Field>();
            fields.Add(Place.Field.Id);
            fields.Add(Place.Field.Name);
            fields.Add(Place.Field.LatLng);
            fields.Add(Place.Field.Address);
            Intent intent = new Autocomplete.IntentBuilder(AutocompleteActivityMode.Overlay, fields)
                .SetCountry("Us")
                .Build(this);
            StartActivityForResult(intent, 1);
        }

        private void BtntestConnection_Click(object sender, EventArgs e)
        {
            InitDatabase();
        }

        void InitDatabase()
        {
            var app = FirebaseApp.InitializeApp(this);
            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetApplicationId("ethouberapp")
                    .SetApiKey(Resources.GetString(Resource.String.FireBApiKey))//
                    .SetDatabaseUrl("https://ethouberapp.firebaseio.com")
                    .SetStorageBucket("ethouberapp.appspot.com")
                    .Build();
                app = FirebaseApp.InitializeApp(this, options);
                database = FirebaseDatabase.GetInstance(app);
            }
            else {
                database = FirebaseDatabase.GetInstance(app);
            }

            DatabaseReference dbref = database.GetReference("UserSupport");
            dbref.SetValue("Ticket");
            Toast.MakeText(this, "Db reference completed",ToastLength.Short).Show();
         }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Android.App.Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 1)
            {
                if (resultCode == Android.App.Result.Ok)
                {
                    var place = Autocomplete.GetPlaceFromIntent(data);
                    pickupLbl.Text = place.Address.ToString();
                    mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(place.LatLng,15));
                    LatLng loc = new LatLng(place.LatLng.Latitude, place.LatLng.Longitude);
                    MarkerOptions options = new MarkerOptions()
                        .SetTitle(place.Address.ToString())
                        .SetPosition(loc)
                        //.SetIcon((BitmapDescriptor)Resource.Drawable.greenmarker)
                        ;
                    mainMap.AddMarker(options);
                }
            }

            if (requestCode == 2)
            {
                if (resultCode == Android.App.Result.Ok)
                {
                    var place = Autocomplete.GetPlaceFromIntent(data);
                    destinationupLbl.Text = place.Address.ToString();
                    mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(place.LatLng, 15));
                    LatLng loc = new LatLng(place.LatLng.Latitude, place.LatLng.Longitude);
                    MarkerOptions options = new MarkerOptions()
                        .SetTitle(place.Address.ToString())
                        .SetPosition(loc)
                       // .SetIcon((BitmapDescriptor)Resource.Drawable.redmarker)
                        ;
                    mainMap.AddMarker(options);
                }
            }
        }
        
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        /*
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }*/

        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //    MenuInflater.Inflate(Resource.Menu.menu_main, menu);
        //    return true;
        //}

    }
}
