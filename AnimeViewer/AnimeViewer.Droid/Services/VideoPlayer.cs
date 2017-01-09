using Android.App;
using Android.Content;
using Android.Net;
using AnimeViewer.Droid.Services;
using AnimeViewer.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(VideoPlayer))]

namespace AnimeViewer.Droid.Services
{
	public class VideoPlayer : IVideoPlayer
	{
		public bool Play(string source)
		{
			var activity = (Activity)Forms.Context;

			try {
				var parsedSource = Uri.Parse(source);
				var i = new Intent(Intent.ActionView, parsedSource);
				i.SetDataAndType(parsedSource, "video/mp4");
				activity.StartActivity(i);

				return true;
			} catch {
				return false;
			}
		}
	}
}