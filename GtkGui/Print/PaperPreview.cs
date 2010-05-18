
using System;
using Cairo;
using Pango;
using Gtk;
using Gdk;
using System.Collections;
using MyInventory.Model;
using System.ComponentModel;

namespace MyInventory.GtkGui
{
	public abstract class PaperPreview : DrawingArea
	{
		public PaperPreview()
		{
			ExposeEvent += OnExpose;
		}
		
		protected abstract void OnLayoutChanged(object o,PropertyChangedEventArgs args);		
		protected abstract void OnExpose(object o, ExposeEventArgs args);
		                 
		protected void ScaleFactors(out double x, out double y) {
			x = Gdk.Screen.Default.Height / Gdk.Screen.Default.HeightMm;
			y = Gdk.Screen.Default.Width / Gdk.Screen.Default.WidthMm;
		}
		
		protected void GetInnerRegion(ref double x, ref double y, ref double w, ref double h){
			x += 1;
			y += 1;
			w = w-2-shadowOffset;
			h = h-2-shadowOffset;
		}
		
		protected void DrawPaper(Cairo.Context cr, double x, double y, double w, double h){
			// make the background white with a line around
			cr.Rectangle(x+shadowOffset*2,y+shadowOffset*2,w-shadowOffset*2,h-shadowOffset*2);
			cr.Color = shadowColor;
			cr.Fill();
			GetInnerRegion(ref x,ref y,ref w,ref h);
			cr.Rectangle(x,y,w,h);
			cr.Color  = new Cairo.Color(0, 0, 0);
			cr.StrokePreserve();
			cr.Color  = new Cairo.Color(1, 1, 1);
			cr.Fill();
		}
		
		protected void DrawVerticalSectionIndicator(string text, Cairo.Context cr, Pango.Layout layout, double x, double y, out double w, double h){	
			cr.MoveTo(x,y);
			cr.LineTo(x+SectionSerifeWidth*2,y);
			cr.MoveTo(x+SectionSerifeWidth,y);
			cr.LineTo(x+SectionSerifeWidth,y+h);
			cr.MoveTo(x,y+h);
			cr.LineTo(x+SectionSerifeWidth*2,y+h);
			cr.Color = new Cairo.Color(0, 0, 0);
			cr.LineWidth = 1;
			layout.Alignment = Pango.Alignment.Left;
			layout.SetText (text);
            layout.Ellipsize = EllipsizeMode.Middle;
			layout.Justify = true;
			int lh,lw;
			layout.GetPixelSize(out lw, out lh);
			w=(double)lw+SectionSerifeWidth*2;
			cr.Color = new Cairo.Color(0, 0, 0);
			cr.MoveTo(x+SectionSerifeWidth*2,y+(h-lh)/2);
			Pango.CairoHelper.ShowLayout (cr, layout);
		}

		protected void DrawHorizontalSectionIndicator(string text, Cairo.Context cr, Pango.Layout layout, double x, double y, double w, out double h){	
			cr.MoveTo(x,y);
			cr.LineTo(x,y+SectionSerifeWidth*2);
			cr.MoveTo(x,y+SectionSerifeWidth);
			cr.LineTo(x+w,y+SectionSerifeWidth);
			cr.MoveTo(x+w,y);
			cr.LineTo(x+w,y+SectionSerifeWidth*2);
			cr.Color = new Cairo.Color(0, 0, 0);
			cr.LineWidth = 1;
			cr.Stroke();
			
			layout.Width = (int)(w*Pango.Scale.PangoScale);
			layout.Alignment = Pango.Alignment.Center;
			layout.SetText (text);
            layout.Ellipsize = EllipsizeMode.Middle;
			layout.Justify = true;
			cr.Color = new Cairo.Color(0, 0, 0);
			cr.MoveTo(x,y+SectionSerifeWidth*2);
			Pango.CairoHelper.ShowLayout (cr, layout);
			
			int lw,lh;
			layout.GetPixelSize(out lw, out lh);
			h=(double)lh+SectionSerifeWidth*2;
		}
		
		private const double SectionSerifeWidth = 4;
		private readonly Cairo.Color shadowColor = new Cairo.Color(148.0/255.0,147.0/255.0,121.0/255.0);
		private const double shadowOffset = 2;
	}
	
	class PagePreview : PaperPreview
	{
		public PagePreview(Model.Settings settings)
		{
			this.settings = settings;
			settings.PageLayout.PropertyChanged += OnLayoutChanged;
			UpdateSize();
		}
		
		protected override void OnLayoutChanged(object o,PropertyChangedEventArgs args) {
			QueueDraw();
		}
		
