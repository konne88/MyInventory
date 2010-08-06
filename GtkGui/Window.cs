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
using System.Collections.ObjectModel;

namespace MyInventory.GtkGui {
    public class Window : Gtk.Window
    {
		private Window(Builder builder)
		: base(builder.GetRawObject("myInventoryWindow"))
		{
			builder.Autoconnect (this);
			
			DeleteEvent += OnWindowDelete;
		}
		
        public Window (Inventory inventory)
		: this(new Builder("window.ui"))
        {
			Inventory = inventory;
			
			// load the menues and toolbars
			uiManager = new UIManager();
			
			// create the file actions
			Gtk.Action saveInventoryAction = new Gtk.Action("saveFile","Save","Save the active inventory",Stock.Save);
			saveInventoryAction.Activated += OnSaveInventory;
			Gtk.Action printLabelsAction = new Gtk.Action("printLabels","Print Labels","Print labels for items.",Stock.Print);
			printLabelsAction.Activated += OnPrintLabels;
			Gtk.Action quitAction = new Gtk.Action("quit","Quit","Quit the application",Stock.Quit);
			quitAction.Activated += OnQuit;
			Gtk.Action fileAction = new Gtk.Action("file","File");
			ActionGroup fileActionGroup = new ActionGroup("file");
			fileActionGroup.Add(saveInventoryAction);
			fileActionGroup.Add(printLabelsAction);
			fileActionGroup.Add(quitAction);
			fileActionGroup.Add(fileAction);
			uiManager.InsertActionGroup(fileActionGroup,0);	
			
			// create items box
			itemsBox = new ItemsBox(inventory.Items, uiManager);
			itemsBox.DrawDescriptionEntry += OnDrawDescriptionEntry;
			itemsBox.ShowMe += OnShowItemsBox;
			itemsAlign.Add(itemsBox);
			
			// create locations box
			locationsBox = new LocationsBox(inventory.Locations, uiManager);
			locationsBox.DrawDescriptionEntry += OnDrawDescriptionEntry;
			locationsBox.ShowMe += OnShowLocationsBox;
			locationsBox.GotoItem += OnGotoLocationsItem;
			locationsAlign.Add(locationsBox);
			
			// create tags box
			tagsBox = new TagsBox(inventory.Tags, uiManager);
			tagsBox.ShowMe += OnShowTagsBox;
			tagsAlign.Add(tagsBox);

			// create tool and menubar
			uiManager.AddUiFromResource("window_menues.xml");
			menuBar = (MenuBar) uiManager.GetWidget("/menuBar");
			toolbar = (Toolbar) uiManager.GetWidget("/toolbar");
			toolbar.IconSize = IconSize.LargeToolbar;
			toolbar.ToolbarStyle = ToolbarStyle.Both;
			
			mainBox.PackStart(menuBar,false,true,0);
			mainBox.PackStart(toolbar,false,true,0);
			
			// laod category icons
			itemsTabImage.Pixbuf = ((GtkSettings)Inventory.Settings).ItemsTabIcon;
			locationsTabImage.Pixbuf = ((GtkSettings)Inventory.Settings).LocationsTabIcon;
			tagsTabImage.Pixbuf = ((GtkSettings)Inventory.Settings).TagsTabIcon;
			
			this.Icon = ((GtkSettings)Inventory.Settings).WindowIcon;
			this.Resize(((GtkSettings)Inventory.Settings).MainWindowWidth,((GtkSettings)Inventory.Settings).MainWindowHeight);
        }
		
		private void End(){
			/*new MessageDialog(this,DialogFlags.Modal,MessageType.Question,
                                  GTK_MESSAGE_ERROR,
                                  GTK_BUTTONS_CLOSE,
                                  "Error loading file '%s': %s",
                                  filename, g_strerror (errno));
 gtk_dialog_run (GTK_DIALOG (dialog));
 gtk_widget_destroy (dialog);

			
			*/
			Inventory.Settings.Save();
			Application.Quit();
		}
		
		private void OnSaveInventory(object o, EventArgs a) 
		{
			Inventory.Settings.Save();
			Inventory.Save();
			Console.WriteLine("Saved");
		}

		private void OnPrintLabels(object o, EventArgs a) 
		{
			// create print dialog
			if(printDialog == null){
				printDialog = new PrintDialog(Inventory, uiManager);
			}
			printDialog.ShowAll();
		}
		
		private void OnQuit(object o, EventArgs args){
			End();
		}
		
		private void OnShowLocationsBox(object sender, ShowMeEventArgs args){
			categoryBook.ShowTabByWidget(locationsAlign);			
		}
				
		private void OnGotoLocationsItem (object entry, GotoItemEventArgs args) {
			itemsBox.SelectItem(args.Item);
		}
		
		private void OnShowItemsBox(object sender, ShowMeEventArgs args){
			categoryBook.ShowTabByWidget(itemsAlign);			
		}

		private void OnShowTagsBox(object sender, ShowMeEventArgs args){
			categoryBook.ShowTabByWidget(tagsAlign);			
		}
		
		private void OnWindowDelete (object o, DeleteEventArgs args) 
		{
			End();
			args.RetVal = true;
		}
		
		private void OnDrawDescriptionEntry (object sender, DrawDescriptionEntryEventArgs args) {
			Entry entry = (Entry)sender;
			if(entry.Text == ""){
				//int x,y;
				//entry.GetLayoutOffsets(out x, out y);
				string desc = args.Description;
				Gdk.Window win = args.Window;
				Pango.Layout layout = entry.CreatePangoLayout(desc);
				Gdk.GC gc = new Gdk.GC(win);
				// set the color
		        Gdk.Color color = ((GtkSettings)(Inventory.Settings)).EntryDescriptionColor;
		        Gdk.Colormap colormap = Gdk.Colormap.System;
		        colormap.AllocColor (ref color, true, true);
		        gc.Foreground = color;
				// draw the text (x and y are just good guesses on my system).		
				win.DrawLayout(gc,2,1,layout);
			}
		}
		
		private readonly Inventory Inventory;
		
		private UIManager uiManager;
		private MenuBar menuBar;
		private Toolbar toolbar;
		
		[Builder.Object] private Box mainBox;
        [Builder.Object] private   Notebook categoryBook;
		[Builder.Object] private     Gtk.Image itemsTabImage;
		[Builder.Object] private     Alignment itemsAlign;
		private                        ItemsBox itemsBox;
		[Builder.Object] private     Gtk.Image locationsTabImage;
		[Builder.Object] private     Alignment locationsAlign;
		private                        LocationsBox locationsBox;
		[Builder.Object] private     Gtk.Image tagsTabImage;
		[Builder.Object] private     Alignment tagsAlign;
		private                        TagsBox tagsBox;
		private                  PrintDialog printDialog;
    }
}