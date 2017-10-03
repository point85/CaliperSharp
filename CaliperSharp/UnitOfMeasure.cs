using System;
using System.Collections.Generic;
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
namespace org.point85.uom
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
		private const char MULT = (char)0xB7;
		private const char DIV = '/';
		private const char POW = '^';
		private const char SQ = (char)0xB2;
		private const char CUBED = (char)0xB3;
		private const char LP = '(';
		private const char RP = ')';
		private const char ONE_CHAR = '1';

		// registry of unit conversion factor
		private Dictionary<UnitOfMeasure, double> ConversionRegistry = new Dictionary<UnitOfMeasure, double>();

		// conversion to another Unit of Measure in the same recognized measurement
		// system (y = ax + b)
		// scaling factor (a)
		private double ScalingFactor = double.MinValue;

		// offset (b)
		private double Offset = double.MinValue;

		// x-axis unit
		private UnitOfMeasure AbscissaUnit;

		// unit enumerations for the various systems of measurement, e.g. KILOGRAM
		private Nullable<Unit> UnitEnumeration;

		// unit type, e.g. MASS
		private UnitType UOMType = UnitType.UNCLASSIFIED;

		// conversion to another Unit of Measure in a different measurement system
		private double BridgeScalingFactor;

		// offset (b)
		private double BridgeOffset;

		// x-axis unit
		private UnitOfMeasure BridgeAbscissaUnit;

		// cached base symbol
		private string BaseSymbol;

		// user-defined category
		private string Category = MeasurementSystem.GetUnitString("default.category.text");

		// base UOMs and exponents for a product of two power UOMs follow
		// power base unit, product multiplier or quotient dividend
		private UnitOfMeasure UOM1;

		// product multiplicand or quotient divisor
		private UnitOfMeasure UOM2;

		// exponent
		private Nullable<int> Exponent1;

		// second exponent
		private Nullable<int> Exponent2;

		/**
 * Construct a default unit of measure
 */
		public UnitOfMeasure() : base()
		{
		}

		internal UnitOfMeasure(UnitType type, string name, string symbol, string description) : base(name, symbol, description)
		{
			SetUnitType(type);
		}

		/**
 * Get the unit of measure's x-axis unit of measure for the relation y = ax
 * + b.
 * 
 * @return {@link UnitOfMeasure}
 */
		public UnitOfMeasure GetAbscissaUnit()
		{
			return AbscissaUnit != null ? AbscissaUnit : this;
		}

		/**
 * Set the unit of measure's x-axis unit of measure for the relation y = ax
 * + b.
 * 
 * @param abscissaUnit
 *            {@link UnitOfMeasure}
 */
		public void SetAbscissaUnit(UnitOfMeasure abscissaUnit)
		{
			this.AbscissaUnit = abscissaUnit;
		}

		/**
 * Get the unit of measure's 'a' factor (slope) for the relation y = ax + b.
 * 
 * @return Factor
 */
		public double GetScalingFactor()
		{
			return ScalingFactor != double.MinValue ? ScalingFactor : 1;
		}

		/**
 * Set the unit of measure's 'a' factor (slope) for the relation y = ax + b.
 * 
 * @param scalingFactor
 *            Scaling factor
 */
		public void SetScalingFactor(double scalingFactor)
		{
			ScalingFactor = scalingFactor;
		}

		/**
 * Get the unit of measure's 'b' offset (intercept) for the relation y = ax
 * + b.
 * 
 * @return Offset
 */
		public double GetOffset()
		{
			return Offset != double.MinValue ? Offset : 0;
		}

		/**
 * Set the unit of measure's 'b' offset (intercept) for the relation y = ax
 * + b.
 * 
 * @param offset
 *            Offset
 */
		public void SetOffset(double offset)
		{
			Offset = offset;
		}

		/**
 * Get the unit's enumerated type
 * 
 * @return {@link Unit}
 */
		public Unit? GetEnumeration()
		{
			return UnitEnumeration;
		}

		/**
		 * Set the unit's enumerated type
		 * 
		 * @param unit
		 *            {@link Unit}
		 */
		public void SetEnumeration(Unit? unit)
		{
			UnitEnumeration = unit;
		}
		/**
 * Get the type of the unit.
 * 
 * @return {@link UnitType}
 */
		public UnitType GetUnitType()
		{
			return UOMType;
		}

		/**
		 * Set the type of the unit.
		 * 
		 * @param unitType
		 *            {@link UnitType}
		 */
		public void SetUnitType(UnitType unitType)
		{
			UOMType = unitType;
		}

		/**
 * Get the bridge UOM scaling factor
 * 
 * @return Scaling factor
 */
		public double GetBridgeScalingFactor()
		{
			return BridgeScalingFactor;
		}

		private void SetBridgeScalingFactor(double factor)
		{
			BridgeScalingFactor = factor;
		}

		/**
		 * Get the bridge UOM abscissa UOM
		 * 
		 * @return {@link UnitOfMeasure}
		 */
		public UnitOfMeasure GetBridgeAbscissaUnit()
		{
			return BridgeAbscissaUnit;
		}

		private void SetBridgeAbscissaUnit(UnitOfMeasure uom)
		{
			BridgeAbscissaUnit = uom;
		}

		/**
		 * Get the bridge UOM offset
		 * 
		 * @return Offset
		 */
		public double GetBridgeOffset()
		{
			return BridgeOffset;
		}

		private void SetBridgeOffset(double offset)
		{
			BridgeOffset = offset;
		}

		private void SetBaseSymbol(string symbol)
		{
			BaseSymbol = symbol;
		}

		/**
 * Get the category
 * 
 * @return Category
 */
		public string GetCategory()
		{
			return Category;
		}

		/**
		 * Set the category
		 * 
		 * @param category
		 *            Category
		 */
		public void SetCategory(string category)
		{
			Category = category;
		}

		private UnitOfMeasure GetUOM1()
		{
			return UOM1;
		}

		private void SetUOM1(UnitOfMeasure uom)
		{
			UOM1 = uom;
		}

		private UnitOfMeasure GetUOM2()
		{
			return UOM2;
		}

		private void SetUOM2(UnitOfMeasure uom)
		{
			UOM2 = uom;
		}

		private int? GetExponent1()
		{
			return Exponent1;
		}

		private void SetExponent1(int exponent)
		{
			Exponent1 = exponent;
		}

		private int? GetExponent2()
		{
			return Exponent2;
		}

		private void SetExponent2(int exponent)
		{
			Exponent2 = exponent;
		}

		private void SetPowerProduct(UnitOfMeasure uom1, int exponent1)
		{
			SetUOM1(uom1);
			SetExponent1(exponent1);
		}

		private void SetPowerProduct(UnitOfMeasure uom1, int exponent1, UnitOfMeasure uom2, int exponent2)
		{
			SetUOM1(uom1);
			SetExponent1(exponent1);
			SetUOM2(uom2);
			SetExponent2(exponent2);
		}

		/**
 * Get the measurement type
 * 
 * @return {@link MeasurementType}
 */
		public MeasurementType GetMeasurementType()
		{
			MeasurementType type = MeasurementType.SCALAR;

			if (Exponent2.HasValue && Exponent2.Value < 0)
			{
				type = MeasurementType.QUOTIENT;
			}
			else if (Exponent2.HasValue && Exponent2.Value > 0)
			{
				type = MeasurementType.PRODUCT;
			}
			else if (GetUOM1() != null && Exponent1.HasValue)
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
	* @
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
			return this.Equals(GetAbscissaUnit()) ? true : false;
		}

		/**
 * Get the exponent
 * 
 * @return Exponent
 */
		public int? GetPowerExponent()
		{
			return GetExponent1();
		}

		/**
 * Get the dividend unit of measure
 * 
 * @return {@link UnitOfMeasure}
 */
		public UnitOfMeasure GetDividend()
		{
			return GetUOM1();
		}

		/**
		 * Get the divisor unit of measure
		 * 
		 * @return {@link UnitOfMeasure}
		 */
		public UnitOfMeasure GetDivisor()
		{
			return GetUOM2();
		}

		/**
 * Set the base unit of measure and exponent
 * 
 * @param base
 *            Base unit of measure
 * @param exponent
 *            Exponent
 * @
 *             Exception
 */
		public void SetPowerUnit(UnitOfMeasure baseUOM, int exponent)
		{
			if (baseUOM == null)
			{
				string msg = String.Format(MeasurementSystem.GetMessage("base.cannot.be.null"), Symbol);
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

		internal static string GeneratePowerSymbol(UnitOfMeasure baseUOM, int exponent)
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

		internal UnitOfMeasure ClonePower(UnitOfMeasure uom)
		{

			UnitOfMeasure newUOM = new UnitOfMeasure();
			newUOM.SetUnitType(GetUnitType());

			// check if quotient
			int exponent = 1;
			if (GetPowerExponent().HasValue)
			{
				exponent = GetPowerExponent().Value;
			}

			UnitOfMeasure one = MeasurementSystem.GetSystem().GetOne();
			if (GetMeasurementType().Equals(MeasurementType.QUOTIENT))
			{
				if (GetDividend().Equals(one))
				{
					exponent = GetExponent2().Value;
				}
				else if (GetDivisor().Equals(one))
				{
					exponent = GetExponent1().Value;
				}
			}

			newUOM.SetPowerUnit(uom, exponent);
			string symbol = UnitOfMeasure.GeneratePowerSymbol(uom, exponent);
			newUOM.Symbol = symbol;
			newUOM.Name = symbol;

			return newUOM;
		}

		internal UnitOfMeasure ClonePowerProduct(UnitOfMeasure uom1, UnitOfMeasure uom2)
		{
			bool invert = false;
			UnitOfMeasure one = MeasurementSystem.GetSystem().GetOne();

			// check if quotient
			if (GetMeasurementType().Equals(MeasurementType.QUOTIENT))
			{
				if (uom2.Equals(one))
				{
					string msg = String.Format(MeasurementSystem.GetMessage("incompatible.units"), this, one);
					throw new Exception(msg);
				}
				invert = true;
			}
			else
			{
				if (uom1.Equals(one) || uom2.Equals(one))
				{
					string msg = String.Format(MeasurementSystem.GetMessage("incompatible.units"), this, one);
					throw new Exception(msg);
				}
			}

			UnitOfMeasure newUOM = uom1.MultiplyOrDivide(uom2, invert);
			newUOM.SetUnitType(GetUnitType());

			return newUOM;
		}

		private void CheckOffset(UnitOfMeasure other)
		{
			if (other.GetOffset().CompareTo(0) != 0)
			{
				string msg = String.Format(MeasurementSystem.GetMessage("offset.not.supported"), other.ToString());
				throw new Exception(msg);
			}
		}

		/**
 * Set the multiplier and multiplicand
 * 
 * @param multiplier
 *            Multiplier
 * @param multiplicand
 *            Multiplicand
 * @throws Exception
 *             Exception
 */
		public void SetProductUnits(UnitOfMeasure multiplier, UnitOfMeasure multiplicand)
		{
			if (multiplier == null)
			{
				string msg = String.Format(MeasurementSystem.GetMessage("multiplier.cannot.be.null"), Symbol);
				throw new Exception(msg);
			}

			if (multiplicand == null)
			{
				string msg = String.Format(MeasurementSystem.GetMessage("multiplicand.cannot.be.null"), Symbol);
				throw new Exception(msg);
			}

			SetPowerProduct(multiplier, 1, multiplicand, 1);
		}

		/**
 * Set the dividend and divisor
 * 
 * @param dividend
 *            Dividend
 * @param divisor
 *            Divisor
 * @throws Exception
 *             Exception
 */
		public void SetQuotientUnits(UnitOfMeasure dividend, UnitOfMeasure divisor)
		{
			if (dividend == null)
			{

				string msg = String.Format(MeasurementSystem.GetMessage("dividend.cannot.be.null"), Symbol);
				throw new Exception(msg);
			}

			if (divisor == null)
			{
				string msg = String.Format(MeasurementSystem.GetMessage("divisor.cannot.be.null"), Symbol);
				throw new Exception(msg);
			}

			SetPowerProduct(dividend, 1, divisor, -1);
		}

		private UnitOfMeasure MultiplyOrDivide(UnitOfMeasure other, bool invert)
		{
			if (other == null)
			{
				throw new Exception(MeasurementSystem.GetMessage("unit.cannot.be.null"));
			}

			if (other.Equals(MeasurementSystem.GetSystem().GetOne()))
			{
				return this;
			}

			CheckOffset(this);
			CheckOffset(other);

			// product or quotient
			UnitOfMeasure result = new UnitOfMeasure();

			if (!invert)
			{
				result.SetProductUnits(this, other);
			}
			else
			{
				result.SetQuotientUnits(this, other);
			}

			// this base symbol map
			Reducer thisPowerMap = GetBaseMap();
			Dictionary<UnitOfMeasure, int> thisMap = thisPowerMap.Terms;
			double thisFactor = thisPowerMap.MapScalingFactor;

			// other base symbol map
			Reducer otherPowerMap = other.GetBaseMap();
			Dictionary<UnitOfMeasure, int> otherMap = otherPowerMap.Terms;
			double otherFactor = otherPowerMap.MapScalingFactor;

			// create a map of the unit of measure powers
			Dictionary<UnitOfMeasure, int> resultMap = new Dictionary<UnitOfMeasure, int>();

			// iterate over the multiplier's unit map
			foreach (KeyValuePair<UnitOfMeasure, int> thisEntry in thisMap)
			{
				UnitOfMeasure thisUOM = thisEntry.Key;
				int thisPower = thisEntry.Value;
				bool inMap = otherMap.TryGetValue(thisUOM, out int otherPower);

				if (inMap)
				{
					if (!invert)
					{
						// add to multiplier's power
						thisPower += otherPower;
					}
					else
					{
						// subtract from dividend's power
						thisPower -= otherPower;
					}

					// remove multiplicand or divisor UOM
					otherMap.Remove(thisUOM);
				}

				if (thisPower != 0)
				{
					resultMap[thisUOM] = thisPower;
				}
			}

			// add any remaining multiplicand terms and invert any remaining divisor
			// terms
			foreach (KeyValuePair<UnitOfMeasure, int> otherEntry in otherMap)
			{
				UnitOfMeasure otherUOM = otherEntry.Key;
				int otherPower = otherEntry.Value;

				if (!invert)
				{
					resultMap[otherUOM] = otherPower;
				}
				else
				{
					resultMap[otherUOM] = -otherPower;
				}
			}

			// get the base symbol and possibly base UOM
			Reducer resultPowerMap = new Reducer();
			resultPowerMap.Terms = resultMap;

			string baseSymbol = resultPowerMap.BuildBaseString();
			UnitOfMeasure baseUOM = MeasurementSystem.GetSystem().GetBaseUOM(baseSymbol);

			if (baseUOM != null)
			{
				// there is a conversion to the base UOM
				double resultFactor;
				if (!invert)
				{
					resultFactor = thisFactor * otherFactor;
				}
				else
				{
					resultFactor = thisFactor / otherFactor;
				}
				result.SetScalingFactor(resultFactor);
				result.SetAbscissaUnit(baseUOM);
				result.SetUnitType(baseUOM.GetUnitType());
			}

			if (!invert)
			{
				result.Symbol = GenerateProductSymbol(result.GetMultiplier(), result.GetMultiplicand());
			}
			else
			{
				result.Symbol = GenerateQuotientSymbol(result.GetDividend(), result.GetDivisor());
			}

			// constrain to a maximum length
			if (result.Symbol.Length > MAX_SYMBOL_LENGTH)
			{
				result.Symbol = GenerateIntermediateSymbol();
			}

			return result;
		}

		/**
		 * Get the multiplier
		 * 
		 * @return {@link UnitOfMeasure}
		 */
		public UnitOfMeasure GetMultiplier()
		{
			return GetUOM1();
		}

		/**
		 * Get the multiplicand
		 * 
		 * @return {@link UnitOfMeasure}
		 */
		public UnitOfMeasure GetMultiplicand()
		{
			return GetUOM2();
		}

		/**
		 * Multiply two units of measure to create a third one.
		 * 
		 * @param multiplicand
		 *            {@link UnitOfMeasure}
		 * @return {@link UnitOfMeasure}
		 * @throws Exception
		 *             Exception
		 */
		public UnitOfMeasure Multiply(UnitOfMeasure multiplicand)
		{
			return MultiplyOrDivide(multiplicand, false);
		}

		/**
 * Divide two units of measure to create a third one.
 * 
 * @param divisor
 *            {@link UnitOfMeasure}
 * @return {@link UnitOfMeasure}
 * @throws Exception
 *             Exception
 */
		public UnitOfMeasure Divide(UnitOfMeasure divisor)
		{
			return MultiplyOrDivide(divisor, true);
		}

		// this method is for optimization of double addition
		/*
		internal static double DoubleAdd(double a, double b)
		{
			double value;

			if (b.CompareTo(0) == 0)
			{
				value = a;
			}
			else
			{
				value = a + b;
			}

			return value;
		}
		*/
		/*
		// this method is for optimization of double subtraction
		internal static double DoubleSubtract(double a, double b)
		{
			double value;

			if (b.CompareTo(0) == 0)
			{
				value = a;
			}
			else
			{
				value = a - b;
			}

			return value;
		}
		*/
		/*
		// this method is for optimization of double multiplication
		internal static double DoubleMultiply(double a, double b)
		{
			double value;

			if (b.CompareTo(1) == 0)
			{
				value = a;
			}
			else
			{
				value = a * b;
			}
			return value;
		}
		*/
		/*
		// this method is for optimization of double division
		internal static double DoubleDivide(double a, double b)
		{
			double value;

			if (b.CompareTo(1) == 0)
			{
				value = a;
			}
			else
			{
				value = a / b;
			}
			return value;
		}
		*/

		/*
	// this method is for optimization of double exponentiation
	internal static double DoublePower(double powerBase, int exponent)
	{
		double value;

		if (exponent == 1)
		{
			value = powerBase;
		}
		else if (exponent == 0)
		{
			value = 1;
		}
		else
		{
			value = (double)Math.Pow((double)powerBase, exponent);
		}
		return value;
	}
	*/

		/**
 * Define a conversion with the specified scaling factor, abscissa unit of
 * measure and scaling factor.
 * 
 * @param scalingFactor
 *            Factor
 * @param abscissaUnit
 *            {@link UnitOfMeasure}
 * @param offset
 *            Offset
 * @
 *             Exception
 */
		public void SetConversion(double scalingFactor, UnitOfMeasure abscissaUnit, double offset)
		{
			if (abscissaUnit == null)
			{
				throw new Exception(MeasurementSystem.GetMessage("unit.cannot.be.null"));
			}

			// self conversion is special
			if (this.Equals(abscissaUnit))
			{
				if (scalingFactor.CompareTo(1) != 0 || offset.CompareTo(0) != 0)
				{
					throw new Exception(MeasurementSystem.GetMessage("conversion.not.allowed"));
				}
			}

			// unit has been previously cached, so first remove it, then cache again
			MeasurementSystem.GetSystem().UnregisterUnit(this);
			SetBaseSymbol(null);

			SetScalingFactor(scalingFactor);
			SetAbscissaUnit(abscissaUnit);
			SetOffset(offset);

			// re-cache
			MeasurementSystem.GetSystem().RegisterUnit(this);
		}

		/**
		 * Define a conversion with a scaling factor of 1 and offset of 0 for the
		 * specified abscissa unit of measure.
		 * 
		 * @param abscissaUnit
		 *            {@link UnitOfMeasure}
		 * @
		 *             Exception
		 */
		public void SetConversion(UnitOfMeasure abscissaUnit)
		{
			this.SetConversion(1, abscissaUnit, 0);
		}

		/**
		 * Define a conversion with an offset of 0 for the specified scaling factor
		 * and abscissa unit of measure.
		 * 
		 * @param scalingFactor
		 *            Factor
		 * @param abscissaUnit
		 *            {@link UnitOfMeasure}
		 * @
		 *             Exception
		 */
		public void SetConversion(double scalingFactor, UnitOfMeasure abscissaUnit)
		{
			this.SetConversion(scalingFactor, abscissaUnit, 0);
		}

		/**
		 * Construct a conversion with an offset of 0 for the specified scaling
		 * factor and abscissa unit of measure.
		 * 
		 * @param scalingFactor
		 *            Factor
		 * @param abscissaUnit
		 *            {@link UnitOfMeasure}
		 * @
		 *             Exception
		 */
		public void SetConversion(string scalingFactor, UnitOfMeasure abscissaUnit)
		{
			this.SetConversion(Quantity.CreateAmount(scalingFactor), abscissaUnit);
		}


		public override int GetHashCode()
		{
			int hashName = Name == null ? 0 : Name.GetHashCode();
			int hashAge = Symbol.GetHashCode();

			return hashName ^ hashAge;
		}

		/**
 * Compare this unit of measure to another
 * 
 * @return true if equal
 */
		public override bool Equals(Object other)
		{
			if (other == null || GetType() != other.GetType())
			{
				return false;
			}
			UnitOfMeasure otherUnit = (UnitOfMeasure)other;

			// same enumerations
			Unit? thisEnumeration = GetEnumeration();
			Unit? otherEnumeration = otherUnit.GetEnumeration();

			if (thisEnumeration != null && otherEnumeration != null && !thisEnumeration.Equals(otherEnumeration))
			{
				return false;
			}

			// same abscissa unit symbols
			string thisSymbol = GetAbscissaUnit().Symbol;
			string otherSymbol = otherUnit.GetAbscissaUnit().Symbol;

			if (!thisSymbol.Equals(otherSymbol))
			{
				return false;
			}

			// same factors
			if (GetScalingFactor().CompareTo(otherUnit.GetScalingFactor()) != 0)
			{
				return false;
			}

			// same offsets
			if (GetOffset().CompareTo(otherUnit.GetOffset()) != 0)
			{
				return false;
			}

			return true;
		}

		/**
		* Compare this unit of measure to another one.
		* 
		* @param other
		*            unit of measure
		* @return -1 if less than, 0 if equal and 1 if greater than
*/
		public int CompareTo(UnitOfMeasure other)
		{
			//return Symbol.CompareTo(other.Symbol);
			return Compare(other);
		}

		protected int Compare(UnitOfMeasure other)
		{
			return Symbol.CompareTo(other.Symbol);
		}

		private static void CheckTypes(UnitOfMeasure uom1, UnitOfMeasure uom2)
		{
			UnitType thisType = uom1.GetUnitType();
			UnitType targetType = uom2.GetUnitType();

			if (thisType != UnitType.UNCLASSIFIED && targetType != UnitType.UNCLASSIFIED && !thisType.Equals(UnitType.UNITY)
					&& !targetType.Equals(UnitType.UNITY) && !thisType.Equals(targetType))
			{
				string msg = String.Format(MeasurementSystem.GetMessage("must.be.same.as"), uom1, uom1.GetUnitType(),
						uom2, uom2.GetUnitType());
				throw new Exception(msg);
			}
		}

		private double ConvertScalarToScalar(UnitOfMeasure targetUOM)
		{
			UnitOfMeasure thisAbscissa = GetAbscissaUnit();
			double thisFactor = GetScalingFactor();

			double scalingFactor;

			if (thisAbscissa.Equals(targetUOM))
			{
				// direct conversion
				scalingFactor = thisFactor;
			}
			else
			{
				// indirect conversion
				scalingFactor = ConvertUnit(targetUOM);
			}
			return scalingFactor;
		}

		private PathParameters TraversePath()
		{
			UnitOfMeasure pathUOM = this;
			double pathFactor = 1;

			while (true)
			{
				double scalingFactor = pathUOM.GetScalingFactor();
				UnitOfMeasure abscissa = pathUOM.GetAbscissaUnit();

				pathFactor = pathFactor * scalingFactor;

				if (pathUOM.Equals(abscissa))
				{
					break;
				}

				// next UOM on path
				pathUOM = abscissa;
			}

			return new PathParameters(pathUOM, pathFactor);
		}

		private double GetBridgeFactor(UnitOfMeasure uom)
		{
			double factor = 1;

			// check for our bridge
			if (GetBridgeAbscissaUnit() != null)
			{
				factor = GetBridgeScalingFactor();
			}
			else
			{
				// try other side
				if (uom.GetBridgeAbscissaUnit() != null)
				{
					UnitOfMeasure toUOM = uom.GetBridgeAbscissaUnit();

					if (toUOM.Equals(this))
					{
						factor = (double)1 / uom.GetBridgeScalingFactor();
					}
				}
			}

			return factor;
		}

		private double ConvertUnit(UnitOfMeasure targetUOM)
		{
			// get path factors in each system
			PathParameters thisParameters = TraversePath();
			PathParameters targetParameters = targetUOM.TraversePath();

			double thisPathFactor = thisParameters.PathFactor;
			UnitOfMeasure thisBase = thisParameters.PathUOM;

			double targetPathFactor = targetParameters.PathFactor;
			UnitOfMeasure targetBase = targetParameters.PathUOM;

			// check for a base conversion unit bridge
			double? bridgeFactor = thisBase.GetBridgeFactor(targetBase);

			if (bridgeFactor.HasValue)
			{
				thisPathFactor = thisPathFactor * bridgeFactor.Value;
			}

			// new path amount
			double scalingFactor = thisPathFactor / targetPathFactor;

			return scalingFactor;
		}

		/**
		* Get the factor to convert to the unit of measure
		* 
		* @param targetUOM
		*            Target {@link UnitOfMeasure}
		* @return conversion factor
		* @
		*             Exception
*/
		public double GetConversionFactor(UnitOfMeasure targetUOM)
		{
			if (targetUOM == null)
			{
				throw new Exception(MeasurementSystem.GetMessage("unit.cannot.be.null"));
			}

			// first check the cache
			double cachedFactor;

			if (ConversionRegistry.TryGetValue(targetUOM, out cachedFactor))
			{
				return cachedFactor;
			}

			CheckTypes(this, targetUOM);

			Reducer fromPowerMap = GetBaseMap();
			Reducer toPowerMap = targetUOM.GetBaseMap();

			Dictionary<UnitOfMeasure, int> fromMap = fromPowerMap.Terms;
			Dictionary<UnitOfMeasure, int> toMap = toPowerMap.Terms;

			if (fromMap.Count != toMap.Count)
			{
				string msg = String.Format(MeasurementSystem.GetMessage("incompatible.units"), this, targetUOM);
				throw new Exception(msg);
			}

			double fromFactor = fromPowerMap.MapScalingFactor;
			double toFactor = toPowerMap.MapScalingFactor;

			double factor = 1;

			// compute map factor
			int matchCount = 0;

			foreach (KeyValuePair<UnitOfMeasure, int> fromEntry in fromMap)
			{
				UnitType fromType = fromEntry.Key.GetUnitType();
				UnitOfMeasure fromUOM = fromEntry.Key;
				int fromPower = fromEntry.Value;

				foreach (KeyValuePair<UnitOfMeasure, int> toEntry in toMap)
				{
					UnitType toType = toEntry.Key.GetUnitType();

					if (fromType.Equals(toType))
					{
						matchCount++;
						UnitOfMeasure toUOM = toEntry.Key;
						double bd = fromUOM.ConvertScalarToScalar(toUOM);
						bd = Math.Pow(bd, fromPower);
						factor = factor * bd;
						break;
					}
				} // to map
			} // from map

			if (matchCount != fromMap.Count)
			{
				string msg = String.Format(MeasurementSystem.GetMessage("incompatible.units"), this, targetUOM);
				throw new Exception(msg);
			}

			cachedFactor = factor * (fromFactor / toFactor);

			// cache it
			ConversionRegistry[targetUOM] = cachedFactor;

			return cachedFactor;
		}

		/**
 * Invert a unit of measure to create a new one
 * 
 * @return {@link UnitOfMeasure}
 * @throws Exception
 *             Exception
 */
		public UnitOfMeasure Invert()
		{
			UnitOfMeasure inverted = null;

			if (GetExponent2().HasValue && GetExponent2().Value < 0)
			{
				inverted = GetDivisor().Divide(GetDividend());
			}
			else
			{
				inverted = MeasurementSystem.GetSystem().GetOne().Divide(this);
			}

			return inverted;
		}

		/**
 * Set the conversion to another fundamental unit of measure
 * 
 * @param scalingFactor
 *            Scaling factor
 * @param abscissaUnit
 *            X-axis unit
 * @param offset
 *            Offset
 * @throws Exception
 *             Exception
 */
		public void SetBridgeConversion(double scalingFactor, UnitOfMeasure abscissaUnit, double? offset)
		{
			SetBridgeScalingFactor(scalingFactor);
			SetBridgeAbscissaUnit(abscissaUnit);

			if (offset.HasValue)
			{
				SetBridgeOffset(offset.Value);
			}
		}

		/**
 * Remove all cached conversions
 */
		public void ClearCache()
		{
			ConversionRegistry.Clear();
		}

		/**
 * Get the unit of measure corresponding to the base symbol
 * 
 * @return {@link UnitOfMeasure}
 * @throws Exception
 *             Exception
 */
		public UnitOfMeasure GetBaseUOM()
		{
			string baseSymbol = GetBaseSymbol();
			return MeasurementSystem.GetSystem().GetBaseUOM(baseSymbol);
		}

		/**
 * Get the base unit of measure for the power
 * 
 * @return {@link UnitOfMeasure}
 */
		public UnitOfMeasure GetPowerBase()
		{
			return GetUOM1();
		}

		/**
 * Create a String representation of this unit of measure
 * 
 * @return String representation
 */
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			// type
			sb.Append(MeasurementSystem.UnitsManager.GetString("unit.type.text")).Append(' ').Append(GetUnitType().ToString()).Append(", ");

			// unit enumeration
			if (GetEnumeration().HasValue)
			{
				sb.Append(MeasurementSystem.UnitsManager.GetString("enum.text")).Append(' ').Append(GetEnumeration().ToString()).Append(", ");
			}

			// symbol
			sb.Append(MeasurementSystem.UnitsManager.GetString("symbol.text")).Append(' ').Append(Symbol);
			sb.Append(", ").Append(MeasurementSystem.UnitsManager.GetString("conversion.text")).Append(' ');

			// scaling factor
			if (GetScalingFactor().CompareTo(1) != 0)
			{
				sb.Append(GetScalingFactor().ToString()).Append(MULT);
			}

			// abscissa unit
			if (GetAbscissaUnit() != null)
			{
				sb.Append(GetAbscissaUnit().Symbol);
			}

			// offset
			if (GetOffset().CompareTo(0) != 0)
			{
				sb.Append(" + ").Append(GetOffset().ToString());
			}

			sb.Append(", ").Append(MeasurementSystem.UnitsManager.GetString("base.text")).Append(' ');

			// base symbol
			sb.Append(GetBaseSymbol());

			return sb.ToString();
		}

		// reduce a unit of measure to its most basic scalar units of measure.
		private class Reducer
		{
			private const int MAX_RECURSIONS = 100;

			// starting level
			private const int STARTING_LEVEL = -1;

			// UOMs and their exponents
			internal Dictionary<UnitOfMeasure, int> Terms { get; set; } = new Dictionary<UnitOfMeasure, int>();

			// the overall scaling factor
			internal double MapScalingFactor { get; set; } = 1;

			// list of exponents down a path to the leaf UOM
			internal List<int> PathExponents = new List<int>();

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
				double scalingFactor = unit.GetScalingFactor();

				// explode the abscissa unit
				UnitOfMeasure abscissaUnit = unit.GetAbscissaUnit();

				UnitOfMeasure uom1 = abscissaUnit.GetUOM1();
				UnitOfMeasure uom2 = abscissaUnit.GetUOM2();

				int? exp1 = abscissaUnit.GetExponent1();
				int? exp2 = abscissaUnit.GetExponent2();

				// scaling
				if (PathExponents.Count > 0)
				{
					int lastExponent = PathExponents[PathExponents.Count - 1];

					// compute the overall scaling factor
					double factor = 1;
					for (int i = 0; i < Math.Abs(lastExponent); i++)
					{
						factor = factor * scalingFactor;
					}

					if (lastExponent < 0)
					{
						MapScalingFactor = MapScalingFactor / factor;
					}
					else
					{
						MapScalingFactor = MapScalingFactor * factor;
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
						double currentMapFactor = MapScalingFactor;
						MapScalingFactor = 1;
						ExplodeRecursively(abscissaUnit, STARTING_LEVEL);
						MapScalingFactor = MapScalingFactor * currentMapFactor;
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
			internal UnitOfMeasure PathUOM { get; private set; }
			internal double PathFactor { get; private set; }

			internal PathParameters(UnitOfMeasure pathUOM, double pathFactor)
			{
				PathUOM = pathUOM;
				PathFactor = pathFactor;
			}
		}

	} // end UnitOfMeasure class
} // end namespace
