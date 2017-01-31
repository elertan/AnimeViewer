using FFImageLoading.Forms.Touch;
using FFImageLoading.Transformations;
using Foundation;
using JPC.BindablePicker;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms.Themes;
using Xamarin.Forms.Themes.iOS;
using XFGloss.iOS;

namespace AnimeViewer.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        public static AppDelegate Delegate;
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.Init();
            /********** ADD THIS CALL TO INITIALIZE XFGloss *********/
            Library.Init();
            LoadApplication(new App());

            var x = typeof(DarkThemeResources);
            x = typeof(UnderlineEffect);
            x = typeof(BlurredTransformation);
            x = typeof(BindablePicker);

            CachedImageRenderer.Init();
            //app.SetStatusBarHidden(true, UIStatusBarAnimation.Fade);

            Delegate = this;

            return base.FinishedLaunching(app, options);
        }
    }
}