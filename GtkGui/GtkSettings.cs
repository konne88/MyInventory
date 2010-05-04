using System;
using MyInventory.Model;

namespace MyInventory.GtkGui
{	
	public class GtkSettings : Settings
	{
		public GtkSettings() {
			ProductIcon = new Gdk.Pixbuf(null,"product.png");
			RoomIcon = new Gdk.Pixbuf(null,"room.png");	
			EstateIcon = new Gdk.Pixbuf(null,"estate.png");
			
			MemoryImage = typeof(GtkMemoryImage);
		}
		
		public Gdk.Color EntryDescriptionColor = new Gdk.Color (0xAA,0xAA,0xAA);
		
		public Gdk.Pixbuf ProductIcon;
		public Gdk.Pixbuf RoomIcon;
		public Gdk.Pixbuf EstateIcon;
	}
}
