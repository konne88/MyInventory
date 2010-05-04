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
using System.Collections.Generic;

namespace MyInventory.Model
{
	public class ItemLocationReferenceException : ApplicationException 
	{
		public ItemLocationReferenceException(Location[] refs)
		: base("An item can't be deleted since some locations are still referencing it.")
		{
			Data["LocationReferences"] = refs;
		}
	}
	
	public class Items : PositionableIdCollection<Item,ObservableCollection<Item>>
	{
		public Items(Inventory inventory)
		{			
			Inventory = inventory;
		}
		
		public Item New(uint id, Type type)
		{
			ConstructorInfo con = type.GetConstructor(new Type[]{typeof(uint),typeof(Items)});
			Item i = (Item)con.Invoke(new object[]{id,this});
			Add(i);
			return i;
		}
		
		public void TestRemove(Item item){
			if(!object.ReferenceEquals(item.Items,this))
				throw new ArgumentException("Actions were taken on an item using an item container the item isn't part of.");
			
			List<Location> refs = new List<Location>();
			foreach(Location loc in Inventory.Locations){
				if(object.ReferenceEquals(loc.Item,item)){
					refs.Add(loc);
				}
			}
			if(refs.Count != 0)
				throw new ItemLocationReferenceException(refs.ToArray());
		}
		
		public bool CanRemove(Item item) {
			try {
				TestRemove(item);
			} catch(ItemLocationReferenceException) {
				return false;
			}
		
			return true;
		}
		
		public override void Remove(Item item)
		{
			TestRemove(item);
			
			base.Remove(item);
		}
		
		public readonly Inventory Inventory;
		
		public void Serialize(XmlWriter writer) 
		{
			if(Count == 0) return;
			
			writer.WriteStartElement("items");
			foreach(Item item in Positions){
				item.Serialize(writer);
			}
			writer.WriteEndElement();
		}
		
		public void DeserializeProperties(XPathNavigator nav) {
			XPathNodeIterator iter = nav.SelectChildren(XPathNodeType.Element);
			
			foreach(XPathNavigator current in iter){
				Type type;
				switch(current.LocalName){
				case "room":
					type = typeof(Room);
					break;
				case "product":
					type = typeof(Product);
					break;
				case "estate":
					type = typeof(Estate);
					break;
				default:
					continue;
				}
				uint id = XmlConvert.ToUInt32(current.SelectSingleNode("id").Value);
							
				Item item = New(id,type);
				item.DeserializeProperties(current);
				Positions.Add(item);
			}
		}
	}
}
