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
	public class ObservableIdObject : IdObject, INotifyPropertyChanged
	{
		public ObservableIdObject(uint id)
		: base(id)
		{}
		
		protected void SetNotifyProperty<T>(ref T prop, T val,string name){
			if(val == null && prop == null)
				return;
			
			if(
			   ( val == null || prop == null) ||
			   ( !prop.Equals(val) )
			)
			{
				prop = val;
				NotifyPropertyChanged(name);
			}
		}
		
		protected void NotifyPropertyChanged(string name)
        {
			if (PropertyChanged != null)
			{
			   PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
        }
		
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
