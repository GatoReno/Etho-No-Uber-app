using Firebase.Database;
using Foundation;
using System;
using UIKit;

namespace EthoUber_Ios
{
    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            BtnTest.TouchUpInside += BtnTest_TouchUpInside;
        }

        private void BtnTest_TouchUpInside(object sender, EventArgs e)
        {
            DatabaseReference reference = Database.DefaultInstance.GetRootReference().GetChild("UserSupport");
            reference.SetValue<NSString>((NSString)"Fire IOS");
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
       


        }
    }
}