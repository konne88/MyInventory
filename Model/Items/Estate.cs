using System;
using System.Xml;
using System.Xml.XPath;

namespace MyInventory.Model
{
	public class Estate : Item {
		public Estate(uint id, Items items):base(id,items){}
		public override string GetSearchString() {	
			string str = base.GetSearchString();
			if(Street != null)  str+=" "+Street;
			if(Country != null) str+=" "+Country;
			if(Zip != null)     str+=" "+Zip;
			if(City != null)    str+=" "+City;
			return str;
		}
		
		public override Descriptions GetDescription(){
			Descriptions ds = base.GetDescription();
			
			if(!string.IsNullOrEmpty(Street))
				ds.Add(DescriptionPriority.HighMedium,Street);
			if(!string.IsNullOrEmpty(Country))
				ds.Add(DescriptionPriority.Medium,Country);
			if(!string.IsNullOrEmpty(Zip))
				ds.Add(DescriptionPriority.LowMedium,Zip);
			if(!string.IsNullOrEmpty(City))
				ds.Add(DescriptionPriority.HighMedium,City);
			
			return ds;
		}
		
		public override void Serialize(XmlWriter writer) {
			writer.WriteStartElement("estate");
			SerializeProperties(writer);
			writer.WriteEndElement();
		}
		
		protected override void SerializeProperties(XmlWriter writer) {
			base.SerializeProperties(writer);
			
			if(!string.IsNullOrEmpty(Street))
				writer.WriteElementString("street", Street);
			if(!string.IsNullOrEmpty(Country))
				writer.WriteElementString("country", Country);
			if(!string.IsNullOrEmpty(Zip))
				writer.WriteElementString("zip", Zip);		
			if(!string.IsNullOrEmpty(City))
				writer.WriteElementString("city", City);
		}
		
		public override void DeserializeProperties(XPathNavigator nav) {
			base.DeserializeProperties(nav);
			
			XPathNodeIterator iter = nav.SelectChildren(XPathNodeType.Element);
			foreach(XPathNavigator current in iter){
				switch(current.LocalName){
				case "street":
					Street = current.Value;
					break;
				case "country":
					Country = current.Value;
					break;
				case "zip":
					Zip = current.Value;
					break;
				case "city":
					City = current.Value;
					break;
				}
			}
		}
		//------------------------------------------------------------
		//                       Properties
		//------------------------------------------------------------
		public override string ItemType {
			get {return "Estate";}
		}
		private string _street;
		public string Street {
			set { SetNotifyProperty(ref _street, value, "Street");}
			get { return _street; }
		}
		private string _country;
		public string Country {
			set { SetNotifyProperty(ref _country, value, "Country"); }
			get { return _country; }
		}
		private string _zip;
		public string Zip {
			set { SetNotifyProperty(ref _zip, value, "Zip"); }
			get { return _zip; }
		}
		private string _city;
		public string City {
			set { SetNotifyProperty(ref _city, value, "City"); }
			get { return _city; }
		}
	}
	
}
