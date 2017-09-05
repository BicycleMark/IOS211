using System;
using Foundation;
using UIKit;
using System.Runtime.InteropServices;

namespace BackgroundDownload
{
	public class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window
		{
			get;
			set;
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			Console.WriteLine ("FinishedLaunching()");

			// For iOS8 we must get permission to show local notifications.
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0))
			{
				var settings = UIUserNotificationSettings.GetSettingsForTypes (UIUserNotificationType.Alert, new NSSet ());
				if (UIApplication.SharedApplication.CurrentUserNotificationSettings != settings)
				{
					UIApplication.SharedApplication.RegisterUserNotificationSettings (settings);
				}
			}

			return true;
		}

		/// <summary>
		/// We have to call this if our transfer (of all files!) is done.
		/// </summary>
		public static Action BackgroundSessionCompletionHandler;

		// TODO: Override HandleEventsForBackgroundUrl() to retain the download completion handler if iOS restarts the app.

		/// <summary>
		/// Import private API to allow exiting app manually.
		/// For demo purposes only! Do not use this in productive apps!
		/// </summary>
		/// <param name="status">Status.</param>
		[DllImport("__Internal", EntryPoint = "exit")]
		public static extern void Exit(int status);
 	}
}