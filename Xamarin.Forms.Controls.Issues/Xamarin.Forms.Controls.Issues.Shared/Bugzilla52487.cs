using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(
		IssueTracker.Bugzilla,
		52487,
		"ListView with Recycle + HasUnevenRows generates lots (and lots!) of content view",
		// https://bugzilla.xamarin.com/show_bug.cgi?id=52487
		PlatformAffected.iOS
	)]
	public class Bugzilla52487 : TestContentPage
	{
		const int FontSize = 12;
		const int MinScrollDelta = 2;
		const int ItemsCount = 1000;
		const int DefaultItemHeight = 300;

		static Tuple<int, int, int> Mix = new Tuple<int, int, int>(255, 255, 100);
		static IEnumerable<Color> ColorGenerator()
		{
			double colorDelta = 0;
			while (true)
			{
				colorDelta += 2 * Math.PI / 100;
				var r = (Math.Sin(colorDelta) + 1) / 2 * 255;
				var g = (Math.Sin(colorDelta * 2) + 1) / 2 * 255;
				var b = (Math.Sin(colorDelta * 3) + 1) / 2 * 255;

				if (Mix != null)
				{
					r = (r + Mix.Item1) / 2;
					g = (g + Mix.Item2) / 2;
					b = (b + Mix.Item3) / 2;
				}

				yield return Color.FromRgb((int)r, (int)g, (int)b);
			}
		}

		[Preserve(AllMembers = true)]
		internal sealed class Retain : 
			ListViewSpy<Retain>
		{
			public Retain() { }
		}

		[Preserve(AllMembers = true)]
		internal sealed class UnevenRecycleElement : 
			ListViewSpy<UnevenRecycleElement>
		{
			public UnevenRecycleElement() 
				: base(ListViewCachingStrategy.RecycleElement, 
					  hasUnevenRows: true) { }
		}

		[Preserve(AllMembers = true)]
		internal sealed class UnevenRecycleElementAndDataTemplate : 
			ListViewSpy<UnevenRecycleElementAndDataTemplate>
		{
			public UnevenRecycleElementAndDataTemplate() 
				: base(ListViewCachingStrategy.RecycleElementAndDataTemplate, 
					  hasUnevenRows: true) { }
		}

		[Preserve(AllMembers = true)]
		internal sealed class RecycleElement : 
			ListViewSpy<RecycleElement>
		{
			public RecycleElement() 
				: base(ListViewCachingStrategy.RecycleElement) { }
		}

		[Preserve(AllMembers = true)]
		internal sealed class RecycleElementAndDataTemplate : 
			ListViewSpy<RecycleElementAndDataTemplate>
		{
			public RecycleElementAndDataTemplate() 
				: base(ListViewCachingStrategy.RecycleElementAndDataTemplate) { }
		}

		[Preserve(AllMembers = true)]
		public abstract class ListViewSpy<T> : ListViewSpy
		{
			[Preserve(AllMembers = true)] 
			class Selector : DataTemplateSelector
			{
				DataTemplate _dataTemplate;

				public Selector()
				{
					// RecycleElementAndDataTemplate requires 
					// that the DataTemplate use the .ctor that takes a type
					_dataTemplate = new DataTemplate(typeof(CellSpy));
				}

				protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
				{
					__counter.OnSelectTemplate++;

					// RecycleElementAndDataTemplate requires that 
					// DataTempalte be a function of the _type_ of the item
					if (!(item is Item))
						throw new ArgumentException();

					return _dataTemplate;
				}
			}

			[Preserve(AllMembers = true)]
			class CellSpy : ViewCell
			{
				static IEnumerator<Color> Colors = ColorGenerator().GetEnumerator();
				static Color GetColor()
				{
					Colors.MoveNext();
					var color = Colors.Current;
					return color;
				}

				readonly int _id;
				readonly Label _label;

				public CellSpy()
				{
					_id = __counter.CellAlloc++;

					View = _label = new Label
					{
						BackgroundColor = GetColor(),
						VerticalTextAlignment = TextAlignment.Center,
						HorizontalTextAlignment = TextAlignment.Center,
					};

					_label.SetBinding(HeightRequestProperty, nameof(Item.Value));
				}

				private Item BindingContext
					=> (Item)base.BindingContext;

				protected override void OnBindingContextChanged()
				{
					base.OnBindingContextChanged();
					__counter.CellBind++;
					__counter.Update();
				}

				protected override void OnAppearing()
				{
					_label.Text = ToString();
					__counter.AttachCell(_id);
					__counter.Update();
				}

				public new int Id
					=> _id;

				public override string ToString()
					=> $"{BindingContext.Id} -> {_id}";

				~CellSpy()
				{
					int id;
					__counter.DetachCell(_id);
					// update would be off UI thread
				}
			}

			[Preserve(AllMembers = true)]
			class Item : INotifyPropertyChanged
			{
				internal static int s_height = DefaultItemHeight;

				int _id;
				int _height;

				internal Item(int id)
				{
					_height = s_height / (id % 2 + 1);
					_id = id;
					Interlocked.Increment(ref __counter.ItemAlloc);
				}

				public int Id
					=> _id;
				public int Value
				{
					get { return _height; }
					set
					{
						_height = value;
						OnPropertyChanged();
					}
				}

				public event PropertyChangedEventHandler PropertyChanged;
				protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
					=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

				public override string ToString()
					=> $"{Id}, value={Value}";

				~Item()
				{
					Interlocked.Increment(ref __counter.ItemFree);
				}
			}

			[Preserve(AllMembers = true)]
			class ItemSpy : IReadOnlyList<Item>
			{
				int _count;
				List<WeakReference<Item>> _items;

				internal ItemSpy(int count)
				{
					_count = count;
					_items = new List<WeakReference<Item>>(
						Enumerable.Range(0, count)
						.Select(o => new WeakReference<Item>(null))
					);
				}

				public Item this[int index]
				{
					get
					{
						__counter.ViewModelAsk.Add(index);

						var weakItem = _items[index];

						Item item;
						if (!weakItem.TryGetTarget(out item))
						{
							_items[index] = 
								new WeakReference<Item>(
									item = new Item(index));
						}

						return item;
					}
				}

				public int Count
					=> _count;

				public IEnumerator<Item> GetEnumerator()
				{
					for (var i = 0; i < Count; i++)
						yield return this[i];
				}

				IEnumerator IEnumerable.GetEnumerator()
					=> GetEnumerator();
			}

			[Preserve(AllMembers = true)]
			class Counter : StackLayout
			{
				static Label CreateLabel()
					=> new Label() { FontSize = FontSize };

				internal void Update()
				{
					int attachedCount = 0;
					lock (this)
					{
						attachedCount = CellAttached.Count;
					}

					CellAllocsLabel.Text
						= $"New={CellAlloc}";
					CellAppearedLabel.Text
						= $"Atch={attachedCount}";
					CellBindsLabel.Text
						= $"Bind={CellBind}";
					ItemAliveLabel.Text
						= $"VM={ItemAlloc - ItemFree}";
				}

				internal int CellAlloc;
				internal int CellFree;
				internal int CellBind;
				internal int ItemAlloc;
				internal int ItemFree;
				internal int OnSelectTemplate;
				internal HashSet<int> ViewModelAsk 
					= new HashSet<int>();

				HashSet<int> CellAttached 
					= new HashSet<int>();

				Label CellAppearedLabel = CreateLabel();
				Label CellAllocsLabel = CreateLabel();
				Label CellBindsLabel = CreateLabel();
				Label ItemMaxIndexLabel = CreateLabel();
				Label ItemAliveLabel = CreateLabel();

				internal Counter()
				{
					Children.Add(CellAppearedLabel);
					Children.Add(CellAllocsLabel);
					Children.Add(CellBindsLabel);
					Children.Add(ItemMaxIndexLabel);
					Children.Add(ItemAliveLabel);
					Update();
				}

				internal void AttachCell(int id)
				{
					lock (this)
						CellAttached.Add(id);
				}
				internal void DetachCell(int id)
				{
					lock (this)
						CellAttached.Remove(id);
				}
			}

			// Cell is activated via DataTemplate using default ctor which
			// makes it difficult to pass the counter to the cell. So we make
			// it static to give cell access and create a different generic
			// instantiation for each type of ListView to get different counters
			static Counter __counter = new Counter();

			int _appeared;
			int _disappeared;
			ListView _listView;
			Item[] _items;
			ItemSpy _itemSpy;

			public ListViewSpy(
				ListViewCachingStrategy cachingStrategy = ListViewCachingStrategy.RetainElement,
				bool hasUnevenRows = false)
			{
				_itemSpy = new ItemSpy(ItemsCount);

				var dataTemplate = new DataTemplate(typeof(CellSpy));
				if (cachingStrategy != ListViewCachingStrategy.RetainElement)
					dataTemplate = new Selector();

				_listView = new ListView(cachingStrategy)
				{
					HasUnevenRows = hasUnevenRows,
					// see https://github.com/xamarin/Xamarin.Forms/pull/994/files
					//RowHeight = 50,
					ItemsSource = _itemSpy,
					ItemTemplate = dataTemplate
				};
				_listView.ItemAppearing += (o, e)
					=> _appeared = ((Item)e.Item).Id;
				_listView.ItemDisappearing += (o, e)
					=> _disappeared = ((Item)e.Item).Id;
				Children.Add(_listView);

				Children.Add(__counter);
			}

			internal override void Down()
			{
				var target = Math.Max(_appeared, _disappeared);
				target += Math.Abs(_appeared - _disappeared) + MinScrollDelta;
				if (target >= _itemSpy.Count)
					target = _itemSpy.Count - 1;

				_listView.ScrollTo(_itemSpy[target], ScrollToPosition.MakeVisible, animated: true);
			}
			internal override void Up()
			{
				var target = Math.Min(_appeared, _disappeared);
				target -= Math.Abs(_appeared - _disappeared) + MinScrollDelta;
				if (target < 0)
					target = 0;

				_listView.ScrollTo(_itemSpy[target], ScrollToPosition.MakeVisible, animated: true);
			}
			internal override void UpdateHeights(double multipule)
			{
				if (multipule < 0 && Item.s_height < 40)
					return;

				Item.s_height = (int)(Item.s_height * multipule);
			}
		}

		[Preserve(AllMembers = true)]
		public abstract class ListViewSpy : StackLayout
		{
			internal abstract void Down();
			internal abstract void Up();
			internal abstract void UpdateHeights(double difference);
		}

		protected override void Init()
		{
			var stackLayout = new StackLayout();

			var buttonsGrid = new Grid();

			var listViews = new ListViewSpy[]
			{
				new Retain(),
				new RecycleElement(),
				new UnevenRecycleElement(),
				new UnevenRecycleElementAndDataTemplate(),
			};

			int height = 300;

			var moreButton = new Button() { Text = "x2" };
			moreButton.Clicked += (o, s)
				=> listViews.ForEach(x => x.UpdateHeights(2));
			buttonsGrid.Children.AddHorizontal(moreButton);

			var lessButton = new Button() { Text = $"{'\u00F7'}2" };
			lessButton.Clicked += (o, s)
				=> listViews.ForEach(x => x.UpdateHeights(.5));
			buttonsGrid.Children.AddHorizontal(lessButton);

			var upButton = new Button() { Text = "Up" };
			upButton.Clicked += (o, s)
				=> listViews.ForEach(x => x.Up());
			buttonsGrid.Children.AddHorizontal(upButton);

			var downButton = new Button() { Text = "Down" };
			downButton.Clicked += (o, s)
				=> listViews.ForEach(x => x.Down());
			buttonsGrid.Children.AddHorizontal(downButton);

			var listViewGrid = new Grid();
			foreach (var o in listViews)
				listViewGrid.Children.AddHorizontal(o);

			Content = new StackLayout {
				Children = {
					listViewGrid,
					buttonsGrid,
				}
			};
		}

#if UITEST
		//[Test]
		//public void Bugzilla56896Test()
		//{
		//	RunningApp.WaitForElement(q => q.Marked(Instructions));
		//	var count = int.Parse(RunningApp.Query(q => q.Marked(ConstructorCountId))[0].Text);
		//	Assert.IsTrue(count < 100); // Failing test makes ~15000 constructor calls
		//	var time = int.Parse(RunningApp.Query(q => q.Marked(TimeId))[0].Text);
		//	Assert.IsTrue(count < 2000); // Failing test takes ~4000ms
		//}
#endif
	}
}