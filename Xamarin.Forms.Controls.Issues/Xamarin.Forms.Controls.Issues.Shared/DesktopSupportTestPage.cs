using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 9090, "Desktop Support", PlatformAffected.All)]
	public class DesktopSupportTestPage : TestNavigationPage
	{
		protected override void Init()
		{
			PushAsync(Menu());
		}

		ContentPage Menu()
		{
			var layout = new StackLayout();

			layout.Children.Add(new Label { Text = "Select a test below" });

			foreach (var test in GenerateTests)
			{
				layout.Children.Add(MenuButton(test));
			}

			return new ContentPage { Content = layout };
		}

		Button MenuButton(TestDesktop test)
		{
			var button = new Button { Text = test.TestName, AutomationId = test.AutomationId };

			button.Clicked += (sender, args) => test.Command();

			return button;
		}

		Page RightClickSupportPage()
		{
			var layout = new StackLayout();
			var label = new Label { Text = "Click the box with left and right click" };
			var box = new BoxView { BackgroundColor = Color.Red, WidthRequest = 100, HeightRequest = 100 };

			var btn = new Button { Text = "Clear", Command = new Command(() => label.Text = "") };
			var btnNormal = new Button
			{
				Text = "Normal Click",
				Command = new Command(() =>
				{
					label.Text = "";
					box.GestureRecognizers.Clear();
					box.GestureRecognizers.Add(new ClickGestureRecognizer
					{
						Command = new Command((obj) =>
						{
							label.Text = "Clicked";
						})
					});
				})
			};
			var btnRight = new Button
			{
				Text = "Right Click",
				Command = new Command(() =>
				{
					label.Text = "";
					box.GestureRecognizers.Clear();
					box.GestureRecognizers.Add(new ClickGestureRecognizer
					{
						Buttons = ButtonsMask.Secondary,
						Command = new Command((obj) =>
						{
							label.Text = "Right Clicked";
						})
					});
				})
			};
			var btnBoth = new Button
			{
				Text = "Both Clicks",
				Command = new Command(() =>
				{
					box.GestureRecognizers.Clear();
					label.Text = "";
					box.GestureRecognizers.Add(new ClickGestureRecognizer
					{
						Buttons = ButtonsMask.Primary | ButtonsMask.Secondary,
						Command = new Command((obj) =>
						{
							label.Text = "Left and Right Clicked";
						})
					});
				})
			};


			layout.Children.Add(label);
			layout.Children.Add(box);
			layout.Children.Add(btn);
			layout.Children.Add(btnNormal);
			layout.Children.Add(btnRight);
			layout.Children.Add(btnBoth);
			return new ContentPage { Content = layout };
		}

		Page MenusSupportPage()
		{
			var layout = new StackLayout();
			var label = new Label { Text = "Adding menus" };
			var btn = new Button { Text = "Clear", Command = new Command(() => Application.Current.MainMenu.Clear()) };
			var btnAdd = new Button { Text = "Add Menu Hello", Command = new Command(() => AddMenu(1)) };
			var btnAdd3 = new Button { Text = "Add 3 Menu Hello", Command = new Command(() => AddMenu(3)) };
			var btnAdd3Add2 = new Button { Text = "Add Menu Hello with 2 Subitems", Command = new Command(() => AddMenu(3, true, 2)) };
			var btnAddImage = new Button { Text = "Add Menu Hello With Icon", Command = new Command(() => AddMenu(1, true, 1, withImage:true)) };
			var btnAddChangeText = new Button { Text = "Add Menu Change Text and disable after 3 seconds", Command = new Command(async () => 
			{
				AddMenu(1,true);
				await Task.Delay(3000);
				Application.Current.MainMenu[0].Items[0].Text = "hello changed";
				Application.Current.MainMenu[0].Items[0].IsEnabled = false;
			})};
			layout.Children.Add(btn);
			layout.Children.Add(btnAdd);
			layout.Children.Add(btnAdd3);
			layout.Children.Add(btnAdd3Add2);
			layout.Children.Add(btnAddImage);
			layout.Children.Add(btnAddChangeText);
			layout.Children.Add(label);
			return new ContentPage { Title = "Menus", Content = layout };
		}

		void AddMenu(int count, bool addMenuItems = false, int countMenuItems = 1, bool withImage = false)
		{
			for (int i = 0; i < count; i++)
			{
				var menu = new Forms.Menu { Text = $"hello {i}" };
				if (addMenuItems)
				{
					for (int j = 0; j < countMenuItems; j++)
					{
						var item = new MenuItem { Text = $"hello menu item {i}.{j}" };
						if(withImage)
						{
							item.Icon = Icon = "bank.png";
						}
						menu.Items.Add(item);
					}
				}
				Application.Current.MainMenu.Add(menu);
			}
		}

		IEnumerable<TestDesktop> GenerateTests
		{
			get
			{
				var testList = new List<TestDesktop>();
				testList.Add(new TestDesktop("Quit") { Command = () => { Application.Current.Quit(); } });
				testList.Add(new TestDesktop("RightClick") { Command = async () => { await Navigation.PushAsync(RightClickSupportPage()); } });
				testList.Add(new TestDesktop("Menus") { Command = async () => { await Navigation.PushAsync(MenusSupportPage()); } });

				return testList;
			}
		}

		public class TestDesktop
		{
			public TestDesktop(string name)
			{
				TestName = name;
			}

			public string TestName
			{
				get;
				set;
			}

			public string AutomationId => $"desktoptest_{TestName}";

			public Action Command
			{
				get;
				set;
			}

		}

	}
}
