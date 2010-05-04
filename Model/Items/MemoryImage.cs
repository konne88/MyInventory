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
using System.Reflection;

namespace MyInventory.Model
{
	abstract public class MemoryImage
	{
		public MemoryImage(string path){
			_path = path;
		}
		
		public abstract void Load();
		
		private string _path;
		public string Path {
			get { return _path; }
		}
		
		public static MemoryImage Create(Inventory inv, string path){
			Type type = inv.Settings.MemoryImage;
			ConstructorInfo con = type.GetConstructor(new Type[]{typeof(string)});
			return (MemoryImage)con.Invoke(new object[]{path});
		}
	}
}
