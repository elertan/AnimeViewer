using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineAnimeApi.Models;

namespace AnimeViewer.EventArguments
{
    public class AnimesUpdatedEventArgs : EventArgs
    {
        public List<Anime> Animes { get; set; }
        public int Page { get; set; }
    }
}
