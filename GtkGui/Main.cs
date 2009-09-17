using System;
using Gtk;
using MyInventory.Model;

namespace MyInventory.GtkGui
{
	class GtkMyInventory {
		public static void Main (string[] args)
	    {
			Gtk.Application.Init ();		
			
			Inventory inv = Inventory.Load("inventory",new GtkSettings());
	
			Window w = new Window (inv);
			w.ShowAll ();
			Gtk.Application.Run ();
	    }
	}
}
