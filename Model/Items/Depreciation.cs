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

namespace MyInventory.Model
{
	public enum DepreciationMethod {
		Degressive,
		Linear,
		Progressive
	}
	
	public class Depreciation {
		static public double InvokeMethod(DepreciationMethod method, 
		                           double cost, 
		                           double life, 
		                           double usefulLife,
		                           double war) 
		{
			switch(method){
			case DepreciationMethod.Linear:
				return Linear(cost,life,usefulLife,war);
			case DepreciationMethod.Progressive:
				return Progressive(cost,life,usefulLife,war);
			case DepreciationMethod.Degressive:
				return Degressive(cost,life,usefulLife,war);
			default:
				return cost;
			}
		}
		
		
		static public double Linear(double cost, 
		                            double life, 
		                            double usefulLife, 
		                            double war)
		{
			if(life <= 0) return cost;
			if(usefulLife <= 0)	return 0;
			if(life>=usefulLife) return 0;
			
			//f(x)=        m       *  x + c 
			double y = -cost/usefulLife*life+cost;
			return y;
		}
		
		static public double Degressive(double cost, 
		                                double life, 
		                                double usefulLife, 
		                                double war) 
		{	
			if(life <= 0) return cost;
			if(usefulLife <= 0)	return 0;
			if(life>=usefulLife) return 0;
			
			//f(x)=  a*x^2 +b*x+ c 
			//f(u) = 0
			//f(0) = u
			//q = (2*c)/(u^2+u)
			//f(1) = c-u*q
			double c = cost;
			double u = usefulLife;
			double x = life;
			double a = c/(u+u*u);
			double b = -(a*u+c/u);
			double y = a*x*x+b*x+c;
			return y;
		}

	static public double Progressive(double cost,
		                             double life,
		                             double usefulLife,
		                             double war)
		{
		if(life <= 0) return cost;
		if(usefulLife <= 0)	return 0;
		if(life>=usefulLife) return 0;
			
		//f(x)=  a*x^2 +b*x+ c 
		//f(u) = 0
		//f(0) = u
		//q = (2*c)/(u^2+u)
		//f(1) = c-q
		double c = cost;
		double u = usefulLife;
		double x = life;
		double a = -c/(u+u*u);
		double b = a;
		double y = a*x*x+b*x+c;
		return y;
	}
}

}
