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
using Java.Util;

namespace EthoUber.Activities
{
    [Activity(Label = "@string/app_name",
        Theme = "@style/NotUberTheme",
        MainLauncher = false)]


    public class RegisterActivity : AppCompatActivity
    {
        #region ui Objects
        TextInputLayout nameText;
        TextInputLayout phoneText;
        TextInputLayout emailText;
        TextInputLayout passText;
        TextInputLayout passConfirmText;
        Button registerBtn;
        TextView textViewRegistered2;
        #endregion
        #region firebase Params
        FirebaseAuth dbAuth;
        FirebaseDatabase database;
        TaskCompletionListener TaskCompletionListener = new TaskCompletionListener();
        #endregion
        #region public Params
        string name, phone, email, password, passwordConfirm;
        #endregion
        #region preferences
        ISharedPreferences prefs = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor editor;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            this.SupportActionBar?.SetDisplayHomeAsUpEnabled(true);
            InitDatabase();
            dbAuth = FirebaseAuth.Instance;
            SetContentView(Resource.Layout.register_layout);
            ConnectControl();

           
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();//es el mismo evento del boton fisico
                    return true;
                default: return true;

            }
        }

        void ConnectControl() {

            nameText = (TextInputLayout)FindViewById(Resource.Id.nameText);
            phoneText = (TextInputLayout)FindViewById(Resource.Id.phoneNumber);
            emailText = (TextInputLayout)FindViewById(Resource.Id.emailText);
            passText = (TextInputLayout)FindViewById(Resource.Id.passwordText);
            passConfirmText = (TextInputLayout)FindViewById(Resource.Id.passwordTextConfirm);
            registerBtn = (Button)FindViewById(Resource.Id.btnRegist);
            textViewRegistered2 = (TextView)FindViewById(Resource.Id.textViewRegistered2); 
            registerBtn.Click += RegisterBtn_Click;
            textViewRegistered2.Click += TextViewRegistered2_Click;
        }

        private void TextViewRegistered2_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(LoginActivity));
            StartActivity(intent);
            Finish();
        }

        private void RegisterBtn_Click(object sender, EventArgs e)
        {

            password = passText.EditText.Text;
            passwordConfirm = passConfirmText.EditText.Text;
            name = nameText.EditText.Text;
            phone = phoneText.EditText.Text;
            email = emailText.EditText.Text;

            if (password != passwordConfirm)
            {

                Toast.MakeText(this, "Password doesn't match!", ToastLength.Short).Show();
            }
            else if (password.Length < 8) {
                Toast.MakeText(this, "Password to short!", ToastLength.Short).Show();

            }
            else if (name.Length < 3)
            {
                Toast.MakeText(this, "Name is to shot, please use your full name!", ToastLength.Short).Show();
            }
            else if (phone.Length < 9)
            {
                Toast.MakeText(this, "Please enter a valid phone number!", ToastLength.Short).Show();
            }
            else if (!email.Contains("@")) {
                Toast.MakeText(this, "Please enter a valid email!", ToastLength.Short).Show();
            }
            else
            {

                RegisterUser(name, email, phone, password);
            }

        }
        void RegisterUser(string name, string email, string phone, string pass) {


            TaskCompletionListener.Success += TaskCompetionListener_Success;
            TaskCompletionListener.Failer += TaskCompetionListener_Failer;

            dbAuth.CreateUserWithEmailAndPassword(email, pass)
                .AddOnSuccessListener(this, TaskCompletionListener)
                .AddOnFailureListener(this, TaskCompletionListener)
                ;
        }

        private void TaskCompetionListener_Failer(object sender, EventArgs e)
        {
            Toast.MakeText(this, "Registration FAILED please use a valid mail !", ToastLength.Short).Show();
        }

        private void TaskCompetionListener_Success(object sender, EventArgs e)
        {
            Toast.MakeText(this, "User Registration successful!", ToastLength.Short).Show();
            HashMap userMap = new HashMap();
            userMap.Put("email", email);
            userMap.Put("phone", phone);
            userMap.Put("userName", name);

            DatabaseReference userRef = database.GetReference("users/" + dbAuth.CurrentUser.Uid);
            userRef.SetValue(userMap);
        }

        void SaveToSharedPref()
        {
           
            editor = prefs.Edit();
            editor.PutString("email", email);
            editor.PutString("userName", name);
            editor.PutString("phone", phone);

            editor.Apply();
        }
        void RetriveData()
        {
            string email = prefs.GetString("email","");
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
                database = FirebaseDatabase.GetInstance(app);
            }
            else
            {
                database = FirebaseDatabase.GetInstance(app);
            }

        }




    }
}
