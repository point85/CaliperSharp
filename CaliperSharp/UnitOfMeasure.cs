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

using System;
using System.Collections.Generic;
using System.Text;

namespace Point85.Caliper.UnitOfMeasure
{
	/// <summary>
	/// <p>
	/// A UnitOfMeasure can have a linear conversion (y = ax + b) to another unit of
	/// measure in the same internationally recognized measurement system of
	/// International Customary, SI, US or British Imperial. Or, the unit of measure
	/// can have a conversion to another custom unit of measure. It is owned by the
	/// unified MeasurementSystem defined by this project.
	/// </p>
	/// <p>
	/// A unit of measure is categorized by scalar (simple unit), quotient (divisor
	/// and dividend units), product (multiplier and multiplicand units) or power
	/// (unit with an integral exponent). More than one representation of a unit of
	/// measure is possible. For example, a unit of "per second" could be a quotient
	/// of "1/s" (e.g. an inverted second) or a power of s^-1.
	/// </p>
	/// <p>
	/// A unit of measure also has an enumerated UnitType (for example LENGTH
	/// or MASS) and a unique Unit discriminator (for example METRE).
	/// A basic unit (a.k.a fundamental unit in the SI system) can have a bridge
	/// conversion to another basic unit in another recognized measurement system.
	/// This conversion is defined unidirectionally. For example, an International
	/// Customary foot is 0.3048 SI metres. The conversion from metre to foot is just
	/// the inverse of this relationship.
	/// </p>
	/// <p>
	/// A unit of measure has a base symbol, for example 'm' for metre. A base symbol
	/// is one that consists only of the symbols for the base units of measure. In
	/// the SI system, the base units are well-defined. The derived units such as
	/// Newton all have base symbols expressed in the fundamental units of length
	/// (metre), mass (kilogram), time (second), temperature (Kelvin), plane angle
	/// (radian), electric charge (Coulomb) and luminous intensity (candela). In the
	/// US and British systems, base units are not defined. Caliper uses foot for
	/// length, pound mass for mass and Rankine for temperature. This base symbol is
	/// used in unit of measure conversions to uniquely identify the target unit.
	/// </p>
	/// <p>
	/// The SI system has defined prefixes (e.g. "centi") for 1/100th of another unit
	/// (e.g. metre). Instead of defining all the possible unit of measure
	/// combinations, the MeasurementSystem is able to create units by
	/// specifying the Prefix and target unit of measure. Similarly, computer
	/// science has defined prefixes for bytes (e.g. "mega").
	/// </p>
	/// </summary>
	public class UnitOfMeasure : Symbolic, IComparable<UnitOfMeasure>
	{
		/// <summary>
		/// Unit of measure types
		/// </summary>
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

		/// <summary>
		/// unit enumerations for the various systems of measurement, e.g. KILOGRAM
		/// </summary>
		public Nullable<Unit> Enumeration { get; set; }

		/// <summary>
		/// unit type, e.g. MASS
		/// </summary>
		public UnitType UOMType { get; set; } = UnitType.UNCLASSIFIED;

		/// <summary>
		/// conversion to another Unit of Measure in a different measurement system
		/// </summary>
		public double BridgeScalingFactor { get; private set; }

		/// <summary>
		/// offset (b)
		/// </summary>
		public double BridgeOffset { get; private set; } = double.MinValue;

		/// <summary>
		/// x-axis unit
		/// </summary>
		public UnitOfMeasure BridgeAbscissaUnit { get; private set; }

		// cached base symbol
		private string BaseSymbol;

		/// <summary>
		/// user-defined category
		/// </summary>
		public string Category { get; set; } = MeasurementSystem.GetUnitString("default.category.text");

		// base UOMs and exponents for a product of two power UOMs follow
		// power base unit, product multiplier or quotient dividend
		private UnitOfMeasure UOM1;

		// product multiplicand or quotient divisor
		private UnitOfMeasure UOM2;

		// exponent
		private int Exponent1 = int.MinValue;

