using System;
using System.Collections.Generic;

namespace KissAnime.Models
{
	/// <summary>
	/// An episode of an anime.
	/// </summary>
	public class Episode
	{
		/// <summary>
		/// Gets or sets the name of the episode.
		/// </summary>
		/// <value>The name.</value>
		public string Name {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the episode URI.
		/// </summary>
		/// <value>The episode URI.</value>
		public Uri EpisodeUri {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the video sources.
		/// </summary>
		/// <value>The video sources.</value>
		public List<VideoSource> VideoSources {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the anime the episode is from.
		/// </summary>
		/// <value>The anime.</value>
		public Anime Anime {
			get;
			set;
		}
	}
}
