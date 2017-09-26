using System.Text;
/*
MIT License

Copyright (c) 2016 Kent Randall

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
namespace CaliperSharp
{
	/**
 * This class represents an object that is identified by a name and symbol with
 * an optional description. Units of measure are such objects.
 * 
 * @author Kent Randall
 *
 */
	public abstract class Symbolic
	{
		protected Symbolic()
		{

		}

		protected Symbolic(string name, string symbol, string description)
		{
			this.Name = name;
			this.Symbol = symbol;
			this.Description = description;
		}

		// name, for example "speed of light"
		public string Name { get; set; }

		// symbol or abbreviation, e.g. "Vc"
		public string Symbol { get; set; }

		// description
		public string Description { get; set; }

		/**
 * Create a String representation
 */
		
	public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			// symbol
			if (Symbol != null)
			{
				sb.Append(" (").Append(Symbol);
			}

			// name
			if (Name != null)
			{
				sb.Append(", ").Append(Name);
			}

			// description
			if (Description != null)
			{
				sb.Append(", ").Append(Description).Append(')');
			}

			return sb.ToString();
		}


	}
}
