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
using System.ComponentModel;

namespace MyInventory.Model
{
	public class Label
    {
		public Label(LabelLayout layout) {
			Description = "Label Test Item\nBed, Home, Time, Tracking";
			InventoryAbbreviation = "KWTZ";
			Id = 1337;
			Layout = layout;
		}
		
		public Label(Item item, LabelLayout layout) {
			Description = item.GetDescription().ToString(2);
			Id = item.Id;
			InventoryAbbreviation = "KWTZ";
			Layout = layout;
		}
		
		public LabelLayout Layout;
		public readonly string Description;
		public readonly uint Id;
		public readonly string InventoryAbbreviation;
	}
	
	public class LabelLayout : ObservableObject {
		// The space on top and bottom of the barcode
		// in relation to the entire thing being 1
		public LabelLayout(){
			spacingSize = 0.05;
			font = "sans 5";
		}
		
		private double spacingSize;
		public double SpacingSize {
			set {
				SetNotifyProperty<double>(ref spacingSize,value,"SpacingSize");
			}
			get {
				return spacingSize;
			}
		}
		
		private string font;
		public string Font {
			set {
				SetNotifyProperty<string>(ref font,value,"Font");
			}
			get {
				return font;
			}
		}
	}
}
