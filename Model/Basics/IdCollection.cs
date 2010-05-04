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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MyInventory.Model
{
	public class IdCollection<T> 
	: IEnumerable, INotifyCollectionChanged
	where T : IdObject 
	{
		public IdCollection()
		{
			_ids = new Dictionary<uint,T>();
		}
		
		protected void Add(T item) {
			_ids.Add(item.Id,item);
			
			if(CollectionChanged != null)
				CollectionChanged(this,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,item));
		}
		
		public virtual void Remove(T item) {
			if(!_ids.Remove(item.Id)){
				throw new ArgumentException("Item to remove is not part of the collection.");
			}
				
			if(CollectionChanged != null)
				CollectionChanged(this,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,item));
		}
		
		public T this[uint id] {
			get {
				return _ids[id];
			}
		}
		
		/// <summary>
		///   Generates an Item ID that isn't used yet.
		/// </summary>
		public uint UnusedId {
			get {
				uint id = 1;
				while(IsIdUsed(id)) ++id;
				return id;
			}
		}
		/// <summary>
		///   Checks if the id is used yet.
		/// </summary>
		public bool IsIdUsed(uint id){
			return _ids.ContainsKey(id);
		}
		/// <summary>
		///    Gets the id that is highter then the passed one
		///    And is still unused.
		/// </summary>
		public uint GetNextUnusedId(uint current){
			uint id = current+1;
			while(IsIdUsed(id)) ++id;
			return id;
		}	
		/// <summary>
		///    Gets the id that is smaller then the passed one
		///    And is still unused.
		///    If there is no such id the passed one is returned,
		///    So make sure it is unused
		/// </summary>
		public uint GetPrevUnusedId(uint current){
			uint id = current-1;
			while(IsIdUsed(id)) --id;
			Console.WriteLine("=");
			Console.WriteLine(id);
			return (id==0)?current:id;
		}
		
		public int Count {
			get {return _ids.Count;}
		}
		
		public IEnumerator GetEnumerator(){
			return _ids.Values.GetEnumerator();			
		}
		
		private Dictionary<uint,T> _ids;
		
		public event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}
