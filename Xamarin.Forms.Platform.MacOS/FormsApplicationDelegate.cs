using System;
using System.ComponentModel;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	public abstract class FormsApplicationDelegate : NSApplicationDelegate
	{
		Application _application;
		bool _isSuspended;

		public abstract NSWindow MainWindow { get; }

		protected override void Dispose(bool disposing)
		{
			if (disposing && _application != null)
				_application.PropertyChanged -= ApplicationOnPropertyChanged;

			base.Dispose(disposing);
		}

		protected void LoadApplication(Application application)
		{
			if (application == null)
				throw new ArgumentNullException(nameof(application));

			Application.SetCurrentApplication(application);
			_application = application;

			application.PropertyChanged += ApplicationOnPropertyChanged;
		}

		public override void DidFinishLaunching(Foundation.NSNotification notification)
		{
			if (MainWindow == null)
				throw new InvalidOperationException("Please provide a main window in your app");

			MainWindow.Display();
			MainWindow.MakeKeyAndOrderFront(NSApplication.SharedApplication);
			if (_application == null)
				throw new InvalidOperationException("You MUST invoke LoadApplication () before calling base.FinishedLaunching ()");

			SetMainPage();

			SetMainMenu();

			_application.SendStart();
		}

		public override void DidBecomeActive(Foundation.NSNotification notification)
		{
			// applicationDidBecomeActive
			// execute any OpenGL ES drawing calls
			if (_application == null || !_isSuspended) return;
			_isSuspended = false;
			_application.SendResume();
		}

		public override async void DidResignActive(Foundation.NSNotification notification)
		{
			// applicationWillResignActive
			if (_application == null) return;
			_isSuspended = true;
			await _application.SendSleepAsync();
		}

		void ApplicationOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Application.MainPage))
				UpdateMainPage();
		}

		void SetMainPage()
		{
			UpdateMainPage();
		}

		void UpdateMainPage()
		{
			if (_application.MainPage == null)
				return;

			var platformRenderer = (PlatformRenderer)MainWindow.ContentViewController;
			MainWindow.ContentViewController = _application.MainPage.CreateViewController();
			(platformRenderer?.Platform as IDisposable)?.Dispose();
		}

		void SetMainMenu()
		{
			_application.MainMenu.PropertyChanged += MainMenuOnPropertyChanged;
			MainMenuOnPropertyChanged(this, null);
		}

		void MainMenuOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			//for now we can't remove the 1st menu item
			for (var i = NSApplication.SharedApplication.MainMenu.Count - 1; i > 0; i--)
				NSApplication.SharedApplication.MainMenu.RemoveItemAt(i);
			AddMenu(_application.MainMenu, NSApplication.SharedApplication.MainMenu);
		}

		void AddMenu(Menu menus, NSMenu nsMenu)
		{
			foreach (var menu in menus)
			{
				var menuItem = new NSMenuItem(menu.Text);
				var subMenu = new NSMenu(menu.Text);
				menuItem.Submenu = subMenu;
				foreach (var item in menu.Items)
				{
					var subMenuItem = new NSMenuItem(item.Text, (sender, e) => item.Activate());
					UpdateMenuItem(item, subMenuItem, new string[] { nameof(MenuItem.IsEnabled), nameof(MenuItem.IsEnabled) });
					subMenu.AddItem(subMenuItem);
					item.PropertyChanged += (sender, e) => UpdateMenuItem((sender as MenuItem), subMenuItem, new string[] { e.PropertyName });
				}
				AddMenu(menu, subMenu);
				nsMenu.AddItem(menuItem);
			}
		}

		void UpdateMenuItem(MenuItem item, NSMenuItem menuItem, string[] properties)
		{
			foreach (var property in properties)
			{
				if (property.Equals(nameof(MenuItem.Text)))
				{
					menuItem.Title = item.Text;
				}
				if (property.Equals(nameof(MenuItem.IsEnabled)))
				{
					menuItem.Enabled = item.IsEnabled;
				}
				if (property.Equals(nameof(MenuItem.Icon)))
				{
					if (!string.IsNullOrEmpty(item.Icon))
						menuItem.Image = new NSImage(item.Icon);
					else
						menuItem.Image = null;
				}
			}
		}
	}
}