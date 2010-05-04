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
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MyInventory.Model
{
	public class ObserveableList<T> : List<T>, INotifyCollectionChanged
	{
		public ObserveableList(){}
		
		public override void Add(T v){
			base.Add(v);
			
		}
		
		public override void Remove(T v){
			base.Remove(v);
			
		}
		
		public override void RemoveAt(int pos){
			base.RemoveAt(pos);

		}
		
		public override void Insert(T v, int pos){
			base.Insert(v,pos);

		}
		
		public event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}
