using System;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using System.Collections.ObjectModel;

namespace MyInventory.Model
{
	public class ObservableTree<T> : ObservableCollection<ObservableTreeNode<T>> 
	{
		public ObservableTree()
		{}
		
		public void InsertItemAt(int[] path, ObservableTreeNode<T> item) 
		{
			ObservableCollection<ObservableTreeNode<T>> node = this;
			for(int i=0 ; i<path.Length-1 ; ++i)
				node = node[path[i]];
			
			node.Insert(path[path.Length-1], item);
		}
		
		public void RemoveItemAt(int[] path) 
		{
			Console.WriteLine(path.Length);
			
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