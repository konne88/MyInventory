using System.Runtime.InteropServices;
using System;
using Gtk;
using MyInventory.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;

namespace MyInventory.GtkGui {
    public class ItemsBox : HBox, IDrawDescriptionEntry, IShowMe
    {
		private ItemsBox(Builder builder)
		: base(builder.GetRawObject("itemsBox"))
		{
			builder.Autoconnect (this);
			
			itemsViewFilter.ExposeEvent += OnExposeDescriptionEntry;
		}
		
        public ItemsBox (Items items, UIManager uiManager)
		: this(new Builder("items_box.ui"))
        {
			Items = items;
			
			// create the actions
			Gtk.Action create = new Gtk.Action("createItem","Create Item","",Stock.Add);
			create.Activated += OnCreateItem;
			Gtk.Action delete = new Gtk.Action("deleteItem","Delete Item","",Stock.Remove);
			delete.Activated += OnDeleteItem;
			Gtk.Action action = new Gtk.Action("item","Item");
			
			ActionGroup group = new ActionGroup("item");
			group.Add(create);
			group.Add(delete);
			group.Add(action);			
			uiManager.InsertActionGroup(group,0);
			
			// create item column
			TreeViewColumn col = new TreeViewColumn ();
			col.Title = "Item";
			col.Expand = true;
			CellRenderer render;			
			render = new CellRendererPixbuf ();
			col.PackStart (render, false);
			col.AddAttribute (render, "pixbuf", 1);
			render = new CellRendererText ();
			col.PackStart (render, true);
			col.AddAttribute (render, "text", 2);
			itemsView.AppendColumn(col);
			
			// create all other columns
			itemsView.AppendColumn ("ID", new Gtk.CellRendererText (), "text", 3);
			itemsView.AppendColumn ("Type", new Gtk.CellRendererText (), "text", 4);
			itemsView.AppendColumn ("Cost", new Gtk.CellRendererText (), "text", 5);
			itemsView.AppendColumn ("Value", new Gtk.CellRendererText (), "text", 6);
			
			// set the model
			TreeModel model = new TreeModelAdapter(new ItemsModel(items));
			model.RowInserted += OnRowInserted;
			TreeModelFilter filter = new TreeModelFilter (model, null);
			filter.VisibleFunc = new TreeModelFilterVisibleFunc (FilterItems);	
			itemsView.Model = filter;
			
			// create other widgets
			itemEditBox = new ItemEditBox(items,uiManager);
			itemEditBox.DrawDescriptionEntry += OnDrawDescriptionEntry;
			itemEditBox.ShowMe += OnShowItemEditBox;
			itemEditAlign.Add(itemEditBox);
			
			itemCreateBox = new ItemCreateBox(items);
			itemCreateBox.ShowMe += OnShowItemCreateBox;
			itemCreateAlign.Add(itemCreateBox);
			
			itemsView.Selection.Mode = SelectionMode.Browse;
			itemsView.Selection.Changed += OnItemSelectionChanged;
			
			// create the popups
			uiManager.AddUiFromResource("items_box_menues.xml");
			itemPopup = (Menu) uiManager.GetWidget("/itemPopup");
        }
		
		public void SelectItem(Model.Item item){
			if(ShowMe != null)
				ShowMe(this,new ShowMeEventArgs());
			
			if(item == null)
				itemCreateBox.CreateItem();
			else {
				itemsView.Model.Foreach(delegate (TreeModel model, TreePath path, TreeIter iter) {
					if(object.ReferenceEquals(model.GetValue(iter,0), item)){
						// show the item by removing letters from the filter-string until the new item is visible
						TreeModelFilter filter = (TreeModelFilter)itemsView.Model;
						
						while(true){
							if(FilterItems(model,iter) == true) break;
							string text = itemsViewFilter.Text;
							itemsViewFilter.Text = text.Remove(text.Length-1,1);
						}
						filter.Refilter();
						
						// select the item
						path = filter.ConvertChildPathToPath(path);
						itemsView.Selection.SelectPath(path);
						itemsView.ScrollToCell(path,null, true, 0.5f, 0f);		
						
						return true;
					}
					return false;
				});
			}
		}
		
		private bool FilterItems (TreeModel model, Gtk.TreeIter iter)
		{
			//Random randObj = new Random();
			//int v = randObj.Next(2) ;
			//return v == 0;
			/* FIXME 
			 * Occurs while filtering sometimes
		     *
		 	 * (MyInventory:29083): Gtk-CRITICAL **: gtk_tree_model_filter_real_unref_node: assertion `filter->priv->stamp == iter->stamp' failed
			 */
			// this function doesn't pass the filter but the actual model used
			Model.Item item = (Model.Item)model.GetValue(iter,0);
			if(item == null) return false;
			return item.MatchesKey(itemsViewFilter.Text);
		}
		
