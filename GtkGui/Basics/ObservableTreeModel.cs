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
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MyInventory.GtkGui
{
	abstract public class ObservableTreeModel<T> : ListNotifyModel<ObservableCollection<ObservableTreeNode<T>>>
	{
		public ObservableTreeModel(ObservableTree<T> tree)
		: base(tree)
		{}
		
		protected override ObservableCollection<ObservableTreeNode<T>> GetCollection(object node){
			return (ObservableTreeNode<T>)node;
		}
		
		protected override object GetObject(object node){
			return ((ObservableTreeNode<T>)node).Header;
		}
	}
}
