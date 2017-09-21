// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace ExtendedMusicPlayer
{
	[Register ("MusicCell")]
	partial class MusicCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView imgViewCover { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblArtist { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblDownloaded { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblSongTitle { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblWebsite { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIProgressView progressBar { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (imgViewCover != null) {
				imgViewCover.Dispose ();
				imgViewCover = null;
			}
			if (lblArtist != null) {
				lblArtist.Dispose ();
				lblArtist = null;
			}
			if (lblDownloaded != null) {
				lblDownloaded.Dispose ();
				lblDownloaded = null;
			}
			if (lblSongTitle != null) {
				lblSongTitle.Dispose ();
				lblSongTitle = null;
			}
			if (lblWebsite != null) {
				lblWebsite.Dispose ();
				lblWebsite = null;
			}
			if (progressBar != null) {
				progressBar.Dispose ();
				progressBar = null;
			}
		}
	}
}
