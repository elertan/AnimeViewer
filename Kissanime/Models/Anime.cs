using System;
using System.Collections.Generic;

namespace KissAnime.Models
{
	/// <summary>
	/// An anime.
	/// </summary>
	public class Anime
	{
		/// <summary>
		/// Gets or sets the name of the anime.
		/// </summary>
		/// <value>The name.</value>
		public string Name {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the summary.
		/// </summary>
		/// <value>The summary.</value>
		public string Summary {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the anime URI where the anime resides.
		/// </summary>
		/// <value>The anime URI.</value>
		public Uri AnimeUri {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the image URI.
		/// </summary>
		/// <value>The image URI.</value>
		public Uri ImageUri {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the episodes of the anime.
		/// </summary>
		/// <value>The episodes.</value>
		public List<Episode> Episodes {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the genres.
		/// </summary>
		/// <value>The genre.</value>
		public List<string> Genres {
			get;
			set;
		}
	}
}
