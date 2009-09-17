using System;
using MyInventory.Model;
using System.Collections;
using System.Collections.ObjectModel;

namespace MyInventory.GtkGui
{
	public class LocationsModel : ObservableTreeModel<Location>
	{
		public LocationsModel(Locations locs)
		: base(locs.Positions)
		{
			Locations = locs;
		}
		
		protected override Type ProvideColumnType(int col){
			switch(col){
			case 0:
				return typeof(Location);
			case 1:
				return typeof(Gdk.Pixbuf);
			case 4:
				return typeof(bool);
			case 5:
				return typeof(bool);
			default:
				return typeof(string);
			}
		}
		
		protected override object GetColumnValue(object o, int col){
			Location loc = (Location)o;
			
			switch(col){
			case 0:
				return loc;
			case 1:
				Model.Image img = loc.Item.Images.MainImage;
				if(img != null)
					return ((GtkMemoryImage)img.PreviewImage).Pixbuf;
				if(loc.Item is Product)
					return ((GtkSettings)Locations.Inventory.Settings).ProductIcon;
				if(loc.Item is Room)
					return ((GtkSettings)Locations.Inventory.Settings).RoomIcon;
				if(loc.Item is Estate)
					return ((GtkSettings)Locations.Inventory.Settings).EstateIcon;
				break;
			case 2:
				return loc.Item.Description;
			case 3:
				return loc.Item.Id.ToString();
			case 4:
				return loc.Labeled;
			case 5:
				Product product = loc.Item as Product;
				if(product != null)
					return product.LabelMethod != LabelMethod.None;
				else
					return false;
			case 6:
				return loc.Amount.ToString();
			}
			
			return "Column not defined";
		}
			
		public override int NColumns { 
			get {
				return 6;
			}
		}
		
		private readonly Locations Locations;
	}
}
