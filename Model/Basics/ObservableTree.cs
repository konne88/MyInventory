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
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyInventory.Model
{
	public class ObservableTree<T> : ObservableCollection<ObservableTreeNode<T>> 
	{
		public ObservableTree()
		{}
		
		public T[] GetAncestorsOf(int[] path){
			List<T> ancestors = new List<T>();
			ObservableCollection<ObservableTreeNode<T>> list = this;
						
			for(int depth=0 ; depth<path.Length ; ++depth){
				ObservableTreeNode<T> node = list[path[depth]];
				ancestors.Add(node.Header);
				list = node;
			}
			
			return ancestors.ToArray();
		}
				
		public void InsertItemAt(int[] path, ObservableTreeNode<T> item) 
		{
			ObservableCollection<ObservableTreeNode<T>> node = this;
			for(int i=0 ; i<path.Length-1 ; ++i)
				node = node[path[i]];
			
			node.Insert(path[path.Length-1], item);
		}
		
		public void RemoveItemAt(int[] path) 
		{			
			ObservableCollection<ObservableTreeNode<T>> node = this;
			for(int i=0 ; i<path.Length-1 ; ++i)
				node = node[path[i]];
			
			node.RemoveAt(path[path.Length-1]);
		}
		
		protected override void InsertItem(int pos, ObservableTreeNode<T> item){
			base.InsertItem(pos,item);
			item.Parent = null;
		}
	}
}