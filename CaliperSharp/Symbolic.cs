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

using System.Text;

namespace Point85.Caliper.UnitOfMeasure
{
	public interface ISymbolic
	{
		/// <summary>
		/// name, for example "speed of light"
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// symbol or abbreviation, e.g. "Vc"
		/// </summary>
		public string Symbol { get; set; }

		/// <summary>
		/// description
		/// </summary>
		public string Description { get; set; }
	}

	/// <summary>
	/// This class represents an object that is identified by a name and symbol with
	/// an optional description. Units of measure are such objects.
	/// </summary>
	public abstract class Symbolic : ISymbolic
	{
		/// <summary>
		/// name, for example "speed of light"
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// symbol or abbreviation, e.g. "Vc"
		/// </summary>
		public string Symbol { get; set; }

		/// <summary>
		/// description
		/// </summary>
		public string Description { get; set; }

		protected Symbolic()
		{
		}

		/// <summary>
		/// Create a symbolic object
		/// </summary>
		/// <param name="name">Name</param>
		/// <param name="symbol">Symbol</param>
		/// <param name="description">Description</param>
		protected Symbolic(string name, string symbol, string description)
		{
			this.Name = name;
			this.Symbol = symbol;
			this.Description = description;
		}

		public override string ToString() => ToString(Name, Symbol, Description);

		/// <summary>
		/// Build a string representation of a Symbolic object
		/// </summary>
		/// <returns>String value</returns>
		public static string ToString(string? name, string? symbol, string? description)
		{
			StringBuilder sb = new StringBuilder();

			bool hasContent = false;

			// symbol
			if (symbol != null)
			{
				if (!hasContent)
				{
					sb.Append(" (");
					hasContent = true;
				}
				sb.Append(symbol);
			}

			// name
			if (name != null)
			{
				if (!hasContent)
				{
					sb.Append(" (");
					hasContent = true;
				}
				else
				{
					sb.Append(", ");
				}
				sb.Append(name);
			}

			// description
			if (description != null)
			{
				if (!hasContent)
				{
					sb.Append(" (");
					hasContent = true;
				}
				else
				{
					sb.Append(", ");
				}
				sb.Append(description);
			}

			// close parenthesis if we opened one
			if (hasContent)
			{
				sb.Append(')');
			}

			return sb.ToString();
		}
	}
}
