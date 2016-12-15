using SQLite.Net.Attributes;

namespace AnimeViewer.Models
{
    public class AnimeDto
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string PageUrl { get; set; }

        public string Summary { get; set; }

        public bool ContainsAllInformation { get; set; }
    }
}