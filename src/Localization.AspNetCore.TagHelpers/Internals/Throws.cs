using System;
using System.Runtime.CompilerServices;

namespace Localization.AspNetCore.TagHelpers.Internals
{
	internal static class Throws
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NotNull(object value, string name)
		{
			if (value == null)
				throw new ArgumentNullException(name);
		}
	}
}
