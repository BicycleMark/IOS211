using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AVFoundation;
using MediaPlayer;

namespace ExtendedMusicPlayer
{
	public partial class MusicListController : UITableViewController
	{
		public MusicListController (IntPtr handle) : base (handle)
		{
		}

		int currentSongIndex;
		UIBarButtonItem stopBtn;

		/// <summary>
		/// Import private API to allow exiting app manually.
		/// For demo purposes only! Do not use this in productive apps!
		/// </summary>
		/// <param name="status">Status.</param>
		[DllImport("__Internal", EntryPoint = "exit")]
		public static extern void Exit(int status);

		/// <summary>
		/// Every session needs a unique identifier.
		/// </summary>
		const string SessionId = "com.xamarin.musictransfersession";

		/// <summary>
		/// Our session used for transfer.
		/// </summary>
		public NSUrlSession Session {
			get;
			set;
		}

		AVAudioPlayer audioPlayer;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Add a bar button item to exit the app manually. Don't do this in productive apps - Apple won't approve it!
			// We have it here to demonstrate that iOS will relaunch the app if a download has finished.
			this.NavigationItem.LeftBarButtonItem = new UIBarButtonItem ("Quit", UIBarButtonItemStyle.Plain, delegate {
				// Store all download info to disk. If iOS terminates the app, this would be handled in WillTerminate().
				// However this won't be triggered if the app is killed manually by this call.
				AppDelegate.SerializeAvailableDownloads();

				// Exit application with code 3.
				MusicListController.Exit(3);
			});

			// Magic line: allows starting music playback when app is backgrounded or even terminated!
			UIApplication.SharedApplication.BeginReceivingRemoteControlEvents ();

			// We must set up an AVAudioSession and tell iOS we want to playback audio. Only then can music playback be used in the background.
			var audioSession = AVAudioSession.SharedInstance ();
			audioSession.SetCategory (AVAudioSessionCategory.Playback);
			audioSession.SetActive (true);

			// Add a bar button item to reset all downloads.
			var refreshBtn = new UIBarButtonItem (UIBarButtonSystemItem.Refresh);
			refreshBtn.Clicked += async (sender, e) => {
				// Stop audio.
				this.PlayAudio(null);

				var pendingTasks = await this.Session.GetTasks2Async();
				if(pendingTasks != null && pendingTasks.DownloadTasks != null)
				{
					foreach(var task in pendingTasks.DownloadTasks)
					{
						task.Cancel();
					}
				}

				AppDelegate.AvailableDownloads.ForEach(info => info.Reset(true));

				AppDelegate.ResetAvailableDownloads();
				((DelegateTableViewSource<DownloadInfo>)(this.TableView.Source)).Items = AppDelegate.AvailableDownloads;

				this.TableView.ReloadData();

				AppDelegate.SerializeAvailableDownloads();
			};


			// Add a button to stops playback.
			this.stopBtn = new UIBarButtonItem (UIBarButtonSystemItem.Stop) {
				Enabled = false
			};
			this.stopBtn.Clicked += (sender, e) => this.StopAudio();

			this.NavigationItem.RightBarButtonItems = new UIBarButtonItem[] { refreshBtn, this.stopBtn };


			// Initialize our session config. We use a background session to enabled out of process uploads/downloads. Note that there are other configurations available:
			// - DefaultSessionConfiguration: behaves like NSUrlConnection. Used if *background* transfer is not required.
			// - EphemeralSessionConfiguration: use if you want to achieve something like private browsing where all sesion info (e.g. cookies) is kept in memory only.
			using (var sessionConfig = UIDevice.CurrentDevice.CheckSystemVersion(8, 0)
				? NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration(SessionId)
				: NSUrlSessionConfiguration.BackgroundSessionConfiguration (SessionId))
			{
				// Allow downloads over cellular network too.
				sessionConfig.AllowsCellularAccess = true;

				// We want our app to be launched if required.
				//sessionConfig.SessionSendsLaunchEvents = true;

				// Give the OS a hint about what we are downloading. This helps iOS to prioritize. For example "Background" is used to download data that was not requested by the user and
				// should be ready if the app gets activated.
				sessionConfig.NetworkServiceType = NSUrlRequestNetworkServiceType.Default;

				// Configure how many downloads we allow at the same time. Set to 2 to see that further downloads start once the first two have been completed.
				sessionConfig.HttpMaximumConnectionsPerHost = 2;

				// Create a session delegate and the session itself
				// Initialize the session itself with the configuration and a session delegate.
				var sessionDelegate = new SessionDelegate (this);
				this.Session = NSUrlSession.FromConfiguration (sessionConfig, (INSUrlSessionDelegate)sessionDelegate, null);
			}

			this.TableView.Source = new DelegateTableViewSource<DownloadInfo>(this.TableView, "MUSIC_CELL")
			{
				Items = AppDelegate.AvailableDownloads,
				GetCellFunc = (item, cell) =>
				{
					var musicCell = (MusicCell)cell;
					musicCell.InitFromDownloadInfo(item);
					return musicCell;
				},
				RowSelectedFunc = (downloadInfo, cell, indexPath) =>
				{
					// If a row gets tapped, download or play.
					if(downloadInfo.Status != DownloadInfo.STATUS.Completed & downloadInfo.Status != DownloadInfo.STATUS.Downloading)
					{
						// Queue download.
						this.EnqueueDownloadAsync(downloadInfo);

						// Update UI once more. Download initialization might have failed	.
						((MusicCell)cell).InitFromDownloadInfo (downloadInfo);

						this.TableView.ReloadRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.None);
					}
					else if(downloadInfo.Status == DownloadInfo.STATUS.Completed)
					{
						// Play MP3.
						this.currentSongIndex = indexPath.Row;
						this.PlayAudio(downloadInfo);
					}
				}
			};

