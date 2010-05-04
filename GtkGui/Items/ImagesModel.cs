using System;
using MyInventory.Model;
using System.Collections;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MyInventory.GtkGui
{
	public class ImagesModel : ObservableCollectionModel<Image>
	{
		public ImagesModel(Images imgs)
		: base(imgs.Positions)
		{
			Images = imgs;			
		}
		
		protected override Type ProvideColumnType(int col){
			switch(col){
			case 0:
				return typeof(Image);
			case 1:
				return typeof(bool);
			case 2:
				return typeof(Gdk.Pixbuf);
			case 4:
				return typeof(Gdk.Color);
			default:
				return typeof(string);
			}
		}
		
		protected override object GetColumnValue(object o, int col){
			Image img = (Image)o;
			
			switch(col){
			case 0:
				return img;
			case 1:
				return object.ReferenceEquals(img, img.Images.MainImage);
			case 2:
				return ((GtkMemoryImage)img.PreviewImage).Pixbuf;
			case 3:
				if(string.IsNullOrEmpty(img.Memo))
					return "Memo";
				else
					return img.Memo;
			case 4:
				if(string.IsNullOrEmpty(img.Memo))
					return ((GtkSettings)Images.Inventory.Settings).EntryDescriptionColor;
				else
					return new Gdk.Color(0,0,0);
			}
			
			return "Column not defined";
		}
			
		public override int NColumns { 
			get {
				return 5;
			}
		}
		
		private readonly Images Images;
	}
}
