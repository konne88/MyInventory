using System;
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;
using System.Collections.Specialized;

namespace MyInventory.Model
{
	public abstract class Item : ObservableIdObject {
		//------------------------------------------------------------
		//                       Common
		//------------------------------------------------------------
		public Item(uint id, Items items) : base(id)
		{
			Items = items;
			Images = new Images(this);
			Tags = new ItemTags(this);
			Tags.CollectionChanged += OnTagsChanged;
			Images.CollectionChanged += OnImagesChanged;
			Images.PropertyChanged += OnImagesChanged;
			PropertyChanged += OnUpdateCache;
			// needs to be called manually since the delegate
			// wasn't set when the id was changed.
			UpdateCache();
		}
				
		public abstract void Serialize(XmlWriter writer);
		
		protected virtual void SerializeProperties(XmlWriter writer) {
			writer.WriteElementString("id",XmlConvert.ToString(Id));
			if(!string.IsNullOrEmpty(Name))
				writer.WriteElementString("name", Name);
			if(!string.IsNullOrEmpty(Memo))
				writer.WriteElementString("memo", Memo);
			Images.Serialize(writer);
			Tags.Serialize(writer);
		}
		
		public virtual void DeserializeProperties(XPathNavigator nav) {			
			XPathNodeIterator iter = nav.SelectChildren(XPathNodeType.Element);
			foreach(XPathNavigator current in iter){
				switch(current.LocalName){
				case "name":
					Name = current.Value;
					break;
				case "memo":		
					Memo = current.Value;
					break;
				case "images":
					Images.DeserializeProperties(current);
					break;
				case "tags":
					Tags.DeserializeProperties(current);
					break;
				}
			}
		}
		//------------------------------------------------------------
		//                       Cached Values
		//------------------------------------------------------------				
		private void OnUpdateCache(object o,PropertyChangedEventArgs args) {
			switch(args.PropertyName){
			case "Description":
				break;
			case "SearchString":
				break;
			default:
				UpdateCache();
				break;
			}
		}
		
		protected virtual void UpdateCache() {
			string desc = GetDescription().ToString();
			SetNotifyProperty(ref _desc, desc, "Description");
			string search = GetSearchString().ToLower();
			SetNotifyProperty(ref _search, search, "SearchString");
		}
		
		public virtual Descriptions GetDescription(){
			Descriptions ds = new Descriptions();
			
			if(!string.IsNullOrEmpty(Name))
				ds.Add(DescriptionPriority.VeryHigh,Name);
			if(!string.IsNullOrEmpty(Memo))
				ds.Add(DescriptionPriority.High,Memo);
			
			ds.Add(DescriptionPriority.Low,ItemType+" "+Id.ToString());
			ds.Add(DescriptionPriority.VeryLow,"");
			ds.Add(DescriptionPriority.VeryLow,"");
			
			return ds;
		}
		private string _desc;
		public string Description {
			get { return _desc; }
		}
		
		public virtual string GetSearchString() {
			string str = Id.ToString();
			if(!string.IsNullOrEmpty(Name)) str+=" "+Name;
			if(!string.IsNullOrEmpty(Memo)) str+=" "+Memo;
			str += " "+this.ItemType;
						
			return str;
		}
		
		public bool MatchesKey(string key){
			if(string.IsNullOrEmpty(key)) return true;
			if(string.IsNullOrEmpty(SearchString)) return false;
			
			string[] parts = key.ToLower().Split(' ');
			foreach(string s in parts)
				if(! string.IsNullOrEmpty(s))
					if(SearchString.Contains(s) == false)
						// if this item doesn't contain part of the key it doesn't match
						return false;
			
			return true;
		}
		
		private string _search;
		public string SearchString {
			get { return _search; }
		}
		
		private void OnImagesChanged(object sender, object args){
			NotifyPropertyChanged("Images");
		}
		
		private void OnTagsChanged(object sender, NotifyCollectionChangedEventArgs args){
			NotifyPropertyChanged("Tags");
		}
		
		//------------------------------------------------------------
		//                           Properties
		//------------------------------------------------------------
		private string _name;
		public string Name {
			set { SetNotifyProperty(ref _name, value, "Name"); }
			get { return _name; }
		}
		private string _memo;
		public string Memo {
			set { SetNotifyProperty(ref _memo, value, "Memo"); }
			get { return _memo; }
		}
		
		public abstract string ItemType {
			get;
		}
		
		readonly public Images Images;
		
		readonly public ItemTags Tags;
		
		readonly public Items Items;
		public Inventory Inventory {
			get { return Items.Inventory; }			
		}
	}
}
