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
        public bool Play(string source)
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

            // Allow sound thru muted state
            AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Playback);
            // Remove visible statusbar
            UIApplication.SharedApplication.SetStatusBarHidden(true, UIStatusBarAnimation.Fade);

            return true;
        }
    }
}