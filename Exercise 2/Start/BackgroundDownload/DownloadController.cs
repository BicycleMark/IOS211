using Foundation;
using System;
using UIKit;
using System.IO;

namespace BackgroundDownload
{
	public partial class DownloadController : UIViewController
	{
		public DownloadController (IntPtr handle) : base (handle)
		{
		}

		/// <summary>
		/// Url of the 5MB monkey PNG file.
		/// </summary>
		const string downloadUrl = "http://xamarinuniversity.blob.core.windows.net/ios210/huge_monkey.png";

		/// <summary>
		/// Alternative URL for a smaller file in case of lower bandwidth.
		/// </summary>
		//const string downloadUrl = "http://xamarinuniversity.blob.core.windows.net/ios210/huge_monkey_sm.png";

		/// <summary>
		/// This is where the PNG will be saved to.
		/// </summary>
		public static string targetFilename =  Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "huge_monkey.png");

		/// <summary>
		/// Every session needs a unique identifier.
		/// </summary>
		const string sessionId = "com.xamarin.transfersession";

		/// <summary>
		/// Our session used for transfer.
		/// </summary>
		public NSUrlSession session;

		/// <summary>
		/// Gets called by the delegate and will update the progress bar as the download runs.
		/// </summary>
		/// <param name="percentage">Percentage.</param>
		public void UpdateProgress(float percentage)
		{
			this.progressView.SetProgress (percentage, true);
		}

		/// <summary>
		/// Gets called by the delegate and tells the controller to load and view the downloaded image.
		/// </summary>
		public void LoadImage()
		{
			this.imgView.Image = UIImage.FromFile (DownloadController.targetFilename);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Add a bar button item to exit the app manually. Don't do this in productive apps - Apple won't approve it!
			// We have it here to demonstrate that iOS will relaunch the app if a download has finished.
			this.NavigationItem.LeftBarButtonItem = new UIBarButtonItem ("Quit", UIBarButtonItemStyle.Plain, delegate {
				AppDelegate.Exit(3);
			});

			// Add a bar button item to reset the download.
			var refreshBtn = new UIBarButtonItem (UIBarButtonSystemItem.Refresh);
			refreshBtn.Clicked += async (sender, e) => {
				// Cancel all pending downloads.
				if(this.session != null)
				{
					var pendingTasks = await this.session.GetTasks2Async();
					if(pendingTasks != null && pendingTasks.DownloadTasks != null)
					{
						foreach(var task in pendingTasks.DownloadTasks)
						{
							task.Cancel();
						}
					}
				}
				// Delete downloaded file.
				if(File.Exists(targetFilename))
				{
					File.Delete(targetFilename);
				}

				// Update UI.
				this.imgView.Image = null;
				this.progressView.SetProgress(0, true);
				this.btnStartDownload.SetTitle("Start download!", UIControlState.Normal);
				this.btnStartDownload.Enabled = true;
			};

			this.NavigationItem.RightBarButtonItems = new UIBarButtonItem[] { refreshBtn };

			// Setup the NSUrlSession.
			this.InitializeSession ();

			// Start the download if the button is pressed.
			this.btnStartDownload.TouchUpInside += (sender, e) => {
				this.btnStartDownload.SetTitle("Download started...", UIControlState.Normal);
				this.btnStartDownload.Enabled = false;
				this.EnqueueDownload();
			};
		}

		/// <summary>
		/// Initializes the session.
		/// </summary>
		void InitializeSession()
		{
			// TODO: Initialize NSUrlSession.
		}

		/// <summary>
		/// Adds the download to the session.
		/// </summary>
		void EnqueueDownload()
		{
			Console.WriteLine ("Creating new download task.");

			// TODO: Create a new download task and start it.
		}
	}
}
