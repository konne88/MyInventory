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
