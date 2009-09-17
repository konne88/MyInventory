using System;
using System.Collections;
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;
using System.Collections.Specialized;

namespace MyInventory.Model
{
	public class Product : Item {
		public Product(uint id,Items items):base(id,items)
		{
			PropertyChanged += OnUpdateValue;
		}
		
		public override string GetSearchString() {	
			string str = base.GetSearchString();
			if(!string.IsNullOrEmpty(Brand))  str+=" "+Brand;
			if(!string.IsNullOrEmpty(Model))  str+=" "+Model;
			if(!string.IsNullOrEmpty(Serial)) str+=" "+Serial;
			if(Warranty.Months != 0)  str+=" "+Warranty.ToString();
			str+=" "+Weight.ToString("F3")+"kg";
			foreach(ItemTag t in Tags)
				str+=t.Name+" ";
			return str;
		}
		
		public override Descriptions GetDescription(){
			Descriptions ds = base.GetDescription();
			
			if(!string.IsNullOrEmpty(Brand)) 
				ds.Add(DescriptionPriority.Medium,Brand);
			if(!string.IsNullOrEmpty(Model))
				ds.Add(DescriptionPriority.Medium,Model);
			if(!string.IsNullOrEmpty(Serial))
				ds.Add(DescriptionPriority.Medium,Serial);
			if(Tags.Count>0){
				int i = 0;
				string[] tags = new string[Tags.Count];
				foreach(ItemTag t in Tags){
					tags[i] = t.Name;
					++i;
				}
				ds.Add(DescriptionPriority.Medium,string.Join(", ",tags));
			}
			if(Weight!=0)
				ds.Add(DescriptionPriority.LowMedium,Weight.ToString("F3")+"kg");
			if(Warranty.Months!=0)
				ds.Add(
				       DescriptionPriority.LowMedium,
				       ((Warranty.Months<0)?"Lifetime":Warranty.ToString()+" year")+" warranty"
				       );
			
			return ds;
		}
		
		private void OnUpdateValue(object o,PropertyChangedEventArgs args) {
			string prop = args.PropertyName;
			if(prop=="PurchaseDate" || 
			   prop=="UsefulLife" || 
			   prop=="Warranty" || 
			   prop=="DepreciationMethod" ||
			   prop=="Cost" ||
			   prop=="Owner")
			{
				double v = 0;
				if(Owner == true){
					v = Cost;
					if(PurchaseDate != null){
						// everything is in days
						double life = (DateTime.Now-(DateTime)PurchaseDate).TotalDays;
						double useful = UsefulLife.ToTimeSpan((DateTime)PurchaseDate).TotalDays;
						double war = Warranty.ToTimeSpan((DateTime)PurchaseDate).TotalDays;
						
						if(useful<=double.Epsilon)
							useful = war;
						if(useful<=double.Epsilon)
							useful = Inventory.Settings.UsefulLifeStandard;
						
						v = Depreciation.InvokeMethod(DepreciationMethod,Cost,life,useful,war);
					}
				}
				SetNotifyProperty(ref _value, v, "Value");
			}
		}
				
		public override void Serialize(XmlWriter writer) {
			writer.WriteStartElement("product");
			SerializeProperties(writer);
			writer.WriteEndElement();
		}
			
		protected override void SerializeProperties(XmlWriter writer) {
			base.SerializeProperties(writer);
			
			if(!string.IsNullOrEmpty(Serial))
				writer.WriteElementString("serial", Serial);
			if(!string.IsNullOrEmpty(Brand))
			    writer.WriteElementString("brand", Brand);
			if(!string.IsNullOrEmpty(Model))
				writer.WriteElementString("model", Model);
			if(PurchaseDate != null){
				writer.WriteStartElement("purchase-date");
				writer.WriteValue((DateTime)PurchaseDate);
				writer.WriteEndElement();
			}
			if(PurchasedAmount!=0)
				writer.WriteElementString("purchased-amount",XmlConvert.ToString(PurchasedAmount));
			if(Weight!=0.0)
				writer.WriteElementString("weight",XmlConvert.ToString(Weight));
			if(Cost!=0.0)
				writer.WriteElementString("cost",XmlConvert.ToString(Cost));
			
			switch(LabelMethod){
			case LabelMethod.None:
				break;
			case LabelMethod.Print:
				writer.WriteElementString("label-method","print");
				break;
			case LabelMethod.Paint:
				writer.WriteElementString("label-method","paint");
				break;
			}
			
			switch(DepreciationMethod){
			case DepreciationMethod.Degressive:
				break;
			case DepreciationMethod.Progressive:
				writer.WriteElementString("depreciation-method","progressive");
				break;
			case DepreciationMethod.Linear:
				writer.WriteElementString("depreciation-method","linear");
				break;
			}
			
			if(Warranty.Months != 0)
				writer.WriteElementString("warranty",Warranty.ToXmlString());
			if(UsefulLife.Months != 0)
				writer.WriteElementString("useful-life",UsefulLife.ToXmlString());
		}
		
