using CoreGraphics;
using Embedding.XF;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

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
		UIViewController _hello;
		UINavigationController _navigation;
		UIBarButtonItem _helloButton;
		ViewController _mainController;

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{ 
			Forms.Init();

			Shared = this;
			_window = new UIWindow(UIScreen.MainScreen.Bounds);

			UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes
			{
				TextColor = UIColor.White
			});

			_mainController = Storyboard.InstantiateInitialViewController() as ViewController;
			_navigation = new UINavigationController(_mainController);

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

		public UIBarButtonItem CreateHelloButton()
		{
			if (_helloButton == null)
			{
				var btn = new UIButton(new CGRect(0, 0, 88, 44));
				btn.SetTitle("Hello", UIControlState.Normal);
				btn.SetTitleColor(UIColor.Blue, UIControlState.Normal);
				btn.TouchUpInside += (sender, e) => ShowHello();

				_helloButton = new UIBarButtonItem(btn);
			}

			return _helloButton;
		}

		public void ShowHello()
		{
			// Create a XF History page as a view controller
			if (_hello == null)
			{
				_hello = new Hello().CreateViewController();
			}

			// And push it onto the navigation stack
			_navigation.PushViewController(_hello, true);
		}

	}
}

