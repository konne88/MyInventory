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

// infos on code 128
// http://en.wikipedia.org/wiki/Code_128

using System;
using System.Collections.Generic;

namespace MyInventory.Model
{
	public class Code128 : IBarcode
	{
		public Code128()
		{
			Silence = 10;
		}
		
		public void WriteString(string str)
		{
			WriteString(str,Encoding.B);
		}
		
		public void WriteString(string str, Encoding enc)
		{
			// activate the passed code
			if(LastEncodingUsed != enc){
				if(LastEncodingUsed == Encoding.None)
					Code.Add(102+((int)enc));
				else
					Code.Add(102-((int)enc));
			}
			LastEncodingUsed = enc;
			
			// write the passed characters
			if(enc == Encoding.C){
				if(str.Length % 2 != 0)
					throw new FormatException("If code C is used, the string may only contain numbers with an even amount of digits.");
				
				IEnumerator<char> enu = str.GetEnumerator();
				while(enu.MoveNext()){
					int i = int.Parse(new string(enu.Current,1))*10;
					enu.MoveNext();
					i+= int.Parse(new string(enu.Current,1));
					Code.Add(i);
				}
			}
			else {
				try {
					foreach(char c in str){
						if(enc == Encoding.A)
							Code.Add(EncodingsA[c]);
						else	// B and None
							Code.Add(EncodingsB[c]);
					}
				}
				catch(KeyNotFoundException ex) {
					throw new FormatException("String contains unsupported characters for this code.",ex);
				}
			}
		}
		
		public void WriteEnd()
		{
			if(Code.Count > 0){
				// calc the checksum
				int check = Code[0];
				for(int i=1 ; i<Code.Count ; ++i){
					check += Code[i]*i;
				}
				Code.Add(check%103);
				
				// write stop
				Code.Add(106);
			}
		}
		
		public enum Encoding {
			None,			
			A,  // upper case and control characters
			B,  // upper and lower case letters
			C   // double dense numbers
		};
		
		public int BarcodeWidth {
			get {
				if(Code[Code.Count-1] != 106)
					throw new BarcodeNoStopException();
				
				return Code.Count*11+2+Silence*2;
			}
		}
		
		public int Silence {
			get;
			set;
		}
		private Encoding LastEncodingUsed = Encoding.None;
		private List<int> Code = new List<int>();
		
		public bool[] Bars {
			get {
				List<bool> bars = new List<bool>();
				foreach(int c in Code){
					bars.AddRange(CodeFont[c]);
				}
				return bars.ToArray();
			}
		}
		
		static Code128()
		{
			string common = 
				" !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_";
			string encA = 
				"\x00\x01\x02\x03\x04\x05\x06\x07\x08\x09\x0A\x0B\x0C\x0D\x0E\x0F"+
				"\x10\x11\x12\x13\x14\x15\x16\x17\x18\x19\x1A\x1B\x1C\x1D\x1E\x1F";
			string encB = 
				"`abcdefghijklmnopqrstuvwxyz{|}~\x7F";
			
			int i = 0;
			foreach(char c in common){
				EncodingsA.Add(c,i);
				EncodingsB.Add(c,i);
				++i;
			}
			int a = i;
			foreach(char c in encA){
				EncodingsA.Add(c,a);
				++a;
			}
			int b = i;
			foreach(char c in encB){
				EncodingsB.Add(c,b);
				++b;
			}
		}
		
