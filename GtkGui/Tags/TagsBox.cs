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

using System.Runtime.InteropServices;
using System;
using Gtk;
using MyInventory.Model;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MyInventory.GtkGui {	
    public class TagsBox : HBox, IShowMe
    {
		private TagsBox(Builder builder)
		: base(builder.GetRawObject("tagsBox"))
		{
			builder.Autoconnect (this);
		}
		
		public TagsBox (Tags tags, UIManager uiManager)
		: this(new Builder("tags_box.ui"))
		{
			Tags = tags;
			
			// create the actions
			Gtk.Action create = new Gtk.Action("createTag","Create Tag","",Stock.Add);
			create.Activated += OnCreateTag;
			Gtk.Action delete = new Gtk.Action("deleteTag","Delete Tag","",Stock.Remove);
			delete.Activated += OnDeleteTag;
			Gtk.Action action = new Gtk.Action("tag","Tag");
			
			ActionGroup group = new ActionGroup("tag");
			group.Add(create);
			group.Add(delete);
			group.Add(action);
			uiManager.InsertActionGroup(group,0);
			
			// create the tagsItemView
			TreeViewColumn col = new TreeViewColumn ();
			col.Title = "Items with this Tag";
			col.Expand = true;
			CellRenderer render = new CellRendererPixbuf ();
			col.PackStart (render, false);
			col.AddAttribute (render, "pixbuf", 1);
			render = new CellRendererText ();
			col.PackStart (render, true);
			col.AddAttribute (render, "text", 2);
			tagItemsView.AppendColumn(col);
			
			TreeModel model = new TreeModelAdapter(new ItemsModel(Tags.Inventory.Items));
			TreeModelFilter filter = new TreeModelFilter (model , null);
			tagItemsView.Model = filter;
			filter.VisibleFunc = new TreeModelFilterVisibleFunc (FilterTagItems);
			
			// create the tagsView
			col = new TreeViewColumn ();
			tagNameEditColumn = col;
			col.Title = "Tag";
			render = new CellRendererText ();
			(render as CellRendererText).Editable = true;
			(render as CellRendererText).Edited += OnTagNameEdited;
			col.PackStart (render, true);
			col.AddAttribute (render, "text", 1);
			tagsView.AppendColumn(col);
			
			tagsView.Selection.Changed += OnTagSelectionChanged;
			tagsView.Reorderable = true;
			tagsView.EnableSearch = false;
			tags.CollectionChanged += OnTagCreation;
			tagsView.Model = new TreeModelAdapter(new TagsModel(Tags));
			tagsView.Model.RowInserted += OnRowInserted;
			tagsView.RowCollapsed += OnSelectRowExpandedCollapsed;
			tagsView.RowExpanded += OnSelectRowExpandedCollapsed;
			
			// create the popups
			uiManager.AddUiFromResource("tags_box_menues.xml");
			tagPopup = (Menu) uiManager.GetWidget("/tagPopup");
		}
		
		private void OnSelectRowExpandedCollapsed(object o, object args) {
			((TreeModelFilter)tagItemsView.Model).Refilter();
		}
		
		private void GetTagChildTags(TreePath parent, TreeModel model, ref List<Tag> list) {
			TreeIter iter;
			TreePath path = parent.Copy();
			path.Down();
			
			while(model.GetIter(out iter, path)) {
				list.Add((Tag)model.GetValue(iter, 0));
				GetTagChildTags(path, model, ref list);
				path.Next();
			}
		}
		
		private bool FilterTagItems (TreeModel itemModel, TreeIter itemIter)
		{
			// this function doesn't pass the filter but the actual model used
			
			TreeModel tagModel;
			TreeIter tagIter;
			if(!tagsView.Selection.GetSelected(out tagModel, out tagIter))
				return false;
			TreePath tagPath = tagModel.GetPath(tagIter);
			
			// we search in items for the currently selected tag
			Tag tag = (Tag)tagModel.GetValue(tagIter,0);
			if(tag == null)
				return false;
			
			// and if the selected tag is not expanded, we also show it's children
			List<Tag> tags = new List<Tag>();
			tags.Add(tag);
			if(!tagsView.GetRowExpanded(tagPath))
				GetTagChildTags(tagPath,tagModel,ref tags);
			
			// we get the current item and check if any of the tags exist in it
			Model.Item item = (Model.Item)itemModel.GetValue(itemIter,0);
			if(item == null) 
				return false;
			
			foreach(Tag t in tags){
				foreach(ItemTag it in item.Tags){
					if(object.ReferenceEquals(t, it.Tag)){
						return true;
					}
				}
			}
			
			return false;
		}
		
		private void OnTagSelectionChanged(object sender, EventArgs args)
		{
			((TreeModelFilter)tagItemsView.Model).Refilter();
		}
		
		public void OnRowInserted(object o, RowInsertedArgs args){
			if(ShowMe != null)
				ShowMe(this,new ShowMeEventArgs());
			
			TreeModel model = tagsView.Model;
			
			if(TagJustCreated == true){
				// activate and edit the created tag
				TreePath path = args.Path;
				tagsView.ExpandToPath(path);
				tagsView.SetCursor(path,tagNameEditColumn,true);
				TagJustCreated = false;
			}
		}
		
		private void OnCreateTag(object sender, EventArgs args)
		{
			// create the new tag
			Tag tag = Tags.New();
			ObservableTreeNode<Tag> otn = new ObservableTreeNode<Tag>(tag,null);
			
			// add the new item as a sibling of the first selection		
			TreePath[] selections = tagsView.Selection.GetSelectedRows();
			TreeModel model = (TreeModel)tagsView.Model;
			
			if(selections != null && selections.Length != 0){
				// something is selected
				TreePath pos = selections[0];
				pos.Next();
				Tags.Positions.InsertItemAt(pos.Indices,otn);
			}
			else {
				// nothing is selected
				Tags.Positions.Add(otn);
			}
		}
				
		private void OnTagCreation(object sender, NotifyCollectionChangedEventArgs args) {
			if(args.Action == NotifyCollectionChangedAction.Add){
				TagJustCreated = true;
			}
		}
		
		private void TestDeleteTags(TreePath path){
			path.Down();
			
			TreeModel model = (TreeModel)tagsView.Model;
			TreeIter iter;
			while(model.GetIter(out iter,path) == true){
				// we also need test delete the children of the tag
				TestDeleteTags(path.Copy());
				Tag tag = (Tag)model.GetValue(iter,0);
				// this won't test delete the children
				Tags.TestRemove(tag);
				
				// do move to next, since the item is not removed from the positions yet
				path.Next();
			}
		}
		
		private void DeleteTags(TreePath path){
			path.Down();
			
			TreeModel model = (TreeModel)tagsView.Model;
			TreeIter iter;
			while(model.GetIter(out iter,path) == true){
				// we also need delete the children of the tag
				DeleteTags(path.Copy());
				Tag tag = (Tag)model.GetValue(iter,0);
				// this won't delete the children
				Tags.Remove(tag);
				
				// do move to next, since the item is not removed from the positions yet
				path.Next();
			}
		}
				
		private void OnDeleteTag(object sender, EventArgs args)
		{
			// get the selected location
			TreeModel model = (TreeModel)tagsView.Model;
			TreeIter iter;
			tagsView.Selection.GetSelected(out iter);
			TreePath path = model.GetPath(iter);
			Tag tag = (Tag)model.GetValue(iter,0);
			
			// now we need to test if the tag or it's child tags
			// do have references to items, if so we can't delete
			try {
				TestDeleteTags(path.Copy());
				Tags.TestRemove(tag);
			}
			catch(TagItemReferenceException){
				Console.WriteLine("Tag can't be deleted since it is still referenced by Items");
				return;
			}
			
			// now knowing, that no tags are referenced
			// we need to delete the children of the tag and
			// the tag itsseld from Tags
			DeleteTags(path.Copy());
			Tags.Remove(tag);
			
			// finally remove everything from Positions
			Tags.Positions.RemoveItemAt(path.Indices);
		}
		
		private void OnTagNameEdited(object o, EditedArgs args) {
			TreeModel filter = tagsView.Model;
			TreeIter iter;
			filter.GetIter (out iter, new TreePath (args.Path));
			Tag tag = (Tag)filter.GetValue (iter, 0);
			
			string name = args.NewText.Trim();
			
			try {
				tag.Name = name;
			}
			catch(ArgumentException) {
				Console.WriteLine("Can't change the tagname cause it is eigther empty or already in use.");
			}
		}
		
		private void OnTagsViewPopup(object o, WidgetEventArgs args){
			if(args.Event is Gdk.EventButton){
				Gdk.EventButton e = (Gdk.EventButton) args.Event;
			
				//3 is the right mouse button
				if(e.Button == 3){
					tagPopup.Popup(null,null,null,e.Button, e.Time);
				}	
			}
		}
		
		public event ShowMeEventHandler ShowMe;
		
		private readonly Tags Tags;
		private bool TagJustCreated = false;
		
		private Menu tagPopup;
		
		private TreeViewColumn tagNameEditColumn;
		
		[Builder.Object] private TreeView tagsView;
		[Builder.Object] private TreeView tagItemsView;
    }
}