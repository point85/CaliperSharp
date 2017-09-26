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
		private static readonly string MESSAGES_BUNDLE_NAME = "Message";

		// resource bundle for exception messages
		private static ResourceManager Messages;

		// standard unified system
		private static MeasurementSystem UnifiedSystem = new MeasurementSystem();

		// name of resource set with translatable strings for UOMs (e.g. time)
		private static readonly string UNIT_BUNDLE_NAME = "Unit";

		// unit resource bundle (e.g. time units)
		private ResourceSet Symbols;

		// registry by unit symbol
		private ConcurrentDictionary<string, UnitOfMeasure> SymbolRegistry = new ConcurrentDictionary<string, UnitOfMeasure>();

		// registry by base symbol
		private ConcurrentDictionary<string, UnitOfMeasure> BaseRegistry = new ConcurrentDictionary<string, UnitOfMeasure>();

		// registry for units by enumeration
		private ConcurrentDictionary<Unit, UnitOfMeasure> UnitRegistry = new ConcurrentDictionary<Unit, UnitOfMeasure>();

		private MeasurementSystem()
		{
			// common unit strings
			symbols = ResourceBundle.getBundle(UNIT_BUNDLE_NAME, Locale.getDefault());
			messages = ResourceBundle.getBundle(MESSAGES_BUNDLE_NAME, Locale.getDefault());
		}

		// get a particular message by its key
		static string GetMessage(string key)
		{
			return messages.getString(key);
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
	}
}
