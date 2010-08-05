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
using System.Xml.Serialization;

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
		public LabelLayout(){
			spacingSize = 0.05;
			textSize = 0.15;
			fontName = "Sans";
			useBarcode = true;
			useBarcodeText = true;
			useDescription = true;
		}
		
		// The space on top and bottom of the barcode
		// in relation to the entire thing being 1
		private double spacingSize;
		[XmlElement("spacing-size")]
		public double SpacingSize {
			set {
				SetNotifyProperty<double>(ref spacingSize,value,"SpacingSize");
			}
			get {
				return spacingSize;
			}
		}
		
		// The size of one line of text
		// in relation to the entire thing being 1
		private double textSize;
		[XmlElement("text-size")]
		public double TextSize {
			set {
				SetNotifyProperty<double>(ref textSize,value,"TextSize");
			}
			get {
				return textSize;
			}
		}
		
		private string fontName;
		[XmlElement("font-name")]
		public string FontName {
			set {
				SetNotifyProperty<string>(ref fontName,value,"FontName");
			}
			get {
				return fontName;
			}
		}
		
		// Properties decribing what infomration is 
		// rendered onto the label
		private bool useDescription;
		[XmlElement("use-description")]
		public bool UseDescription {
			set {
				SetNotifyProperty<bool>(ref useDescription,value,"UseDescription");
			}
			get {
				return useDescription;
			}
		}
	
		private bool useBarcode;
		[XmlElement("use-barcode")]
		public bool UseBarcode {
			set {
				SetNotifyProperty<bool>(ref useBarcode,value,"UseBarcode");
			}
			get {
				return useBarcode;
			}
		}
		
		private bool useBarcodeText;
		[XmlElement("use-barcode-text")]
		public bool UseBarcodeText {
			set {
				SetNotifyProperty<bool>(ref useBarcodeText,value,"UseBarcodeText");
			}
			get {
				return useBarcodeText;
			}
		}
	}
}
