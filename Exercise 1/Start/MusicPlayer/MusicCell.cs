using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace MusicPlayer
{
	partial class MusicCell : UITableViewCell
	{
		public MusicCell (IntPtr handle) : base (handle)
		{
			this.SelectionStyle = UITableViewCellSelectionStyle.None;
		}

		public void InitFromSongInfo(SongInfo songInfo)
		{
			// Update dynamic text.
			this.lblSongTitle.Font = UIFont.PreferredHeadline;
			this.lblArtist.Font = UIFont.PreferredSubheadline;
			this.lblWebsite.Font = UIFont.PreferredFootnote;


			this.lblSongTitle.Text = songInfo.Title;
			this.lblArtist.Text = songInfo.Artist;
			this.lblWebsite.Text = songInfo.Website;
			this.imgViewCover.Image = songInfo.CoverImage != null ? UIImage.FromFile(songInfo.CoverImage) : null;
		}
	}
}
