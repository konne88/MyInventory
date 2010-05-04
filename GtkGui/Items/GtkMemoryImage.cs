/* MyInventory - Keep track of your private assets. 
 * Copyright (C) 2008-2010 Konstantin Weitz
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 3
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

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