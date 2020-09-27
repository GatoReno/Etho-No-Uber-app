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
        }
    }
}