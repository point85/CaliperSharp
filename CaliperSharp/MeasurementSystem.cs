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
		public static String GetUnitString(String key)
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
 * @throws Exception
 *             Exception
 */
		public UnitOfMeasure GetOne()
		{
			return GetUOM(Unit.ONE);
		}

		/**
 * Get the unit of measure with this unique enumerated type
 * 
 * @param unit
 *            {@link Unit}
 * @return {@link UnitOfMeasure}
 * @throws Exception
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

				case Unit.SECOND:
					// second
					uom = CreateScalarUOM(UnitType.TIME, Unit.SECOND, UnitsManager.GetString("sec.name"),
							UnitsManager.GetString("sec.symbol"), UnitsManager.GetString("sec.desc"));
					break;

				case Unit.MINUTE:
					// minute
					uom = CreateScalarUOM(UnitType.TIME, Unit.MINUTE, UnitsManager.GetString("min.name"),
							UnitsManager.GetString("min.symbol"), UnitsManager.GetString("min.desc"));
					uom.SetConversion(Quantity.createAmount("60"), getUOM(Unit.SECOND));
					break;

				case Unit.HOUR:
					// hour
					uom = CreateScalarUOM(UnitType.TIME, Unit.HOUR, UnitsManager.GetString("hr.name"),
							UnitsManager.GetString("hr.symbol"), UnitsManager.GetString("hr.desc"));
					uom.setConversion(Quantity.createAmount("3600"), getUOM(Unit.SECOND));
					break;

				case Unit.DAY:
					// day
					uom = CreateScalarUOM(UnitType.TIME, Unit.DAY, UnitsManager.GetString("day.name"),
							UnitsManager.GetString("day.symbol"), UnitsManager.GetString("day.desc"));
					uom.setConversion(Quantity.createAmount("86400"), getUOM(Unit.SECOND));
					break;

				case Unit.WEEK:
					// week
					uom = CreateScalarUOM(UnitType.TIME, Unit.WEEK, UnitsManager.GetString("week.name"),
							UnitsManager.GetString("week.symbol"), UnitsManager.GetString("week.desc"));
					uom.setConversion(Quantity.createAmount("604800"), getUOM(Unit.SECOND));
					break;

				case Unit.JULIAN_YEAR:
					// Julian year
					uom = CreateScalarUOM(UnitType.TIME, Unit.JULIAN_YEAR, UnitsManager.GetString("jyear.name"),
							UnitsManager.GetString("jyear.symbol"), UnitsManager.GetString("jyear.desc"));
					uom.setConversion(Quantity.createAmount("3.1557600E+07"), getUOM(Unit.SECOND));

					break;

				case Unit.SQUARE_SECOND:
					// square second
					uom = createPowerUOM(UnitType.TIME_SQUARED, Unit.SQUARE_SECOND, UnitsManager.GetString("s2.name"),
							UnitsManager.GetString("s2.symbol"), UnitsManager.GetString("s2.desc"), getUOM(Unit.SECOND), 2);
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
					uom.setConversion(getOne());
					break;

				case Unit.STERADIAN:
					// solid angle steradian (sr)
					uom = CreateScalarUOM(UnitType.SOLID_ANGLE, Unit.STERADIAN, UnitsManager.GetString("steradian.name"),
							UnitsManager.GetString("steradian.symbol"), UnitsManager.GetString("steradian.desc"));
					uom.setConversion(getOne());
					break;

				case Unit.DEGREE:
					// degree of arc
					uom = CreateScalarUOM(UnitType.PLANE_ANGLE, Unit.DEGREE, UnitsManager.GetString("degree.name"),
							UnitsManager.GetString("degree.symbol"), UnitsManager.GetString("degree.desc"));
					uom.setConversion(Quantity.divideAmounts(String.valueOf(Math.PI), "180"), getUOM(Unit.RADIAN));
					break;

				case Unit.ARC_SECOND:
					// degree of arc
					uom = CreateScalarUOM(UnitType.PLANE_ANGLE, Unit.ARC_SECOND, UnitsManager.GetString("arcsec.name"),
							UnitsManager.GetString("arcsec.symbol"), UnitsManager.GetString("arcsec.desc"));
					uom.setConversion(Quantity.divideAmounts(String.valueOf(Math.PI), "648000"), getUOM(Unit.RADIAN));
					break;

				case Unit.METRE:
					// fundamental length
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.METRE, UnitsManager.GetString("m.name"),
							UnitsManager.GetString("m.symbol"), UnitsManager.GetString("m.desc"));
					break;

				case Unit.DIOPTER:
					// per metre
					uom = createQuotientUOM(UnitType.RECIPROCAL_LENGTH, Unit.DIOPTER, UnitsManager.GetString("diopter.name"),
							UnitsManager.GetString("diopter.symbol"), UnitsManager.GetString("diopter.desc"), getOne(),
							getUOM(Unit.METRE));
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
					uom.setConversion(Prefix.KILO.getFactor(), getUOM(Unit.KILOGRAM));
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
					uom.setConversion(Prefix.MILLI.getFactor(), getUOM(Unit.KILOGRAM));
					break;

				case Unit.CARAT:
					// carat
					uom = CreateScalarUOM(UnitType.MASS, Unit.CARAT, UnitsManager.GetString("carat.name"),
							UnitsManager.GetString("carat.symbol"), UnitsManager.GetString("carat.desc"));
					uom.setConversion(Quantity.createAmount("0.2"), getUOM(Unit.GRAM));
					break;

				case Unit.SQUARE_METRE:
					// square metre
					uom = createPowerUOM(UnitType.AREA, Unit.SQUARE_METRE, UnitsManager.GetString("m2.name"),
							UnitsManager.GetString("m2.symbol"), UnitsManager.GetString("m2.desc"), getUOM(Unit.METRE), 2);
					break;

				case Unit.HECTARE:
					// hectare
					uom = CreateScalarUOM(UnitType.AREA, Unit.HECTARE, UnitsManager.GetString("hectare.name"),
							UnitsManager.GetString("hectare.symbol"), UnitsManager.GetString("hectare.desc"));
					uom.setConversion(Quantity.createAmount("10000"), getUOM(Unit.SQUARE_METRE));
					break;

				case Unit.METRE_PER_SEC:
					// velocity
					uom = createQuotientUOM(UnitType.VELOCITY, Unit.METRE_PER_SEC, UnitsManager.GetString("mps.name"),
							UnitsManager.GetString("mps.symbol"), UnitsManager.GetString("mps.desc"), getUOM(Unit.METRE), getSecond());
					break;

				case Unit.METRE_PER_SEC_SQUARED:
					// acceleration
					uom = createQuotientUOM(UnitType.ACCELERATION, Unit.METRE_PER_SEC_SQUARED, UnitsManager.GetString("mps2.name"),
							UnitsManager.GetString("mps2.symbol"), UnitsManager.GetString("mps2.desc"), getUOM(Unit.METRE),
							getUOM(Unit.SQUARE_SECOND));
					break;

				case Unit.CUBIC_METRE:
					// cubic metre
					uom = createPowerUOM(UnitType.VOLUME, Unit.CUBIC_METRE, UnitsManager.GetString("m3.name"),
							UnitsManager.GetString("m3.symbol"), UnitsManager.GetString("m3.desc"), getUOM(Unit.METRE), 3);
					break;

				case Unit.LITRE:
					// litre
					uom = CreateScalarUOM(UnitType.VOLUME, Unit.LITRE, UnitsManager.GetString("litre.name"),
							UnitsManager.GetString("litre.symbol"), UnitsManager.GetString("litre.desc"));
					uom.setConversion(Prefix.MILLI.getFactor(), getUOM(Unit.CUBIC_METRE));
					break;

				case Unit.CUBIC_METRE_PER_SEC:
					// flow (volume)
					uom = createQuotientUOM(UnitType.VOLUMETRIC_FLOW, Unit.CUBIC_METRE_PER_SEC,
							UnitsManager.GetString("m3PerSec.name"), UnitsManager.GetString("m3PerSec.symbol"),
							UnitsManager.GetString("m3PerSec.desc"), getUOM(Unit.CUBIC_METRE), getSecond());
					break;

				case Unit.KILOGRAM_PER_SEC:
					// flow (mass)
					uom = createQuotientUOM(UnitType.MASS_FLOW, Unit.KILOGRAM_PER_SEC, UnitsManager.GetString("kgPerSec.name"),
							UnitsManager.GetString("kgPerSec.symbol"), UnitsManager.GetString("kgPerSec.desc"), getUOM(Unit.KILOGRAM),
							getSecond());
					break;

				case Unit.KILOGRAM_PER_CU_METRE:
					// kg/m^3
					uom = createQuotientUOM(UnitType.DENSITY, Unit.KILOGRAM_PER_CU_METRE, UnitsManager.GetString("kg_m3.name"),
							UnitsManager.GetString("kg_m3.symbol"), UnitsManager.GetString("kg_m3.desc"), getUOM(Unit.KILOGRAM),
							getUOM(Unit.CUBIC_METRE));
					break;

				case Unit.PASCAL_SECOND:
					// dynamic viscosity
					uom = createProductUOM(UnitType.DYNAMIC_VISCOSITY, Unit.PASCAL_SECOND, UnitsManager.GetString("pascal_sec.name"),
							UnitsManager.GetString("pascal_sec.symbol"), UnitsManager.GetString("pascal_sec.desc"), getUOM(Unit.PASCAL),
							getSecond());
					break;

				case Unit.SQUARE_METRE_PER_SEC:
					// kinematic viscosity
					uom = createQuotientUOM(UnitType.KINEMATIC_VISCOSITY, Unit.SQUARE_METRE_PER_SEC,
							UnitsManager.GetString("m2PerSec.name"), UnitsManager.GetString("m2PerSec.symbol"),
							UnitsManager.GetString("m2PerSec.desc"), getUOM(Unit.SQUARE_METRE), getSecond());
					break;

				case Unit.CALORIE:
					// thermodynamic calorie
					uom = CreateScalarUOM(UnitType.ENERGY, Unit.CALORIE, UnitsManager.GetString("calorie.name"),
							UnitsManager.GetString("calorie.symbol"), UnitsManager.GetString("calorie.desc"));
					uom.setConversion(Quantity.createAmount("4.184"), getUOM(Unit.JOULE));
					break;

				case Unit.NEWTON:
					// force F = m·A (newton)
					uom = createProductUOM(UnitType.FORCE, Unit.NEWTON, UnitsManager.GetString("newton.name"),
							UnitsManager.GetString("newton.symbol"), UnitsManager.GetString("newton.desc"), getUOM(Unit.KILOGRAM),
							getUOM(Unit.METRE_PER_SEC_SQUARED));
					break;

				case Unit.NEWTON_METRE:
					// newton-metre
					uom = createProductUOM(UnitType.ENERGY, Unit.NEWTON_METRE, UnitsManager.GetString("n_m.name"),
							UnitsManager.GetString("n_m.symbol"), UnitsManager.GetString("n_m.desc"), getUOM(Unit.NEWTON),
							getUOM(Unit.METRE));
					break;

				case Unit.JOULE:
					// energy (joule)
					uom = createProductUOM(UnitType.ENERGY, Unit.JOULE, UnitsManager.GetString("joule.name"),
							UnitsManager.GetString("joule.symbol"), UnitsManager.GetString("joule.desc"), getUOM(Unit.NEWTON),
							getUOM(Unit.METRE));
					break;

				case Unit.ELECTRON_VOLT:
					// ev
					Quantity e = this.getQuantity(Constant.ELEMENTARY_CHARGE);
					uom = createProductUOM(UnitType.ENERGY, Unit.ELECTRON_VOLT, UnitsManager.GetString("ev.name"),
								UnitsManager.GetString("ev.symbol"), UnitsManager.GetString("ev.desc"), e.getUOM(), getUOM(Unit.VOLT));
					uom.setScalingFactor(e.getAmount());
					break;

				case Unit.WATT_HOUR:
					// watt-hour
					uom = createProductUOM(UnitType.ENERGY, Unit.WATT_HOUR, UnitsManager.GetString("wh.name"),
							UnitsManager.GetString("wh.symbol"), UnitsManager.GetString("wh.desc"), getUOM(Unit.WATT), getHour());
					break;

				case Unit.WATT:
					// power (watt)
					uom = createQuotientUOM(UnitType.POWER, Unit.WATT, UnitsManager.GetString("watt.name"),
							UnitsManager.GetString("watt.symbol"), UnitsManager.GetString("watt.desc"), getUOM(Unit.JOULE), getSecond());
					break;

				case Unit.HERTZ:
					// frequency (hertz)
					uom = createQuotientUOM(UnitType.FREQUENCY, Unit.HERTZ, UnitsManager.GetString("hertz.name"),
							UnitsManager.GetString("hertz.symbol"), UnitsManager.GetString("hertz.desc"), getOne(), getSecond());
					break;

				case Unit.RAD_PER_SEC:
					// angular frequency
					uom = createQuotientUOM(UnitType.FREQUENCY, Unit.RAD_PER_SEC, UnitsManager.GetString("radpers.name"),
							UnitsManager.GetString("radpers.symbol"), UnitsManager.GetString("radpers.desc"), getUOM(Unit.RADIAN),
							getSecond());
					BigDecimal twoPi = new BigDecimal("2").multiply(new BigDecimal(Math.PI), UnitOfMeasure.MATH_CONTEXT);
					uom.setConversion(BigDecimal.ONE.divide(twoPi, UnitOfMeasure.MATH_CONTEXT), getUOM(Unit.HERTZ));
					break;

				case Unit.PASCAL:
					// pressure
					uom = createQuotientUOM(UnitType.PRESSURE, Unit.PASCAL, UnitsManager.GetString("pascal.name"),
							UnitsManager.GetString("pascal.symbol"), UnitsManager.GetString("pascal.desc"), getUOM(Unit.NEWTON),
							getUOM(Unit.SQUARE_METRE));
					break;

				case Unit.ATMOSPHERE:
					// pressure
					uom = CreateScalarUOM(UnitType.PRESSURE, Unit.ATMOSPHERE, UnitsManager.GetString("atm.name"),
							UnitsManager.GetString("atm.symbol"), UnitsManager.GetString("atm.desc"));
					uom.setConversion(Quantity.createAmount("101325"), getUOM(Unit.PASCAL));
					break;

				case Unit.BAR:
					// pressure
					uom = CreateScalarUOM(UnitType.PRESSURE, Unit.BAR, UnitsManager.GetString("bar.name"),
							UnitsManager.GetString("bar.symbol"), UnitsManager.GetString("bar.desc"));
					uom.setConversion(BigDecimal.ONE, getUOM(Unit.PASCAL), Quantity.createAmount("1.0E+05"));
					break;

				case Unit.COULOMB:
					// charge (coulomb)
					uom = createProductUOM(UnitType.ELECTRIC_CHARGE, Unit.COULOMB, UnitsManager.GetString("coulomb.name"),
							UnitsManager.GetString("coulomb.symbol"), UnitsManager.GetString("coulomb.desc"), getUOM(Unit.AMPERE),
							getSecond());
					break;

				case Unit.VOLT:
					// voltage (volt)
					uom = createQuotientUOM(UnitType.ELECTROMOTIVE_FORCE, Unit.VOLT, UnitsManager.GetString("volt.name"),
							UnitsManager.GetString("volt.symbol"), UnitsManager.GetString("volt.desc"), getUOM(Unit.WATT),
							getUOM(Unit.AMPERE));
					break;

				case Unit.OHM:
					// resistance (ohm)
					uom = createQuotientUOM(UnitType.ELECTRIC_RESISTANCE, Unit.OHM, UnitsManager.GetString("ohm.name"),
							UnitsManager.GetString("ohm.symbol"), UnitsManager.GetString("ohm.desc"), getUOM(Unit.VOLT),
							getUOM(Unit.AMPERE));
					break;

				case Unit.FARAD:
					// capacitance (farad)
					uom = createQuotientUOM(UnitType.ELECTRIC_CAPACITANCE, Unit.FARAD, UnitsManager.GetString("farad.name"),
							UnitsManager.GetString("farad.symbol"), UnitsManager.GetString("farad.desc"), getUOM(Unit.COULOMB),
							getUOM(Unit.VOLT));
					break;

				case Unit.FARAD_PER_METRE:
					// electric permittivity (farad/metre)
					uom = createQuotientUOM(UnitType.ELECTRIC_PERMITTIVITY, Unit.FARAD_PER_METRE,
							UnitsManager.GetString("fperm.name"), UnitsManager.GetString("fperm.symbol"), UnitsManager.GetString("fperm.desc"),
							getUOM(Unit.FARAD), getUOM(Unit.METRE));
					break;

				case Unit.AMPERE_PER_METRE:
					// electric field strength(ampere/metre)
					uom = createQuotientUOM(UnitType.ELECTRIC_FIELD_STRENGTH, Unit.AMPERE_PER_METRE,
							UnitsManager.GetString("aperm.name"), UnitsManager.GetString("aperm.symbol"), UnitsManager.GetString("aperm.desc"),
							getUOM(Unit.AMPERE), getUOM(Unit.METRE));
					break;

				case Unit.WEBER:
					// magnetic flux (weber)
					uom = createProductUOM(UnitType.MAGNETIC_FLUX, Unit.WEBER, UnitsManager.GetString("weber.name"),
							UnitsManager.GetString("weber.symbol"), UnitsManager.GetString("weber.desc"), getUOM(Unit.VOLT), getSecond());
					break;

				case Unit.TESLA:
					// magnetic flux density (tesla)
					uom = createQuotientUOM(UnitType.MAGNETIC_FLUX_DENSITY, Unit.TESLA, UnitsManager.GetString("tesla.name"),
							UnitsManager.GetString("tesla.symbol"), UnitsManager.GetString("tesla.desc"), getUOM(Unit.WEBER),
							getUOM(Unit.SQUARE_METRE));
					break;

				case Unit.HENRY:
					// inductance (henry)
					uom = createQuotientUOM(UnitType.ELECTRIC_INDUCTANCE, Unit.HENRY, UnitsManager.GetString("henry.name"),
							UnitsManager.GetString("henry.symbol"), UnitsManager.GetString("henry.desc"), getUOM(Unit.WEBER),
							getUOM(Unit.AMPERE));
					break;

				case Unit.SIEMENS:
					// electrical conductance (siemens)
					uom = createQuotientUOM(UnitType.ELECTRIC_CONDUCTANCE, Unit.SIEMENS, UnitsManager.GetString("siemens.name"),
							UnitsManager.GetString("siemens.symbol"), UnitsManager.GetString("siemens.desc"), getUOM(Unit.AMPERE),
							getUOM(Unit.VOLT));
					break;

				case Unit.CELSIUS:
					// °C = °K - 273.15
					uom = CreateScalarUOM(UnitType.TEMPERATURE, Unit.CELSIUS, UnitsManager.GetString("celsius.name"),
							UnitsManager.GetString("celsius.symbol"), UnitsManager.GetString("celsius.desc"));
					uom.setConversion(BigDecimal.ONE, getUOM(Unit.KELVIN), Quantity.createAmount("273.15"));
					break;

				case Unit.LUMEN:
					// luminous flux (lumen)
					uom = createProductUOM(UnitType.LUMINOUS_FLUX, Unit.LUMEN, UnitsManager.GetString("lumen.name"),
							UnitsManager.GetString("lumen.symbol"), UnitsManager.GetString("lumen.desc"), getUOM(Unit.CANDELA),
							getUOM(Unit.STERADIAN));
					break;

				case Unit.LUX:
					// illuminance (lux)
					uom = createQuotientUOM(UnitType.ILLUMINANCE, Unit.LUX, UnitsManager.GetString("lux.name"),
							UnitsManager.GetString("lux.symbol"), UnitsManager.GetString("lux.desc"), getUOM(Unit.LUMEN),
							getUOM(Unit.SQUARE_METRE));
					break;

				case Unit.BECQUEREL:
					// radioactivity (becquerel). Same base symbol as Hertz
					uom = CreateScalarUOM(UnitType.RADIOACTIVITY, Unit.BECQUEREL, UnitsManager.GetString("becquerel.name"),
							UnitsManager.GetString("becquerel.symbol"), UnitsManager.GetString("becquerel.desc"));
					break;

				case Unit.GRAY:
					// gray (Gy)
					uom = createQuotientUOM(UnitType.RADIATION_DOSE_ABSORBED, Unit.GRAY, UnitsManager.GetString("gray.name"),
							UnitsManager.GetString("gray.symbol"), UnitsManager.GetString("gray.desc"), getUOM(Unit.JOULE),
							getUOM(Unit.KILOGRAM));
					break;

				case Unit.SIEVERT:
					// sievert (Sv)
					uom = createQuotientUOM(UnitType.RADIATION_DOSE_EFFECTIVE, Unit.SIEVERT, UnitsManager.GetString("sievert.name"),
							UnitsManager.GetString("sievert.symbol"), UnitsManager.GetString("sievert.desc"), getUOM(Unit.JOULE),
							getUOM(Unit.KILOGRAM));
					break;

				case Unit.SIEVERTS_PER_HOUR:
					uom = createQuotientUOM(UnitType.RADIATION_DOSE_RATE, Unit.SIEVERTS_PER_HOUR, UnitsManager.GetString("sph.name"),
							UnitsManager.GetString("sph.symbol"), UnitsManager.GetString("sph.desc"), getUOM(Unit.SIEVERT), getHour());
					break;

				case Unit.KATAL:
					// katal (kat)
					uom = createQuotientUOM(UnitType.CATALYTIC_ACTIVITY, Unit.KATAL, UnitsManager.GetString("katal.name"),
							UnitsManager.GetString("katal.symbol"), UnitsManager.GetString("katal.desc"), getUOM(Unit.MOLE), getSecond());
					break;

				case Unit:
					// Unit (U)
					uom = CreateScalarUOM(UnitType.CATALYTIC_ACTIVITY, Unit.UNIT, UnitsManager.GetString("unit.name"),
							UnitsManager.GetString("unit.symbol"), UnitsManager.GetString("unit.desc"));
					uom.setConversion(Quantity.divideAmounts("1.0E-06", "60"), getUOM(Unit.KATAL));
					break;

				case Unit.INTERNATIONAL_UNIT:
					uom = CreateScalarUOM(UnitType.SUBSTANCE_AMOUNT, Unit.INTERNATIONAL_UNIT, UnitsManager.GetString("iu.name"),
							UnitsManager.GetString("iu.symbol"), UnitsManager.GetString("iu.desc"));
					break;

				case Unit.ANGSTROM:
					// length
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.ANGSTROM, UnitsManager.GetString("angstrom.name"),
							UnitsManager.GetString("angstrom.symbol"), UnitsManager.GetString("angstrom.desc"));
					uom.setConversion(Quantity.createAmount("0.1"), getUOM(Prefix.NANO, getUOM(Unit.METRE)));
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
					uom.setConversion(Quantity.createAmount("8"), getUOM(Unit.BIT));
					break;

				case Unit.WATTS_PER_SQ_METRE:
					uom = createQuotientUOM(UnitType.IRRADIANCE, Unit.WATTS_PER_SQ_METRE, UnitsManager.GetString("wsm.name"),
							UnitsManager.GetString("wsm.symbol"), UnitsManager.GetString("wsm.desc"), getUOM(Unit.WATT),
							getUOM(Unit.SQUARE_METRE));
					break;

				case Unit.PARSEC:
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.PARSEC, UnitsManager.GetString("parsec.name"),
							UnitsManager.GetString("parsec.symbol"), UnitsManager.GetString("parsec.desc"));
					uom.setConversion(Quantity.createAmount("3.08567758149137E+16"), getUOM(Unit.METRE));
					break;

				case Unit.ASTRONOMICAL_UNIT:
					uom = CreateScalarUOM(UnitType.LENGTH, Unit.ASTRONOMICAL_UNIT, UnitsManager.GetString("au.name"),
							UnitsManager.GetString("au.symbol"), UnitsManager.GetString("au.desc"));
					uom.setConversion(Quantity.createAmount("1.49597870700E+11"), getUOM(Unit.METRE));
					break;

				default:
					break;
			}

			return uom;
		}

		private UnitOfMeasure CreateUOM(UnitType type, Unit id, string name, string symbol, string description)
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

		private UnitOfMeasure CreateScalarUOM(UnitType type, Unit id, string name, string symbol, string description)
		{
			UnitOfMeasure uom = CreateUOM(type, id, name, symbol, description);
			uom.UnitEnumeration = id;
			RegisterUnit(uom);

			return uom;
		}

		/**
 * Cache this unit of measure
 * 
 * @param uom
 *            {@link UnitOfMeasure} to cache
 * @throws Exception
 *             Exception
 */
		public void RegisterUnit(UnitOfMeasure uom)
		{
			String key = uom.Symbol;

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

			if (id != null)
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
 * @throws Exception
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
				if (uom.UnitEnumeration != null)
				{
					UnitRegistry.TryRemove(uom.UnitEnumeration, out removedUOM);
				}

				// remove by symbol and base symbol
				SymbolRegistry.TryRemove(uom.Symbol, out removedUOM);
				BaseRegistry.TryRemove(uom.GetBaseSymbol(), out removedUOM);
			}
		}
	}
}
