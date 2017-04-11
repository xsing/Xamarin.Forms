
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	internal class PerformanceScenario
	{
		public View View { get; set; }
		public string Name { get; private set; }
		public double ExpectedRenderTime { get; private set; }

		public PerformanceScenario() { }
		public PerformanceScenario(string name, double renderTime)
		{
			Name = name;
			ExpectedRenderTime = renderTime;
		}
	}
}
