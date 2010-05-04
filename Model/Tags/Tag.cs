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
				if(value == _name)
					return;
				if(Tags.IsTagNameUsed(value))
					throw new ArgumentException("Name is already in use");
				
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
