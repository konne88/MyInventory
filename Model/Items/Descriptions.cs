/* MyInventory - Keep track of your private assets. 
 * Copyright (C) 2008-2010 Konstantin Weitz
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 3
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

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
