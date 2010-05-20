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
		
		public readonly Gdk.Color EntryDescriptionColor = new Gdk.Color (0xAA,0xAA,0xAA);
		
		public readonly Gdk.Pixbuf ProductIcon;
		public readonly Gdk.Pixbuf RoomIcon;
		public readonly Gdk.Pixbuf EstateIcon;
		
		// print preview layout
		public readonly double PaperWidth = 65; 	// mm
		public readonly double SectionWidth = 80;
		public readonly double SectionHeight = 40;
		public readonly double SectionPadding = 10;
		public readonly double LabelPadding = 2;
		public readonly double PageHeightPerWidth = Math.Sqrt(2);
		public readonly double LabelHeightPerWidth = 0.5;
	}
}
