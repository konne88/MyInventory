using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MyInventory.Model
{
	public class ItemTags : ObservableCollection<ItemTag>
	{
		public ItemTags(Item item){
			_item = item;
		}
		
		public ItemTag[] GetUnusedTags(ItemTag ignoreTag){
			List<ItemTag> list = new List<ItemTag>();
			GetUnusedTags(list,Tags.Positions,ignoreTag);
			return list.ToArray();
		}
		/// <summary>
		///   Adds all tags to unused, that are not an item of the array or a child/parent of one such items.
		///   E.g. 
		/// 
		///   Consider the following tag structure
		///   - Electronics
		///     - PC
		///       - Storage
		///     - TV
		///       - DVD
		///   - Clothing
		/// 
		///  If PC is used, only TV, DVD and Clothing is unused
		///  If Storage is used TV,DVD and Clothing are unused
		///  If Electronics is used only Clothing is unused
		/// 
		/// </summary>
		/// <param name="used">
		/// An array of <see cref="Tag"/> containing all used tags. 
		/// </param>
		/// <param name="unused">
		/// A <see cref="ArrayList"/> that will be filled with all the tags that are not used yet.
		/// </param>
		/// <param name="col">
		/// A <see cref="ObservableCollection<ObservableTreeNode<Tag>> "/> a node of the tree or the root
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/> that is true if all children of the item were added to unused.
		/// </returns>
		public bool GetUnusedTags(List<ItemTag> unused, 
		                          ObservableCollection<ObservableTreeNode<Tag>> col,
		                          ItemTag ignoreTag)
		{	
			bool isUsed;
			bool allItemsUnused = true;
						
			foreach(ObservableTreeNode<Tag> node in col){
				Tag tag = node.Header;
				isUsed = false;
				
				foreach(ItemTag u in this){
					if(object.ReferenceEquals(ignoreTag,u))
						continue;
						
					if(object.ReferenceEquals(tag,u.Tag)){
						isUsed = true;
						break;
					}
				}
				
				//if this node and also all of it's children are unused
				if(isUsed == false && GetUnusedTags(unused, node, ignoreTag) == true)
					unused.Add(new ItemTag(tag));
				else
					allItemsUnused = false;
			}
			
			return allItemsUnused;
		}
		
		private Item _item;
		public Item ParentItem {
			get {return _item;}
		}
		public Items ParentItems {
			get {return ParentItem.Items;}
		}
		public Inventory Inventory {
			get {return ParentItems.Inventory;}
		}
		public Tags Tags {
			get {return Inventory.Tags;}
		}
		
		public void Serialize(XmlWriter writer) {
			if(Count == 0) return;
			
			writer.WriteStartElement("tags");
			foreach(ItemTag tag in this){
				writer.WriteStartElement("tag");
				writer.WriteAttributeString("id",XmlConvert.ToString(tag.Id));
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
		
		public virtual void DeserializeProperties(XPathNavigator nav) {			
			XPathNodeIterator iter = nav.SelectChildren(XPathNodeType.Element);
			foreach(XPathNavigator current in iter){
				uint id = XmlConvert.ToUInt32(current.SelectSingleNode("@id").Value);
				Add(new ItemTag(Tags[id]));
			}
		}
	}
}
