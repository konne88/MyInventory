
using System;
using Cairo;
using Pango;
using Gtk;
using Gdk;
using System.Collections;
using MyInventory.Model;

namespace MyInventory.GtkGui
{
	public abstract class PaperPreview : DrawingArea
	{
		public PaperPreview()
		{
			ExposeEvent += OnExpose;
		}
		
		protected abstract void OnExpose(object o, ExposeEventArgs args);
		
		protected void GetInnerRegion(ref double x, ref double y, ref double w, ref double h){
			x += 1;
			y += 1;
			w = w-2-shadowOffset;
			h = h-2-shadowOffset;
		}
		
		protected void DrawPaper(Cairo.Context cr, double x, double y, double w, double h){
			// make the background white with a line around
			cr.Rectangle(shadowOffset*2,shadowOffset*2,w-shadowOffset*2,h-shadowOffset*2);
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
			cr.Stroke();

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
		public PagePreview(PageLayout pl)
		{
			layout = pl;
		}
		
		protected override void OnExpose(object o, ExposeEventArgs args)
		{
			using(Cairo.Context cr = Gdk.CairoHelper.Create(GdkWindow)){
				// make the background white with a line around
				double x = 0;
				double y = 0;
				double tw = this.WidthRequest;
				double th = this.HeightRequest;
				double w = tw-sectionWidth;
				double h = th-sectionHeight;
				Pango.Layout pl = new Pango.Layout(this.PangoContext);
				
				DrawPaper(cr,x,y,w,h);
				
				ArrayList labels = new ArrayList();
				for(int i=0;i<layout.LabelsPerPage;++i){
					labels.Add( new PseudoLabelRenderer() );
				}
				
				GetInnerRegion(ref x,ref y,ref w,ref h);
				PageRenderer pr = new PageRenderer(Layout,labels);
				pr.Render(cr,pl,x,y,w,h);
				
				double nn;
				
				DrawVerticalSectionIndicator("2mm",cr,pl,w+sectionPadding,y,out nn,h);
				DrawHorizontalSectionIndicator("1mm",cr,pl,x,h+sectionPadding,w,out nn);
			}
		}
		
		private readonly double sectionWidth = 100;
		private readonly double sectionHeight = 40;
		private readonly double sectionPadding = 10;
		private PageLayout layout;
		public PageLayout Layout {
			set {
				layout = value;
				QueueDraw();
			}
			get {
				return layout;
			}
		}
	}
	
	class LabelPreview : PaperPreview
	{		
		public LabelPreview(LabelLayout l)
		{
			layout = l;
		}
		
		protected override void OnExpose(object o, ExposeEventArgs args)
		{
			using(Cairo.Context cr = Gdk.CairoHelper.Create(GdkWindow)){
				double x = 0;
				double y = 0;
				double w = this.WidthRequest;
				double h = this.HeightRequest;
				DrawPaper(cr,x,y,w,h);
				GetInnerRegion(ref x,ref y,ref w,ref h);
				
				Pango.Layout pl = new Pango.Layout(this.PangoContext);
				
				LabelRenderer l = new LabelRenderer(Layout);
				int labelPadding = 3;
				l.Render(cr,pl,
				         labelPadding,labelPadding,
				         this.WidthRequest-2*labelPadding,
				         this.HeightRequest-2*labelPadding);				
			}
		}
		
		private LabelLayout layout;
		public LabelLayout Layout {
			set {
				layout = value;
				QueueDraw();
			}
			get {
				return layout;
			}
		}
		
		private const double labelPadding = 3;
	}
}
