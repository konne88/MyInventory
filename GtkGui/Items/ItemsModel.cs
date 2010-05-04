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
	public class ItemsModel : ObservableCollectionModel<Item>
	{
		public ItemsModel(Items items)
		: base(items.Positions)
		{
			Items = items;			
		}
		
		protected override Type ProvideColumnType(int col){
			switch(col){
			case 0:
				return typeof(Item);
			case 1:
				return typeof(Gdk.Pixbuf);
			default:
				return typeof(string);
			}
		}
		
		protected override object GetColumnValue(object o, int col){
			Item item = (Item)o;
			Product product = item as Product;
			Room room = item as Room;
			Estate estate = item as Estate;
			
			switch(col){
			case 0:
				return item;
			case 1:
				Model.Image img = item.Images.MainImage;
				if(img != null)
					return ((GtkMemoryImage)img.PreviewImage).Pixbuf;
				if(product != null)
					return ((GtkSettings)Items.Inventory.Settings).ProductIcon;
				if(room != null)
					return ((GtkSettings)Items.Inventory.Settings).RoomIcon;
				if(estate != null)
					return ((GtkSettings)Items.Inventory.Settings).EstateIcon;
				break;
			case 2:
				return item.Description.EmptyIfNull();
			case 3:
				return item.Id.ToString();
			case 4:
				return item.ItemType;
			case 5:	
				if(product != null)
					return product.Cost.ToString("#0.00€");
			 	else
					return "";
			case 6:
				if(product != null)
					return product.Value.ToString("#0.00€");
			 	else
					return "";	
			}
			
			return "Column not defined";
		}
			
		public override int NColumns { 
			get {
				return 7;
			}
		}
		
		private readonly Items Items;
	}
}
