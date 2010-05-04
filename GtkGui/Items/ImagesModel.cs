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
using MyInventory.Model;
using System.Collections;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MyInventory.GtkGui
{
	public class ImagesModel : ObservableCollectionModel<Image>
	{
		public ImagesModel(Images imgs)
		: base(imgs.Positions)
		{
			Images = imgs;			
		}
		
		protected override Type ProvideColumnType(int col){
			switch(col){
			case 0:
				return typeof(Image);
			case 1:
				return typeof(bool);
			case 2:
				return typeof(Gdk.Pixbuf);
			case 4:
				return typeof(Gdk.Color);
			default:
				return typeof(string);
			}
		}
		
		protected override object GetColumnValue(object o, int col){
			Image img = (Image)o;
			
			switch(col){
			case 0:
				return img;
			case 1:
				return object.ReferenceEquals(img, img.Images.MainImage);
			case 2:
				return ((GtkMemoryImage)img.PreviewImage).Pixbuf;
			case 3:
				if(string.IsNullOrEmpty(img.Memo))
					return "Memo";
				else
					return img.Memo;
			case 4:
				if(string.IsNullOrEmpty(img.Memo))
					return ((GtkSettings)Images.Inventory.Settings).EntryDescriptionColor;
				else
					return new Gdk.Color(0,0,0);
			}
			
			return "Column not defined";
		}
			
		public override int NColumns { 
			get {
				return 5;
			}
		}
		
		private readonly Images Images;
	}
}
