// infos on code 128
// http://en.wikipedia.org/wiki/Code_128

using Cairo;
using System;
using MyInventory.Model;
using Pango;

namespace MyInventory.GtkGui {	
	public class Label
    {
		public Label(LabelSettings settings){
			Settings = settings;
		}
		
		public void Render(Cairo.Context cr, Layout layout, double x, double y, double width, double height)
		{
			cr.Save();
			
			cr.Translate (x,y);
			
			// make the background white
			cr.Color  = new Cairo.Color(1, 1, 1);
			cr.Rectangle(0,0,width,height);
			cr.Fill();
			
			// calc some values
			double padding = Math.Min(width,height)*Settings.PaddingSize;
			double w = width - padding*2;
			double h = height - padding*2;
			double spacing = h*Settings.Spacing;
			int lw;
			string prefix = "INV-"+InventoryAbbreviation+"-";
			string idString = Id.ToString("0000000000");
			
			cr.Translate (padding, padding);
						
			// basic text stuff
			layout.FontDescription = FontDescription.FromString("sans 10");
			layout.Width = (int)(w*Scale.PangoScale);
			layout.Alignment = Alignment.Center;
			
			// draw the description
			int descH;
			Layout descLayout = layout.Copy();
			descLayout.SetText (Description);
			descLayout.GetPixelSize(out lw, out descH);			
            layout.Ellipsize = EllipsizeMode.Middle;
			layout.Justify = true;
			cr.Color = new Cairo.Color(0, 0, 0);
			Pango.CairoHelper.ShowLayout (cr, descLayout);
			
			// draw the barcode	text
			int codeH;
			Layout codeLayout = layout.Copy();
			codeLayout.SetText (prefix+idString);
			codeLayout.GetPixelSize(out lw, out codeH);
            cr.MoveTo(0,h-(double)codeH);
			cr.Color = new Cairo.Color(0, 0, 0);
			Pango.CairoHelper.ShowLayout (cr, codeLayout);

			// draw the barcode	
			double barcodeH = h-((double)codeH+(double)descH+spacing*2);
			if(barcodeH>0){
				Barcode bc = new Barcode();
				bc.WriteString(prefix,Barcode.Encoding.B);
				bc.WriteString(idString,Barcode.Encoding.C);
				bc.WriteEnd();
				bc.Render(cr,0,descH+spacing,w,barcodeH);
			}       
			cr.Restore();
		}
		
		public string Description;
		public uint Id;
		public LabelSettings Settings;
		public string InventoryAbbreviation;
	}
	
	public class LabelSettings {
		public double PaddingSize = 0.03;  // Percentage of the smaller edge
		public double DescriptionSize = 0.23;
		public double BarcodeSize = 0.5;
		public double BarcodeTextSize = 0.23;
		public double Spacing = 0.02;
		public string Font = "sans 10";
	}
}