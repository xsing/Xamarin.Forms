using System;

namespace Xamarin.Forms
{
	public class ClickedEventArgs : EventArgs
	{
		public ClickedEventArgs(object commandParameter)
		{
			Parameter = commandParameter;
		}

		public int Buttons
		{
			get;
			set;
		}

		public object Parameter { get; private set; }
	}
}