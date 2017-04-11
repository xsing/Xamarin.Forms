using System.ComponentModel;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	internal class PerformanceViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		readonly PerformanceProvider _Provider;

		public PerformanceViewModel(PerformanceProvider provider)
		{
			_Provider = provider;
		}

		double _ActualRenderTime;
		public double ActualRenderTime
		{
			get
			{
				return _ActualRenderTime;
			}
			set
			{
				if (value == _ActualRenderTime)
					return;

				_ActualRenderTime = value;

				PropertyChanged(this, new PropertyChangedEventArgs(nameof(ActualRenderTime)));
			}
		}

		double _ExpectedRenderTime;
		public double ExpectedRenderTime
		{
			get
			{
				return _ExpectedRenderTime;
			}
			set
			{
				if (value == _ExpectedRenderTime)
					return;

				_ExpectedRenderTime = value;

				PropertyChanged(this, new PropertyChangedEventArgs(nameof(ExpectedRenderTime)));
			}
		}

		string _Outcome;
		public string Outcome
		{
			get
			{
				return _Outcome;
			}
			set
			{
				if (value == _Outcome)
					return;

				_Outcome = value;

				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Outcome)));
			}
		}

		string _Scenario;
		public string Scenario
		{
			get
			{
				return _Scenario;
			}
			set
			{
				if (value == _Scenario)
					return;

				_Scenario = value;

				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Scenario)));
			}
		}

		View _View;
		public View View
		{
			get
			{
				return _View;
			}
			set
			{
				if (value == _View)
					return;

				_View = value;

				PropertyChanged(this, new PropertyChangedEventArgs(nameof(View)));
			}
		}

		public void RunTest(PerformanceScenario scenario)
		{
			Scenario = scenario.Name;
			ExpectedRenderTime = scenario.ExpectedRenderTime;

			_Provider.Clear();
			View = scenario.View;
		}
	}
}
