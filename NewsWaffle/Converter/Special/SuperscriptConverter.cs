using System;
using System.Text;
namespace NewsWaffle.Converter.Special
{
	public class SuperscriptConverter
	{

		public string Original { get; private set; }
		public string Converted { get; private set; }

		StringBuilder buffer = new StringBuilder();

		public bool IsFullyConverted { get; private set; } = true;

		public bool Convert(string s)
        {
			Original = s;
			Converted = "";

			buffer.Clear();
			IsFullyConverted = true;
			foreach(char c in s)
            {
				buffer.Append(ConvertChar(c));
				if(!IsFullyConverted)
                {
					return false;
                }
            }
			Converted = buffer.ToString();
			return IsFullyConverted;
        }

		public char ConvertChar(char c)
        {
			switch(c)
            {
				case '0':
					return '\u2070';
				case '1':
					return '\u00B9';
				case '2':
					return '\u00B2';
				case '3':
					return '\u00B3';
				case '4':
					return '\u2074';
				case '5':
					return '\u2075';
				case '6':
					return '\u2076';
				case '7':
					return '\u2077';
				case '8':
					return '\u2078';
				case '9':
					return '\u2079';

				//ASCII plus
				case '+':
				//small plus sign
				case '\uFE62':
				//full width plus sign
				case '\uFF0B':
					return '\u207A';

				//ASCII minus
				case '-':
				//small hyphen-minus
				case '\uFE63':
				//full width plus sign
				case '\uFF0D':
				//minus sign
				case '\u2212':
					return '\u207B';

				//ASCII equals
				case '=':
				//small equals sign
				case '\uFE66':
				//full width equals sign
				case '\uFF1D':
					return '\u207C';

				case '(':
					return '\u207D';
				case ')':
					return '\u207E';

				//Lowercase
				case 'a':
					return '\u1D43';
				case 'b':
					return '\u1D47';
				case 'c':
					return '\u1D9C';
				case 'd':
					return '\u1D48';
				case 'e':
					return '\u1D49';
				case 'f':
					return '\u1DA0';
				case 'g':
					return '\u1D4D';
				case 'h':
					return '\u02B0';
				case 'i':
					return '\u2071';
				case 'j':
					return '\u02B2';
				case 'k':
					return '\u1D4F';
				case 'l':
					return '\u02E1';
				case 'm':
					return '\u1D50';
				case 'n':
					return '\u207F';
				case 'o':
					return '\u1D52';
				case 'p':
					return '\u1D56';
				// there is no widely support Q subscript
				//case 'q':
				case 'r':
					return '\u02B3';
				case 's':
					return '\u02E2';
				case 't':
					return '\u1D57';
				case 'u':
					return '\u1D58';
				case 'v':
					return '\u1D5B';
				case 'w':
					return '\u02B7';
				case 'x':
					return '\u02E3';
				case 'y':
					return '\u02B8';
				case 'z':
					return '\u1DBB';

				//uppercase
				case 'A':
					return '\u1D2C';
				case 'B':
					return '\u1D2E';
				case 'D':
					return '\u1D30';
				case 'E':
					return '\u1D31';
				case 'G':
					return '\u1D33';
				case 'H':
					return '\u1D34';
				case 'I':
					return '\u1D35';
				case 'J':
					return '\u1D36';
				case 'K':
					return '\u1D37';
				case 'L':
					return '\u1D38';
				case 'M':
					return '\u1D39';
				case 'N':
					return '\u1D3A';
				case 'O':
					return '\u1D3C';
				case 'P':
					return '\u1D3E';
				case 'R':
					return '\u1D3F';
				case 'T':
					return '\u1D40';
				case 'U':
					return '\u1D41';
				case 'V':
					return '\u2C7D';
				case 'W':
					return '\u1D42';

				//greek
				case 'α':
					return '\u1D45';
				case 'β':
					return '\u1D5D';
				case 'γ':
					return '\u1D5E';
				case 'δ':
					return '\u1D5F';
				case '∊':
					return '\u1D4B';
				case 'θ':
					return '\u1DBF';
				case 'ι':
					return '\u1DA5';
				case 'Φ':
					return '\u1DB2';
				case 'φ':
					return '\u1D60';
				case 'χ':
					return '\u1D61';
			}
			IsFullyConverted = false;
			return c;
		}
	}
}