		public override void DeserializeProperties(XPathNavigator nav) {
			base.DeserializeProperties(nav);
			
			XPathNodeIterator iter = nav.SelectChildren(XPathNodeType.Element);
			foreach(XPathNavigator current in iter){
				switch(current.LocalName){
				case "serial":
					Serial = current.Value;
					break;
				case "brand":
					Brand = current.Value;
					break;
				case "model":
					Model = current.Value;
					break;
				case "purchase-date":
					PurchaseDate = current.ValueAsDateTime;
					break;
				case "purchased-amount":
					PurchasedAmount = Convert.ToUInt32(current.Value);
					break;
				case "weight":
					Weight = current.ValueAsDouble;
					break;
				case "label-method":
					switch(current.Value){
					case "none":
						LabelMethod = LabelMethod.None;
						break;
					case "print":
						LabelMethod = LabelMethod.Print;
						break;
					case "paint":
						LabelMethod = LabelMethod.Paint;
						break;
					}
					break;
				case "cost":
					Cost = current.ValueAsDouble;
					break;
				case "warranty":
					Warranty = RelativeTimeSpan.ParseXml(current.Value);
					break;
				case "useful-life":
					UsefulLife = RelativeTimeSpan.ParseXml(current.Value);
					break;
				case "depreciation-method":
					switch(current.Value){
					case "linear":
						DepreciationMethod = DepreciationMethod.Linear;
						break;
					case "progressive":
						DepreciationMethod = DepreciationMethod.Progressive;
						break;
					case "degressive":
						DepreciationMethod = DepreciationMethod.Degressive;
						break;
					}
					break;
				}
			}
		}
		
		//------------------------------------------------------------
		//                       Properties
		//------------------------------------------------------------
		public override string ItemType {
			get {return "Product";}
		}
		private string _serial;
		public string Serial {
			set { SetNotifyProperty(ref _serial, value, "Serial"); }
			get { return _serial; }
		}
		private string _brand;
		public string Brand {
			set { SetNotifyProperty(ref _brand, value, "Brand"); }
			get { return _brand; }
		}
		private string _model;
		public string Model {
			set { SetNotifyProperty(ref _model, value, "Model"); }
			get { return _model; }
		}
		private DateTime? _purchased;
		public DateTime? PurchaseDate {
			set { SetNotifyProperty(ref _purchased, value, "PurchaseDate"); }
			get { return _purchased; }
		}
		private RelativeTimeSpan _usefulLife;
		public RelativeTimeSpan UsefulLife {
			set { SetNotifyProperty(ref _usefulLife, value, "UsefulLife"); }
			get { return _usefulLife; }
		}
		private uint _purchasedAmount;
		public uint PurchasedAmount {
			set { SetNotifyProperty(ref _purchasedAmount, value, "PurchasedAmount"); }
			get { return _purchasedAmount; }
		}
		private RelativeTimeSpan _warranty;
		public RelativeTimeSpan Warranty {
			set { SetNotifyProperty(ref _warranty, value, "Warranty"); }
			get { return _warranty; }
		}
		private double _weight;
		public double Weight {// in kilos
			set { SetNotifyProperty(ref _weight, value, "Weight"); }
			get { return _weight; }
		}
		private LabelMethod _labelMethod = LabelMethod.None;
		public LabelMethod LabelMethod {
			set { SetNotifyProperty(ref _labelMethod, value, "LabelMethod"); }
			get { return _labelMethod; }
		}
		private bool _owner = true;
		public bool Owner {
			set { SetNotifyProperty(ref _owner, value, "Owner"); }
			get { return _owner; }
		}
		private double _cost;
		public double Cost {   // in euros 
			set { SetNotifyProperty(ref _cost, value, "Cost"); }
			get { return _cost; }
		}
		private DepreciationMethod _depreciationMethod = DepreciationMethod.Degressive;
		public DepreciationMethod DepreciationMethod {
			set { SetNotifyProperty(ref _depreciationMethod, value, "DepreciationMethod"); }
			get { return _depreciationMethod; }
		}
		private double _value;
		public double Value {
			get {
				return _value;
			}
		}

	}
}
