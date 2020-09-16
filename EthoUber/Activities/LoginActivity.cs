
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using EthoUber.Events;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

namespace EthoUber.Activities
{
    [Activity(Label = "@string/app_name",
        Theme = "@style/NotUberTheme",
        MainLauncher =true)]
    public class LoginActivity : AppCompatActivity
    {
        TextInputLayout emailTxt, passTxt;
            TextView textViewNotRegistered;
        Button loginBtn;
        CoordinatorLayout rootView;

        FirebaseAuth dbAuth;
    

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.login_layout);

            emailTxt = (TextInputLayout)FindViewById(Resource.Id.emailText);
            passTxt = (TextInputLayout)FindViewById(Resource.Id.passwordText);
            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootLogin);
            loginBtn = (Button)FindViewById(Resource.Id.btnLogin);
            textViewNotRegistered = (TextView)FindViewById(Resource.Id.textViewNotRegistered);
            textViewNotRegistered.Click += TextViewNotRegistered_Click;
            loginBtn.Click += LoginBtn_Click;
            InitDatabase();


        }

        private void TextViewNotRegistered_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(RegisterActivity));
            StartActivity(intent);
            Finish();
        }

        private void LoginBtn_Click(object sender, EventArgs e)
        {
            string email, pass;
            email = emailTxt.EditText.Text;
            pass = passTxt.EditText.Text;

            if (!email.Contains("@"))
            {
                Toast.MakeText(this, " Please try a valid email  ", ToastLength.Short).Show();
            }
            else if (pass.Length < 8)
            {
                Toast.MakeText(this, " Please provide a valid password  ", ToastLength.Short).Show();
            }

            TaskCompletionListener TaskCompletionListener = new TaskCompletionListener();
            TaskCompletionListener.Success += TaskCompetionListener_Success;
            TaskCompletionListener.Failer += TaskCompetionListener_Failer;

            dbAuth.SignInWithEmailAndPassword(email, pass)
                .AddOnSuccessListener(TaskCompletionListener)
                .AddOnFailureListener(TaskCompletionListener)
                ;

        }

        private void TaskCompetionListener_Failer(object sender, EventArgs e)
        {
            Toast.MakeText(this, " Please try again ", ToastLength.Short).Show();
        }

        private void TaskCompetionListener_Success(object sender, EventArgs e)
        {
            StartActivity(typeof(MainActivity));
            Finish();
        }

        void InitDatabase()
        {
            var app = FirebaseApp.InitializeApp(this);
            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetApplicationId("ethouberapp")
                    .SetApiKey("AIzaSyBazRVFLI9bdr0IMJAaN2kHS0UROuN0tb8")
                    .SetDatabaseUrl("https://ethouberapp.firebaseio.com")
                    .SetStorageBucket("ethouberapp.appspot.com")
                    .Build();
                app = FirebaseApp.InitializeApp(this, options);
                dbAuth = FirebaseAuth.Instance;
            }
            else
            {
                dbAuth = FirebaseAuth.Instance;
            }

        }
    }
}
