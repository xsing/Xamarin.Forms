using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IPerformanceProvider
	{
		void Stop(string tag, string path, string member);

		void Start(string tag, string path, string member);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public class Performance
	{
		public static IPerformanceProvider Provider { get; private set; }

		public static void SetProvider(IPerformanceProvider instance)
		{
			Provider = instance;
		}

		public static void Start(string tag = null, [CallerFilePath] string path = null, [CallerMemberName] string member = null)
		{
			Provider?.Start(tag, path, member);
		}

		public static void Stop(string tag = null, [CallerFilePath] string path = null, [CallerMemberName] string member = null)
		{
			Provider?.Stop(tag, path, member);
		}
	}
}