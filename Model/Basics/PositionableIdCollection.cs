using System;
using System.Collections.Specialized;

namespace MyInventory.Model
{
	public class PositionableIdCollection<T,P> : IdCollection<T> 
		where P : INotifyCollectionChanged, new() 
		where T : IdObject
	{
		public PositionableIdCollection(){
			_positions = new P();
		}
		
		private P _positions;
		public P Positions {
			get {
				return _positions;
			}
		}
	}
}
