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
using System.ComponentModel;
using Gtk;
using MyInventory.Model;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MyInventory.GtkGui {
	public class ItemTagChooser : EntryCompletion {
		public ItemTagChooser(){}
		public ItemTagChooser(System.IntPtr i):base(i){}
	
		protected override bool OnMatchSelected (TreeModel filter, TreeIter iter){
			Entry entry = (Entry)Entry;
			entry.Text = ((ItemTag)filter.GetValue(iter,0)).Name;
			entry.FinishEditing();
			entry.RemoveWidget();
			return true;
		}
	}
	
    public class ItemEditBox : VBox, IDrawDescriptionEntry, IShowMe
    {
		private ItemEditBox(Builder builder)
		: base(builder.GetRawObject("itemEditBox"))
		{
			builder.Autoconnect (this);
			
			itemEditWeight.ValueChanged += OnItemPropertyChanged;
			itemEditWarranty.ValueChanged += OnItemPropertyChanged;
			itemEditAmount.ValueChanged += OnItemPropertyChanged;
			itemEditUsefulLife.ValueChanged += OnItemPropertyChanged;
			itemEditCost.ValueChanged += OnItemPropertyChanged;
			itemEditDepreciationMethod.Changed += OnItemPropertyChanged;
			itemEditLabelMethod.Changed += OnItemPropertyChanged;
			
			itemEditName.ExposeEvent += OnExposeDescriptionEntry;
			itemEditMemo.ExposeEvent += OnExposeDescriptionEntry;
			itemEditBarcode.ExposeEvent += OnExposeDescriptionEntry;
			itemEditBrand.ExposeEvent += OnExposeDescriptionEntry;
			itemEditModel.ExposeEvent += OnExposeDescriptionEntry;
			itemEditZip.ExposeEvent += OnExposeDescriptionEntry;
			itemEditCity.ExposeEvent += OnExposeDescriptionEntry;
			itemEditStreet.ExposeEvent += OnExposeDescriptionEntry;
			itemEditCountry.ExposeEvent += OnExposeDescriptionEntry;
			itemEditPurchase.ExposeEvent += OnExposeDescriptionEntry;
			itemEditWarranty.ExposeEvent += OnExposeDescriptionEntry;
			itemEditWeight.ExposeEvent += OnExposeDescriptionEntry;
			itemEditCost.ExposeEvent += OnExposeDescriptionEntry;
			itemEditUsefulLife.ExposeEvent += OnExposeDescriptionEntry;
			itemEditAmount.ExposeEvent += OnExposeDescriptionEntry;
		}
		
        public ItemEditBox (Items items, UIManager uiManager)
		: this(new Builder("item_edit_box.ui"))
        {
			Items = items;
			
			// create image the actions
			Gtk.Action addImageAction = new Gtk.Action("addImage","Add Image...","",Stock.Open);
			addImageAction.Activated += OnAddItemImage;
			Gtk.Action removeImageAction = new Gtk.Action("removeImage","Remove Image","",Stock.Remove);
			removeImageAction.Activated += OnRemoveItemImage;
			Gtk.Action imageAction = new Gtk.Action("itemImage","Image");
			
			ActionGroup imageActionGroup = new ActionGroup("itemImage");
			imageActionGroup.Add(addImageAction);
			imageActionGroup.Add(removeImageAction);
			imageActionGroup.Add(imageAction);
			uiManager.InsertActionGroup(imageActionGroup,0);
			
			// create the tag actions
			Gtk.Action addTagAction = new Gtk.Action("addTag","Add Tag","",Stock.Add);
			addTagAction.Activated += OnAddItemTag;
			Gtk.Action removeTagAction = new Gtk.Action("removeTag","Remove Tag","",Stock.Remove);
			removeTagAction.Activated += OnRemoveItemTag;
			Gtk.Action tagAction = new Gtk.Action("itemTag","Tag");
			
			ActionGroup tagActionGroup = new ActionGroup("itemTag");
			tagActionGroup.Add(addTagAction);
			tagActionGroup.Add(removeTagAction);
			tagActionGroup.Add(tagAction);
			uiManager.InsertActionGroup(tagActionGroup,0);
			
			// add columns to the item type combobox
			CellRenderer render = new CellRendererText ();
			itemEditType.PackStart(render, true);
			itemEditType.AddAttribute(render, "text", 0);
			
			// add columns and model to the depreciation combobox
			render = new CellRendererPixbuf ();
			itemEditDepreciationMethod.PackStart(render, false);
			itemEditDepreciationMethod.AddAttribute(render, "pixbuf", 0);
			render = new CellRendererText ();
			itemEditDepreciationMethod.PackStart(render, true);
			itemEditDepreciationMethod.AddAttribute(render, "text", 1);
			
			ListStore list = new ListStore(typeof(Gdk.Pixbuf),typeof(string),typeof(DepreciationMethod));
			list.AppendValues(new Gdk.Pixbuf(null,"degressive.png"),"Degressive",DepreciationMethod.Degressive);
			list.AppendValues(new Gdk.Pixbuf(null,"linear.png"),"Linear",DepreciationMethod.Linear);
			list.AppendValues(new Gdk.Pixbuf(null,"progressive.png"),"Progressive",DepreciationMethod.Progressive);
			itemEditDepreciationMethod.Model = list;
			
			// add columns and model to the label combobox
			render = new CellRendererPixbuf ();
			itemEditLabelMethod.PackStart(render, false);
			itemEditLabelMethod.AddAttribute(render, "pixbuf", 0);
			render = new CellRendererText ();
			itemEditLabelMethod.PackStart(render, true);
			itemEditLabelMethod.AddAttribute(render, "text", 1);
			
			list = new ListStore(typeof(Gdk.Pixbuf),typeof(string),typeof(DepreciationMethod));
			list.AppendValues(null,"Not Labelable",LabelMethod.None);
			list.AppendValues(null,"Printed Labels",LabelMethod.Print);
			list.AppendValues(null,"Painted Labels",LabelMethod.Paint);
			itemEditLabelMethod.Model = list;
			
			purchaseDatePicker = new PurchaseDatePicker();
			purchaseDatePicker.DateChanged += OnPickerDateChanged;
			itemEditShowCalendarIcon.Pixbuf = new Gdk.Pixbuf(null,"calendar.png");
			
			// create the images columns
			TreeViewColumn col = new TreeViewColumn ();
			col.Title = "images";
			render = new CellRendererToggle ();
			(render as CellRendererToggle).Toggled += OnMainImageToggle;
			(render as CellRendererToggle).Radio = true;
			col.PackStart(render, false);
			col.AddAttribute(render, "active", 1);			
			render = new CellRendererPixbuf ();
			col.PackStart       (render, false);
			col.AddAttribute(render, "pixbuf", 2);			
			render = new CellRendererText ();
			(render as CellRendererText).Editable = true;
			(render as CellRendererText).Edited += OnItemImageMemoEdited;
			render.EditingStarted += OnItemImageMemoEditingStarted;
			col.PackStart       (render, true);
			col.AddAttribute(render, "text", 3);
			col.AddAttribute(render, "foreground-gdk",4);
			itemEditImages.AppendColumn(col);
			
			itemEditImages.HeadersVisible = false;			
			TargetEntry target = new TargetEntry("text/plain",TargetFlags.OtherApp,0);
			TargetEntry[] targetArray = new TargetEntry[1];
			targetArray[0] = target;
			itemEditImages.EnableModelDragDest(targetArray, Gdk.DragAction.Copy);
			itemEditImages.DragDataReceived += OnItemImageDragDataReceived;
						
			// create the tags cells
			render = new CellRendererText ();
			itemEditTagCell = render;
			(render as CellRendererText).Editable = true;
			(render as CellRendererText).Edited += OnItemTagNameEdited;
			render.EditingStarted += OnStartItemTagEdit;
			itemEditTags.PackStart (render, false);
			itemEditTags.AddAttribute(render, "text", 1);
			
			// create the tag completion
			itemTagCompletion = new ItemTagChooser();
			itemTagCompletion.MatchFunc = ItemTabCompletionMatch;
			itemTagCompletion.MinimumKeyLength = 0;
			itemTagCompletion.PopupSetWidth = false;
			render = new CellRendererText ();
			itemTagCompletion.PackStart (render, true);
			itemTagCompletion.AddAttribute(render, "text", 1);
			
			// create the popup menues
			uiManager.AddUiFromResource("item_edit_box_menues.xml");
			itemImagePopup = (Menu) uiManager.GetWidget("/itemImagePopup");
        	itemTagPopup = (Menu) uiManager.GetWidget("/itemTagPopup");
		}
		
		public void EditItem(Model.Item item)
		{
			if(ShowMe != null)
				ShowMe(this,new ShowMeEventArgs());
			
			if(currentlyEditedItem != null)
				item.Images.PropertyChanged -= OnMainImageChanged;
			
			currentlyEditedItem = item;
			
			//set the right item type
			ListStore typeModel = new ListStore(typeof(string));
			typeModel.AppendValues(item.ItemType);
			itemEditType.Model = typeModel;
			TreeIter iter;
			typeModel.GetIterFirst(out iter);
			itemEditType.SetActiveIter(iter);
			
			// set the images
			itemEditImages.Model = new TreeModelAdapter( new ImagesModel( item.Images ) );
			// set a callback to see if the mainimage was changed
			item.Images.PropertyChanged += OnMainImageChanged;
			
			// set the tags
			itemEditTags.Model = new TreeModelAdapter( new ItemTagsModel( item.Tags ) );
			itemEditTags.Model.RowInserted += OnTagRowInserted;
			
			// fill the entries etc with the item's data
			itemEditName.Text = item.Name.EmptyIfNull();
			itemEditMemo.Text = item.Memo.EmptyIfNull();
			itemEditId.Value  = item.Id;
			
			if(item is Product)
				EditProduct((Product)item);
			else if(item is Estate)
				EditEstate((Estate)item);
			else if(item is Room)
				EditRoom((Room)item);
		}
		
		private void EditProduct(Product product) {
			itemEditCategoryBook.ShowTabByWidget(itemEditProductAlign);
			
			itemEditBrand.Text        = product.Brand.EmptyIfNull();
			itemEditModel.Text        = product.Model.EmptyIfNull();
			itemEditBarcode.Text      = product.Serial.EmptyIfNull();
			itemEditWeight.Value      = product.Weight;
			itemEditWarranty.Value    = product.Warranty.Months;
			itemEditPurchase.Text     = (product.PurchaseDate == null)?"":((DateTime)product.PurchaseDate).ToString("d");
			itemEditCost.Value        = product.Cost;
			itemEditUsefulLife.Value  = product.UsefulLife.Months;
			itemEditAmount.Value      = product.PurchasedAmount;
			itemEditOwner.Active      = product.Owner;
			
			itemEditLabelMethod.Model.Foreach(delegate(TreeModel m, TreePath p, TreeIter i){
				LabelMethod method = (LabelMethod)m.GetValue(i,2);
				if(method == product.LabelMethod){
					itemEditLabelMethod.SetActiveIter(i);
					return true;
				}
				return false;
			});
			
			itemEditDepreciationMethod.Model.Foreach(delegate(TreeModel m, TreePath p, TreeIter i){
				DepreciationMethod method = (DepreciationMethod)m.GetValue(i,2);
				if(method == product.DepreciationMethod){
					itemEditDepreciationMethod.SetActiveIter(i);
					return true;
				}
				return false;
			});
		}
		
		private void EditEstate(Estate estate) {
			itemEditCategoryBook.ShowTabByWidget(itemEditEstateAlign);
			
			itemEditZip.Text     = estate.Zip;
			itemEditCity.Text    = estate.City;
			itemEditStreet.Text  = estate.Street;
			itemEditCountry.Text = estate.Country;
		}
		
		private void EditRoom(Room room) {
			itemEditCategoryBook.ShowTabByWidget(itemEditRoomAlign);
		}
		
		private void OnMainImageChanged(object sender, PropertyChangedEventArgs args){
			if(args.PropertyName == "MainImage"){
				itemEditImages.Model.Foreach(delegate(TreeModel model, TreePath path, TreeIter iter){
					itemEditImages.Model.EmitRowChanged(path,iter);
					return false;
				});
			}
		}
		
		private void ItemPropertyChanged(Widget w, Model.Item item){
			if(object.ReferenceEquals(w,itemEditName)) 
				item.Name = itemEditName.Text;
			else if(object.ReferenceEquals(w,itemEditMemo))
				item.Memo = itemEditMemo.Text;
			else if(item is Product)
				ProductPropertyChanged(w,(Product)item);
			else if(item is Room)
				RoomPropertyChanged(w,(Room)item);	
			else if(item is Estate)
				EstatePropertyChanged(w,(Estate)item);
		}
		
		private void ProductPropertyChanged(Widget w, Product product){
			if(object.ReferenceEquals(w,itemEditBrand))     
				product.Brand = itemEditBrand.Text;
			else if(object.ReferenceEquals(w,itemEditModel))     
				product.Model = itemEditModel.Text;
			else if(object.ReferenceEquals(w,itemEditBarcode))   
				product.Serial = itemEditBarcode.Text;
			else if(object.ReferenceEquals(w,itemEditWarranty)){
				RelativeTimeSpan span = new RelativeTimeSpan();
				span.Months = itemEditWarranty.ValueAsInt;
				product.Warranty = span;
			}
			else if(object.ReferenceEquals(w,itemEditOwner)){
				bool s = itemEditOwner.Active;
				itemEditAmount.Sensitive = s;
				itemEditUsefulLife.Sensitive = s;
				itemEditCost.Sensitive = s;
				itemEditWarranty.Sensitive = s;
				itemEditPurchase.Sensitive = s;
				itemEditDepreciationMethod.Sensitive = s;
				itemEditLabelMethod.Sensitive = s;
				itemEditShowCalendar.Sensitive = s;
				product.Owner = s;
			}				
			else if(object.ReferenceEquals(w,itemEditWeight)){
				product.Weight = itemEditWeight.Value;}
			else if(object.ReferenceEquals(w,itemEditAmount))    
				product.PurchasedAmount = (uint)itemEditAmount.ValueAsInt;
			else if(object.ReferenceEquals(w,itemEditUsefulLife)){
				RelativeTimeSpan span = new RelativeTimeSpan();
				span.Months = itemEditUsefulLife.ValueAsInt;
				product.UsefulLife = span;
			}
			else if(object.ReferenceEquals(w,itemEditCost))      
				product.Cost = itemEditCost.Value;
			else if(object.ReferenceEquals(w,itemEditPurchase)){
				try {
					product.PurchaseDate = DateTime.Parse(itemEditPurchase.Text);
				} catch {
					product.PurchaseDate = null;
				}
			}
			else if(object.ReferenceEquals(w,itemEditDepreciationMethod)){
				TreeIter iter;
				itemEditDepreciationMethod.GetActiveIter(out iter);
				product.DepreciationMethod = (DepreciationMethod)itemEditDepreciationMethod.Model.GetValue(iter,2);
			}
			else if(object.ReferenceEquals(w,itemEditLabelMethod)){
				TreeIter iter;
				itemEditLabelMethod.GetActiveIter(out iter);
				product.LabelMethod = (LabelMethod)itemEditLabelMethod.Model.GetValue(iter,2);
			}
		}
		
		private void EstatePropertyChanged(Widget w, Estate estate){
			if(object.ReferenceEquals(w,itemEditZip))	  
				estate.Zip = itemEditZip.Text;
			else if(object.ReferenceEquals(w,itemEditCity))      
				estate.City = itemEditCity.Text;
			else if(object.ReferenceEquals(w,itemEditStreet))    
				estate.Street = itemEditStreet.Text;
			else if(object.ReferenceEquals(w,itemEditCountry))   
				estate.Country = itemEditCountry.Text;
		}
		
		private void RoomPropertyChanged(Widget w, Room room)
		{}
		
	    private void OnItemImageDragDataReceived(object o, DragDataReceivedArgs args){
	        string fileUri = System.Text.Encoding.Default.GetString(args.SelectionData.Data);
	        Uri uri = new Uri(fileUri);
			Model.Image img = currentlyEditedItem.Images.New(uri.LocalPath);
			currentlyEditedItem.Images.Positions.Add(img);
	    }
	    
		private void OnAddItemImage(object widget, EventArgs args){
			Gtk.FileChooserDialog fc=
				new Gtk.FileChooserDialog("Choose an image for the item",
			                            (Gtk.Window)this.Toplevel,
			                            FileChooserAction.Open,
			                            "Cancel",ResponseType.Cancel,
			                            "Add Image",ResponseType.Accept);
	
			if (fc.Run() == (int)ResponseType.Accept){
				Model.Image img = currentlyEditedItem.Images.New(fc.Filename);
				currentlyEditedItem.Images.Positions.Add(img);
			}
			//Don't forget to call Destroy() or the FileChooserDialog window won't get closed.
			fc.Destroy();
		}
		
		private void OnRemoveItemImage(object widget, EventArgs args){
			// get the currently selected item
			TreeModel filter;
			TreeIter iter;
			if(itemEditImages.Selection.GetSelected(out filter, out iter) == false){
				Console.WriteLine("No image selected that could be deleted");
				return;
			}
			TreePath path = filter.GetPath(iter);
			Model.Image img = (Model.Image)filter.GetValue(iter, 0);
			
			currentlyEditedItem.Images.Positions.RemoveAt(path.Indices[path.Depth-1]);
			currentlyEditedItem.Images.Remove(img);
		}
		
		private void OnItemImageMemoEditingStarted(object o, EditingStartedArgs args) {
			TreeModel filter = itemEditImages.Model;
			TreeIter iter;
			filter.GetIter(out iter, new TreePath(args.Path));
			((Entry)args.Editable).Text = ((Model.Image)filter.GetValue(iter,0)).Memo.EmptyIfNull();
		}
		
	    private void OnItemImageMemoEdited(object o, EditedArgs args) {
	        TreeModel filter = itemEditImages.Model;
	        TreeIter iter;
	        filter.GetIter (out iter, new TreePath(args.Path));
	        ((Model.Image)filter.GetValue(iter, 0)).Memo = args.NewText;
	    }
	    
		private void OnMainImageToggle(object o,ToggledArgs args) {
			TreeIter iter;
			TreeModel filter = itemEditImages.Model;
			
			filter.GetIter (out iter, new TreePath(args.Path));
			Model.Image img = (Model.Image)filter.GetValue(iter,0);	
			img.Images.MainImage = img;
		}
		
		public void OnPickerDateChanged(object sender, DateChangedEventArgs args)
		{
			Product product = (Product)currentlyEditedItem;
			product.PurchaseDate = args.Date;
			itemEditPurchase.Text = (args.Date == null)?"":((DateTime)args.Date).ToString("d");
		}
		
		private void OnItemPropertyChanged(object sender, EventArgs args) {
			ItemPropertyChanged((Widget)sender,currentlyEditedItem);
		}
		
		private void OnDisplayPurchaseDatePicker (object o, EventArgs args){
			Product product = (Product)currentlyEditedItem;
			purchaseDatePicker.PickDate(product.PurchaseDate, itemEditShowCalendar);
		}
		
		private void OnItemTagNameEdited(object o, EditedArgs args) {
			// free the model
			itemTagCompletion.Model = null;
			
			Tag newTag = Inventory.Tags.GetTagByName(args.NewText);
			
			if(newTag == null){
				Console.WriteLine("The entered tag doesn't exist yet. Therefore the value of the tag isn't changed.");
				return;
			}
			
			int pos = (new TreePath(args.Path)).Indices[0];
			currentlyEditedItem.Tags[pos].Tag = newTag;
		}
		
		private bool ItemTabCompletionMatch (EntryCompletion comp, string key, TreeIter iter) {
			string name = ((ItemTag)comp.Model.GetValue(iter,0)).Name;
			return name.ToLower().Contains(key.Trim().ToLower());
		}

		private void OnStartItemTagEdit(object o, EditingStartedArgs args){
			// get the currently edited tag
			ItemTag tag = currentlyEditedItem.Tags[(new TreePath(args.Path)).Indices[0]];
			ItemTag[] comp = currentlyEditedItem.Tags.GetUnusedTags(tag);
			itemTagCompletion.Model = new TreeModelAdapter( new ItemTagsModel( comp ));
			
			// assign the stuff to the entry
			Entry entry = (Entry)args.Editable;
			entry.Completion = itemTagCompletion;
			entry.Text = tag.Name;
		}
		
		private void OnAddItemTag(object widget, EventArgs args){
			// check if there is an unused tag that we could add
			ItemTag[] unused = currentlyEditedItem.Tags.GetUnusedTags(null);
			if(unused.Length == 0){
				Console.WriteLine("Adding a tag is impossible since there are no unused tags left.");
				return;
			}
			
			// append tag and set the standard tag to the first tag that is useable
			currentlyEditedItem.Tags.Add(unused[0]);
		}
		
		
		public void OnTagRowInserted(object o, RowInsertedArgs args){
			if(ShowMe != null)
				ShowMe(this,new ShowMeEventArgs());
			
			itemEditTags.SelectPath(args.Path);
			itemEditTags.ScrollToPath(args.Path, 0.5f, 0f);
			/* FIXME:
			 * This call fails and outputs
			 * (MyInventory:11489): Gtk-CRITICAL **: gtk_icon_view_get_cell_area: assertion `info->position < item->n_cells' failed
			 * 
			 * No clue why this is the case but it doesn't seem to harm anybody
			 */
			itemEditTags.SetCursor(args.Path,itemEditTagCell,true);
		}
		
		private void OnRemoveItemTag(object o, EventArgs args){
			// get the selection
			TreePath[] selects = itemEditTags.SelectedItems;
			if(selects == null || selects.Length == 0){
				Console.WriteLine("Can't delete tag since there is none selected");
				return;
			}
			// remove the tag, it's "single-select" and only a list
			// this explains the array acces
			currentlyEditedItem.Tags.RemoveAt(selects[0].Indices[0]);
		}
		/*
		private void OnAddItemImage(object widget, EventArgs args){		
			FileChooserDialog fc = new FileChooserDialog(
			    "Choose an Image", this.window, FileChooserAction.Open,
				"Cancel",
				ResponseType.Cancel,
				"Open",
				ResponseType.Accept
			);
			
			if (fc.Run() == (int)ResponseType.Accept) {
				AddItemImage(fc.Filename);
			}
				
			//Don't forget to call Destroy() or the FileChooserDialog window won't get closed.
			fc.Destroy();
		}
			
		private void OnRemoveItemImage(object o, EventArgs args){
			// get the currently selected image		
			TreeModel filter;
			TreeIter iter;
			if(itemEditImages.Selection.GetSelected(out filter, out iter) == false) return;
			Image img = (Image)filter.GetValue(iter, 0);
			
			// if this is the main img ...
			if(object.ReferenceEquals(currentlyEditedItem.MainImage,img)){
				// ...set the main-img to be s.th else
				currentlyEditedItem.MainImage = null;
				foreach(object[] ob in currentlyEditedItem.Images){
					if(! object.ReferenceEquals(ob[0],img)){
						currentlyEditedItem.MainImage = (Image)ob[0];
						// repaint the main-image so that all of the radio states are right
						EmitModelObjectChanged(ob[0] as Image,filter,0);
						break;
					}
				}
			}
			
			// delete all the resources that were allocated, once the image was created
			img.DeleteResources(settings.inventoryEditPath);
			
			// remove the image
			// if there was a filter this wouldn't work.
			currentlyEditedItem.Images.Remove(ref iter);
			
			// repaint the item in the itemsView the img was removed from
			EmitModelObjectChanged(currentlyEditedItem,itemsView.Model,0);
		}
		*/
		private void OnItemImagesViewPopup(object o, WidgetEventArgs args){
			if(args.Event is Gdk.EventButton){
				Gdk.EventButton e = (Gdk.EventButton) args.Event;
			
				//3 is the right mouse button
				if(e.Button == 3){
					itemImagePopup.Popup(null,null,null,e.Button, e.Time);
				}	
			}
		}
		
		private void OnItemTagsViewPopup(object o, WidgetEventArgs args){
			if(args.Event is Gdk.EventButton){
				Gdk.EventButton e = (Gdk.EventButton) args.Event;
			
				//3 is the right mouse button
				if(e.Button == 3){
					itemTagPopup.Popup(null,null,null,e.Button, e.Time);
				}	
			}
		}				
		
		private void OnRelativeTimeSpanOutput(object o, OutputArgs args){			
			SpinButton spin = (SpinButton) o;
			string text;
			
			if(spin.ValueAsInt == 0)
				text = "";
			else {
				RelativeTimeSpan span = new RelativeTimeSpan();
				span.Months = spin.ValueAsInt;
				text = span.ToString();
			}
			
			if(spin.Text != text) spin.Text = text;
			args.RetVal = 1;
		}
		
		private void OnRelativeTimeSpanInput(object o, InputArgs args){
			SpinButton spin = (SpinButton) o;
			double val;
			
			try {
				if(spin.Text == "")
					val = 0;
				else
					val = RelativeTimeSpan.Parse(spin.Text).Months;
			}
			catch {
				val = spin.Value;
			}
			
			args.NewValue = val;
			args.RetVal = 1;
		}
				
		private void OnWarrantyOutput(object o, OutputArgs args){
			SpinButton spin = (SpinButton) o;
			if(spin.Value < 0) {
				string text;
				text = "∞";
				if(spin.Text != text) spin.Text = text;
				args.RetVal = 1;
			}
			else OnRelativeTimeSpanOutput(o,args);
		}
		
		private void OnWarrantyInput(object o, InputArgs args) {
			SpinButton spin = (SpinButton) o;
			if(spin.Text == "∞" || spin.Text == "infinite"){
				args.NewValue = -1;
				args.RetVal = 1;
			}
			else OnRelativeTimeSpanInput(o,args);
		}
		
		private void OnDescriptiveSpinButtonOutput(object o, OutputArgs args){
			if(Math.Round(((SpinButton)o).Value,4) == 0){
				((Entry)o).Text = "";
				args.RetVal = 1;
			}
			else args.RetVal = 0;
		}
		
		private void OnExposeDescriptionEntry (object entry, ExposeEventArgs args) {
			if(DrawDescriptionEntry != null){
				string desc = "no desc";
				
				if(object.ReferenceEquals(entry, itemEditName)) desc = "Name";
				else if(object.ReferenceEquals(entry, itemEditMemo)) desc = "Memo";
				else if(object.ReferenceEquals(entry, itemEditBarcode)) desc = "Identification Code";
				else if(object.ReferenceEquals(entry, itemEditBrand)) desc = "Brand";
				else if(object.ReferenceEquals(entry, itemEditModel)) desc = "Model";
				else if(object.ReferenceEquals(entry, itemEditZip)) desc = "Zip";
				else if(object.ReferenceEquals(entry, itemEditCity)) desc = "City";
				else if(object.ReferenceEquals(entry, itemEditStreet)) desc = "Street";
				else if(object.ReferenceEquals(entry, itemEditCountry)) desc = "Country";
				else if(object.ReferenceEquals(entry, itemEditPurchase)) desc = "Date of purchase";
				else if(object.ReferenceEquals(entry, itemEditWarranty)) desc = "Warranty in";
				else if(object.ReferenceEquals(entry, itemEditWeight)) desc = "Weight in";
				else if(object.ReferenceEquals(entry, itemEditCost)) desc = "Asset Cost in";
				else if(object.ReferenceEquals(entry, itemEditUsefulLife)) desc = "Useful Life in";
				else if(object.ReferenceEquals(entry, itemEditAmount)) desc = "Purchased";
				
				DrawDescriptionEntry(entry,new DrawDescriptionEntryEventArgs(desc,args.Event.Window));
			}
		}
		
		private readonly Items Items;
		private Inventory Inventory {
			get {
				return Items.Inventory;
			}			
		}
		
		public event ShowMeEventHandler ShowMe;		
		public event DrawDescriptionEntryEventHandler DrawDescriptionEntry;
		
		private Model.Item currentlyEditedItem;
		
		private CellRenderer itemEditTagCell;
		private EntryCompletion itemTagCompletion;
		
        private Menu itemImagePopup;
	    private Menu itemTagPopup;
		
		[Builder.Object] private SpinButton itemEditId;
		[Builder.Object] private ComboBox itemEditType;
		[Builder.Object] private Entry itemEditName;
		[Builder.Object] private Entry itemEditMemo;		
		[Builder.Object] private IconView itemEditTags;
		[Builder.Object] private TreeView itemEditImages;
		[Builder.Object] private Notebook itemEditCategoryBook;
		[Builder.Object] private   Alignment itemEditProductAlign;
		[Builder.Object] private     Entry itemEditBarcode;
		[Builder.Object] private     Entry itemEditModel;
		[Builder.Object] private     Entry itemEditBrand;
		[Builder.Object] private     CheckButton itemEditOwner;
		[Builder.Object] private     Entry itemEditPurchase;
		[Builder.Object] private     ComboBox itemEditDepreciationMethod;
		[Builder.Object] private     Button itemEditShowCalendar;
		[Builder.Object] private       Gtk.Image itemEditShowCalendarIcon;
		private                        PurchaseDatePicker purchaseDatePicker;
		[Builder.Object] private     SpinButton itemEditWarranty;
		[Builder.Object] private     SpinButton itemEditWeight;
		[Builder.Object] private     SpinButton itemEditCost;
		[Builder.Object] private     SpinButton itemEditUsefulLife;
		[Builder.Object] private     SpinButton itemEditAmount;
		[Builder.Object] private     ComboBox itemEditLabelMethod;
		[Builder.Object] private   Alignment itemEditEstateAlign;
		[Builder.Object] private     Entry itemEditZip;
		[Builder.Object] private     Entry itemEditCity;
		[Builder.Object] private     Entry itemEditStreet;
		[Builder.Object] private     Entry itemEditCountry;
		[Builder.Object] private   Alignment itemEditRoomAlign;
    }
}