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
