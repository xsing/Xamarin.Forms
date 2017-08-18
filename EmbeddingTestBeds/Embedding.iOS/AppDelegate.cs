using System;
using CoreGraphics;
using Embedding.XF;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace Embedding.iOS
{
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		public static AppDelegate Shared;
		public static UIStoryboard Storyboard = UIStoryboard.FromName("Main", null);

		UIViewController _hello;
		UIViewController _alertsAndActionSheets;

		UIBarButtonItem _helloButton;
		UIBarButtonItem _alertsAndActionSheetsButton;

		UIWindow _window;
		UINavigationController _navigation;
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

		//public UIBarButtonItem CreateAlertsAndActionSheetsButton()
		//{
		//	if (_alertsAndActionSheetsButton == null)
		//	{
		//		var btn = new UIButton(new CGRect(0, 0, 88, 44));
		//		btn.SetTitle("AaA", UIControlState.Normal);
		//		btn.SetTitleColor(UIColor.Blue, UIControlState.Normal);
		//		btn.TouchUpInside += (sender, e) => ShowAlertsAndActionSheets();

		//		_alertsAndActionSheetsButton  = new UIBarButtonItem(btn);
		//	}

		//	return _alertsAndActionSheetsButton;
		//}

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

		public void ShowAlertsAndActionSheets()
		{
			// Create a XF History page as a view controller
			if (_alertsAndActionSheets == null)
			{
				_alertsAndActionSheets = new AlertsAndActionSheets().CreateViewController();
			}

			// And push it onto the navigation stack
			_navigation.PushViewController(_alertsAndActionSheets, true);
		}
	}
}