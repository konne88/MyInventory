using System;
using System.ComponentModel;

namespace MyInventory.Model
{
	public class IdObject {
		public IdObject(uint id){
			_id = id;
		}
		
		private uint _id;
		public uint Id {
			get { return _id; }
		}
	}
}
