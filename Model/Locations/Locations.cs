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
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using System.Collections.ObjectModel;

namespace MyInventory.Model
{
	public class Locations : PositionableIdCollection<Location,ObservableTree<Location>>
	{
		public Locations(Inventory inventory)
		{
			Inventory = inventory;
		}
			
		public Location New(Item item)
		{
			return New(UnusedId,item);
		}
		
		public Location New(uint id, Item item)
		{
			Location l = new Location(id,item,this);
			Add(l);
			return l;
		}
		
		readonly public Inventory Inventory;
		
		private void SerializeNode(ObservableTreeNode<Location> node, XmlWriter writer)
		{
			writer.WriteStartElement("location");
			node.Header.SerializeProperties(writer);
			foreach(ObservableTreeNode<Location> child in node){
				SerializeNode(child,writer);
			}
			writer.WriteEndElement();
		}
		
		public void Serialize(XmlWriter writer) 
		{
			if(Count == 0) return;
			
			writer.WriteStartElement("locations");
			foreach(ObservableTreeNode<Location> node in Positions){
				SerializeNode(node,writer);
			}
			writer.WriteEndElement();
		}
		
		private ObservableTreeNode<Location> DeserializeNode
			(ObservableTreeNode<Location> parent, XPathNavigator nav)
		{
			// create the node
			uint id = XmlConvert.ToUInt32(nav.SelectSingleNode("@id").Value);
			uint itemId = XmlConvert.ToUInt32(nav.SelectSingleNode("@item-id").Value);
			Location loc = New(id,Inventory.Items[itemId]);
			loc.DeserializeProperties(nav);
			ObservableTreeNode<Location> node = new ObservableTreeNode<Location>(loc,parent);
			
			// add sub nodes
			XPathNodeIterator iter = nav.SelectChildren(XPathNodeType.Element);
			foreach(XPathNavigator current in iter){
				node.Add(DeserializeNode(node,current));
			}
			
			return node;
		}
		
		public void DeserializeProperties(XPathNavigator nav) {
			XPathNodeIterator iter = nav.SelectChildren(XPathNodeType.Element);
			foreach(XPathNavigator current in iter){
				Positions.Add(DeserializeNode(null,current));
			}
		}
	}
}