		// second exponent
		private int Exponent2 = int.MinValue;

		/// <summary>Construct a default unit of measure</summary>
		public UnitOfMeasure() : base()
		{
		}

		internal UnitOfMeasure(UnitType type, string name, string symbol, string description) : base(name, symbol, description)
		{
			UOMType = type;
		}

		/// <summary>
		/// Check to see if the exponent has a value
		/// </summary>
		/// <param name="exponent"></param>
		/// <returns>True if it has been set</returns>
		public bool ExponentHasValue(int exponent)
		{
			return exponent == int.MinValue ? false : true;
		}

		/// <summary>Get the unit of measure's x-axis unit of measure for the relation y = ax + b.</summary>
		/// 
		/// <returns> UnitOfMeasure </returns>
		public UnitOfMeasure GetAbscissaUnit()
		{
			return AbscissaUnit ?? (this);
		}

		/// <summary>Set the unit of measure's x-axis unit of measure for the relation y = ax + b.</summary>
		/// 
		/// <param name="abscissaUnit">UnitOfMeasure</param>
		public void SetAbscissaUnit(UnitOfMeasure abscissaUnit)
		{
			this.AbscissaUnit = abscissaUnit;
		}

		/// <summary>Get the unit of measure's 'a' factor (slope) for the relation y = ax + b.</summary>
		/// 
		/// <returns> Factor</returns>
		public double GetScalingFactor()
		{
			return ScalingFactor != double.MinValue ? ScalingFactor : 1;
		}

		/// <summary>Set the unit of measure's 'a' factor (slope) for the relation y = ax + b.</summary>
		/// 
		/// <param name="scalingFactor">Scaling factor</param>
		public void SetScalingFactor(double scalingFactor)
		{
			ScalingFactor = scalingFactor;
		}

		/// <summary>Get the unit of measure's 'b' offset (intercept) for the relation y = ax + b.</summary>
		/// 
		/// <returns> Offset</returns>
		public double GetOffset()
		{
			return Offset != double.MinValue ? Offset : 0;
		}

		/// <summary>Set the unit of measure's 'b' offset (intercept) for the relation y = ax + b.</summary>
		/// 
		/// <param name="offset">Offset</param>
		public void SetOffset(double offset)
		{
			Offset = offset;
		}

