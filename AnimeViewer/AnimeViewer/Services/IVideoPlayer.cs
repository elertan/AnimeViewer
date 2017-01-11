namespace AnimeViewer.Services
{
    /// <summary>
    ///     Used to play videos on the specific platform
    /// </summary>
    public interface IVideoPlayer
    {
        /// <summary>
        ///     Plays a video by a video source
        /// </summary>
        /// <param name="source">The source</param>
        /// <returns>Success or not</returns>
        bool Play(string source);
    }
}