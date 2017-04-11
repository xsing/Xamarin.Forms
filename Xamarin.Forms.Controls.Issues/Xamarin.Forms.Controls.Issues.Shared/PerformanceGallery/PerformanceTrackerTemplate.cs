using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	internal class PerformanceTrackerTemplate : StackLayout
	{
		public PerformanceTrackerTemplate()
		{
			var scenarioLabel = new Label
			{
				BackgroundColor = Color.Blue,
				TextColor = Color.White,
				HorizontalTextAlignment = TextAlignment.Center,
				HeightRequest = 25
			};
			scenarioLabel.SetBinding(Label.TextProperty, new TemplateBinding(nameof(PerformanceTracker.Scenario)));
			Children.Add(scenarioLabel);

			var renderTimeLabel = new Label
			{
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				HorizontalTextAlignment = TextAlignment.Center,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 25
			};
			renderTimeLabel.SetBinding(Label.TextProperty, new TemplateBinding(nameof(PerformanceTracker.RenderTime)));

			var expectedLabel = new Label
			{
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				HorizontalTextAlignment = TextAlignment.Center,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 25
			};
			expectedLabel.SetBinding(Label.TextProperty, new TemplateBinding(nameof(PerformanceTracker.ExpectedRenderTime)));

			var outcomeLabel = new Label
			{
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				HorizontalTextAlignment = TextAlignment.Center,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 25
			};
			outcomeLabel.SetBinding(Label.TextProperty, new TemplateBinding(nameof(PerformanceTracker.Outcome)));

			var horStack = new StackLayout { Orientation = StackOrientation.Horizontal };
			horStack.Children.Add(renderTimeLabel);
			horStack.Children.Add(expectedLabel);
			horStack.Children.Add(outcomeLabel);

			Children.Add(horStack);

			Children.Add(new ContentPresenter());
		}
	}
}
