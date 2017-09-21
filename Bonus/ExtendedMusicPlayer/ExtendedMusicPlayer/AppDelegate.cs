using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ExtendedMusicPlayer
{
	public class AppDelegate : UIApplicationDelegate
	{
		/// <summary>
		/// Path and filename of serialized info about possible downloads.
		/// </summary>
		public static string DownloadInfoFileLocation = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "serialized.dat");

		/// <summary>
		/// Prepare some files for download.
		/// </summary>
		public static List<DownloadInfo> AvailableDownloads = null;

		/// <summary>
		/// Resets the available downloads.
		/// </summary>
		public static void ResetAvailableDownloads()
		{
			AvailableDownloads = new List<DownloadInfo> {
				new DownloadInfo
				{
					DestinationFilename = "earforce.mp3",
					Source = "http://xamarinuniversity.blob.core.windows.net/ios210/earforce_ceremony.mp3",
					Status = DownloadInfo.STATUS.Idle,
					Title = "Ceremony",
					Artist = "Earforce",
					Website = "http://www.earforce-bigband.de",
					CoverImage = "music_earforce.png"
				},

				new DownloadInfo
				{
					DestinationFilename = "epic.mp3",
					Source = "http://xamarinuniversity.blob.core.windows.net/ios210/epic.mp3",
					Status = DownloadInfo.STATUS.Idle,
					Title = "Epic",
					Artist = "Bensound",
					Website = "http://www.bensound.com",
					CoverImage = "music_epic.png"
				},

				new DownloadInfo
				{
					DestinationFilename = "jazzcomedy.mp3",
					Source = "http://xamarinuniversity.blob.core.windows.net/ios210/jazzcomedy.mp3",
					Status = DownloadInfo.STATUS.Idle,
					Title = "Jazz Comedy",
					Artist = "Bensound",
					Website = "http://www.bensound.com",
					CoverImage = "music_jazzcomedy.png"
				}
			};
		}

		/// <summary>
		/// Serializes available downloads.
		/// </summary>
		public static void SerializeAvailableDownloads()
		{
			Console.WriteLine ("Serializing available downloads.");
			using(Stream stream = File.Open(DownloadInfoFileLocation, FileMode.Create))
			{
				var formatter = new BinaryFormatter();

				formatter.Serialize(stream, AvailableDownloads);
			}
		}

		/// <summary>
		/// Helper to find a DownloadInfo object via its task identifier.
		/// </summary>
		/// <returns>The download info by task identifier.</returns>
		/// <param name="taskId">Task identifier.</param>
		public static DownloadInfo GetDownloadInfoByTaskId(nuint taskId)
		{
			return AvailableDownloads.FirstOrDefault (x => x.TaskId == (nint)taskId);
		}

		/// <summary>
		/// Helper to find a DownloadInfo object via its task identifier.
		/// </summary>
		/// <returns>The download info by task identifier.</returns>
		/// <param name="taskId">Task identifier.</param>
		public static int GetDownloadInfoIndexByTaskId(nuint taskId)
		{
			return AvailableDownloads.FindIndex (x => x.TaskId == (nint)taskId);
		}

		public override UIWindow Window
		{
			get;
			set;
		}

		public override void DidEnterBackground (UIApplication application)
		{
			// App is getting terminated. Store all download info to disk so we can resume later.
			SerializeAvailableDownloads ();
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			Console.WriteLine ("FinishedLaunching() - with options");

			// For iOS8 we must get permission to show local notifications.
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0))
			{
				var settings = UIUserNotificationSettings.GetSettingsForTypes (UIUserNotificationType.Alert, new NSSet ());
				if (UIApplication.SharedApplication.CurrentUserNotificationSettings != settings)
				{
					UIApplication.SharedApplication.RegisterUserNotificationSettings (settings);
				}
			}

			// Try to deserialize existing download information. If there isn't any, create a new set.
			if (File.Exists (DownloadInfoFileLocation))
			{
				Console.WriteLine ("Deserializing existing download data.");
				try
				{
					using (var stream = File.Open (DownloadInfoFileLocation, FileMode.Open))
					{
						var formatter = new BinaryFormatter ();
						AppDelegate.AvailableDownloads = (List<DownloadInfo>)formatter.Deserialize (stream);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine ("Failed to deserialize download data: " + ex);
				}
			}
			else
			{
				Console.WriteLine ("No serialized download info found. Creating new.");
				ResetAvailableDownloads ();
			}

			return true;
		}

		/// <summary>
		/// We have to call this if our transfer (of all files!) is done.
		/// </summary>
		public static Action BackgroundSessionCompletionHandler
		{
			get;
			set;
		}


		/// <summary>
		/// Gets called by iOS if we are required to handle something regarding our background downloads.
		/// </summary>
		public override void HandleEventsForBackgroundUrl (UIApplication application, string sessionIdentifier, Action completionHandler)
		{
			Console.WriteLine ("HandleEventsForBackgroundUrl(): " + sessionIdentifier);
			// We get a completion handler which we are supposed to call if our transfer is done.
			BackgroundSessionCompletionHandler = completionHandler;
		}
 	}
}