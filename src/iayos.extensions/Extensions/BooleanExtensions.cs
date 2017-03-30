using System.Diagnostics;

namespace iayos.extensions
{
	public static class BooleanExtensions
	{
		[DebuggerStepThrough]
		public static int ToBit(this bool myBool)
		{
			return (myBool) ? 1 : 0;
		}

		[DebuggerStepThrough]
		public static int ToBit(this bool? myBool)
		{
			return myBool.GetValueOrDefault() ? 1 : 0;
		}
	}
}
