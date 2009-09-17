using System;
using MyInventory.Model;
using System.Collections;
using System.Collections.ObjectModel;

namespace MyInventory.GtkGui
{
	public class TagsModel : ObservableTreeModel<Tag>
	{
		public TagsModel(Tags tags)
		: base(tags.Positions)
		{
			Tags = tags;
		}
		
		protected override Type ProvideColumnType(int col){
			switch(col){
			case 0:
				return typeof(Tag);
			default:
				return typeof(string);
			}
		}
		
		protected override object GetColumnValue(object o, int col){
			Tag tag = (Tag)o;
			
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
		
		private readonly Tags Tags;
	}
}
