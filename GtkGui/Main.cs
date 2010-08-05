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
using System.IO;

namespace MyInventory.GtkGui
{
	class GtkMyInventory {
		public static void Main (string[] args)
	    {
			Gtk.Application.Init ();
			
			// get the home folder
			String path = Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			path = Path.Combine(path,".my_inventory");
			path = Path.Combine(path,"inventory");
			
			Console.WriteLine("Loading the inventory from the path '"+path+"'");
			
			Inventory inv = Inventory.Load(path,new GtkSettings());
	
			Window w = new Window (inv);
			w.ShowAll ();
			Gtk.Application.Run ();
	    }
	}
}
