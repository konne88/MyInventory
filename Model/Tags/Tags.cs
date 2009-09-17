using System;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MyInventory.Model
{
	public class Tags : PositionableIdCollection<Tag,ObservableTree<Tag>>
	{
		public Tags(Inventory inventory)
		{
			Inventory = inventory;
		}
		
		public Tag New(string name) {
			return New(UnusedId,name);
		}
		
		public Tag New(uint id, string name)
		{
			Tag t = new Tag(id,name,this);
			Add(t);
			return t;
		}
		
		public Tag GetTagByName(string name){
			name = name.ToLower();
			foreach(Tag t in this){
				if(t.Name.ToLower() == name)
					return t;
			}
			return null;
		}
		
		private void SerializeNode(ObservableTreeNode<Tag> node, XmlWriter writer)
		{
			writer.WriteStartElement("tag");
			node.Header.SerializeProperties(writer);
			foreach(ObservableTreeNode<Tag> child in node){
				SerializeNode(child,writer);
			}
			writer.WriteEndElement();
		}
		
		public void Serialize(XmlWriter writer) 
		{
			if(Count == 0) return;
			
			writer.WriteStartElement("tags");
			foreach(ObservableTreeNode<Tag> node in Positions){
				SerializeNode(node,writer);
			}
			writer.WriteEndElement();
		}
		
		private ObservableTreeNode<Tag> DeserializeNode
			(ObservableTreeNode<Tag> parent, XPathNavigator nav)
		{
			// create the node
			uint id = XmlConvert.ToUInt32(nav.SelectSingleNode("@id").Value);
			string name = nav.SelectSingleNode("@name").Value;
			Tag tag = New(id,name);
			tag.DeserializeProperties(nav);
			ObservableTreeNode<Tag> node = new ObservableTreeNode<Tag>(tag,parent);
			
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
		
		public readonly Inventory Inventory;
	}
}
