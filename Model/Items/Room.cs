using System;
using System.Xml;
using System.Xml.XPath;

namespace MyInventory.Model
{
	public class Room : Item {
		public Room(uint id,Items items):base(id,items)
		{}
		
		public override string ItemType {
			get {return "Room";}
		}
		public override void Serialize(XmlWriter writer) {
			writer.WriteStartElement("room");
			SerializeProperties(writer);
			writer.WriteEndElement();
		}
	}
}