			this.TableView.EstimatedRowHeight = 100;
			this.TableView.RowHeight = UITableView.AutomaticDimension;
		}

		/// <summary>
		/// Stops audio output.
		/// </summary>
		void StopAudio()
		{
			if(this.audioPlayer != null)
			{
				this.audioPlayer.Stop();
			}
			this.stopBtn.Enabled = false;
			this.TableView.DeselectRow (NSIndexPath.FromRowSection (this.currentSongIndex, 0), true);
		}

		/// <summary>
		/// Plays the MP3 associated with the download. Stops output first.
		/// </summary>
		/// <param name="downloadInfo">Download info.</param>
		public void PlayAudio(DownloadInfo downloadInfo)
		{
			this.StopAudio ();

			if (downloadInfo == null)
			{
				return;
			}

			Console.WriteLine("Playing file '{0}'.", downloadInfo.DestinationFilename);

			if(!File.Exists(downloadInfo.FullDestinationFilePath))
			{
				Console.WriteLine("Cannot find file '{0}'!", downloadInfo.FullDestinationFilePath);
				return;
			}

			NSError error = null;

			// Use AVAudioPlayer to load the MP3 file and start playing.
			this.audioPlayer = new AVAudioPlayer(NSUrl.FromFilename(downloadInfo.FullDestinationFilePath), "mp3", out error);
			if(error == null)
			{
				audioPlayer.Play();
				this.stopBtn.Enabled = true;

				// Update information about currently played song.
				MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = new MPNowPlayingInfo
				{
					Artist = downloadInfo.Artist,
					Title = downloadInfo.Title
				};

				// Update selection in the table view.
				this.TableView.SelectRow (NSIndexPath.FromRowSection (this.currentSongIndex, 0), true, UITableViewScrollPosition.Middle);
			}
			else
			{
				Console.WriteLine("Error playing back audio: " + error);
			}
		}

		NSObject contentSizeNotification;

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			contentSizeNotification = UIApplication.Notifications.ObserveContentSizeCategoryChanged ((sender, args) => {
				this.TableView.ReloadData ();
			});
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			contentSizeNotification.Dispose ();
		}

		/// <summary>
		/// Adds another download to the queue.
		/// </summary>
		/// <returns>The download task.</returns>
		/// <param name="downloadInfo">Download info.</param>
		void EnqueueDownloadAsync(DownloadInfo downloadInfo)
		{
			Console.WriteLine ("Creating new download task.");
			// Create a new download task.
			var downloadTask = this.Session.CreateDownloadTask (NSUrl.FromString (downloadInfo.Source));

			// The creation can fail. 
			if (downloadTask == null)
			{
				downloadInfo.Status = DownloadInfo.STATUS.Idle;
				var alert = new UIAlertController
				{
					Message = "Failed to create download task! Please retry."
				};
				alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
				PresentViewController(alert, true, null);

				return;
			}

			// The downloadInfo objects knows the NSUrlSession task it is related to.
			downloadInfo.TaskId = (int)downloadTask.TaskIdentifier;

			// Set download status so that UI updates and download cannot be restarted if it is already running.
			downloadInfo.Status = DownloadInfo.STATUS.Downloading;

			// Resume / start the download.
			downloadTask.Resume ();
			Console.WriteLine ("Starting download of '{0}'. State of task: '{1}'. ID: '{2}'", downloadInfo.Title, downloadTask.State, downloadTask.TaskIdentifier);
		}

		/// <summary>
		/// Gets called if a remote control event is received
		/// </summary>
		/// <param name="remoteEvent">Remote event.</param>
		public override void RemoteControlReceived (UIEvent remoteEvent)
		{
			if (remoteEvent.Type != UIEventType.RemoteControl)
			{
				return;
			}

			Console.WriteLine ("Received remote control event: " + remoteEvent.Subtype);

			switch (remoteEvent.Subtype)
			{
				case UIEventSubtype.RemoteControlTogglePlayPause:
					if (this.audioPlayer != null)
					{
						this.StopAudio ();
					}
					else
					{
						this.PlayAudio (AppDelegate.AvailableDownloads [this.currentSongIndex]);
					}
					break;

				case UIEventSubtype.RemoteControlPause:
				case UIEventSubtype.RemoteControlStop:
					this.StopAudio ();
					break;

				case UIEventSubtype.RemoteControlPlay:
				case UIEventSubtype.RemoteControlPreviousTrack:
				case UIEventSubtype.RemoteControlNextTrack:
					if (remoteEvent.Subtype == UIEventSubtype.RemoteControlPreviousTrack)
					{
						if (this.currentSongIndex > 0)
						{
							this.currentSongIndex--;
						}
					}
					else if (remoteEvent.Subtype == UIEventSubtype.RemoteControlNextTrack)
					{
						if (this.currentSongIndex < AppDelegate.AvailableDownloads.Count - 1)
						{
							this.currentSongIndex++;
						}
					}
					this.PlayAudio (AppDelegate.AvailableDownloads [this.currentSongIndex]);
					break;
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			// React to remote control events.
			UIApplication.SharedApplication.BeginReceivingRemoteControlEvents ();
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

			// Stop reacting to remote control events.
			UIApplication.SharedApplication.EndReceivingRemoteControlEvents ();
		}
	}
}
