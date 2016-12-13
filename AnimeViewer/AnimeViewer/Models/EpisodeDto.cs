using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace AnimeViewer.Models
{
    public class EpisodeDto
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string  Name { get; set; }
    }
}
