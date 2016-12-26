using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace AnimeViewer.Models
{
    public class Episode
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Anime))]
        public int AnimeId { get; set; }

        [ManyToOne]
        public Anime Anime { get; set; }

        public string Name { get; set; }
        public string EpisodeUrl { get; set; }
    }
}