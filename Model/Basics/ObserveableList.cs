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
