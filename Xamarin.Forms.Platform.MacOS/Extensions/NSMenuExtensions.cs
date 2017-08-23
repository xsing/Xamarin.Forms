using System;
using AppKit;

namespace Xamarin.Forms.Platform.macOS.Extensions
{
	public static class NSMenuExtensions
	{
		public static NSMenuItem ToNSMenuItem(this MenuItem menuItem, int i = -1)
		{
			var nsMenuItem = new NSMenuItem(menuItem.Text ?? "");
			nsMenuItem.Tag = i;
			nsMenuItem.Enabled = menuItem.IsEnabled;
			if (nsMenuItem.Enabled)
				nsMenuItem.Activated += (sender, e) =>
				{
					menuItem.Activate();
				};
			if (!string.IsNullOrEmpty(menuItem.Icon))
				nsMenuItem.Image = new NSImage(menuItem.Icon);

			return nsMenuItem;
		}
	}
}
