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
        }
    }
}