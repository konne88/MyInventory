
using System;
using System.Collections;
using System.Xml.Serialization;

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
			labelRepeatX = 6;
			labelRepeatY = 12;
			labelWidth = 2.5*25.4;
			labelHeight = 1.25*25.4;
		}
		
		private double paddingX;
		[XmlElement("padding-x")]
		public double PaddingX {
			set {
				SetNotifyProperty<double>(ref paddingX,value,"PaddingX");
			}
			get {
				return paddingX;
			}
		}
		
		private double paddingY;
		[XmlElement("padding-y")]
		public double PaddingY {
			set {
				SetNotifyProperty<double>(ref paddingY,value,"PaddingY");
			}
			get {
				return paddingY;
			}
		}
		private int labelRepeatX;
		[XmlElement("label-repeat-x")]
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
		[XmlElement("label-repeat-y")]
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
		[XmlElement("label-width")]
		public double LabelWidth {
			set {
				SetNotifyProperty<double>(ref labelWidth,value,"LabelWidth");
			}
			get {
				return labelWidth;
			}
		}		
		private double labelHeight;
		[XmlElement("label-height")]
		public double LabelHeight {
			set {
				SetNotifyProperty<double>(ref labelHeight,value,"LabelHeight");
			}
			get {
				return labelHeight;
			}
		}
		[XmlIgnore]
		public int LabelsPerPage {
			get {
				return LabelRepeatX*LabelRepeatY;
			}
		}
	}
}
