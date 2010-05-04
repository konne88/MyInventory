using System;
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;

namespace MyInventory.Model
{
	public class ItemTag : INotifyPropertyChanged {
		public ItemTag(Tag tag)
		{
			Tag = tag;
		}
		
		private void OnTagChanged(object sender, PropertyChangedEventArgs args){
			if (PropertyChanged != null)
			   PropertyChanged(this, args);
		}
		
		private Tag _tag;
		public Tag Tag {
			set {
				if(!object.ReferenceEquals(Tag,value)){
					if(Tag != null)
						Tag.PropertyChanged -= OnTagChanged;
					_tag = value;
					if(Tag != null)
						Tag.PropertyChanged += OnTagChanged;
					if(PropertyChanged != null)
						PropertyChanged(this, new PropertyChangedEventArgs("Tag"));
				}
			}
			get {
				return _tag;
			}
		}
		
		public string Name {
			set { Tag.Name = value; }
			get { return Tag.Name; }
		}
		
		public uint Id {
			get { return Tag.Id; }
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
