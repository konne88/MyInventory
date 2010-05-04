using System;
using System.Collections;

namespace MyInventory.Model
{
	public enum DescriptionPriority {
		VeryLow,
		Low,
		LowMedium,
		Medium,
		HighMedium,
		High,
		VeryHigh
	}
	
	public class Descriptions
	{
		private class Pair {
			public Pair(DescriptionPriority priority,string val){
				Priority = priority;
				Value = val;
			}
			public DescriptionPriority Priority;
			public string Value;
		}
		
		private class ComparePriorities : IComparer {
			public int Compare(object a, object b){
				return (int)(((Pair)b).Priority) - (int)(((Pair)a).Priority);
			}
		}
		
		public Descriptions()
		{
			_list = new ArrayList();
		}
		
		public void Add(DescriptionPriority priority, string val){
			_list.Add(new Pair(priority,val));
		}
		
		public override string ToString (){
			return ToString(3);
		}
		
		public string ToString (int rows)
		{
			_list.Sort(new ComparePriorities());
			string s = "";
			uint i = 0;
			foreach(Pair pair in _list){
				s += pair.Value;
				if(i == rows-1)
					break;
				s+="\n";
				++i;
			}
			return s;
		}
		
		ArrayList _list;
	}
}
