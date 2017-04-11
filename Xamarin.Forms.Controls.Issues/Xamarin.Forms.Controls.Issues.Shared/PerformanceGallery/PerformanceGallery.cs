using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms.Internals;
using Xamarin.Forms.CustomAttributes;
using System.Threading.Tasks;
using System.Diagnostics;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 0, "Performance Testing")]
	public class PerformanceGallery : TestContentPage
	{
		const string Success = "SUCCESS";
		const string Fail = "FAIL";
		const string Next = "Next Scenario";
		const string Pending = "PENDING";

		PerformanceTracker _PerformanceTracker = new PerformanceTracker();
		int _TestNumber = 0;
		List<PerformanceScenario> _TestCases = new List<PerformanceScenario>();
		PerformanceProvider _PerformanceProvider = new PerformanceProvider();
		PerformanceViewModel ViewModel => BindingContext as PerformanceViewModel;

		protected override void Init()
		{
			_PerformanceTracker.LayoutChanged += PerformanceTracker_LayoutChanged;

			BindingContext = new PerformanceViewModel(_PerformanceProvider);
			Performance.SetProvider(_PerformanceProvider);

			_TestCases.AddRange(InflatePerformanceScenarios());

			var nextButton = new Button { Text = Next };
			nextButton.Clicked += NextButton_Clicked;

			Content = new StackLayout { Children = { nextButton, _PerformanceTracker } };
		}

		async void NextButton_Clicked(object sender, EventArgs e)
		{
			if (_TestCases?.Count == 0 || _TestNumber + 1 > _TestCases?.Count)
				return;

			ViewModel.View = null;
			ViewModel.ActualRenderTime = 0;
			ViewModel.Outcome = Pending;
			ViewModel.RunTest(_TestCases[_TestNumber++]);

			// arbitrary delay to wait for the view to render
			await Task.Delay((int)Math.Round(ViewModel.ExpectedRenderTime * 1.5));
			await DisplayResults();
		}

		async void PerformanceTracker_LayoutChanged(object sender, EventArgs e)
		{
			// catch any additional work that may have happened after our arbitrary delay
			// we can't rely on this firing if we're running tests on the same type of view. 
			// XF is smart enough to reduce, recycle, reuse.
			await DisplayResults();
		}

		async Task DisplayResults()
		{
			// attempt to wait for async rendering to finish
			await Task.Yield();

			var stats = _PerformanceProvider.Statistics.Where(stat => !stat.Value.IsDetail);
			ViewModel.ActualRenderTime = stats.Select(q => q.Value).Sum(q => TimeSpan.FromTicks(q.TotalTime).TotalMilliseconds);

			// perf should be within 10%
			if (Math.Abs(ViewModel.ActualRenderTime - ViewModel.ExpectedRenderTime) > ViewModel.ExpectedRenderTime * 0.10)
				ViewModel.Outcome = Fail;
			else
				ViewModel.Outcome = Success;

			_PerformanceProvider.DumpStats();
		}

		static IEnumerable<Type> FindPerformanceScenarios()
		{
			return typeof(PerformanceGallery).GetTypeInfo().Assembly.DefinedTypes.Select(o => o.AsType())
													.Where(typeInfo => typeof(PerformanceScenario).IsAssignableFrom(typeInfo));
		}

		static IEnumerable<PerformanceScenario> InflatePerformanceScenarios()
		{
			return FindPerformanceScenarios()
										.Select(o => (PerformanceScenario)Activator.CreateInstance(o))
										.Where(scenario => scenario.View != null);
		}

		~PerformanceGallery()
		{
			Performance.SetProvider(null);
		}

#if UITEST
		[Test]
		public void PerformanceTest()
		{
			var testCasesCount = FindPerformanceScenarios().Count();

			for (int i = 0; i < testCasesCount; i++)
			{
				RunningApp.WaitForElement(q => q.Marked(Next));
				RunningApp.Tap(q => q.Marked(Next));
				RunningApp.WaitForElement(q => q.Marked(Success));
			}
		}
#endif
	}
}