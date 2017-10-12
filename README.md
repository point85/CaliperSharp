# CaliperSharp
The CaliperSharp library project manages units of measure and conversions between them.  It is a port of the java Caliper library (see https://github.com/point85/caliper).  CaliperSharp is designed to be lightweight and simple to use, yet comprehensive.  It includes a large number of pre-defined units of measure commonly found in science, engineering, technology, finance and the household.  These recognized systems of measurement include the International System of Units (SI), International Customary, United States and British Imperial.  Custom units of measure can also be created in the CaliperSharp unified measurement system.  Custom units are specific to a trade or industry such as industrial packaging where units of can, bottle, case and pallet are typical.  Custom units can be added to the unified system for units that are not pre-defined. 

A CaliperSharp measurement system is a collection of units of measure where each pair has a linear relationship, i.e. y = ax + b where 'x' is the abscissa unit to be converted, 'y' (the ordinate) is the converted unit, 'a' is the scaling factor and 'b' is the offset.  In the absence of a defined conversion, a unit will always have a conversion to itself.  A bridge unit conversion is defined to convert between the fundamental SI and International customary units of mass (i.e. kilogram to pound mass), length (i.e. metre to foot) and temperature (i.e. Kelvin to Rankine).  These three bridge conversions permit unit of measure conversions between the two systems.  A custom unit can define any bridge conversion such as a bottle to US fluid ounces or litres.
 
## Concepts

The diagram below illustrates these concepts.
![CaliperSharpDiagram](https://github.com/point85/CaliperSharp/master/CaliperSharp/Documentation/CaliperSharpDiagram.png)
 
All units are owned by the unified measurement system. Units 'x' and 'y' belong to a relational system (such as SI or International Customary).  Units 'w' and 'z' belong to a second relational system.  Unit 'y' has a linear conversion to unit 'x'; therefore 'x' must be defined before 'y' can be defined.  Unit 'x' is also related to 'y' by x = (y - b)/a.  Unit 'w' has a conversion to unit 'z'.  Unit 'z' is related to itself by z = z + 0. Unit 'x' has a bridge conversion defined to unit 'z' (for example a foot to a metre).  Note that a bridge conversion from 'z' to 'x' is not necessary since it is the inverse of the conversion from 'x' to 'z'.
 
*Scalar Unit* 

A simple unit, for example a metre, is defined as a scalar UOM.  A special scalar unit of measure is unity or dimensionless "1".  

*Product Unit*

A unit of measure that is the product of two other units is defined as a product UOM.  An example is a Joule which is a Newton·metre.  

*Quotient Unit*  

A unit of measure that is the quotient of two other units is defined as a quotient UOM. An example is a velocity, e.g. metre/second.  

*Power Unit*

A unit of measure that has an exponent for a base unit is defined as a power UOM. An example is area in metre^2. Note that an exponent of 0 is unity, and an exponent of 1 is the base unit itself. An exponent of 2 is a product unit where the multiplier and multiplicand are the base unit.  A power of -1 is a quotient unit of measure where the dividend is 1 and the divisor is the base unit.  

*Type*

Units are classified by type, e.g. length, mass, time and temperature.  Only units of the same type can be converted to one another. Pre-defined units of measure are also enumerated, e.g. kilogram, Newton, metre, etc.  Custom units (e.g. a 1 litre bottle) do not have a pre-defined type or enumeration and are referred to by a unique base symbol.

*Base Symbol*
 
All units have a base symbol that is the most reduced form of the unit.  For example, a Newton is kilogram·metre/second^2.  The base symbol is used in the measurement system to register each unit and to discern the result of arithmetic operations on quantities.  For example, dividing a quantity of Newton·metres by a quantity of metres results in a quantity of Newtons. 

*Quantity*

A quantity is an amount (implemented as a floating point double precision number) together with a unit of measure.  When arithmetic operations are performed on quantities, the original units can be transformed.  For example, multiplying a length quantity in metres by a force quantity in Newtons results in a quantity of energy in Joules (or Newton-metres).

*Product of Powers*

A unit of measure is represented internally as a product of two other power units of measure:

![CaliperSharp Diagram](https://github.com/point85/CaliperSharp/CaliperSharp/blob/master/Documentation/PowerProduct.png)

For a simple scalar UOM (e.g. kilogram), both of the UOMs are null with the exponents defaulted to 0.  For a product UOM (e.g. Newton), the first UOM is the multiplier and the second is the multiplicand with both exponents set to 1.  For a quotient UOM (e.g. kilograms/hour), the first UOM is the dividend and the second is the divisor.  The dividend has an exponent of 1 and the divisor an exponent of -1.  For a power UOM (e.g. square metres), the first UOM is the base and the exponent is the power.  In this case, the second UOM is null with the exponent defaulted to 0.

From the two power products, a unit of measure can then be recursively reduced to a map of base units of measure and corresponding exponents along with a scaling factor.  For example, a Newton reduces to (kg, 1), (m, 1), (s, -2) in the SI system.  Multiplying, dividing and converting a unit of measure is accomplished by merging the two maps (i.e. "cancelling out" units) and then computing the overall scaling factor.  The base symbol is obtained directly from the final map.
 
## Code Examples
The singleton unified MeasurementSystem is obtained by calling:
```cs
MeasurementSystem sys = MeasurementSystem.getSystem();
```
The Unit.properties file defines the name, symbol, description and symbol for each of the predefined units in the following code examples.  The Unit.properties file can be edited for localization.  For example, 'metres' can be changed to use the US spelling 'meters' or descriptions can be translated to another language.

The metre scalar UOM is created by the MeasurementSystem as follows:
```cs
UnitOfMeasure	uom = CreateScalarUOM(UnitType.LENGTH, Unit.METRE, UnitsManager.GetString("m.name"),
	UnitsManager.GetString("m.symbol"), UnitsManager.GetString("m.desc"));
``` 

The square metre power UOM is created by the MeasurementSystem as follows: 
```cs
UnitOfMeasure uom = CreatePowerUOM(UnitType.AREA, Unit.SQUARE_METRE, UnitsManager.GetString("m2.name"),
	UnitsManager.GetString("m2.symbol"), UnitsManager.GetString("m2.desc"), GetUOM(Unit.METRE), 2);
```

The metre per second quotient UOM is created by the MeasurementSystem as follows: 
```cs
UnitOfMeasure uom = CreateQuotientUOM(UnitType.VELOCITY, Unit.METRE_PER_SEC, UnitsManager.GetString("mps.name"),
	UnitsManager.GetString("mps.symbol"), UnitsManager.GetString("mps.desc"), GetUOM(Unit.METRE), GetSecond());;
```

The Newton product UOM is created by the MeasurementSystem as follows: 
```cs
UnitOfMeasure uom = CreateProductUOM(UnitType.FORCE, Unit.NEWTON, UnitsManager.GetString("newton.name"),
	UnitsManager.GetString("newton.symbol"), UnitsManager.GetString("newton.desc"), GetUOM(Unit.KILOGRAM),
	GetUOM(Unit.METRE_PER_SEC_SQUARED));
```

A millisecond is 1/1000th of a second with a defined prefix and created as:

```cs
UnitOfMeasure second = sys.GetSecond();
UnitOfMeasure msec = sys.GetUOM(Prefix.MILLI, second);
```

For a second example, a US gallon = 231 cubic inches:
```cs			
UnitOfMeasure 	uom = CreateScalarUOM(UnitType.VOLUME, Unit.US_GALLON, UnitsManager.GetString("us_gallon.name"),
	UnitsManager.GetString("us_gallon.symbol"), UnitsManager.GetString("us_gallon.desc"));
uom.SetConversion(231, GetUOM(Unit.CUBIC_INCH));
```

When creating the foot unit of measure in the unified measurement system, a bridge conversion to metre is defined (1 foot = 0.3048m):
```cs
UnitOfMeasure 	uom = CreateScalarUOM(UnitType.LENGTH, Unit.FOOT, UnitsManager.GetString("foot.name"),
	UnitsManager.GetString("foot.symbol"), UnitsManager.GetString("foot.desc"));

// bridge to SI
uom.SetBridgeConversion(0.3048, GetUOM(Unit.METRE), null);
```

Custom units and conversions can also be created:
```cs
// gallons per hour
UnitOfMeasure gph = sys.CreateQuotientUOM(UnitType.VOLUMETRIC_FLOW, "gph", "gal/hr", "gallons per hour",
	sys.GetUOM(Unit.US_GALLON), sys.GetHour());

// 1 16 oz can = 16 fl. oz.
UnitOfMeasure one16ozCan = sys.CreateScalarUOM(UnitType.VOLUME, "16 oz can", "16ozCan", "16 oz can");
one16ozCan.SetConversion(16, sys.GetUOM(Unit.US_FLUID_OUNCE));

// 400 cans = 50 US gallons
Quantity q400 = new Quantity(400, one16ozCan);
Quantity q50 = q400.Convert(sys.GetUOM(Unit.US_GALLON));

// 1 12 oz can = 12 fl.oz.
UnitOfMeasure one12ozCan = sys.CreateScalarUOM(UnitType.VOLUME, "12 oz can", "12ozCan", "12 oz can");
one12ozCan.SetConversion(12, sys.GetUOM(Unit.US_FLUID_OUNCE));

// 48 12 oz cans = 36 16 oz cans
Quantity q48 = new Quantity(48, one12ozCan);
Quantity q36 = q48.Convert(one16ozCan);

// 6 12 oz cans = 1 6-pack of 12 oz cans
UnitOfMeasure sixPackCan = sys.CreateScalarUOM(UnitType.VOLUME, "6-pack", "6PCan", "6-pack of 12 oz cans");
sixPackCan.SetConversion(6, one12ozCan);

// 1 case = 4 6-packs
UnitOfMeasure fourPackCase = sys.CreateScalarUOM(UnitType.VOLUME, "6-pack case", "4PCase", "four 6-packs");
fourPackCase.SetConversion(4, sixPackCan);
		
// A beer bottling line is rated at 2000 12 ounce cans/hour (US) at the
// filler. The case packer packs four 6-packs of cans into a case.
// Assuming no losses, what should be the rating of the case packer in
// cases per hour? And, what is the draw-down rate on the holding tank
// in gallons/minute?
UnitOfMeasure canph = sys.CreateQuotientUOM(one12ozCan, sys.GetHour());
UnitOfMeasure caseph = sys.CreateQuotientUOM(fourPackCase, sys.GetHour());
UnitOfMeasure gpm = sys.CreateQuotientUOM(sys.GetUOM(Unit.US_GALLON), sys.GetMinute());

// filler production rate
Quantity filler = new Quantity(2000, canph);

// tank draw-down
Quantity draw = filler.Convert(gpm);

// case packer production
Quantity packer = filler.Convert(caseph);
```

Quantities can be added, subtracted and converted:
```cs
UnitOfMeasure m = sys.getUOM(Unit.METRE);
UnitOfMeasure cm = sys.getUOM(Prefix.CENTI, m);
		
Quantity q1 = new Quantity("2", m);
Quantity q2 = new Quantity("2", cm);
		
// add two quantities.  q3 is 2.02 metre
Quantity q3 = q1.add(q2);
		
// q4 is 202 cm
Quantity q4 = q3.convert(cm);
		
// subtract q1 from q3 to get 0.02m
q3 = q3.subtract(q1);
```

as well as multiplied and divided:
```cs
UnitOfMeasure m = sys.GetUOM(Unit.METRE);
UnitOfMeasure cm = sys.GetUOM(Prefix.CENTI, m);

Quantity q1 = new Quantity(2, m);
Quantity q2 = new Quantity(2, cm);

// add two quantities.  q3 is 2.02 metre
Quantity q3 = q1.Add(q2);

// q4 is 202 cm
Quantity q4 = q3.Convert(cm);

// subtract q1 from q3 to get 0.02m
q3 = q3.Subtract(q1);
```

and inverted:
```cs
UnitOfMeasure mps = sys.GetUOM(Unit.METRE_PER_SEC);
Quantity q1 = new Quantity(10, mps);

// q2 = 0.1 sec/m
Quantity q2 = q1.Invert();
```

To make working with linearly scaled units of measure (with no offset) easier, the MeasurementSystem's GetUOM() using a Prefix can be used.  This method accepts a Prefix object and the unit of measure that it is scaled against.  The resulting unit of measure has a name concatented with the Prefix's name and target unit name.  The symbol is formed similarly.  For example, a centilitre (cL) is created from the pre-defined litre by:
```cs
UnitOfMeasure litre = sys.GetUOM(Unit.LITRE);
UnitOfMeasure cL = sys.GetUOM(Prefix.CENTI, litre);
```
and, a megabyte (MB = 2^20 bytes) is created by:
```cs
UnitOfMeasure mB = sys.GetUOM(Prefix.MEBI, Unit.BYTE);
```

*Implicit Conversions*

A quantity can be converted to another unit of measure without requiring the target UOM to first be created.  If the quantity has a product or quotient UOM, use the ConvertToPowerProduct() method.  For example:

```cs
// convert 1 newton-metre to pound force-inches
Quantity nmQ = new Quantity(1, sys.GetUOM(Unit.NEWTON_METRE));
Quantity lbfinQ = nmQ.ConvertToPowerProduct(sys.GetUOM(Unit.POUND_FORCE), sys.GetUOM(Unit.INCH));
```

If the quantity has power UOM, use the ConvertToPower() method.  For example:

```cs
// convert 1 square metre to square inches
Quantity m2Q = new Quantity(1, sys.GetUOM(Unit.SQUARE_METRE));
Quantity in2Q = m2Q.ConvertToPower(sys.GetUOM(Unit.INCH));
```

Other UOMs can be converted using the Convert() method.

## Physical Unit Equation Examples

One's Body Mass Index (BMI) can be calculated as:
```cs
Quantity height = new Quantity(2, Unit.METRE);
Quantity mass = new Quantity(100, Unit.KILOGRAM);
Quantity bmi = mass.Divide(height.Multiply(height));
```

Einstein's famous E = mc^2:
```cs
Quantity c = sys.GetQuantity(Constant.LIGHT_VELOCITY);
Quantity m = new Quantity(1, Unit.KILOGRAM);
Quantity e = m.Multiply(c).Multiply(c);
```

Ideal Gas Law, PV = nRT.  A cylinder of argon gas contains 50.0 L of Ar at 18.4 atm and 127 °C.  How many moles of argon are in the cylinder?
```cs
Quantity p = new Quantity(18.4, Unit.ATMOSPHERE).Convert(Unit.PASCAL);
Quantity v = new Quantity(50, Unit.LITRE).Convert(Unit.CUBIC_METRE);
Quantity t = new Quantity(127, Unit.CELSIUS).Convert(Unit.KELVIN);
Quantity n = p.Multiply(v).Divide(sys.GetQuantity(Constant.GAS_CONSTANT).Multiply(t));
```

Photon energy using Planck's constant:
```cs
// energy of red light photon = Planck's constant times the frequency
Quantity frequency = new Quantity(400, sys.GetUOM(Prefix.TERA, Unit.HERTZ));
Quantity ev = sys.GetQuantity(Constant.PLANCK_CONSTANT).Multiply(frequency).Convert(Unit.ELECTRON_VOLT);

// wavelength of red light in nanometres (approx 749.48)
Quantity frequency = new Quantity(400, sys.GetUOM(Prefix.TERA, Unit.HERTZ));
Quantity wavelength = sys.GetQuantity(Constant.LIGHT_VELOCITY).Divide(frequency).Convert(sys.GetUOM(Prefix.NANO, Unit.METRE));
```

Newton's second law of motion (F = ma). Weight of 1 kg in lbf:
```cs
Quantity mkg = new Quantity(1, Unit.KILOGRAM);
Quantity f = mkg.Multiply(sys.GetQuantity(Constant.GRAVITY)).Convert(Unit.POUND_FORCE);
```
Units per volume of solution, C = A x (m/V)
```cs
// create the "A" unit of measure
UnitOfMeasure activityUnit = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "activity", "act",
	"activity of material", sys.GetUOM(Unit.UNIT), sys.GetUOM(Prefix.MILLI, Unit.GRAM));

// calculate concentration
Quantity activity = new Quantity(1, activityUnit);
Quantity grams = new Quantity(1, Unit.GRAM).Convert(Prefix.MILLI, Unit.GRAM);
Quantity volume = new Quantity(1, sys.GetUOM(Prefix.MILLI, Unit.LITRE));
Quantity concentration = activity.Multiply(grams.Divide(volume));
Quantity katals = concentration.Multiply(new Quantity(1, Unit.LITRE)).Convert(Unit.KATAL);
```
Black body radiation:

```cs
// The Stefan-Boltzmann law states that the power emitted per unit area
// of the surface of a black body is directly proportional to the fourth
// power of its absolute temperature: sigma * T^4
// calculate at 1000 Kelvin
Quantity temp = new Quantity(1000, Unit.KELVIN);
Quantity intensity = sys.GetQuantity(Constant.STEFAN_BOLTZMANN).Multiply(temp.Power(4));
```

Expansion of the universe:

```cs
// Hubble's law, v = H0 x D. Let D = 10 Mpc
Quantity d = new Quantity(10, sys.GetUOM(Prefix.MEGA, sys.GetUOM(Unit.PARSEC)));
Quantity h0 = sys.GetQuantity(Constant.HUBBLE_CONSTANT);
Quantity velocity = h0.Multiply(d);
```

Device Characteristic Life

```cs
// A device has an activation energy of 0.5 and a characteristic life of
// 2,750 hours at an accelerated temperature of 150 degrees Celsius.
// Calculate the characteristic life at an expected use temperature of
// 85 degrees Celsius.

// Convert the Boltzman constant from J/K to eV/K for the Arrhenius equation
Quantity j = new Quantity(1, Unit.JOULE);
Quantity eV = j.Convert(Unit.ELECTRON_VOLT);
// Boltzmann constant
Quantity Kb = sys.GetQuantity(Constant.BOLTZMANN_CONSTANT).Multiply(eV.GetAmount());
// accelerated temperature
Quantity Ta = new Quantity(150, Unit.CELSIUS);
// expected use temperature
Quantity Tu = new Quantity(85, Unit.CELSIUS);
// calculate the acceleration factor
Quantity factor1 = Tu.Convert(Unit.KELVIN).Invert().Subtract(Ta.Convert(Unit.KELVIN).Invert());
Quantity factor2 = Kb.Invert().Multiply(0.5);
Quantity factor3 = factor1.Multiply(factor2);
double AF = Math.Exp(factor3.GetAmount());
// calculate longer life at expected use temperature
Quantity life85 = new Quantity(2750, Unit.HOUR);
Quantity life150 = life85.Multiply(AF);
```

## Financial Examples

Value of a stock portfolio:

```cs
// John has 100 shares of Alphabet Class A stock. How much is his
// portfolio worth in euros when the last trade was $838.96 and a US
// dollar is worth 0.94 euros?
UnitOfMeasure euro = sys.getUOM(Unit.EURO);
UnitOfMeasure usd = sys.getUOM(Unit.US_DOLLAR);
usd.setConversion("0.94", euro);

UnitOfMeasure googl = sys.CreateScalarUOM(UnitType.FINANCIAL, "Alphabet A", "GOOGL",
		"Alphabet (formerly Google) Class A shares");
googl.setConversion("838.96", usd);
Quantity portfolio = new Quantity("100", googl);
Quantity value = portfolio.convert(euro);
```

## Medical Examples

```cs
// Convert Unit to nanokatal
UnitOfMeasure u = sys.GetUOM(Unit.UNIT);
UnitOfMeasure katal = sys.GetUOM(Unit.KATAL);
Quantity q1 = new Quantity(1, u);
Quantity q2 = q1.Convert(sys.GetUOM(Prefix.NANO, katal));

// test result Equivalent
UnitOfMeasure eq = sys.GetUOM(Unit.EQUIVALENT);
UnitOfMeasure litre = sys.GetUOM(Unit.LITRE);
UnitOfMeasure mEqPerL = sys.CreateQuotientUOM(UnitType.MOLAR_CONCENTRATION, "milliNormal", "mEq/L",
	"solute per litre of solvent ", sys.GetUOM(Prefix.MILLI, eq), litre);
Quantity testResult = new Quantity(5.0, mEqPerL);

// blood cell count test results
UnitOfMeasure k = sys.GetUOM(Prefix.KILO, sys.GetOne());
UnitOfMeasure uL = sys.GetUOM(Prefix.MICRO, Unit.LITRE);
UnitOfMeasure kul = sys.CreateQuotientUOM(UnitType.MOLAR_CONCENTRATION, "K/uL", "K/uL",
	"thousands per microlitre", k, uL);
testResult = new Quantity(7.0, kul);

UnitOfMeasure fL = sys.GetUOM(Prefix.FEMTO, Unit.LITRE);
testResult = new Quantity(90, fL);

// TSH test result
UnitOfMeasure uIU = sys.GetUOM(Prefix.MICRO, Unit.INTERNATIONAL_UNIT);
UnitOfMeasure mL = sys.GetUOM(Prefix.MILLI, Unit.LITRE);
UnitOfMeasure uiuPerml = sys.CreateQuotientUOM(UnitType.MOLAR_CONCENTRATION, "uIU/mL", "uIU/mL",
	"micro IU per millilitre", uIU, mL);
testResult = new Quantity(2.0, uiuPerml);
```

### Caching
A unit of measure once created is registered in two dictionaries, one by its base symbol key and the second one by its enumeration key.  Caching greatly increases performance since the unit of measure is created only once.  Methods are provided to clear the cache of all instances as well as to unregister a particular instance.

The double value of a unit of measure conversion is also cached.  This performance optimization eliminates the need to calculate the conversion multiple times if many quantities are being converted at once; for example, operations upon a vector or matrix of quantities all with the same unit of measure.

## Localization
All externally visible text is defined in two .properties files.  The Unit.properties file has the name (.name), symbol (.symbol) and description (.desc) for a unit of measure as well as ToString() method text.  The Message.properties file has the text for an exception.  A default English file for each is included in the project.  The files can be edited to be used by another language, or the English version can be edited, e.g. to change "metre" to "meter".  For example, a metre's text is:

```cs
# metre
m.name = metre
m.symbol = m
m.desc = The length of the path travelled by light in vacuum during a time interval of 1/299792458 of a second.
```

and for an exception:
```cs
already.created = The unit of measure with symbol {0} has already been created by {1}.  Did you intend to scale this unit with a linear conversion?
```

## Project Structure
The CaliperSharp library depends on the .Net Framework 4+.  The unit tests depend on .Net Framework 4.6.1+.

The CaliperSharp library has the following structure:
 * `CaliperSharp` - CaliperSharp.csproj and the C# source files.  The calipersharp.doxygen is the Dosygen configuration file.
 * `./Resources` - contains the .properties files
 * `./Documentation` - Doxygen HTML docs