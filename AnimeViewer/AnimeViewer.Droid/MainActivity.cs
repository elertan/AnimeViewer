using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using FFImageLoading.Forms.Droid;
using FFImageLoading.Transformations;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Themes;
using Xamarin.Forms.Themes.Android;
using XFGloss.Droid;

namespace AnimeViewer.Droid
{
    [Activity(Label = "AnimeViewer", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Forms.Init(this, bundle);
            LoadApplication(new App());

            // IMPORTANT: Initialize XFGloss AFTER calling LoadApplication on the Android platform
            Library.Init(this, bundle);

            var x = typeof(DarkThemeResources);
            x = typeof(UnderlineEffect);
            x = typeof(BlurredTransformation);

            CachedImageRenderer.Init();
            UserDialogs.Init(() => (Activity) Forms.Context);

            //Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            //Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            //Window.SetStatusBarColor(Color.ParseColor("#0d9666"));
        }
    }
}