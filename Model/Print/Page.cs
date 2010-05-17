
using System;
using System.Collections;

namespace MyInventory.Model
{
	public class PageLayout {
		public double PaddingX = 3;// / 25.4;
		public double PaddingY = 3;// / 25.4;
		public int LabelRepeatX = 2;
		public int LabelRepeatY = 4;
		public double LabelWidth = 2.5*25.4;
		public double LabelHeight = 1.25*25.4;
		public int LabelsPerPage {
			get {
				return LabelRepeatX*LabelRepeatY;
			}
		}
	}
	
	public class Page
	{
		public Page(PageLayout layout,ArrayList labels) {
			Layout = layout;
			Labels = labels;
		}
		
		public PageLayout Layout;
		public ArrayList Labels;
	}
}
