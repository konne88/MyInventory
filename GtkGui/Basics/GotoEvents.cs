using Gtk;
using System;
using MyInventory.Model;

namespace MyInventory.GtkGui {
	public class GotoItemEventArgs : EventArgs 
	{
		public GotoItemEventArgs(Model.Item item)
		{
			Item = item;
		}
		
		public Model.Item Item;
	}
	
	public delegate void GotoItemEventHandler(object sender, GotoItemEventArgs args);
	
	public interface IGotoItem
	{
		event GotoItemEventHandler GotoItem;
	}
}