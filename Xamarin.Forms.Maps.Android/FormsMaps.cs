using System;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Maps;
using Android.OS;
using Xamarin.Forms.Maps.Android;

namespace Xamarin
{
	public static class FormsMaps
	{
		public static bool IsInitialized { get; private set; }

		// TODO hartez 2017/08/30 21:50:46 See if we can obsolete this	
		public static Context Context { get; private set; }

		public static void Init(Activity activity, Bundle bundle)
		{
			if (IsInitialized)
				return;
			IsInitialized = true;

			Context = activity;

			MapRenderer.Bundle = bundle;

#pragma warning disable 618
			if (GooglePlayServicesUtil.IsGooglePlayServicesAvailable(activity) == ConnectionResult.Success)
#pragma warning restore 618
			{
				try
				{
					MapsInitializer.Initialize(activity);
				}
				catch (Exception e)
				{
					Console.WriteLine("Google Play Services Not Found");
					Console.WriteLine("Exception: {0}", e);
				}
			}

			// TODO hartez 2017/08/30 21:49:00 Need to make sure this gets re-initialized if Context changes	
			new GeocoderBackend(activity).Register();
		}
	}
}