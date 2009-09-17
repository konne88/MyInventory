using System;

namespace MyInventory.GtkGui {
	public static class StringEx
	{
		public static string EmptyIfNull(this string self)
		{
			return (self==null)?"":self;
		}
	}
}