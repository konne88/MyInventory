
using System;
using System.Drawing;

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
	}
}
