/*
MIT License

Copyright (c) 2016 - 2025 Kent Randall Point85

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

using System.Collections.Concurrent;

namespace Point85.Caliper.UnitOfMeasure
{
	/// <summary>
	/// A MeasurementSystem is a collection of units of measure that have a linear
	/// relationship to each other: y = ax + b where x is the unit to be converted, y
	/// is the converted unit, a is the scaling factor and b is the offset. <br>
	/// See
	/// <ul>
	/// <li>Wikipedia: <i><a href=
	/// "https://en.wikipedia.org/wiki/International_System_of_Units">International
	/// System of Units</a></i></li>
	/// <li>Table of conversions:
	/// <i><a href="https://en.wikipedia.org/wiki/Conversion_of_units">Conversion of
	/// Units</a></i></li>
	/// <li>Unified Code for Units of Measure :
	/// <i><a href="http://unitsofmeasure.org/trac">UCUM</a></i></li>
	/// <li>SI derived units:
	/// <i><a href="https://en.wikipedia.org/wiki/SI_derived_unit">SI Derived
	/// Units</a></i></li>
	/// <li>US system:
	/// <i><a href="https://en.wikipedia.org/wiki/United_States_customary_units">US
	/// Units</a></i></li>
	/// <li>British Imperial system:
	/// <i><a href="https://en.wikipedia.org/wiki/Imperial_units">British Imperial
	/// Units</a></i></li>
	/// <li>JSR 363: <i><a href=
	/// "https://java.net/downloads/unitsofmeasurement/JSR363Specification_EDR.pdf">JSR
	/// 363 Specification</a></i></li>
	/// </ul>
	/// <br>
	/// The MeasurementSystem class creates:
	/// <ul>
	/// <li>7 SI fundamental units of measure</li>
	/// <li>20 SI units derived from these fundamental units</li>
	/// <li>other units in the International Customary, US and British Imperial
	/// systems</li>
	/// <li>any number of custom units of measure</li>
	/// </ul>
	/// </summary>
	public class MeasurementSystem
	{
		// standard unified system
		private static MeasurementSystem UnifiedSystem = new MeasurementSystem();

		// registry by unit symbol
		private ConcurrentDictionary<string, UnitOfMeasure> SymbolRegistry = new ConcurrentDictionary<string, UnitOfMeasure>();

		// registry by base symbol
		private ConcurrentDictionary<string, UnitOfMeasure> BaseRegistry = new ConcurrentDictionary<string, UnitOfMeasure>();

		// registry for units by enumeration
		private ConcurrentDictionary<Unit, UnitOfMeasure> UnitRegistry = new ConcurrentDictionary<Unit, UnitOfMeasure>();

		// registry for base UOM map by unit type
		private ConcurrentDictionary<UnitType, ConcurrentDictionary<UnitType, int>> UnitTypeRegistry = new ConcurrentDictionary<UnitType, ConcurrentDictionary<UnitType, int>>();

		// floating point precision comparison
		public static double EPSILON = 1e-10;

		// instance lock object
		private readonly object _instanceLock = new object();

		private MeasurementSystem()
		{
		}

		/// <summary>Get the units of measure cached by their symbol</summary>
		/// 
		/// <returns>Symbol cache</returns>
		/// 
		public ConcurrentDictionary<string, UnitOfMeasure> GetSymbolCache()
		{
			return SymbolRegistry;
		}

		/// <summary>Get the units of measure cached by their base symbol</summary>
		/// 
		/// <returns>Base symbol cache</returns>
		/// 
		public ConcurrentDictionary<string, UnitOfMeasure> GetBaseSymbolCache()
		{
			return BaseRegistry;
		}

		/// <summary>Get the units of measure cached by their Unit enumeration</summary>
		/// 
		/// <returns>Enumeration cache</returns>
		/// 
		public ConcurrentDictionary<Unit, UnitOfMeasure> GetEnumerationCache()
		{
			return UnitRegistry;
		}

		/// <summary>
		/// get a particular message by its key
		/// </summary>
		public static string GetMessage(string key)
		{
			return CaliperSharp.Properties.Resources.ResourceManager.GetString(key);
		}

		/// <summary>
		/// get a particular unit string by its key
		/// </summary>
		public static string GetUnitString(string key)
		{
			return GetMessage(key);
		}

		/// <summary>Get the unified system of units of measure for International Customary,
		/// SI, US, British Imperial as well as custom systems</summary>
		/// 
		/// <returns>MeasurementSystem</returns>
		/// 
		public static MeasurementSystem GetSystem()
		{
			return UnifiedSystem;
		}

		/// <summary>Get the unit of measure for unity 'one'</summary>
		/// 
		/// <returns>UnitOfMeasure</returns>
		/// 
		public UnitOfMeasure GetOne()
		{
			return GetUOM(Unit.ONE);
		}

		/// <summary>Get the fundamental unit of measure of time</summary>
		/// 
		/// <returns>UnitOfMeasure</returns>
		/// 
		public UnitOfMeasure GetSecond()
		{
			return GetUOM(Unit.SECOND);
		}

		/// <summary>Get the unit of measure for a minute (60 seconds)</summary>
		/// 
		/// <returns>UnitOfMeasure</returns>
		/// 
		public UnitOfMeasure GetMinute()
		{
			return GetUOM(Unit.MINUTE);
		}

		/// <summary>Get the unit of measure for an hour (60 minutes)</summary>
		/// 
		/// <returns>UnitOfMeasure</returns>
		/// 
		public UnitOfMeasure GetHour()
		{
			return GetUOM(Unit.HOUR);
		}

		/// <summary>Get the unit of measure for one day (24 hours)</summary>
		/// 
		/// <returns>UnitOfMeasure</returns>
		/// 
		public UnitOfMeasure GetDay()
		{
			return GetUOM(Unit.DAY);
		}

		/// <summary>Get the unit of measure with this unique enumerated type</summary>
		/// 
		/// <param name="unit">Unit</param>
		///           
		/// <returns>UnitOfMeasure</returns>
		/// 
		public UnitOfMeasure GetUOM(Unit unit)
		{
			UnitRegistry.TryGetValue(unit, out UnitOfMeasure uom);

			if (uom == null)
			{
				uom = CreateUOM(unit);
			}
			return uom;
		}

		/// <summary>Get the unit of measure with this unique symbol</summary>
		/// 
		/// <param name="symbol">Symbol</param>
		///            
		/// <returns>UnitOfMeasure</returns>
		/// 
		public UnitOfMeasure GetUOM(string symbol)
		{
			SymbolRegistry.TryGetValue(symbol, out UnitOfMeasure uom);
			return uom;
		}


		/// <summary>Get the unit of measure with this base symbol</summary>
		/// 
		/// <param name="symbol">Base symbol</param>
		///            
		/// <returns>UnitOfMeasure</returns>
		/// 
		public UnitOfMeasure GetBaseUOM(string symbol)
		{
			BaseRegistry.TryGetValue(symbol, out UnitOfMeasure uom);
			return uom;
		}

		/// <summary>Create or fetch a unit of measure linearly scaled by the Prefix
		/// against the target unit of measure.</summary>
		/// 
		/// <param name="prefix">Scaling Prefix with the scaling factor, e.g. 1000</param>
		/// <param name="targetUOM">abscissa UnitOfMeasure</param>
		///      
		/// <returns>UnitOfMeasure</returns>
		/// 
		public UnitOfMeasure GetUOM(Prefix prefix, UnitOfMeasure targetUOM)
		{
			string symbol = prefix.Symbol + targetUOM.Symbol;

			UnitOfMeasure scaled = GetUOM(symbol);

			// if not found, create it
			if (scaled == null)
			{
				// generate a name and description
				string name = prefix.Name + targetUOM.Name;
				string description = prefix.Factor + " " + targetUOM.Name;

				// scaling factor
				double scalingFactor = targetUOM.ScalingFactor * prefix.Factor;

				// create the unit of measure and set conversion
				scaled = CreateScalarUOM(targetUOM.UOMType, null, name, symbol, description);
				scaled.SetConversion(scalingFactor, targetUOM.AbscissaUnit);
			}
			return scaled;
		}

		/// <summary>Create or fetch a unit of measure linearly scaled by the Prefix
		/// against the target unit of measure.</summary>
		/// 
		/// <param name="prefix">Scaling prefix with the scaling factor, e.g. 1000</param>
		/// <param name="unit">Unit</param>
		///        
		/// <returns>UnitOfMeasure</returns>
		///
		public UnitOfMeasure GetUOM(Prefix prefix, Unit unit)
		{
			return GetUOM(prefix, MeasurementSystem.GetSystem().GetUOM(unit));
		}


		private UnitOfMeasure CreateUOM(Unit enumeration)
		{
			UnitOfMeasure uom = null;

			// SI
			uom = CreateSIUnit(enumeration);

			if (uom != null)
			{
				return uom;
			}

			// Customary
			uom = CreateCustomaryUnit(enumeration);

			if (uom != null)
			{
				return uom;
			}

			// US
			uom = CreateUSUnit(enumeration);

			if (uom != null)
			{
				return uom;
			}

			// British
			uom = CreateBRUnit(enumeration);

			if (uom != null)
			{
				return uom;
			}

			// currency
			uom = CreateFinancialUnit(enumeration);

			return uom;
		}

		private UnitOfMeasure CreateSIUnit(Unit unit)
		{
			// In addition to the two dimensionless derived units radian (rad) and
			// steradian (sr), 20 other derived units have special names as defined
			// below. The seven fundamental SI units are metre, kilogram, kelvin,
			// ampere, candela and mole.

			UnitOfMeasure uom = null;

			switch (unit)
			{
				case Unit.ONE:
					// unity
					uom = CreateScalarUOM(UnitType.UNITY, Unit.ONE, MeasurementSystem.GetUnitString("one.name"),
							MeasurementSystem.GetUnitString("one.symbol"), MeasurementSystem.GetUnitString("one.desc"));
					break;

				case Unit.PERCENT:
					uom = CreateScalarUOM(UnitType.UNITY, Unit.PERCENT, MeasurementSystem.GetUnitString("percent.name"),
							MeasurementSystem.GetUnitString("percent.symbol"), MeasurementSystem.GetUnitString("percent.desc"));
					uom.SetConversion(0.01, GetOne());
					break;

				case Unit.SECOND:
					// second
					uom = CreateScalarUOM(UnitType.TIME, Unit.SECOND, MeasurementSystem.GetUnitString("sec.name"),
							MeasurementSystem.GetUnitString("sec.symbol"), MeasurementSystem.GetUnitString("sec.desc"));
					break;

				case Unit.MINUTE:
					// minute
					uom = CreateScalarUOM(UnitType.TIME, Unit.MINUTE, MeasurementSystem.GetUnitString("min.name"),
							MeasurementSystem.GetUnitString("min.symbol"), MeasurementSystem.GetUnitString("min.desc"));
					uom.SetConversion(60, GetUOM(Unit.SECOND));
					break;

				case Unit.HOUR:
					// hour
					uom = CreateScalarUOM(UnitType.TIME, Unit.HOUR, MeasurementSystem.GetUnitString("hr.name"),
							MeasurementSystem.GetUnitString("hr.symbol"), MeasurementSystem.GetUnitString("hr.desc"));
					uom.SetConversion(3600, GetUOM(Unit.SECOND));
					break;

				case Unit.DAY:
					// day
					uom = CreateScalarUOM(UnitType.TIME, Unit.DAY, MeasurementSystem.GetUnitString("day.name"),
							MeasurementSystem.GetUnitString("day.symbol"), MeasurementSystem.GetUnitString("day.desc"));
					uom.SetConversion(86400, GetUOM(Unit.SECOND));
					break;

				case Unit.WEEK:
					// week
					uom = CreateScalarUOM(UnitType.TIME, Unit.WEEK, MeasurementSystem.GetUnitString("week.name"),
							MeasurementSystem.GetUnitString("week.symbol"), MeasurementSystem.GetUnitString("week.desc"));
					uom.SetConversion(604800, GetUOM(Unit.SECOND));
					break;

				case Unit.JULIAN_YEAR:
					// Julian year
					uom = CreateScalarUOM(UnitType.TIME, Unit.JULIAN_YEAR, MeasurementSystem.GetUnitString("jyear.name"),
							MeasurementSystem.GetUnitString("jyear.symbol"), MeasurementSystem.GetUnitString("jyear.desc"));
					uom.SetConversion(3.1557600E+07, GetUOM(Unit.SECOND));

					break;

				case Unit.SQUARE_SECOND:
					// square second
					uom = CreatePowerUOM(UnitType.TIME_SQUARED, Unit.SQUARE_SECOND, MeasurementSystem.GetUnitString("s2.name"),
							MeasurementSystem.GetUnitString("s2.symbol"), MeasurementSystem.GetUnitString("s2.desc"), GetUOM(Unit.SECOND), 2);
					break;

				case Unit.MOLE:
					// substance amount
					uom = CreateScalarUOM(UnitType.SUBSTANCE_AMOUNT, Unit.MOLE, MeasurementSystem.GetUnitString("mole.name"),
							MeasurementSystem.GetUnitString("mole.symbol"), MeasurementSystem.GetUnitString("mole.desc"));
					break;

				case Unit.EQUIVALENT:
					// substance amount
					uom = CreateScalarUOM(UnitType.SUBSTANCE_AMOUNT, Unit.EQUIVALENT, MeasurementSystem.GetUnitString("equivalent.name"),
							MeasurementSystem.GetUnitString("equivalent.symbol"), MeasurementSystem.GetUnitString("equivalent.desc"));
					break;

				case Unit.DECIBEL:
					// decibel
					uom = CreateScalarUOM(UnitType.INTENSITY, Unit.DECIBEL, MeasurementSystem.GetUnitString("db.name"),
							MeasurementSystem.GetUnitString("db.symbol"), MeasurementSystem.GetUnitString("db.desc"));
					break;

				case Unit.RADIAN:
					// plane angle radian (rad)
					uom = CreateScalarUOM(UnitType.PLANE_ANGLE, Unit.RADIAN, MeasurementSystem.GetUnitString("radian.name"),
							MeasurementSystem.GetUnitString("radian.symbol"), MeasurementSystem.GetUnitString("radian.desc"));
					uom.SetConversion(GetOne());
					break;

				case Unit.STERADIAN:
					// solid angle steradian (sr)
					uom = CreateScalarUOM(UnitType.SOLID_ANGLE, Unit.STERADIAN, MeasurementSystem.GetUnitString("steradian.name"),
							MeasurementSystem.GetUnitString("steradian.symbol"), MeasurementSystem.GetUnitString("steradian.desc"));
					uom.SetConversion(GetOne());
					break;

				case Unit.DEGREE:
					// degree of arc
					uom = CreateScalarUOM(UnitType.PLANE_ANGLE, Unit.DEGREE, MeasurementSystem.GetUnitString("degree.name"),
							MeasurementSystem.GetUnitString("degree.symbol"), MeasurementSystem.GetUnitString("degree.desc"));
					uom.SetConversion(Math.PI / (double)180, GetUOM(Unit.RADIAN));
					break;

				case Unit.ARC_SECOND:
					// degree of arc
					uom = CreateScalarUOM(UnitType.PLANE_ANGLE, Unit.ARC_SECOND, MeasurementSystem.GetUnitString("arcsec.name"),
							MeasurementSystem.GetUnitString("arcsec.symbol"), MeasurementSystem.GetUnitString("arcsec.desc"));
					uom.SetConversion(Math.PI / (double)648000, GetUOM(Unit.RADIAN));
					break;

				case Unit.METRE:
					// fundamental length
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.METRE, MeasurementSystem.GetUnitString("m.name"),
							MeasurementSystem.GetUnitString("m.symbol"), MeasurementSystem.GetUnitString("m.desc"));
					break;

				case Unit.DIOPTER:
					// per metre
					uom = CreateQuotientUOM(UnitType.RECIPROCAL_LENGTH, Unit.DIOPTER, MeasurementSystem.GetUnitString("diopter.name"),
							MeasurementSystem.GetUnitString("diopter.symbol"), MeasurementSystem.GetUnitString("diopter.desc"), GetOne(),
							GetUOM(Unit.METRE));
					break;

				case Unit.KILOGRAM:
					// fundamental mass
					uom = CreateScalarUOM(UnitType.MASS, Unit.KILOGRAM, MeasurementSystem.GetUnitString("kg.name"),
							MeasurementSystem.GetUnitString("kg.symbol"), MeasurementSystem.GetUnitString("kg.desc"));
					break;

				case Unit.TONNE:
					// mass
					uom = CreateScalarUOM(UnitType.MASS, Unit.TONNE, MeasurementSystem.GetUnitString("tonne.name"),
							MeasurementSystem.GetUnitString("tonne.symbol"), MeasurementSystem.GetUnitString("tonne.desc"));
					uom.SetConversion(Prefix.KILO.Factor, GetUOM(Unit.KILOGRAM));
					break;

				case Unit.KELVIN:
					// fundamental temperature
					uom = CreateScalarUOM(UnitType.TEMPERATURE, Unit.KELVIN, MeasurementSystem.GetUnitString("kelvin.name"),
							MeasurementSystem.GetUnitString("kelvin.symbol"), MeasurementSystem.GetUnitString("kelvin.desc"));
					break;

				case Unit.AMPERE:
					// electric current
					uom = CreateScalarUOM(UnitType.ELECTRIC_CURRENT, Unit.AMPERE, MeasurementSystem.GetUnitString("amp.name"),
							MeasurementSystem.GetUnitString("amp.symbol"), MeasurementSystem.GetUnitString("amp.desc"));
					break;

				case Unit.CANDELA:
					// luminosity
					uom = CreateScalarUOM(UnitType.LUMINOSITY, Unit.CANDELA, MeasurementSystem.GetUnitString("cd.name"),
							MeasurementSystem.GetUnitString("cd.symbol"), MeasurementSystem.GetUnitString("cd.desc"));
					break;

				case Unit.MOLARITY:
					// molar concentration
					uom = CreateQuotientUOM(UnitType.MOLAR_CONCENTRATION, Unit.MOLARITY, MeasurementSystem.GetUnitString("molarity.name"),
							MeasurementSystem.GetUnitString("molarity.symbol"), MeasurementSystem.GetUnitString("molarity.desc"), GetUOM(Unit.MOLE),
							GetUOM(Unit.LITRE));
					break;

				case Unit.GRAM:
					// gram
					uom = CreateScalarUOM(UnitType.MASS, Unit.GRAM, MeasurementSystem.GetUnitString("gram.name"),
							MeasurementSystem.GetUnitString("gram.symbol"), MeasurementSystem.GetUnitString("gram.desc"));
					uom.SetConversion(Prefix.MILLI.Factor, GetUOM(Unit.KILOGRAM));
					break;

				case Unit.CARAT:
					// carat
					uom = CreateScalarUOM(UnitType.MASS, Unit.CARAT, MeasurementSystem.GetUnitString("carat.name"),
							MeasurementSystem.GetUnitString("carat.symbol"), MeasurementSystem.GetUnitString("carat.desc"));
					uom.SetConversion(0.2, GetUOM(Unit.GRAM));
					break;

				case Unit.SQUARE_METRE:
					// square metre
					uom = CreatePowerUOM(UnitType.AREA, Unit.SQUARE_METRE, MeasurementSystem.GetUnitString("m2.name"),
							MeasurementSystem.GetUnitString("m2.symbol"), MeasurementSystem.GetUnitString("m2.desc"), GetUOM(Unit.METRE), 2);
					break;

				case Unit.HECTARE:
					// hectare
					uom = CreateScalarUOM(UnitType.AREA, Unit.HECTARE, MeasurementSystem.GetUnitString("hectare.name"),
							MeasurementSystem.GetUnitString("hectare.symbol"), MeasurementSystem.GetUnitString("hectare.desc"));
					uom.SetConversion(10000, GetUOM(Unit.SQUARE_METRE));
					break;

				case Unit.METRE_PER_SEC:
					// velocity
					uom = CreateQuotientUOM(UnitType.VELOCITY, Unit.METRE_PER_SEC, MeasurementSystem.GetUnitString("mps.name"),
							MeasurementSystem.GetUnitString("mps.symbol"), MeasurementSystem.GetUnitString("mps.desc"), GetUOM(Unit.METRE), GetSecond());
					break;

				case Unit.METRE_PER_SEC_SQUARED:
					// acceleration
					uom = CreateQuotientUOM(UnitType.ACCELERATION, Unit.METRE_PER_SEC_SQUARED, MeasurementSystem.GetUnitString("mps2.name"),
							MeasurementSystem.GetUnitString("mps2.symbol"), MeasurementSystem.GetUnitString("mps2.desc"), GetUOM(Unit.METRE),
							GetUOM(Unit.SQUARE_SECOND));
					break;

				case Unit.CUBIC_METRE:
					// cubic metre
					uom = CreatePowerUOM(UnitType.VOLUME, Unit.CUBIC_METRE, MeasurementSystem.GetUnitString("m3.name"),
							MeasurementSystem.GetUnitString("m3.symbol"), MeasurementSystem.GetUnitString("m3.desc"), GetUOM(Unit.METRE), 3);
					break;

				case Unit.LITRE:
					// litre
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.LITRE, MeasurementSystem.GetUnitString("litre.name"),
							MeasurementSystem.GetUnitString("litre.symbol"), MeasurementSystem.GetUnitString("litre.desc"));
					uom.SetConversion(Prefix.MILLI.Factor, GetUOM(Unit.CUBIC_METRE));
					break;

				case Unit.CUBIC_METRE_PER_SEC:
					// flow (volume)
					uom = CreateQuotientUOM(UnitType.VOLUMETRIC_FLOW, Unit.CUBIC_METRE_PER_SEC,
							MeasurementSystem.GetUnitString("m3PerSec.name"), MeasurementSystem.GetUnitString("m3PerSec.symbol"),
							MeasurementSystem.GetUnitString("m3PerSec.desc"), GetUOM(Unit.CUBIC_METRE), GetSecond());
					break;

				case Unit.KILOGRAM_PER_SEC:
					// flow (mass)
					uom = CreateQuotientUOM(UnitType.MASS_FLOW, Unit.KILOGRAM_PER_SEC, MeasurementSystem.GetUnitString("kgPerSec.name"),
							MeasurementSystem.GetUnitString("kgPerSec.symbol"), MeasurementSystem.GetUnitString("kgPerSec.desc"), GetUOM(Unit.KILOGRAM),
							GetSecond());
					break;

				case Unit.KILOGRAM_PER_CU_METRE:
					// kg/m^3
					uom = CreateQuotientUOM(UnitType.DENSITY, Unit.KILOGRAM_PER_CU_METRE, MeasurementSystem.GetUnitString("kg_m3.name"),
							MeasurementSystem.GetUnitString("kg_m3.symbol"), MeasurementSystem.GetUnitString("kg_m3.desc"), GetUOM(Unit.KILOGRAM),
							GetUOM(Unit.CUBIC_METRE));
					break;

				case Unit.PASCAL_SECOND:
					// dynamic viscosity
					uom = CreateProductUOM(UnitType.DYNAMIC_VISCOSITY, Unit.PASCAL_SECOND, MeasurementSystem.GetUnitString("pascal_sec.name"),
							MeasurementSystem.GetUnitString("pascal_sec.symbol"), MeasurementSystem.GetUnitString("pascal_sec.desc"), GetUOM(Unit.PASCAL),
							GetSecond());
					break;

				case Unit.SQUARE_METRE_PER_SEC:
					// kinematic viscosity
					uom = CreateQuotientUOM(UnitType.KINEMATIC_VISCOSITY, Unit.SQUARE_METRE_PER_SEC,
							MeasurementSystem.GetUnitString("m2PerSec.name"), MeasurementSystem.GetUnitString("m2PerSec.symbol"),
							MeasurementSystem.GetUnitString("m2PerSec.desc"), GetUOM(Unit.SQUARE_METRE), GetSecond());
					break;

				case Unit.CALORIE:
					// thermodynamic calorie
					uom = CreateScalarUOM(UnitType.ENERGY, Unit.CALORIE, MeasurementSystem.GetUnitString("calorie.name"),
							MeasurementSystem.GetUnitString("calorie.symbol"), MeasurementSystem.GetUnitString("calorie.desc"));
					uom.SetConversion(4.184, GetUOM(Unit.JOULE));
					break;

				case Unit.NEWTON:
					// force F = m·A (newton)
					uom = CreateProductUOM(UnitType.FORCE, Unit.NEWTON, MeasurementSystem.GetUnitString("newton.name"),
							MeasurementSystem.GetUnitString("newton.symbol"), MeasurementSystem.GetUnitString("newton.desc"), GetUOM(Unit.KILOGRAM),
							GetUOM(Unit.METRE_PER_SEC_SQUARED));
					break;

				case Unit.NEWTON_METRE:
					// newton-metre
					uom = CreateProductUOM(UnitType.ENERGY, Unit.NEWTON_METRE, MeasurementSystem.GetUnitString("n_m.name"),
							MeasurementSystem.GetUnitString("n_m.symbol"), MeasurementSystem.GetUnitString("n_m.desc"), GetUOM(Unit.NEWTON),
							GetUOM(Unit.METRE));
					break;

				case Unit.JOULE:
					// energy (joule)
					uom = CreateProductUOM(UnitType.ENERGY, Unit.JOULE, MeasurementSystem.GetUnitString("joule.name"),
							MeasurementSystem.GetUnitString("joule.symbol"), MeasurementSystem.GetUnitString("joule.desc"), GetUOM(Unit.NEWTON),
							GetUOM(Unit.METRE));
					break;

				case Unit.ELECTRON_VOLT:
					// ev
					Quantity e = this.GetQuantity(Constant.ELEMENTARY_CHARGE);
					uom = CreateProductUOM(UnitType.ENERGY, Unit.ELECTRON_VOLT, MeasurementSystem.GetUnitString("ev.name"),
								MeasurementSystem.GetUnitString("ev.symbol"), MeasurementSystem.GetUnitString("ev.desc"), e.UOM, GetUOM(Unit.VOLT));
					uom.ScalingFactor = e.Amount;
					break;

				case Unit.WATT_HOUR:
					// watt-hour
					uom = CreateProductUOM(UnitType.ENERGY, Unit.WATT_HOUR, MeasurementSystem.GetUnitString("wh.name"),
							MeasurementSystem.GetUnitString("wh.symbol"), MeasurementSystem.GetUnitString("wh.desc"), GetUOM(Unit.WATT), GetHour());
					break;

				case Unit.WATT:
					// power (watt)
					uom = CreateQuotientUOM(UnitType.POWER, Unit.WATT, MeasurementSystem.GetUnitString("watt.name"),
							MeasurementSystem.GetUnitString("watt.symbol"), MeasurementSystem.GetUnitString("watt.desc"), GetUOM(Unit.JOULE), GetSecond());
					break;

				case Unit.HERTZ:
					// frequency (hertz)
					uom = CreateQuotientUOM(UnitType.FREQUENCY, Unit.HERTZ, MeasurementSystem.GetUnitString("hertz.name"),
							MeasurementSystem.GetUnitString("hertz.symbol"), MeasurementSystem.GetUnitString("hertz.desc"), GetOne(), GetSecond());
					break;

				case Unit.RAD_PER_SEC:
					// angular frequency
					uom = CreateQuotientUOM(UnitType.FREQUENCY, Unit.RAD_PER_SEC, MeasurementSystem.GetUnitString("radpers.name"),
							MeasurementSystem.GetUnitString("radpers.symbol"), MeasurementSystem.GetUnitString("radpers.desc"), GetUOM(Unit.RADIAN),
							GetSecond());
					uom.SetConversion((double)1 / ((double)2 * Math.PI), GetUOM(Unit.HERTZ));
					break;

				case Unit.PASCAL:
					// pressure
					uom = CreateQuotientUOM(UnitType.PRESSURE, Unit.PASCAL, MeasurementSystem.GetUnitString("pascal.name"),
							MeasurementSystem.GetUnitString("pascal.symbol"), MeasurementSystem.GetUnitString("pascal.desc"), GetUOM(Unit.NEWTON),
							GetUOM(Unit.SQUARE_METRE));
					break;

				case Unit.ATMOSPHERE:
					// pressure
					uom = CreateScalarUOM(UnitType.PRESSURE, Unit.ATMOSPHERE, MeasurementSystem.GetUnitString("atm.name"),
							MeasurementSystem.GetUnitString("atm.symbol"), MeasurementSystem.GetUnitString("atm.desc"));
					uom.SetConversion(101325, GetUOM(Unit.PASCAL));
					break;

				case Unit.BAR:
					// pressure
					uom = CreateScalarUOM(UnitType.PRESSURE, Unit.BAR, MeasurementSystem.GetUnitString("bar.name"),
							MeasurementSystem.GetUnitString("bar.symbol"), MeasurementSystem.GetUnitString("bar.desc"));
					uom.SetConversion(1, GetUOM(Unit.PASCAL), 1.0E+05);
					break;

				case Unit.COULOMB:
					// charge (coulomb)
					uom = CreateProductUOM(UnitType.ELECTRIC_CHARGE, Unit.COULOMB, MeasurementSystem.GetUnitString("coulomb.name"),
							MeasurementSystem.GetUnitString("coulomb.symbol"), MeasurementSystem.GetUnitString("coulomb.desc"), GetUOM(Unit.AMPERE),
							GetSecond());
					break;

				case Unit.VOLT:
					// voltage (volt)
					uom = CreateQuotientUOM(UnitType.ELECTROMOTIVE_FORCE, Unit.VOLT, MeasurementSystem.GetUnitString("volt.name"),
							MeasurementSystem.GetUnitString("volt.symbol"), MeasurementSystem.GetUnitString("volt.desc"), GetUOM(Unit.WATT),
							GetUOM(Unit.AMPERE));
					break;

				case Unit.OHM:
					// resistance (ohm)
					uom = CreateQuotientUOM(UnitType.ELECTRIC_RESISTANCE, Unit.OHM, MeasurementSystem.GetUnitString("ohm.name"),
							MeasurementSystem.GetUnitString("ohm.symbol"), MeasurementSystem.GetUnitString("ohm.desc"), GetUOM(Unit.VOLT),
							GetUOM(Unit.AMPERE));
					break;

				case Unit.FARAD:
					// capacitance (farad)
					uom = CreateQuotientUOM(UnitType.ELECTRIC_CAPACITANCE, Unit.FARAD, MeasurementSystem.GetUnitString("farad.name"),
							MeasurementSystem.GetUnitString("farad.symbol"), MeasurementSystem.GetUnitString("farad.desc"), GetUOM(Unit.COULOMB),
							GetUOM(Unit.VOLT));
					break;

				case Unit.FARAD_PER_METRE:
					// electric permittivity (farad/metre)
					uom = CreateQuotientUOM(UnitType.ELECTRIC_PERMITTIVITY, Unit.FARAD_PER_METRE,
							MeasurementSystem.GetUnitString("fperm.name"), MeasurementSystem.GetUnitString("fperm.symbol"), MeasurementSystem.GetUnitString("fperm.desc"),
							GetUOM(Unit.FARAD), GetUOM(Unit.METRE));
					break;

				case Unit.AMPERE_PER_METRE:
					// electric field strength(ampere/metre)
					uom = CreateQuotientUOM(UnitType.ELECTRIC_FIELD_STRENGTH, Unit.AMPERE_PER_METRE,
							MeasurementSystem.GetUnitString("aperm.name"), MeasurementSystem.GetUnitString("aperm.symbol"), MeasurementSystem.GetUnitString("aperm.desc"),
							GetUOM(Unit.AMPERE), GetUOM(Unit.METRE));
					break;

				case Unit.WEBER:
					// magnetic flux (weber)
					uom = CreateProductUOM(UnitType.MAGNETIC_FLUX, Unit.WEBER, MeasurementSystem.GetUnitString("weber.name"),
							MeasurementSystem.GetUnitString("weber.symbol"), MeasurementSystem.GetUnitString("weber.desc"), GetUOM(Unit.VOLT), GetSecond());
					break;

				case Unit.TESLA:
					// magnetic flux density (tesla)
					uom = CreateQuotientUOM(UnitType.MAGNETIC_FLUX_DENSITY, Unit.TESLA, MeasurementSystem.GetUnitString("tesla.name"),
							MeasurementSystem.GetUnitString("tesla.symbol"), MeasurementSystem.GetUnitString("tesla.desc"), GetUOM(Unit.WEBER),
							GetUOM(Unit.SQUARE_METRE));
					break;

				case Unit.HENRY:
					// inductance (henry)
					uom = CreateQuotientUOM(UnitType.ELECTRIC_INDUCTANCE, Unit.HENRY, MeasurementSystem.GetUnitString("henry.name"),
							MeasurementSystem.GetUnitString("henry.symbol"), MeasurementSystem.GetUnitString("henry.desc"), GetUOM(Unit.WEBER),
							GetUOM(Unit.AMPERE));
					break;

				case Unit.SIEMENS:
					// electrical conductance (siemens)
					uom = CreateQuotientUOM(UnitType.ELECTRIC_CONDUCTANCE, Unit.SIEMENS, MeasurementSystem.GetUnitString("siemens.name"),
							MeasurementSystem.GetUnitString("siemens.symbol"), MeasurementSystem.GetUnitString("siemens.desc"), GetUOM(Unit.AMPERE),
							GetUOM(Unit.VOLT));
					break;

				case Unit.CELSIUS:
					// °C = °K - 273.15
					uom = CreateScalarUOM(UnitType.TEMPERATURE, Unit.CELSIUS, MeasurementSystem.GetUnitString("celsius.name"),
							MeasurementSystem.GetUnitString("celsius.symbol"), MeasurementSystem.GetUnitString("celsius.desc"));
					uom.SetConversion(1, GetUOM(Unit.KELVIN), 273.15);
					break;

				case Unit.LUMEN:
					// luminous flux (lumen)
					uom = CreateProductUOM(UnitType.LUMINOUS_FLUX, Unit.LUMEN, MeasurementSystem.GetUnitString("lumen.name"),
							MeasurementSystem.GetUnitString("lumen.symbol"), MeasurementSystem.GetUnitString("lumen.desc"), GetUOM(Unit.CANDELA),
							GetUOM(Unit.STERADIAN));
					break;

				case Unit.LUX:
					// illuminance (lux)
					uom = CreateQuotientUOM(UnitType.ILLUMINANCE, Unit.LUX, MeasurementSystem.GetUnitString("lux.name"),
							MeasurementSystem.GetUnitString("lux.symbol"), MeasurementSystem.GetUnitString("lux.desc"), GetUOM(Unit.LUMEN),
							GetUOM(Unit.SQUARE_METRE));
					break;

				case Unit.BECQUEREL:
					// radioactivity (becquerel). Same base symbol as Hertz
					uom = CreateQuotientUOM(UnitType.RADIOACTIVITY, Unit.BECQUEREL, MeasurementSystem.GetUnitString("becquerel.name"),
							MeasurementSystem.GetUnitString("becquerel.symbol"), MeasurementSystem.GetUnitString("becquerel.desc"), GetOne(), GetSecond());
					break;

				case Unit.GRAY:
					// gray (Gy)
					uom = CreateQuotientUOM(UnitType.RADIATION_DOSE_ABSORBED, Unit.GRAY, MeasurementSystem.GetUnitString("gray.name"),
							MeasurementSystem.GetUnitString("gray.symbol"), MeasurementSystem.GetUnitString("gray.desc"), GetUOM(Unit.JOULE),
							GetUOM(Unit.KILOGRAM));
					break;

				case Unit.SIEVERT:
					// sievert (Sv)
					uom = CreateQuotientUOM(UnitType.RADIATION_DOSE_EFFECTIVE, Unit.SIEVERT, MeasurementSystem.GetUnitString("sievert.name"),
							MeasurementSystem.GetUnitString("sievert.symbol"), MeasurementSystem.GetUnitString("sievert.desc"), GetUOM(Unit.JOULE),
							GetUOM(Unit.KILOGRAM));
					break;

				case Unit.SIEVERTS_PER_HOUR:
					uom = CreateQuotientUOM(UnitType.RADIATION_DOSE_RATE, Unit.SIEVERTS_PER_HOUR, MeasurementSystem.GetUnitString("sph.name"),
							MeasurementSystem.GetUnitString("sph.symbol"), MeasurementSystem.GetUnitString("sph.desc"), GetUOM(Unit.SIEVERT), GetHour());
					break;

				case Unit.KATAL:
					// katal (kat)
					uom = CreateQuotientUOM(UnitType.CATALYTIC_ACTIVITY, Unit.KATAL, MeasurementSystem.GetUnitString("katal.name"),
							MeasurementSystem.GetUnitString("katal.symbol"), MeasurementSystem.GetUnitString("katal.desc"), GetUOM(Unit.MOLE), GetSecond());
					break;

				case Unit.UNIT:
					// Unit (U)
					uom = CreateScalarUOM(UnitType.CATALYTIC_ACTIVITY, Unit.UNIT, MeasurementSystem.GetUnitString("unit.name"),
							MeasurementSystem.GetUnitString("unit.symbol"), MeasurementSystem.GetUnitString("unit.desc"));
					uom.SetConversion(1.0E-06 / (double)60, GetUOM(Unit.KATAL));
					break;

				case Unit.INTERNATIONAL_UNIT:
					uom = CreateScalarUOM(UnitType.SUBSTANCE_AMOUNT, Unit.INTERNATIONAL_UNIT, MeasurementSystem.GetUnitString("iu.name"),
							MeasurementSystem.GetUnitString("iu.symbol"), MeasurementSystem.GetUnitString("iu.desc"));
					break;

				case Unit.ANGSTROM:
					// length
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.ANGSTROM, MeasurementSystem.GetUnitString("angstrom.name"),
							MeasurementSystem.GetUnitString("angstrom.symbol"), MeasurementSystem.GetUnitString("angstrom.desc"));
					uom.SetConversion(0.1, GetUOM(Prefix.NANO, GetUOM(Unit.METRE)));
					break;

				case Unit.BIT:
					// computer bit
					uom = CreateScalarUOM(UnitType.COMPUTER_SCIENCE, Unit.BIT, MeasurementSystem.GetUnitString("bit.name"),
							MeasurementSystem.GetUnitString("bit.symbol"), MeasurementSystem.GetUnitString("bit.desc"));
					break;

				case Unit.BYTE:
					// computer byte
					uom = CreateScalarUOM(UnitType.COMPUTER_SCIENCE, Unit.BYTE, MeasurementSystem.GetUnitString("byte.name"),
							MeasurementSystem.GetUnitString("byte.symbol"), MeasurementSystem.GetUnitString("byte.desc"));
					uom.SetConversion(8, GetUOM(Unit.BIT));
					break;

				case Unit.WATTS_PER_SQ_METRE:
					uom = CreateQuotientUOM(UnitType.IRRADIANCE, Unit.WATTS_PER_SQ_METRE, MeasurementSystem.GetUnitString("wsm.name"),
							MeasurementSystem.GetUnitString("wsm.symbol"), MeasurementSystem.GetUnitString("wsm.desc"), GetUOM(Unit.WATT),
							GetUOM(Unit.SQUARE_METRE));
					break;

				case Unit.PARSEC:
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.PARSEC, MeasurementSystem.GetUnitString("parsec.name"),
							MeasurementSystem.GetUnitString("parsec.symbol"), MeasurementSystem.GetUnitString("parsec.desc"));
					uom.SetConversion(3.08567758149137E+16, GetUOM(Unit.METRE));
					break;

				case Unit.ASTRONOMICAL_UNIT:
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.ASTRONOMICAL_UNIT, MeasurementSystem.GetUnitString("au.name"),
							MeasurementSystem.GetUnitString("au.symbol"), MeasurementSystem.GetUnitString("au.desc"));
					uom.SetConversion(1.49597870700E+11, GetUOM(Unit.METRE));
					break;

				default:
					break;
			}

			return uom;
		}

		private UnitOfMeasure CreateCustomaryUnit(Unit unit)
		{
			UnitOfMeasure uom = null;

			switch (unit)
			{
				case Unit.RANKINE:
					// Rankine (base)
					uom = CreateScalarUOM(UnitType.TEMPERATURE, Unit.RANKINE, MeasurementSystem.GetUnitString("rankine.name"),
							MeasurementSystem.GetUnitString("rankine.symbol"), MeasurementSystem.GetUnitString("rankine.desc"));

					// create bridge to SI
					uom.SetBridgeConversion((double)5 / (double)9, GetUOM(Unit.KELVIN), null);
					break;

				case Unit.FAHRENHEIT:
					// Fahrenheit
					uom = CreateScalarUOM(UnitType.TEMPERATURE, Unit.FAHRENHEIT, MeasurementSystem.GetUnitString("fahrenheit.name"),
							MeasurementSystem.GetUnitString("fahrenheit.symbol"), MeasurementSystem.GetUnitString("fahrenheit.desc"));
					uom.SetConversion(1, GetUOM(Unit.RANKINE), 459.67);
					break;

				case Unit.POUND_MASS:
					// lb mass (base)
					uom = CreateScalarUOM(UnitType.MASS, Unit.POUND_MASS, MeasurementSystem.GetUnitString("lbm.name"),
							MeasurementSystem.GetUnitString("lbm.symbol"), MeasurementSystem.GetUnitString("lbm.desc"));

					// create bridge to SI
					uom.SetBridgeConversion(0.45359237, GetUOM(Unit.KILOGRAM), null);
					break;

				case Unit.OUNCE:
					// ounce
					uom = CreateScalarUOM(UnitType.MASS, Unit.OUNCE, MeasurementSystem.GetUnitString("ounce.name"),
							MeasurementSystem.GetUnitString("ounce.symbol"), MeasurementSystem.GetUnitString("ounce.desc"));
					uom.SetConversion(0.0625, GetUOM(Unit.POUND_MASS));
					break;

				case Unit.TROY_OUNCE:
					// troy ounce
					uom = CreateScalarUOM(UnitType.MASS, Unit.TROY_OUNCE, MeasurementSystem.GetUnitString("troy_oz.name"),
							MeasurementSystem.GetUnitString("troy_oz.symbol"), MeasurementSystem.GetUnitString("troy_oz.desc"));
					uom.SetConversion(0.06857142857, GetUOM(Unit.POUND_MASS));
					break;

				case Unit.SLUG:
					// slug
					uom = CreateScalarUOM(UnitType.MASS, Unit.SLUG, MeasurementSystem.GetUnitString("slug.name"),
							MeasurementSystem.GetUnitString("slug.symbol"), MeasurementSystem.GetUnitString("slug.desc"));
					Quantity g = GetQuantity(Constant.GRAVITY).Convert(GetUOM(Unit.FEET_PER_SEC_SQUARED));
					uom.SetConversion(g.Amount, GetUOM(Unit.POUND_MASS));
					break;

				case Unit.FOOT:
					// foot (foot is base conversion unit)
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.FOOT, MeasurementSystem.GetUnitString("foot.name"),
							MeasurementSystem.GetUnitString("foot.symbol"), MeasurementSystem.GetUnitString("foot.desc"));

					// bridge to SI
					uom.SetBridgeConversion(0.3048, GetUOM(Unit.METRE), null);
					break;

				case Unit.INCH:
					// inch
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.INCH, MeasurementSystem.GetUnitString("inch.name"),
							MeasurementSystem.GetUnitString("inch.symbol"), MeasurementSystem.GetUnitString("inch.desc"));
					uom.SetConversion((double)1 / (double)12, GetUOM(Unit.FOOT));
					break;

				case Unit.MIL:
					// inch
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.MIL, MeasurementSystem.GetUnitString("mil.name"),
							MeasurementSystem.GetUnitString("mil.symbol"), MeasurementSystem.GetUnitString("mil.desc"));
					uom.SetConversion(Prefix.MILLI.Factor, GetUOM(Unit.INCH));
					break;

				case Unit.POINT:
					// point
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.POINT, MeasurementSystem.GetUnitString("point.name"),
							MeasurementSystem.GetUnitString("point.symbol"), MeasurementSystem.GetUnitString("point.desc"));
					uom.SetConversion((double)1 / (double)72, GetUOM(Unit.INCH));
					break;

				case Unit.YARD:
					// yard
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.YARD, MeasurementSystem.GetUnitString("yard.name"),
							MeasurementSystem.GetUnitString("yard.symbol"), MeasurementSystem.GetUnitString("yard.desc"));
					uom.SetConversion(3, GetUOM(Unit.FOOT));
					break;

				case Unit.MILE:
					// mile
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.MILE, MeasurementSystem.GetUnitString("mile.name"),
							MeasurementSystem.GetUnitString("mile.symbol"), MeasurementSystem.GetUnitString("mile.desc"));
					uom.SetConversion(5280, GetUOM(Unit.FOOT));
					break;

				case Unit.NAUTICAL_MILE:
					// nautical mile
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.NAUTICAL_MILE, MeasurementSystem.GetUnitString("NM.name"),
							MeasurementSystem.GetUnitString("NM.symbol"), MeasurementSystem.GetUnitString("NM.desc"));
					uom.SetConversion(6080, GetUOM(Unit.FOOT));
					break;

				case Unit.FATHOM:
					// fathom
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.FATHOM, MeasurementSystem.GetUnitString("fth.name"),
							MeasurementSystem.GetUnitString("fth.symbol"), MeasurementSystem.GetUnitString("fth.desc"));
					uom.SetConversion(6, GetUOM(Unit.FOOT));

					break;

				case Unit.PSI:
					// psi
					uom = CreateQuotientUOM(UnitType.PRESSURE, Unit.PSI, MeasurementSystem.GetUnitString("psi.name"),
							MeasurementSystem.GetUnitString("psi.symbol"), MeasurementSystem.GetUnitString("psi.desc"), GetUOM(Unit.POUND_FORCE),
							GetUOM(Unit.SQUARE_INCH));
					break;

				case Unit.IN_HG:
					// inches of Mercury
					uom = CreateScalarUOM(UnitType.PRESSURE, Unit.IN_HG, MeasurementSystem.GetUnitString("inhg.name"),
							MeasurementSystem.GetUnitString("inhg.symbol"), MeasurementSystem.GetUnitString("inhg.desc"));
					UnitOfMeasure u1 = CreateProductUOM(GetUOM(Unit.FOOT), GetUOM(Unit.SQUARE_SECOND));
					UnitOfMeasure u2 = CreateQuotientUOM(GetUOM(Unit.POUND_MASS), u1);
					uom.SetConversion(2275.520677, u2);
					break;

				case Unit.SQUARE_INCH:
					// square inch
					uom = CreatePowerUOM(UnitType.AREA, Unit.SQUARE_INCH, MeasurementSystem.GetUnitString("in2.name"),
							MeasurementSystem.GetUnitString("in2.symbol"), MeasurementSystem.GetUnitString("in2.desc"), GetUOM(Unit.INCH), 2);
					uom.SetConversion((double)1 / (double)144, GetUOM(Unit.SQUARE_FOOT));
					break;

				case Unit.SQUARE_FOOT:
					// square foot
					uom = CreatePowerUOM(UnitType.AREA, Unit.SQUARE_FOOT, MeasurementSystem.GetUnitString("ft2.name"),
							MeasurementSystem.GetUnitString("ft2.symbol"), MeasurementSystem.GetUnitString("ft2.desc"), GetUOM(Unit.FOOT), 2);
					break;

				case Unit.SQUARE_YARD:
					// square yard
					uom = CreatePowerUOM(UnitType.AREA, Unit.SQUARE_YARD, MeasurementSystem.GetUnitString("yd2.name"),
							MeasurementSystem.GetUnitString("yd2.symbol"), MeasurementSystem.GetUnitString("yd2.desc"), GetUOM(Unit.YARD), 2);
					break;

				case Unit.ACRE:
					// acre
					uom = CreateScalarUOM(UnitType.AREA, Unit.ACRE, MeasurementSystem.GetUnitString("acre.name"),
							MeasurementSystem.GetUnitString("acre.symbol"), MeasurementSystem.GetUnitString("acre.desc"));
					uom.SetConversion(43560, GetUOM(Unit.SQUARE_FOOT));
					break;

				case Unit.CUBIC_INCH:
					// cubic inch
					uom = CreatePowerUOM(UnitType.VOLUME, Unit.CUBIC_INCH, MeasurementSystem.GetUnitString("in3.name"),
							MeasurementSystem.GetUnitString("in3.symbol"), MeasurementSystem.GetUnitString("in3.desc"), GetUOM(Unit.INCH), 3);
					uom.SetConversion((double)1 / (double)1728, GetUOM(Unit.CUBIC_FOOT));
					break;

				case Unit.CUBIC_FOOT:
					// cubic feet
					uom = CreatePowerUOM(UnitType.VOLUME, Unit.CUBIC_FOOT, MeasurementSystem.GetUnitString("ft3.name"),
							MeasurementSystem.GetUnitString("ft3.symbol"), MeasurementSystem.GetUnitString("ft3.desc"), GetUOM(Unit.FOOT), 3);
					break;

				case Unit.CUBIC_FEET_PER_SEC:
					// flow (volume)
					uom = CreateQuotientUOM(UnitType.VOLUMETRIC_FLOW, Unit.CUBIC_FEET_PER_SEC,
							MeasurementSystem.GetUnitString("ft3PerSec.name"), MeasurementSystem.GetUnitString("ft3PerSec.symbol"),
							MeasurementSystem.GetUnitString("ft3PerSec.desc"), GetUOM(Unit.CUBIC_FOOT), GetSecond());
					break;

				case Unit.CORD:
					// cord
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.CORD, MeasurementSystem.GetUnitString("cord.name"),
							MeasurementSystem.GetUnitString("cord.symbol"), MeasurementSystem.GetUnitString("cord.desc"));
					uom.SetConversion(128, GetUOM(Unit.CUBIC_FOOT));
					break;

				case Unit.CUBIC_YARD:
					// cubic yard
					uom = CreatePowerUOM(UnitType.VOLUME, Unit.CUBIC_YARD, MeasurementSystem.GetUnitString("yd3.name"),
							MeasurementSystem.GetUnitString("yd3.symbol"), MeasurementSystem.GetUnitString("yd3.desc"), GetUOM(Unit.YARD), 3);
					break;

				case Unit.FEET_PER_SEC:
					// feet/sec
					uom = CreateQuotientUOM(UnitType.VELOCITY, Unit.FEET_PER_SEC, MeasurementSystem.GetUnitString("fps.name"),
							MeasurementSystem.GetUnitString("fps.symbol"), MeasurementSystem.GetUnitString("fps.desc"), GetUOM(Unit.FOOT), GetSecond());
					break;

				case Unit.KNOT:
					// knot
					uom = CreateScalarUOM(UnitType.VELOCITY, Unit.KNOT, MeasurementSystem.GetUnitString("knot.name"),
							MeasurementSystem.GetUnitString("knot.symbol"), MeasurementSystem.GetUnitString("knot.desc"));
					uom.SetConversion((double)6080 / (double)3600, GetUOM(Unit.FEET_PER_SEC));
					break;

				case Unit.FEET_PER_SEC_SQUARED:
					// acceleration
					uom = CreateQuotientUOM(UnitType.ACCELERATION, Unit.FEET_PER_SEC_SQUARED, MeasurementSystem.GetUnitString("ftps2.name"),
							MeasurementSystem.GetUnitString("ftps2.symbol"), MeasurementSystem.GetUnitString("ftps2.desc"), GetUOM(Unit.FOOT),
							GetUOM(Unit.SQUARE_SECOND));
					break;

				case Unit.HP:
					// HP (mechanical)
					uom = CreateProductUOM(UnitType.POWER, Unit.HP, MeasurementSystem.GetUnitString("hp.name"),
							MeasurementSystem.GetUnitString("hp.symbol"), MeasurementSystem.GetUnitString("hp.desc"), GetUOM(Unit.POUND_FORCE),
							GetUOM(Unit.FEET_PER_SEC));
					uom.ScalingFactor = 550d;
					break;

				case Unit.BTU:
					// BTU = 1055.056 Joules (778.169 ft-lbf)
					uom = CreateScalarUOM(UnitType.ENERGY, Unit.BTU, MeasurementSystem.GetUnitString("btu.name"),
							MeasurementSystem.GetUnitString("btu.symbol"), MeasurementSystem.GetUnitString("btu.desc"));
					uom.SetConversion(778.1692622659652, GetUOM(Unit.FOOT_POUND_FORCE));
					break;

				case Unit.FOOT_POUND_FORCE:
					// ft-lbf
					uom = CreateProductUOM(UnitType.ENERGY, Unit.FOOT_POUND_FORCE, MeasurementSystem.GetUnitString("ft_lbf.name"),
							MeasurementSystem.GetUnitString("ft_lbf.symbol"), MeasurementSystem.GetUnitString("ft_lbf.desc"), GetUOM(Unit.FOOT),
							GetUOM(Unit.POUND_FORCE));
					break;

				case Unit.POUND_FORCE:
					// force F = m·A (lbf)
					uom = CreateProductUOM(UnitType.FORCE, Unit.POUND_FORCE, MeasurementSystem.GetUnitString("lbf.name"),
							MeasurementSystem.GetUnitString("lbf.symbol"), MeasurementSystem.GetUnitString("lbf.desc"), GetUOM(Unit.POUND_MASS),
							GetUOM(Unit.FEET_PER_SEC_SQUARED));

					// factor is acceleration of gravity
					Quantity gravity = GetQuantity(Constant.GRAVITY).Convert(GetUOM(Unit.FEET_PER_SEC_SQUARED));
					uom.ScalingFactor = gravity.Amount;
					break;

				case Unit.GRAIN:
					// mass
					uom = CreateScalarUOM(UnitType.MASS, Unit.GRAIN, MeasurementSystem.GetUnitString("grain.name"),
							MeasurementSystem.GetUnitString("grain.symbol"), MeasurementSystem.GetUnitString("grain.desc"));
					uom.SetConversion((double)1 / (double)7000, GetUOM(Unit.POUND_MASS));
					break;

				case Unit.MILES_PER_HOUR:
					// velocity
					uom = CreateScalarUOM(UnitType.VELOCITY, Unit.MILES_PER_HOUR, MeasurementSystem.GetUnitString("mph.name"),
							MeasurementSystem.GetUnitString("mph.symbol"), MeasurementSystem.GetUnitString("mph.desc"));
					uom.SetConversion((double)5280 / (double)3600, GetUOM(Unit.FEET_PER_SEC));
					break;

				case Unit.REV_PER_MIN:
					// rpm
					uom = CreateQuotientUOM(UnitType.FREQUENCY, Unit.REV_PER_MIN, MeasurementSystem.GetUnitString("rpm.name"),
							MeasurementSystem.GetUnitString("rpm.symbol"), MeasurementSystem.GetUnitString("rpm.desc"), GetOne(), GetMinute());
					break;

				default:
					break;
			}

			return uom;
		}

		private UnitOfMeasure CreateUSUnit(Unit unit)
		{
			UnitOfMeasure uom = null;

			switch (unit)
			{

				case Unit.US_GALLON:
					// gallon
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.US_GALLON, MeasurementSystem.GetUnitString("us_gallon.name"),
							MeasurementSystem.GetUnitString("us_gallon.symbol"), MeasurementSystem.GetUnitString("us_gallon.desc"));
					uom.SetConversion(231, GetUOM(Unit.CUBIC_INCH));
					break;

				case Unit.US_BARREL:
					// barrel
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.US_BARREL, MeasurementSystem.GetUnitString("us_bbl.name"),
							MeasurementSystem.GetUnitString("us_bbl.symbol"), MeasurementSystem.GetUnitString("us_bbl.desc"));
					uom.SetConversion(42, GetUOM(Unit.US_GALLON));
					break;

				case Unit.US_BUSHEL:
					// bushel
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.US_BUSHEL, MeasurementSystem.GetUnitString("us_bu.name"),
							MeasurementSystem.GetUnitString("us_bu.symbol"), MeasurementSystem.GetUnitString("us_bu.desc"));
					uom.SetConversion(2150.42058, GetUOM(Unit.CUBIC_INCH));
					break;

				case Unit.US_FLUID_OUNCE:
					// fluid ounce
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.US_FLUID_OUNCE, MeasurementSystem.GetUnitString("us_fl_oz.name"),
							MeasurementSystem.GetUnitString("us_fl_oz.symbol"), MeasurementSystem.GetUnitString("us_fl_oz.desc"));
					uom.SetConversion(0.0078125, GetUOM(Unit.US_GALLON));
					break;

				case Unit.US_CUP:
					// cup
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.US_CUP, MeasurementSystem.GetUnitString("us_cup.name"),
							MeasurementSystem.GetUnitString("us_cup.symbol"), MeasurementSystem.GetUnitString("us_cup.desc"));
					uom.SetConversion(8, GetUOM(Unit.US_FLUID_OUNCE));
					break;

				case Unit.US_PINT:
					// pint
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.US_PINT, MeasurementSystem.GetUnitString("us_pint.name"),
							MeasurementSystem.GetUnitString("us_pint.symbol"), MeasurementSystem.GetUnitString("us_pint.desc"));
					uom.SetConversion(16, GetUOM(Unit.US_FLUID_OUNCE));
					break;

				case Unit.US_QUART:
					// quart
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.US_QUART, MeasurementSystem.GetUnitString("us_quart.name"),
							MeasurementSystem.GetUnitString("us_quart.symbol"), MeasurementSystem.GetUnitString("us_quart.desc"));
					uom.SetConversion(32, GetUOM(Unit.US_FLUID_OUNCE));
					break;

				case Unit.US_TABLESPOON:
					// tablespoon
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.US_TABLESPOON, MeasurementSystem.GetUnitString("us_tbsp.name"),
							MeasurementSystem.GetUnitString("us_tbsp.symbol"), MeasurementSystem.GetUnitString("us_tbsp.desc"));
					uom.SetConversion(0.5, GetUOM(Unit.US_FLUID_OUNCE));
					break;

				case Unit.US_TEASPOON:
					// teaspoon
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.US_TEASPOON, MeasurementSystem.GetUnitString("us_tsp.name"),
							MeasurementSystem.GetUnitString("us_tsp.symbol"), MeasurementSystem.GetUnitString("us_tsp.desc"));
					uom.SetConversion((double)1 / (double)6, GetUOM(Unit.US_FLUID_OUNCE));
					break;

				case Unit.US_TON:
					// ton
					uom = CreateScalarUOM(UnitType.MASS, Unit.US_TON, MeasurementSystem.GetUnitString("us_ton.name"),
							MeasurementSystem.GetUnitString("us_ton.symbol"), MeasurementSystem.GetUnitString("us_ton.desc"));
					uom.SetConversion(2000, GetUOM(Unit.POUND_MASS));
					break;

				default:
					break;
			}

			return uom;
		}

		private UnitOfMeasure CreateBRUnit(Unit unit)
		{
			UnitOfMeasure uom = null;

			switch (unit)
			{
				case Unit.BR_GALLON:
					// gallon
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.BR_GALLON, MeasurementSystem.GetUnitString("br_gallon.name"),
							MeasurementSystem.GetUnitString("br_gallon.symbol"), MeasurementSystem.GetUnitString("br_gallon.desc"));
					uom.SetConversion(277.4194327916215, GetUOM(Unit.CUBIC_INCH));
					break;

				case Unit.BR_BUSHEL:
					// bushel
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.BR_BUSHEL, MeasurementSystem.GetUnitString("br_bu.name"),
							MeasurementSystem.GetUnitString("br_bu.symbol"), MeasurementSystem.GetUnitString("br_bu.desc"));
					uom.SetConversion(8, GetUOM(Unit.BR_GALLON));
					break;

				case Unit.BR_FLUID_OUNCE:
					// fluid ounce
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.BR_FLUID_OUNCE, MeasurementSystem.GetUnitString("br_fl_oz.name"),
							MeasurementSystem.GetUnitString("br_fl_oz.symbol"), MeasurementSystem.GetUnitString("br_fl_oz.desc"));
					uom.SetConversion(0.00625, GetUOM(Unit.BR_GALLON));
					break;

				case Unit.BR_CUP:
					// cup
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.BR_CUP, MeasurementSystem.GetUnitString("br_cup.name"),
							MeasurementSystem.GetUnitString("br_cup.symbol"), MeasurementSystem.GetUnitString("br_cup.desc"));
					uom.SetConversion(8, GetUOM(Unit.BR_FLUID_OUNCE));
					break;

				case Unit.BR_PINT:
					// pint
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.BR_PINT, MeasurementSystem.GetUnitString("br_pint.name"),
							MeasurementSystem.GetUnitString("br_pint.symbol"), MeasurementSystem.GetUnitString("br_pint.desc"));
					uom.SetConversion(20, GetUOM(Unit.BR_FLUID_OUNCE));
					break;

				case Unit.BR_QUART:
					// quart
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.BR_QUART, MeasurementSystem.GetUnitString("br_quart.name"),
							MeasurementSystem.GetUnitString("br_quart.symbol"), MeasurementSystem.GetUnitString("br_quart.desc"));
					uom.SetConversion(40, GetUOM(Unit.BR_FLUID_OUNCE));
					break;

				case Unit.BR_TABLESPOON:
					// tablespoon
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.BR_TABLESPOON, MeasurementSystem.GetUnitString("br_tbsp.name"),
							MeasurementSystem.GetUnitString("br_tbsp.symbol"), MeasurementSystem.GetUnitString("br_tbsp.desc"));
					uom.SetConversion(0.625, GetUOM(Unit.BR_FLUID_OUNCE));
					break;

				case Unit.BR_TEASPOON:
					// teaspoon
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.BR_TEASPOON, MeasurementSystem.GetUnitString("br_tsp.name"),
							MeasurementSystem.GetUnitString("br_tsp.symbol"), MeasurementSystem.GetUnitString("br_tsp.desc"));
					uom.SetConversion((double)5 / (double)24, GetUOM(Unit.BR_FLUID_OUNCE));
					break;

				case Unit.BR_TON:
					// ton
					uom = CreateScalarUOM(UnitType.MASS, Unit.BR_TON, MeasurementSystem.GetUnitString("br_ton.name"),
							MeasurementSystem.GetUnitString("br_ton.symbol"), MeasurementSystem.GetUnitString("br_ton.desc"));
					uom.SetConversion(2240, GetUOM(Unit.POUND_MASS));
					break;

				default:
					break;
			}

			return uom;
		}

		private UnitOfMeasure CreateFinancialUnit(Unit unit)
		{
			UnitOfMeasure uom = null;

			switch (unit)
			{

				case Unit.US_DOLLAR:
					uom = CreateScalarUOM(UnitType.CURRENCY, Unit.US_DOLLAR, MeasurementSystem.GetUnitString("us_dollar.name"),
							MeasurementSystem.GetUnitString("us_dollar.symbol"), MeasurementSystem.GetUnitString("us_dollar.desc"));
					break;

				case Unit.EURO:
					uom = CreateScalarUOM(UnitType.CURRENCY, Unit.EURO, MeasurementSystem.GetUnitString("euro.name"),
							MeasurementSystem.GetUnitString("euro.symbol"), MeasurementSystem.GetUnitString("euro.desc"));
					break;

				case Unit.YUAN:
					uom = CreateScalarUOM(UnitType.CURRENCY, Unit.YUAN, MeasurementSystem.GetUnitString("yuan.name"),
							MeasurementSystem.GetUnitString("yuan.symbol"), MeasurementSystem.GetUnitString("yuan.desc"));
					break;

				default:
					break;
			}

			return uom;
		}

		private UnitOfMeasure CreateUOM(UnitType type, Unit? id, string name, string symbol, string description)
		{

			if (symbol == null || symbol.Length == 0)
			{
				throw new Exception(MeasurementSystem.GetMessage("symbol.cannot.be.null"));
			}

			SymbolRegistry.TryGetValue(symbol, out UnitOfMeasure uom);
			if (uom == null)
			{
				// create a new one
				uom = new UnitOfMeasure(type, name, symbol, description);
			}
			return uom;
		}

		private UnitOfMeasure CreateScalarUOM(UnitType type, Unit? id, string name, string symbol, string description)
		{
			UnitOfMeasure uom = CreateUOM(type, id, name, symbol, description);
			uom.Enumeration = id;
			RegisterUnit(uom);

			return uom;
		}

		/// <summary>Create a unit of measure that is not a power, product or quotient</summary>
		/// 
		/// <param name="type">UnitType</param>            
		/// <param name="name">Name of unit of measure</param>          
		/// <param name="symbol">Symbol (must be unique)</param>           
		/// <param name="description">Description of unit of measure</param>
		/// 
		/// <returns>UnitOfMeasure</returns>
		///
		public UnitOfMeasure CreateScalarUOM(UnitType type, string name, string symbol, string description)

		{
			return CreateScalarUOM(type, null, name, symbol, description);
		}

		/// <summary>Create a unit of measure with a base raised to an integral power</summary>
		/// 
		/// <param name="type">UnitType</param>         
		/// <param name="id">Unit</param>        
		/// <param name="name">Name of unit of measure</param>          
		/// <param name="symbol">Symbol (must be unique)</param>          
		/// <param name="description">Description of unit of measure</param>          
		/// <param name="baseUOM">UnitOfMeasure</param>          
		/// <param name="exponent">Exponent</param>
		/// 
		/// <returns>UnitOfMeasure</returns>
		///
		public UnitOfMeasure CreatePowerUOM(UnitType type, Unit? id, string name, string symbol, string description, UnitOfMeasure baseUOM, int exponent)
		{
			UnitOfMeasure uom = CreateUOM(type, id, name, symbol, description);
			uom.SetPowerUnit(baseUOM, exponent);
			uom.Enumeration = id;
			RegisterUnit(uom);
			return uom;
		}

		/// <summary>Create a unit of measure with a base raised to an integral exponent</summary>
		/// <param name="type">UnitType</param>
		/// <param name="name">Name of unit of measure</param>          
		/// <param name="symbol">Symbol (must be unique)</param>        
		/// <param name="description">Description of unit of measure</param>       
		/// <param name="baseUOM">UnitOfMeasure</param>            
		/// <param name="exponent">Exponent</param>
		///            
		/// <returns>UnitOfMeasure</returns>
		///
		public UnitOfMeasure CreatePowerUOM(UnitType type, string name, string symbol, string description, UnitOfMeasure baseUOM, int exponent)
		{
			return CreatePowerUOM(type, null, name, symbol, description, baseUOM, exponent);
		}

		/// <summary>Create a unit of measure with a base raised to an integral exponent</summary>
		/// 
		/// <param name="baseUOM">UnitOfMeasure</param>          
		/// <param name="exponent">Exponent</param>
		///            
		/// <returns>UnitOfMeasure</returns>
		///
		public UnitOfMeasure CreatePowerUOM(UnitOfMeasure baseUOM, int exponent)
		{
			if (baseUOM == null)
			{
				string msg = String.Format(MeasurementSystem.GetMessage("base.cannot.be.null"), "");
				throw new Exception(msg);
			}

			string symbol = UnitOfMeasure.GeneratePowerSymbol(baseUOM, exponent);
			return CreatePowerUOM(UnitType.UNCLASSIFIED, null, null, symbol, null, baseUOM, exponent);
		}


		/// <summary>Create a unit of measure that is the product of two other units of
		/// measure</summary>
		/// 
		/// <param name="type">UnitType</param>
		/// <param name="id">Unit</param>
		/// <param name="name">Name of unit of measure</param>          
		/// <param name="symbol">Symbol (must be unique)</param>          
		/// <param name="description">Description of unit of measure</param>          
		/// <param name="multiplier">UnitOfMeasure multiplier</param>          
		/// <param name="multiplicand">UnitOfMeasure multiplicand</param>
		///                     
		/// <returns>UnitOfMeasure</returns>
		///
		public UnitOfMeasure CreateProductUOM(UnitType type, Unit? id, string name, string symbol, string description,
				UnitOfMeasure multiplier, UnitOfMeasure multiplicand)
		{

			UnitOfMeasure uom = CreateUOM(type, id, name, symbol, description);
			uom.SetProductUnits(multiplier, multiplicand);
			uom.Enumeration = id;
			RegisterUnit(uom);
			return uom;
		}

		/// <summary>Create a unit of measure that is the product of two other units of
		/// measure</summary>
		/// 
		/// <param name="type">UnitType</param>
		/// <param name="name">Name of unit of measure</param>           
		/// <param name="symbol">Symbol (must be unique)</param>         
		/// <param name="description">Description of unit of measure</param>        
		/// <param name="multiplier">UnitOfMeasure multiplier</param>          
		/// <param name="multiplicand">UnitOfMeasure multiplicand</param>          
		///            
		/// <returns>UnitOfMeasure</returns>
		///
		public UnitOfMeasure CreateProductUOM(UnitType type, string name, string symbol, string description,
				UnitOfMeasure multiplier, UnitOfMeasure multiplicand)
		{
			return CreateProductUOM(type, null, name, symbol, description, multiplier, multiplicand);
		}

		/// <summary>Create a unit of measure that is the product of two other units of
		/// measure</summary>
		/// 
		/// <param name="multiplier">UnitOfMeasure multiplier</param>           
		/// <param name="multiplicand">UnitOfMeasure multiplicand</param>
		///            
		/// <returns>UnitOfMeasure</returns>
		///
		public UnitOfMeasure CreateProductUOM(UnitOfMeasure multiplier, UnitOfMeasure multiplicand)
		{
			if (multiplier == null)
			{
				string msg = String.Format(MeasurementSystem.GetMessage("multiplier.cannot.be.null"), "");
				throw new Exception(msg);
			}

			if (multiplicand == null)
			{
				string msg = String.Format(MeasurementSystem.GetMessage("multiplicand.cannot.be.null"), "");
				throw new Exception(msg);
			}

			string symbol = UnitOfMeasure.GenerateProductSymbol(multiplier, multiplicand);
			return CreateProductUOM(UnitType.UNCLASSIFIED, null, null, symbol, null, multiplier, multiplicand);
		}

		/// <summary>Create a unit of measure that is a unit divided by another unit</summary>
		/// 
		/// <param name="type">UnitType</param>
		///            }
		/// <param name="id">Unit</param>
		///            }
		/// <param name="name">Name of unit of measure</param>
		///            
		/// <param name="symbol">Symbol (must be unique)</param>
		///            
		/// <param name="description">Description of unit of measure</param>
		///            
		/// <param name="dividend">UnitOfMeasure</param>
		///            
		/// <param name="divisor">UnitOfMeasure</param>
		///            
		/// <returns>UnitOfMeasure</returns>
		///
		public UnitOfMeasure CreateQuotientUOM(UnitType type, Unit? id, string name, string symbol, string description,
				UnitOfMeasure dividend, UnitOfMeasure divisor)
		{
			UnitOfMeasure uom = CreateUOM(type, id, name, symbol, description);
			uom.SetQuotientUnits(dividend, divisor);
			uom.Enumeration = id;
			RegisterUnit(uom);
			return uom;
		}


		/// <summary>Create a unit of measure that is a unit divided by another unit</summary>
		/// 
		/// <param name="type">UnitType</param>
		/// <param name="name">Name of unit of measure</param>           
		/// <param name="symbol">Symbol (must be unique)</param>          
		/// <param name="description">Description of unit of measure</param>          
		/// <param name="dividend">UnitOfMeasure</param>         
		/// <param name="divisor">UnitOfMeasure</param>
		///            
		/// <returns>UnitOfMeasure</returns>
		///
		public UnitOfMeasure CreateQuotientUOM(UnitType type, string name, string symbol, string description,
				UnitOfMeasure dividend, UnitOfMeasure divisor)
		{
			return this.CreateQuotientUOM(type, null, name, symbol, description, dividend, divisor);
		}

		/// <summary>Create a unit of measure that is a unit divided by another unit</summary>
		/// 
		/// <param name="dividend">UnitOfMeasure</param>          
		/// <param name="divisor">UnitOfMeasure</param>
		///            
		/// <returns>UnitOfMeasure</returns>
		///
		public UnitOfMeasure CreateQuotientUOM(UnitOfMeasure dividend, UnitOfMeasure divisor)
		{
			if (dividend == null)
			{
				string msg = String.Format(MeasurementSystem.GetMessage("dividend.cannot.be.null"), "");
				throw new Exception(msg);
			}

			if (divisor == null)
			{
				string msg = String.Format(MeasurementSystem.GetMessage("divisor.cannot.be.null"), "");
				throw new Exception(msg);
			}

			string symbol = UnitOfMeasure.GenerateQuotientSymbol(dividend, divisor);
			return CreateQuotientUOM(UnitType.UNCLASSIFIED, null, null, symbol, null, dividend, divisor);
		}

		/// <summary>Get the quantity defined as a contant value</summary>
		/// 
		/// <param name="constant">Constant</param>
		///            
		/// <returns>Quantity</returns>
		///
		public Quantity GetQuantity(Constant constant)
		{
			Quantity named = null;

			switch (constant)
			{
				case Constant.LIGHT_VELOCITY:
					named = new Quantity(299792458, GetUOM(Unit.METRE_PER_SEC));
					named.Name = MeasurementSystem.GetUnitString("light.name");
					named.Symbol = MeasurementSystem.GetUnitString("light.symbol");
					named.Description = MeasurementSystem.GetUnitString("light.desc");
					break;

				case Constant.LIGHT_YEAR:
					Quantity year = new Quantity(1, GetUOM(Unit.JULIAN_YEAR));
					named = GetQuantity(Constant.LIGHT_VELOCITY).Multiply(year);
					named.Name = MeasurementSystem.GetUnitString("ly.name");
					named.Symbol = MeasurementSystem.GetUnitString("ly.symbol");
					named.Description = MeasurementSystem.GetUnitString("ly.desc");
					break;

				case Constant.GRAVITY:
					named = new Quantity(9.80665, GetUOM(Unit.METRE_PER_SEC_SQUARED));
					named.Name = MeasurementSystem.GetUnitString("gravity.name");
					named.Symbol = MeasurementSystem.GetUnitString("gravity.symbol");
					named.Description = MeasurementSystem.GetUnitString("gravity.desc");
					break;

				case Constant.PLANCK_CONSTANT:
					UnitOfMeasure js = CreateProductUOM(GetUOM(Unit.JOULE), GetSecond());
					named = new Quantity(6.62607015E-34, js);
					named.Name = MeasurementSystem.GetUnitString("planck.name");
					named.Symbol = MeasurementSystem.GetUnitString("planck.symbol");
					named.Description = MeasurementSystem.GetUnitString("planck.desc");
					break;

				case Constant.BOLTZMANN_CONSTANT:
					UnitOfMeasure jk = CreateQuotientUOM(GetUOM(Unit.JOULE), GetUOM(Unit.KELVIN));
					named = new Quantity(1.380649E-23, jk);
					named.Name = MeasurementSystem.GetUnitString("boltzmann.name");
					named.Symbol = MeasurementSystem.GetUnitString("boltzmann.symbol");
					named.Description = MeasurementSystem.GetUnitString("boltzmann.desc");
					break;

				case Constant.AVOGADRO_CONSTANT:
					// NA
					named = new Quantity(6.02214076E+23, GetOne());
					named.Name = MeasurementSystem.GetUnitString("avo.name");
					named.Symbol = MeasurementSystem.GetUnitString("avo.symbol");
					named.Description = MeasurementSystem.GetUnitString("avo.desc");
					break;

				case Constant.GAS_CONSTANT:
					// R
					named = GetQuantity(Constant.BOLTZMANN_CONSTANT).Multiply(GetQuantity(Constant.AVOGADRO_CONSTANT));
					named.Name = MeasurementSystem.GetUnitString("gas.name");
					named.Symbol = MeasurementSystem.GetUnitString("gas.symbol");
					named.Description = MeasurementSystem.GetUnitString("gas.desc");
					break;

				case Constant.ELEMENTARY_CHARGE:
					// e
					named = new Quantity(1.602176634E-19, GetUOM(Unit.COULOMB));
					named.Name = MeasurementSystem.GetUnitString("e.name");
					named.Symbol = MeasurementSystem.GetUnitString("e.symbol");
					named.Description = MeasurementSystem.GetUnitString("e.desc");
					break;

				case Constant.FARADAY_CONSTANT:
					// F = e.NA
					Quantity qe = GetQuantity(Constant.ELEMENTARY_CHARGE);
					named = qe.Multiply(GetQuantity(Constant.AVOGADRO_CONSTANT));
					named.Name = MeasurementSystem.GetUnitString("faraday.name");
					named.Symbol = MeasurementSystem.GetUnitString("faraday.symbol");
					named.Description = MeasurementSystem.GetUnitString("faraday.desc");
					break;

				case Constant.ELECTRIC_PERMITTIVITY:
					// epsilon0 = 1/(mu0*c^2)
					Quantity vc = GetQuantity(Constant.LIGHT_VELOCITY);
					named = GetQuantity(Constant.MAGNETIC_PERMEABILITY).Multiply(vc).Multiply(vc).Invert();
					named.Name = MeasurementSystem.GetUnitString("eps0.name");
					named.Symbol = MeasurementSystem.GetUnitString("eps0.symbol");
					named.Description = MeasurementSystem.GetUnitString("eps0.desc");
					break;

				case Constant.MAGNETIC_PERMEABILITY:
					// mu0
					UnitOfMeasure hm = CreateQuotientUOM(GetUOM(Unit.HENRY), GetUOM(Unit.METRE));
					double fourPi = 4.0E-07 * Math.PI;
					named = new Quantity(fourPi, hm);
					named.Name = MeasurementSystem.GetUnitString("mu0.name");
					named.Symbol = MeasurementSystem.GetUnitString("mu0.symbol");
					named.Description = MeasurementSystem.GetUnitString("mu0.desc");
					break;

				case Constant.ELECTRON_MASS:
					// me
					named = new Quantity(9.1093835611E-28, GetUOM(Unit.GRAM));
					named.Name = MeasurementSystem.GetUnitString("me.name");
					named.Symbol = MeasurementSystem.GetUnitString("me.symbol");
					named.Description = MeasurementSystem.GetUnitString("me.desc");
					break;

				case Constant.PROTON_MASS:
					// mp
					named = new Quantity(1.67262189821E-24, GetUOM(Unit.GRAM));
					named.Name = MeasurementSystem.GetUnitString("mp.name");
					named.Symbol = MeasurementSystem.GetUnitString("mp.symbol");
					named.Description = MeasurementSystem.GetUnitString("mp.desc");
					break;

				case Constant.STEFAN_BOLTZMANN:
					UnitOfMeasure k4 = CreatePowerUOM(GetUOM(Unit.KELVIN), 4);
					UnitOfMeasure sb = CreateQuotientUOM(GetUOM(Unit.WATTS_PER_SQ_METRE), k4);
					named = new Quantity(5.67036713E-08, sb);
					named.Name = MeasurementSystem.GetUnitString("sb.name");
					named.Symbol = MeasurementSystem.GetUnitString("sb.symbol");
					named.Description = MeasurementSystem.GetUnitString("sb.desc");
					break;

				case Constant.HUBBLE_CONSTANT:
					UnitOfMeasure kps = GetUOM(Prefix.KILO, GetUOM(Unit.METRE_PER_SEC));
					UnitOfMeasure mpc = GetUOM(Prefix.MEGA, GetUOM(Unit.PARSEC));
					UnitOfMeasure hubble = CreateQuotientUOM(kps, mpc);
					named = new Quantity(71.9, hubble);
					named.Name = MeasurementSystem.GetUnitString("hubble.name");
					named.Symbol = MeasurementSystem.GetUnitString("hubble.symbol");
					named.Description = MeasurementSystem.GetUnitString("hubble.desc");
					break;

				case Constant.CAESIUM_FREQUENCY:
					named = new Quantity(9192631770d, GetUOM(Unit.HERTZ));
					named.Name = MeasurementSystem.GetUnitString("caesium.name");
					named.Symbol = MeasurementSystem.GetUnitString("caesium.symbol");
					named.Description = MeasurementSystem.GetUnitString("caesium.desc");
					break;

				case Constant.LUMINOUS_EFFICACY:
					UnitOfMeasure kcd = CreateQuotientUOM(GetUOM(Unit.LUMEN), GetUOM(Unit.WATT));
					named = new Quantity(683d, kcd);
					named.Name = MeasurementSystem.GetUnitString("kcd.name");
					named.Symbol = MeasurementSystem.GetUnitString("kcd.symbol");
					named.Description = MeasurementSystem.GetUnitString("kcd.desc");
					break;

				default:
					break;
			}

			return named;
		}

		/// <summary>Cache this unit of measure</summary>
		/// 
		/// <param name="uom">UnitOfMeasure to cache</param>
		///            
		public void RegisterUnit(UnitOfMeasure uom)
		{
			string key = uom.Symbol;

			// get first by symbol
			if (SymbolRegistry.ContainsKey(key))
			{
				// already cached
				return;
			}

			// cache it
			SymbolRegistry[key] = uom;

			// next by unit enumeration
			Unit? id = uom.Enumeration;

			if (id.HasValue)
			{
				UnitRegistry[id.Value] = uom;
			}

			// finally by base symbol
			key = uom.GetBaseSymbol();

			if (!BaseRegistry.ContainsKey(key))
			{
				BaseRegistry[key] = uom;
			}
		}

		/// <summary>Remove a unit from the cache</summary>
		/// 
		/// <param name="uom">UnitOfMeasure to uncache</param>
		/// 
		public void UnregisterUnit(UnitOfMeasure uom)
		{
			if (uom == null)
			{
				return;
			}

			lock (_instanceLock)
			{
				UnitOfMeasure removedUOM;
				if (uom.Enumeration.HasValue)
				{
					UnitRegistry.TryRemove(uom.Enumeration.Value, out removedUOM);
				}

				// remove by symbol and base symbol
				SymbolRegistry.TryRemove(uom.Symbol, out removedUOM);
				BaseRegistry.TryRemove(uom.GetBaseSymbol(), out removedUOM);
			}
		}

		/// <summary>Remove all cached units of measure</summary>
		public void ClearCache()
		{
			SymbolRegistry.Clear();
			BaseRegistry.Clear();
			UnitRegistry.Clear();
		}

		/// <summary>Get all units currently cached by this measurement system</summary>
		/// 
		/// <returns>List of UnitOfMeasure</returns>
		/// 
		public List<UnitOfMeasure> GetRegisteredUnits()
		{
			ICollection<UnitOfMeasure> units = SymbolRegistry.Values;
			List<UnitOfMeasure> list = new List<UnitOfMeasure>(units);
			list.Sort();
			return list;
		}

		/// <summary>Get all the units of measure of the specified type</summary>
		/// 
		/// <param name="type">UnitType</param>
		///            
		/// <returns>List of UnitOfMeasure</returns>
		///
		public List<UnitOfMeasure> GetUnitsOfMeasure(UnitType type)
		{
			List<UnitOfMeasure> units = new List<UnitOfMeasure>();

			switch (type)
			{
				case UnitType.LENGTH:
					// SI
					units.Add(GetUOM(Unit.METRE));
					units.Add(GetUOM(Unit.ANGSTROM));
					units.Add(GetUOM(Unit.PARSEC));
					units.Add(GetUOM(Unit.ASTRONOMICAL_UNIT));

					// customary
					units.Add(GetUOM(Unit.FOOT));
					units.Add(GetUOM(Unit.INCH));
					units.Add(GetUOM(Unit.MIL));
					units.Add(GetUOM(Unit.POINT));
					units.Add(GetUOM(Unit.YARD));
					units.Add(GetUOM(Unit.MILE));
					units.Add(GetUOM(Unit.NAUTICAL_MILE));
					units.Add(GetUOM(Unit.FATHOM));
					break;

				case UnitType.MASS:
					// SI
					units.Add(GetUOM(Unit.KILOGRAM));
					units.Add(GetUOM(Unit.TONNE));
					units.Add(GetUOM(Unit.CARAT));

					// customary
					units.Add(GetUOM(Unit.POUND_MASS));
					units.Add(GetUOM(Unit.OUNCE));
					units.Add(GetUOM(Unit.TROY_OUNCE));
					units.Add(GetUOM(Unit.SLUG));
					units.Add(GetUOM(Unit.GRAIN));

					// US
					units.Add(GetUOM(Unit.US_TON));

					// British
					units.Add(GetUOM(Unit.BR_TON));
					break;

				case UnitType.TIME:
					units.Add(GetUOM(Unit.SECOND));
					units.Add(GetUOM(Unit.MINUTE));
					units.Add(GetUOM(Unit.HOUR));
					units.Add(GetUOM(Unit.DAY));
					units.Add(GetUOM(Unit.WEEK));
					units.Add(GetUOM(Unit.JULIAN_YEAR));

					break;

				case UnitType.ACCELERATION:
					units.Add(GetUOM(Unit.METRE_PER_SEC_SQUARED));
					units.Add(GetUOM(Unit.FEET_PER_SEC_SQUARED));
					break;

				case UnitType.AREA:
					// customary
					units.Add(GetUOM(Unit.SQUARE_INCH));
					units.Add(GetUOM(Unit.SQUARE_FOOT));
					units.Add(GetUOM(Unit.SQUARE_YARD));
					units.Add(GetUOM(Unit.ACRE));

					// SI
					units.Add(GetUOM(Unit.SQUARE_METRE));
					units.Add(GetUOM(Unit.HECTARE));

					break;

				case UnitType.CATALYTIC_ACTIVITY:
					units.Add(GetUOM(Unit.KATAL));
					units.Add(GetUOM(Unit.UNIT));
					break;

				case UnitType.COMPUTER_SCIENCE:
					units.Add(GetUOM(Unit.BIT));
					units.Add(GetUOM(Unit.BYTE));
					break;

				case UnitType.DENSITY:
					units.Add(GetUOM(Unit.KILOGRAM_PER_CU_METRE));
					break;

				case UnitType.DYNAMIC_VISCOSITY:
					units.Add(GetUOM(Unit.PASCAL_SECOND));
					break;

				case UnitType.ELECTRIC_CAPACITANCE:
					units.Add(GetUOM(Unit.FARAD));
					break;

				case UnitType.ELECTRIC_CHARGE:
					units.Add(GetUOM(Unit.COULOMB));
					break;

				case UnitType.ELECTRIC_CONDUCTANCE:
					units.Add(GetUOM(Unit.SIEMENS));
					break;

				case UnitType.ELECTRIC_CURRENT:
					units.Add(GetUOM(Unit.AMPERE));
					break;

				case UnitType.ELECTRIC_FIELD_STRENGTH:
					units.Add(GetUOM(Unit.AMPERE_PER_METRE));
					break;

				case UnitType.ELECTRIC_INDUCTANCE:
					units.Add(GetUOM(Unit.HENRY));
					break;

				case UnitType.ELECTRIC_PERMITTIVITY:
					units.Add(GetUOM(Unit.FARAD_PER_METRE));
					break;

				case UnitType.ELECTRIC_RESISTANCE:
					units.Add(GetUOM(Unit.OHM));
					break;

				case UnitType.ELECTROMOTIVE_FORCE:
					units.Add(GetUOM(Unit.VOLT));
					break;

				case UnitType.ENERGY:
					// customary
					units.Add(GetUOM(Unit.BTU));
					units.Add(GetUOM(Unit.FOOT_POUND_FORCE));

					// SI
					units.Add(GetUOM(Unit.CALORIE));
					units.Add(GetUOM(Unit.NEWTON_METRE));
					units.Add(GetUOM(Unit.JOULE));
					units.Add(GetUOM(Unit.WATT_HOUR));
					units.Add(GetUOM(Unit.ELECTRON_VOLT));
					break;

				case UnitType.CURRENCY:
					units.Add(GetUOM(Unit.US_DOLLAR));
					units.Add(GetUOM(Unit.EURO));
					units.Add(GetUOM(Unit.YUAN));
					break;

				case UnitType.FORCE:
					// customary
					units.Add(GetUOM(Unit.POUND_FORCE));

					// SI
					units.Add(GetUOM(Unit.NEWTON));
					break;

				case UnitType.FREQUENCY:
					units.Add(GetUOM(Unit.REV_PER_MIN));
					units.Add(GetUOM(Unit.HERTZ));
					units.Add(GetUOM(Unit.RAD_PER_SEC));
					break;

				case UnitType.ILLUMINANCE:
					units.Add(GetUOM(Unit.LUX));
					break;

				case UnitType.INTENSITY:
					units.Add(GetUOM(Unit.DECIBEL));
					break;

				case UnitType.IRRADIANCE:
					units.Add(GetUOM(Unit.WATTS_PER_SQ_METRE));
					break;

				case UnitType.KINEMATIC_VISCOSITY:
					units.Add(GetUOM(Unit.SQUARE_METRE_PER_SEC));
					break;

				case UnitType.LUMINOSITY:
					units.Add(GetUOM(Unit.CANDELA));
					break;

				case UnitType.LUMINOUS_FLUX:
					units.Add(GetUOM(Unit.LUMEN));
					break;

				case UnitType.MAGNETIC_FLUX:
					units.Add(GetUOM(Unit.WEBER));
					break;

				case UnitType.MAGNETIC_FLUX_DENSITY:
					units.Add(GetUOM(Unit.TESLA));
					break;

				case UnitType.MASS_FLOW:
					units.Add(GetUOM(Unit.KILOGRAM_PER_SEC));
					break;

				case UnitType.MOLAR_CONCENTRATION:
					units.Add(GetUOM(Unit.MOLARITY));
					break;

				case UnitType.PLANE_ANGLE:
					units.Add(GetUOM(Unit.DEGREE));
					units.Add(GetUOM(Unit.RADIAN));
					units.Add(GetUOM(Unit.ARC_SECOND));
					break;

				case UnitType.POWER:
					units.Add(GetUOM(Unit.HP));
					units.Add(GetUOM(Unit.WATT));
					break;

				case UnitType.PRESSURE:
					// customary
					units.Add(GetUOM(Unit.PSI));
					units.Add(GetUOM(Unit.IN_HG));

					// SI
					units.Add(GetUOM(Unit.PASCAL));
					units.Add(GetUOM(Unit.ATMOSPHERE));
					units.Add(GetUOM(Unit.BAR));
					break;

				case UnitType.RADIATION_DOSE_ABSORBED:
					units.Add(GetUOM(Unit.GRAY));
					break;

				case UnitType.RADIATION_DOSE_EFFECTIVE:
					units.Add(GetUOM(Unit.SIEVERT));
					break;

				case UnitType.RADIATION_DOSE_RATE:
					units.Add(GetUOM(Unit.SIEVERTS_PER_HOUR));
					break;

				case UnitType.RADIOACTIVITY:
					units.Add(GetUOM(Unit.BECQUEREL));
					break;

				case UnitType.RECIPROCAL_LENGTH:
					units.Add(GetUOM(Unit.DIOPTER));
					break;

				case UnitType.SOLID_ANGLE:
					units.Add(GetUOM(Unit.STERADIAN));
					break;

				case UnitType.SUBSTANCE_AMOUNT:
					units.Add(GetUOM(Unit.MOLE));
					units.Add(GetUOM(Unit.EQUIVALENT));
					units.Add(GetUOM(Unit.INTERNATIONAL_UNIT));
					break;

				case UnitType.TEMPERATURE:
					// customary
					units.Add(GetUOM(Unit.RANKINE));
					units.Add(GetUOM(Unit.FAHRENHEIT));

					// SI
					units.Add(GetUOM(Unit.KELVIN));
					units.Add(GetUOM(Unit.CELSIUS));
					break;

				case UnitType.TIME_SQUARED:
					units.Add(GetUOM(Unit.SQUARE_SECOND));
					break;

				case UnitType.UNCLASSIFIED:
					break;

				case UnitType.UNITY:
					units.Add(GetUOM(Unit.ONE));
					units.Add(GetUOM(Unit.PERCENT));
					break;

				case UnitType.VELOCITY:
					// customary
					units.Add(GetUOM(Unit.FEET_PER_SEC));
					units.Add(GetUOM(Unit.MILES_PER_HOUR));
					units.Add(GetUOM(Unit.KNOT));

					// SI
					units.Add(GetUOM(Unit.METRE_PER_SEC));
					break;

				case UnitType.VOLUME:
					// British
					units.Add(GetUOM(Unit.BR_BUSHEL));
					units.Add(GetUOM(Unit.BR_CUP));
					units.Add(GetUOM(Unit.BR_FLUID_OUNCE));
					units.Add(GetUOM(Unit.BR_GALLON));
					units.Add(GetUOM(Unit.BR_PINT));
					units.Add(GetUOM(Unit.BR_QUART));
					units.Add(GetUOM(Unit.BR_TABLESPOON));
					units.Add(GetUOM(Unit.BR_TEASPOON));

					// customary
					units.Add(GetUOM(Unit.CUBIC_FOOT));
					units.Add(GetUOM(Unit.CUBIC_YARD));
					units.Add(GetUOM(Unit.CUBIC_INCH));
					units.Add(GetUOM(Unit.CORD));

					// SI
					units.Add(GetUOM(Unit.CUBIC_METRE));
					units.Add(GetUOM(Unit.LITRE));

					// US
					units.Add(GetUOM(Unit.US_BARREL));
					units.Add(GetUOM(Unit.US_BUSHEL));
					units.Add(GetUOM(Unit.US_CUP));
					units.Add(GetUOM(Unit.US_FLUID_OUNCE));
					units.Add(GetUOM(Unit.US_GALLON));
					units.Add(GetUOM(Unit.US_PINT));
					units.Add(GetUOM(Unit.US_QUART));
					units.Add(GetUOM(Unit.US_TABLESPOON));
					units.Add(GetUOM(Unit.US_TEASPOON));
					break;

				case UnitType.VOLUMETRIC_FLOW:
					units.Add(GetUOM(Unit.CUBIC_METRE_PER_SEC));
					units.Add(GetUOM(Unit.CUBIC_FEET_PER_SEC));
					break;

				default:
					break;
			}

			return units;
		}

		/// <summary>
		/// Get the map of fundamental unit types for this unit type
		/// </summary>
		/// <param name="unitType">Type of unit of measure</param>
		/// <returns>Dictionary of unit type and exponent</returns>
		public ConcurrentDictionary<UnitType, int> GetTypeMap(UnitType unitType)
		{
			// check cache
			if (UnitTypeRegistry.TryGetValue(unitType, out ConcurrentDictionary<UnitType, int> cachedMap))
			{
				return cachedMap;
			}

			// create map
			cachedMap = new ConcurrentDictionary<UnitType, int>();
			UnitTypeRegistry[unitType] = cachedMap;

			// base types have empty maps
			switch (unitType)
			{
				case UnitType.UNITY:
					break;
				case UnitType.LENGTH:
					break;
				case UnitType.MASS:
					break;
				case UnitType.TIME:
					break;
				case UnitType.ELECTRIC_CURRENT:
					break;
				case UnitType.TEMPERATURE:
					break;
				case UnitType.SUBSTANCE_AMOUNT:
					break;
				case UnitType.LUMINOSITY:
					break;
				case UnitType.AREA:
					cachedMap[UnitType.LENGTH] = 2;
					break;
				case UnitType.VOLUME:
					cachedMap[UnitType.LENGTH] = 3;
					break;
				case UnitType.DENSITY:
					cachedMap[UnitType.MASS] = 1;
					cachedMap[UnitType.LENGTH] = -3;
					break;
				case UnitType.VELOCITY:
					cachedMap[UnitType.LENGTH] = 1;
					cachedMap[UnitType.TIME] = -1;
					break;
				case UnitType.VOLUMETRIC_FLOW:
					cachedMap[UnitType.LENGTH] = 3;
					cachedMap[UnitType.TIME] = -1;
					break;
				case UnitType.MASS_FLOW:
					cachedMap[UnitType.MASS] = 1;
					cachedMap[UnitType.TIME] = -1;
					break;
				case UnitType.FREQUENCY:
					cachedMap[UnitType.TIME] = -1;
					break;
				case UnitType.ACCELERATION:
					cachedMap[UnitType.LENGTH] = 1;
					cachedMap[UnitType.TIME] = -2;
					break;
				case UnitType.FORCE:
					cachedMap[UnitType.MASS] = 1;
					cachedMap[UnitType.LENGTH] = 1;
					cachedMap[UnitType.TIME] = -2;
					break;
				case UnitType.PRESSURE:
					cachedMap[UnitType.MASS] = 1;
					cachedMap[UnitType.LENGTH] = -1;
					cachedMap[UnitType.TIME] = -2;
					break;
				case UnitType.ENERGY:
					cachedMap[UnitType.MASS] = 1;
					cachedMap[UnitType.LENGTH] = 2;
					cachedMap[UnitType.TIME] = -2;
					break;
				case UnitType.POWER:
					cachedMap[UnitType.MASS] = 1;
					cachedMap[UnitType.LENGTH] = 2;
					cachedMap[UnitType.TIME] = -3;
					break;
				case UnitType.ELECTRIC_CHARGE:
					cachedMap[UnitType.ELECTRIC_CURRENT] = 1;
					cachedMap[UnitType.TIME] = 1;
					break;
				case UnitType.ELECTROMOTIVE_FORCE:
					cachedMap[UnitType.LENGTH] = 2;
					cachedMap[UnitType.MASS] = 1;
					cachedMap[UnitType.ELECTRIC_CURRENT] = -1;
					cachedMap[UnitType.TIME] = -3;
					break;
				case UnitType.ELECTRIC_RESISTANCE:
					cachedMap[UnitType.MASS] = 1;
					cachedMap[UnitType.LENGTH] = -3;
					cachedMap[UnitType.ELECTRIC_CURRENT] = 2;
					cachedMap[UnitType.TIME] = 4;
					break;
				case UnitType.ELECTRIC_CAPACITANCE:
					cachedMap[UnitType.MASS] = -1;
					cachedMap[UnitType.LENGTH] = 2;
					cachedMap[UnitType.ELECTRIC_CURRENT] = -2;
					cachedMap[UnitType.TIME] = -3;
					break;
				case UnitType.ELECTRIC_PERMITTIVITY:
					cachedMap[UnitType.MASS] = -1;
					cachedMap[UnitType.LENGTH] = -3;
					cachedMap[UnitType.ELECTRIC_CURRENT] = 2;
					cachedMap[UnitType.TIME] = 4;
					break;
				case UnitType.ELECTRIC_FIELD_STRENGTH:
					cachedMap[UnitType.ELECTRIC_CURRENT] = 1;
					cachedMap[UnitType.LENGTH] = -1;
					break;
				case UnitType.MAGNETIC_FLUX:
					cachedMap[UnitType.MASS] = 1;
					cachedMap[UnitType.LENGTH] = 2;
					cachedMap[UnitType.ELECTRIC_CURRENT] = -1;
					cachedMap[UnitType.TIME] = -2;
					break;
				case UnitType.MAGNETIC_FLUX_DENSITY:
					cachedMap[UnitType.MASS] = 1;
					cachedMap[UnitType.ELECTRIC_CURRENT] = -1;
					cachedMap[UnitType.TIME] = -2;
					break;
				case UnitType.ELECTRIC_INDUCTANCE:
					cachedMap[UnitType.MASS] = 1;
					cachedMap[UnitType.LENGTH] = 2;
					cachedMap[UnitType.ELECTRIC_CURRENT] = -2;
					cachedMap[UnitType.TIME] = -2;
					break;
				case UnitType.ELECTRIC_CONDUCTANCE:
					cachedMap[UnitType.MASS] = -1;
					cachedMap[UnitType.LENGTH] = -2;
					cachedMap[UnitType.ELECTRIC_CURRENT] = 2;
					cachedMap[UnitType.TIME] = 3;
					break;
				case UnitType.LUMINOUS_FLUX:
					cachedMap[UnitType.LUMINOSITY] = 1;
					break;
				case UnitType.ILLUMINANCE:
					cachedMap[UnitType.LUMINOSITY] = 1;
					cachedMap[UnitType.LENGTH] = -2;
					break;
				case UnitType.RADIATION_DOSE_ABSORBED:
					cachedMap[UnitType.LENGTH] = 2;
					cachedMap[UnitType.TIME] = -2;
					break;
				case UnitType.RADIATION_DOSE_EFFECTIVE:
					cachedMap[UnitType.LENGTH] = 2;
					cachedMap[UnitType.TIME] = -2;
					break;
				case UnitType.RADIATION_DOSE_RATE:
					cachedMap[UnitType.LENGTH] = 2;
					cachedMap[UnitType.TIME] = -3;
					break;
				case UnitType.RADIOACTIVITY:
					cachedMap[UnitType.TIME] = -1;
					break;
				case UnitType.CATALYTIC_ACTIVITY:
					cachedMap[UnitType.SUBSTANCE_AMOUNT] = 1;
					cachedMap[UnitType.TIME] = -1;
					break;
				case UnitType.DYNAMIC_VISCOSITY:
					cachedMap[UnitType.MASS] = 1;
					cachedMap[UnitType.LENGTH] = 1;
					cachedMap[UnitType.TIME] = -1;
					break;
				case UnitType.KINEMATIC_VISCOSITY:
					cachedMap[UnitType.LENGTH] = 2;
					cachedMap[UnitType.TIME] = -1;
					break;
				case UnitType.RECIPROCAL_LENGTH:
					cachedMap[UnitType.LENGTH] = -1;
					break;
				case UnitType.PLANE_ANGLE:
					break;
				case UnitType.SOLID_ANGLE:
					break;
				case UnitType.INTENSITY:
					break;
				case UnitType.COMPUTER_SCIENCE:
					break;
				case UnitType.TIME_SQUARED:
					cachedMap[UnitType.TIME] = 2;
					break;
				case UnitType.MOLAR_CONCENTRATION:
					cachedMap[UnitType.SUBSTANCE_AMOUNT] = 1;
					cachedMap[UnitType.LENGTH] = -3;
					break;
				case UnitType.IRRADIANCE:
					cachedMap[UnitType.MASS] = 1;
					cachedMap[UnitType.TIME] = -3;
					break;
				case UnitType.CURRENCY:
					break;
				case UnitType.UNCLASSIFIED:
					break;
				default:
					break;
			}
			return cachedMap;
		}
	} // end MeasurementSystem
} // end namespace
