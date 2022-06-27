using System;
using System.Text;
namespace NewsWaffle.Converter.Special
{
	public class SubscriptConverter
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
            }
			Converted = buffer.ToString();
			return IsFullyConverted;
        }

		public char ConvertChar(char c)
        {
			switch(c)
            {
				case '0':
					return '\u2080';
				case '1':
					return '\u2081';
				case '2':
					return '\u2082';
				case '3':
					return '\u2083';
				case '4':
					return '\u2084';
				case '5':
					return '\u2085';
				case '6':
					return '\u2086';
				case '7':
					return '\u2087';
				case '8':
					return '\u2088';
				case '9':
					return '\u2089';

				//ASCII plus
				case '+':
				//small plus sign
				case '\uFE62':
				//full width plus sign
				case '\uFF0B':
					return '\u208A';

				//ASCII minus
				case '-':
				//small hyphen-minus
				case '\uFE63':
				//full width plus sign
				case '\uFF0D':
				//minus sign
				case '\u2212':
					return '\u208B';

				//ASCII equals
				case '=':
				//small equals sign
				case '\uFE66':
				//full width equals sign
				case '\uFF1D':
					return '\u208C';

				case '(':
					return '\u208D';
				case ')':
					return '\u208E';

				//some letters
				case 'a':
				case 'A':
					return '\u2090';
				case 'e':
				case 'E':
					return '\u2091';
				case 'h':
				case 'H':
					return '\u2095';
				case 'i':
				case 'I':
					return '\u1D62';
				case 'j':
				case 'J':
					return '\u2C7C';
				case 'k':
				case 'K':
					return '\u2096';
				case 'l':
				case 'L':
					return '\u2097';
				case 'm':
				case 'M':
					return '\u2098';
				case 'n':
				case 'N':
					return '\u2099';
				case 'o':
				case 'O':
					return '\u2092';
				case 'p':
				case 'P':
					return '\u209A';
				case 'r':
				case 'R':
					return '\u1D63';
				case 's':
				case 'S':
					return '\u209B';
				case 't':
				case 'T':
					return '\u209C';
				case 'u':
				case 'U':
					return '\u1D64';
				case 'v':
				case 'V':
					return '\u1D65';
				case 'x':
				case 'X':
					return '\u2093';

				//greek
				case 'β':
					return '\u1D66';
				case 'γ':
					return '\u1D67';
				case 'ρ':
					return '\u1D68';
				case 'φ':
					return '\u1D69';
				case 'χ':
					return '\u1D6A';
			}
			IsFullyConverted = false;
			return c;
		}
	}
}

