using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android
{
	public static class PageExtensions
	{
		public static Fragment CreateFragment(this ContentPage view, Context context)
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

		public static global::Android.Support.V4.App.Fragment CreateSupportFragment(this ContentPage view, Context context)
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

			// TODO hartez 2017/08/31 17:47:58 This and EmbeddedFragment probably need to keep Platform refs and override Dispose to dispose of them	
			return new EmbeddedSupportFragment(vg);
		}

		class DefaultApplication : Application
		{
		}

		class EmbeddedFragment : Fragment
		{
			readonly ViewGroup _content;

			public EmbeddedFragment()
			{
			}

			public EmbeddedFragment(ViewGroup content)
			{
				_content = content;
			}

			public override global::Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				return _content;
			}
		}

		class EmbeddedSupportFragment : global::Android.Support.V4.App.Fragment
		{
			readonly ViewGroup _content;

			public EmbeddedSupportFragment()
			{
			}

			public EmbeddedSupportFragment(ViewGroup content)
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