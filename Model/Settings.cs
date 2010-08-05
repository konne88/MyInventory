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
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MyInventory.Model
{
	[XmlRoot("settings")]
	public class Settings
	{
		public Settings() {
			InventoryPath = Path.Combine(GetPersistentPath(),"inventory");
		}
		
		// no signal will imply that the following properties have changed
		// therefore you should not change them during application runtime
		// changes will only apply after a restart
		[XmlElement("preview-width")]
		public int PreviewWidth = 40;
		[XmlElement("preview-height")]
		public int PreviewHeight = 40;
		[XmlElement("preview-border-width")]
		public int PreviewBorderWidth = 1;
		[XmlIgnore]	// is unfortunately not serializable
		public Color PreviewBorderColor = Color.Black;
		
		[XmlIgnore]
		public Type MemoryImage;
		
		[XmlElement("useful-life-standard")]
		public double UsefulLifeStandard = 5*365; //5 years in days
		
		[XmlElement("inventory-path")]
		public string InventoryPath;
		[XmlElement("modified-inventory-path")]
		public string ModifiedInventoryPath = "/tmp/modified_inventory/";
		
		// The content of the following two properties will signal changes
		[XmlElement("label-layout")]
		public LabelLayout LabelLayout = new LabelLayout();
		[XmlElement("page-layout")]
		public PageLayout PageLayout = new PageLayout();
		
		public void CleanModifiedInventory(){
			try {
				System.IO.Directory.Delete(ModifiedInventoryPath, true);
			} catch (DirectoryNotFoundException){
				// doesn't matter if it doesn't exist
			}
		}
		
		public void Save() {
			string path = GetSettingsPath();
			
			string dir = Path.GetDirectoryName(path);
			if(! Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			
			XmlSerializer serializer = new XmlSerializer(this.GetType());
			Stream stream = new FileStream(path, FileMode.Create);
			serializer.Serialize(stream, this);
			stream.Close();
		}
		
		static public Settings Load() {
			string path = GetSettingsPath();
			
			Settings settings;
		
			if(File.Exists(path)){
				XmlSerializer serializer = new XmlSerializer(typeof(Settings));
				Stream stream = new FileStream(path, FileMode.Open);
				settings = (Settings)serializer.Deserialize(stream);
     			stream.Close();
			}
			else settings = new Settings();
			
			return settings;
		}
		
		static protected string GetPersistentPath() {
			// get the home folder
			string path = Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			path = Path.Combine(path,".myinventory");
			return path;
		}
		
		static protected string GetSettingsPath() {
			return Path.Combine(GetPersistentPath(),"settings.xml");
		}
	}
}
