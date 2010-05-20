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
using System.ComponentModel;

namespace MyInventory.GtkGui {
	public class PrintDialog : Dialog
    {
		private PrintDialog(Builder builder)
		: base(builder.GetRawObject("printDialog"))
		{
			builder.Autoconnect (this);
		}
		
        public PrintDialog(Inventory inv, UIManager uiManager)
		: this(new Builder("print_dialog.ui"))
        {
			Inventory = inv;
			
			pagePreview = new PagePreview( (GtkSettings)inv.Settings);
			pagePreviewBox.Add(pagePreview);

			labelPreview = new LabelPreview( (GtkSettings)inv.Settings );
			labelPreviewBox.Add(labelPreview);
			
			Inventory.Settings.LabelLayout.PropertyChanged += OnLayoutPropertyChanged;
			Inventory.Settings.PageLayout.PropertyChanged += OnLayoutPropertyChanged;
			
			pagePaddingX.ValueChanged += OnLayoutChanged;
			pagePaddingY.ValueChanged += OnLayoutChanged;
			labelRepeatX.ValueChanged += OnLayoutChanged;
			labelRepeatY.ValueChanged += OnLayoutChanged;
			labelWidth.ValueChanged += OnLayoutChanged;
			labelHeight.ValueChanged += OnLayoutChanged;
			
			UpdateLayoutWidgets();
		}
		
		private void UpdateLayoutWidgets() {
			pagePaddingX.Value = Inventory.Settings.PageLayout.PaddingX;
			pagePaddingY.Value = Inventory.Settings.PageLayout.PaddingY;
			labelHeight.Value = Inventory.Settings.PageLayout.LabelHeight;
			labelWidth.Value = Inventory.Settings.PageLayout.LabelWidth;
			labelRepeatX.Value = (double)Inventory.Settings.PageLayout.LabelRepeatX;
			labelRepeatY.Value = (double)Inventory.Settings.PageLayout.LabelRepeatY;
		}
		
		private void OnLayoutPropertyChanged(object s, PropertyChangedEventArgs args){
			UpdateLayoutWidgets();
		}
		
		private void OnLayoutChanged(object sender, EventArgs args){
			if(object.ReferenceEquals(sender,pagePaddingX))
				Inventory.Settings.PageLayout.PaddingX = pagePaddingX.Value;
			else if(object.ReferenceEquals(sender,pagePaddingY))
				Inventory.Settings.PageLayout.PaddingY = pagePaddingY.Value;
			else if(object.ReferenceEquals(sender,labelRepeatX))
				Inventory.Settings.PageLayout.LabelRepeatX = labelRepeatX.ValueAsInt;
			else if(object.ReferenceEquals(sender,labelRepeatY))
				Inventory.Settings.PageLayout.LabelRepeatY = labelRepeatY.ValueAsInt;
			else if(object.ReferenceEquals(sender,labelWidth))
				Inventory.Settings.PageLayout.LabelWidth = labelWidth.Value;
			else if(object.ReferenceEquals(sender,labelHeight))
				Inventory.Settings.PageLayout.LabelHeight = labelHeight.Value;
		}
		
		private void OnUnitOutput(object o, OutputArgs args){
			SpinButton spin = (SpinButton) o;
			string text = spin.Value.ToString()+"mm";
			if(spin.Text != text)
				spin.Text = text;
			args.RetVal = 1;
		}
		
		private void OnUnitInput(object o, InputArgs args) {
			SpinButton spin = (SpinButton) o;
			try {
				args.NewValue = UnitConversions.LengthToMm(spin.Text);
			} catch (FormatException) {
				args.NewValue = 0.0;
			}
			args.RetVal = 1;
		}
		
		private PageLayout GetPageLayout() {
			// calc page layout here
			PageLayout pageLayout = new PageLayout();
			/*pageLayout.PaddingX = pagePaddingX.Value;
			pageLayout.PaddingY = pagePaddingY.Value;
			pageLayout.LabelRepeatX = labelRepeatX.ValueAsInt;
			pageLayout.LabelRepeatY = labelRepeatY.ValueAsInt;
			pageLayout.LabelWidth = labelWidth.Value;
			pageLayout.LabelHeight = labelHeight.Value;*/
			return pageLayout;
		}
		
		private LabelLayout GetLabelLayout() {
			// calc label layout here
			LabelLayout labelLayout = new LabelLayout();
			return labelLayout;
		}
		
		private void OnPrint(object sender, EventArgs arguments){	
			PrintOperation printing = new PrintOperation();
			
			List<LabelRenderer> labels = new List<LabelRenderer>();
			foreach(Model.Item item in Inventory.Items){
				Product p = item as Product;
				if(p != null && p.LabelMethod == LabelMethod.Print){
					labels.Add(new LabelRenderer(item,Inventory.Settings.LabelLayout));
				}
			}
			
			int labelsPerPage = Inventory.Settings.PageLayout.LabelsPerPage;
			int currentLabel = 0;
			
			printing.BeginPrint += delegate (object obj, Gtk.BeginPrintArgs args)
			{
				printing.NPages = (int)Math.Ceiling( (double)labels.Count/(labelsPerPage));
				printing.Unit = Unit.Mm;
			};
			
            printing.DrawPage += delegate (object obj, Gtk.DrawPageArgs args)
			{
				PrintContext context = args.Context;
				
				using(Context cr = context.CairoContext){
					// scale down the cairo context
					
					// //context.DpiY,
					
					Pango.Layout layout = context.CreatePangoLayout();
					ArrayList pageLabels = new ArrayList();
					// it is impossible that we have 0 items, otherwise the page would be empty
					do {
						pageLabels.Add(labels[currentLabel]);
						++currentLabel;
					}
					while (currentLabel%labelsPerPage != 0 && currentLabel < labels.Count-1);
					
					PageRenderer page = new PageRenderer(pageLabels,Inventory.Settings.PageLayout);
					page.Render(cr,layout,0,0,context.Width,context.Height);
				}
			};
			
			printing.Run (PrintOperationAction.PrintDialog, (Gtk.Window)this.Toplevel);
		}
		
		private readonly Inventory Inventory;
		
		[Builder.Object] private Box printSettings;
		[Builder.Object] private Button PrintButton;
		[Builder.Object] private Alignment labelPreviewBox;
		[Builder.Object] private Alignment pagePreviewBox;
		                 private LabelPreview labelPreview;
		                 private PagePreview pagePreview;
		
		[Builder.Object] private SpinButton pagePaddingX;
		[Builder.Object] private SpinButton pagePaddingY;
		[Builder.Object] private SpinButton labelRepeatX;
		[Builder.Object] private SpinButton labelRepeatY;
		[Builder.Object] private SpinButton labelWidth;
		[Builder.Object] private SpinButton labelHeight;
    }
}