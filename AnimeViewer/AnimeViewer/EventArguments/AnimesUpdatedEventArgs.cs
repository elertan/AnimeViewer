using System;
using System.Collections.Generic;
using AnimeViewer.Models;

namespace AnimeViewer.EventArguments
{
    public class AnimesUpdatedEventArgs : EventArgs
    {
        public List<Anime> Animes { get; set; }
        public int Page { get; set; }
    }
}