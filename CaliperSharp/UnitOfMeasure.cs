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
		private readonly int MAX_SYMBOL_LENGTH = 16;

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
		* Get the exponent
		* 
		* @return Exponent
*/
		public Nullable<int> GetPowerExponent()
		{
			return Exponent1;
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

		UnitOfMeasure ClonePower(UnitOfMeasure uom)
		{

			UnitOfMeasure newUOM = new UnitOfMeasure();
			newUOM.UnitType = UnitType;

			// check if quotient
			int? exponent = 1;
			if (GetPowerExponent() != null)
			{
				exponent = GetPowerExponent();
			}

			UnitOfMeasure one = MeasurementSystem.GetSystem().GetOne();
			if (GetMeasurementType().Equals(MeasurementType.QUOTIENT))
			{
				if (GetDividend().equals(one))
				{
					exponent = getExponent2();
				}
				else if (getDivisor().equals(one))
				{
					exponent = getExponent1();
				}
			}
			newUOM.setPowerUnit(uom, exponent);
			String symbol = UnitOfMeasure.generatePowerSymbol(uom, exponent);
			newUOM.setSymbol(symbol);
			newUOM.setName(symbol);

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
				value = decimal.Multiply(a,b);
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


	} // end UnitOfMeasure class
} // end namespace
