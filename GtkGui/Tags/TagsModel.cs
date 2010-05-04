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
