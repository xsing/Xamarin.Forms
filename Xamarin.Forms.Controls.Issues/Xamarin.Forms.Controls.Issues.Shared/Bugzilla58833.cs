using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.Generic;
using System.Diagnostics;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 58833, "ListView SelectedItem Binding does not fire", PlatformAffected.Android)]
	public class Bugzilla58833 : TestContentPage
	{
		const string Success = "ItemSelected Success";
		Label label;

		[Preserve(AllMembers = true)]
		class TestCell : ViewCell
		{
			Label label;

			static int s_index;

			public TestCell()
			{
				label = new Label();
				//label.GestureRecognizers.Add(new TapGestureRecognizer
				//{
				//	Command = new Command(() =>
				//	{
				//		Debug.WriteLine($">>>>> TapGesture Fired");
				//	})
				//});
				View = label;
				ContextActions.Add(new MenuItem { Text = s_index++ + " Action" });
			}

			protected override void OnBindingContextChanged()
			{
				base.OnBindingContextChanged();
				label.Text = (string)BindingContext;
			}
		}

		protected override void Init()
		{
			label = new Label();
			var items = new List<string>();
			for (int i = 0; i < 5; i++)
				items.Add($"Item #{i}");

			var list = new ListView
			{
				ItemTemplate = new DataTemplate(typeof(TestCell)),
				ItemsSource = items
			};
			list.ItemSelected += List_ItemSelected;

			Content = new StackLayout
			{
				Children = {
					label,
					list
				}
			};
		}

		void List_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			label.Text = Success;
		}

#if UITEST
		[Test]
		public void Bugzilla58833Test()
		{
			RunningApp.WaitForElement(q => q.Marked($"Item #1"));
			RunningApp.Tap(q => q.Marked($"Item #1"));
			RunningApp.WaitForElement(q => q.Marked(Success));
		}
#endif
	}
}