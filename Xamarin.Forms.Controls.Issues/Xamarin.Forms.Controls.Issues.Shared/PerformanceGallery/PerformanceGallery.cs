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
		const double Threshold = 0.25;

		double TopThreshold => 1 + Threshold;
		double BottomThreshold = 1 - Threshold;

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
			await Task.Delay((int)Math.Round(ViewModel.ExpectedRenderTime * TopThreshold * 1.5));
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

			// perf should be within threshold
			if (Math.Abs(ViewModel.ActualRenderTime - ViewModel.ExpectedRenderTime) > ViewModel.ExpectedRenderTime * Threshold)
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

			List<string> warnings = new List<string>();

			for (int i = 0; i < testCasesCount; i++)
			{
				RunningApp.WaitForElement(q => q.Marked(Next));
				RunningApp.Tap(q => q.Marked(Next));

				try
				{
					RunningApp.WaitForElement(q => q.Marked(Success));
				}
				catch (Exception)
				{
					var message = GetFailureMessage();
					if (!warnings.Contains(message))
						warnings.Add(message);
				}
			}

			if (warnings.Any())
				Assert.Inconclusive($"Performance threshold exceeded.\r\n{string.Join("\r\n", warnings)}");
		}

		string GetFailureMessage()
		{
			double expected = 0;
			double.TryParse(GetText(PerformanceTrackerTemplate.ExpectedId), out expected);

			var scenario = GetText(PerformanceTrackerTemplate.ScenarioId);
			var actual = GetText(PerformanceTrackerTemplate.ActualId);

			return $" - Scenario \"{scenario}\" failed. Expected {expected * BottomThreshold}-{expected * TopThreshold}ms, Actual {actual}ms.";
		}

		string GetText(string id)
		{
			return RunningApp.Query(q => q.Marked(id))[0].Text;
		}
#endif
	}
}