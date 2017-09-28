using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.Collections.Concurrent;

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
 * A MeasurementSystem is a collection of units of measure that have a linear
 * relationship to each other: y = ax + b where x is the unit to be converted, y
 * is the converted unit, a is the scaling factor and b is the offset. <br>
 * See
 * <ul>
 * <li>Wikipedia: <i><a href=
 * "https://en.wikipedia.org/wiki/International_System_of_Units">International
 * System of Units</a></i></li>
 * <li>Table of conversions:
 * <i><a href="https://en.wikipedia.org/wiki/Conversion_of_units">Conversion of
 * Units</a></i></li>
 * <li>Unified Code for Units of Measure :
 * <i><a href="http://unitsofmeasure.org/trac">UCUM</a></i></li>
 * <li>SI derived units:
 * <i><a href="https://en.wikipedia.org/wiki/SI_derived_unit">SI Derived
 * Units</a></i></li>
 * <li>US system:
 * <i><a href="https://en.wikipedia.org/wiki/United_States_customary_units">US
 * Units</a></i></li>
 * <li>British Imperial system:
 * <i><a href="https://en.wikipedia.org/wiki/Imperial_units">British Imperial
 * Units</a></i></li>
 * <li>JSR 363: <i><a href=
 * "https://java.net/downloads/unitsofmeasurement/JSR363Specification_EDR.pdf">JSR
 * 363 Specification</a></i></li>
 * </ul>
 * <br>
 * The MeasurementSystem class creates:
 * <ul>
 * <li>7 SI fundamental units of measure</li>
 * <li>20 SI units derived from these fundamental units</li>
 * <li>other units in the International Customary, US and British Imperial
 * systems</li>
 * <li>any number of custom units of measure</li>
 * </ul>
 *
 */
	public class MeasurementSystem
	{
		// name of resource bundle with translatable strings for exception messages
		private static readonly string MESSAGE_RESOURCE_NAME = "Message";

		// name of resource set with translatable strings for UOMs (e.g. time)
		private static readonly string UNIT_RESOURCE_NAME = "Unit";

		// resource manager for exception messages
		private static ResourceManager MessagesManager;

		// resource manager for units
		private static ResourceManager UnitsManager;

		// standard unified system
		private static MeasurementSystem UnifiedSystem = new MeasurementSystem();

		// registry by unit symbol
		private ConcurrentDictionary<string, UnitOfMeasure> SymbolRegistry = new ConcurrentDictionary<string, UnitOfMeasure>();

		// registry by base symbol
		private ConcurrentDictionary<string, UnitOfMeasure> BaseRegistry = new ConcurrentDictionary<string, UnitOfMeasure>();

		// registry for units by enumeration
		private ConcurrentDictionary<Unit, UnitOfMeasure> UnitRegistry = new ConcurrentDictionary<Unit, UnitOfMeasure>();

		private MeasurementSystem()
		{
			// common unit strings
			MessagesManager = new ResourceManager(MESSAGE_RESOURCE_NAME,
										 typeof(MeasurementSystem).Assembly);

			UnitsManager = new ResourceManager(UNIT_RESOURCE_NAME,
							 typeof(MeasurementSystem).Assembly);
		}

		// get a particular message by its key
		public static string GetMessage(string key)
		{
			return MessagesManager.GetString(key);
		}

		// get a particular unit string by its key
		public static string GetUnitString(string key)
		{
			return UnitsManager.GetString(key);
		}

		/**
 * Get the unified system of units of measure for International Customary,
 * SI, US, British Imperial as well as custom systems
 * 
 * @return {@link MeasurementSystem}
 */
		public static MeasurementSystem GetSystem()
		{
			return UnifiedSystem;
		}

		/**
 * Get the unit of measure for unity 'one'
 * 
 * @return {@link UnitOfMeasure}
 * @
 *             Exception
 */
		public UnitOfMeasure GetOne()
		{
			return GetUOM(Unit.ONE);
		}

		/**
 * Get the fundamental unit of measure of time
 * 
 * @return {@link UnitOfMeasure}
 * @
 *             Exception Exception
 */
		public UnitOfMeasure GetSecond()
		{
			return GetUOM(Unit.SECOND);
		}

		/**
		 * Get the unit of measure for a minute (60 seconds)
		 * 
		 * @return {@link UnitOfMeasure}
		 * @
		 *             Exception
		 */
		public UnitOfMeasure GetMinute()
		{
			return GetUOM(Unit.MINUTE);
		}

		/**
		 * Get the unit of measure for an hour (60 minutes)
		 * 
		 * @return {@link UnitOfMeasure}
		 * @
		 *             Exception
		 */
		public UnitOfMeasure GetHour()
		{
			return GetUOM(Unit.HOUR);
		}

		/**
		 * Get the unit of measure for one day (24 hours)
		 * 
		 * @return {@link UnitOfMeasure}
		 * @
		 *             Exception
		 */
		public UnitOfMeasure GetDay()
		{
			return GetUOM(Unit.DAY);
		}

		/**
	* Get the unit of measure with this unique enumerated type
	* 
	* @param unit
	*            {@link Unit}
	* @return {@link UnitOfMeasure}
	* @
	*             Exception Exception
*/
		public UnitOfMeasure GetUOM(Unit unit)
		{
			UnitOfMeasure uom = UnitRegistry[unit];

			if (uom == null)
			{
				uom = CreateUOM(unit);
			}
			return uom;
		}

		/**
		 * Get the unit of measure with this unique symbol
		 * 
		 * @param symbol
		 *            Symbol
		 * @return {@link UnitOfMeasure}
		 */
		public UnitOfMeasure GetUOM(string symbol)
		{
			return SymbolRegistry[symbol];
		}

		/**
 * Get the unit of measure with this base symbol
 * 
 * @param symbol
 *            Base symbol
 * @return {@link UnitOfMeasure}
 */
		public UnitOfMeasure GetBaseUOM(string symbol)
		{
			return BaseRegistry[symbol];
		}

		/**
 * Create or fetch a unit of measure linearly scaled by the {@link Prefix}
 * against the target unit of measure.
 * 
 * @param prefix
 *            {@link Prefix} Scaling prefix with the scaling factor, e.g.
 *            1000
 * @param tarGetUOM
 *            abscissa {@link UnitOfMeasure}
 * @return {@link UnitOfMeasure}
 * @
 *             Exception
 */
		public UnitOfMeasure GetUOM(Prefix prefix, UnitOfMeasure tarGetUOM)
		{
			string symbol = prefix.Symbol + tarGetUOM.Symbol;

			UnitOfMeasure scaled = GetUOM(symbol);

			// if not found, create it
			if (scaled == null)
			{
				// generate a name and description
				string name = prefix.Name + tarGetUOM.Name;
				string description = prefix.Factor + " " + tarGetUOM.Name;

				// scaling factor
				decimal scalingFactor = UnitOfMeasure.DecimalMultiply(tarGetUOM.ScalingFactor,
						prefix.Factor);

				// create the unit of measure and set conversion
				scaled = CreateScalarUOM(tarGetUOM.UOMType, null, name, symbol, description);
				scaled.SetConversion(scalingFactor, tarGetUOM.AbscissaUnit);
			}
			return scaled;
		}

		/**
		* Create or fetch a unit of measure linearly scaled by the {@link Prefix}
		* against the target unit of measure.
		* 
		* @param prefix
		*            {@link Prefix} Scaling prefix with the scaling factor, e.g.
		*            1000
		* @param unit
		*            {@link Unit}
		* @return {@link UnitOfMeasure}
		* @
		*             Exception
*/
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
					uom = CreateScalarUOM(UnitType.UNITY, Unit.ONE, UnitsManager.GetString("one.name"),
							UnitsManager.GetString("one.symbol"), UnitsManager.GetString("one.desc"));
					break;

				case Unit.PERCENT:
					uom = CreateScalarUOM(UnitType.UNITY, Unit.PERCENT, UnitsManager.GetString("percent.name"),
							UnitsManager.GetString("percent.symbol"), UnitsManager.GetString("percent.desc"));
					uom.SetConversion(Quantity.CreateAmount("0.01"), GetOne());
					break;

				case Unit.SECOND:
					// second
					uom = CreateScalarUOM(UnitType.TIME, Unit.SECOND, UnitsManager.GetString("sec.name"),
							UnitsManager.GetString("sec.symbol"), UnitsManager.GetString("sec.desc"));
					break;

				case Unit.MINUTE:
					// minute
					uom = CreateScalarUOM(UnitType.TIME, Unit.MINUTE, UnitsManager.GetString("min.name"),
							UnitsManager.GetString("min.symbol"), UnitsManager.GetString("min.desc"));
					uom.SetConversion(Quantity.CreateAmount("60"), GetUOM(Unit.SECOND));
					break;

				case Unit.HOUR:
					// hour
					uom = CreateScalarUOM(UnitType.TIME, Unit.HOUR, UnitsManager.GetString("hr.name"),
							UnitsManager.GetString("hr.symbol"), UnitsManager.GetString("hr.desc"));
					uom.SetConversion(Quantity.CreateAmount("3600"), GetUOM(Unit.SECOND));
					break;

				case Unit.DAY:
					// day
					uom = CreateScalarUOM(UnitType.TIME, Unit.DAY, UnitsManager.GetString("day.name"),
							UnitsManager.GetString("day.symbol"), UnitsManager.GetString("day.desc"));
					uom.SetConversion(Quantity.CreateAmount("86400"), GetUOM(Unit.SECOND));
					break;

				case Unit.WEEK:
					// week
					uom = CreateScalarUOM(UnitType.TIME, Unit.WEEK, UnitsManager.GetString("week.name"),
							UnitsManager.GetString("week.symbol"), UnitsManager.GetString("week.desc"));
					uom.SetConversion(Quantity.CreateAmount("604800"), GetUOM(Unit.SECOND));
					break;

				case Unit.JULIAN_YEAR:
					// Julian year
					uom = CreateScalarUOM(UnitType.TIME, Unit.JULIAN_YEAR, UnitsManager.GetString("jyear.name"),
							UnitsManager.GetString("jyear.symbol"), UnitsManager.GetString("jyear.desc"));
					uom.SetConversion(Quantity.CreateAmount("3.1557600E+07"), GetUOM(Unit.SECOND));

					break;

				case Unit.SQUARE_SECOND:
					// square second
					uom = CreatePowerUOM(UnitType.TIME_SQUARED, Unit.SQUARE_SECOND, UnitsManager.GetString("s2.name"),
							UnitsManager.GetString("s2.symbol"), UnitsManager.GetString("s2.desc"), GetUOM(Unit.SECOND), 2);
					break;

				case Unit.MOLE:
					// substance amount
					uom = CreateScalarUOM(UnitType.SUBSTANCE_AMOUNT, Unit.MOLE, UnitsManager.GetString("mole.name"),
							UnitsManager.GetString("mole.symbol"), UnitsManager.GetString("mole.desc"));
					break;

				case Unit.EQUIVALENT:
					// substance amount
					uom = CreateScalarUOM(UnitType.SUBSTANCE_AMOUNT, Unit.EQUIVALENT, UnitsManager.GetString("equivalent.name"),
							UnitsManager.GetString("equivalent.symbol"), UnitsManager.GetString("equivalent.desc"));
					break;

				case Unit.DECIBEL:
					// decibel
					uom = CreateScalarUOM(UnitType.INTENSITY, Unit.DECIBEL, UnitsManager.GetString("db.name"),
							UnitsManager.GetString("db.symbol"), UnitsManager.GetString("db.desc"));
					break;

				case Unit.RADIAN:
					// plane angle radian (rad)
					uom = CreateScalarUOM(UnitType.PLANE_ANGLE, Unit.RADIAN, UnitsManager.GetString("radian.name"),
							UnitsManager.GetString("radian.symbol"), UnitsManager.GetString("radian.desc"));
					uom.SetConversion(GetOne());
					break;

				case Unit.STERADIAN:
					// solid angle steradian (sr)
					uom = CreateScalarUOM(UnitType.SOLID_ANGLE, Unit.STERADIAN, UnitsManager.GetString("steradian.name"),
							UnitsManager.GetString("steradian.symbol"), UnitsManager.GetString("steradian.desc"));
					uom.SetConversion(GetOne());
					break;

				case Unit.DEGREE:
					// degree of arc
					uom = CreateScalarUOM(UnitType.PLANE_ANGLE, Unit.DEGREE, UnitsManager.GetString("degree.name"),
							UnitsManager.GetString("degree.symbol"), UnitsManager.GetString("degree.desc"));
					uom.SetConversion(UnitOfMeasure.DecimalDivide(new decimal(Math.PI), 180), GetUOM(Unit.RADIAN));
					break;

				case Unit.ARC_SECOND:
					// degree of arc
					uom = CreateScalarUOM(UnitType.PLANE_ANGLE, Unit.ARC_SECOND, UnitsManager.GetString("arcsec.name"),
							UnitsManager.GetString("arcsec.symbol"), UnitsManager.GetString("arcsec.desc"));
					uom.SetConversion(UnitOfMeasure.DecimalDivide(new decimal(Math.PI), 648000), GetUOM(Unit.RADIAN));
					break;

				case Unit.METRE:
					// fundamental length
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.METRE, UnitsManager.GetString("m.name"),
							UnitsManager.GetString("m.symbol"), UnitsManager.GetString("m.desc"));
					break;

				case Unit.DIOPTER:
					// per metre
					uom = CreateQuotientUOM(UnitType.RECIPROCAL_LENGTH, Unit.DIOPTER, UnitsManager.GetString("diopter.name"),
							UnitsManager.GetString("diopter.symbol"), UnitsManager.GetString("diopter.desc"), GetOne(),
							GetUOM(Unit.METRE));
					break;

				case Unit.KILOGRAM:
					// fundamental mass
					uom = CreateScalarUOM(UnitType.MASS, Unit.KILOGRAM, UnitsManager.GetString("kg.name"),
							UnitsManager.GetString("kg.symbol"), UnitsManager.GetString("kg.desc"));
					break;

				case Unit.TONNE:
					// mass
					uom = CreateScalarUOM(UnitType.MASS, Unit.TONNE, UnitsManager.GetString("tonne.name"),
							UnitsManager.GetString("tonne.symbol"), UnitsManager.GetString("tonne.desc"));
					uom.SetConversion(Prefix.KILO.Factor, GetUOM(Unit.KILOGRAM));
					break;

				case Unit.KELVIN:
					// fundamental temperature
					uom = CreateScalarUOM(UnitType.TEMPERATURE, Unit.KELVIN, UnitsManager.GetString("kelvin.name"),
							UnitsManager.GetString("kelvin.symbol"), UnitsManager.GetString("kelvin.desc"));
					break;

				case Unit.AMPERE:
					// electric current
					uom = CreateScalarUOM(UnitType.ELECTRIC_CURRENT, Unit.AMPERE, UnitsManager.GetString("amp.name"),
							UnitsManager.GetString("amp.symbol"), UnitsManager.GetString("amp.desc"));
					break;

				case Unit.CANDELA:
					// luminosity
					uom = CreateScalarUOM(UnitType.LUMINOSITY, Unit.CANDELA, UnitsManager.GetString("cd.name"),
							UnitsManager.GetString("cd.symbol"), UnitsManager.GetString("cd.desc"));
					break;

				case Unit.PH:
					// molar concentration
					uom = CreateScalarUOM(UnitType.MOLAR_CONCENTRATION, Unit.PH, UnitsManager.GetString("ph.name"),
							UnitsManager.GetString("ph.symbol"), UnitsManager.GetString("ph.desc"));
					break;

				case Unit.GRAM: // gram
					uom = CreateScalarUOM(UnitType.MASS, Unit.GRAM, UnitsManager.GetString("gram.name"),
							UnitsManager.GetString("gram.symbol"), UnitsManager.GetString("gram.desc"));
					uom.SetConversion(Prefix.MILLI.Factor, GetUOM(Unit.KILOGRAM));
					break;

				case Unit.CARAT:
					// carat
					uom = CreateScalarUOM(UnitType.MASS, Unit.CARAT, UnitsManager.GetString("carat.name"),
							UnitsManager.GetString("carat.symbol"), UnitsManager.GetString("carat.desc"));
					uom.SetConversion(Quantity.CreateAmount("0.2"), GetUOM(Unit.GRAM));
					break;

				case Unit.SQUARE_METRE:
					// square metre
					uom = CreatePowerUOM(UnitType.AREA, Unit.SQUARE_METRE, UnitsManager.GetString("m2.name"),
							UnitsManager.GetString("m2.symbol"), UnitsManager.GetString("m2.desc"), GetUOM(Unit.METRE), 2);
					break;

				case Unit.HECTARE:
					// hectare
					uom = CreateScalarUOM(UnitType.AREA, Unit.HECTARE, UnitsManager.GetString("hectare.name"),
							UnitsManager.GetString("hectare.symbol"), UnitsManager.GetString("hectare.desc"));
					uom.SetConversion(Quantity.CreateAmount("10000"), GetUOM(Unit.SQUARE_METRE));
					break;

				case Unit.METRE_PER_SEC:
					// velocity
					uom = CreateQuotientUOM(UnitType.VELOCITY, Unit.METRE_PER_SEC, UnitsManager.GetString("mps.name"),
							UnitsManager.GetString("mps.symbol"), UnitsManager.GetString("mps.desc"), GetUOM(Unit.METRE), GetSecond());
					break;

				case Unit.METRE_PER_SEC_SQUARED:
					// acceleration
					uom = CreateQuotientUOM(UnitType.ACCELERATION, Unit.METRE_PER_SEC_SQUARED, UnitsManager.GetString("mps2.name"),
							UnitsManager.GetString("mps2.symbol"), UnitsManager.GetString("mps2.desc"), GetUOM(Unit.METRE),
							GetUOM(Unit.SQUARE_SECOND));
					break;

				case Unit.CUBIC_METRE:
					// cubic metre
					uom = CreatePowerUOM(UnitType.VOLUME, Unit.CUBIC_METRE, UnitsManager.GetString("m3.name"),
							UnitsManager.GetString("m3.symbol"), UnitsManager.GetString("m3.desc"), GetUOM(Unit.METRE), 3);
					break;

				case Unit.LITRE:
					// litre
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.LITRE, UnitsManager.GetString("litre.name"),
							UnitsManager.GetString("litre.symbol"), UnitsManager.GetString("litre.desc"));
					uom.SetConversion(Prefix.MILLI.Factor, GetUOM(Unit.CUBIC_METRE));
					break;

				case Unit.CUBIC_METRE_PER_SEC:
					// flow (volume)
					uom = CreateQuotientUOM(UnitType.VOLUMETRIC_FLOW, Unit.CUBIC_METRE_PER_SEC,
							UnitsManager.GetString("m3PerSec.name"), UnitsManager.GetString("m3PerSec.symbol"),
							UnitsManager.GetString("m3PerSec.desc"), GetUOM(Unit.CUBIC_METRE), GetSecond());
					break;

				case Unit.KILOGRAM_PER_SEC:
					// flow (mass)
					uom = CreateQuotientUOM(UnitType.MASS_FLOW, Unit.KILOGRAM_PER_SEC, UnitsManager.GetString("kgPerSec.name"),
							UnitsManager.GetString("kgPerSec.symbol"), UnitsManager.GetString("kgPerSec.desc"), GetUOM(Unit.KILOGRAM),
							GetSecond());
					break;

				case Unit.KILOGRAM_PER_CU_METRE:
					// kg/m^3
					uom = CreateQuotientUOM(UnitType.DENSITY, Unit.KILOGRAM_PER_CU_METRE, UnitsManager.GetString("kg_m3.name"),
							UnitsManager.GetString("kg_m3.symbol"), UnitsManager.GetString("kg_m3.desc"), GetUOM(Unit.KILOGRAM),
							GetUOM(Unit.CUBIC_METRE));
					break;

				case Unit.PASCAL_SECOND:
					// dynamic viscosity
					uom = CreateProductUOM(UnitType.DYNAMIC_VISCOSITY, Unit.PASCAL_SECOND, UnitsManager.GetString("pascal_sec.name"),
							UnitsManager.GetString("pascal_sec.symbol"), UnitsManager.GetString("pascal_sec.desc"), GetUOM(Unit.PASCAL),
							GetSecond());
					break;

				case Unit.SQUARE_METRE_PER_SEC:
					// kinematic viscosity
					uom = CreateQuotientUOM(UnitType.KINEMATIC_VISCOSITY, Unit.SQUARE_METRE_PER_SEC,
							UnitsManager.GetString("m2PerSec.name"), UnitsManager.GetString("m2PerSec.symbol"),
							UnitsManager.GetString("m2PerSec.desc"), GetUOM(Unit.SQUARE_METRE), GetSecond());
					break;

				case Unit.CALORIE:
					// thermodynamic calorie
					uom = CreateScalarUOM(UnitType.ENERGY, Unit.CALORIE, UnitsManager.GetString("calorie.name"),
							UnitsManager.GetString("calorie.symbol"), UnitsManager.GetString("calorie.desc"));
					uom.SetConversion(Quantity.CreateAmount("4.184"), GetUOM(Unit.JOULE));
					break;

				case Unit.NEWTON:
					// force F = m·A (newton)
					uom = CreateProductUOM(UnitType.FORCE, Unit.NEWTON, UnitsManager.GetString("newton.name"),
							UnitsManager.GetString("newton.symbol"), UnitsManager.GetString("newton.desc"), GetUOM(Unit.KILOGRAM),
							GetUOM(Unit.METRE_PER_SEC_SQUARED));
					break;

				case Unit.NEWTON_METRE:
					// newton-metre
					uom = CreateProductUOM(UnitType.ENERGY, Unit.NEWTON_METRE, UnitsManager.GetString("n_m.name"),
							UnitsManager.GetString("n_m.symbol"), UnitsManager.GetString("n_m.desc"), GetUOM(Unit.NEWTON),
							GetUOM(Unit.METRE));
					break;

				case Unit.JOULE:
					// energy (joule)
					uom = CreateProductUOM(UnitType.ENERGY, Unit.JOULE, UnitsManager.GetString("joule.name"),
							UnitsManager.GetString("joule.symbol"), UnitsManager.GetString("joule.desc"), GetUOM(Unit.NEWTON),
							GetUOM(Unit.METRE));
					break;

				case Unit.ELECTRON_VOLT:
					// ev
					Quantity e = this.GetQuantity(Constant.ELEMENTARY_CHARGE);
					uom = CreateProductUOM(UnitType.ENERGY, Unit.ELECTRON_VOLT, UnitsManager.GetString("ev.name"),
								UnitsManager.GetString("ev.symbol"), UnitsManager.GetString("ev.desc"), e.UOM, GetUOM(Unit.VOLT));
					uom.ScalingFactor = e.Amount;
					break;

				case Unit.WATT_HOUR:
					// watt-hour
					uom = CreateProductUOM(UnitType.ENERGY, Unit.WATT_HOUR, UnitsManager.GetString("wh.name"),
							UnitsManager.GetString("wh.symbol"), UnitsManager.GetString("wh.desc"), GetUOM(Unit.WATT), GetHour());
					break;

				case Unit.WATT:
					// power (watt)
					uom = CreateQuotientUOM(UnitType.POWER, Unit.WATT, UnitsManager.GetString("watt.name"),
							UnitsManager.GetString("watt.symbol"), UnitsManager.GetString("watt.desc"), GetUOM(Unit.JOULE), GetSecond());
					break;

				case Unit.HERTZ:
					// frequency (hertz)
					uom = CreateQuotientUOM(UnitType.FREQUENCY, Unit.HERTZ, UnitsManager.GetString("hertz.name"),
							UnitsManager.GetString("hertz.symbol"), UnitsManager.GetString("hertz.desc"), GetOne(), GetSecond());
					break;

				case Unit.RAD_PER_SEC:
					// angular frequency
					uom = CreateQuotientUOM(UnitType.FREQUENCY, Unit.RAD_PER_SEC, UnitsManager.GetString("radpers.name"),
							UnitsManager.GetString("radpers.symbol"), UnitsManager.GetString("radpers.desc"), GetUOM(Unit.RADIAN),
							GetSecond());
					decimal twoPi = decimal.Multiply(2, new decimal(Math.PI));
					uom.SetConversion(decimal.Divide(decimal.One, twoPi), GetUOM(Unit.HERTZ));
					break;

				case Unit.PASCAL:
					// pressure
					uom = CreateQuotientUOM(UnitType.PRESSURE, Unit.PASCAL, UnitsManager.GetString("pascal.name"),
							UnitsManager.GetString("pascal.symbol"), UnitsManager.GetString("pascal.desc"), GetUOM(Unit.NEWTON),
							GetUOM(Unit.SQUARE_METRE));
					break;

				case Unit.ATMOSPHERE:
					// pressure
					uom = CreateScalarUOM(UnitType.PRESSURE, Unit.ATMOSPHERE, UnitsManager.GetString("atm.name"),
							UnitsManager.GetString("atm.symbol"), UnitsManager.GetString("atm.desc"));
					uom.SetConversion(Quantity.CreateAmount("101325"), GetUOM(Unit.PASCAL));
					break;

				case Unit.BAR:
					// pressure
					uom = CreateScalarUOM(UnitType.PRESSURE, Unit.BAR, UnitsManager.GetString("bar.name"),
							UnitsManager.GetString("bar.symbol"), UnitsManager.GetString("bar.desc"));
					uom.SetConversion(decimal.One, GetUOM(Unit.PASCAL), Quantity.CreateAmount("1.0E+05"));
					break;

				case Unit.COULOMB:
					// charge (coulomb)
					uom = CreateProductUOM(UnitType.ELECTRIC_CHARGE, Unit.COULOMB, UnitsManager.GetString("coulomb.name"),
							UnitsManager.GetString("coulomb.symbol"), UnitsManager.GetString("coulomb.desc"), GetUOM(Unit.AMPERE),
							GetSecond());
					break;

				case Unit.VOLT:
					// voltage (volt)
					uom = CreateQuotientUOM(UnitType.ELECTROMOTIVE_FORCE, Unit.VOLT, UnitsManager.GetString("volt.name"),
							UnitsManager.GetString("volt.symbol"), UnitsManager.GetString("volt.desc"), GetUOM(Unit.WATT),
							GetUOM(Unit.AMPERE));
					break;

				case Unit.OHM:
					// resistance (ohm)
					uom = CreateQuotientUOM(UnitType.ELECTRIC_RESISTANCE, Unit.OHM, UnitsManager.GetString("ohm.name"),
							UnitsManager.GetString("ohm.symbol"), UnitsManager.GetString("ohm.desc"), GetUOM(Unit.VOLT),
							GetUOM(Unit.AMPERE));
					break;

				case Unit.FARAD:
					// capacitance (farad)
					uom = CreateQuotientUOM(UnitType.ELECTRIC_CAPACITANCE, Unit.FARAD, UnitsManager.GetString("farad.name"),
							UnitsManager.GetString("farad.symbol"), UnitsManager.GetString("farad.desc"), GetUOM(Unit.COULOMB),
							GetUOM(Unit.VOLT));
					break;

				case Unit.FARAD_PER_METRE:
					// electric permittivity (farad/metre)
					uom = CreateQuotientUOM(UnitType.ELECTRIC_PERMITTIVITY, Unit.FARAD_PER_METRE,
							UnitsManager.GetString("fperm.name"), UnitsManager.GetString("fperm.symbol"), UnitsManager.GetString("fperm.desc"),
							GetUOM(Unit.FARAD), GetUOM(Unit.METRE));
					break;

				case Unit.AMPERE_PER_METRE:
					// electric field strength(ampere/metre)
					uom = CreateQuotientUOM(UnitType.ELECTRIC_FIELD_STRENGTH, Unit.AMPERE_PER_METRE,
							UnitsManager.GetString("aperm.name"), UnitsManager.GetString("aperm.symbol"), UnitsManager.GetString("aperm.desc"),
							GetUOM(Unit.AMPERE), GetUOM(Unit.METRE));
					break;

				case Unit.WEBER:
					// magnetic flux (weber)
					uom = CreateProductUOM(UnitType.MAGNETIC_FLUX, Unit.WEBER, UnitsManager.GetString("weber.name"),
							UnitsManager.GetString("weber.symbol"), UnitsManager.GetString("weber.desc"), GetUOM(Unit.VOLT), GetSecond());
					break;

				case Unit.TESLA:
					// magnetic flux density (tesla)
					uom = CreateQuotientUOM(UnitType.MAGNETIC_FLUX_DENSITY, Unit.TESLA, UnitsManager.GetString("tesla.name"),
							UnitsManager.GetString("tesla.symbol"), UnitsManager.GetString("tesla.desc"), GetUOM(Unit.WEBER),
							GetUOM(Unit.SQUARE_METRE));
					break;

				case Unit.HENRY:
					// inductance (henry)
					uom = CreateQuotientUOM(UnitType.ELECTRIC_INDUCTANCE, Unit.HENRY, UnitsManager.GetString("henry.name"),
							UnitsManager.GetString("henry.symbol"), UnitsManager.GetString("henry.desc"), GetUOM(Unit.WEBER),
							GetUOM(Unit.AMPERE));
					break;

				case Unit.SIEMENS:
					// electrical conductance (siemens)
					uom = CreateQuotientUOM(UnitType.ELECTRIC_CONDUCTANCE, Unit.SIEMENS, UnitsManager.GetString("siemens.name"),
							UnitsManager.GetString("siemens.symbol"), UnitsManager.GetString("siemens.desc"), GetUOM(Unit.AMPERE),
							GetUOM(Unit.VOLT));
					break;

				case Unit.CELSIUS:
					// °C = °K - 273.15
					uom = CreateScalarUOM(UnitType.TEMPERATURE, Unit.CELSIUS, UnitsManager.GetString("celsius.name"),
							UnitsManager.GetString("celsius.symbol"), UnitsManager.GetString("celsius.desc"));
					uom.SetConversion(decimal.One, GetUOM(Unit.KELVIN), Quantity.CreateAmount("273.15"));
					break;

				case Unit.LUMEN:
					// luminous flux (lumen)
					uom = CreateProductUOM(UnitType.LUMINOUS_FLUX, Unit.LUMEN, UnitsManager.GetString("lumen.name"),
							UnitsManager.GetString("lumen.symbol"), UnitsManager.GetString("lumen.desc"), GetUOM(Unit.CANDELA),
							GetUOM(Unit.STERADIAN));
					break;

				case Unit.LUX:
					// illuminance (lux)
					uom = CreateQuotientUOM(UnitType.ILLUMINANCE, Unit.LUX, UnitsManager.GetString("lux.name"),
							UnitsManager.GetString("lux.symbol"), UnitsManager.GetString("lux.desc"), GetUOM(Unit.LUMEN),
							GetUOM(Unit.SQUARE_METRE));
					break;

				case Unit.BECQUEREL:
					// radioactivity (becquerel). Same base symbol as Hertz
					uom = CreateScalarUOM(UnitType.RADIOACTIVITY, Unit.BECQUEREL, UnitsManager.GetString("becquerel.name"),
							UnitsManager.GetString("becquerel.symbol"), UnitsManager.GetString("becquerel.desc"));
					break;

				case Unit.GRAY:
					// gray (Gy)
					uom = CreateQuotientUOM(UnitType.RADIATION_DOSE_ABSORBED, Unit.GRAY, UnitsManager.GetString("gray.name"),
							UnitsManager.GetString("gray.symbol"), UnitsManager.GetString("gray.desc"), GetUOM(Unit.JOULE),
							GetUOM(Unit.KILOGRAM));
					break;

				case Unit.SIEVERT:
					// sievert (Sv)
					uom = CreateQuotientUOM(UnitType.RADIATION_DOSE_EFFECTIVE, Unit.SIEVERT, UnitsManager.GetString("sievert.name"),
							UnitsManager.GetString("sievert.symbol"), UnitsManager.GetString("sievert.desc"), GetUOM(Unit.JOULE),
							GetUOM(Unit.KILOGRAM));
					break;

				case Unit.SIEVERTS_PER_HOUR:
					uom = CreateQuotientUOM(UnitType.RADIATION_DOSE_RATE, Unit.SIEVERTS_PER_HOUR, UnitsManager.GetString("sph.name"),
							UnitsManager.GetString("sph.symbol"), UnitsManager.GetString("sph.desc"), GetUOM(Unit.SIEVERT), GetHour());
					break;

				case Unit.KATAL:
					// katal (kat)
					uom = CreateQuotientUOM(UnitType.CATALYTIC_ACTIVITY, Unit.KATAL, UnitsManager.GetString("katal.name"),
							UnitsManager.GetString("katal.symbol"), UnitsManager.GetString("katal.desc"), GetUOM(Unit.MOLE), GetSecond());
					break;

				case Unit.UNIT:
					// Unit (U)
					uom = CreateScalarUOM(UnitType.CATALYTIC_ACTIVITY, Unit.UNIT, UnitsManager.GetString("unit.name"),
							UnitsManager.GetString("unit.symbol"), UnitsManager.GetString("unit.desc"));
					uom.SetConversion(Quantity.DivideAmounts("1.0E-06", "60"), GetUOM(Unit.KATAL));
					break;

				case Unit.INTERNATIONAL_UNIT:
					uom = CreateScalarUOM(UnitType.SUBSTANCE_AMOUNT, Unit.INTERNATIONAL_UNIT, UnitsManager.GetString("iu.name"),
							UnitsManager.GetString("iu.symbol"), UnitsManager.GetString("iu.desc"));
					break;

				case Unit.ANGSTROM:
					// length
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.ANGSTROM, UnitsManager.GetString("angstrom.name"),
							UnitsManager.GetString("angstrom.symbol"), UnitsManager.GetString("angstrom.desc"));
					uom.SetConversion(Quantity.CreateAmount("0.1"), GetUOM(Prefix.NANO, GetUOM(Unit.METRE)));
					break;

				case Unit.BIT:
					// computer bit
					uom = CreateScalarUOM(UnitType.COMPUTER_SCIENCE, Unit.BIT, UnitsManager.GetString("bit.name"),
							UnitsManager.GetString("bit.symbol"), UnitsManager.GetString("bit.desc"));
					break;

				case Unit.BYTE:
					// computer byte
					uom = CreateScalarUOM(UnitType.COMPUTER_SCIENCE, Unit.BYTE, UnitsManager.GetString("byte.name"),
							UnitsManager.GetString("byte.symbol"), UnitsManager.GetString("byte.desc"));
					uom.SetConversion(Quantity.CreateAmount("8"), GetUOM(Unit.BIT));
					break;

				case Unit.WATTS_PER_SQ_METRE:
					uom = CreateQuotientUOM(UnitType.IRRADIANCE, Unit.WATTS_PER_SQ_METRE, UnitsManager.GetString("wsm.name"),
							UnitsManager.GetString("wsm.symbol"), UnitsManager.GetString("wsm.desc"), GetUOM(Unit.WATT),
							GetUOM(Unit.SQUARE_METRE));
					break;

				case Unit.PARSEC:
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.PARSEC, UnitsManager.GetString("parsec.name"),
							UnitsManager.GetString("parsec.symbol"), UnitsManager.GetString("parsec.desc"));
					uom.SetConversion(Quantity.CreateAmount("3.08567758149137E+16"), GetUOM(Unit.METRE));
					break;

				case Unit.ASTRONOMICAL_UNIT:
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.ASTRONOMICAL_UNIT, UnitsManager.GetString("au.name"),
							UnitsManager.GetString("au.symbol"), UnitsManager.GetString("au.desc"));
					uom.SetConversion(Quantity.CreateAmount("1.49597870700E+11"), GetUOM(Unit.METRE));
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
					uom = CreateScalarUOM(UnitType.TEMPERATURE, Unit.RANKINE, UnitsManager.GetString("rankine.name"),
							UnitsManager.GetString("rankine.symbol"), UnitsManager.GetString("rankine.desc"));

					// create bridge to SI
					uom.SetBridgeConversion(Quantity.DivideAmounts("5", "9"), GetUOM(Unit.KELVIN), null);
					break;

				case Unit.FAHRENHEIT:
					// Fahrenheit
					uom = CreateScalarUOM(UnitType.TEMPERATURE, Unit.FAHRENHEIT, UnitsManager.GetString("fahrenheit.name"),
							UnitsManager.GetString("fahrenheit.symbol"), UnitsManager.GetString("fahrenheit.desc"));
					uom.SetConversion(decimal.One, GetUOM(Unit.RANKINE), Quantity.CreateAmount("459.67"));
					break;

				case Unit.POUND_MASS:
					// lb mass (base)
					uom = CreateScalarUOM(UnitType.MASS, Unit.POUND_MASS, UnitsManager.GetString("lbm.name"),
							UnitsManager.GetString("lbm.symbol"), UnitsManager.GetString("lbm.desc"));

					// create bridge to SI
					uom.SetBridgeConversion(Quantity.CreateAmount("0.45359237"), GetUOM(Unit.KILOGRAM), null);
					break;

				case Unit.OUNCE:
					// ounce
					uom = CreateScalarUOM(UnitType.MASS, Unit.OUNCE, UnitsManager.GetString("ounce.name"),
							UnitsManager.GetString("ounce.symbol"), UnitsManager.GetString("ounce.desc"));
					uom.SetConversion(Quantity.CreateAmount("0.0625"), GetUOM(Unit.POUND_MASS));
					break;

				case Unit.TROY_OUNCE:
					// troy ounce
					uom = CreateScalarUOM(UnitType.MASS, Unit.TROY_OUNCE, UnitsManager.GetString("troy_oz.name"),
							UnitsManager.GetString("troy_oz.symbol"), UnitsManager.GetString("troy_oz.desc"));
					uom.SetConversion(Quantity.CreateAmount("31.1034768"), GetUOM(Unit.GRAM));
					break;

				case Unit.SLUG:
					// slug
					uom = CreateScalarUOM(UnitType.MASS, Unit.SLUG, UnitsManager.GetString("slug.name"),
							UnitsManager.GetString("slug.symbol"), UnitsManager.GetString("slug.desc"));
					Quantity g = GetQuantity(Constant.GRAVITY).Convert(GetUOM(Unit.FEET_PER_SEC_SQUARED));
					uom.SetConversion(g.Amount, GetUOM(Unit.POUND_MASS));
					break;

				case Unit.FOOT:
					// foot (foot is base conversion unit)
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.FOOT, UnitsManager.GetString("foot.name"),
							UnitsManager.GetString("foot.symbol"), UnitsManager.GetString("foot.desc"));

					// bridge to SI
					uom.SetBridgeConversion(Quantity.CreateAmount("0.3048"), GetUOM(Unit.METRE), null);
					break;

				case Unit.INCH:
					// inch
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.INCH, UnitsManager.GetString("inch.name"),
							UnitsManager.GetString("inch.symbol"), UnitsManager.GetString("inch.desc"));
					uom.SetConversion(Quantity.DivideAmounts("1", "12"), GetUOM(Unit.FOOT));
					break;

				case Unit.MIL:
					// inch
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.MIL, UnitsManager.GetString("mil.name"),
							UnitsManager.GetString("mil.symbol"), UnitsManager.GetString("mil.desc"));
					uom.SetConversion(Prefix.MILLI.Factor, GetUOM(Unit.INCH));
					break;

				case Unit.POINT:
					// point
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.POINT, UnitsManager.GetString("point.name"),
							UnitsManager.GetString("point.symbol"), UnitsManager.GetString("point.desc"));
					uom.SetConversion(Quantity.DivideAmounts("1", "72"), GetUOM(Unit.INCH));
					break;

				case Unit.YARD:
					// yard
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.YARD, UnitsManager.GetString("yard.name"),
							UnitsManager.GetString("yard.symbol"), UnitsManager.GetString("yard.desc"));
					uom.SetConversion(Quantity.CreateAmount("3"), GetUOM(Unit.FOOT));
					break;

				case Unit.MILE:
					// mile
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.MILE, UnitsManager.GetString("mile.name"),
							UnitsManager.GetString("mile.symbol"), UnitsManager.GetString("mile.desc"));
					uom.SetConversion(Quantity.CreateAmount("5280"), GetUOM(Unit.FOOT));
					break;

				case Unit.NAUTICAL_MILE:
					// nautical mile
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.NAUTICAL_MILE, UnitsManager.GetString("NM.name"),
							UnitsManager.GetString("NM.symbol"), UnitsManager.GetString("NM.desc"));
					uom.SetConversion(Quantity.CreateAmount("6080"), GetUOM(Unit.FOOT));
					break;

				case Unit.FATHOM:
					// fathom
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.FATHOM, UnitsManager.GetString("fth.name"),
							UnitsManager.GetString("fth.symbol"), UnitsManager.GetString("fth.desc"));
					uom.SetConversion(Quantity.CreateAmount("6"), GetUOM(Unit.FOOT));

					break;

				case Unit.PSI:
					// psi
					uom = CreateQuotientUOM(UnitType.PRESSURE, Unit.PSI, UnitsManager.GetString("psi.name"),
							UnitsManager.GetString("psi.symbol"), UnitsManager.GetString("psi.desc"), GetUOM(Unit.POUND_FORCE),
							GetUOM(Unit.SQUARE_INCH));
					break;

				case Unit.IN_HG:
					// inches of Mercury
					uom = CreateScalarUOM(UnitType.PRESSURE, Unit.IN_HG, UnitsManager.GetString("inhg.name"),
							UnitsManager.GetString("inhg.symbol"), UnitsManager.GetString("inhg.desc"));
					uom.SetConversion(Quantity.CreateAmount("0.4911531047"), GetUOM(Unit.PSI));
					break;

				case Unit.SQUARE_INCH:
					// square inch
					uom = CreatePowerUOM(UnitType.AREA, Unit.SQUARE_INCH, UnitsManager.GetString("in2.name"),
							UnitsManager.GetString("in2.symbol"), UnitsManager.GetString("in2.desc"), GetUOM(Unit.INCH), 2);
					uom.SetConversion(Quantity.DivideAmounts("1", "144"), GetUOM(Unit.SQUARE_FOOT));
					break;

				case Unit.SQUARE_FOOT:
					// square foot
					uom = CreatePowerUOM(UnitType.AREA, Unit.SQUARE_FOOT, UnitsManager.GetString("ft2.name"),
							UnitsManager.GetString("ft2.symbol"), UnitsManager.GetString("ft2.desc"), GetUOM(Unit.FOOT), 2);
					break;

				case Unit.SQUARE_YARD:
					// square yard
					uom = CreatePowerUOM(UnitType.AREA, Unit.SQUARE_YARD, UnitsManager.GetString("yd2.name"),
							UnitsManager.GetString("yd2.symbol"), UnitsManager.GetString("yd2.desc"), GetUOM(Unit.YARD), 2);
					break;

				case Unit.ACRE:
					// acre
					uom = CreateScalarUOM(UnitType.AREA, Unit.ACRE, UnitsManager.GetString("acre.name"),
							UnitsManager.GetString("acre.symbol"), UnitsManager.GetString("acre.desc"));
					uom.SetConversion(Quantity.CreateAmount("43560"), GetUOM(Unit.SQUARE_FOOT));
					break;

				case Unit.CUBIC_INCH:
					// cubic inch
					uom = CreatePowerUOM(UnitType.VOLUME, Unit.CUBIC_INCH, UnitsManager.GetString("in3.name"),
							UnitsManager.GetString("in3.symbol"), UnitsManager.GetString("in3.desc"), GetUOM(Unit.INCH), 3);
					uom.SetConversion(Quantity.DivideAmounts("1", "1728"), GetUOM(Unit.CUBIC_FOOT));
					break;

				case Unit.CUBIC_FOOT:
					// cubic feet
					uom = CreatePowerUOM(UnitType.VOLUME, Unit.CUBIC_FOOT, UnitsManager.GetString("ft3.name"),
							UnitsManager.GetString("ft3.symbol"), UnitsManager.GetString("ft3.desc"), GetUOM(Unit.FOOT), 3);
					break;

				case Unit.CUBIC_FEET_PER_SEC:
					// flow (volume)
					uom = CreateQuotientUOM(UnitType.VOLUMETRIC_FLOW, Unit.CUBIC_FEET_PER_SEC,
							UnitsManager.GetString("ft3PerSec.name"), UnitsManager.GetString("ft3PerSec.symbol"),
							UnitsManager.GetString("ft3PerSec.desc"), GetUOM(Unit.CUBIC_FOOT), GetSecond());
					break;

				case Unit.CORD:
					// cord
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.CORD, UnitsManager.GetString("cord.name"),
							UnitsManager.GetString("cord.symbol"), UnitsManager.GetString("cord.desc"));
					uom.SetConversion(Quantity.CreateAmount("128"), GetUOM(Unit.CUBIC_FOOT));
					break;

				case Unit.CUBIC_YARD:
					// cubic yard
					uom = CreatePowerUOM(UnitType.VOLUME, Unit.CUBIC_YARD, UnitsManager.GetString("yd3.name"),
							UnitsManager.GetString("yd3.symbol"), UnitsManager.GetString("yd3.desc"), GetUOM(Unit.YARD), 3);
					break;

				case Unit.FEET_PER_SEC:
					// feet/sec
					uom = CreateQuotientUOM(UnitType.VELOCITY, Unit.FEET_PER_SEC, UnitsManager.GetString("fps.name"),
							UnitsManager.GetString("fps.symbol"), UnitsManager.GetString("fps.desc"), GetUOM(Unit.FOOT), GetSecond());
					break;

				case Unit.KNOT:
					// knot
					uom = CreateScalarUOM(UnitType.VELOCITY, Unit.KNOT, UnitsManager.GetString("knot.name"),
							UnitsManager.GetString("knot.symbol"), UnitsManager.GetString("knot.desc"));
					uom.SetConversion(Quantity.DivideAmounts("6080", "3600"), GetUOM(Unit.FEET_PER_SEC));
					break;

				case Unit.FEET_PER_SEC_SQUARED:
					// acceleration
					uom = CreateQuotientUOM(UnitType.ACCELERATION, Unit.FEET_PER_SEC_SQUARED, UnitsManager.GetString("ftps2.name"),
							UnitsManager.GetString("ftps2.symbol"), UnitsManager.GetString("ftps2.desc"), GetUOM(Unit.FOOT),
							GetUOM(Unit.SQUARE_SECOND));
					break;

				case Unit.HP:
					// HP (mechanical)
					uom = CreateProductUOM(UnitType.POWER, Unit.HP, UnitsManager.GetString("hp.name"),
							UnitsManager.GetString("hp.symbol"), UnitsManager.GetString("hp.desc"), GetUOM(Unit.POUND_FORCE),
							GetUOM(Unit.FEET_PER_SEC));
					uom.ScalingFactor = Quantity.CreateAmount("550");
					break;

				case Unit.BTU:
					// BTU = 1055.056 Joules (778.169 ft-lbf)
					uom = CreateScalarUOM(UnitType.ENERGY, Unit.BTU, UnitsManager.GetString("btu.name"),
							UnitsManager.GetString("btu.symbol"), UnitsManager.GetString("btu.desc"));
					uom.SetConversion(Quantity.CreateAmount("778.1692622659652"), GetUOM(Unit.FOOT_POUND_FORCE));
					break;

				case Unit.FOOT_POUND_FORCE:
					// ft-lbf
					uom = CreateProductUOM(UnitType.ENERGY, Unit.FOOT_POUND_FORCE, UnitsManager.GetString("ft_lbf.name"),
							UnitsManager.GetString("ft_lbf.symbol"), UnitsManager.GetString("ft_lbf.desc"), GetUOM(Unit.FOOT),
							GetUOM(Unit.POUND_FORCE));
					break;

				case Unit.POUND_FORCE:
					// force F = m·A (lbf)
					uom = CreateProductUOM(UnitType.FORCE, Unit.POUND_FORCE, UnitsManager.GetString("lbf.name"),
							UnitsManager.GetString("lbf.symbol"), UnitsManager.GetString("lbf.desc"), GetUOM(Unit.POUND_MASS),
							GetUOM(Unit.FEET_PER_SEC_SQUARED));

					// factor is acceleration of gravity
					Quantity gravity = GetQuantity(Constant.GRAVITY).Convert(GetUOM(Unit.FEET_PER_SEC_SQUARED));
					uom.ScalingFactor = gravity.Amount;
					break;

				case Unit.GRAIN:
					// mass
					uom = CreateScalarUOM(UnitType.MASS, Unit.GRAIN, UnitsManager.GetString("grain.name"),
							UnitsManager.GetString("grain.symbol"), UnitsManager.GetString("grain.desc"));
					uom.SetConversion(Quantity.DivideAmounts("1", "7000"), GetUOM(Unit.POUND_MASS));
					break;

				case Unit.MILES_PER_HOUR:
					// velocity
					uom = CreateScalarUOM(UnitType.VELOCITY, Unit.MILES_PER_HOUR, UnitsManager.GetString("mph.name"),
							UnitsManager.GetString("mph.symbol"), UnitsManager.GetString("mph.desc"));
					uom.SetConversion(Quantity.DivideAmounts("5280", "3600"), GetUOM(Unit.FEET_PER_SEC));
					break;

				case Unit.REV_PER_MIN:
					// rpm
					uom = CreateQuotientUOM(UnitType.FREQUENCY, Unit.REV_PER_MIN, UnitsManager.GetString("rpm.name"),
							UnitsManager.GetString("rpm.symbol"), UnitsManager.GetString("rpm.desc"), GetOne(), GetMinute());
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
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.US_GALLON, UnitsManager.GetString("us_gallon.name"),
							UnitsManager.GetString("us_gallon.symbol"), UnitsManager.GetString("us_gallon.desc"));
					uom.SetConversion(Quantity.CreateAmount("231"), GetUOM(Unit.CUBIC_INCH));
					break;

				case Unit.US_BARREL:
					// barrel
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.US_BARREL, UnitsManager.GetString("us_bbl.name"),
							UnitsManager.GetString("us_bbl.symbol"), UnitsManager.GetString("us_bbl.desc"));
					uom.SetConversion(Quantity.CreateAmount("42"), GetUOM(Unit.US_GALLON));
					break;

				case Unit.US_BUSHEL:
					// bushel
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.US_BUSHEL, UnitsManager.GetString("us_bu.name"),
							UnitsManager.GetString("us_bu.symbol"), UnitsManager.GetString("us_bu.desc"));
					uom.SetConversion(Quantity.CreateAmount("2150.42058"), GetUOM(Unit.CUBIC_INCH));
					break;

				case Unit.US_FLUID_OUNCE:
					// fluid ounce
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.US_FLUID_OUNCE, UnitsManager.GetString("us_fl_oz.name"),
							UnitsManager.GetString("us_fl_oz.symbol"), UnitsManager.GetString("us_fl_oz.desc"));
					uom.SetConversion(Quantity.CreateAmount("0.0078125"), GetUOM(Unit.US_GALLON));
					break;

				case Unit.US_CUP:
					// cup
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.US_CUP, UnitsManager.GetString("us_cup.name"),
							UnitsManager.GetString("us_cup.symbol"), UnitsManager.GetString("us_cup.desc"));
					uom.SetConversion(Quantity.CreateAmount("8"), GetUOM(Unit.US_FLUID_OUNCE));
					break;

				case Unit.US_PINT:
					// pint
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.US_PINT, UnitsManager.GetString("us_pint.name"),
							UnitsManager.GetString("us_pint.symbol"), UnitsManager.GetString("us_pint.desc"));
					uom.SetConversion(Quantity.CreateAmount("16"), GetUOM(Unit.US_FLUID_OUNCE));
					break;

				case Unit.US_QUART:
					// quart
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.US_QUART, UnitsManager.GetString("us_quart.name"),
							UnitsManager.GetString("us_quart.symbol"), UnitsManager.GetString("us_quart.desc"));
					uom.SetConversion(Quantity.CreateAmount("32"), GetUOM(Unit.US_FLUID_OUNCE));
					break;

				case Unit.US_TABLESPOON:
					// tablespoon
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.US_TABLESPOON, UnitsManager.GetString("us_tbsp.name"),
							UnitsManager.GetString("us_tbsp.symbol"), UnitsManager.GetString("us_tbsp.desc"));
					uom.SetConversion(Quantity.CreateAmount("0.5"), GetUOM(Unit.US_FLUID_OUNCE));
					break;

				case Unit.US_TEASPOON:
					// teaspoon
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.US_TEASPOON, UnitsManager.GetString("us_tsp.name"),
							UnitsManager.GetString("us_tsp.symbol"), UnitsManager.GetString("us_tsp.desc"));
					uom.SetConversion(Quantity.DivideAmounts("1", "6"), GetUOM(Unit.US_FLUID_OUNCE));
					break;

				case Unit.US_TON:
					// ton
					uom = CreateScalarUOM(UnitType.MASS, Unit.US_TON, UnitsManager.GetString("us_ton.name"),
							UnitsManager.GetString("us_ton.symbol"), UnitsManager.GetString("us_ton.desc"));
					uom.SetConversion(Quantity.CreateAmount("2000"), GetUOM(Unit.POUND_MASS));
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
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.BR_GALLON, UnitsManager.GetString("br_gallon.name"),
							UnitsManager.GetString("br_gallon.symbol"), UnitsManager.GetString("br_gallon.desc"));
					uom.SetConversion(Quantity.CreateAmount("277.4194327916215"), GetUOM(Unit.CUBIC_INCH));
					break;

				case Unit.BR_BUSHEL:
					// bushel
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.BR_BUSHEL, UnitsManager.GetString("br_bu.name"),
							UnitsManager.GetString("br_bu.symbol"), UnitsManager.GetString("br_bu.desc"));
					uom.SetConversion(Quantity.CreateAmount("8"), GetUOM(Unit.BR_GALLON));
					break;

				case Unit.BR_FLUID_OUNCE:
					// fluid ounce
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.BR_FLUID_OUNCE, UnitsManager.GetString("br_fl_oz.name"),
							UnitsManager.GetString("br_fl_oz.symbol"), UnitsManager.GetString("br_fl_oz.desc"));
					uom.SetConversion(Quantity.CreateAmount("0.00625"), GetUOM(Unit.BR_GALLON));
					break;

				case Unit.BR_CUP:
					// cup
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.BR_CUP, UnitsManager.GetString("br_cup.name"),
							UnitsManager.GetString("br_cup.symbol"), UnitsManager.GetString("br_cup.desc"));
					uom.SetConversion(Quantity.CreateAmount("8"), GetUOM(Unit.BR_FLUID_OUNCE));
					break;

				case Unit.BR_PINT:
					// pint
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.BR_PINT, UnitsManager.GetString("br_pint.name"),
							UnitsManager.GetString("br_pint.symbol"), UnitsManager.GetString("br_pint.desc"));
					uom.SetConversion(Quantity.CreateAmount("20"), GetUOM(Unit.BR_FLUID_OUNCE));
					break;

				case Unit.BR_QUART:
					// quart
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.BR_QUART, UnitsManager.GetString("br_quart.name"),
							UnitsManager.GetString("br_quart.symbol"), UnitsManager.GetString("br_quart.desc"));
					uom.SetConversion(Quantity.CreateAmount("40"), GetUOM(Unit.BR_FLUID_OUNCE));
					break;

				case Unit.BR_TABLESPOON:
					// tablespoon
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.BR_TABLESPOON, UnitsManager.GetString("br_tbsp.name"),
							UnitsManager.GetString("br_tbsp.symbol"), UnitsManager.GetString("br_tbsp.desc"));
					uom.SetConversion(Quantity.CreateAmount("0.625"), GetUOM(Unit.BR_FLUID_OUNCE));
					break;

				case Unit.BR_TEASPOON:
					// teaspoon
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.BR_TEASPOON, UnitsManager.GetString("br_tsp.name"),
							UnitsManager.GetString("br_tsp.symbol"), UnitsManager.GetString("br_tsp.desc"));
					uom.SetConversion(Quantity.DivideAmounts("5", "24"), GetUOM(Unit.BR_FLUID_OUNCE));
					break;

				case Unit.BR_TON:
					// ton
					uom = CreateScalarUOM(UnitType.MASS, Unit.BR_TON, UnitsManager.GetString("br_ton.name"),
							UnitsManager.GetString("br_ton.symbol"), UnitsManager.GetString("br_ton.desc"));
					uom.SetConversion(Quantity.CreateAmount("2240"), GetUOM(Unit.POUND_MASS));
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
					uom = CreateScalarUOM(UnitType.CURRENCY, Unit.US_DOLLAR, UnitsManager.GetString("us_dollar.name"),
							UnitsManager.GetString("us_dollar.symbol"), UnitsManager.GetString("us_dollar.desc"));
					break;

				case Unit.EURO:
					uom = CreateScalarUOM(UnitType.CURRENCY, Unit.EURO, UnitsManager.GetString("euro.name"),
							UnitsManager.GetString("euro.symbol"), UnitsManager.GetString("euro.desc"));
					break;

				case Unit.YUAN:
					uom = CreateScalarUOM(UnitType.CURRENCY, Unit.YUAN, UnitsManager.GetString("yuan.name"),
							UnitsManager.GetString("yuan.symbol"), UnitsManager.GetString("yuan.desc"));
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

			UnitOfMeasure uom = null;

			if (SymbolRegistry.ContainsKey(symbol))
			{
				uom = SymbolRegistry[symbol];
			}
			else
			{
				// create a new one
				uom = new UnitOfMeasure(type, name, symbol, description);
			}
			return uom;
		}

		private UnitOfMeasure CreateScalarUOM(UnitType type, Unit? id, string name, string symbol, string description)
		{
			UnitOfMeasure uom = CreateUOM(type, id, name, symbol, description);
			uom.UnitEnumeration = id;
			RegisterUnit(uom);

			return uom;
		}

		/**
 * Create a unit of measure that is not a power, product or quotient
 * 
 * @param type
 *            {@link UnitType}
 * @param name
 *            Name of unit of measure
 * @param symbol
 *            Symbol (must be unique)
 * @param description
 *            Description of unit of measure
 * @return {@link UnitOfMeasure}
 * @
 *             Exception
 */
		public UnitOfMeasure CreateScalarUOM(UnitType type, string name, string symbol, string description)

		{
			return CreateScalarUOM(type, null, name, symbol, description);
		}

		/**
	* Create a unit of measure with a base raised to an integral power
	* 
	* @param type
	*            {@link UnitType}
	* @param id
	*            {@link Unit}
	* @param name
	*            Name of unit of measure
	* @param symbol
	*            Symbol (must be unique)
	* @param description
	*            Description of unit of measure
	* @param base
	*            {@link UnitOfMeasure}
	* @param power
	*            Exponent
	* @return {@link UnitOfMeasure}
	* @
	*             Exception
*/
		public UnitOfMeasure CreatePowerUOM(UnitType type, Unit? id, string name, string symbol, string description, UnitOfMeasure baseUOM, int power)
		{
			UnitOfMeasure uom = CreateUOM(type, id, name, symbol, description);
			uom.SetPowerUnit(baseUOM, power);
			uom.UnitEnumeration = id;
			RegisterUnit(uom);
			return uom;
		}

		/**
		 * Create a unit of measure with a base raised to an integral exponent
		 * 
		 * @param type
		 *            {@link UnitType}
		 * @param name
		 *            Name of unit of measure
		 * @param symbol
		 *            Symbol (must be unique)
		 * @param description
		 *            Description of unit of measure
		 * @param base
		 *            {@link UnitOfMeasure}
		 * @param exponent
		 *            Exponent
		 * @return {@link UnitOfMeasure}
		 * @
		 *             Exception
		 */
		public UnitOfMeasure CreatePowerUOM(UnitType type, string name, string symbol, string description, UnitOfMeasure baseUOM, int exponent)
		{
			return CreatePowerUOM(type, null, name, symbol, description, baseUOM, exponent);
		}

		/**
		 * Create a unit of measure with a base raised to an integral exponent
		 * 
		 * @param base
		 *            {@link UnitOfMeasure}
		 * @param exponent
		 *            Exponent
		 * @return {@link UnitOfMeasure}
		 * @
		 *             Exception
		 */
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

		/**
	 * Create a unit of measure that is the product of two other units of
	 * measure
	 * 
	 * @param type
	 *            {@link UnitType}
	 * @param id
	 *            {@link Unit}
	 * @param name
	 *            Name of unit of measure
	 * @param symbol
	 *            Symbol (must be unique)
	 * @param description
	 *            Description of unit of measure
	 * @param multiplier
	 *            {@link UnitOfMeasure} multiplier
	 * @param multiplicand
	 *            {@link UnitOfMeasure} multiplicand
	 * @return {@link UnitOfMeasure}
	 * @
	 *             Exception
	 */
		public UnitOfMeasure CreateProductUOM(UnitType type, Unit? id, string name, string symbol, string description,
				UnitOfMeasure multiplier, UnitOfMeasure multiplicand)
		{

			UnitOfMeasure uom = CreateUOM(type, id, name, symbol, description);
			uom.SetProductUnits(multiplier, multiplicand);
			uom.UnitEnumeration = id;
			RegisterUnit(uom);
			return uom;
		}

		/**
		 * Create a unit of measure that is the product of two other units of
		 * measure
		 * 
		 * @param type
		 *            {@link UnitType}
		 * @param name
		 *            Name of unit of measure
		 * @param symbol
		 *            Symbol (must be unique)
		 * @param description
		 *            Description of unit of measure
		 * @param multiplier
		 *            {@link UnitOfMeasure} multiplier
		 * @param multiplicand
		 *            {@link UnitOfMeasure} multiplicand
		 * @return {@link UnitOfMeasure}
		 * @
		 *             Exception
		 */
		public UnitOfMeasure CreateProductUOM(UnitType type, string name, string symbol, string description,
				UnitOfMeasure multiplier, UnitOfMeasure multiplicand)
		{
			return CreateProductUOM(type, null, name, symbol, description, multiplier, multiplicand);
		}

		/**
		 * Create a unit of measure that is the product of two other units of
		 * measure
		 * 
		 * @param multiplier
		 *            {@link UnitOfMeasure} multiplier
		 * @param multiplicand
		 *            {@link UnitOfMeasure} multiplicand
		 * @return {@link UnitOfMeasure}
		 * @
		 *             Exception
		 */
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

		/**
	 * Create a unit of measure that is a unit divided by another unit
	 * 
	 * @param type
	 *            {@link UnitType}
	 * @param id
	 *            {@link Unit}
	 * @param name
	 *            Name of unit of measure
	 * @param symbol
	 *            Symbol (must be unique)
	 * @param description
	 *            Description of unit of measure
	 * @param dividend
	 *            {@link UnitOfMeasure}
	 * @param divisor
	 *            {@link UnitOfMeasure}
	 * @return {@link UnitOfMeasure}
	 * @
	 *             Exception
	 */
		public UnitOfMeasure CreateQuotientUOM(UnitType type, Unit? id, string name, string symbol, string description,
				UnitOfMeasure dividend, UnitOfMeasure divisor)
		{
			UnitOfMeasure uom = CreateUOM(type, id, name, symbol, description);
			uom.SetQuotientUnits(dividend, divisor);
			uom.UnitEnumeration = id;
			RegisterUnit(uom);
			return uom;
		}

		/**
		 * Create a unit of measure that is a unit divided by another unit
		 * 
		 * @param type
		 *            {@link UnitType}
		 * @param name
		 *            Name of unit of measure
		 * @param symbol
		 *            Symbol (must be unique)
		 * @param description
		 *            Description of unit of measure
		 * @param dividend
		 *            {@link UnitOfMeasure}
		 * @param divisor
		 *            {@link UnitOfMeasure}
		 * @return {@link UnitOfMeasure}
		 * @
		 *             Exception
		 */
		public UnitOfMeasure CreateQuotientUOM(UnitType type, string name, string symbol, string description,
				UnitOfMeasure dividend, UnitOfMeasure divisor)
		{
			return this.CreateQuotientUOM(type, null, name, symbol, description, dividend, divisor);
		}

		/**
		 * Create a unit of measure that is a unit divided by another unit
		 * 
		 * @param dividend
		 *            {@link UnitOfMeasure}
		 * @param divisor
		 *            {@link UnitOfMeasure}
		 * @return {@link UnitOfMeasure}
		 * @
		 *             Exception
		 */
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

		/**
 * Get the quantity defined as a contant value
 * 
 * @param constant
 *            {@link Constant}
 * @return {@link Quantity}
 * @throws Exception
 *             Exception
 */
		public Quantity GetQuantity(Constant constant)
		{
			Quantity named = null;

			switch (constant)
			{
				case Constant.LIGHT_VELOCITY:
					named = new Quantity(Quantity.CreateAmount("299792458"), GetUOM(Unit.METRE_PER_SEC));
					named.Name = UnitsManager.GetString("light.name");
					named.Symbol = UnitsManager.GetString("light.symbol");
					named.Description = UnitsManager.GetString("light.desc");
					break;

				case Constant.LIGHT_YEAR:
					Quantity year = new Quantity(decimal.One, GetUOM(Unit.JULIAN_YEAR));
					named = GetQuantity(Constant.LIGHT_VELOCITY).Multiply(year);
					named.Name = UnitsManager.GetString("ly.name");
					named.Symbol = UnitsManager.GetString("ly.symbol");
					named.Description = UnitsManager.GetString("ly.desc");
					break;

				case Constant.GRAVITY:
					named = new Quantity(Quantity.CreateAmount("9.80665"), GetUOM(Unit.METRE_PER_SEC_SQUARED));
					named.Name = UnitsManager.GetString("gravity.name");
					named.Symbol = UnitsManager.GetString("gravity.symbol");
					named.Description = UnitsManager.GetString("gravity.desc");
					break;

				case Constant.PLANCK_CONSTANT:
					UnitOfMeasure js = CreateProductUOM(GetUOM(Unit.JOULE), GetSecond());
					named = new Quantity(Quantity.CreateAmount("6.62607004081E-34"), js);
					named.Name = UnitsManager.GetString("planck.name");
					named.Symbol = UnitsManager.GetString("planck.symbol");
					named.Description = UnitsManager.GetString("planck.desc");
					break;

				case Constant.BOLTZMANN_CONSTANT:
					UnitOfMeasure jk = CreateQuotientUOM(GetUOM(Unit.JOULE), GetUOM(Unit.KELVIN));
					named = new Quantity(Quantity.CreateAmount("1.3806485279E-23"), jk);
					named.Name = UnitsManager.GetString("boltzmann.name");
					named.Symbol = UnitsManager.GetString("boltzmann.symbol");
					named.Description = UnitsManager.GetString("boltzmann.desc");
					break;

				case Constant.AVAGADRO_CONSTANT:
					// NA
					named = new Quantity(Quantity.CreateAmount("6.02214085774E+23"), GetOne());
					named.Name = UnitsManager.GetString("avo.name");
					named.Symbol = UnitsManager.GetString("avo.symbol");
					named.Description = UnitsManager.GetString("avo.desc");
					break;

				case Constant.GAS_CONSTANT:
					// R
					named = GetQuantity(Constant.BOLTZMANN_CONSTANT).Multiply(GetQuantity(Constant.AVAGADRO_CONSTANT));
					named.Name = UnitsManager.GetString("gas.name");
					named.Symbol = UnitsManager.GetString("gas.symbol");
					named.Description = UnitsManager.GetString("gas.desc");
					break;

				case Constant.ELEMENTARY_CHARGE:
					// e
					named = new Quantity(Quantity.CreateAmount("1.602176620898E-19"), GetUOM(Unit.COULOMB));
					named.Name = UnitsManager.GetString("e.name");
					named.Symbol = UnitsManager.GetString("e.symbol");
					named.Description = UnitsManager.GetString("e.desc");
					break;

				case Constant.FARADAY_CONSTANT:
					// F = e.NA
					Quantity qe = GetQuantity(Constant.ELEMENTARY_CHARGE);
					named = qe.Multiply(GetQuantity(Constant.AVAGADRO_CONSTANT));
					named.Name = UnitsManager.GetString("faraday.name");
					named.Symbol = UnitsManager.GetString("faraday.symbol");
					named.Description = UnitsManager.GetString("faraday.desc");
					break;

				case Constant.ELECTRIC_PERMITTIVITY:
					// epsilon0 = 1/(mu0*c^2)
					Quantity vc = GetQuantity(Constant.LIGHT_VELOCITY);
					named = GetQuantity(Constant.MAGNETIC_PERMEABILITY).Multiply(vc).Multiply(vc).Invert();
					named.Name = UnitsManager.GetString("eps0.name");
					named.Symbol = UnitsManager.GetString("eps0.symbol");
					named.Description = UnitsManager.GetString("eps0.desc");
					break;

				case Constant.MAGNETIC_PERMEABILITY:
					// mu0
					UnitOfMeasure hm = CreateQuotientUOM(GetUOM(Unit.HENRY), GetUOM(Unit.METRE));
					decimal fourPi = new decimal(4.0E-07 * Math.PI);
					named = new Quantity(fourPi, hm);
					named.Name = UnitsManager.GetString("mu0.name");
					named.Symbol = UnitsManager.GetString("mu0.symbol");
					named.Description = UnitsManager.GetString("mu0.desc");
					break;

				case Constant.ELECTRON_MASS:
					// me
					named = new Quantity(Quantity.CreateAmount("9.1093835611E-28"), GetUOM(Unit.GRAM));
					named.Name = UnitsManager.GetString("me.name");
					named.Symbol = UnitsManager.GetString("me.symbol");
					named.Description = UnitsManager.GetString("me.desc");
					break;

				case Constant.PROTON_MASS:
					// mp
					named = new Quantity(Quantity.CreateAmount("1.67262189821E-24"), GetUOM(Unit.GRAM));
					named.Name = UnitsManager.GetString("mp.name");
					named.Symbol = UnitsManager.GetString("mp.symbol");
					named.Description = UnitsManager.GetString("mp.desc");
					break;

				case Constant.STEFAN_BOLTZMANN:
					UnitOfMeasure k4 = CreatePowerUOM(GetUOM(Unit.KELVIN), 4);
					UnitOfMeasure sb = CreateQuotientUOM(GetUOM(Unit.WATTS_PER_SQ_METRE), k4);
					named = new Quantity(Quantity.CreateAmount("5.67036713E-08"), sb);
					named.Name = UnitsManager.GetString("sb.name");
					named.Symbol = UnitsManager.GetString("sb.symbol");
					named.Description = UnitsManager.GetString("sb.desc");
					break;

				case Constant.HUBBLE_CONSTANT:
					UnitOfMeasure kps = GetUOM(Prefix.KILO, GetUOM(Unit.METRE_PER_SEC));
					UnitOfMeasure mpc = GetUOM(Prefix.MEGA, GetUOM(Unit.PARSEC));
					UnitOfMeasure hubble = CreateQuotientUOM(kps, mpc);
					named = new Quantity(Quantity.CreateAmount("71.9"), hubble);
					named.Name = UnitsManager.GetString("hubble.name");
					named.Symbol = UnitsManager.GetString("hubble.symbol");
					named.Description = UnitsManager.GetString("hubble.desc");
					break;

				default:
					break;
			}

			return named;
		}

		/**
		* Cache this unit of measure
		* 
		* @param uom
		*            {@link UnitOfMeasure} to cache
		* @
		*             Exception
*/
		public void RegisterUnit(UnitOfMeasure uom)
		{
			string key = uom.Symbol;

			// get first by symbol
			UnitOfMeasure current = SymbolRegistry[key];

			if (current != null)
			{
				// already cached
				return;
			}

			// cache it
			SymbolRegistry[key] = uom;

			// next by unit enumeration
			Unit? id = uom.UnitEnumeration;

			if (id.HasValue)
			{
				UnitRegistry[id.Value] = uom;
			}

			// finally by base symbol
			key = uom.GetBaseSymbol();

			if (BaseRegistry[key] == null)
			{
				BaseRegistry[key] = uom;
			}
		}

		/**
	* Remove a unit from the cache
	* 
	* @param uom
	*            {@link UnitOfMeasure} to remove
	* @
	*             Exception
*/
		public void UnregisterUnit(UnitOfMeasure uom)
		{
			if (uom == null)
			{
				return;
			}

			lock (new object())
			{
				UnitOfMeasure removedUOM;
				if (uom.UnitEnumeration.HasValue)
				{
					UnitRegistry.TryRemove(uom.UnitEnumeration.Value, out removedUOM);
				}

				// remove by symbol and base symbol
				SymbolRegistry.TryRemove(uom.Symbol, out removedUOM);
				BaseRegistry.TryRemove(uom.GetBaseSymbol(), out removedUOM);
			}
		}
	}
}
