using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Embedding.XF;
using Xamarin.Forms.Platform.UWP;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Embedding.UWP
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			InitializeComponent();

			HelloFlyout.Content = new Hello().CreateFrameworkElement();
		}

		void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(Page1));
		}
	}
}