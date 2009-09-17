using System;
using System.Reflection;

namespace MyInventory.Model
{
	abstract public class MemoryImage
	{
		public MemoryImage(string path){
			_path = path;
		}
		
		public abstract void Load();
		
		private string _path;
		public string Path {
			get { return _path; }
		}
		
		public static MemoryImage Create(Inventory inv, string path){
			Type type = inv.Settings.MemoryImage;
			ConstructorInfo con = type.GetConstructor(new Type[]{typeof(string)});
			return (MemoryImage)con.Invoke(new object[]{path});
		}
	}
}
