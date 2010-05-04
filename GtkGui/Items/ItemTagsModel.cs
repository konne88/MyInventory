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
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Gtk;

namespace MyInventory.GtkGui
{
	public class ItemTagsModel : EnumerableNotifyModel<IEnumerable<ItemTag>>
	{
		public ItemTagsModel(IEnumerable<ItemTag> itemTags)
		: base(itemTags)
		{
			ItemTags = itemTags;
		}
		
		protected override IEnumerable<ItemTag> GetCollection(object node){
			return null;
		}
		
		protected override object GetObject(object node){
			return node;
		}
		
		public override TreeModelFlags Flags { 
			get {
				return TreeModelFlags.ListOnly;
			}
		}
		
		protected override Type ProvideColumnType(int col){
			switch(col){
			case 0:
				return typeof(ItemTag);
			default:
				return typeof(string);
			}
		}
		
		protected override object GetColumnValue(object o, int col){
			ItemTag tag = (ItemTag)o;
			
			switch(col){
			case 0:
				return tag;
			case 1:
				return tag.Name;
			}
			
			return "Column not defined";
		}
		
		public override int NColumns { 
			get {
				return 2;
			}
		}
		
		private readonly IEnumerable<ItemTag> ItemTags;
	}
}
