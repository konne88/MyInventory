/* MyInventory - Keep track of your private assets. 
 * Copyright (C) 2008-2010 Konstantin Weitz
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 3
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

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
		public Inventory(Settings settings) {
			Settings = settings;
			Settings.CleanModifiedInventory();
			Tags = new Tags(this);
			Items = new Items(this);
			Locations = new Locations(this);
		}
		
		public readonly Items Items;
		public readonly Tags Tags;
		public readonly Locations Locations;
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
			if(! Directory.Exists(Settings.InventoryPath)) Directory.CreateDirectory(Settings.InventoryPath);
			
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			
			XmlWriter writer = XmlWriter.Create(System.IO.Path.Combine(Settings.InventoryPath,"inventory.xml"),settings);
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
		
		static public Inventory Load(Settings settings) {
			Inventory inventory = new Inventory(settings);
			String invFile = System.IO.Path.Combine(settings.InventoryPath,"inventory.xml");
			
			if(File.Exists(invFile)){
				XPathDocument doc = new XPathDocument(invFile);
				XPathNavigator nav = doc.CreateNavigator();
				inventory.DeserializeProperties(nav.SelectSingleNode("inventory"));
			}
			return inventory;
		}
	}
}