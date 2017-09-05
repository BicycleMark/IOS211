using System;
using System.IO;
using System.Collections.Generic;

namespace MusicPlayer
{
	/// <summary>
	/// Song information
	/// </summary>
	public sealed class SongInfo
	{
		/// <summary>
		/// A list of available songs.
		/// </summary>
		public static List<SongInfo> Songs = new List<SongInfo> {
			new SongInfo
			{
				DestinationFilename = "earforce_ceremony.mp3",
				Title = "Ceremony",
				Artist = "Earforce",
				Website = "http://www.earforce-bigband.de",
				CoverImage = "music_earforce.png"
			},

			new SongInfo
			{
				DestinationFilename = "epic.mp3",
				Title = "Epic",
				Artist = "Bensound",
				Website = "http://www.bensound.com",
				CoverImage = "music_epic.png"
			},

			new SongInfo
			{
				DestinationFilename = "jazzcomedy.mp3",
				Title = "Jazz Comedy",
				Artist = "Bensound",
				Website = "http://www.bensound.com",
				CoverImage = "music_jazzcomedy.png"
			}
		};

		public string Title
		{
			get;
			set;
		}

		public string Artist
		{
			get;
			set;
		}

		public string Website
		{
			get;
			set;
		}

		public string CoverImage
		{
			get;
			set;
		}

		public string DestinationFilename
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the full path to the downloaded file from the app's documents folder.
		/// </summary>
		/// <value>The full destination file path.</value>
		public string FullDestinationFilePath
		{
			get
			{
				if (string.IsNullOrWhiteSpace (this.DestinationFilename))
				{
					return null;
				}

				var docsPath = Path.Combine("Songs", Path.GetFileName(this.DestinationFilename));
				return docsPath;
			}
		}

		public override string ToString ()
		{
			return string.Format ("[SongInfo: Title={0}, DestinationFile={1}]", Title, DestinationFilename);
		}
	}
}

