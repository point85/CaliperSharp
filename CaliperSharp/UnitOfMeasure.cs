﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
 * <p>
 * A UnitOfMeasure can have a linear conversion (y = ax + b) to another unit of
 * measure in the same internationally recognized measurement system of
 * International Customary, SI, US or British Imperial. Or, the unit of measure
 * can have a conversion to another custom unit of measure. It is owned by the
 * unified {@link MeasurementSystem} defined by this project.
 * </p>
 * 
 * <p>
 * A unit of measure is categorized by scalar (simple unit), quotient (divisor
 * and dividend units), product (multiplier and multiplicand units) or power
 * (unit with an integral exponent). More than one representation of a unit of
 * measure is possible. For example, a unit of "per second" could be a quotient
 * of "1/s" (e.g. an inverted second) or a power of s^-1.
 * </p>
 * 
 * <p>
 * A unit of measure also has an enumerated {@link UnitType} (for example LENGTH
 * or MASS) and a unique {@link Unit} discriminator (for example METRE). <br>
 * A basic unit (a.k.a fundamental unit in the SI system) can have a bridge
 * conversion to another basic unit in another recognized measurement system.
 * This conversion is defined unidirectionally. For example, an International
 * Customary foot is 0.3048 SI metres. The conversion from metre to foot is just
 * the inverse of this relationship.
 * </p>
 * 
 * <p>
 * A unit of measure has a base symbol, for example 'm' for metre. A base symbol
 * is one that consists only of the symbols for the base units of measure. In
 * the SI system, the base units are well-defined. The derived units such as
 * Newton all have base symbols expressed in the fundamental units of length
 * (metre), mass (kilogram), time (second), temperature (Kelvin), plane angle
 * (radian), electric charge (Coulomb) and luminous intensity (candela). In the
 * US and British systems, base units are not defined. Caliper uses foot for
 * length, pound mass for mass and Rankine for temperature. This base symbol is
 * used in unit of measure conversions to uniquely identify the target unit.
 * </p>
 * <p>
 * The SI system has defined prefixes (e.g. "centi") for 1/100th of another unit
 * (e.g. metre). Instead of defining all the possible unit of measure
 * combinations, the {@link MeasurementSystem} is able to create units by
 * specifying the {@link Prefix} and target unit of measure. Similarly, computer
 * science has defined prefixes for bytes (e.g. "mega").
 * 
 * @author Kent Randall
 *
 */
	public class UnitOfMeasure : Symbolic, IComparable<UnitOfMeasure>
	{
		// UOM types
		public enum MeasurementType
		{
			SCALAR, PRODUCT, QUOTIENT, POWER
		};

		// maximum length of the symbol
		private const int MAX_SYMBOL_LENGTH = 16;

		// multiply, divide and power symbols
		internal static readonly char MULT = (char)0xB7;
		internal static readonly char DIV = '/';
		internal static readonly char POW = '^';
		internal static readonly char SQ = (char)0xB2;
		internal static readonly char CUBED = (char)0xB3;
		internal static readonly char LP = '(';
		internal static readonly char RP = ')';
		internal static readonly char ONE_CHAR = '1';

		// registry of unit conversion factor
		private Dictionary<UnitOfMeasure, decimal> ConversionRegistry = new Dictionary<UnitOfMeasure, decimal>();

		// conversion to another Unit of Measure in the same recognized measurement
		// system (y = ax + b)
		// scaling factor (a)
		//private decimal ScalingFactor;
		public decimal ScalingFactor { get; set; }

		// offset (b)
		//private decimal offset;
		public decimal Offset { get; set; }

		// x-axis unit
		//private UnitOfMeasure abscissaUnit;
		public UnitOfMeasure AbscissaUnit { get; set; }

		// unit enumerations for the various systems of measurement, e.g. KILOGRAM
		public Unit Unit { get; set; }

		// unit type, e.g. MASS
		public UnitType UnitType { get; set; } = UnitType.UNCLASSIFIED;

		// conversion to another Unit of Measure in a different measurement system
		public decimal BridgeScalingFactor { get; set; }

		// offset (b)
		public decimal BridgeOffset { get; set; }

		// x-axis unit
		public UnitOfMeasure BridgeAbscissaUnit { get; set; }

		// cached base symbol
		private string BaseSymbol { get; set; }

		// user-defined category
		public string Category { get; set; } = MeasurementSystem.GetUnitString("default.category.text");

		// base UOMs and exponents for a product of two power UOMs follow
		// power base unit, product multiplier or quotient dividend
		internal UnitOfMeasure UOM1 { get; set; }

		// product multiplicand or quotient divisor
		internal UnitOfMeasure UOM2 { get; set; }

		// exponent
		internal Nullable<int> Exponent1 { get; set; }

		// second exponent
		internal Nullable<int> Exponent2 { get; set; }

		/**
 * Construct a default unit of measure
 */
		public UnitOfMeasure() : base()
		{

		}

		internal UnitOfMeasure(UnitType type, string name, string symbol, string description) : base(name, symbol, description)
		{
			UnitType = type;
		}

		private void SetPowerProduct(UnitOfMeasure uom1, int exponent1)
		{
			UOM1 = uom1;
			Exponent1 = exponent1;
		}

		private void SetPowerProduct(UnitOfMeasure uom1, int exponent1, UnitOfMeasure uom2, int exponent2)
		{
			UOM1 = uom1;
			Exponent1 = exponent1;
			UOM2 = uom2;
			Exponent2 = exponent2;
		}

		/**
 * Get the measurement type
 * 
 * @return {@link MeasurementType}
 */
		public MeasurementType GetMeasurementType()
		{
			MeasurementType type = MeasurementType.SCALAR;

			if (Exponent2 != null && Exponent2 < 0)
			{
				type = MeasurementType.QUOTIENT;
			}
			else if (Exponent2 != null && Exponent2 > 0)
			{
				type = MeasurementType.PRODUCT;
			}
			else if (UOM1 != null && Exponent1 != null)
			{
				type = MeasurementType.POWER;
			}

			return type;
		}

		private Reducer GetBaseMap()
		{
			lock (new object())
			{
				Reducer reducer = new Reducer();
				reducer.Explode(this);
				return reducer;
			}
		}

		/**
	* Get the unit of measure's symbol in the fundamental units for that
	* system. For example a Newton is a kg.m/s2.
	* 
	* @return Base symbol
	* @throws Exception
	*             Exception
*/
		public string GetBaseSymbol()
		{
			lock (new object())
			{
				if (BaseSymbol == null)
				{
					Reducer powerMap = GetBaseMap();
					BaseSymbol = powerMap.BuildBaseString();
				}
				return BaseSymbol;
			}
		}

		/**
 * Check to see if this unit of measure has a conversion to another unit of
 * measure other than itself.
 * 
 * @return True if it does not
 */
		public bool IsTerminal()
		{
			return this.Equals(AbscissaUnit) ? true : false;
		}

		/**
 * Get the exponent
 * 
 * @return Exponent
 */
		public int? GetPowerExponent()
		{
			return Exponent1;
		}

		/**
 * Get the dividend unit of measure
 * 
 * @return {@link UnitOfMeasure}
 */
		public UnitOfMeasure GetDividend()
		{
			return UOM1;
		}

		/**
		 * Get the divisor unit of measure
		 * 
		 * @return {@link UnitOfMeasure}
		 */
		public UnitOfMeasure GetDivisor()
		{
			return UOM2;
		}

		/**
 * Set the base unit of measure and exponent
 * 
 * @param base
 *            Base unit of measure
 * @param exponent
 *            Exponent
 * @throws Exception
 *             Exception
 */
		public void SetPowerUnit(UnitOfMeasure baseUOM, int exponent)
		{
			if (baseUOM == null)
			{
				String msg = String.Format(MeasurementSystem.GetMessage("base.cannot.be.null"), Symbol);
				throw new Exception(msg);
			}
			SetPowerProduct(baseUOM, exponent);
		}

		// generate a symbol for units of measure created as the result of
		// intermediate multiplication and division operations. These symbols are
		// not cached.
		private static string GenerateIntermediateSymbol()
		{
			return Environment.TickCount.ToString("X");
		}

		internal static String GeneratePowerSymbol(UnitOfMeasure baseUOM, int exponent)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(baseUOM.Symbol).Append(POW).Append(exponent);
			return sb.ToString();
		}

		internal static string GenerateProductSymbol(UnitOfMeasure multiplier, UnitOfMeasure multiplicand)
		{
			StringBuilder sb = new StringBuilder();

			if (multiplier.Equals(multiplicand))
			{
				sb.Append(multiplier.Symbol).Append(SQ);
			}
			else
			{
				sb.Append(multiplier.Symbol).Append(MULT).Append(multiplicand.Symbol);
			}
			return sb.ToString();
		}

		internal static string GenerateQuotientSymbol(UnitOfMeasure dividend, UnitOfMeasure divisor)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(dividend.Symbol).Append(DIV).Append(divisor.Symbol);
			return sb.ToString();
		}

		UnitOfMeasure ClonePower(UnitOfMeasure uom)
		{

			UnitOfMeasure newUOM = new UnitOfMeasure();
			newUOM.UnitType = UnitType;

			// check if quotient
			int exponent = 1;
			if (GetPowerExponent() != null)
			{
				exponent = GetPowerExponent().Value;
			}

			UnitOfMeasure one = MeasurementSystem.GetSystem().GetOne();
			if (GetMeasurementType().Equals(MeasurementType.QUOTIENT))
			{
				if (GetDividend().Equals(one))
				{
					exponent = Exponent2.Value;
				}
				else if (GetDivisor().Equals(one))
				{
					exponent = Exponent1.Value;
				}
			}

			newUOM.SetPowerUnit(uom, exponent);
			String symbol = UnitOfMeasure.GeneratePowerSymbol(uom, exponent);
			newUOM.Symbol = symbol;
			newUOM.Name = symbol;

			return newUOM;
		}

		// this method is for optimization of decimal multiplication
		internal static decimal DecimalMultiply(decimal a, decimal b)
		{
			decimal value;

			if (b.CompareTo(decimal.One) == 0)
			{
				value = a;
			}
			else
			{
				value = decimal.Multiply(a, b);
			}
			return value;
		}

		// this method is for optimization of decimal division
		internal static decimal DecimalDivide(decimal a, decimal b)
		{
			decimal value;

			if (b.CompareTo(decimal.One) == 0)
			{
				value = a;
			}
			else
			{
				value = decimal.Divide(a, b);
			}
			return value;
		}

		// this method is for optimization of decimal exponentiation
		internal static decimal DecimalPower(decimal powerBase, int exponent)
		{
			decimal value;

			if (exponent == 1)
			{
				value = powerBase;
			}
			else if (exponent == 0)
			{
				value = decimal.One;
			}
			else
			{
				value = (decimal)Math.Pow((double)powerBase, exponent);
			}
			return value;
		}

		public override int GetHashCode()
		{
			int hashName = Name == null ? 0 : Name.GetHashCode();
			int hashAge = Symbol.GetHashCode();

			return hashName ^ hashAge;
		}

		/**
		* Compare this unit of measure to another one.
		* 
		* @param other
		*            unit of measure
		* @return -1 if less than, 0 if equal and 1 if greater than
*/
		int IComparable<UnitOfMeasure>.CompareTo(UnitOfMeasure other)
		{
			return Symbol.CompareTo(other.Symbol);
		}

		// reduce a unit of measure to its most basic scalar units of measure.
		private class Reducer
		{
			private static readonly int MAX_RECURSIONS = 100;

			// starting level
			private static readonly int STARTING_LEVEL = -1;

			// UOMs and their exponents
			private Dictionary<UnitOfMeasure, int> Terms { get; set; } = new Dictionary<UnitOfMeasure, int>();

			// the overall scaling factor
			private decimal MapScalingFactor { get; set; } = decimal.One;

			// list of exponents down a path to the leaf UOM
			private List<int> PathExponents = new List<int>();

			// recursion counter
			private int Counter = 0;

			internal Reducer()
			{

			}

			internal void Explode(UnitOfMeasure unit)
			{
				ExplodeRecursively(unit, STARTING_LEVEL);
			}

			private void ExplodeRecursively(UnitOfMeasure unit, int level)
			{
				if (++Counter > MAX_RECURSIONS)
				{
					string msg = String.Format(MeasurementSystem.GetMessage("circular.references"),
							unit.Symbol);
					throw new Exception(msg);
				}

				// down a level
				level++;

				// scaling factor to abscissa unit
				decimal scalingFactor = unit.ScalingFactor;

				// explode the abscissa unit
				UnitOfMeasure abscissaUnit = unit.AbscissaUnit;

				UnitOfMeasure uom1 = abscissaUnit.UOM1;
				UnitOfMeasure uom2 = abscissaUnit.UOM2;

				int? exp1 = abscissaUnit.Exponent1;
				int? exp2 = abscissaUnit.Exponent2;

				// scaling
				if (PathExponents.Count > 0)
				{
					int lastExponent = PathExponents[PathExponents.Count - 1];

					// compute the overall scaling factor
					decimal factor = decimal.One;
					for (int i = 0; i < Math.Abs(lastExponent); i++)
					{
						factor = UnitOfMeasure.DecimalMultiply(factor, scalingFactor);
					}

					if (lastExponent < 0)
					{
						MapScalingFactor = UnitOfMeasure.DecimalDivide(MapScalingFactor, factor);
					}
					else
					{
						MapScalingFactor = UnitOfMeasure.DecimalMultiply(MapScalingFactor, factor);
					}
				}
				else
				{
					MapScalingFactor = scalingFactor;
				}

				if (uom1 == null)
				{
					if (!abscissaUnit.IsTerminal())
					{
						// keep exploding down the conversion path
						decimal currentMapFactor = MapScalingFactor;
						MapScalingFactor = decimal.One;
						ExplodeRecursively(abscissaUnit, STARTING_LEVEL);
						MapScalingFactor = UnitOfMeasure.DecimalMultiply(MapScalingFactor, currentMapFactor);
					}
					else
					{

						// multiply out all of the exponents down the path
						int pathExponent = 1;

						foreach (int exp in PathExponents)
						{
							pathExponent = pathExponent * exp;
						}

						bool invert = pathExponent < 0 ? true : false;

						for (int i = 0; i < Math.Abs(pathExponent); i++)
						{
							AddTerm(abscissaUnit, invert);
						}
					}
				}
				else
				{
					// explode UOM #1
					PathExponents.Add(exp1.Value);
					ExplodeRecursively(uom1, level);
					PathExponents.Remove(level);
				}

				if (uom2 != null)
				{
					// explode UOM #2
					PathExponents.Add(exp2.Value);
					ExplodeRecursively(uom2, level);
					PathExponents.Remove(level);
				}

				// up a level
				level--;
			}

			// add a UOM and exponent pair to the map of reduced terms
			private void AddTerm(UnitOfMeasure uom, bool invert)
			{
				int unitPower = 1;
				int power = 0;

				if (!invert)
				{
					// get existing power
					if (!Terms.ContainsKey(uom))
					{
						// add first time
						power = unitPower;
					}
					else
					{
						// increment existing power
						if (!uom.Equals(MeasurementSystem.GetSystem().GetOne()))
						{
							power = Terms[uom] + unitPower;
						}
					}
				}
				else
				{
					// denominator with negative powers
					if (!Terms.ContainsKey(uom))
					{
						// add first time
						power = -unitPower;
					}
					else
					{
						// decrement existing power
						if (!uom.Equals(MeasurementSystem.GetSystem().GetOne()))
						{
							power = Terms[uom] - unitPower;
						}
					}
				}

				if (power == 0)
				{
					Terms.Remove(uom);
				}
				else
				{
					if (!uom.Equals(MeasurementSystem.GetSystem().GetOne()))
					{
						Terms[uom] = power;
					}
				}
			}

			// compose the base symbol
			internal string BuildBaseString()
			{
				StringBuilder numerator = new StringBuilder();
				StringBuilder denominator = new StringBuilder();

				int numeratorCount = 0;
				int denominatorCount = 0;

				// sort units by symbol (ascending)
				SortedSet<UnitOfMeasure> keys = new SortedSet<UnitOfMeasure>(Terms.Keys);

				foreach (UnitOfMeasure unit in keys)
				{
					int power = Terms[unit];

					if (power < 0)
					{
						// negative, put in denominator
						if (denominator.Length > 0)
						{
							denominator.Append(UnitOfMeasure.MULT);
						}

						if (!unit.Equals(MeasurementSystem.GetSystem().GetOne()))
						{
							denominator.Append(unit.Symbol);
							denominatorCount++;
						}

						if (power < -1)
						{
							if (power == -2)
							{
								denominator.Append(UnitOfMeasure.SQ);
							}
							else if (power == -3)
							{
								denominator.Append(UnitOfMeasure.CUBED);
							}
							else
							{
								denominator.Append(UnitOfMeasure.POW).Append(Math.Abs(power));
							}
						}
					}
					else if (power >= 1 && !unit.Equals(MeasurementSystem.GetSystem().GetOne()))
					{
						// positive, put in numerator
						if (numerator.Length > 0)
						{
							numerator.Append(UnitOfMeasure.MULT);
						}

						numerator.Append(unit.Symbol);
						numeratorCount++;

						if (power > 1)
						{
							if (power == 2)
							{
								numerator.Append(UnitOfMeasure.SQ);
							}
							else if (power == 3)
							{
								numerator.Append(UnitOfMeasure.CUBED);
							}
							else
							{
								numerator.Append(UnitOfMeasure.POW).Append(power);
							}
						}
					}
					else
					{
						// unary, don't add a '1'
					}
				}

				if (numeratorCount == 0)
				{
					numerator.Append(UnitOfMeasure.ONE_CHAR);
				}

				string result = null;

				if (denominatorCount == 0)
				{
					result = numerator.ToString();
				}
				else
				{
					if (denominatorCount == 1)
					{
						result = numerator.Append(UnitOfMeasure.DIV).Append(denominator).ToString();
					}
					else
					{
						result = numerator.Append(UnitOfMeasure.DIV).Append(UnitOfMeasure.LP).Append(denominator).Append(UnitOfMeasure.RP).ToString();
					}
				}

				return result;
			} // end unit of measure iteration
		} // end Reducer class

		// UOM, scaling factor and power cumulative along a conversion path
		private class PathParameters
		{
			private UnitOfMeasure PathUOM;
			private decimal PathFactor;

			private PathParameters(UnitOfMeasure pathUOM, decimal pathFactor)
			{
				PathUOM = pathUOM;
				PathFactor = pathFactor;
			}

			private UnitOfMeasure GetPathUOM()
			{
				return PathUOM;
			}

			private decimal GetPathFactor()
			{
				return PathFactor;
			}
		}

	} // end UnitOfMeasure class
} // end namespace
