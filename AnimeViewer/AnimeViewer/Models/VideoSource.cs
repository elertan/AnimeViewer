namespace AnimeViewer.Models
{
    public class VideoSource
    {
        /// <summary>
        ///     Gets or sets the quality. (Ex: 720p or 1080p)
        /// </summary>
        /// <value>The quality.</value>
        public string Quality { get; set; }

        /// <summary>
        ///     Gets or sets the source URI.
        /// </summary>
        /// <value>The source URI.</value>
        public string SourceUrl { get; set; }
    }
}