namespace CaliperSharp
{
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

	/**
* UnitType is an enumeration of unit of measure types. Only units of measure
* with the same type can be converted.
* 
* @author Kent Randall
*
*/
	public enum UnitType
	{
		// dimension-less "1"
		UNITY,

		// fundamental
		LENGTH, MASS, TIME, ELECTRIC_CURRENT, TEMPERATURE, SUBSTANCE_AMOUNT, LUMINOSITY,

		// other physical
		AREA, VOLUME, DENSITY, VELOCITY, VOLUMETRIC_FLOW, MASS_FLOW, FREQUENCY, ACCELERATION, FORCE, PRESSURE, ENERGY, POWER, ELECTRIC_CHARGE,
		ELECTROMOTIVE_FORCE, ELECTRIC_RESISTANCE, ELECTRIC_CAPACITANCE, ELECTRIC_PERMITTIVITY, ELECTRIC_FIELD_STRENGTH,
		MAGNETIC_FLUX, MAGNETIC_FLUX_DENSITY, ELECTRIC_INDUCTANCE, ELECTRIC_CONDUCTANCE,
		LUMINOUS_FLUX, ILLUMINANCE, RADIATION_DOSE_ABSORBED, RADIATION_DOSE_EFFECTIVE, RADIATION_DOSE_RATE, RADIOACTIVITY, CATALYTIC_ACTIVITY, DYNAMIC_VISCOSITY,
		KINEMATIC_VISCOSITY, RECIPROCAL_LENGTH, PLANE_ANGLE, SOLID_ANGLE, INTENSITY, COMPUTER_SCIENCE, TIME_SQUARED, MOLAR_CONCENTRATION, IRRADIANCE,

		// currency
		CURRENCY,

		// unclassified.  Reserved for use when creating custom units of measure.
		UNCLASSIFIED
	}
}
