using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimeViewer.Models;

namespace AnimeViewer.EventArguments
{
    public class UpdatedCachedAnimeEventArgs : EventArgs
    {
        public Anime Anime { get; set; }
    }
}
