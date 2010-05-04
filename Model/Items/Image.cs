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

namespace MyInventory.Model
{
	public class Image : ObservableIdObject
	{
		public Image(uint id, string fullsizePath, string previewPath, Images images) : base(id) {
			_images = images;
			
			_fullsizeImage = MemoryImage.Create(Inventory,fullsizePath);
			_previewImage = MemoryImage.Create(Inventory,previewPath);
			_previewImage.Load();
		}
		
		public void Serialize(XmlWriter writer) {
			writer.WriteStartElement("image");
			writer.WriteAttributeString("id",XmlConvert.ToString(Id));
			if(!string.IsNullOrEmpty(Memo))
				writer.WriteAttributeString("memo",Memo);
			writer.WriteEndElement();
		}
		
		public virtual void DeserializeProperties(XPathNavigator nav) {
			XPathNavigator node = nav.SelectSingleNode("@memo");
			if(node != null){
				Memo = node.Value;
			}
		}
		
		private string _memo;
		public string Memo {
			set { SetNotifyProperty(ref _memo,value,"Memo"); }
			get { return _memo; }
		}
		
		private MemoryImage _fullsizeImage;
		public MemoryImage FullsizeImage {
			get {
				_fullsizeImage.Load();
				return _fullsizeImage;
			}
		}
		
		private MemoryImage _previewImage;
		public MemoryImage PreviewImage {
			get {return _previewImage;}
		}
		
		private Images _images;
		public Images Images {
			get {return _images;}
		}
		public Item ParentItem {
			get {return Images.ParentItem;}
		}
		public Items Items {
			get {return ParentItem.Items;}
		}
		public Inventory Inventory {
			get {return Items.Inventory;}
		}
	}
}
