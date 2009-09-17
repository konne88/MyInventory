using System;
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;

namespace MyInventory.Model
{
	public class Location : ObservableIdObject {
		public Location(uint id, Item item, Locations locations) : base(id)
		{
			Item = item;
			_locations = locations;
		}
		
		public virtual void SerializeProperties(XmlWriter writer) {
			writer.WriteAttributeString("id",XmlConvert.ToString(Id));
			writer.WriteAttributeString("item-id",XmlConvert.ToString(Item.Id));
			if(Amount != 1)
				writer.WriteAttributeString("amount", XmlConvert.ToString(Amount));
			if(Labeled == true)
				writer.WriteAttributeString("labeled", "true");
		}
		
		public virtual void DeserializeProperties(XPathNavigator nav) {			
			XPathNodeIterator iter = nav.Select("@*");
			
			foreach(XPathNavigator current in iter){
				switch(current.LocalName){
				case "amount":
					Amount = current.ValueAsInt;
					break;
				case "labeled":
					Labeled = current.ValueAsBoolean;
					break;
				}
			}
		}
		
		private Item _item;
		public virtual Item Item {
			set { 
				if(value == null)
					throw new ArgumentNullException("Item");
				SetNotifyProperty(ref _item, value, "Item");  
			}
			get { return _item; }
		}
		
		private int _amount = 1;
		public int Amount {			
			set { SetNotifyProperty(ref _amount, value, "Amount"); }
			get { return _amount; }
		}
		
		private bool _labeled = false;
		public bool Labeled {
			set { SetNotifyProperty(ref _labeled, value, "Labeled"); }
			get { return _labeled; }
		}
		
		private Locations _locations;
		public Locations Locations {
			get { return _locations; }
		}
		public Inventory Inventory {
			get { return Locations.Inventory; }			
		}
	}
}
