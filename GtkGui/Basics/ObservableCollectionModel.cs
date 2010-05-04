using System;
using Gtk;
using MyInventory.Model;
using System.Collections;
using System.Collections.ObjectModel;

namespace MyInventory.GtkGui
{
	abstract public class ObservableCollectionModel<T> : ListNotifyModel<ObservableCollection<T>>
	{
		public ObservableCollectionModel(ObservableCollection<T> list)
		: base(list)
		{}
		
		protected override ObservableCollection<T> GetCollection(object node){
			return null;
		}
		
		protected override object GetObject(object node){
			return node;
		}
		
		public override TreeModelFlags Flags { 
			get {
				return TreeModelFlags.ListOnly;
			}
		}
	}
}
