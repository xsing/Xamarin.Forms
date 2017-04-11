using System;
using System.Linq;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.GalleryPages.PerformanceGallery.Scenarios
{
	[Preserve(AllMembers = true)]
	internal class ImageScenario1 : PerformanceScenario
	{
		public ImageScenario1()
		: base("Embedded image", 450)
		{
			View = new Image { Source = "coffee.png" };
		}
	}
}
