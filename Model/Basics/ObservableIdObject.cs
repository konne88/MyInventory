using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MyInventory.Model
{
	public class ObservableIdObject : IdObject, INotifyPropertyChanged
	{
		public ObservableIdObject(uint id)
		: base(id)
		{}
		
		protected void SetNotifyProperty<T>(ref T prop, T val,string name){
			if(val == null && prop == null)
				return;
			
			if(
			   ( val == null || prop == null) ||
			   ( !prop.Equals(val) )
			)
			{
				prop = val;
				NotifyPropertyChanged(name);
			}
		}
		
		protected void NotifyPropertyChanged(string name)
        {
			if (PropertyChanged != null)
			{
			   PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
        }
		
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
