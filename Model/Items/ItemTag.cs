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
	public class ItemTag : INotifyPropertyChanged {
		public ItemTag(Tag tag)
		{
			Tag = tag;
		}
		
		private void OnTagChanged(object sender, PropertyChangedEventArgs args){
			if (PropertyChanged != null)
			   PropertyChanged(this, args);
		}
		
		private Tag _tag;
		public Tag Tag {
			set {
				if(!object.ReferenceEquals(Tag,value)){
					if(Tag != null)
						Tag.PropertyChanged -= OnTagChanged;
					_tag = value;
					if(Tag != null)
						Tag.PropertyChanged += OnTagChanged;
					if(PropertyChanged != null)
						PropertyChanged(this, new PropertyChangedEventArgs("Tag"));
				}
			}
			get {
				return _tag;
			}
		}
		
		public string Name {
			set { Tag.Name = value; }
			get { return Tag.Name; }
		}
		
		public uint Id {
			get { return Tag.Id; }
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
