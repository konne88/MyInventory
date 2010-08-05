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

using Cairo;
using System;
using MyInventory.Model;
using Pango;

namespace MyInventory.GtkGui {
	public class PseudoLabelRenderer : IRenderable
    {	
		public void Render(Cairo.Context cr, Layout layout, double x, double y, double width, double height)
		{
			cr.Save();
			cr.Translate (x,y);
			
			// make the background white
			cr.Rectangle(1,1,width-2,height-2);
			cr.LineWidth = 1;
			cr.Color  = new Cairo.Color(0, 0, 0);
			cr.StrokePreserve();
			cr.Color  = new Cairo.Color(1, 1, 1);
			cr.Fill();
			cr.Restore();
		}
	}
	
	public class LabelRenderer : Label, IRenderable
    {
		public LabelRenderer(LabelLayout layout)
		: base(layout)
		{}
		
		public LabelRenderer(Model.Item item, LabelLayout layout)
		: base(item,layout)
		{}
		
		private string CalcFont(Model.LabelLayout ll,Layout layout,double heigth) {
			// the idea of this function is, to go throuth all fontsizes
			// and find an appropriate appriximation to the current screen size
			// of the text.
			// 
			// for now this is done by simply testing all sizes until
			// we find one larger than the one we need and return it
			// This is inefficient and could be done much.
			// better with a binary Search like algorythm
			
			int h = (int)(heigth*ll.TextSize);
			int sizeH;  // the heigth of the currently tried size
			int lw;         // not needed
			int size = 0;
			Layout l = layout.Copy();
			l.SetText ("f");        // some text to get the height of
			
			while(true){
				++size;
				l.FontDescription = FontDescription.FromString(ll.FontName+" "+size);
				l.GetPixelSize(out lw, out sizeH);
				// return the size
				if(sizeH > h){
   		        	return ll.FontName+" "+size;
				}
			}
		}

		public virtual void Render(Cairo.Context cr, Layout layout, double x, double y, double w, double h)
		{
			cr.Save();
			cr.Translate (x,y);
			
			// make the background white
			cr.Color  = new Cairo.Color(1, 1, 1);
			cr.Rectangle(0,0,w,h);
			cr.Fill();
			
			// calc some values
			int lw;
			string prefix = "INV-"+InventoryAbbreviation+"-";
			string idString = Id.ToString("0000000000");
			
			// basic text stuff
			layout.FontDescription = FontDescription.FromString(CalcFont(Layout,layout,h));
			layout.Width = (int)(w*Scale.PangoScale);
			layout.Alignment = Alignment.Center;
			
			// draw the description
			int descH = 0;
			if(Layout.UseDescription){
				Layout descLayout = layout.Copy();
				descLayout.SetText (Description);
				descLayout.GetPixelSize(out lw, out descH);
	            layout.Ellipsize = EllipsizeMode.Middle;
				layout.Justify = true;
				cr.Color = new Cairo.Color(0, 0, 0);
				Pango.CairoHelper.ShowLayout (cr, descLayout);
			}

			// draw the barcode	text
			int codeH = 0;
			if(Layout.UseBarcodeText){
				Layout codeLayout = layout.Copy();
				codeLayout.SetText (prefix+idString);
				codeLayout.GetPixelSize(out lw, out codeH);
	            cr.MoveTo(0,h-(double)codeH);
				cr.Color = new Cairo.Color(0, 0, 0);
				Pango.CairoHelper.ShowLayout (cr, codeLayout);
			}
			
			// draw the barcode	
			if(Layout.UseBarcode){
				double barcodeH = h-((double)codeH+(double)descH+Layout.SpacingSize*2);
				if(barcodeH>0){
					IBarcode bc = new Code128();
					bc.WriteString(prefix+idString);
					bc.WriteEnd();
					RenderBarcode(bc,cr,0,descH+Layout.SpacingSize,w,barcodeH);
				}
			}
			cr.Restore();
		}
		
		private void RenderBarcode(IBarcode bc, Cairo.Context cr, double x, double y, double width, double height)
		{
			// throws an exception if no stop was added
			double barWidth = width/(double)bc.BarcodeWidth;
			
			cr.Save();
			
			cr.LineWidth = barWidth;
			cr.Color = new Cairo.Color(0, 0, 0);
			
			x += bc.Silence*barWidth;
			foreach(bool b in bc.Bars){
				if(b){
					cr.MoveTo(x,y);
					cr.RelLineTo(0,height);
				}
				x+=barWidth;
			}
			
			cr.Stroke();	
			cr.Restore();
		}
	}
}