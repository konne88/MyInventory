using System;
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;

namespace MyInventory.Model
{
	public class Tag : ObservableIdObject {
		public Tag(uint id, string name, Tags tags) : base(id)
		{
			Tags = tags;
			Name = name;
		}
		
		public virtual void SerializeProperties(XmlWriter writer) {
			writer.WriteAttributeString("id",XmlConvert.ToString(Id));
			writer.WriteAttributeString("name",Name);
		}
		
		public virtual void DeserializeProperties(XPathNavigator nav) {}
		
		private string _name;
		public virtual string Name {
			set {
				if(value == null)
					throw new ArgumentNullException("Name");
				if(value == "")
					throw new ArgumentException("Name must not be empty");
				SetNotifyProperty(ref _name, value, "Name");
			}
			get { return _name; }
		}
		
		public readonly Tags Tags;
		public Inventory Inventory {
			get { return Tags.Inventory; }			
		}
	}
}
