using System;
using Windows.UI.Xaml;

namespace Xamarin.Forms.Platform.UWP
{
	public static class PageExtensions
	{
		public static FrameworkElement CreateFrameworkElement(this VisualElement view)
		{
			if (!Forms.IsInitialized)
				throw new InvalidOperationException("call Forms.Init() before this");

			var root = new Windows.UI.Xaml.Controls.Page();

			new WindowsPlatform(root).SetPlatformDisconnected(view);

			var frameworkElement = view.GetOrCreateRenderer() as FrameworkElement;

			if (frameworkElement == null)
			{
				throw new InvalidOperationException($"Could not find or create a renderer for the VisualElement {view}");
			}

			frameworkElement.Loaded += (sender, args) =>
			{
				view.Layout(new Rectangle(0, 0, frameworkElement.ActualWidth, frameworkElement.ActualHeight));
			};

			return frameworkElement;
		}
	}
}