		private void SetBaseSymbol(string symbol)
		{
			BaseSymbol = symbol;
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

		/// <summary>Get the measurement type</summary>
		/// 
		/// <returns> MeasurementType</returns>
		public MeasurementType GetMeasurementType()
		{
			MeasurementType type = MeasurementType.SCALAR;

			if (ExponentHasValue(Exponent2) && Exponent2 < 0)
			{
				type = MeasurementType.QUOTIENT;
			}
			else if (ExponentHasValue(Exponent2) && Exponent2 > 0)
			{
				type = MeasurementType.PRODUCT;
			}
			else if (UOM1 != null && ExponentHasValue(Exponent1))
			{
				type = MeasurementType.POWER;
			}

			return type;
		}

		private Reducer GetReducer()
		{
			lock (new object())
			{
				Reducer reducer = new Reducer();
				reducer.Explode(this);
				return reducer;
			}
		}

		/// <summary>Get the unit of measure's symbol in the fundamental units for that
		/// system. For example a Newton is a kg.m/s2.</summary>
		/// 
		/// <returns> Base symbol</returns>
		public string GetBaseSymbol()
		{
			lock (new object())
			{
				if (BaseSymbol == null)
				{
					Reducer powerMap = GetReducer();
					BaseSymbol = powerMap.BuildBaseString();
				}
				return BaseSymbol;
			}
		}

		/// <summary>Check to see if this unit of measure has a conversion to another unit of
		/// measure other than itself.</summary>
		/// 
		/// <returns> True if it does not</returns>
		public bool IsTerminal()
		{
			return this.Equals(GetAbscissaUnit()) ? true : false;
		}

		/// <summary>Get the exponent of a power unit</summary>
		/// 
		/// <returns> Exponent</returns>
		public int GetPowerExponent()
		{
			return Exponent1;
		}

		/// <summary>Get the dividend unit of measure</summary>
		/// 
		/// <returns> UnitOfMeasure</returns>
		public UnitOfMeasure GetDividend()
		{
			return UOM1;
		}

		/// <summary>Get the divisor unit of measure</summary>
		/// 
		/// <returns> UnitOfMeasure</returns>
		public UnitOfMeasure GetDivisor()
		{
			return UOM2;
		}

		/// <summary>Set the base unit of measure and exponent</summary>
		/// 
		/// <param name="baseUOM">Base unit of measure</param>
		/// <param name="exponent">Exponent</param>
		public void SetPowerUnit(UnitOfMeasure baseUOM, int exponent)
		{
			if (baseUOM == null)
			{
				string msg = String.Format(MeasurementSystem.GetMessage("base.cannot.be.null"), Symbol);
				throw new Exception(msg);
			}

			// special cases
			if (exponent == -1)
			{
				SetPowerProduct(MeasurementSystem.GetSystem().GetOne(), 1, baseUOM, -1);
			}
			else
			{
				SetPowerProduct(baseUOM, exponent);
			}
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
			newUOM.UOMType = UOMType;

			// check if quotient
			int exponent = 1;
			if (ExponentHasValue(GetPowerExponent()))
			{
				exponent = GetPowerExponent();
			}

			UnitOfMeasure one = MeasurementSystem.GetSystem().GetOne();
			if (GetMeasurementType().Equals(MeasurementType.QUOTIENT))
			{
				if (GetDividend().Equals(one))
				{
					exponent = Exponent2;
				}
				else if (GetDivisor().Equals(one))
				{
					exponent = Exponent1;
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
			newUOM.UOMType = UOMType;

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


		/// <summary>Set the multiplier and multiplicand</summary>
		/// 
		/// <param name="multiplier">Multiplier</param>          
		/// <param name="multiplicand">Multiplicand</param>           
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

		/// <summary>Set the dividend and divisor</summary>
		/// 
		/// <param name="dividend">Dividend</param>        
		/// <param name="divisor">Divisor</param>           
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

			CheckOffset(this);
			CheckOffset(other);

			// this base symbol map
			Reducer thisReducer = GetReducer();
			Dictionary<UnitOfMeasure, int> thisMap = thisReducer.GetTerms();

			// other base symbol map
			Reducer otherReducer = other.GetReducer();
			Dictionary<UnitOfMeasure, int> otherMap = otherReducer.GetTerms();

			// create a map of the unit of measure powers
			Dictionary<UnitOfMeasure, int> resultMap = new Dictionary<UnitOfMeasure, int>();

			// iterate over the multiplier's unit map
			foreach (KeyValuePair<UnitOfMeasure, int> thisEntry in thisMap)
			{
				UnitOfMeasure thisUOM = thisEntry.Key;
				int thisPower = thisEntry.Value;

				if (otherMap.TryGetValue(thisUOM, out int otherPower))
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
			Reducer resultReducer = new Reducer();
			resultReducer.SetTerms(resultMap);

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

			String baseSymbol = resultReducer.BuildBaseString();
			UnitOfMeasure baseUOM = MeasurementSystem.GetSystem().GetBaseUOM(baseSymbol);

			if (baseUOM != null)
			{
				// there is a conversion to the base UOM
				double thisFactor = thisReducer.GetScalingFactor();
				double otherFactor = otherReducer.GetScalingFactor();

				double resultFactor = 0;
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
				result.UOMType = baseUOM.UOMType;
			}

			return result;
		}


		/// <summary>Get the multiplier</summary>
		/// 
		/// <returns> UnitOfMeasure</returns>
		public UnitOfMeasure GetMultiplier()
		{
			return UOM1;
		}

		/// <summary>Get the multiplicand</summary>
		/// 
		/// <returns> UnitOfMeasure</returns>
		public UnitOfMeasure GetMultiplicand()
		{
			return UOM2;
		}

		/// <summary>Multiply two units of measure to create a third one.</summary>
		/// 
		/// <param name="multiplicand">UnitOfMeasure</param>
		/// <returns> UnitOfMeasure </returns>
		public UnitOfMeasure Multiply(UnitOfMeasure multiplicand)
		{
			return MultiplyOrDivide(multiplicand, false);
		}

		/// <summary>Divide two units of measure to create a third one.</summary>
		/// 
		/// <param name="divisor">UnitOfMeasure</param>
		///            
		/// <returns> UnitOfMeasure</returns>
		public UnitOfMeasure Divide(UnitOfMeasure divisor)
		{
			return MultiplyOrDivide(divisor, true);
		}

		/// <summary>Define a conversion with the specified scaling factor, abscissa unit of
		/// measure and scaling factor.</summary>
		/// 
		/// <param name="scalingFactor">Factor</param>        
		/// <param name="abscissaUnit">UnitOfMeasure</param>          
		/// <param name="offset">Offset</param>
		/// 
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


		/// <summary>Define a conversion with a scaling factor of 1 and offset of 0 for the
		/// specified abscissa unit of measure.</summary>
		/// 
		/// <param name="abscissaUnit">UnitOfMeasure</param>
		///            
		public void SetConversion(UnitOfMeasure abscissaUnit)
		{
			this.SetConversion(1, abscissaUnit, 0);
		}


		/// <summary>Define a conversion with an offset of 0 for the specified scaling factor
		/// and abscissa unit of measure.</summary>
		/// 
		/// <param name="scalingFactor">Factor</param>
		///            
		/// <param name="abscissaUnit">UnitOfMeasure</param>
		///            
		public void SetConversion(double scalingFactor, UnitOfMeasure abscissaUnit)
		{
			this.SetConversion(scalingFactor, abscissaUnit, 0);
		}

		/// <summary>Construct a conversion with an offset of 0 for the specified scaling
		/// factor and abscissa unit of measure.</summary>
		/// 
		/// <param name="scalingFactor">Factor</param>
		///            
		/// <param name="abscissaUnit">UnitOfMeasure</param>          
		///            
		public void SetConversion(string scalingFactor, UnitOfMeasure abscissaUnit)
		{
			this.SetConversion(Quantity.CreateAmount(scalingFactor), abscissaUnit);
		}

		/// <summary>
		/// Compute a hash code
		/// </summary>
		/// <returns>Hash code</returns>
		public override int GetHashCode()
		{
			int hashName = Name == null ? 0 : Name.GetHashCode();
			int hashSymbol = Symbol.GetHashCode();

			return hashName ^ hashSymbol;
		}

		/// <summary>
		/// Compare this unit of measure to another
		/// </summary>
		/// <param name="other">UnitOfMeasure</param>
		/// <returns>True if equal</returns>
		public override bool Equals(Object other)
		{
			if (other == null || GetType() != other.GetType())
			{
				return false;
			}
			UnitOfMeasure otherUnit = (UnitOfMeasure)other;

			// same enumerations
			Unit? thisEnumeration = Enumeration;
			Unit? otherEnumeration = otherUnit.Enumeration;

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


		/// <summary>Compare this unit of measure to another one.</summary>
		/// 
		/// <param name="other">UnitOfMeasure</param>
		/// <returns>-1 if less than, 0 if equal and 1 if greater than</returns>
		/// 
		public int CompareTo(UnitOfMeasure other)
		{
			//return Symbol.CompareTo(other.Symbol);
			return Compare(other);
		}

		/// <summary>
		/// Compare this unit of measure to another one for overriding
		/// </summary>
		/// <param name="other">UnitOfMeasure</param>
		/// <returns>-1 if less than, 0 if equal and 1 if greater than</returns>
		protected int Compare(UnitOfMeasure other)
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
				string msg = String.Format(MeasurementSystem.GetMessage("must.be.same.as"), uom1, uom1.UOMType,
						uom2, uom2.UOMType);
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
						factor = (double)1 / uom.BridgeScalingFactor;
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


		/// <summary>Get the factor to convert to the unit of measure</summary>
		/// 
		/// <param name="targetUOM">Target unit of measure</param>
		///            
		/// <returns> conversion factor</returns>
		/// 
		public double GetConversionFactor(UnitOfMeasure targetUOM)
		{
			if (targetUOM == null)
			{
				throw new Exception(MeasurementSystem.GetMessage("unit.cannot.be.null"));
			}

			// first check the cache
			if (ConversionRegistry.TryGetValue(targetUOM, out double cachedFactor))
			{
				return cachedFactor;
			}

			CheckTypes(this, targetUOM);

			Reducer fromPowerMap = GetReducer();
			Reducer toPowerMap = targetUOM.GetReducer();

			Dictionary<UnitOfMeasure, int> fromMap = fromPowerMap.GetTerms();
			Dictionary<UnitOfMeasure, int> toMap = toPowerMap.GetTerms();

			if (fromMap.Count != toMap.Count)
			{
				string msg = String.Format(MeasurementSystem.GetMessage("incompatible.units"), this, targetUOM);
				throw new Exception(msg);
			}

			double fromFactor = fromPowerMap.GetScalingFactor();
			double toFactor = toPowerMap.GetScalingFactor();

			double factor = 1;

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

		/// <summary>Invert a unit of measure to create a new one</summary>
		/// 
		/// <returns> UnitOfMeasure</returns>
		/// 
		public UnitOfMeasure Invert()
		{
			UnitOfMeasure inverted = null;

			if (ExponentHasValue(Exponent2) && Exponent2 < 0)
			{
				inverted = GetDivisor().Divide(GetDividend());
			}
			else
			{
				inverted = MeasurementSystem.GetSystem().GetOne().Divide(this);
			}

			return inverted;
		}


		/// <summary>Set the conversion to another fundamental unit of measure</summary>
		/// 
		/// <param name="scalingFactor">Scaling factor</param>         
		/// <param name="abscissaUnit">X-axis unit</param>           
		/// <param name="offset">Offset</param>
		///            
		public void SetBridgeConversion(double scalingFactor, UnitOfMeasure abscissaUnit, double? offset)
		{
			BridgeScalingFactor = scalingFactor;
			BridgeAbscissaUnit = abscissaUnit;

			if (offset.HasValue)
			{
				BridgeOffset = offset.Value;
			}
		}

		/// <summary>Remove all cached conversions</summary>
		public void ClearCache()
		{
			ConversionRegistry.Clear();
		}

		/// <summary>Get the unit of measure corresponding to the base symbol</summary>
		/// 
		/// <returns> UnitOfMeasure</returns>
		/// 
		public UnitOfMeasure GetBaseUOM()
		{
			string baseSymbol = GetBaseSymbol();
			return MeasurementSystem.GetSystem().GetBaseUOM(baseSymbol);
		}

		/// <summary>Get the base unit of measure for the power</summary>
		/// 
		/// <returns> UnitOfMeasure</returns>
		///
		public UnitOfMeasure GetPowerBase()
		{
			return UOM1;
		}

		/// <summary>
		/// Build a string representation of this unit of measure
		/// </summary>
		/// <returns>String value</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			// type
			sb.Append(MeasurementSystem.UnitsManager.GetString("unit.type.text")).Append(' ').Append(UOMType.ToString()).Append(", ");

			// unit enumeration
			if (Enumeration.HasValue)
			{
				sb.Append(MeasurementSystem.UnitsManager.GetString("enum.text")).Append(' ').Append(Enumeration.ToString()).Append(", ");
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
		internal class Reducer
		{
			private const int MAX_RECURSIONS = 100;

			// starting level
			private const int STARTING_LEVEL = -1;

			// UOMs and their exponents
			private Dictionary<UnitOfMeasure, int> Terms = new Dictionary<UnitOfMeasure, int>();

			// the overall scaling factor
			private double MapScalingFactor = 1;

			// list of exponents down a path to the leaf UOM
			private List<int> PathExponents = new List<int>();

			// recursion Counter
			private int Counter = 0;

			internal Reducer()
			{

			}

			internal double GetScalingFactor()
			{
				return this.MapScalingFactor;
			}

			internal Dictionary<UnitOfMeasure, int> GetTerms()
			{
				return Terms;
			}

			internal void SetTerms(Dictionary<UnitOfMeasure, int> terms)
			{
				this.Terms = terms;
			}

			internal void Explode(UnitOfMeasure unit)
			{
				ExplodeRecursively(unit, STARTING_LEVEL);
			}

			private void ExplodeRecursively(UnitOfMeasure unit, int level)
			{
				if (++Counter > MAX_RECURSIONS)
				{
					String msg = String.Format(MeasurementSystem.GetMessage("circular.references"),
							unit.Symbol);
					throw new Exception(msg);
				}

				// down a level
				level++;

				// scaling factor to abscissa unit
				double scalingFactor = unit.GetScalingFactor();

				// explode the abscissa unit
				UnitOfMeasure abscissaUnit = unit.GetAbscissaUnit();

				UnitOfMeasure uom1 = abscissaUnit.UOM1;
				UnitOfMeasure uom2 = abscissaUnit.UOM2;

				int exp1 = abscissaUnit.Exponent1;
				int exp2 = abscissaUnit.Exponent2;

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
					PathExponents.Add(exp1);
					ExplodeRecursively(uom1, level);
					PathExponents.RemoveAt(level);
				}

				if (uom2 != null)
				{
					// explode UOM #2
					PathExponents.Add(exp2);
					ExplodeRecursively(uom2, level);
					PathExponents.RemoveAt(level);
				}

				// up a level
				level--;
			}

			// add a UOM and exponent pair to the map of reduced Terms
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
			internal String BuildBaseString()
			{
				StringBuilder numerator = new StringBuilder();
				StringBuilder denominator = new StringBuilder();

				int numeratorCount = 0;
				int denominatorCount = 0;

				// sort units by symbol (ascending)
				SortedDictionary<UnitOfMeasure, int> keys = new SortedDictionary<UnitOfMeasure, int>(Terms);

				foreach (KeyValuePair<UnitOfMeasure, int> pair in keys)
				{
					UnitOfMeasure unit = pair.Key;
					int power = pair.Value;

					if (power < 0)
					{
						// negative, put in denominator
						if (denominator.Length > 0)
						{
							denominator.Append(MULT);
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
								denominator.Append(SQ);
							}
							else if (power == -3)
							{
								denominator.Append(CUBED);
							}
							else
							{
								denominator.Append(POW).Append(Math.Abs(power));
							}
						}
					}
					else if (power >= 1 && !unit.Equals(MeasurementSystem.GetSystem().GetOne()))
					{
						// positive, put in numerator
						if (numerator.Length > 0)
						{
							numerator.Append(MULT);
						}

						numerator.Append(unit.Symbol);
						numeratorCount++;

						if (power > 1)
						{
							if (power == 2)
							{
								numerator.Append(SQ);
							}
							else if (power == 3)
							{
								numerator.Append(CUBED);
							}
							else
							{
								numerator.Append(POW).Append(power);
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
					numerator.Append(ONE_CHAR);
				}

				String result = null;

				if (denominatorCount == 0)
				{
					result = numerator.ToString();
				}
				else
				{
					if (denominatorCount == 1)
					{
						result = numerator.Append(DIV).Append(denominator).ToString();
					}
					else
					{
						result = numerator.Append(DIV).Append(LP).Append(denominator).Append(RP).ToString();
					}
				}

				return result;
			} // end unit of measure iteration

			public override string ToString()
			{
				return MapScalingFactor + ", " + Terms.ToString();
			}
		}

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
