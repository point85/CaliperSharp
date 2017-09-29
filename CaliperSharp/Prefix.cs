using System;
using System.Collections.Generic;

namespace org.point85.uom
{
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

	/**
	 * The Prefix class defines SI unit of measure prefixes as well as those
	 * found in computer science.
	 */
	public class Prefix
	{
		// list of pre-defined prefixes
		private static List<Prefix> prefixes = new List<Prefix>();

		// SI prefix 10^24
		public static readonly Prefix YOTTA = new Prefix("yotta", "Y", 1.0E+24);
		// SI prefix 10^21
		public static readonly Prefix ZETTA = new Prefix("zetta", "Z", 1.0E+21);
		// SI prefix 10^18
		public static readonly Prefix EXA = new Prefix("exa", "E", 1.0E+18);
		// SI prefix 10^15
		public static readonly Prefix PETA = new Prefix("petta", "P", 1.0E+15);
		// SI prefix 10^12
		public static readonly Prefix TERA = new Prefix("tera", "T", 1.0E+12);
		// SI prefix 10^9
		public static readonly Prefix GIGA = new Prefix("giga", "G", 1.0E+09);
		// SI prefix 10^6
		public static readonly Prefix MEGA = new Prefix("mega", "M", 1.0E+06);
		// SI prefix 10^3
		public static readonly Prefix KILO = new Prefix("kilo", "k", 1.0E+03);
		// SI prefix 10^2
		public static readonly Prefix HECTO = new Prefix("hecto", "h", 1.0E+02);
		// SI prefix 10
		public static readonly Prefix DEKA = new Prefix("deka", "da", 1.0E+01);
		// SI prefix 10^-1
		public static readonly Prefix DECI = new Prefix("deci", "d", 1.0E-01);
		// SI prefix 10^-2
		public static readonly Prefix CENTI = new Prefix("centi", "c", 1.0E-02);
		// SI prefix 10^-3
		public static readonly Prefix MILLI = new Prefix("milli", "m", 1.0E-03);
		// SI prefix 10^-6
		public static readonly Prefix MICRO = new Prefix("micro", "\u03BC", 1.0E-06);
		// SI prefix 10^-9
		public static readonly Prefix NANO = new Prefix("nano", "n", 1.0E-09);
		// SI prefix 10^-12
		public static readonly Prefix PICO = new Prefix("pico", "p", 1.0E-12);
		// SI prefix 10^-15
		public static readonly Prefix FEMTO = new Prefix("femto", "f", 1.0E-15);
		// SI prefix 10^-18
		public static readonly Prefix ATTO = new Prefix("atto", "a", 1.0E-18);
		// SI prefix 10^-21
		public static readonly Prefix ZEPTO = new Prefix("zepto", "z", 1.0E-21);
		// SI prefix 10^-24
		public static readonly Prefix YOCTO = new Prefix("yocto", "y", 1.0E-24);

		// Digital information prefixes for bytes established by the International
		// Electrotechnical Commission (IEC) in 1998
		public static readonly Prefix KIBI = new Prefix("kibi", "Ki", 1024);
		//
		public static readonly Prefix MEBI = new Prefix("mebi", "Mi", 1.048576E+06);
		//
		public static readonly Prefix GIBI = new Prefix("gibi", "Gi", 1.073741824E+09);

		public Prefix(string prefixName, string symbol, double factor)
		{
			Name = prefixName;
			Symbol = symbol;
			Factor = factor;

			prefixes.Add(this);
		}

		/**
		 * Get the name of the prefix
		 * 
		 * @return prefix name
		 */
		public string Name { get; set; }


		/**
		 * Get the symbol for the prefix
		 * 
		 * @return symbol
		 */
		public string Symbol { get; set; }

		/**
		 * Get the scaling factor
		 * 
		 * @return Scaling factor
		 */
		public double Factor { get; set; }

		/**
		 * Find the prefix with the specified name
		 * 
		 * @param name
		 *            Name of prefix
		 * @return {@link Prefix}
		 */
		public static Prefix FromName(string name)
		{
			Prefix prefix = null;

			foreach (Prefix p in prefixes)
			{
				if (p.Name.Equals(name))
				{
					prefix = p;
					break;
				}
			}

			return prefix;
		}

		/**
		 * Find the prefix with the specified scaling factor
		 * 
		 * @param factor
		 *            Scaling factor
		 * @return {@link Prefix}
		 */
		public static Prefix FromFactor(double factor)
		{
			Prefix prefix = null;

			foreach (Prefix p in prefixes)
			{
				if (p.Factor.CompareTo(factor) == 0)
				{
					prefix = p;
					break;
				}
			}

			return prefix;
		}

		/**
		 * Get the list of pre-defined prefixes
		 * 
		 * @return Prefix list
		 */
		public static List<Prefix> GetDefinedPrefixes()
		{
			return prefixes;
		}

		/**
		 * Create a String representation of this Prefix
		 */
		public override string ToString()
		{
			return Name + ", " + Symbol + ", " + Factor;
		}
	}

}
