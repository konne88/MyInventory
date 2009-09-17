using System;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using System.Collections.ObjectModel;

namespace MyInventory.Model
{
	public class ObservableTreeNode<T> : ObservableCollection<ObservableTreeNode<T>>
	{
		public ObservableTreeNode(T header, ObservableTreeNode<T> parent)
		{
			Header = header;
			Parent = parent;
		}

		protected override void InsertItem(int pos, ObservableTreeNode<T> item){
			base.InsertItem(pos,item);
			item.Parent = this;
		}
		
		private T _header;
		public T Header {
			set { _header = value; }
			get { return _header;	}
		}
		
		public ObservableTreeNode<T> Parent;
	}
}
