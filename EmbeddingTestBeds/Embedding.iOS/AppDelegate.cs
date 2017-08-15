using CoreGraphics;
using Embedding.XF;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace Embedding.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		public static AppDelegate Shared;
		public static UIStoryboard Storyboard = UIStoryboard.FromName("Main", null);

		UIWindow _window;
		UIViewController _history;
		UINavigationController _navigation;
		UIBarButtonItem _aboutButton;
		ViewController _weatherController;

		public override UIWindow Window
		{
			get;
			set;
		}

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			Forms.Init();

			Shared = this;
			_window = new UIWindow(UIScreen.MainScreen.Bounds);

			UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes
			{
				TextColor = UIColor.White
			});

			_weatherController = Storyboard.InstantiateInitialViewController() as ViewController;
			_navigation = new UINavigationController(_weatherController);

			// Listen for lookup requests from the history tracker
			//MessagingCenter.Subscribe<History, string>(this, History.HistoryItemSelected, (history, postalCode) =>
			//{
			//	_navigation.PopToRootViewController(true);
			//	_weatherController.SetPostalCode(postalCode);
			//});

			_window.RootViewController = _navigation;
			_window.MakeKeyAndVisible();

			return true;
		}

		public UIBarButtonItem CreateAboutButton()
		{
			if (_aboutButton == null)
			{
				var btn = new UIButton(new CGRect(0, 0, 88, 44));
				btn.SetTitle("History", UIControlState.Normal);
				btn.TouchUpInside += (sender, e) => ShowAbout();

				_aboutButton = new UIBarButtonItem(btn);
			}
			return _aboutButton;
		}

		public void ShowAbout()
		{
			// Create a XF History page as a view controller
			if (_history == null)
			{
				_history = new Hello().CreateViewController();
			}

			// And push it onto the navigation stack
			_navigation.PushViewController(_history, true);
		}

	}
}

