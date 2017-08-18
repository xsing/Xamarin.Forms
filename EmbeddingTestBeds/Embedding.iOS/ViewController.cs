using System;

using UIKit;

namespace Embedding.iOS
{
	public partial class ViewController : UIViewController
	{
		public ViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			NavigationItem.RightBarButtonItem = AppDelegate.Shared.CreateHelloButton();
			ShowAlertsAndActionSheets.TouchUpInside += (sender, args) => AppDelegate.Shared.ShowAlertsAndActionSheets();
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}