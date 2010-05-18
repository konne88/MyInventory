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
using System.Drawing;
using System.ComponentModel;

namespace MyInventory.Model
{	
	public class Settings
	{		
		public int PreviewWidth = 40;
		public int PreviewHeight = 40;
		public int PreviewBorderWidth = 1;
		public Color PreviewBorderColor = Color.Black;
		
		public Type MemoryImage;
		
		public double UsefulLifeStandard = 5*365; //5 years in days
		
		public string ModifiedInventoryPath = "/tmp/modified_inventory/";
		
		public LabelLayout LabelLayout = new LabelLayout();
		public PageLayout PageLayout = new PageLayout();
	}
}
