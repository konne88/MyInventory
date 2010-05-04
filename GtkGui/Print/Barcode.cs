// infos on code 128
// http://en.wikipedia.org/wiki/Code_128

using Cairo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MyInventory.GtkGui {
	public class BarcodeNoStopException : ApplicationException
	{
		public BarcodeNoStopException() : base("No Stop code was added to the end of the Barcode.")
		{}
	}
	
	public class Barcode
    {
		public Barcode()
		{}
		
		public void Render(Context cr, double x, double y, double width, double height)
		{
			// throws an exception if no stop was added
			double barWidth = width/(double)BarcodeWidth;
			
			cr.Save();
			
			cr.LineWidth = barWidth;
			cr.Color = new Color(0, 0, 0);
			
			x += Silence*barWidth;
			foreach(int c in Code){
				foreach(bool b in CodeFont[c]){
					if(b){
						cr.MoveTo(x,y);
						cr.RelLineTo(0,height);
					}
					x+=barWidth;
				}
			}
			
			cr.Stroke();
			
			cr.Restore();
		}
		
		public int BarcodeWidth {
			get {
				if(Code[Code.Count-1] != 106)
					throw new BarcodeNoStopException();
				
				return Code.Count*11+2+Silence*2;
			}
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
		
		private Encoding LastEncodingUsed = Encoding.None;
		public int Silence = 10;
		public List<int> Code = new List<int>();
		
		static Barcode()
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
		private bool[][] CodeFont = {
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
		
		static public void GetReg(){
			Regex r =  new Regex("[01]{11}");
			MatchCollection res = r.Matches(
@" VALUE  	 WHICH REPRESENTS IN
CHARACTER SET 	ENCODING 	VALUE 	WHICH REPRESENTS IN
CHARACTER SET 	ENCODING
        A 	B 	C 	                                      A 	B 	C
00 	SP 	SP 	00 	11011001100 	53 	U 	U 	53 	11011101110
01 	! 	! 	01 	11001101100 	54 	V 	V 	54 	11101011000
02 	 	02 	11001100110 	55 	W 	W 	55 	11101000110
03 	# 	# 	03 	10010011000 	56 	X 	X 	56 	11100010110
04 	$ 	$ 	04 	10010001100 	57 	Y 	Y 	57 	11101101000
05 	% 	% 	05 	10001001100 	58 	Z 	Z 	58 	11101100010
06 	& 	& 	06 	10011001000 	59 	[ 	[ 	59 	11100011010
07 	' 	' 	07 	10011000100 	60 	\ 	\ 	60 	11101111010
08 	( 	( 	08 	10001100100 	61 	] 	] 	61 	11001000010
09 	) 	) 	09 	11001001000 	62 	^ 	^ 	62 	11110001010
10 	* 	* 	10 	11001000100 	63 	_ 	_ 	63 	10100110000
11 	+ 	+ 	11 	11000100100 	64 	NUL 	` 	64 	10100001100
12 	, 	, 	12 	10110011100 	65 	SOH 	a 	65 	10010110000
13 	- 	- 	13 	10011011100 	66 	STX 	b 	66 	10010000110
14 	. 	. 	14 	10011001110 	67 	ETX 	c 	67 	10000101100
15 	/ 	/ 	15 	10111001100 	68 	EOT 	d 	68 	10000100110
16 	0 	0 	16 	10011101100 	69 	ENQ 	e 	69 	10110010000
17 	1 	1 	17 	10011100110 	70 	ACK 	f 	70 	10110000100
18 	2 	2 	18 	11001110010 	71 	BEL 	g 	71 	10011010000
19 	3 	3 	19 	11001011100 	72 	BS 	h 	72 	10011000010
20 	4 	4 	20 	11001001110 	73 	HT 	i 	73 	10000110100
21 	5 	5 	21 	11011100100 	74 	LF 	j 	74 	10000110010
22 	6 	6 	22 	11001110100 	75 	VT 	k 	75 	11000010010
23 	7 	7 	23 	11101101110 	76 	FF 	l 	76 	11001010000
24 	8 	8 	24 	11101001100 	77 	CR 	m 	77 	11110111010
25 	9 	9 	25 	11100101100 	78 	SO 	n 	78 	11000010100
26 	: 	: 	26 	11100100110 	79 	SI 	o 	79 	10001111010
27 	; 	; 	27 	11101100100 	80 	DLE 	p 	80 	10100111100
28 	< 	< 	28 	11100110100 	81 	DC1 	q 	81 	10010111100
29 	= 	= 	29 	11100110010 	82 	DC2 	r 	82 	10010011110
30 	> 	> 	30 	11011011000 	83 	DC3 	s 	83 	10111100100
31 	? 	? 	31 	11011000110 	84 	DC4 	t 	84 	10011110100
32 	@ 	@ 	32 	11000110110 	85 	NAK 	u 	85 	10011110010
33 	A 	A 	33 	10100011000 	86 	SYN 	v 	86 	11110100100
34 	B 	B 	34 	10001011000 	87 	ETB 	w 	87 	11110010100
35 	C 	C 	35 	10001000110 	88 	CAN 	x 	88 	11110010010
36 	D 	D 	36 	10110001000 	89 	EM 	y 	89 	11011011110
37 	E 	E 	37 	10001101000 	90 	SUB 	z 	90 	11011110110
38 	F 	F 	38 	10001100010 	91 	ESC 	{ 	91 	11110110110
39 	G 	G 	39 	11010001000 	92 	FS 	| 	92 	10101111000
40 	H 	H 	40 	11000101000 	93 	GS 	} 	93 	10100011110
41 	I 	I 	41 	11000100010 	94 	RS 	~ 	94 	10001011110
42 	J 	J 	42 	10110111000 	95 	US 	DEL 	95 	10111101000
43 	K 	K 	43 	10110001110 	96 	FNC3 	FNC3 	96 	10111100010
44 	L 	L 	44 	10001101110 	97 	FNC2 	FNC2 	97 	11110101000
45 	M 	M 	45 	10111011000 	98 	SHIFT 	SHIFT 	98 	11110100010
46 	N 	N 	46 	10111000110 	99 	Code C 	Code C 	99 	10111011110
47 	O 	O 	47 	10001110110 	100 	Code B 	FNC4 	Code B 	10111101110
48 	P 	P 	48 	11101110110 	101 	FNC4 	Code A 	Code A 	11101011110
49 	Q 	Q 	49 	11010001110 	102 	FNC1 	FNC1 	FNC1 	11110101110
50 	R 	R 	50 	11000101110 	103 	START A 	START A 	START A 	11010000100
51 	S 	S 	51 	11011101000 	104 	START B 	START B 	START B 	11010010000
52 	T 	T 	52 	11011100010 	105 	START C 	START C 	START C 	11010011100
	  	  	");
	  	  	
			// STOP is 1100011101011
	  	  	
	  	  	ArrayList a = new ArrayList();
	  	  	ArrayList b = new ArrayList();
	  	  	
	  	  	bool odd = true;
	  	  	foreach(Match m in res){
	  	  		string s = m.ToString();
				
				if(odd) a.Add(s);
	  	  		else b.Add(s);
	  	  		
	  	  		odd = !odd;
	  	  	}
	  	  	
	  	  	string x = "bool[][] CodeFont = {";
			int c = 0;
	  	  	foreach (string s in a){
	  	  		x += "\n\tnew bool[] "+IntToBool(s)+",  // "+c.ToString();
				++c;
	  	  	}
	  	  	foreach (string s in b){
	  	  		x += "\n\tnew bool[] "+IntToBool(s)+",  // "+c.ToString();
				++c;
	  	  	}
	  	  	
	  	  	x = x + "\n};";
			
			Console.WriteLine(x);
			
			Console.WriteLine("");
			Console.WriteLine("");
			
			string esc = "";
			for(int s = 0; s < 2;++s){
				for(int t = 0; t < 16 ;++t){
					esc += "\\x" + s.ToString("X") + t.ToString("X");
				}
			}
			
			Console.WriteLine(esc);
		}
		
		static public string IntToBool(string s){
			string r = "{";
			foreach(char c in s){
				r  += (c == '1')?"true, ":"false,";
			}
			return r.Trim().Substring(0,r.Length-1) + "}";
		}
    }
}