using AnimeViewer.iOS.Services;
using AnimeViewer.Services;
using AVFoundation;
using AVKit;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(VideoPlayer))]

namespace AnimeViewer.iOS.Services
{
    public class VideoPlayer : IVideoPlayer
    {
        public void Play(string source)
        {
            var player = new AVPlayer(new NSUrl(source));
            var playerViewController = new AVPlayerViewController {Player = player};

            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
                vc = vc.PresentedViewController;
            vc.PresentViewController(playerViewController, true, null);
            playerViewController.View.Frame = vc.View.Frame;
            player.Play();

            UIApplication.SharedApplication.SetStatusBarHidden(true, UIStatusBarAnimation.Fade);
        }
    }
}