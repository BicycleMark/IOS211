using System;
using System.IO;

namespace ExtendedMusicPlayer
{
	/// <summary>
	/// Contains status information about the current downloads.
	/// </summary>
	[Serializable]
	public sealed class DownloadInfo
	{
		public enum STATUS
		{
			Idle,
			Downloading,
			Cancelled,
			Completed,
		}

		public DownloadInfo ()
		{
			this.Progress = -1;
			this.TaskId = -1;
			this.Status = STATUS.Idle;
		}

		public void Reset(bool delete)
		{
			this.Progress = -1.0f;
			this.Status = STATUS.Idle;
			this.TaskId = -1;

			if (delete)
			{
				if (this.FullDestinationFilePath != null)
				{
					try
					{
						var filename = this.FullDestinationFilePath;
						File.Delete (filename);
					}
					catch (Exception ex)
					{
						Console.WriteLine ("Failed to delete '{0}': {1}", this.FullDestinationFilePath, ex);
					}
				}
			}
			this.DestinationFilename = null;
		}

		/// <summary>
		/// Title shown during download.
		/// </summary>
		/// <value>The title.</value>
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

		/// <summary>
		/// Where the file is downloaded from.
		/// </summary>
		public string Source
		{
			get;
			set;
		}

		/// <summary>
		/// Download progress.
		/// </summary>
		public float Progress {
			get;
			set;
		}

		/// <summary>
		/// NSUrlSession gives as unique Task IDs.
		/// </summary>
		public nint TaskId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		public STATUS Status {
			get;
			set;
		}

		/// <summary>
		/// The name of the downloaded file on disk.
		/// </summary>
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

				var docsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.GetFileName(this.DestinationFilename));
				return docsPath;
			}
		}

		public override string ToString ()
		{
			return string.Format ("[DownloadInfo: Title={0}, Source={1}, Progress={2}, TaskId={3}, Status={4}, DestinationFile={5}]", Title, Source, Progress, TaskId, Status, DestinationFilename);
		}
	}
}

