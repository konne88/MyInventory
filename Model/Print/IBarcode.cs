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
	public class BarcodeNoStopException : ApplicationException
	{
		public BarcodeNoStopException() : base("No Stop code was added to the end of the Barcode.")
		{}
	}
	
	public interface IBarcode
	{
		void WriteString(string str);
		void WriteEnd();
		
		int Silence {
			get;
			set;
		}
		int BarcodeWidth {
			get;
		}
		bool[] Bars {
			get;
		}
    }
}
