using Gtk;
using System;

namespace MyInventory.GtkGui {
	public class ShowMeEventArgs : EventArgs 
	{
		public ShowMeEventArgs()
		{}
	}
	
	public delegate void ShowMeEventHandler(object sender, ShowMeEventArgs args);
	
	public interface IShowMe
	{
		event ShowMeEventHandler ShowMe;
	}
}