		private static Dictionary<char, int> EncodingsA = new Dictionary<char, int>();
		private static Dictionary<char, int> EncodingsB = new Dictionary<char, int>();
		private static bool[][] CodeFont = {
			new bool[] {true, true, false,true, true, false,false,true, true, false,false},  // 0
			new bool[] {true, true, false,false,true, true, false,true, true, false,false},  // 1
			new bool[] {true, true, false,false,true, true, false,false,true, true, false},  // 2
			new bool[] {true, false,false,true, false,false,true, true, false,false,false},  // 3
			new bool[] {true, false,false,true, false,false,false,true, true, false,false},  // 4
			new bool[] {true, false,false,false,true, false,false,true, true, false,false},  // 5
			new bool[] {true, false,false,true, true, false,false,true, false,false,false},  // 6
			new bool[] {true, false,false,true, true, false,false,false,true, false,false},  // 7
			new bool[] {true, false,false,false,true, true, false,false,true, false,false},  // 8
			new bool[] {true, true, false,false,true, false,false,true, false,false,false},  // 9
			new bool[] {true, true, false,false,true, false,false,false,true, false,false},  // 10
			new bool[] {true, true, false,false,false,true, false,false,true, false,false},  // 11
			new bool[] {true, false,true, true, false,false,true, true, true, false,false},  // 12
			new bool[] {true, false,false,true, true, false,true, true, true, false,false},  // 13
			new bool[] {true, false,false,true, true, false,false,true, true, true, false},  // 14
			new bool[] {true, false,true, true, true, false,false,true, true, false,false},  // 15
			new bool[] {true, false,false,true, true, true, false,true, true, false,false},  // 16
			new bool[] {true, false,false,true, true, true, false,false,true, true, false},  // 17
			new bool[] {true, true, false,false,true, true, true, false,false,true, false},  // 18
			new bool[] {true, true, false,false,true, false,true, true, true, false,false},  // 19
			new bool[] {true, true, false,false,true, false,false,true, true, true, false},  // 20
			new bool[] {true, true, false,true, true, true, false,false,true, false,false},  // 21
			new bool[] {true, true, false,false,true, true, true, false,true, false,false},  // 22
			new bool[] {true, true, true, false,true, true, false,true, true, true, false},  // 23
			new bool[] {true, true, true, false,true, false,false,true, true, false,false},  // 24
			new bool[] {true, true, true, false,false,true, false,true, true, false,false},  // 25
			new bool[] {true, true, true, false,false,true, false,false,true, true, false},  // 26
			new bool[] {true, true, true, false,true, true, false,false,true, false,false},  // 27
			new bool[] {true, true, true, false,false,true, true, false,true, false,false},  // 28
			new bool[] {true, true, true, false,false,true, true, false,false,true, false},  // 29
			new bool[] {true, true, false,true, true, false,true, true, false,false,false},  // 30
			new bool[] {true, true, false,true, true, false,false,false,true, true, false},  // 31
			new bool[] {true, true, false,false,false,true, true, false,true, true, false},  // 32
			new bool[] {true, false,true, false,false,false,true, true, false,false,false},  // 33
			new bool[] {true, false,false,false,true, false,true, true, false,false,false},  // 34
			new bool[] {true, false,false,false,true, false,false,false,true, true, false},  // 35
			new bool[] {true, false,true, true, false,false,false,true, false,false,false},  // 36
			new bool[] {true, false,false,false,true, true, false,true, false,false,false},  // 37
			new bool[] {true, false,false,false,true, true, false,false,false,true, false},  // 38
			new bool[] {true, true, false,true, false,false,false,true, false,false,false},  // 39
			new bool[] {true, true, false,false,false,true, false,true, false,false,false},  // 40
			new bool[] {true, true, false,false,false,true, false,false,false,true, false},  // 41
			new bool[] {true, false,true, true, false,true, true, true, false,false,false},  // 42
			new bool[] {true, false,true, true, false,false,false,true, true, true, false},  // 43
			new bool[] {true, false,false,false,true, true, false,true, true, true, false},  // 44
			new bool[] {true, false,true, true, true, false,true, true, false,false,false},  // 45
			new bool[] {true, false,true, true, true, false,false,false,true, true, false},  // 46
			new bool[] {true, false,false,false,true, true, true, false,true, true, false},  // 47
			new bool[] {true, true, true, false,true, true, true, false,true, true, false},  // 48
			new bool[] {true, true, false,true, false,false,false,true, true, true, false},  // 49
			new bool[] {true, true, false,false,false,true, false,true, true, true, false},  // 50
			new bool[] {true, true, false,true, true, true, false,true, false,false,false},  // 51
			new bool[] {true, true, false,true, true, true, false,false,false,true, false},  // 52
			new bool[] {true, true, false,true, true, true, false,true, true, true, false},  // 53
			new bool[] {true, true, true, false,true, false,true, true, false,false,false},  // 54
			new bool[] {true, true, true, false,true, false,false,false,true, true, false},  // 55
			new bool[] {true, true, true, false,false,false,true, false,true, true, false},  // 56
			new bool[] {true, true, true, false,true, true, false,true, false,false,false},  // 57
			new bool[] {true, true, true, false,true, true, false,false,false,true, false},  // 58
			new bool[] {true, true, true, false,false,false,true, true, false,true, false},  // 59
			new bool[] {true, true, true, false,true, true, true, true, false,true, false},  // 60
			new bool[] {true, true, false,false,true, false,false,false,false,true, false},  // 61
			new bool[] {true, true, true, true, false,false,false,true, false,true, false},  // 62
			new bool[] {true, false,true, false,false,true, true, false,false,false,false},  // 63
			new bool[] {true, false,true, false,false,false,false,true, true, false,false},  // 64
			new bool[] {true, false,false,true, false,true, true, false,false,false,false},  // 65
			new bool[] {true, false,false,true, false,false,false,false,true, true, false},  // 66
			new bool[] {true, false,false,false,false,true, false,true, true, false,false},  // 67
			new bool[] {true, false,false,false,false,true, false,false,true, true, false},  // 68
			new bool[] {true, false,true, true, false,false,true, false,false,false,false},  // 69
			new bool[] {true, false,true, true, false,false,false,false,true, false,false},  // 70
			new bool[] {true, false,false,true, true, false,true, false,false,false,false},  // 71
			new bool[] {true, false,false,true, true, false,false,false,false,true, false},  // 72
			new bool[] {true, false,false,false,false,true, true, false,true, false,false},  // 73
			new bool[] {true, false,false,false,false,true, true, false,false,true, false},  // 74
			new bool[] {true, true, false,false,false,false,true, false,false,true, false},  // 75
			new bool[] {true, true, false,false,true, false,true, false,false,false,false},  // 76
			new bool[] {true, true, true, true, false,true, true, true, false,true, false},  // 77
			new bool[] {true, true, false,false,false,false,true, false,true, false,false},  // 78
			new bool[] {true, false,false,false,true, true, true, true, false,true, false},  // 79
			new bool[] {true, false,true, false,false,true, true, true, true, false,false},  // 80
			new bool[] {true, false,false,true, false,true, true, true, true, false,false},  // 81
			new bool[] {true, false,false,true, false,false,true, true, true, true, false},  // 82
			new bool[] {true, false,true, true, true, true, false,false,true, false,false},  // 83
			new bool[] {true, false,false,true, true, true, true, false,true, false,false},  // 84
			new bool[] {true, false,false,true, true, true, true, false,false,true, false},  // 85
			new bool[] {true, true, true, true, false,true, false,false,true, false,false},  // 86
			new bool[] {true, true, true, true, false,false,true, false,true, false,false},  // 87
			new bool[] {true, true, true, true, false,false,true, false,false,true, false},  // 88
			new bool[] {true, true, false,true, true, false,true, true, true, true, false},  // 89
			new bool[] {true, true, false,true, true, true, true, false,true, true, false},  // 90
			new bool[] {true, true, true, true, false,true, true, false,true, true, false},  // 91
			new bool[] {true, false,true, false,true, true, true, true, false,false,false},  // 92
			new bool[] {true, false,true, false,false,false,true, true, true, true, false},  // 93
			new bool[] {true, false,false,false,true, false,true, true, true, true, false},  // 94
			new bool[] {true, false,true, true, true, true, false,true, false,false,false},  // 95
			new bool[] {true, false,true, true, true, true, false,false,false,true, false},  // 96
			new bool[] {true, true, true, true, false,true, false,true, false,false,false},  // 97
			new bool[] {true, true, true, true, false,true, false,false,false,true, false},  // 98
			new bool[] {true, false,true, true, true, false,true, true, true, true, false},  // 99
			new bool[] {true, false,true, true, true, true, false,true, true, true, false},  // 100
			new bool[] {true, true, true, false,true, false,true, true, true, true, false},  // 101
			new bool[] {true, true, true, true, false,true, false,true, true, true, false},  // 102
			new bool[] {true, true, false,true, false,false,false,false,true, false,false},  // 103
			new bool[] {true, true, false,true, false,false,true, false,false,false,false},  // 104
			new bool[] {true, true, false,true, false,false,true, true, true, false,false},  // 105
			new bool[] {true, true, false,false,false,true, true, true, false,true, false,true, true}  // 106
		};
	}
}
