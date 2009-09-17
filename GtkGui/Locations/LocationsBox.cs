using System.Runtime.InteropServices;
using System;
using Gtk;
using MyInventory.Model;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MyInventory.GtkGui {
	
	public class LocationItemChooser : EntryCompletion {
		public LocationItemChooser(){}
		public LocationItemChooser(System.IntPtr i):base(i){}
	
		protected override bool OnMatchSelected (TreeModel filter, TreeIter iter){
			Location.Item = (Model.Item)filter.GetValue(iter,0);
			Entry entry = (Entry)Entry;
			entry.FinishEditing();
			entry.RemoveWidget();
			return true;
		}
		
		public Location Location;
	}
	
	public class LocationsFilter : TreeModelFilter , TreeDragDestImplementor {
		public LocationsFilter(LocationsModel model)
		: base(new TreeModelAdapter( model ),null)
		{
			LocationsModel = model;
		}
		
		public bool DragDataReceived(TreePath path, SelectionData data){
			return LocationsModel.DragDataReceived(path,data);
		}
		
		public bool RowDropPossible(TreePath path, SelectionData data){
			return LocationsModel.RowDropPossible(path,data);
		}
		
		private LocationsModel LocationsModel;
	}
	
    public class LocationsBox : VBox, IDrawDescriptionEntry, IShowMe, IGotoItem
	{
		private LocationsBox(Builder builder)
		: base(builder.GetRawObject("locationsBox"))
		{
			builder.Autoconnect (this);
			
			locationsViewFilter.ExposeEvent += OnExposeDescriptionEntry;
		}
		
        public LocationsBox (Locations locations, UIManager uiManager)
		: this(new Builder("locations_box.ui"))
        {
			Locations = locations;
			
			// create the actions
			Gtk.Action create = new Gtk.Action("createLocation","Create Location","",Stock.Add);
			create.Activated += OnCreateLocation;
			Gtk.Action delete = new Gtk.Action("deleteLocation","Delete Location","",Stock.Remove);
			delete.Activated += OnDeleteLocation;
			Gtk.Action gotoItem = new Gtk.Action("gotoLocationItem","Goto Item","",Stock.GoForward);
			gotoItem.Activated += OnGotoLocationItem;
			Gtk.Action action = new Gtk.Action("location","Location");
			
			ActionGroup group = new ActionGroup("location");
			group.Add(create);
			group.Add(delete);
			group.Add(gotoItem);
			group.Add(action);
			uiManager.InsertActionGroup(group,0);
			
			// create item column with id
			TreeViewColumn col = new TreeViewColumn ();
			locationsItemColumn = col;
			col.Title = "Item";
			col.Expand = true;
			CellRenderer render;
			render = new CellRendererPixbuf ();
			col.PackStart (render, false);
			col.AddAttribute (render, "pixbuf", 1);
			render = new CellRendererText ();
			(render as CellRendererText).Editable = true;
			render.EditingStarted += OnStartLocationItemEdit;
			col.PackStart (render, true);
			col.AddAttribute (render, "text", 2);
			locationsView.AppendColumn(col);
			locationsView.AppendColumn ("ID", new Gtk.CellRendererText (), "text", 3);
			
			// create the labeled column
			col = new TreeViewColumn ();
			col.Title = "Labeled";
			render = new CellRendererToggle ();
			(render as CellRendererToggle).Toggled += OnLabeledToggle;
			col.PackStart (render, false);
			col.AddAttribute (render, "active", 4);
			col.AddAttribute (render, "activatable", 5);
			locationsView.AppendColumn(col);
			
			// create the amount column
			col    = new TreeViewColumn ();
			col.Title = "Amount";
			render = new CellRendererSpin ();
			(render as CellRendererText).Editable = true;
			(render as CellRendererText).Edited += OnAmountEdited;		
			Adjustment adj = new Adjustment(0, 0, 0, 0, 0, 0);  //set all limits etc to 0
			adj.Upper = 1000000000;  // assign some special values, that aren't 0
			adj.PageIncrement = 10;
			adj.StepIncrement = 1;
			(render as CellRendererSpin).Adjustment = adj;
			col.PackStart (render, false);
			col.AddAttribute (render, "text", 6);
			locationsView.AppendColumn (col);
			
			//set model etc
			locations.CollectionChanged += OnLocationCreation;
			TreeModelFilter filter = new LocationsFilter ( new LocationsModel( locations ));
			filter.Model.RowInserted += OnRowInserted;
			filter.VisibleFunc = new TreeModelFilterVisibleFunc (FilterLocations);
	 		locationsView.Selection.Mode = SelectionMode.Multiple;
			locationsView.Model = filter;
			locationsView.Reorderable = true;
			
			// create the items chooser completion
			locationCompletion = new LocationItemChooser();
			TreeModel compModel = new TreeModelAdapter( new ItemsModel(locations.Inventory.Items));
			locationCompletion.Model = compModel;
			locationCompletion.MatchFunc = LocationItemCompletionMatch;
			locationCompletion.MinimumKeyLength = 0;
			// add the item info cell renderer to the completion	
			render = new CellRendererText ();
			locationCompletion.PackStart (render, true);
			locationCompletion.AddAttribute (render, "text", 2);
			
			// create the popups
			uiManager.AddUiFromResource("locations_box_menues.xml");
			locationPopup = (Menu) uiManager.GetWidget("/locationPopup");
	    }
		
		private bool FilterLocations (TreeModel model, Gtk.TreeIter iter)
		{
			// this function doesn't pass the filter but the actual model used
			/* FIXME:
			 * If locationView is filtered down so that no item is displayed anymore, the following pops up
			 * 
			 * (MyInventory:11584): Gtk-CRITICAL **: gtk_tree_model_row_has_child_toggled: assertion `path != NULL' failed
			 * 
			 * There is an article about this message, saying that it isn't a problem
			 * http://archives.seul.org/geda/user/Jan-2009/msg00072.html
			 */
			Location loc = (model.GetValue(iter,0) as Location);
			if(loc == null) return false;
			Model.Item item = loc.Item;
			if(item == null) return false;
			
			// now we got the item, let's check if we need to display it
			string key = locationsViewFilter.Text;
			if( item.MatchesKey(key) == true){
				return true;
			}
			else {  // if it doesn't match we also need to show it if one of it's children matches		
				TreeIter child;
				if(model.IterChildren(out child,iter)){
					return DoesSiblingLocationNeedToBeDisplayed(model,child, key);
				}
				else {
					return false; // no children			
				}
			}
		}
		
		private bool DoesSiblingLocationNeedToBeDisplayed(TreeModel model, TreeIter node, string key){	
			bool retVal = false;
			
			do {
				if((model.GetValue(node,0) as Location).Item.MatchesKey(key)){
					retVal = true;
					break;
				}
				else {
					TreeIter child;
					if(model.IterChildren(out child,node)){
						if(DoesSiblingLocationNeedToBeDisplayed(model,child,key) == true){
							retVal = true;
							break;
						}
					}
				}
			} while(model.IterNext(ref node));
			
			return retVal;
		}
		
		private bool LocationItemCompletionMatch (EntryCompletion comp, string key, TreeIter iter) {
			Model.Item item = (Model.Item)comp.Model.GetValue(iter,0);
			if(item == null) return false;
			return item.MatchesKey(key);
		}
		
		private void OnLocationsViewPopup(object o, WidgetEventArgs args){
			if(args.Event is Gdk.EventButton){
				Gdk.EventButton e = (Gdk.EventButton) args.Event;
			
				//3 is the right mouse button
				if(e.Button == 3){
					locationPopup.Popup(null,null,null,e.Button, e.Time);
				}
			}
		}
		
		private void OnCreateLocation(object sender, EventArgs args)
		{		
			// check if an item is already created
			IEnumerator iter = Locations.Inventory.Items.GetEnumerator();
			if(!iter.MoveNext()){
				Console.WriteLine("You need to create an item first in order to add a location.");
				return;
			}
			
			// create the new location
			Location loc = Locations.New((Model.Item)iter.Current);
			ObservableTreeNode<Location> otn = new ObservableTreeNode<Location>(loc,null);
			
			// add the new item as a sibling of the first selection		
			TreePath[] selections = locationsView.Selection.GetSelectedRows();
			TreeModelFilter filter = (TreeModelFilter)locationsView.Model;

			if(selections != null && selections.Length != 0){
				// something is selected
				TreePath pos = filter.ConvertPathToChildPath(selections[0]);
				pos.Next();
				Locations.Positions.InsertItemAt(pos.Indices,otn);
			}
			else {
				// nothing is selected
				Locations.Positions.Add(otn);
			}
		}
		
		private void DeleteLocations(TreePath path){
			path.Down();
			
			TreeModel model = ((TreeModelFilter)locationsView.Model).Model;
			TreeIter iter;
			while(model.GetIter(out iter,path) == true){
				// we also need delete the children of the location
				DeleteLocations(path.Copy());				
				Location loc = (Location)model.GetValue(iter,0);
				// this won't delete the children
				Locations.Remove(loc);
				
				// do move to next, since the location is not removed from the positions yet
				path.Next();
			}
		}
		
		private void DeleteSelectedLocations(TreePath path){
			if(path == null)
				path = TreePath.NewFirst();
			else
				path.Down();
			
			TreeModelFilter filter = (TreeModelFilter)locationsView.Model;
			TreeIter iter;
			while(filter.GetIter(out iter,path) == true){
				if(locationsView.Selection.PathIsSelected(path)){
					TreePath childPath = filter.ConvertPathToChildPath(path);
					TreeIter childIter = filter.ConvertIterToChildIter(iter);
					
					// we also need delete the children of the location
					DeleteLocations(childPath.Copy());
					
					// now that we found an iter that needs to be removed,
					// remove it
					Location loc = (Location)filter.Model.GetValue(childIter,0);
					
					// this won't delete the children
					Locations.Remove(loc);
					// this will also delete all the children
					Locations.Positions.RemoveItemAt(childPath.Indices);
					// do not move to next, since the next item will now be at the current position
				}
				else {
					DeleteSelectedLocations(path.Copy());
					path.Next();
				}
			}
		}	
		
		private void OnDeleteLocation(object sender, EventArgs args)
		{
			// we can't simply get all selected items and
			// then remove each of them cause after 
			// removing one item all the other iters or paths
			// become invalid
			// therefore we go throught the tree and check if 
			// an item is selected and remove it if so
			DeleteSelectedLocations(null);
		}
				
		private void OnGotoLocationItem (object sender, EventArgs args) {
			if(GotoItem != null){
				TreePath[] paths = locationsView.Selection.GetSelectedRows();
				if(paths.Length > 0){
					TreeIter iter;
					TreeModel model = locationsView.Model;
					if(model.GetIter(out iter, paths[0])){
						Location loc = (Location)model.GetValue(iter,0);
						GotoItem(this,new GotoItemEventArgs(loc.Item));
					}
				}
			}
		}
				
		private void OnLocationCreation(object sender, NotifyCollectionChangedEventArgs args) {
			if(args.Action == NotifyCollectionChangedAction.Add){
				LocationJustCreated = true;
			}
		}
				
		public void OnRowInserted(object o, RowInsertedArgs args){
			if(ShowMe != null)
				ShowMe(this,new ShowMeEventArgs());
			
			TreeModelFilter filter = (TreeModelFilter)locationsView.Model;
			TreeModel model = filter.Model;
			// show the created location by removing letters from the filter-string until the new item is visible
			while(true){
				if(FilterLocations(model,args.Iter) == true) break;
				string text = locationsViewFilter.Text;
				locationsViewFilter.Text = text.Remove(text.Length-1,1);
			}
			filter.Refilter();
			
			if(LocationJustCreated == true){
				// activate and edit the created location
				TreePath path = filter.ConvertChildPathToPath(args.Path);
				locationsView.ExpandToPath(path);
				locationsView.SetCursor(path,locationsItemColumn,true);
				LocationJustCreated = false;
			}
		}
		
		private void OnStartLocationItemEdit(object o, EditingStartedArgs args){
			TreeModel filter = locationsView.Model;
			Entry entry = (Entry)args.Editable;
			TreeIter iter;
			Location loc;
			
			filter.GetIter(out iter, new TreePath(args.Path));
			loc = (Location)filter.GetValue(iter,0);
			locationCompletion.Location = loc;
			entry.Completion = locationCompletion;
			if(loc.Item != null && loc.Item.Name != null) entry.Text = loc.Item.Name;
		}
		
		public void OnLocationsFilterChanged(object o,EventArgs args){
			(locationsView.Model as TreeModelFilter).Refilter();
		}
		
	    private void OnLabeledToggle(object o,ToggledArgs args) {
	        TreeIter iter;
	        TreeModel filter = (TreeModel)locationsView.Model;
	        
	        filter.GetIter (out iter, new TreePath(args.Path));
	        Location loc = (Location)filter.GetValue(iter,0);
	        if(loc.Item is Product){
	            if((loc.Item as Product).LabelMethod != LabelMethod.None){
	                loc.Labeled = !loc.Labeled;
	            }
	        }
	    }
		
		private void OnAmountEdited (object sender, EditedArgs args)
		{
			TreeModel filter = locationsView.Model;
			TreeIter iter;
			filter.GetIter (out iter, new TreePath (args.Path));
			(filter.GetValue (iter, 0) as Location).Amount = int.Parse(args.NewText);
		}
		
		private void OnExposeDescriptionEntry (object entry, ExposeEventArgs args) {
			if(DrawDescriptionEntry != null){
				string desc = "no desc";				
				if(object.ReferenceEquals(entry, locationsViewFilter)) desc = "Filter Locations";				
				DrawDescriptionEntry(entry,new DrawDescriptionEntryEventArgs(desc,args.Event.Window));
			}
		}
		
		public event GotoItemEventHandler GotoItem;
		public event ShowMeEventHandler ShowMe;
		public event DrawDescriptionEntryEventHandler DrawDescriptionEntry;
		
		private bool LocationJustCreated = false;
		private readonly Locations Locations;
		
		private TreeViewColumn locationsItemColumn;
		
		private Menu locationPopup;
		
		private                  LocationItemChooser locationCompletion;
        [Builder.Object] private TreeView locationsView;
		[Builder.Object] private Entry locationsViewFilter;
    }
}