using System.Collections.Generic;

namespace NineAnimeApi.Models
{
    public class Anime
    {
        public string Identifier { get; set; }
        public string PageUrl { get; set; }

        public string PosterImageUrl { get; set; }

        public string Name { get; set; }

        public string OtherNames { get; set; }

        public string Type { get; set; }

        public string Genres { get; set; }

        public string DateAired { get; set; }

        public string Status { get; set; }

        public float Score { get; set; }

        public int AmountOfScores { get; set; }

        public float Rating { get; set; }

        public int AmountOfRatings { get; set; }

        public string Duration { get; set; }

        public string Quality { get; set; }

        public string Summary { get; set; }

        public string Keywords { get; set; }

        public List<Episode> Episodes { get; set; }
    }
}