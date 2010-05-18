
using System;
using System.Collections;

namespace MyInventory.Model
{
	public class Page
	{
		public Page(ArrayList labels, PageLayout layout) {
			Layout = layout;
			Labels = labels;
		}
		
		public PageLayout Layout;
		public ArrayList Labels;
	}
	
	public class PageLayout : ObservableObject {
		public PageLayout() {
			paddingX = 3;
			paddingY = 3;
			labelRepeatX = 2;
			labelRepeatY = 4;
			labelWidth = 2.5*25.4;
			labelHeight = 1.25*25.4;
		}
		
		private double paddingX;
		public double PaddingX {
			set {
				SetNotifyProperty<double>(ref paddingX,value,"PaddingX");
			}
			get {
				return paddingX;
			}
		}
		private double paddingY;
		public double PaddingY {
			set {
				SetNotifyProperty<double>(ref paddingY,value,"PaddingY");
			}
			get {
				return paddingY;
			}
		}
		private int labelRepeatX;
		public int LabelRepeatX {
			set {
				SetNotifyProperty<int>(ref labelRepeatX,value,"LabelRepeatX");
				NotifyPropertyChanged("LabelsPerPage");
			}
			get {
				return labelRepeatX;
			}
		}
		private int labelRepeatY;
		public int LabelRepeatY {
			set {
				SetNotifyProperty<int>(ref labelRepeatY,value,"LabelRepeatY");
				NotifyPropertyChanged("LabelsPerPage");
			}
			get {
				return labelRepeatY;
			}
		}
		private double labelWidth;
		public double LabelWidth {
			set {
				SetNotifyProperty<double>(ref labelWidth,value,"LabelWidth");
			}
			get {
				return labelWidth;
			}
		}		
		private double labelHeight;
		public double LabelHeight {
			set {
				SetNotifyProperty<double>(ref labelHeight,value,"LabelHeight");
			}
			get {
				return labelHeight;
			}
		}
		public int LabelsPerPage {
			get {
				return LabelRepeatX*LabelRepeatY;
			}
		}
	}
}