		private void UpdateSize(){
			double sx,sy;
			ScaleFactors(out sx, out sy);
			double w = paperWidth*sx+sectionWidth;
			double h = paperWidth*pageHeightPerWidth*sy+sectionHeight;
			SetSizeRequest((int)w, (int)h);
		}
		
		protected override void OnExpose(object o, ExposeEventArgs args)
		{
			using(Cairo.Context cr = Gdk.CairoHelper.Create(GdkWindow)){
				// make the background white with a line around
				double x = 0;
				double y = 0;
				double w = Allocation.Width  -sectionWidth;
				double h = Allocation.Height -sectionHeight;
				Pango.Layout pl = new Pango.Layout(this.PangoContext);
				
				DrawPaper(cr,x,y,w,h);
				
				ArrayList labels = new ArrayList();
				for(int i=0;i < settings.PageLayout.LabelsPerPage;++i){
					labels.Add( new PseudoLabelRenderer() );
				}
				
				GetInnerRegion(ref x,ref y,ref w,ref h);
				
				PageLayout layout = new PageLayout();
				layout.LabelRepeatX = settings.PageLayout.LabelRepeatX;
				layout.LabelRepeatY = settings.PageLayout.LabelRepeatY;
				layout.PaddingX = settings.PageLayout.PaddingX;				
				layout.PaddingY = settings.PageLayout.PaddingY;
				layout.LabelWidth = (w-2*layout.PaddingX-labelPadding*(layout.LabelRepeatX-1)) / layout.LabelRepeatX;
				layout.LabelHeight = (h-2*layout.PaddingY-labelPadding*(layout.LabelRepeatY-1)) / layout.LabelRepeatY;
				
				PageRenderer pr = new PageRenderer(labels,layout);
				pr.Render(cr,pl,x,y,w,h);
				
				//double nn;
				//DrawVerticalSectionIndicator(settings.PageLayout.LabelRepeatY.ToString()+"x",cr,pl,x+w+sectionPadding,y,out nn,h);
				//DrawHorizontalSectionIndicator(settings.PageLayout.LabelRepeatX.ToString()+"x",cr,pl,x,y+h+sectionPadding,w,out nn);
			}
		}
		
		private readonly double labelPadding = 2;
		private readonly double paperWidth = 65; 	// mm
		private readonly double sectionWidth = 80;
		private readonly double sectionHeight = 40;
		private readonly double sectionPadding = 10;
		private readonly double pageHeightPerWidth = Math.Sqrt(2);
		private Model.Settings settings;
	}
	
	class LabelPreview : PaperPreview
	{		
		public LabelPreview(Model.Settings settings)
		{
			this.settings = settings;
			settings.PageLayout.PropertyChanged += OnLayoutChanged;
			UpdateSize();
		}
		
		private void UpdateSize(){
			double x,y;
			ScaleFactors(out x, out y);
			SetSizeRequest((int)(paperWidth*x +sectionWidth),
			               (int)(paperWidth*labelHeightPerWidth*y +sectionHeight));
		}
		
		protected override void OnLayoutChanged(object o, PropertyChangedEventArgs args) {
			UpdateSize();
			QueueDraw();
		}
		
		protected override void OnExpose(object o, ExposeEventArgs args)
		{
			using(Cairo.Context cr = Gdk.CairoHelper.Create(GdkWindow)){
				
				Console.WriteLine();
				
				double x = 0;
				double y = 0;
				double w = Allocation.Width  -sectionWidth;
				double h = Allocation.Height -sectionHeight;
				DrawPaper(cr,x,y,w,h);
				GetInnerRegion(ref x,ref y,ref w,ref h);
				
				Pango.Layout pl = new Pango.Layout(this.PangoContext);
				
				double nn;
				DrawVerticalSectionIndicator(settings.PageLayout.LabelHeight.ToString("N1")+"mm",cr,pl,x+w+sectionPadding,y,out nn,h);
				DrawHorizontalSectionIndicator(settings.PageLayout.LabelWidth.ToString("N1")+"mm",cr,pl,x,y+h+sectionPadding,w,out nn);
				
				LabelRenderer l = new LabelRenderer(settings.LabelLayout);
				int labelPadding = 3;
				l.Render(cr,pl,
				         x+labelPadding,y+labelPadding,
				         w-2*labelPadding,
				         h-2*labelPadding);
			}
		}
		
		private readonly double paperWidth = 65; 	// mm
		private readonly double labelHeightPerWidth = 0.5;
		private Model.Settings settings;
		private const double labelPadding = 3;
		private readonly double sectionWidth = 80;
		private readonly double sectionHeight = 40;
		private readonly double sectionPadding = 10;
	}
}
