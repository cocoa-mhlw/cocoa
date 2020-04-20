using System;
using UIKit;

namespace Covid19Radar.iOS.Controls
{
    public class UIBackwardsTextField : UITextField
    {
        // A delegate type for hooking up change notifications.
        public delegate void DeleteBackwardEventHandler(object sender, EventArgs e);

        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event DeleteBackwardEventHandler OnDeleteBackward;


        public void OnDeleteBackwardPressed()
        {
            OnDeleteBackward?.Invoke(null, null);
        }

        public UIBackwardsTextField()
        {
            BorderStyle = UITextBorderStyle.RoundedRect;
            ClipsToBounds = true;
        }

        public override void DeleteBackward()
        {
            base.DeleteBackward();
            OnDeleteBackwardPressed();
        }
    }
}
