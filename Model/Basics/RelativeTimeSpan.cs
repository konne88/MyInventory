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
using System.Xml.Serialization;

namespace MyInventory.Model
{
	[XmlTypeAttribute("relative-time-span")]
	public struct RelativeTimeSpan {
		[XmlIgnoreAttribute]
		public int Years {
			get {
				return (int)((double)Months/12);
			}
			set {
				Months = value*12 + Months%12;
			}
		}
		[XmlIgnoreAttribute]
		public int MonthsInYear {
			get {
				return Months%12;
			}
			set {
				Months = Years*12+value;
			}
		}
		[XmlElementAttribute("months")]
		public int Months;
		public TimeSpan ToTimeSpan(DateTime spanStart){
			DateTime spanEnd = spanStart.AddMonths(Months);
			return spanEnd - spanStart;
		}
		public override string ToString() {
			string o = Math.Abs(Years).ToString() + ":";
			int m = Math.Abs(MonthsInYear);
			if(m < 10) o += "0";
			o += m.ToString();
			return o;
		}
		public string ToXmlString() {
			string x;
			if(Months < 0)
				x="-P";
			else if(Months == 0)
				return "P0Y";
			else
				x="P";
			
			int y = Math.Abs(Years);
			if(y > 0)
				x += y.ToString() + "Y";
			int m = Math.Abs(MonthsInYear);
			if(m > 0)
				x += m.ToString() + "M";
			
			return x;
		}
		static public RelativeTimeSpan ParseXml(string str) {
			str = str.Trim();			
			RelativeTimeSpan span = new RelativeTimeSpan();
			// check if it's a negative timespan
			bool negative = false;
			if(str[0] == '-'){
				negative = true;
				str = str.Substring(1);
			}
			// check if it starts with P
			if(str[0] != 'P')
				throw new FormatException("P is mandatory in duration");
			str = str.Substring(1);			
			// extract the Year
			try {
				int start = str.IndexOf("Y");
				if(start >= 0){
					span.Years = (int)uint.Parse(str.Substring(0,start));
					str = str.Substring(start+1);
				}
				// extract the Month
				start = str.IndexOf("M");
				if(start >= 0){
					span.MonthsInYear = (int)uint.Parse(str.Substring(0,start));
				}
			}
			catch {
				throw;
			}
			// make negative if nessesary
			if(negative)
				span.Months*=-1;
			
			return span;
		}
		static public RelativeTimeSpan Parse(string str) {
			RelativeTimeSpan s = new RelativeTimeSpan();
			int i= str.IndexOf(':');
			
			try {
				if(i == -1){
					s.Years = (int)uint.Parse(str);
				}
				else {
					s.Years = (int)uint.Parse(str.Substring(0,i));
					s.MonthsInYear = (int)uint.Parse(str.Substring(i+1));
				}
			}
			catch {
				throw;
			}
			return s;
		}
	}
}