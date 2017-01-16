using System.Collections.Generic;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace AnimeViewer.Models
{
    public class Anime
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string PageUrl { get; set; }

        public string Summary { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Episode> Episodes { get; set; }

        public string Genres { get; set; }

        public bool JustUpdated { get; set; }
        public bool IsHot { get; set; }

        public virtual int LanguageId { get; set; }

        public AnimeLanguage Language
        {
            get { return (AnimeLanguage) LanguageId; }
            set { LanguageId = (int) value; }
        }

        public bool ContainsAllInformation { get; set; }
    }

    public enum AnimeLanguage
    {
        JapaneseSub,
        EnglishDub,
        Unknown
    }
}