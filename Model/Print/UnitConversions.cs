using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MyInventory.Model
{
	public static class UnitConversions
	{
		/* Converts a string of the form
		 * [number][unit]
		 * to the corresponding lenght in mm
		 * e.g.
		 * 5m 
		 * turns into
		 * 5000.0
		 */
		public static double LengthToMm(string len)
		{
			Match match = format.Match(len);
			if(!match.Success || match.Groups.Count > 3)
				throw new FormatException();
			
			double val = double.Parse(match.Groups[1].Value);
			
			if(match.Groups.Count == 3){
				string unit = match.Groups[2].Value;
				if(units.ContainsKey(unit)){
					val *= units[unit]*1000.0;
				}
				else {
					throw new FormatException();
				}
			}
			return val;
		}
		
		private static Dictionary<string,double> units;
		private static Regex format;
		
		static UnitConversions() {
			format = new Regex("^(.+?) *([a-zA-Z'\"µ]*)$");
			
			double inch = 25.4 / 1000.0;
			double point = 0.353 / 1000.0;
			double pica = 4.512 / 1000.0;
			
			units = new Dictionary<string, double>();
				units.Add("ym",0.0000000001);
			    units.Add("zm",0.000000001);
			    units.Add("am",0.00000001);
			    units.Add("fm",0.0000001);
			    units.Add("pm",0.000001);
			    units.Add("nm",0.00001);
			    units.Add("um",0.0001);
			    units.Add("µm",0.0001);
			    units.Add("mm",0.001);
				units.Add("",0.001);
			    units.Add("cm",0.01);
			    units.Add("dm",0.1);
				units.Add("m",1);
				units.Add("dam",10.0);
			    units.Add("hm",100.0);
			    units.Add("km",1000.0);
			    units.Add("Mm",10000.0);
			    units.Add("Gm",100000.0);
			    units.Add("Tm",1000000.0);
			    units.Add("Pm",10000000.0);
			    units.Add("Em",100000000.0);
			    units.Add("Zm",1000000000.0);
				units.Add("''",inch);
				units.Add("\"",inch);
				units.Add("inches",inch);
				units.Add("inch",inch);
				units.Add("in",inch);
				units.Add("pt",point);
				units.Add("point",point);
				units.Add("pc",pica);
		}
	}
}
