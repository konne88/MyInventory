using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace MyInventory.Model
{
	/// <summary>
	///   This class represents all the data of an inventory in a way that is 
	///   independent of the window representation used.
	///   This class is XML serializeable but not deserializeable.
	///   Use Save to save this class as a file or use
	///   the static function Load to load an instance of this class.
	/// <summary>
	public class Inventory
	{
		public Inventory(string path, Settings settings) {
			Path = path;
			Settings = settings;
			Tags = new Tags(this);
			Items = new Items(this);
			Locations = new Locations(this);
		}
		
		public readonly Items Items;
		public readonly Tags Tags;
		public readonly Locations Locations;
		public readonly string Path;
		public readonly Settings Settings;
		
		public void Serialize(XmlWriter writer) {
			writer.WriteStartElement("inventory");
			Items.Serialize(writer);
			Locations.Serialize(writer);
			Tags.Serialize(writer);
			writer.WriteEndElement();
		}

		public void DeserializeProperties(XPathNavigator nav) {
			XPathNavigator node;
			node = nav.SelectSingleNode("tags");
			if(node != null)
				Tags.DeserializeProperties(node);
			node = nav.SelectSingleNode("items");
			if(node != null)
				Items.DeserializeProperties(node);
			node = nav.SelectSingleNode("locations");
			if(node != null)
				Locations.DeserializeProperties(node);
		}
		
		public void Save() {
			// save xml
			
			Console.WriteLine(Path);
			if(! Directory.Exists(Path)) Directory.CreateDirectory(Path);
			
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			
			XmlWriter writer = XmlWriter.Create(System.IO.Path.Combine(Path,"inventory.xml"),settings);
			writer.WriteStartDocument();
			Serialize(writer);
			writer.WriteEndDocument();
			writer.Close();
		}
		
		public void Export(string path) {
			//string oldPath = Path;
			//_path = path;
			//Save();
			//_path = oldPath;
		}
		
		static public Inventory Load(string path, Settings settings) {
			Inventory inventory = new Inventory(path,settings);
			
			XPathDocument doc = new XPathDocument(System.IO.Path.Combine(path,"inventory.xml"));
			XPathNavigator nav = doc.CreateNavigator();
			inventory.DeserializeProperties(nav.SelectSingleNode("inventory"));
			
			return inventory;
		}
	}
}