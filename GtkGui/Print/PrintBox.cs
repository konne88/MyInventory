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

// Infos on how to print
// http://www.mail-archive.com/gtk-sharp-list@lists.ximian.com/msg03762.html

using System.Runtime.InteropServices;
using System;
using Gtk;
using Gdk;
using MyInventory.Model;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Cairo;

namespace MyInventory.GtkGui {
	class LabelPreview : DrawingArea
	{		
		public LabelPreview()
		{
			ExposeEvent += OnExpose;
		}
		
		protected void OnExpose(object o, ExposeEventArgs args)
		{
			using(Context cr = CairoHelper.Create(GdkWindow)){
				Label l = new Label(new LabelSettings());
				l.Description = "Label Test Item\nBed, Home, Time, Tracking";
				l.InventoryAbbreviation = "KWTZ";
				l.Id = 1337;
				
				l.Render(cr,new Pango.Layout(this.PangoContext),0,0,250,150);
			}
		}
	}
	
	public class PrintBox : VBox
    {
		private PrintBox(Builder builder)
		: base(builder.GetRawObject("printBox"))
		{
			builder.Autoconnect (this);
		}
		
        public PrintBox (Inventory inv, UIManager uiManager)
		: this(new Builder("print_box.ui"))
        {
			Inventory = inv;
			
			LabelPreview labelPreview = new LabelPreview();
			labelPreview.SetSizeRequest(400,200);
			printSettings.PackStart(labelPreview);
		}
		
		private void OnPageSetup(object sender, EventArgs args){
			//Print.RunPageSetupDialog((Gtk.Window)this.Toplevel, Printing.DefaultPageSetup, Printing.PrintSettings);
		}
		
		private void OnPrint(object sender, EventArgs arguments){	
			PrintOperation Printing = new PrintOperation();
			List<Model.Item> printItems = new List<Model.Item>();
			
			foreach(Model.Item item in Inventory.Items){
				Product p = item as Product;
				if(p != null && p.LabelMethod == LabelMethod.Print){
					printItems.Add(p);
				}
			}
			
			double w = 2.5;
			double h = 1.25;
			double paddingX = 3 / 25.4;
			double paddingY = 3 / 25.4;
			int repeatX = 2;
			int repeatY = 4;
			
			int printingItem = 0;
			
			Printing.BeginPrint += delegate (object obj, Gtk.BeginPrintArgs args)
			{
				Printing.NPages = (int)Math.Ceiling( (double)printItems.Count/(repeatX*repeatY));
			};
			
            Printing.DrawPage += delegate (object obj, Gtk.DrawPageArgs args)
			{
				PrintContext context = args.Context;
			
				using(Context cr = context.CairoContext){
					Pango.Layout layout = context.CreatePangoLayout();
					
					Label l = new Label(new LabelSettings());
					l.InventoryAbbreviation = "KWTZ";
					
					for(int x=0 ; x<repeatX ; ++x){				
						for(int y=0 ; y<repeatY ; ++y){
							if(printingItem >= printItems.Count)
								return;
							
							Model.Item item = printItems[printingItem];
							l.Description = item.GetDescription().ToString(2);
							l.Id = item.Id;
							l.Render(cr,layout,
							         x*(w+paddingX) * context.DpiX,
							         y*(h+paddingY) * context.DpiY,
							         w * context.DpiX,
							         h * context.DpiY);
							++printingItem;
						}
					}
				}
			};
			
			Printing.Run (PrintOperationAction.PrintDialog, (Gtk.Window)this.Toplevel);
		}
		
		private readonly Inventory Inventory;
		
		[Builder.Object] private Box printSettings;
		[Builder.Object] private Button PrintButton;
    }
}