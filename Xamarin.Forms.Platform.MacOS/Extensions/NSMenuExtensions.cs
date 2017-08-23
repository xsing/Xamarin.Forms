using System;
using AppKit;

namespace Xamarin.Forms.Platform.macOS.Extensions
{
	internal static class NSMenuExtensions
	{
		public static NSMenu ToNSMenu(this Menu menus, NSMenu nsMenu = null)
		{
			if (nsMenu == null)
				nsMenu = new NSMenu(menus.Text ?? "");
			foreach (var menu in menus)
			{
				var menuItem = new NSMenuItem(menu.Text ?? "");
				var subMenu = new NSMenu(menu.Text ?? "");
				menuItem.Submenu = subMenu;
				foreach (var item in menu.Items)
				{
					var subMenuItem = item.ToNSMenuItem();
					subMenu.AddItem(subMenuItem);
					item.PropertyChanged += (sender, e) => (sender as MenuItem)?.UpdateNSMenuItem(subMenuItem, new string[] { e.PropertyName });
				}
				nsMenu.AddItem(menuItem);
				menu.ToNSMenu(subMenu);
			}
			return nsMenu;
		}

		public static NSMenuItem ToNSMenuItem(this MenuItem menuItem, int i = -1)
		{
			var nsMenuItem = new NSMenuItem(menuItem.Text ?? "");
			if (i != -1)
				nsMenuItem.Tag = i;
			nsMenuItem.Enabled = menuItem.IsEnabled;
			nsMenuItem.Activated += (sender, e) => menuItem.Activate();
			if (!string.IsNullOrEmpty(menuItem.Icon))
				nsMenuItem.Image = new NSImage(menuItem.Icon);

			return nsMenuItem;
		}

		public static void UpdateNSMenuItem(this MenuItem item, NSMenuItem menuItem, string[] properties)
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
