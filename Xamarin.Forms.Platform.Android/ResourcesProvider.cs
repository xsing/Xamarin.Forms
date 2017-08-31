using Android.Content;
using Android.Content.Res;
using Android.Util;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android
{
	internal class ResourcesProvider : ISystemResourcesProvider
	{
		readonly ResourceDictionary _dictionary;

		public ResourcesProvider(Context context)
		{
			_dictionary = new ResourceDictionary();
			UpdateStyles(context);
		}

		public IResourceDictionary GetSystemResources()
		{
			return _dictionary;
		}

		static Style GetStyle(int style, Context context)
		{
			var result = new Style(typeof(Label));

			double fontSize = 0;
			string fontFamily = null;
			global::Android.Graphics.Color defaultColor = global::Android.Graphics.Color.Argb(0, 0, 0, 0);
			global::Android.Graphics.Color androidColor = defaultColor;

			using (var value = new TypedValue())
			{
				if (context.Theme.ResolveAttribute(style, value, true))
				{
					var styleattrs = new[] { global::Android.Resource.Attribute.TextSize, global::Android.Resource.Attribute.FontFamily, global::Android.Resource.Attribute.TextColor };
					using (TypedArray array = context.ObtainStyledAttributes(value.ResourceId, styleattrs))
					{
						fontSize = context.FromPixels(array.GetDimensionPixelSize(0, -1));
						fontFamily = array.GetString(1);
						androidColor = array.GetColor(2, defaultColor);
					}
				}
			}

			if (fontSize > 0)
				result.Setters.Add(new Setter { Property = Label.FontSizeProperty, Value = fontSize });

			if (!string.IsNullOrEmpty(fontFamily))
				result.Setters.Add(new Setter { Property = Label.FontFamilyProperty, Value = fontFamily });

			if (androidColor != defaultColor)
			{
				result.Setters.Add(new Setter { Property = Label.TextColorProperty, Value = Color.FromRgba(androidColor.R, androidColor.G, androidColor.B, androidColor.A) });
			}

			return result;
		}

		void UpdateStyles(Context context)
		{
			_dictionary[Device.Styles.BodyStyleKey] = new Style(typeof(Label)); // do nothing, its fine
			_dictionary[Device.Styles.TitleStyleKey] = GetStyle(global::Android.Resource.Attribute.TextAppearanceLarge, context);
			_dictionary[Device.Styles.SubtitleStyleKey] = GetStyle(global::Android.Resource.Attribute.TextAppearanceMedium, context);
			_dictionary[Device.Styles.CaptionStyleKey] = GetStyle(global::Android.Resource.Attribute.TextAppearanceSmall, context);
			_dictionary[Device.Styles.ListItemTextStyleKey] = GetStyle(global::Android.Resource.Attribute.TextAppearanceListItem, context);
			_dictionary[Device.Styles.ListItemDetailTextStyleKey] = GetStyle(global::Android.Resource.Attribute.TextAppearanceListItemSmall, context);
		}
	}
}