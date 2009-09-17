using System;
using Gtk;
using MyInventory.Model;

namespace MyInventory.GtkGui {
    public class GtkMemoryImage : Model.MemoryImage
    {
		public GtkMemoryImage(string path)
		: base(path)
		{}
		
		public override void Load()
		{
			if(_pixbuf==null)
				_pixbuf = new Gdk.Pixbuf(Path);
		}
		
		private Gdk.Pixbuf _pixbuf;
		public Gdk.Pixbuf Pixbuf {
			get {
				return _pixbuf;				
			}
		}
    }
}