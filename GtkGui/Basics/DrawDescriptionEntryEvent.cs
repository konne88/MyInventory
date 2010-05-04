using Gtk;
using System;

namespace MyInventory.GtkGui {
	public class DrawDescriptionEntryEventArgs : EventArgs 
	{
		public DrawDescriptionEntryEventArgs(string description, Gdk.Window win)
		{
			Description = description;
			Window = win;
		}
		
		public Gdk.Window Window;
		public string Description;
	}
	public delegate void DrawDescriptionEntryEventHandler(object sender, DrawDescriptionEntryEventArgs args);
	
	public interface IDrawDescriptionEntry
	{
		event DrawDescriptionEntryEventHandler DrawDescriptionEntry;
	}
}