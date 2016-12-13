using System.Collections.Generic;
using System.Linq;
using AnimeViewer.Models;
using NineAnimeApi.Models;

namespace AnimeViewer
{
    public static class ExtensionsMethods
    {
        public static Anime ToAnime(this AnimeDto animeDto)
        {
            return new Anime
            {
                Name = animeDto.Name,
                PosterImageUrl = animeDto.ImageUrl
            };
        }

        public static AnimeDto ToAnimeDto(this Anime anime)
        {
            return new AnimeDto
            {
                Name = anime.Name,
                ImageUrl = anime.PosterImageUrl
            };
        }

        public static List<Anime> ToAnimes(this List<AnimeDto> animeDtos)
        {
            return animeDtos.Select(animeDto => animeDto.ToAnime()).ToList();
        }

        public static List<AnimeDto> ToAnimeDtos(this List<Anime> animes)
        {
            return animes.Select(anime => anime.ToAnimeDto()).ToList();
        }
    }
}