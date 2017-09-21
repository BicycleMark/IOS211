using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace ExtendedMusicPlayer
{
	partial class MusicCell : UITableViewCell
	{
		public MusicCell (IntPtr handle) : base (handle)
		{
		}

		public void InitFromDownloadInfo(DownloadInfo downloadInfo)
		{
			this.lblSongTitle.Font = UIFont.PreferredHeadline;
			this.lblArtist.Font = UIFont.PreferredSubheadline;
			this.lblWebsite.Font = UIFont.PreferredFootnote;
			this.lblDownloaded.Font = UIFont.PreferredSubheadline;


			this.lblSongTitle.Text = downloadInfo.Title;
			this.lblArtist.Text = downloadInfo.Artist;
			this.lblWebsite.Text = downloadInfo.Website;
			this.imgViewCover.Image = downloadInfo.CoverImage != null ? UIImage.FromFile(downloadInfo.CoverImage) : null;

			switch (downloadInfo.Status)
			{
				case DownloadInfo.STATUS.Completed:
					lblDownloaded.Text = "Downloaded!";
					break;

				case DownloadInfo.STATUS.Downloading:
					lblDownloaded.Text = "Downloading...";
					break;

				default:
					lblDownloaded.Text = string.Empty;
					break;
			}


			this.progressBar.Hidden = downloadInfo.Status != DownloadInfo.STATUS.Downloading;
			this.progressBar.SetProgress (downloadInfo.Progress / 1f, true);
		}
	}
}
