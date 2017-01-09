using Android.App;
using Android.Views;
using Android.Widget;
using AnimeViewer.Droid.Services;
using AnimeViewer.Services;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(VideoPlayer))]

namespace AnimeViewer.Droid.Services
{
    public class VideoPlayer : IVideoPlayer
    {
        public void Play(string source)
        {
            var activity = (Activity) Application.Context;
            var videoView = new VideoView(activity);
            videoView.SetVideoPath(source);

            var mediaController = new MediaController(activity);
            videoView.SetMediaController(mediaController);

            var layout = new LinearLayout(activity) {LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)};
            layout.AddView(mediaController);

            activity.AddContentView(layout, layout.LayoutParameters);
        }
    }
}