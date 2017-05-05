using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;

namespace Xamarin.Forms.Platform.Android
{
	public static class PageExtensions
	{
		public static Fragment CreateFragment(this Page view, Context context)
		{
			if (!Forms.IsInitialized)
				throw new InvalidOperationException("call Forms.Init() before this");

			if (!(view.RealParent is Application))
			{
				Application app = new DefaultApplication();
				app.MainPage = view;
			}

			var result = new Platform(context, true);
			result.SetPage(view);

			var vg = result.GetViewGroup();

			return new EmbeddedFragment(vg);
		}

		class DefaultApplication : Application
		{
		}

		class EmbeddedFragment : Fragment
		{
			readonly ViewGroup _content;

			public EmbeddedFragment(ViewGroup content)
			{
				_content = content;
			}

			public override global::Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				return _content;
			}
		}
	}
}