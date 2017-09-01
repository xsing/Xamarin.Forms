using Android.Views;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.Android;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.Android;
using static Android.Views.ViewTreeObserver;

[assembly: ExportRenderer(typeof(PerformanceTracker), typeof(PerformanceTrackerRenderer))]

namespace Xamarin.Forms.ControlGallery.Android
{
	public class PerformanceTrackerRenderer : ViewRenderer, IOnDrawListener
	{
		const int Default_Timeout = 250;
		int _timeout = Default_Timeout;
		DateTime _lastCall;
		bool _sagaComplete;
		Stopwatch _timer = new Stopwatch();

		public PerformanceTrackerRenderer()
		{
			ViewTreeObserver.AddOnDrawListener(this);
		}

		PerformanceTracker PerformanceTracker => Element as PerformanceTracker;

		void IOnDrawListener.OnDraw()
		{
			if (!_timer.IsRunning)
				_timer.Start();

			if (!_sagaComplete)
			{
				_lastCall = DateTime.Now;
				WaitForComplete();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == PerformanceTracker.ContentProperty.PropertyName)
			{
				ResetTest();

				SubscribeChildrenToDraw(this, this);
			}
		}

		static void SubscribeChildrenToDraw(ViewGroup viewGroup, IOnDrawListener observer)
		{
			if (viewGroup == null)
				return;

			for (int i = 0; i < viewGroup.ChildCount; i++)
			{
				var child = viewGroup.GetChildAt(i);
				child.ViewTreeObserver.AddOnDrawListener(observer);

				SubscribeChildrenToDraw(child as ViewGroup, observer);
			}
		}

		static void UnsubscribeChildrenToDraw(ViewGroup viewGroup, IOnDrawListener observer)
		{
			if (viewGroup == null)
				return;

			for (int i = 0; i < viewGroup.ChildCount; i++)
			{
				var child = viewGroup.GetChildAt(i);
				child.ViewTreeObserver.RemoveOnDrawListener(observer);

				UnsubscribeChildrenToDraw(child as ViewGroup, observer);
			}
		}

		void ResetTest()
		{
			_sagaComplete = false;
			_timer.Stop();
			_timer.Reset();
			UnsubscribeChildrenToDraw(this, this);

			int newTimeout = (int)Math.Round(PerformanceTracker.ExpectedRenderTime * 3);
			if (newTimeout > Default_Timeout)
				_timeout = newTimeout;
			else
				_timeout = Default_Timeout;
		}

		async void WaitForComplete()
		{
			await Task.Delay(_timeout);

			if (_sagaComplete)
				return;

			// triggered lastCall: 12/12/2012 12:12:12:000
			// triggered lastCall: 12/12/2012 12:12:12:010
			// timeout 12/12/2012 12:12:12:250 = 12/12/2012 12:12:12:250 : defer timeout
			// timeout 12/12/2012 12:12:12:260 = 12/12/2012 12:12:12:260 : defer timeout
			// timeout 12/12/2012 12:12:12:260 < 12/12/2012 12:12:12:500 : send message
			// timeout 12/12/2012 12:12:12:260 < 12/12/2012 12:12:12:510 : exit

			if (_lastCall.AddMilliseconds(_timeout) >= DateTime.Now)
				WaitForComplete();

			_sagaComplete = true;

			_timer.Stop();

			PerformanceTracker.TotalMilliseconds = _timer.ElapsedMilliseconds - _timeout;

			UnsubscribeChildrenToDraw(this, this);

			MessagingCenter.Send<PerformanceTracker>(PerformanceTracker, PerformanceTracker.RenderCompleteMessage);
		}
	}
}