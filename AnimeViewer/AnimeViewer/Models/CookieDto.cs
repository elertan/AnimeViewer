using SQLite.Net.Attributes;

namespace AnimeViewer.Models
{
    public class CookieDto
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}