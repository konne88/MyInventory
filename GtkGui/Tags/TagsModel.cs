using System;
using Gtk;
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
		
		public override bool RowDropPossible(TreePath destPath, SelectionData data)
		{
			if(!base.RowDropPossible(destPath,data))
				return false;
			
			// need to do this to prevent sideeffect, that actually happen
			destPath = destPath.Copy();
			
			if(!destPath.Up())	// we can always add to toplevel
				return true;
			
			TreeModel srcModel;
			TreePath srcPath;
			TreeIter srcIter;
			Tree.GetRowDragData(data, out srcModel, out srcPath);
			srcModel.GetIter(out srcIter, srcPath);
			
			Tag tag = (Tag)srcModel.GetValue(srcIter,0);
						
			return Tags.CanTagBecomePathsChild(tag,destPath.Indices);
		}
		
		private readonly Tags Tags;
	}
}
