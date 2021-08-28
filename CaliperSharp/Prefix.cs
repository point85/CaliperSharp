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
using System.Collections.Generic;

namespace Point85.Caliper.UnitOfMeasure
{
	/// <summary>
	/// The Prefix class defines SI unit of measure prefixes as well as those
	/// found in computer science. 
	/// </summary>
	public class Prefix
	{
		// list of pre-defined prefixes
		private static List<Prefix> prefixes = new List<Prefix>();

		/// <summary> SI prefix 10^24 </summary>
		public static readonly Prefix YOTTA = new Prefix("yotta", "Y", 1.0E+24);
		/// <summary> SI prefix 10^21 </summary>
		public static readonly Prefix ZETTA = new Prefix("zetta", "Z", 1.0E+21);
		/// <summary> SI prefix 10^18 </summary>
		public static readonly Prefix EXA = new Prefix("exa", "E", 1.0E+18);
		/// <summary> SI prefix 10^15 </summary>
		public static readonly Prefix PETA = new Prefix("peta", "P", 1.0E+15);
		/// <summary> SI prefix 10^12 </summary>
		public static readonly Prefix TERA = new Prefix("tera", "T", 1.0E+12);
		/// <summary> SI prefix 10^9 </summary>
		public static readonly Prefix GIGA = new Prefix("giga", "G", 1.0E+09);
		/// <summary> SI prefix 10^6 </summary>
		public static readonly Prefix MEGA = new Prefix("mega", "M", 1.0E+06);
		/// <summary> SI prefix 10^3 </summary>
		public static readonly Prefix KILO = new Prefix("kilo", "k", 1.0E+03);
		/// <summary> SI prefix 10^2 </summary>
		public static readonly Prefix HECTO = new Prefix("hecto", "h", 1.0E+02);
		/// <summary> SI prefix 10 </summary>
		public static readonly Prefix DEKA = new Prefix("deka", "da", 1.0E+01);
		/// <summary> SI prefix 10^-1 </summary>
		public static readonly Prefix DECI = new Prefix("deci", "d", 1.0E-01);
		/// <summary> SI prefix 10^-2 </summary>
		public static readonly Prefix CENTI = new Prefix("centi", "c", 1.0E-02);
		/// <summary> SI prefix 10^-3 </summary>
		public static readonly Prefix MILLI = new Prefix("milli", "m", 1.0E-03);
		/// <summary> SI prefix 10^-6 </summary>
		public static readonly Prefix MICRO = new Prefix("micro", "\u03BC", 1.0E-06);
		/// <summary> SI prefix 10^-9 </summary>
		public static readonly Prefix NANO = new Prefix("nano", "n", 1.0E-09);
		/// <summary> SI prefix 10^-12 </summary>
		public static readonly Prefix PICO = new Prefix("pico", "p", 1.0E-12);
		/// <summary> SI prefix 10^-15 </summary>
		public static readonly Prefix FEMTO = new Prefix("femto", "f", 1.0E-15);
		/// <summary> SI prefix 10^-18 </summary>
		public static readonly Prefix ATTO = new Prefix("atto", "a", 1.0E-18);
		/// <summary> SI prefix 10^-21 </summary>
		public static readonly Prefix ZEPTO = new Prefix("zepto", "z", 1.0E-21);
		/// <summary> SI prefix 10^-24 </summary>
		public static readonly Prefix YOCTO = new Prefix("yocto", "y", 1.0E-24);

		// Digital information prefixes for bytes established by the International
		// Electrotechnical Commission (IEC) in 1998
		/// <summary>
		///  1024
		/// </summary>
		public static readonly Prefix KIBI = new Prefix("kibi", "Ki", 1024);
		/// <summary>
		/// 1.048576E+06
		/// </summary>
		public static readonly Prefix MEBI = new Prefix("mebi", "Mi", 1.048576E+06);
		/// <summary>
		/// 1.073741824E+09
		/// </summary>
		public static readonly Prefix GIBI = new Prefix("gibi", "Gi", 1.073741824E+09);

		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Symbol
		/// </summary>
		public string Symbol { get; private set; }

		/// <summary>
		/// Multiplying factor
		/// </summary>
		public double Factor { get; private set; }

		/// <summary>Construct a prefix</summary>
		/// <param name="name">Name</param>
		/// <param name="symbol"> Symbol </param>
		/// <param name="factor"> Numerical factor </param>
		public Prefix(string name, string symbol, double factor)
		{
			Name = name;
			Symbol = symbol;
			Factor = factor;

			prefixes.Add(this);
		}

		/// <summary>Find the prefix with the specified name</summary>
		/// 
		/// <param name="name"> Name of prefix</param>
		/// <returns> Prefix </returns>
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

		/// Find the prefix with the specified scaling factor
		/// 
		/// <param name="factor">Scaling factor</param>         
		/// <returns> Prefix </returns>
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

		/// <summary>Get the list of pre-defined prefixes</summary>
		/// 
		/// <returns> Prefix list</returns>
		public static List<Prefix> GetDefinedPrefixes()
		{
			return prefixes;
		}

		/// <summary>
		/// Override ToString()
		/// </summary>
		/// <returns>String representation</returns>
		public override string ToString()
		{
			return Name + ", " + Symbol + ", " + Factor;
		}
	}
}
