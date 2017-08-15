using System;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public static class PageExtensions
	{
		public static UIViewController CreateViewController(this Page page)
		{
			if (!Forms.IsInitialized)
				throw new InvalidOperationException("call Forms.Init() before this");

			if (!(page.RealParent is Application))
			{
				Application app = new DefaultApplication();
				app.MainPage = page;
			}

			var result = new Platform();
			result.SetPage(page);
			return result.ViewController;
		}

		class DefaultApplication : Application
		{
		}
	}
}