using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace EthoUber_Ios
{
    public partial class LoginController : UIViewController
    {
        public LoginController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            EmailTextField.ShouldReturn = (textField) => {
                textField.ResignFirstResponder();
                return true;
            };

            PasswordTextField.ShouldReturn = (textField) => {
                textField.ResignFirstResponder();
                return true;
            };

            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyWillChange);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyWillChange);
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

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            this.View.EndEditing(true);
        }
    }
}