		private void OnItemsViewPopup(object o, WidgetEventArgs args){
			if(args.Event is Gdk.EventButton){
				Gdk.EventButton e = (Gdk.EventButton) args.Event;
			
				//3 is the right mouse button
				if(e.Button == 3){
					itemPopup.Popup(null,null,null,e.Button, e.Time);
				}	
			}
		}
		
		public void OnItemsFilterChanged(object o,EventArgs args){
			(itemsView.Model as TreeModelFilter).Refilter();
		}
		
		private void OnCreateItem(object sender, EventArgs args) {
			itemCreateBox.CreateItem();
		}
		
		private void OnDeleteItem(object o, EventArgs args){
			// get the currently selected item
			TreeModel filter;
			TreeIter iter;
			if(itemsView.Selection.GetSelected(out filter, out iter) == false){
				Console.WriteLine("No item selected that could be deleted");
				return;
			}
			Model.Item item = (Model.Item)filter.GetValue(iter, 0);
			
			try {
				// remove the item
				Items.TestRemove(item);
				TreePath path = filter.GetPath(iter);
				path = (filter as TreeModelFilter).ConvertPathToChildPath(path);
				Items.Positions.RemoveAt(path.Indices[path.Depth-1]);
				Items.Remove(item);
			} catch(ItemLocationReferenceException ex) {
				Console.WriteLine("Item can't be deleted since it is still referenced by Locations");
			}
		}
		
		public void OnRowInserted(object o, RowInsertedArgs args){
			if(ShowMe != null)
				ShowMe(this,new ShowMeEventArgs());
			
			TreeModelFilter filter = (TreeModelFilter)itemsView.Model;
			TreeModel model = filter.Model;
			
			// show the created item by removing letters from the filter-string until the new item is visible
			while(true){
				if(FilterItems(model,args.Iter) == true) break;
				string text = itemsViewFilter.Text;
				itemsViewFilter.Text = text.Remove(text.Length-1,1);
			}
			filter.Refilter();
			
			// select the created item
			TreePath path = filter.ConvertChildPathToPath(args.Path);
			itemsView.Selection.SelectPath(path);
			itemsView.ScrollToCell(path,null, true, 0.5f, 0f);
		}
		
		private void OnItemSelectionChanged(object sender, EventArgs args)
		{
			TreeIter iter;
			TreeModel filter;
			// if an item is selected show the editing dialog
			if(itemsView.Selection.GetSelected(out filter, out iter)){
				itemEditBox.EditItem((Model.Item)filter.GetValue(iter,0));
			}
			// if no item is selected select the first or create a new one
			else {
				if(filter.GetIterFirst(out iter)) 
					itemsView.Selection.SelectIter(iter);
				else
					itemCreateBox.CreateItem();
			}
		}
		
		private void OnDrawDescriptionEntry (object entry, DrawDescriptionEntryEventArgs args) {
			if(DrawDescriptionEntry != null){
				DrawDescriptionEntry(entry,args);
			}
		}
		
		private void OnExposeDescriptionEntry (object entry, ExposeEventArgs args) {
			if(DrawDescriptionEntry != null){
				string desc = "no desc";				
				if(object.ReferenceEquals(entry, itemsViewFilter)) desc = "Filter Items";				
				DrawDescriptionEntry(entry,new DrawDescriptionEntryEventArgs(desc,args.Event.Window));
			}
		}
		
		private void OnShowItemEditBox(object sender, ShowMeEventArgs args){
			if(ShowMe != null)
				ShowMe(this,new ShowMeEventArgs());
			
			itemManipulationBook.ShowTabByWidget(itemEditAlign);
		}
		
		private void OnShowItemCreateBox(object sender, ShowMeEventArgs args){
			if(ShowMe != null)
				ShowMe(this,new ShowMeEventArgs());
			
			itemManipulationBook.ShowTabByWidget(itemCreateAlign);
		}
		
		public event ShowMeEventHandler ShowMe;
		public event DrawDescriptionEntryEventHandler DrawDescriptionEntry;
		
		private readonly Items Items;
		
		private Menu itemPopup;
		
        [Builder.Object] private TreeView itemsView;
		[Builder.Object] private Entry itemsViewFilter;
		[Builder.Object] private Notebook itemManipulationBook;
		[Builder.Object] private   Alignment itemEditAlign;
		private                       ItemEditBox itemEditBox;
		[Builder.Object] private   Alignment itemCreateAlign;
		private                       ItemCreateBox itemCreateBox;

    }
}