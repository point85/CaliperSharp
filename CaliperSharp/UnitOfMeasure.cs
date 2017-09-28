using System;
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
		internal const char MULT = (char)0xB7;
		internal const char DIV = '/';
		internal const char POW = '^';
		internal const char SQ = (char)0xB2;
		internal const char CUBED = (char)0xB3;
		internal const char LP = '(';
		internal const char RP = ')';
		internal const char ONE_CHAR = '1';

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
		public Nullable<Unit> UnitEnumeration { get; set; }

		// unit type, e.g. MASS
		public UnitType UOMType { get; set; } = UnitType.UNCLASSIFIED;

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
			UOMType = type;
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
 * @
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

		internal UnitOfMeasure ClonePower(UnitOfMeasure uom)
		{

			UnitOfMeasure newUOM = new UnitOfMeasure();
			newUOM.UOMType = UOMType;

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

		internal UnitOfMeasure ClonePowerProduct(UnitOfMeasure uom1, UnitOfMeasure uom2)
		{
			bool invert = false;
			UnitOfMeasure one = MeasurementSystem.GetSystem().GetOne();

			// check if quotient
			if (GetMeasurementType().Equals(MeasurementType.QUOTIENT))
			{
				if (uom2.Equals(one))
				{
					String msg = String.Format(MeasurementSystem.GetMessage("incompatible.units"), this, one);
					throw new Exception(msg);
				}
				invert = true;
			}
			else
			{
				if (uom1.Equals(one) || uom2.Equals(one))
				{
					String msg = String.Format(MeasurementSystem.GetMessage("incompatible.units"), this, one);
					throw new Exception(msg);
				}
			}

			UnitOfMeasure newUOM = uom1.MultiplyOrDivide(uom2, invert);
			newUOM.UOMType = UOMType;

			return newUOM;
		}

		private void CheckOffset(UnitOfMeasure other)
		{
			if (other.Offset.CompareTo(decimal.Zero) != 0)
			{
				String msg = String.Format(MeasurementSystem.GetMessage("offset.not.supported"), other.ToString());
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
				String msg = String.Format(MeasurementSystem.GetMessage("multiplier.cannot.be.null"), Symbol);
				throw new Exception(msg);
			}

			if (multiplicand == null)
			{
				String msg = String.Format(MeasurementSystem.GetMessage("multiplicand.cannot.be.null"), Symbol);
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

				String msg = String.Format(MeasurementSystem.GetMessage("dividend.cannot.be.null"), Symbol);
				throw new Exception(msg);
			}

			if (divisor == null)
			{
				String msg = String.Format(MeasurementSystem.GetMessage("divisor.cannot.be.null"), Symbol);
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
			decimal thisFactor = thisPowerMap.MapScalingFactor;

			// other base symbol map
			Reducer otherPowerMap = other.GetBaseMap();
			Dictionary<UnitOfMeasure, int> otherMap = otherPowerMap.Terms;
			decimal otherFactor = otherPowerMap.MapScalingFactor;

			// create a map of the unit of measure powers
			Dictionary<UnitOfMeasure, int> resultMap = new Dictionary<UnitOfMeasure, int>();

			// iterate over the multiplier's unit map
			foreach (KeyValuePair<UnitOfMeasure, int> thisEntry in thisMap)
			{
				UnitOfMeasure thisUOM = thisEntry.Key;
				int thisPower = thisEntry.Value;

				int otherPower = otherMap[thisUOM];

				if (otherPower != null)
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

			String baseSymbol = resultPowerMap.BuildBaseString();
			UnitOfMeasure baseUOM = MeasurementSystem.GetSystem().GetBaseUOM(baseSymbol);

			if (baseUOM != null)
			{
				// there is a conversion to the base UOM
				decimal resultFactor;
				if (!invert)
				{
					resultFactor = DecimalMultiply(thisFactor, otherFactor);
				}
				else
				{
					resultFactor = DecimalDivide(thisFactor, otherFactor);
				}
				result.ScalingFactor = resultFactor;
				result.AbscissaUnit = baseUOM;
				result.UOMType = baseUOM.UOMType;
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
			return UOM1;
		}

		/**
		 * Get the multiplicand
		 * 
		 * @return {@link UnitOfMeasure}
		 */
		public UnitOfMeasure GetMultiplicand()
		{
			return UOM2;
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

		// this method is for optimization of decimal addition
		internal static decimal DecimalAdd(decimal a, decimal b)
		{
			decimal value;

			if (b.CompareTo(decimal.Zero) == 0)
			{
				value = a;
			}
			else
			{
				value = decimal.Add(a, b);
			}

			return value;
		}

		// this method is for optimization of decimal subtraction
		internal static decimal DecimalSubtract(decimal a, decimal b)
		{
			decimal value;

			if (b.CompareTo(decimal.Zero) == 0)
			{
				value = a;
			}
			else
			{
				value = decimal.Subtract(a, b);
			}

			return value;
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
		public void SetConversion(decimal scalingFactor, UnitOfMeasure abscissaUnit, decimal offset)
		{
			if (abscissaUnit == null)
			{
				throw new Exception(MeasurementSystem.GetMessage("unit.cannot.be.null"));
			}

			// self conversion is special
			if (this.Equals(abscissaUnit))
			{
				if (scalingFactor.CompareTo(decimal.One) != 0 || offset.CompareTo(decimal.Zero) != 0)
				{
					throw new Exception(MeasurementSystem.GetMessage("conversion.not.allowed"));
				}
			}

			// unit has been previously cached, so first remove it, then cache again
			MeasurementSystem.GetSystem().UnregisterUnit(this);
			BaseSymbol = null;

			ScalingFactor = scalingFactor;
			AbscissaUnit = abscissaUnit;
			Offset = offset;

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
			this.SetConversion(decimal.One, abscissaUnit, decimal.Zero);
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
		public void SetConversion(decimal scalingFactor, UnitOfMeasure abscissaUnit)
		{
			this.SetConversion(scalingFactor, abscissaUnit, decimal.Zero);
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
			Unit? thisEnumeration = UnitEnumeration;
			Unit? otherEnumeration = otherUnit.UnitEnumeration;

			if (thisEnumeration != null && otherEnumeration != null && !thisEnumeration.Equals(otherEnumeration))
			{
				return false;
			}

			// same abscissa unit symbols
			String thisSymbol = AbscissaUnit.Symbol;
			String otherSymbol = otherUnit.AbscissaUnit.Symbol;

			if (!thisSymbol.Equals(otherSymbol))
			{
				return false;
			}

			// same factors
			if (ScalingFactor.CompareTo(otherUnit.ScalingFactor) != 0)
			{
				return false;
			}

			// same offsets
			if (Offset.CompareTo(otherUnit.Offset) != 0)
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
		int IComparable<UnitOfMeasure>.CompareTo(UnitOfMeasure other)
		{
			return Symbol.CompareTo(other.Symbol);
		}

		private static void CheckTypes(UnitOfMeasure uom1, UnitOfMeasure uom2)
		{
			UnitType thisType = uom1.UOMType;
			UnitType targetType = uom2.UOMType;

			if (thisType != UnitType.UNCLASSIFIED && targetType != UnitType.UNCLASSIFIED && !thisType.Equals(UnitType.UNITY)
					&& !targetType.Equals(UnitType.UNITY) && !thisType.Equals(targetType))
			{
				String msg = String.Format(MeasurementSystem.GetMessage("must.be.same.as"), uom1, uom1.UOMType,
						uom2, uom2.UOMType);
				throw new Exception(msg);
			}
		}

		private decimal ConvertScalarToScalar(UnitOfMeasure targetUOM)
		{
			UnitOfMeasure thisAbscissa = AbscissaUnit;
			decimal thisFactor = ScalingFactor;

			decimal scalingFactor;

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
			decimal pathFactor = decimal.One;

			while (true)
			{
				decimal scalingFactor = pathUOM.ScalingFactor;
				UnitOfMeasure abscissa = pathUOM.AbscissaUnit;

				pathFactor = DecimalMultiply(pathFactor, scalingFactor);

				if (pathUOM.Equals(abscissa))
				{
					break;
				}

				// next UOM on path
				pathUOM = abscissa;
			}

			return new PathParameters(pathUOM, pathFactor);
		}

		private decimal GetBridgeFactor(UnitOfMeasure uom)
		{
			decimal factor = decimal.One;

			// check for our bridge
			if (BridgeAbscissaUnit != null)
			{
				factor = BridgeScalingFactor;
			}
			else
			{
				// try other side
				if (uom.BridgeAbscissaUnit != null)
				{
					UnitOfMeasure toUOM = uom.BridgeAbscissaUnit;

					if (toUOM.Equals(this))
					{
						factor = DecimalDivide(decimal.One, uom.BridgeScalingFactor);
					}
				}
			}

			return factor;
		}

		private decimal ConvertUnit(UnitOfMeasure targetUOM)
		{
			// get path factors in each system
			PathParameters thisParameters = TraversePath();
			PathParameters targetParameters = targetUOM.TraversePath();

			decimal thisPathFactor = thisParameters.PathFactor;
			UnitOfMeasure thisBase = thisParameters.PathUOM;

			decimal targetPathFactor = targetParameters.PathFactor;
			UnitOfMeasure targetBase = targetParameters.PathUOM;

			// check for a base conversion unit bridge
			decimal bridgeFactor = thisBase.GetBridgeFactor(targetBase);

			if (bridgeFactor != null)
			{
				thisPathFactor = DecimalMultiply(thisPathFactor, bridgeFactor);
			}

			// new path amount
			decimal scalingFactor = DecimalDivide(thisPathFactor, targetPathFactor);

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
		public decimal GetConversionFactor(UnitOfMeasure targetUOM)
		{
			if (targetUOM == null)
			{
				throw new Exception(MeasurementSystem.GetMessage("unit.cannot.be.null"));
			}

			// first check the cache
			decimal cachedFactor;

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
				String msg = String.Format(MeasurementSystem.GetMessage("incompatible.units"), this, targetUOM);
				throw new Exception(msg);
			}

			decimal fromFactor = fromPowerMap.MapScalingFactor;
			decimal toFactor = toPowerMap.MapScalingFactor;

			decimal factor = decimal.One;

			// compute map factor
			int matchCount = 0;

			foreach (KeyValuePair<UnitOfMeasure, int> fromEntry in fromMap)
			{
				UnitType fromType = fromEntry.Key.UOMType;
				UnitOfMeasure fromUOM = fromEntry.Key;
				int fromPower = fromEntry.Value;

				foreach (KeyValuePair<UnitOfMeasure, int> toEntry in toMap)
				{
					UnitType toType = toEntry.Key.UOMType;

					if (fromType.Equals(toType))
					{
						matchCount++;
						UnitOfMeasure toUOM = toEntry.Key;
						decimal bd = fromUOM.ConvertScalarToScalar(toUOM);
						bd = DecimalPower(bd, fromPower);
						factor = DecimalMultiply(factor, bd);
						break;
					}
				} // to map
			} // from map

			if (matchCount != fromMap.Count)
			{
				String msg = String.Format(MeasurementSystem.GetMessage("incompatible.units"), this, targetUOM);
				throw new Exception(msg);
			}

			decimal scaling = DecimalDivide(fromFactor, toFactor);
			cachedFactor = DecimalMultiply(factor, scaling);

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

			if (Exponent2 != null && Exponent2 < 0)
			{
				inverted = GetDivisor().Divide(GetDividend());
			}
			else
			{
				inverted = MeasurementSystem.GetSystem().GetOne().Divide(this);
			}

			return inverted;
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
			internal decimal MapScalingFactor { get; set; } = decimal.One;

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
			internal UnitOfMeasure PathUOM { get; private set; }
			internal decimal PathFactor { get; private set; }

			internal PathParameters(UnitOfMeasure pathUOM, decimal pathFactor)
			{
				PathUOM = pathUOM;
				PathFactor = pathFactor;
			}
		}

	} // end UnitOfMeasure class
} // end namespace
