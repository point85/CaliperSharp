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
					uom = CreateScalarUOM(UnitType.UNITY, Unit.ONE, units.getString("one.name"),
							units.getString("one.symbol"), units.getString("one.desc"));
					break;

				case SECOND:
					// second
					uom = createScalarUOM(UnitType.TIME, Unit.SECOND, units.getString("sec.name"),
							units.getString("sec.symbol"), units.getString("sec.desc"));
					break;

				case MINUTE:
					// minute
					uom = createScalarUOM(UnitType.TIME, Unit.MINUTE, units.getString("min.name"),
							units.getString("min.symbol"), units.getString("min.desc"));
					uom.setConversion(Quantity.createAmount("60"), getUOM(Unit.SECOND));
					break;

				case HOUR:
					// hour
					uom = createScalarUOM(UnitType.TIME, Unit.HOUR, units.getString("hr.name"),
							units.getString("hr.symbol"), units.getString("hr.desc"));
					uom.setConversion(Quantity.createAmount("3600"), getUOM(Unit.SECOND));
					break;

				case DAY:
					// day
					uom = createScalarUOM(UnitType.TIME, Unit.DAY, units.getString("day.name"),
							units.getString("day.symbol"), units.getString("day.desc"));
					uom.setConversion(Quantity.createAmount("86400"), getUOM(Unit.SECOND));
					break;

				case WEEK:
					// week
					uom = createScalarUOM(UnitType.TIME, Unit.WEEK, units.getString("week.name"),
							units.getString("week.symbol"), units.getString("week.desc"));
					uom.setConversion(Quantity.createAmount("604800"), getUOM(Unit.SECOND));
					break;

				case JULIAN_YEAR:
					// Julian year
					uom = createScalarUOM(UnitType.TIME, Unit.JULIAN_YEAR, units.getString("jyear.name"),
							units.getString("jyear.symbol"), units.getString("jyear.desc"));
					uom.setConversion(Quantity.createAmount("3.1557600E+07"), getUOM(Unit.SECOND));

					break;

				case SQUARE_SECOND:
					// square second
					uom = createPowerUOM(UnitType.TIME_SQUARED, Unit.SQUARE_SECOND, units.getString("s2.name"),
							units.getString("s2.symbol"), units.getString("s2.desc"), getUOM(Unit.SECOND), 2);
					break;

				case MOLE:
					// substance amount
					uom = createScalarUOM(UnitType.SUBSTANCE_AMOUNT, Unit.MOLE, units.getString("mole.name"),
							units.getString("mole.symbol"), units.getString("mole.desc"));
					break;

				case EQUIVALENT:
					// substance amount
					uom = createScalarUOM(UnitType.SUBSTANCE_AMOUNT, Unit.EQUIVALENT, units.getString("equivalent.name"),
							units.getString("equivalent.symbol"), units.getString("equivalent.desc"));
					break;

				case DECIBEL:
					// decibel
					uom = createScalarUOM(UnitType.INTENSITY, Unit.DECIBEL, units.getString("db.name"),
							units.getString("db.symbol"), units.getString("db.desc"));
					break;

				case RADIAN:
					// plane angle radian (rad)
					uom = createScalarUOM(UnitType.PLANE_ANGLE, Unit.RADIAN, units.getString("radian.name"),
							units.getString("radian.symbol"), units.getString("radian.desc"));
					uom.setConversion(getOne());
					break;

				case STERADIAN:
					// solid angle steradian (sr)
					uom = createScalarUOM(UnitType.SOLID_ANGLE, Unit.STERADIAN, units.getString("steradian.name"),
							units.getString("steradian.symbol"), units.getString("steradian.desc"));
					uom.setConversion(getOne());
					break;

				case DEGREE:
					// degree of arc
					uom = createScalarUOM(UnitType.PLANE_ANGLE, Unit.DEGREE, units.getString("degree.name"),
							units.getString("degree.symbol"), units.getString("degree.desc"));
					uom.setConversion(Quantity.divideAmounts(String.valueOf(Math.PI), "180"), getUOM(Unit.RADIAN));
					break;

				case ARC_SECOND:
					// degree of arc
					uom = createScalarUOM(UnitType.PLANE_ANGLE, Unit.ARC_SECOND, units.getString("arcsec.name"),
							units.getString("arcsec.symbol"), units.getString("arcsec.desc"));
					uom.setConversion(Quantity.divideAmounts(String.valueOf(Math.PI), "648000"), getUOM(Unit.RADIAN));
					break;

				case METRE:
					// fundamental length
					uom = createScalarUOM(UnitType.LENGTH, Unit.METRE, units.getString("m.name"),
							units.getString("m.symbol"), units.getString("m.desc"));
					break;

				case DIOPTER:
					// per metre
					uom = createQuotientUOM(UnitType.RECIPROCAL_LENGTH, Unit.DIOPTER, units.getString("diopter.name"),
							units.getString("diopter.symbol"), units.getString("diopter.desc"), getOne(),
							getUOM(Unit.METRE));
					break;

				case KILOGRAM:
					// fundamental mass
					uom = createScalarUOM(UnitType.MASS, Unit.KILOGRAM, units.getString("kg.name"),
							units.getString("kg.symbol"), units.getString("kg.desc"));
					break;

				case TONNE:
					// mass
					uom = createScalarUOM(UnitType.MASS, Unit.TONNE, units.getString("tonne.name"),
							units.getString("tonne.symbol"), units.getString("tonne.desc"));
					uom.setConversion(Prefix.KILO.getFactor(), getUOM(Unit.KILOGRAM));
					break;

				case KELVIN:
					// fundamental temperature
					uom = createScalarUOM(UnitType.TEMPERATURE, Unit.KELVIN, units.getString("kelvin.name"),
							units.getString("kelvin.symbol"), units.getString("kelvin.desc"));
					break;

				case AMPERE:
					// electric current
					uom = createScalarUOM(UnitType.ELECTRIC_CURRENT, Unit.AMPERE, units.getString("amp.name"),
							units.getString("amp.symbol"), units.getString("amp.desc"));
					break;

				case CANDELA:
					// luminosity
					uom = createScalarUOM(UnitType.LUMINOSITY, Unit.CANDELA, units.getString("cd.name"),
							units.getString("cd.symbol"), units.getString("cd.desc"));
					break;

				case PH:
					// molar concentration
					uom = createScalarUOM(UnitType.MOLAR_CONCENTRATION, Unit.PH, units.getString("ph.name"),
							units.getString("ph.symbol"), units.getString("ph.desc"));
					break;

				case GRAM: // gram
					uom = createScalarUOM(UnitType.MASS, Unit.GRAM, units.getString("gram.name"),
							units.getString("gram.symbol"), units.getString("gram.desc"));
					uom.setConversion(Prefix.MILLI.getFactor(), getUOM(Unit.KILOGRAM));
					break;

				case CARAT:
					// carat
					uom = createScalarUOM(UnitType.MASS, Unit.CARAT, units.getString("carat.name"),
							units.getString("carat.symbol"), units.getString("carat.desc"));
					uom.setConversion(Quantity.createAmount("0.2"), getUOM(Unit.GRAM));
					break;

				case SQUARE_METRE:
					// square metre
					uom = createPowerUOM(UnitType.AREA, Unit.SQUARE_METRE, units.getString("m2.name"),
							units.getString("m2.symbol"), units.getString("m2.desc"), getUOM(Unit.METRE), 2);
					break;

				case HECTARE:
					// hectare
					uom = createScalarUOM(UnitType.AREA, Unit.HECTARE, units.getString("hectare.name"),
							units.getString("hectare.symbol"), units.getString("hectare.desc"));
					uom.setConversion(Quantity.createAmount("10000"), getUOM(Unit.SQUARE_METRE));
					break;

				case METRE_PER_SEC:
					// velocity
					uom = createQuotientUOM(UnitType.VELOCITY, Unit.METRE_PER_SEC, units.getString("mps.name"),
							units.getString("mps.symbol"), units.getString("mps.desc"), getUOM(Unit.METRE), getSecond());
					break;

				case METRE_PER_SEC_SQUARED:
					// acceleration
					uom = createQuotientUOM(UnitType.ACCELERATION, Unit.METRE_PER_SEC_SQUARED, units.getString("mps2.name"),
							units.getString("mps2.symbol"), units.getString("mps2.desc"), getUOM(Unit.METRE),
							getUOM(Unit.SQUARE_SECOND));
					break;

				case CUBIC_METRE:
					// cubic metre
					uom = createPowerUOM(UnitType.VOLUME, Unit.CUBIC_METRE, units.getString("m3.name"),
							units.getString("m3.symbol"), units.getString("m3.desc"), getUOM(Unit.METRE), 3);
					break;

				case LITRE:
					// litre
					uom = createScalarUOM(UnitType.VOLUME, Unit.LITRE, units.getString("litre.name"),
							units.getString("litre.symbol"), units.getString("litre.desc"));
					uom.setConversion(Prefix.MILLI.getFactor(), getUOM(Unit.CUBIC_METRE));
					break;

				case CUBIC_METRE_PER_SEC:
					// flow (volume)
					uom = createQuotientUOM(UnitType.VOLUMETRIC_FLOW, Unit.CUBIC_METRE_PER_SEC,
							units.getString("m3PerSec.name"), units.getString("m3PerSec.symbol"),
							units.getString("m3PerSec.desc"), getUOM(Unit.CUBIC_METRE), getSecond());
					break;

				case KILOGRAM_PER_SEC:
					// flow (mass)
					uom = createQuotientUOM(UnitType.MASS_FLOW, Unit.KILOGRAM_PER_SEC, units.getString("kgPerSec.name"),
							units.getString("kgPerSec.symbol"), units.getString("kgPerSec.desc"), getUOM(Unit.KILOGRAM),
							getSecond());
					break;

				case KILOGRAM_PER_CU_METRE:
					// kg/m^3
					uom = createQuotientUOM(UnitType.DENSITY, Unit.KILOGRAM_PER_CU_METRE, units.getString("kg_m3.name"),
							units.getString("kg_m3.symbol"), units.getString("kg_m3.desc"), getUOM(Unit.KILOGRAM),
							getUOM(Unit.CUBIC_METRE));
					break;

				case PASCAL_SECOND:
					// dynamic viscosity
					uom = createProductUOM(UnitType.DYNAMIC_VISCOSITY, Unit.PASCAL_SECOND, units.getString("pascal_sec.name"),
							units.getString("pascal_sec.symbol"), units.getString("pascal_sec.desc"), getUOM(Unit.PASCAL),
							getSecond());
					break;

				case SQUARE_METRE_PER_SEC:
					// kinematic viscosity
					uom = createQuotientUOM(UnitType.KINEMATIC_VISCOSITY, Unit.SQUARE_METRE_PER_SEC,
							units.getString("m2PerSec.name"), units.getString("m2PerSec.symbol"),
							units.getString("m2PerSec.desc"), getUOM(Unit.SQUARE_METRE), getSecond());
					break;

				case CALORIE:
					// thermodynamic calorie
					uom = createScalarUOM(UnitType.ENERGY, Unit.CALORIE, units.getString("calorie.name"),
							units.getString("calorie.symbol"), units.getString("calorie.desc"));
					uom.setConversion(Quantity.createAmount("4.184"), getUOM(Unit.JOULE));
					break;

				case NEWTON:
					// force F = m·A (newton)
					uom = createProductUOM(UnitType.FORCE, Unit.NEWTON, units.getString("newton.name"),
							units.getString("newton.symbol"), units.getString("newton.desc"), getUOM(Unit.KILOGRAM),
							getUOM(Unit.METRE_PER_SEC_SQUARED));
					break;

				case NEWTON_METRE:
					// newton-metre
					uom = createProductUOM(UnitType.ENERGY, Unit.NEWTON_METRE, units.getString("n_m.name"),
							units.getString("n_m.symbol"), units.getString("n_m.desc"), getUOM(Unit.NEWTON),
							getUOM(Unit.METRE));
					break;

				case JOULE:
					// energy (joule)
					uom = createProductUOM(UnitType.ENERGY, Unit.JOULE, units.getString("joule.name"),
							units.getString("joule.symbol"), units.getString("joule.desc"), getUOM(Unit.NEWTON),
							getUOM(Unit.METRE));
					break;

				case ELECTRON_VOLT:
					// ev
					Quantity e = this.getQuantity(Constant.ELEMENTARY_CHARGE);
					uom = createProductUOM(UnitType.ENERGY, Unit.ELECTRON_VOLT, units.getString("ev.name"),
								units.getString("ev.symbol"), units.getString("ev.desc"), e.getUOM(), getUOM(Unit.VOLT));
					uom.setScalingFactor(e.getAmount());
					break;

				case WATT_HOUR:
					// watt-hour
					uom = createProductUOM(UnitType.ENERGY, Unit.WATT_HOUR, units.getString("wh.name"),
							units.getString("wh.symbol"), units.getString("wh.desc"), getUOM(Unit.WATT), getHour());
					break;

				case WATT:
					// power (watt)
					uom = createQuotientUOM(UnitType.POWER, Unit.WATT, units.getString("watt.name"),
							units.getString("watt.symbol"), units.getString("watt.desc"), getUOM(Unit.JOULE), getSecond());
					break;

				case HERTZ:
					// frequency (hertz)
					uom = createQuotientUOM(UnitType.FREQUENCY, Unit.HERTZ, units.getString("hertz.name"),
							units.getString("hertz.symbol"), units.getString("hertz.desc"), getOne(), getSecond());
					break;

				case RAD_PER_SEC:
					// angular frequency
					uom = createQuotientUOM(UnitType.FREQUENCY, Unit.RAD_PER_SEC, units.getString("radpers.name"),
							units.getString("radpers.symbol"), units.getString("radpers.desc"), getUOM(Unit.RADIAN),
							getSecond());
					BigDecimal twoPi = new BigDecimal("2").multiply(new BigDecimal(Math.PI), UnitOfMeasure.MATH_CONTEXT);
					uom.setConversion(BigDecimal.ONE.divide(twoPi, UnitOfMeasure.MATH_CONTEXT), getUOM(Unit.HERTZ));
					break;

				case PASCAL:
					// pressure
					uom = createQuotientUOM(UnitType.PRESSURE, Unit.PASCAL, units.getString("pascal.name"),
							units.getString("pascal.symbol"), units.getString("pascal.desc"), getUOM(Unit.NEWTON),
							getUOM(Unit.SQUARE_METRE));
					break;

				case ATMOSPHERE:
					// pressure
					uom = createScalarUOM(UnitType.PRESSURE, Unit.ATMOSPHERE, units.getString("atm.name"),
							units.getString("atm.symbol"), units.getString("atm.desc"));
					uom.setConversion(Quantity.createAmount("101325"), getUOM(Unit.PASCAL));
					break;

				case BAR:
					// pressure
					uom = createScalarUOM(UnitType.PRESSURE, Unit.BAR, units.getString("bar.name"),
							units.getString("bar.symbol"), units.getString("bar.desc"));
					uom.setConversion(BigDecimal.ONE, getUOM(Unit.PASCAL), Quantity.createAmount("1.0E+05"));
					break;

				case COULOMB:
					// charge (coulomb)
					uom = createProductUOM(UnitType.ELECTRIC_CHARGE, Unit.COULOMB, units.getString("coulomb.name"),
							units.getString("coulomb.symbol"), units.getString("coulomb.desc"), getUOM(Unit.AMPERE),
							getSecond());
					break;

				case VOLT:
					// voltage (volt)
					uom = createQuotientUOM(UnitType.ELECTROMOTIVE_FORCE, Unit.VOLT, units.getString("volt.name"),
							units.getString("volt.symbol"), units.getString("volt.desc"), getUOM(Unit.WATT),
							getUOM(Unit.AMPERE));
					break;

				case OHM:
					// resistance (ohm)
					uom = createQuotientUOM(UnitType.ELECTRIC_RESISTANCE, Unit.OHM, units.getString("ohm.name"),
							units.getString("ohm.symbol"), units.getString("ohm.desc"), getUOM(Unit.VOLT),
							getUOM(Unit.AMPERE));
					break;

				case FARAD:
					// capacitance (farad)
					uom = createQuotientUOM(UnitType.ELECTRIC_CAPACITANCE, Unit.FARAD, units.getString("farad.name"),
							units.getString("farad.symbol"), units.getString("farad.desc"), getUOM(Unit.COULOMB),
							getUOM(Unit.VOLT));
					break;

				case FARAD_PER_METRE:
					// electric permittivity (farad/metre)
					uom = createQuotientUOM(UnitType.ELECTRIC_PERMITTIVITY, Unit.FARAD_PER_METRE,
							units.getString("fperm.name"), units.getString("fperm.symbol"), units.getString("fperm.desc"),
							getUOM(Unit.FARAD), getUOM(Unit.METRE));
					break;

				case AMPERE_PER_METRE:
					// electric field strength(ampere/metre)
					uom = createQuotientUOM(UnitType.ELECTRIC_FIELD_STRENGTH, Unit.AMPERE_PER_METRE,
							units.getString("aperm.name"), units.getString("aperm.symbol"), units.getString("aperm.desc"),
							getUOM(Unit.AMPERE), getUOM(Unit.METRE));
					break;

				case WEBER:
					// magnetic flux (weber)
					uom = createProductUOM(UnitType.MAGNETIC_FLUX, Unit.WEBER, units.getString("weber.name"),
							units.getString("weber.symbol"), units.getString("weber.desc"), getUOM(Unit.VOLT), getSecond());
					break;

				case TESLA:
					// magnetic flux density (tesla)
					uom = createQuotientUOM(UnitType.MAGNETIC_FLUX_DENSITY, Unit.TESLA, units.getString("tesla.name"),
							units.getString("tesla.symbol"), units.getString("tesla.desc"), getUOM(Unit.WEBER),
							getUOM(Unit.SQUARE_METRE));
					break;

				case HENRY:
					// inductance (henry)
					uom = createQuotientUOM(UnitType.ELECTRIC_INDUCTANCE, Unit.HENRY, units.getString("henry.name"),
							units.getString("henry.symbol"), units.getString("henry.desc"), getUOM(Unit.WEBER),
							getUOM(Unit.AMPERE));
					break;

				case SIEMENS:
					// electrical conductance (siemens)
					uom = createQuotientUOM(UnitType.ELECTRIC_CONDUCTANCE, Unit.SIEMENS, units.getString("siemens.name"),
							units.getString("siemens.symbol"), units.getString("siemens.desc"), getUOM(Unit.AMPERE),
							getUOM(Unit.VOLT));
					break;

				case CELSIUS:
					// °C = °K - 273.15
					uom = createScalarUOM(UnitType.TEMPERATURE, Unit.CELSIUS, units.getString("celsius.name"),
							units.getString("celsius.symbol"), units.getString("celsius.desc"));
					uom.setConversion(BigDecimal.ONE, getUOM(Unit.KELVIN), Quantity.createAmount("273.15"));
					break;

				case LUMEN:
					// luminous flux (lumen)
					uom = createProductUOM(UnitType.LUMINOUS_FLUX, Unit.LUMEN, units.getString("lumen.name"),
							units.getString("lumen.symbol"), units.getString("lumen.desc"), getUOM(Unit.CANDELA),
							getUOM(Unit.STERADIAN));
					break;

				case LUX:
					// illuminance (lux)
					uom = createQuotientUOM(UnitType.ILLUMINANCE, Unit.LUX, units.getString("lux.name"),
							units.getString("lux.symbol"), units.getString("lux.desc"), getUOM(Unit.LUMEN),
							getUOM(Unit.SQUARE_METRE));
					break;

				case BECQUEREL:
					// radioactivity (becquerel). Same base symbol as Hertz
					uom = createScalarUOM(UnitType.RADIOACTIVITY, Unit.BECQUEREL, units.getString("becquerel.name"),
							units.getString("becquerel.symbol"), units.getString("becquerel.desc"));
					break;

				case GRAY:
					// gray (Gy)
					uom = createQuotientUOM(UnitType.RADIATION_DOSE_ABSORBED, Unit.GRAY, units.getString("gray.name"),
							units.getString("gray.symbol"), units.getString("gray.desc"), getUOM(Unit.JOULE),
							getUOM(Unit.KILOGRAM));
					break;

				case SIEVERT:
					// sievert (Sv)
					uom = createQuotientUOM(UnitType.RADIATION_DOSE_EFFECTIVE, Unit.SIEVERT, units.getString("sievert.name"),
							units.getString("sievert.symbol"), units.getString("sievert.desc"), getUOM(Unit.JOULE),
							getUOM(Unit.KILOGRAM));
					break;

				case SIEVERTS_PER_HOUR:
					uom = createQuotientUOM(UnitType.RADIATION_DOSE_RATE, Unit.SIEVERTS_PER_HOUR, units.getString("sph.name"),
							units.getString("sph.symbol"), units.getString("sph.desc"), getUOM(Unit.SIEVERT), getHour());
					break;

				case KATAL:
					// katal (kat)
					uom = createQuotientUOM(UnitType.CATALYTIC_ACTIVITY, Unit.KATAL, units.getString("katal.name"),
							units.getString("katal.symbol"), units.getString("katal.desc"), getUOM(Unit.MOLE), getSecond());
					break;

				case UNIT:
					// Unit (U)
					uom = createScalarUOM(UnitType.CATALYTIC_ACTIVITY, Unit.UNIT, units.getString("unit.name"),
							units.getString("unit.symbol"), units.getString("unit.desc"));
					uom.setConversion(Quantity.divideAmounts("1.0E-06", "60"), getUOM(Unit.KATAL));
					break;

				case INTERNATIONAL_UNIT:
					uom = createScalarUOM(UnitType.SUBSTANCE_AMOUNT, Unit.INTERNATIONAL_UNIT, units.getString("iu.name"),
							units.getString("iu.symbol"), units.getString("iu.desc"));
					break;

				case ANGSTROM:
					// length
					uom = createScalarUOM(UnitType.LENGTH, Unit.ANGSTROM, units.getString("angstrom.name"),
							units.getString("angstrom.symbol"), units.getString("angstrom.desc"));
					uom.setConversion(Quantity.createAmount("0.1"), getUOM(Prefix.NANO, getUOM(Unit.METRE)));
					break;

				case BIT:
					// computer bit
					uom = createScalarUOM(UnitType.COMPUTER_SCIENCE, Unit.BIT, units.getString("bit.name"),
							units.getString("bit.symbol"), units.getString("bit.desc"));
					break;

				case BYTE:
					// computer byte
					uom = createScalarUOM(UnitType.COMPUTER_SCIENCE, Unit.BYTE, units.getString("byte.name"),
							units.getString("byte.symbol"), units.getString("byte.desc"));
					uom.setConversion(Quantity.createAmount("8"), getUOM(Unit.BIT));
					break;

				case WATTS_PER_SQ_METRE:
					uom = createQuotientUOM(UnitType.IRRADIANCE, Unit.WATTS_PER_SQ_METRE, units.getString("wsm.name"),
							units.getString("wsm.symbol"), units.getString("wsm.desc"), getUOM(Unit.WATT),
							getUOM(Unit.SQUARE_METRE));
					break;

				case PARSEC:
					uom = createScalarUOM(UnitType.LENGTH, Unit.PARSEC, units.getString("parsec.name"),
							units.getString("parsec.symbol"), units.getString("parsec.desc"));
					uom.setConversion(Quantity.createAmount("3.08567758149137E+16"), getUOM(Unit.METRE));
					break;

				case ASTRONOMICAL_UNIT:
					uom = createScalarUOM(UnitType.LENGTH, Unit.ASTRONOMICAL_UNIT, units.getString("au.name"),
							units.getString("au.symbol"), units.getString("au.desc"));
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
			uom.Unit = id;
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

		if (current != null) {
			// already cached
			return;
		}

		// cache it
		SymbolRegistry[key] = uom;

		// next by unit enumeration
		Unit? id = uom.Unit;

		if (id != null) {
			UnitRegistry[id.Value] = uom;
		}

	// finally by base symbol
	key = uom.GetBaseSymbol();

		if (baseRegistry.get(key) == null) {
			baseRegistry.put(key, uom);
		}
	}



	}
}
