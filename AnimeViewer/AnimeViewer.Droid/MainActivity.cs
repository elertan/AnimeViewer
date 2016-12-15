using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using FFImageLoading.Forms.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Themes;
using Xamarin.Forms.Themes.Android;
using Color = Android.Graphics.Color;

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

            var x = typeof(DarkThemeResources);
            x = typeof(UnderlineEffect);

            CachedImageRenderer.Init();

            //Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            //Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            //Window.SetStatusBarColor(Color.ParseColor("#0d9666"));
        }
    }
}