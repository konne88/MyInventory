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
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MyInventory.Model
{
	public class TagItemReferenceException : ApplicationException 
	{
		public TagItemReferenceException(Item[] refs)
		: base("A tag can't be deleted since some items are still referencing it.")
		{
			Data["ItemReferences"] = refs;
		}
	}
	
	public class Tags : PositionableIdCollection<Tag,ObservableTree<Tag>>
	{
		public Tags(Inventory inventory)
		{
			Inventory = inventory;
		}
		
		public Tag New() {
			return New(CreateUniqueTagName());
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
		
		public void TestRemove(Tag tag){
			if(!object.ReferenceEquals(tag.Tags,this))
				throw new ArgumentException("Actions were taken on a tag using an tag container the tag isn't part of.");
			
			List<Item> refs = new List<Item>();
			foreach(Item item in Inventory.Items){
				foreach(ItemTag it in item.Tags){
					if(object.ReferenceEquals(it.Tag,tag)){
						refs.Add(item);
						break;
					}
				}
			}
			
			if(refs.Count != 0)
				throw new TagItemReferenceException(refs.ToArray());
		}
		
		public bool CanRemove(Tag tag) {
			try {
				TestRemove(tag);
			} catch(TagItemReferenceException) {
				return false;
			}
		
			return true;
		}
		
		public override void Remove(Tag tag)
		{
			TestRemove(tag);
			
			base.Remove(tag);
		}
		
		public bool CanTagBecomePathsChild(Tag tag, int[] parentPath){
			// This is only true if no item has the parent or ancestors of the parent,
			// and also the tag added to itsself. See ItemTags.GetUnusedTags()
			
			Tag[] ancestors = Positions.GetAncestorsOf(parentPath);
			
			foreach(Item i in Inventory.Items){
				foreach(ItemTag it1 in i.Tags){
					// if the tag is part of the item ...
					if(object.ReferenceEquals(it1.Tag,tag)){
						// then we can check for each ancestor
						// if it is also a part of the item
						foreach(Tag a in ancestors){
							foreach(ItemTag it2 in i.Tags){
								// if that is true,
								// the tag can not become a child
								if(object.ReferenceEquals(it2.Tag,a)){
									return false;
								}
							}
						}				
						break;
					}
				}
			}
			
			return true;
		}
		
		public string CreateUniqueTagName(){
			// calc a unique tag name
			string nameAndNum = "Tag";
			string name = nameAndNum;
			int i = 1;
			while(IsTagNameUsed(nameAndNum)){
				++i;
				nameAndNum = name+" "+i.ToString();
			}
			return nameAndNum;
		}
		
		public Tag GetTagByName(string name){
			name = name.ToLower();
			foreach(Tag t in this){
				if(t.Name.ToLower() == name)
					return t;
			}
			return null;
		}
		
		public bool IsTagNameUsed(string name){
			name = name.ToLower();
			foreach(Tag t in this){
				if(t.Name.ToLower() == name)
					return true;
			}
			return false;
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
