using System;
using MyInventory.Model;
using System.Collections;
using System.Collections.ObjectModel;

namespace MyInventory.GtkGui
{
	public class ItemsModel : ObservableCollectionModel<Item>
	{
		public ItemsModel(Items items)
		: base(items.Positions)
		{
			Items = items;			
		}
		
		protected override Type ProvideColumnType(int col){
			switch(col){
			case 0:
				return typeof(Item);
			case 1:
				return typeof(Gdk.Pixbuf);
			default:
				return typeof(string);
			}
		}
		
		protected override object GetColumnValue(object o, int col){
			Item item = (Item)o;
			Product product = item as Product;
			Room room = item as Room;
			Estate estate = item as Estate;
			
			switch(col){
			case 0:
				return item;
			case 1:
				Model.Image img = item.Images.MainImage;
				if(img != null)
					return ((GtkMemoryImage)img.PreviewImage).Pixbuf;
				if(product != null)
					return ((GtkSettings)Items.Inventory.Settings).ProductIcon;
				if(room != null)
					return ((GtkSettings)Items.Inventory.Settings).RoomIcon;
				if(estate != null)
					return ((GtkSettings)Items.Inventory.Settings).EstateIcon;
				break;
			case 2:
				return item.Description.EmptyIfNull();
			case 3:
				return item.Id.ToString();
			case 4:
				return item.ItemType;
			case 5:	
				if(product != null)
					return product.Cost.ToString("#0.00€");
			 	else
					return "";
			case 6:
				if(product != null)
					return product.Value.ToString("#0.00€");
			 	else
					return "";	
			}
			
			return "Column not defined";
		}
			
		public override int NColumns { 
			get {
				return 7;
			}
		}
		
		private readonly Items Items;
	}
}
