using UIKit;

namespace ExtendedMusicPlayer
{
	/// <summary>
	/// Delegate for file previews.
	/// </summary>
	public class PreviewDelegate : UIDocumentInteractionControllerDelegate
	{
		public PreviewDelegate (UIViewController controller)
		{
			this.controller = controller;	
		}

		UIViewController controller;

		public override UIViewController ViewControllerForPreview (UIDocumentInteractionController controller)
		{
			return this.controller;
		}
	}

}
