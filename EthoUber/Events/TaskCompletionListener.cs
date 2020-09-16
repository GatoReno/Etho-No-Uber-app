using System;
using Android.Gms.Tasks;

namespace EthoUber.Events
{
    public class TaskCompletionListener : Java.Lang.Object, IOnSuccessListener, IOnFailureListener
    {

        public event EventHandler Success;
        public event EventHandler Failer;
        public void OnFailure(Java.Lang.Exception e)
        {
            Failer?.Invoke(this, new EventArgs());
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            Success?.Invoke(this,new EventArgs());
        }
    }
}
