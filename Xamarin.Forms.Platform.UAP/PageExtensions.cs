using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Xamarin.Forms.Platform.UWP
{
	public partial class XFEmbeddedPage : Windows.UI.Xaml.Controls.Page
	{
		// TODO hartez 2017/08/16 14:24:21 Think about whether this should be concurrent	
		internal static Dictionary<Guid, Xamarin.Forms.Page> Pages = new Dictionary<Guid, Page>();

		public XFEmbeddedPage()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			var key = (Guid)e.Parameter;

			// TODO hartez 2017/08/16 14:23:17 Handle page not found	
			var page = Pages[key];
			Pages.Remove(key); // Otherwise the dictionary will hold on to this ref forever
			var frameworkElement = page.CreateFrameworkElement();

			if (frameworkElement == null)
			{
				throw new InvalidOperationException($"Could not find or create a renderer for the Page {page}");
			}

			Root.Content = frameworkElement;
		}
	}

	public static class PageExtensions
	{
		public static FrameworkElement CreateFrameworkElement(this VisualElement visualElement)
		{
			if (!Forms.IsInitialized)
				throw new InvalidOperationException("call Forms.Init() before this");

			var root = new Windows.UI.Xaml.Controls.Page();

			new WindowsPlatform(root).SetPlatformDisconnected(visualElement);

			var frameworkElement = visualElement.GetOrCreateRenderer() as FrameworkElement;

			if (frameworkElement == null)
			{
				throw new InvalidOperationException($"Could not find or create a renderer for the VisualElement {visualElement}");
			}

			frameworkElement.Loaded += (sender, args) =>
			{
				visualElement.Layout(new Rectangle(0, 0, frameworkElement.ActualWidth, frameworkElement.ActualHeight));
			};

			return frameworkElement;
		}

		public static bool Navigate(this Windows.UI.Xaml.Controls.Frame frame, Xamarin.Forms.Page page)
		{
			var id = Guid.NewGuid();
			XFEmbeddedPage.Pages.Add(id, page);
			return frame.Navigate(typeof(XFEmbeddedPage), id);
		}
	}
}