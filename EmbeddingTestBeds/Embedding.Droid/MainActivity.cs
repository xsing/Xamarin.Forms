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
			// Create a XF History page as a fragment
			if (_hello == null)
			{
				_hello = new Hello().CreateSupportFragment(this);
			}

			// And push that fragment onto the stack
			FragmentTransaction ft = SupportFragmentManager.BeginTransaction();

			ft.AddToBackStack(null);
			ft.Replace(Resource.Id.fragment_frame_layout, _hello, "hello");
			
			ft.Commit();
		}
	}

	public class MainFragment : Fragment
	{
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view =  inflater.Inflate(Resource.Layout.MainFragment, container, false);
			Button button = view.FindViewById<Button>(Resource.Id.showEmbeddedButton);

			button.Click += Button_Click;

			// Listen for lookup requests from the history tracker
			//MessagingCenter.Subscribe<History, string>(this, History.HistoryItemSelected, (history, postalCode) =>
			//{
			//	Activity.FragmentManager.PopBackStack();
			//	_lastPostalCode = postalCode;
			//});

			return view;
		}

		void Button_Click(object sender, EventArgs e)
		{
			((MainActivity)Activity).ShowHello();
		}
	}
}

