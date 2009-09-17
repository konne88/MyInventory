using System;
using System.Xml;
using System.Xml.XPath;

namespace MyInventory.Model
{
	public class Image : ObservableIdObject
	{
		public Image(uint id, string fullsizePath, string previewPath, Images images) : base(id) {
			_images = images;
			
			_fullsizeImage = MemoryImage.Create(Inventory,fullsizePath);
			_previewImage = MemoryImage.Create(Inventory,previewPath);
			_previewImage.Load();
		}
		
		public void Serialize(XmlWriter writer) {
			writer.WriteStartElement("image");
			writer.WriteAttributeString("id",XmlConvert.ToString(Id));
			if(!string.IsNullOrEmpty(Memo))
				writer.WriteAttributeString("memo",Memo);
			writer.WriteEndElement();
		}
		
		public virtual void DeserializeProperties(XPathNavigator nav) {
			XPathNavigator node = nav.SelectSingleNode("@memo");
			if(node != null){
				Memo = node.Value;
			}
		}
		
		private string _memo;
		public string Memo {
			set { SetNotifyProperty(ref _memo,value,"Memo"); }
			get { return _memo; }
		}
		
		private MemoryImage _fullsizeImage;
		public MemoryImage FullsizeImage {
			get {
				_fullsizeImage.Load();
				return _fullsizeImage;
			}
		}
		
		private MemoryImage _previewImage;
		public MemoryImage PreviewImage {
			get {return _previewImage;}
		}
		
		private Images _images;
		public Images Images {
			get {return _images;}
		}
		public Item ParentItem {
			get {return Images.ParentItem;}
		}
		public Items Items {
			get {return ParentItem.Items;}
		}
		public Inventory Inventory {
			get {return Items.Inventory;}
		}
	}
}
