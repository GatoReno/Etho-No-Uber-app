using CoreGraphics;
using Firebase.Auth;
using Firebase.Database;
using Foundation;
using System;
using UIKit;

namespace EthoUber_Ios
{
    public partial class RegisterViewController : UIViewController
    {
        public RegisterViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            BtnSignin.Hidden = true;
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyWillChange);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyWillChange);

            EmailTextField.ShouldReturn = (textField) => {
                 textField.ResignFirstResponder();
                 return true;
             };

            NameTextField.ShouldReturn = (textField) => {
                 textField.ResignFirstResponder();
                 return true;
             };

            PasswordTextField.ShouldReturn = (textField) => {
                 textField.ResignFirstResponder();
                 return true;
             };

            PhoneTextField.ShouldReturn = (textField) => {
                 textField.ResignFirstResponder();
                 return true;
             };
            RegisterBtn.TouchUpInside += RegisterBtn_TouchUpInside;
        }

        private void RegisterBtn_TouchUpInside(object sender, EventArgs e)
        {
            string fullname, phone, email, password;
            fullname = NameTextField.Text;
            phone = PhoneTextField.Text;
            email = EmailTextField.Text;
            password = PasswordTextField.Text;
            if (fullname.Length < 5)
            {
                Alert("Error", "Please enter a valid name");
            }
            else if (phone.Length < 8)
            { Alert("Error", "Please enter a valid phone"); }
            else if (!email.Contains("@"))
            { Alert("Error", "Please enter a valid email"); }
            else if (password.Length < 8)
            { Alert("Error", "Please enter a valid password"); }
            else { 

            Auth.DefaultInstance.CreateUser(email, password,
                (AuthDataResult authresult, NSError error) =>
                {
                    if (error == null)
                    {
                        var user = authresult.User;
                        if (user != null)
                        {
                            Alert("Registration Success full", "Please go ahead and login ");
                            var userDictionary = new NSDictionary(
                                "fullname", fullname,
                                "email",email,
                                "phone",phone
                                );
                            // save user details to firebase db
                            DatabaseReference userRef = Database.DefaultInstance.GetRootReference().
                            GetChild("users/"+ authresult.User.Uid);

                            // save user info locally
                            var userDefaults = NSUserDefaults.StandardUserDefaults;


                        }
                        else
                        {
                            Alert("Error", error.LocalizedDescription);
                        }
                    }
                    else {
                        Alert("Error", error.LocalizedDescription);
                    }
                });

            }

        }

        void Alert(string title, string msn)
        {
            var alert = UIAlertController.Create(title, msn, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

            PresentViewController(alert, true, null);
        }
        void KeyWillChange(NSNotification notification)
        {
            if (notification.Name == UIKeyboard.WillShowNotification)
            {
                var keyboard = UIKeyboard.FrameBeginFromNotification(notification);
                CGRect frame = View.Frame;
                frame.Y = -keyboard.Height;
                View.Frame = frame;
            }

            if (notification.Name == UIKeyboard.WillHideNotification)
            {
                var keyboard = UIKeyboard.FrameBeginFromNotification(notification);
                CGRect frame = View.Frame;
                frame.Y = 0;
                View.Frame = frame;
            }
        }
    }
}