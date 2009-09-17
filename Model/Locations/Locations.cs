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
			_inventory = inventory;
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
		
		private Inventory _inventory;
		public Inventory Inventory 
		{
			get {
				return _inventory;
			}
		}
		
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
