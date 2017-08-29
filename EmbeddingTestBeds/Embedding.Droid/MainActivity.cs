using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Embedding.XF;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using View = Android.Views.View;
using Button = Android.Widget.Button;

namespace Embedding.Droid
{
	[Activity(Label = "Embedding.Droid", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : FragmentActivity
	{
		Fragment _hello;
		Fragment _alertsAndActionSheets;
		Fragment _webview;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			Forms.Init(this, null);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			
			var ft = SupportFragmentManager.BeginTransaction();
			ft.Replace(Resource.Id.fragment_frame_layout, new MainFragment(), "main");
			ft.Commit();
		}

		public void ShowHello()
		{
			// Create a XF Hello page as a fragment
			if (_hello == null)
			{
				_hello = new Hello().CreateSupportFragment(this);
			}

			ShowEmbeddedPageFragment(_hello);
		}

		public void ShowWebView()
		{
			// Create a XF Hello page as a fragment
			if (_webview == null)
			{
				_webview= new WebViewExample().CreateSupportFragment(this);
			}

			ShowEmbeddedPageFragment(_webview);
		}

		public void ShowAlertsAndActionSheets()
		{
			if (_alertsAndActionSheets== null)
			{
				_alertsAndActionSheets = new AlertsAndActionSheets().CreateSupportFragment(this);
			}

			ShowEmbeddedPageFragment(_alertsAndActionSheets);
		}

		void ShowEmbeddedPageFragment(Fragment fragment)
		{
			FragmentTransaction ft = SupportFragmentManager.BeginTransaction();

			ft.AddToBackStack(null);
			ft.Replace(Resource.Id.fragment_frame_layout, fragment, "hello");
			
			ft.Commit();
		}
	}

	public class MainFragment : Fragment
	{
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view =  inflater.Inflate(Resource.Layout.MainFragment, container, false);
			var showEmbeddedButton = view.FindViewById<Button>(Resource.Id.showEmbeddedButton);
			var showAlertsActionSheets = view.FindViewById<Button>(Resource.Id.showAlertsActionSheets);
			var showWebView = view.FindViewById<Button>(Resource.Id.showWebView);

			showEmbeddedButton.Click += ShowEmbeddedClick;
			showAlertsActionSheets.Click += ShowAlertsActionSheetsClick;
			showWebView.Click += ShowWebViewOnClick;

			return view;
		}

		void ShowWebViewOnClick(object sender, EventArgs eventArgs)
		{
			((MainActivity)Activity).ShowWebView();
		}

		void ShowAlertsActionSheetsClick(object sender, EventArgs eventArgs)
		{
			((MainActivity)Activity).ShowAlertsAndActionSheets();
		}

		void ShowEmbeddedClick(object sender, EventArgs e)
		{
			((MainActivity)Activity).ShowHello();
		}
	}
}

