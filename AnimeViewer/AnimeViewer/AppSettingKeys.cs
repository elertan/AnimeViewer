namespace AnimeViewer
{
    public static class AppSettingKeys
    {
        // Global
        public static string NotFirstTimeOpeningApp => nameof(NotFirstTimeOpeningApp);

        // Settings
        // Video Player
        public static string VideoQuality => nameof(VideoQuality);
        public static string AutomaticallyPlayNextEpisode => nameof(AutomaticallyPlayNextEpisode);
        public static string AutomaticallyPlayNextEpisodeCancellableDelay => nameof(AutomaticallyPlayNextEpisodeCancellableDelay);
    }
}