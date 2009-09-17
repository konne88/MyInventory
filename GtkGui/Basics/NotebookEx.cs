using Gtk;
using System;

namespace MyInventory.GtkGui {
	public static class NotebookEx
	{
		public static void ShowTabByWidget(this Notebook self, Widget w)
		{
			self.Page = self.PageNum(w);
		}
	}
}