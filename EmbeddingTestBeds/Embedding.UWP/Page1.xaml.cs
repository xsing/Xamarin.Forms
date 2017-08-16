using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Embedding.XF;
using Xamarin.Forms.Platform.UWP;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Embedding.UWP
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Page1 : Page
	{
		public Page1()
		{
			InitializeComponent();
		}

		void Button_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(Page2));
		}

		void Button2_Click(object sender, RoutedEventArgs e)
		{
			// Note that this is different from navigating to a UWP page; the method takes an instance, not a type
			Frame.Navigate(new Page3());
		}
	}
}