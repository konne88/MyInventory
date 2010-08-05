
using Cairo;
using System;
using MyInventory.Model;
using Pango;
using System.Collections;

namespace MyInventory.GtkGui
{
	public class PageRenderer : Page, IRenderable
	{
		public PageRenderer(ArrayList labels,PageLayout layout) 
		: base(labels,layout)
		{}
		
		public void Render(Cairo.Context cr, Layout layout, double tx, double ty, double w, double h) {
			cr.Save();
			cr.Translate(tx+Layout.PaddingX,ty+Layout.PaddingY);
			int printingItem = 0;
			double xpad = (Layout.LabelRepeatX <= 1)?0:
					(w-Layout.PaddingX*2-Layout.LabelRepeatX*Layout.LabelWidth)/(Layout.LabelRepeatX-1);
			double ypad = (Layout.LabelRepeatY <= 1)?0:
					(h-Layout.PaddingY*2-Layout.LabelRepeatY*Layout.LabelHeight)/(Layout.LabelRepeatY-1);
			
			for(int y=0 ; y < this.Layout.LabelRepeatY ; ++y){
				for(int x=0 ; x < this.Layout.LabelRepeatX ; ++x){
					if(printingItem == this.Labels.Count)
						return;
					IRenderable item = (IRenderable) this.Labels[printingItem];
					item.Render (
					    cr,layout,
					    x*(Layout.LabelWidth+xpad),
					    y*(Layout.LabelHeight+ypad),
						Layout.LabelWidth,
						Layout.LabelHeight);
					++printingItem;
				}
			}
			cr.Restore();
		}
	}
}
