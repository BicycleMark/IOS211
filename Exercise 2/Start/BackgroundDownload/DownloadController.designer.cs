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

namespace BackgroundDownload
{
	[Register ("DownloadController")]
	partial class DownloadController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnStartDownload { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView imgView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIProgressView progressView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (btnStartDownload != null) {
				btnStartDownload.Dispose ();
				btnStartDownload = null;
			}
			if (imgView != null) {
				imgView.Dispose ();
				imgView = null;
			}
			if (progressView != null) {
				progressView.Dispose ();
				progressView = null;
			}
		}
	}
}
