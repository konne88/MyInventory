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
using System.Collections;
using System.Collections.ObjectModel;

namespace MyInventory.GtkGui
{
	public class LocationsModel : ObservableTreeModel<Location>
	{
		public LocationsModel(Locations locs)
		: base(locs.Positions)
		{
			Locations = locs;
		}
		
		protected override Type ProvideColumnType(int col){
			switch(col){
			case 0:
				return typeof(Location);
			case 1:
				return typeof(Gdk.Pixbuf);
			case 4:
				return typeof(bool);
			case 5:
				return typeof(bool);
			default:
				return typeof(string);
			}
		}
		
		protected override object GetColumnValue(object o, int col){
			Location loc = (Location)o;
			
			switch(col){
			case 0:
				return loc;
			case 1:
				Model.Image img = loc.Item.Images.MainImage;
				if(img != null)
					return ((GtkMemoryImage)img.PreviewImage).Pixbuf;
				if(loc.Item is Product)
					return ((GtkSettings)Locations.Inventory.Settings).ProductIcon;
				if(loc.Item is Room)
					return ((GtkSettings)Locations.Inventory.Settings).RoomIcon;
				if(loc.Item is Estate)
					return ((GtkSettings)Locations.Inventory.Settings).EstateIcon;
				break;
			case 2:
				return loc.Item.Description;
			case 3:
				return loc.Item.Id.ToString();
			case 4:
				return loc.Labeled;
			case 5:
				Product product = loc.Item as Product;
				if(product != null)
					return product.LabelMethod != LabelMethod.None;
				else
					return false;
			case 6:
				return loc.Amount.ToString();
			}
			
			return "Column not defined";
		}
			
		public override int NColumns { 
			get {
				return 6;
			}
		}
		
		private readonly Locations Locations;
	}
}
