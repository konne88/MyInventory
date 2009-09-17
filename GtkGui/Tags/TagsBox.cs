using System.Runtime.InteropServices;
using System;
using Gtk;
using MyInventory.Model;
using System.Collections;
using System.Collections.ObjectModel;

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
			//create.Activated += OnCreateTag;
			Gtk.Action delete = new Gtk.Action("deleteTag","Delete Tag","",Stock.Remove);
			//delete.Activated += OnDeleteTag;
			Gtk.Action action = new Gtk.Action("tag","Tag");
			
			ActionGroup group = new ActionGroup("tag");
			group.Add(create);
			group.Add(delete);
			group.Add(action);
			uiManager.InsertActionGroup(group,0);
			
			// create the tagsView
			TreeViewColumn col = new TreeViewColumn ();
			tagNameEditColumn = col;
			col.Title = "Tag";
			CellRenderer render = new CellRendererText ();
			//(render as CellRendererText).Editable = true;
			//(render as CellRendererText).Edited += OnTagNameEdited;
			col.PackStart (render, true);
			col.AddAttribute (render, "text", 1);
			tagsView.AppendColumn(col);
			
			//tagsView.Selection.Changed += OnTagSelectionChanged;
			tagsView.Reorderable = true;
			tagsView.Model = new TreeModelAdapter(new TagsModel(Tags));
			
			// create the tagsItemView
			col = new TreeViewColumn ();
			col.Title = "Items with this Tag";
			col.Expand = true;
			render = new CellRendererPixbuf ();
			col.PackStart (render, false);
			col.AddAttribute (render, "pixbuf", 1);
			render = new CellRendererText ();
			col.PackStart (render, true);
			col.AddAttribute (render, "text", 2);
			tagItemsView.AppendColumn(col);
			
			TreeModel model = new TreeModelAdapter(new ItemsModel(Tags.Inventory.Items));
			TreeModelFilter filter = new TreeModelFilter (model , null);
			//filter.VisibleFunc = new TreeModelFilterVisibleFunc (FilterTagItems);
			tagItemsView.Model = filter;
			
			// create the popups
			uiManager.AddUiFromResource("tags_box_menues.xml");
			tagPopup = (Menu) uiManager.GetWidget("/tagPopup");
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
		
		private Menu tagPopup;
		
		private TreeViewColumn tagNameEditColumn;
		
		[Builder.Object] private TreeView tagsView;
		[Builder.Object] private TreeView tagItemsView;
    }
}