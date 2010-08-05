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
using System.Xml.Serialization;
using System.IO;

namespace MyInventory.GtkGui
{	
	[XmlRoot("gtk-settings")]
	public class GtkSettings : Settings
	{
		public GtkSettings() {
			ProductIcon = new Gdk.Pixbuf(null,"product.png");
			RoomIcon = new Gdk.Pixbuf(null,"room.png");	
			EstateIcon = new Gdk.Pixbuf(null,"estate.png");
			
			ItemsTabIcon = new Gdk.Pixbuf(null,"item.png");
			LocationsTabIcon = new Gdk.Pixbuf(null,"location.png");
			TagsTabIcon = new Gdk.Pixbuf(null,"tag.png");
			WindowIcon = ItemsTabIcon;
			
			MemoryImage = typeof(GtkMemoryImage);
		}
		
		// no signal will imply that properties of this class have changed
		// therefore you should not change them during application runtime
		// changes will only apply after a restart
		[XmlIgnore]	// is unfortunately not nicely serializable
		public Gdk.Color EntryDescriptionColor = new Gdk.Color (0xAA,0xAA,0xAA);
		
		[XmlIgnore]
		public Gdk.Pixbuf ProductIcon;
		[XmlIgnore]
		public Gdk.Pixbuf RoomIcon;
		[XmlIgnore]
		public Gdk.Pixbuf EstateIcon;
		[XmlIgnore]
		public Gdk.Pixbuf ItemsTabIcon;
		[XmlIgnore]
		public Gdk.Pixbuf LocationsTabIcon;
		[XmlIgnore]
		public Gdk.Pixbuf TagsTabIcon;
		[XmlIgnore]
		public Gdk.Pixbuf WindowIcon;
		
		// print preview layout
		[XmlElement("paper-width")]
		public double PaperWidth = 65; 	// mm
		[XmlElement("section-width")]
		public double SectionWidth = 80;
		[XmlElement("section-height")]
		public double SectionHeight = 40;
		[XmlElement("section-padding")]
		public double SectionPadding = 10;
		[XmlElement("label-padding")]
		public double LabelPadding = 2;
		[XmlElement("page-height-per-width")]
		public double PageHeightPerWidth = Math.Sqrt(2);
		[XmlElement("label-height-per-width")]
		public double LabelHeightPerWidth = 0.5;
		
		static new public GtkSettings Load() {
			string path = GetSettingsPath();
			
			GtkSettings settings;
			
			if(File.Exists(path)){
				XmlSerializer serializer = new XmlSerializer(typeof(GtkSettings));
				Stream stream = new FileStream(path, FileMode.Open);
				settings = (GtkSettings)serializer.Deserialize(stream);
     			stream.Close();
			}
			else settings = new GtkSettings();
			
			return settings;
		}
	}
}
