using System;
using Foundation;
using UIKit;
using System.IO;
using AVFoundation;
using MediaPlayer;

namespace MusicPlayer
{
	public partial class MusicListController : UITableViewController
	{
		public MusicListController (IntPtr handle) : base (handle)
		{
		}

		AVAudioPlayer audioPlayer;
		UIBarButtonItem stopBtn;
		int currentSongIndex;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// TODO: set up an AVAudioSession and tell iOS we want to playback audio.
//            var audioSession = AVAudioSession.SharedInstance();
//            audioSession.SetCategory (AVAudioSessionCategory.Playback);
//            audioSession.SetActive (true);

			// Setup UI.
			this.InitUi ();
		}

		/// <summary>
		/// Plays a song.
		/// </summary>
		/// <param name="songInfo">Song info.</param>
		void PlayAudio(SongInfo songInfo)
		{
			// Stop previous output.
			this.StopAudio ();

			Console.WriteLine("Playing file '{0}'.", songInfo.DestinationFilename);
			if(!File.Exists(songInfo.FullDestinationFilePath))
			{
				Console.WriteLine("Cannot find file '{0}'!", songInfo.FullDestinationFilePath);
				return;
			}

			NSError error = null;

			// TODO: use AVAudioPlayer to load the MP3 file and start playing.
            // assign it to the existing audioPlayer field so it stays in memory.

			if(error == null)
			{
                // TODO: play song using the audioPlayer field.

				this.stopBtn.Enabled = true;

                // Update information about currently played song.
                MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = new MPNowPlayingInfo
                {
                    Artist = songInfo.Artist,
                    Title = songInfo.Title,
                    PlaybackRate = audioPlayer.Rate,
                    ElapsedPlaybackTime = audioPlayer.CurrentTime
                };

   				// Update selection in the table view.
				this.TableView.SelectRow (NSIndexPath.FromRowSection (this.currentSongIndex, 0), true, UITableViewScrollPosition.Middle);
			}
			else
			{
				Console.WriteLine("Error playing back audio: " + error);
			}
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

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			// TODO: start reacting to remote control events.
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

			// TODO: stop reacting to remote control events.
		}

		/// <summary>
		/// Sets up the UI
		/// </summary>
		void InitUi()
		{
			// Add a button to stops playback.
			this.stopBtn = new UIBarButtonItem (UIBarButtonSystemItem.Stop) {
				Enabled = false
			};
			this.stopBtn.Clicked += (sender, e) => this.StopAudio();
			this.NavigationItem.RightBarButtonItem = this.stopBtn;

			// Fill the tableview with available songs.
			this.TableView.Source = new DelegateTableViewSource<SongInfo>(this.TableView, "MUSIC_CELL")
			{
				// Show a list of available songs.
				Items = SongInfo.Songs,

				// Populate cells with song information.
				GetCellFunc = (item, cell) =>
				{
					var musicCell = (MusicCell)cell;
					musicCell.InitFromSongInfo(item);
					return musicCell;
				},

				// Play song if a row was selected.
				RowSelectedFunc = (songInfo, cell, indexPath) =>
				{
					this.currentSongIndex = indexPath.Row;
					this.PlayAudio(songInfo);
				}
			};

			this.TableView.RowHeight = 150;
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

			Console.WriteLine("Received remote control event: " + remoteEvent.Subtype);

			switch(remoteEvent.Subtype)
			{
				case UIEventSubtype.RemoteControlTogglePlayPause:
					if (this.audioPlayer != null)
					{
						this.StopAudio ();
					}
					else
					{
						this.PlayAudio (SongInfo.Songs[this.currentSongIndex]);
					}
					break;

				case UIEventSubtype.RemoteControlPause:
				case UIEventSubtype.RemoteControlStop:
					this.StopAudio();
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
						if (this.currentSongIndex < SongInfo.Songs.Count - 1)
						{
							this.currentSongIndex++;
						}
					}
					this.PlayAudio(SongInfo.Songs[this.currentSongIndex]);
					break;
			}
		}

#region Support dynamic text size changes
		NSObject contentSizeNotification;

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			contentSizeNotification = UIApplication.Notifications
                .ObserveContentSizeCategoryChanged ((sender, e) => 
                    this.TableView.ReloadData());
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			contentSizeNotification.Dispose ();
		}
#endregion
	}
}