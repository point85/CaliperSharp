using Microsoft.VisualStudio.TestTools.UnitTesting;
using Point85.Caliper.UnitOfMeasure;
using System;

namespace CaliperSharpTests
{
	[TestClass]
	public class TestQuantity : BaseTest
	{
		[TestMethod]
		public void TestCurrencyConversion()
		{
			UnitOfMeasure usd_uom = sys.CreateScalarUOM(UnitType.CURRENCY, "US-Dollar", "USD", "US 'paper' dollar");
			UnitOfMeasure usdt_uom = sys.CreateScalarUOM(UnitType.CURRENCY, "Tether", "USDT", "USD 'stable' coin");

			// Initial conversion rate
			usdt_uom.SetConversion(0.9, usd_uom);

			Quantity portfolio = new Quantity(200, usdt_uom);
			Quantity portfolio_usd = portfolio.Convert(usd_uom);
			Assert.IsTrue(IsCloseTo(portfolio_usd.Amount, 180.0, DELTA6));

			// change conversion rate
			usdt_uom.SetConversion(1.0, usd_uom);
			portfolio_usd = portfolio.Convert(usd_uom);
			Assert.IsTrue(IsCloseTo(portfolio_usd.Amount, 200.0, DELTA6));
		}

		[TestMethod]
		public void TestNamedQuantity()
		{
			Quantity q = new Quantity(10, Unit.CELSIUS);
			Assert.IsTrue(q.ToString() != null);

			// faraday
			Quantity f = sys.GetQuantity(Constant.FARADAY_CONSTANT);
			Quantity qe = sys.GetQuantity(Constant.ELEMENTARY_CHARGE);
			Quantity na = sys.GetQuantity(Constant.AVOGADRO_CONSTANT);
			Quantity eNA = qe.Multiply(na);
			Assert.IsTrue(IsCloseTo(f.Amount, eNA.Amount, DELTA6));
			Assert.IsTrue(IsCloseTo(f.Amount, 96485.332123, DELTA5));

			// epsilon 0
			UnitOfMeasure fm = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "Farad per metre", "F/m", "Farad per metre",
					sys.GetUOM(Unit.FARAD), sys.GetUOM(Unit.METRE));
			Quantity eps0 = sys.GetQuantity(Constant.ELECTRIC_PERMITTIVITY);
			Assert.IsTrue(IsCloseTo(eps0.Amount, 8.854187817E-12, DELTA6));
			Assert.IsTrue(IsCloseTo(eps0.Convert(fm).Amount, 8.854187817E-12, DELTA6));

			// electron atomic mass
			Quantity u = new Quantity(1.66053904020E-24, sys.GetUOM(Unit.GRAM));
			Quantity me = sys.GetQuantity(Constant.ELECTRON_MASS);
			double bd = me.Divide(u).Amount;
			Assert.IsTrue(IsCloseTo(bd, 5.48579909016E-04, DELTA6));

			// proton
			Quantity mp = sys.GetQuantity(Constant.PROTON_MASS);
			bd = mp.Divide(u).Amount;
			Assert.IsTrue(IsCloseTo(bd, 1.00727646687991, DELTA6));

			// caesium
			Quantity cs = sys.GetQuantity(Constant.CAESIUM_FREQUENCY);
			Quantity periods = cs.Multiply(new Quantity(1, Unit.SECOND));
			Assert.IsTrue(IsCloseTo(periods.Amount, 9192631770d, DELTA0));

			// luminous efficacy
			Quantity kcd = sys.GetQuantity(Constant.LUMINOUS_EFFICACY);
			Quantity lum = kcd.Multiply(new Quantity(1, Unit.WATT));
			Assert.IsTrue(IsCloseTo(lum.Amount, 683d, DELTA0));
		}

		[TestMethod]
		public void TestAllUnits()
		{
			foreach (Unit u in Enum.GetValues(typeof(Unit)))
			{
				UnitOfMeasure uom1 = sys.GetUOM(u);
				UnitOfMeasure uom2 = sys.GetUOM(u);
				Assert.IsTrue(uom1.Equals(uom2));

				Quantity q1 = new Quantity(10, uom1);
				Quantity q2 = q1.Convert(uom2);
				Assert.IsTrue(q1.Equals(q2));
			}
		}

		[TestMethod]
		public void TestTime()
		{

			UnitOfMeasure second = sys.GetSecond();
			UnitOfMeasure minute = sys.GetMinute();

			Quantity oneMin = new Quantity(1, minute);
			Quantity oneSec = new Quantity(1, second);
			Quantity Converted = oneMin.Convert(second);
			double bd60 = 60;

			Assert.IsTrue(IsCloseTo(Converted.Amount, bd60, DELTA6));
			Assert.IsTrue(Converted.UOM.Equals(second));

			Quantity sixty = oneMin.Divide(oneSec);
			Assert.IsTrue(IsCloseTo(sixty.Amount, 1, DELTA6));
			Assert.IsTrue(IsCloseTo(sixty.UOM.ScalingFactor, bd60, DELTA6));

			Quantity q1 = sixty.Convert(sys.GetOne());
			Assert.IsTrue(q1.UOM.Equals(sys.GetOne()));
			Assert.IsTrue(IsCloseTo(q1.Amount, bd60, DELTA6));

			q1 = q1.Multiply(oneSec);
			Assert.IsTrue(q1.Convert(second).UOM.Equals(second));
			Assert.IsTrue(IsCloseTo(q1.Amount, bd60, DELTA6));

			q1 = q1.Convert(minute);
			Assert.IsTrue(q1.UOM.Equals(minute));
			Assert.IsTrue(IsCloseTo(q1.Amount, 1, DELTA6));

			Assert.IsTrue(q1.GetHashCode() != 0);

		}

		[TestMethod]
		public void TestTemperature()
		{

			UnitOfMeasure K = sys.GetUOM(Unit.KELVIN);
			UnitOfMeasure C = sys.GetUOM(Unit.CELSIUS);
			UnitOfMeasure R = sys.GetUOM(Unit.RANKINE);
			UnitOfMeasure F = sys.GetUOM(Unit.FAHRENHEIT);

			double bd212 = 212;
			double oneHundred = 100;

			Quantity q1 = new Quantity(bd212, F);
			Quantity q2 = q1.Convert(C);
			Assert.IsTrue(IsCloseTo(q2.Amount, oneHundred, DELTA6));
			Assert.IsTrue(IsCloseTo(q2.Convert(F).Amount, bd212, DELTA6));

			double bd32 = 32;
			q1 = new Quantity(bd32, F);
			q2 = q1.Convert(C);
			Assert.IsTrue(IsCloseTo(q2.Amount, 0, DELTA6));
			Assert.IsTrue(IsCloseTo(q2.Convert(F).Amount, bd32, DELTA6));

			q1 = new Quantity(0, F);
			q2 = q1.Convert(C);
			Assert.IsTrue(IsCloseTo(q2.Amount, -17.7777777777778, DELTA6));
			Assert.IsTrue(IsCloseTo(q2.Convert(F).Amount, 0, DELTA6));

			double bd459 = 459.67;
			q1 = new Quantity(bd459, R);
			q2 = q1.Convert(F);
			Assert.IsTrue(IsCloseTo(q2.Amount, 0, DELTA6));
			Assert.IsTrue(IsCloseTo(q2.Convert(R).Amount, bd459, DELTA6));

			double bd255 = 255.3722222222222;
			q2 = q1.Convert(K);
			Assert.IsTrue(IsCloseTo(q2.Amount, bd255, DELTA6));
			Assert.IsTrue(IsCloseTo(q2.Convert(R).Amount, bd459, DELTA6));

			double bd17 = -17.7777777777778;
			q2 = q1.Convert(C);
			Assert.IsTrue(IsCloseTo(q2.Amount, bd17, DELTA6));
			Assert.IsTrue(IsCloseTo(q2.Convert(R).Amount, bd459, DELTA6));

			double bd273 = 273.15;
			q1 = new Quantity(bd273, K);
			q2 = q1.Convert(C);
			Assert.IsTrue(IsCloseTo(q2.Amount, 0, DELTA6));
			Assert.IsTrue(IsCloseTo(q2.Convert(K).Amount, bd273, DELTA6));

			q1 = new Quantity(0, K);
			q2 = q1.Convert(R);
			Assert.IsTrue(IsCloseTo(q2.Amount, 0, DELTA6));
			Assert.IsTrue(IsCloseTo(q2.Convert(K).Amount, 0, DELTA6));
		}

		[TestMethod]
		public void TestLength()
		{

			UnitOfMeasure m = sys.GetUOM(Unit.METRE);
			UnitOfMeasure cm = sys.GetUOM(Prefix.CENTI, m);
			UnitOfMeasure m2 = sys.GetUOM(Unit.SQUARE_METRE);

			const char squared = (char)0xB2;
			String cmsym = "cm" + squared;
			UnitOfMeasure cm2 = sys.GetUOM(cmsym);

			if (cm2 == null)
			{
				cm2 = sys.CreatePowerUOM(UnitType.AREA, "square centimetres", cmsym, "centimetres squared", cm, 2);
			}

			UnitOfMeasure ft = sys.GetUOM(Unit.FOOT);
			UnitOfMeasure yd = sys.GetUOM(Unit.YARD);
			UnitOfMeasure ft2 = sys.GetUOM(Unit.SQUARE_FOOT);
			UnitOfMeasure in2 = sys.GetUOM(Unit.SQUARE_INCH);

			double oneHundred = 100;

			Quantity q1 = new Quantity(1, ft2);
			Quantity q2 = q1.Convert(in2);
			Assert.IsTrue(IsCloseTo(q2.Amount, 144, DELTA6));
			Assert.IsTrue(IsCloseTo(q2.Convert(ft2).Amount, 1, DELTA6));

			q1 = new Quantity(1, sys.GetUOM(Unit.SQUARE_METRE));
			q2 = q1.Convert(ft2);
			Assert.IsTrue(IsCloseTo(q2.Amount, 10.76391041670972, DELTA6));
			Assert.IsTrue(IsCloseTo(q2.Convert(m2).Amount, 1, DELTA6));

			double bd = 3;
			q1 = new Quantity(bd, ft);
			q2 = q1.Convert(yd);
			Assert.IsTrue(IsCloseTo(q2.Amount, 1, DELTA6));
			Assert.IsTrue(IsCloseTo(q2.Convert(ft).Amount, bd, DELTA6));

			bd = 0.3048;
			q1 = new Quantity(1, ft);
			q2 = q1.Convert(m);
			Assert.IsTrue(IsCloseTo(q2.Amount, bd, DELTA6));
			Assert.IsTrue(IsCloseTo(q2.Convert(ft).Amount, 1, DELTA6));

			bd = oneHundred;
			q1 = new Quantity(bd, cm);
			q2 = q1.Convert(m);
			Assert.IsTrue(IsCloseTo(q2.Amount, 1, DELTA6));
			Assert.IsTrue(IsCloseTo(q2.Convert(cm).Amount, bd, DELTA6));

			// Add
			bd = 50;
			q1 = new Quantity(bd, cm);
			q2 = new Quantity(bd, cm);
			Quantity q3 = q1.Add(q2);
			Assert.IsTrue(IsCloseTo(q3.Amount, bd + bd, DELTA6));
			Assert.IsTrue(IsCloseTo(q3.Convert(m).Amount, 1, DELTA6));

			Quantity q4 = q2.Add(q1);
			Assert.IsTrue(IsCloseTo(q4.Amount, bd + bd, DELTA6));
			Assert.IsTrue(IsCloseTo(q4.Convert(m).Amount, 1, DELTA6));
			Assert.IsTrue(q3.Equals(q4));

			// Subtract
			q3 = q1.Subtract(q2);
			Assert.IsTrue(IsCloseTo(q3.Amount, 0, DELTA6));
			Assert.IsTrue(IsCloseTo(q3.Convert(m).Amount, 0, DELTA6));

			q4 = q2.Subtract(q1);
			Assert.IsTrue(IsCloseTo(q4.Amount, 0, DELTA6));
			Assert.IsTrue(IsCloseTo(q4.Convert(m).Amount, 0, DELTA6));
			Assert.IsTrue(q3.Equals(q4));

			// Multiply
			q3 = q1.Multiply(q2);
			Assert.IsTrue(IsCloseTo(q3.Amount, 2500, DELTA6));

			q4 = q3.Convert(cm2);
			Assert.IsTrue(IsCloseTo(q4.Amount, 2500, DELTA6));

			q4 = q3.Convert(m2);
			Assert.IsTrue(IsCloseTo(q4.Amount, 0.25, DELTA6));

			// Divide
			q4 = q3.Divide(q1);
			Assert.IsTrue(q4.Equals(q2));

		}

		[TestMethod]
		public void TestUSQuantity()
		{
			UnitOfMeasure gal = sys.GetUOM(Unit.US_GALLON);
			UnitOfMeasure in3 = sys.GetUOM(Unit.CUBIC_INCH);
			UnitOfMeasure floz = sys.GetUOM(Unit.US_FLUID_OUNCE);
			UnitOfMeasure qt = sys.GetUOM(Unit.US_QUART);

			Quantity q1 = new Quantity(10, gal);
			Quantity q2 = q1.Convert(in3);
			Assert.IsTrue(IsCloseTo(q2.Amount, 2310, DELTA6));
			Assert.IsTrue(q2.UOM.Equals(in3));

			q1 = new Quantity(128, floz);
			q2 = q1.Convert(qt);
			Assert.IsTrue(IsCloseTo(q2.Amount, 4, DELTA6));
			Assert.IsTrue(q2.UOM.Equals(qt));

			UnitOfMeasure ft = sys.GetUOM(Unit.FOOT);
			UnitOfMeasure inch = sys.GetUOM(Unit.INCH);
			UnitOfMeasure mi = sys.GetUOM(Unit.MILE);

			q1 = new Quantity(10, ft);
			q2 = q1.Convert(inch);

			q1 = new Quantity(1, mi);

			// British cup to US gallon
			q1 = new Quantity(10, sys.GetUOM(Unit.BR_CUP));
			q2 = q1.Convert(sys.GetUOM(Unit.US_GALLON));
			Assert.IsTrue(IsCloseTo(q2.Amount, 0.6, DELTA3));

			// US ton to British ton
			q1 = new Quantity(10, sys.GetUOM(Unit.US_TON));
			q2 = q1.Convert(sys.GetUOM(Unit.BR_TON));
			Assert.IsTrue(IsCloseTo(q2.Amount, 8.928571428, DELTA6));

			// troy ounce to ounce
			q1 = new Quantity(10, Unit.TROY_OUNCE);
			Assert.IsTrue(IsCloseTo(q1.Convert(Unit.OUNCE).Amount, 10.971, DELTA3));

			// deci-litre to quart
			q1 = new Quantity(10, Prefix.DECI, Unit.LITRE);
			q2 = q1.Convert(Unit.US_QUART);
			Assert.IsTrue(IsCloseTo(q2.Amount, 1.0566882, DELTA6));
		}

		[TestMethod]
		public void TestSIQuantity()
		{

			double ten = 10;

			UnitOfMeasure litre = sys.GetUOM(Unit.LITRE);
			UnitOfMeasure m3 = sys.GetUOM(Unit.CUBIC_METRE);
			UnitOfMeasure m2 = sys.GetUOM(Unit.SQUARE_METRE);
			UnitOfMeasure m = sys.GetUOM(Unit.METRE);
			UnitOfMeasure cm = sys.GetUOM(Prefix.CENTI, m);
			UnitOfMeasure mps = sys.GetUOM(Unit.METRE_PER_SEC);
			UnitOfMeasure secPerM = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, null, "s/m", null, sys.GetSecond(), m);
			UnitOfMeasure oneOverM = sys.GetUOM(Unit.DIOPTER);
			UnitOfMeasure fperm = sys.GetUOM(Unit.FARAD_PER_METRE);

			UnitOfMeasure oneOverCm = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, null, "1/cm", null);
			oneOverCm.SetConversion(100, oneOverM);

			Quantity q1 = new Quantity(ten, litre);
			Quantity q2 = q1.Convert(m3);
			Assert.IsTrue(IsCloseTo(q2.Amount, 0.01, DELTA6));
			Assert.IsTrue(q2.UOM.Equals(m3));

			q2 = q1.Convert(litre);
			Assert.IsTrue(IsCloseTo(q2.Amount, ten, DELTA6));
			Assert.IsTrue(q2.UOM.Equals(litre));

			// Add
			q1 = new Quantity(2, m);
			q2 = new Quantity(2, cm);
			Quantity q3 = q1.Add(q2);

			Assert.IsTrue(IsCloseTo(q3.UOM.ScalingFactor, 1, DELTA6));
			Assert.IsTrue(q3.UOM.AbscissaUnit.Equals(m));
			Assert.IsTrue(IsCloseTo(q3.UOM.Offset, 0, DELTA6));
			Assert.IsTrue(IsCloseTo(q3.Amount, 2.02, DELTA6));

			Quantity q4 = q3.Convert(cm);
			Assert.IsTrue(IsCloseTo(q4.Amount, 202, DELTA6));
			Assert.IsTrue(q4.UOM.Equals(cm));

			// Subtract
			q3 = q3.Subtract(q1);
			Assert.IsTrue(IsCloseTo(q3.UOM.ScalingFactor, 1, DELTA6));
			Assert.IsTrue(q3.UOM.AbscissaUnit.Equals(m));
			Assert.IsTrue(IsCloseTo(q3.Amount, 0.02, DELTA6));

			q4 = q3.Convert(cm);
			Assert.IsTrue(IsCloseTo(q4.Amount, 2, DELTA6));
			Assert.IsTrue(q4.UOM.Equals(cm));

			// Multiply
			q3 = q1.Multiply(q2);
			Assert.IsTrue(IsCloseTo(q3.Amount, 4, DELTA6));
			UnitOfMeasure u = q3.UOM;
			Assert.IsTrue(IsCloseTo(u.ScalingFactor, 0.01, DELTA6));
			Assert.IsTrue(u.GetBaseSymbol().Equals(m2.GetBaseSymbol()));

			q4 = q3.Divide(q3);
			Assert.IsTrue(IsCloseTo(q4.Amount, 1, DELTA6));
			Assert.IsTrue(q4.UOM.Equals(sys.GetOne()));

			q4 = q3.Divide(q1);
			Assert.IsTrue(q4.Equals(q2));

			q4 = q3.Convert(m2);
			Assert.IsTrue(IsCloseTo(q4.Amount, 0.04, DELTA6));
			Assert.IsTrue(q4.UOM.Equals(m2));

			// Divide
			q3 = q3.Divide(q2);
			Assert.IsTrue(IsCloseTo(q3.Amount, 2.0, DELTA6));
			Assert.IsTrue(q3.UOM.Equals(m));
			Assert.IsTrue(q3.Equals(q1));

			q3 = q3.Convert(m);
			Assert.IsTrue(IsCloseTo(q3.Amount, 2.0, DELTA6));

			q1 = new Quantity(0, litre);

			try
			{
				q2 = q1.Divide(q1);
				Assert.Fail("Divide by zero)");
			}
			catch (Exception)
			{
			}

			q1 = q3.Convert(cm).Divide(ten);
			Assert.IsTrue(IsCloseTo(q1.Amount, 20, DELTA6));

			// Invert
			q1 = new Quantity(10, mps);
			q2 = q1.Invert();
			Assert.IsTrue(IsCloseTo(q2.Amount, 0.1, DELTA6));
			Assert.IsTrue(q2.UOM.Equals(secPerM));

			q2 = q2.Invert();
			Assert.IsTrue(IsCloseTo(q2.Amount, 10, DELTA6));
			Assert.IsTrue(q2.UOM.Equals(mps));

			q1 = new Quantity(10, cm);
			q2 = q1.Invert();
			Assert.IsTrue(IsCloseTo(q2.Amount, 0.1, DELTA6));
			u = q2.UOM;
			Assert.IsTrue(u.Equals(oneOverCm));

			q2 = q2.Convert(m.Invert());
			Assert.IsTrue(IsCloseTo(q2.Amount, 10, DELTA6));
			Assert.IsTrue(q2.UOM.Equals(oneOverM));

			Assert.IsTrue(q2.ToString() != null);

			// Newton-metres Divided by metres
			q1 = new Quantity(10, sys.GetUOM(Unit.NEWTON_METRE));
			q2 = new Quantity(1, sys.GetUOM(Unit.METRE));
			q3 = q1.Divide(q2);
			Assert.IsTrue(IsCloseTo(q3.Amount, 10, DELTA6));
			Assert.IsTrue(q3.UOM.Equals(sys.GetUOM(Unit.NEWTON)));

			// length multiplied by force
			q1 = new Quantity(10, sys.GetUOM(Unit.NEWTON));
			q2 = new Quantity(1, sys.GetUOM(Unit.METRE));
			q3 = q1.Multiply(q2);
			Assert.IsTrue(IsCloseTo(q3.Amount, 10, DELTA6));
			UnitOfMeasure nm1 = q3.UOM;
			UnitOfMeasure nm2 = sys.GetUOM(Unit.NEWTON_METRE);
			Assert.IsTrue(nm1.GetBaseSymbol().Equals(nm2.GetBaseSymbol()));
			q4 = q3.Convert(sys.GetUOM(Unit.JOULE));
			Assert.IsTrue(q4.UOM.Equals(sys.GetUOM(Unit.JOULE)));

			// farads
			q1 = new Quantity(10, fperm);
			q2 = new Quantity(1, m);
			q3 = q1.Multiply(q2);
			Assert.IsTrue(IsCloseTo(q3.Amount, 10, DELTA6));
			Assert.IsTrue(q3.UOM.Equals(sys.GetUOM(Unit.FARAD)));

			// amps
			q1 = new Quantity(10, sys.GetUOM(Unit.AMPERE_PER_METRE));
			q2 = new Quantity(1, m);
			q3 = q1.Multiply(q2);
			Assert.IsTrue(IsCloseTo(q3.Amount, 10, DELTA6));
			Assert.IsTrue(q3.UOM.Equals(sys.GetUOM(Unit.AMPERE)));

			// Boltzmann and Avogadro
			Quantity boltzmann = sys.GetQuantity(Constant.BOLTZMANN_CONSTANT);
			Quantity avogadro = sys.GetQuantity(Constant.AVOGADRO_CONSTANT);
			Quantity gas = sys.GetQuantity(Constant.GAS_CONSTANT);
			Quantity qR = boltzmann.Multiply(avogadro);
			Assert.IsTrue(IsCloseTo(qR.UOM.ScalingFactor, gas.UOM.ScalingFactor, DELTA6));

			// Sieverts
			q1 = new Quantity(20, sys.GetUOM(Prefix.MILLI, Unit.SIEVERTS_PER_HOUR));
			q2 = new Quantity(24, sys.GetHour());
			q3 = q1.Multiply(q2);
			Assert.IsTrue(IsCloseTo(q3.Amount, 480, DELTA6));

		}

		[TestMethod]
		public void TestPowers()
		{

			UnitOfMeasure m2 = sys.GetUOM(Unit.SQUARE_METRE);
			UnitOfMeasure p2 = sys.CreatePowerUOM(UnitType.AREA, "m2^1", "m2^1", "square metres raised to power 1", m2, 1);
			UnitOfMeasure p4 = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "m2^2", "m2^2", "square metres raised to power 2",
						m2, 2);

			double amount = 10;

			Quantity q1 = new Quantity(amount, m2);
			Quantity q3 = new Quantity(amount, p4);

			Quantity q4 = q3.Divide(q1);
			Assert.IsTrue(IsCloseTo(q4.Amount, 1, DELTA6));
			Assert.IsTrue(q4.UOM.GetBaseUOM().Equals(m2));

			Quantity q2 = q1.Convert(p2);
			Assert.IsTrue(IsCloseTo(q2.Amount, amount, DELTA6));
			Assert.IsTrue(q2.UOM.GetBaseUOM().Equals(m2));

			// power method
			UnitOfMeasure ft = sys.GetUOM(Unit.FOOT);
			UnitOfMeasure ft2 = sys.GetUOM(Unit.SQUARE_FOOT);
			q1 = new Quantity(10, ft);

			q3 = q1.Power(2);
			Assert.IsTrue(IsCloseTo(q3.Amount, 100, DELTA6));
			Assert.IsTrue(q3.UOM.GetBaseSymbol().Equals(ft2.GetBaseSymbol()));

			q4 = q3.Convert(sys.GetUOM(Unit.SQUARE_METRE));
			Assert.IsTrue(IsCloseTo(q4.Amount, 9.290304, DELTA6));

			q3 = q1.Power(1);
			Assert.IsTrue(q3.Amount.Equals(q1.Amount));
			Assert.IsTrue(q3.UOM.GetBaseSymbol().Equals(q1.UOM.GetBaseSymbol()));

			q3 = q1.Power(0);
			Assert.IsTrue(q3.Amount.Equals(1));
			Assert.IsTrue(q3.UOM.GetBaseSymbol().Equals(sys.GetOne().GetBaseSymbol()));

			q3 = q1.Power(-1);
			Assert.IsTrue(q3.Amount.Equals(0.1));
			Assert.IsTrue(q3.UOM.Equals(ft.Invert()));

			q3 = q1.Power(-2);
			Assert.IsTrue(q3.Amount.Equals(0.01));
			Assert.IsTrue(q3.UOM.Equals(ft2.Invert()));
		}

		[TestMethod]
		public void TestSIUnits()
		{
			sys.ClearCache();

			UnitOfMeasure newton = sys.GetUOM(Unit.NEWTON);
			UnitOfMeasure metre = sys.GetUOM(Unit.METRE);
			UnitOfMeasure m2 = sys.GetUOM(Unit.SQUARE_METRE);
			UnitOfMeasure cm = sys.GetUOM(Prefix.CENTI, metre);
			UnitOfMeasure mps = sys.GetUOM(Unit.METRE_PER_SEC);
			UnitOfMeasure joule = sys.GetUOM(Unit.JOULE);
			UnitOfMeasure m3 = sys.GetUOM(Unit.CUBIC_METRE);
			UnitOfMeasure farad = sys.GetUOM(Unit.FARAD);
			UnitOfMeasure nm = sys.GetUOM(Unit.NEWTON_METRE);
			UnitOfMeasure coulomb = sys.GetUOM(Unit.COULOMB);
			UnitOfMeasure volt = sys.GetUOM(Unit.VOLT);
			UnitOfMeasure watt = sys.GetUOM(Unit.WATT);
			UnitOfMeasure cm2 = sys.CreateProductUOM(UnitType.AREA, "square centimetres", "cm" + (char)0xB2, "", cm, cm);
			UnitOfMeasure cv = sys.CreateProductUOM(UnitType.ENERGY, "CxV", "C·V", "Coulomb times Volt", coulomb, volt);
			UnitOfMeasure ws = sys.CreateProductUOM(UnitType.ENERGY, "Wxs", "W·s", "Watt times second", watt,
						sys.GetSecond());
			UnitOfMeasure ft3 = sys.GetUOM(Unit.CUBIC_FOOT);
			UnitOfMeasure hz = sys.GetUOM(Unit.HERTZ);

			double oneHundred = 100;

			Assert.IsTrue(nm.GetBaseSymbol().Equals(joule.GetBaseSymbol()));
			Assert.IsTrue(cv.GetBaseSymbol().Equals(joule.GetBaseSymbol()));
			Assert.IsTrue(ws.GetBaseSymbol().Equals(joule.GetBaseSymbol()));

			Quantity q1 = new Quantity(10, newton);
			Quantity q2 = new Quantity(10, metre);
			Quantity q3 = q1.Multiply(q2);
			Assert.IsTrue(IsCloseTo(q3.Amount, oneHundred, DELTA6));
			Assert.IsTrue(q3.UOM.GetBaseSymbol().Equals(nm.GetBaseSymbol()));
			Assert.IsTrue(IsCloseTo(q3.UOM.ScalingFactor, 1, DELTA6));
			Assert.IsTrue(IsCloseTo(q3.UOM.Offset, 0, DELTA6));

			q3 = q3.Convert(joule);
			Assert.IsTrue(IsCloseTo(q3.Amount, oneHundred, DELTA6));
			Assert.IsTrue(q3.UOM.Equals(joule));
			Assert.IsTrue(IsCloseTo(q3.UOM.ScalingFactor, 1, DELTA6));

			q3 = q3.Convert(nm);
			Assert.IsTrue(IsCloseTo(q3.Amount, oneHundred, DELTA6));
			Assert.IsTrue(q3.UOM.Equals(nm));
			Assert.IsTrue(IsCloseTo(q3.UOM.ScalingFactor, 1, DELTA6));

			double bd1 = 10000;

			q1 = new Quantity(oneHundred, cm);
			q2 = q1.Convert(metre);
			Assert.IsTrue(IsCloseTo(q2.Amount, 1, DELTA6));
			Assert.IsTrue(q2.UOM.Enumeration.Equals(Unit.METRE));
			Assert.IsTrue(IsCloseTo(q2.UOM.ScalingFactor, 1, DELTA6));

			q2 = q2.Convert(cm);
			Assert.IsTrue(IsCloseTo(q2.Amount, oneHundred, DELTA6));
			Assert.IsTrue(IsCloseTo(q2.UOM.ScalingFactor, 0.01, DELTA6));

			q2 = q1;
			q3 = q1.Multiply(q2);
			Assert.IsTrue(IsCloseTo(q3.Amount, 10000, DELTA6));
			Assert.IsTrue(IsCloseTo(q3.UOM.ScalingFactor, 0.0001, DELTA6));
			Assert.IsTrue(IsCloseTo(q3.UOM.Offset, 0, DELTA6));

			Quantity q4 = q3.Convert(m2);
			Assert.IsTrue(q4.UOM.Equals(m2));
			Assert.IsTrue(IsCloseTo(q4.Amount, 1, DELTA6));

			q3 = q3.Convert(m2);
			Assert.IsTrue(IsCloseTo(q3.Amount, 1, DELTA6));
			Assert.IsTrue(q3.UOM.Equals(m2));
			Assert.IsTrue(IsCloseTo(q3.UOM.ScalingFactor, 1, DELTA6));

			q3 = q3.Convert(cm2);
			Assert.IsTrue(IsCloseTo(q3.Amount, bd1, DELTA6));
			Assert.IsTrue(q3.UOM.Equals(cm2));
			Assert.IsTrue(IsCloseTo(q3.UOM.ScalingFactor, 1, DELTA6));

			// power
			Quantity onem3 = new Quantity(1, m3);
			String cm3sym = "cm" + (char)0xB3;
			UnitOfMeasure cm3 = sys.CreatePowerUOM(UnitType.VOLUME, cm3sym, cm3sym, null, cm, 3);
			Quantity megcm3 = new Quantity(1E+06, cm3);

			Quantity qft3 = onem3.Convert(ft3);
			Assert.IsTrue(IsCloseTo(qft3.Amount, 35.31466672148859, DELTA6));

			Quantity qtym3 = qft3.Convert(m3);
			Assert.IsTrue(IsCloseTo(qtym3.Amount, 1, DELTA6));

			Quantity qm3 = megcm3.Convert(m3);
			Assert.IsTrue(IsCloseTo(qm3.Amount, 1, DELTA6));
			qm3 = qm3.Convert(cm3);
			Assert.IsTrue(IsCloseTo(qm3.Amount, 1E+06, DELTA6));

			Quantity qcm3 = onem3.Convert(cm3);
			Assert.IsTrue(IsCloseTo(qcm3.Amount, 1E+06, DELTA6));

			// inversions
			UnitOfMeasure u = metre.Invert();
			string sym = u.AbscissaUnit.Symbol;
			string diop = sys.GetUOM(Unit.DIOPTER).Symbol;
			Assert.IsFalse(sym.Equals(diop));

			u = mps.Invert();
			Assert.IsTrue(u.Symbol.Equals("s/m"));

			UnitOfMeasure uom = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "1/F", "1/F", "one over farad", sys.GetOne(),
					farad);
			Assert.IsTrue(uom.Symbol.Equals("1/F"));

			// hz to radians per sec
			q1 = new Quantity(10, sys.GetUOM(Unit.HERTZ));
			q2 = q1.Convert(sys.GetUOM(Unit.RAD_PER_SEC));
			double twentyPi = (double)20 * Math.PI;
			Assert.IsTrue(IsCloseTo(q2.Amount, twentyPi, DELTA6));

			q3 = q2.Convert(sys.GetUOM(Unit.HERTZ));
			Assert.IsTrue(IsCloseTo(q3.Amount, 10, DELTA6));

			// rpm to radians per second
			q1 = new Quantity(10, sys.GetUOM(Unit.REV_PER_MIN));
			q2 = q1.Convert(sys.GetUOM(Unit.RAD_PER_SEC));
			Assert.IsTrue(IsCloseTo(q2.Amount, 1.04719755119, DELTA6));

			q3 = q2.Convert(sys.GetUOM(Unit.REV_PER_MIN));
			Assert.IsTrue(IsCloseTo(q3.Amount, 10, DELTA6));

			q1 = new Quantity(10, hz);
			q2 = new Quantity(1, sys.GetMinute());
			q3 = q1.Multiply(q2).Convert(sys.GetOne());
			Assert.IsTrue(IsCloseTo(q3.Amount, 600, DELTA6));

			q1 = new Quantity(1, sys.GetUOM(Unit.ELECTRON_VOLT));
			q2 = q1.Convert(sys.GetUOM(Unit.JOULE));
			Assert.IsTrue(IsCloseTo(q2.Amount, 1.60217656535E-19, DELTA6));

		}

		[TestMethod]
		public void TestEquations()
		{
			// body mass index
			Quantity height = new Quantity(2, sys.GetUOM(Unit.METRE));
			Quantity mass = new Quantity(100, sys.GetUOM(Unit.KILOGRAM));
			Quantity bmi = mass.Divide(height.Multiply(height));
			Assert.IsTrue(IsCloseTo(bmi.Amount, 25, DELTA6));

			// E = mc^2
			Quantity c = sys.GetQuantity(Constant.LIGHT_VELOCITY);
			Quantity m = new Quantity(1, sys.GetUOM(Unit.KILOGRAM));
			Quantity e = m.Multiply(c).Multiply(c);
			Assert.IsTrue(IsCloseTo(e.Amount,
					8.987551787368176E+16, 1));

			// Ideal Gas Law, PV = nRT
			// A cylinder of argon gas contains 50.0 L of Ar at 18.4 atm and 127 °C.
			// How many moles of argon are in the cylinder?
			Quantity p = new Quantity(18.4, sys.GetUOM(Unit.ATMOSPHERE)).Convert(Unit.PASCAL);
			Quantity v = new Quantity(50, Unit.LITRE).Convert(Unit.CUBIC_METRE);
			Quantity t = new Quantity(127, Unit.CELSIUS).Convert(Unit.KELVIN);
			Quantity n = p.Multiply(v).Divide(sys.GetQuantity(Constant.GAS_CONSTANT).Multiply(t));
			Assert.IsTrue(IsCloseTo(n.Amount, 28.018664, DELTA6));

			// energy of red light photon = Planck's constant times the frequency
			Quantity frequency = new Quantity(400, sys.GetUOM(Prefix.TERA, Unit.HERTZ));
			Quantity ev = sys.GetQuantity(Constant.PLANCK_CONSTANT).Multiply(frequency).Convert(Unit.ELECTRON_VOLT);
			Assert.IsTrue(IsCloseTo(ev.Amount, 1.65, DELTA2));

			// wavelength of red light in nanometres
			Quantity wavelength = sys.GetQuantity(Constant.LIGHT_VELOCITY).Divide(frequency)
					.Convert(sys.GetUOM(Prefix.NANO, Unit.METRE));
			Assert.IsTrue(IsCloseTo(wavelength.Amount, 749.48, DELTA2));

			// Newton's second law of motion (F = ma). Weight of 1 kg in lbf
			Quantity mkg = new Quantity(1, Unit.KILOGRAM);
			Quantity f = mkg.Multiply(sys.GetQuantity(Constant.GRAVITY)).Convert(Unit.POUND_FORCE);
			Assert.IsTrue(IsCloseTo(f.Amount, 2.20462, DELTA5));

			// units per volume of solution, C = A x (m/V)
			// create the "A" unit of measure
			UnitOfMeasure activityUnit = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "activity", "act",
					"activity of material", sys.GetUOM(Unit.UNIT), sys.GetUOM(Prefix.MILLI, Unit.GRAM));

			// calculate concentration
			Quantity activity = new Quantity(1, activityUnit);
			Quantity grams = new Quantity(1, Unit.GRAM).Convert(Prefix.MILLI, Unit.GRAM);
			Quantity volume = new Quantity(1, sys.GetUOM(Prefix.MILLI, Unit.LITRE));
			Quantity concentration = activity.Multiply(grams.Divide(volume));
			Assert.IsTrue(IsCloseTo(concentration.Amount, 1000, DELTA6));

			Quantity katals = concentration.Multiply(new Quantity(1, Unit.LITRE)).Convert(Unit.KATAL);
			Assert.IsTrue(IsCloseTo(katals.Amount, 0.01666667, DELTA6));

			// The Stefan–Boltzmann law states that the power emitted per unit area
			// of the surface of a black body is directly proportional to the fourth
			// power of its absolute temperature: sigma * T^4

			// calculate at 1000 Kelvin
			Quantity temp = new Quantity(1000, Unit.KELVIN);
			Quantity intensity = sys.GetQuantity(Constant.STEFAN_BOLTZMANN).Multiply(temp.Power(4));
			Assert.IsTrue(IsCloseTo(intensity.Amount, 56703.67, DELTA2));

			// Hubble's law, v = H0 x D. Let D = 10 Mpc
			Quantity d = new Quantity(10, sys.GetUOM(Prefix.MEGA, sys.GetUOM(Unit.PARSEC)));
			Quantity h0 = sys.GetQuantity(Constant.HUBBLE_CONSTANT);
			Quantity velocity = h0.Multiply(d);
			Assert.IsTrue(IsCloseTo(velocity.Amount, 719, DELTA3));

			// Arrhenius equation
			// A device has an activation energy of 0.5 and a characteristic life of
			// 2,750 hours at an accelerated temperature of 150 degrees Celsius.
			// Calculate the characteristic life at an expected use temperature of
			// 85 degrees Celsius.

			// Convert the Boltzman constant from J/K to eV/K for the Arrhenius
			// equation
			// eV per Joule
			Quantity j = new Quantity(1, Unit.JOULE);
			Quantity eV = j.Convert(Unit.ELECTRON_VOLT);
			// Boltzmann constant
			Quantity Kb = sys.GetQuantity(Constant.BOLTZMANN_CONSTANT).Multiply(eV.Amount);
			// accelerated temperature
			Quantity Ta = new Quantity(150, Unit.CELSIUS);
			// expected use temperature
			Quantity Tu = new Quantity(85, Unit.CELSIUS);
			// calculate the acceleration factor
			Quantity factor1 = Tu.Convert(Unit.KELVIN).Invert().Subtract(Ta.Convert(Unit.KELVIN).Invert());
			Quantity factor2 = Kb.Invert().Multiply(0.5);
			Quantity factor3 = factor1.Multiply(factor2);
			double AF = Math.Exp(factor3.Amount);
			// calculate longer life at expected use temperature
			Quantity life85 = new Quantity(2750, Unit.HOUR);
			Quantity life150 = life85.Multiply(AF);
			Assert.IsTrue(IsCloseTo(life150.Amount, 33121.4, DELTA1));
		}

		[TestMethod]
		public void TestPackaging()
		{
			double one = 1;
			double four = 4;
			double six = 6;
			double ten = 10;
			double forty = 40;

			UnitOfMeasure one16ozCan = sys.CreateScalarUOM(UnitType.VOLUME, "16 oz can", "16ozCan", "16 oz can");
			one16ozCan.SetConversion(16, sys.GetUOM(Unit.US_FLUID_OUNCE));

			Quantity q400 = new Quantity(400, one16ozCan);
			Quantity q50 = q400.Convert(sys.GetUOM(Unit.US_GALLON));
			Assert.IsTrue(IsCloseTo(q50.Amount, 50, DELTA6));

			// 1 12 oz can = 12 fl.oz.
			UnitOfMeasure one12ozCan = sys.CreateScalarUOM(UnitType.VOLUME, "12 oz can", "12ozCan", "12 oz can");
			one12ozCan.SetConversion(12, sys.GetUOM(Unit.US_FLUID_OUNCE));

			Quantity q48 = new Quantity(48, one12ozCan);
			Quantity q36 = q48.Convert(one16ozCan);
			Assert.IsTrue(IsCloseTo(q36.Amount, 36, DELTA6));

			// 6 12 oz cans = 1 6-pack of 12 oz cans
			UnitOfMeasure sixPackCan = sys.CreateScalarUOM(UnitType.VOLUME, "6-pack", "6PCan", "6-pack of 12 oz cans");
			sixPackCan.SetConversion(six, one12ozCan);

			UnitOfMeasure fourPackCase = sys.CreateScalarUOM(UnitType.VOLUME, "4 pack case", "4PCase", "case of 4 6-packs");
			fourPackCase.SetConversion(four, sixPackCan);

			double bd = fourPackCase.GetConversionFactor(one12ozCan);
			Assert.IsTrue(IsCloseTo(bd, 24, DELTA6));

			bd = one12ozCan.GetConversionFactor(fourPackCase);

			bd = fourPackCase.GetConversionFactor(sixPackCan);
			bd = sixPackCan.GetConversionFactor(fourPackCase);

			bd = sixPackCan.GetConversionFactor(one12ozCan);
			bd = one12ozCan.GetConversionFactor(sixPackCan);

			Quantity tenCases = new Quantity(ten, fourPackCase);

			Quantity q1 = tenCases.Convert(one12ozCan);
			Assert.IsTrue(IsCloseTo(q1.Amount, 240, DELTA6));

			Quantity q2 = q1.Convert(fourPackCase);
			Assert.IsTrue(IsCloseTo(q2.Amount, 10, DELTA6));

			Quantity fortyPacks = new Quantity(forty, sixPackCan);
			q2 = fortyPacks.Convert(one12ozCan);
			Assert.IsTrue(IsCloseTo(q2.Amount, 240, DELTA6));

			Quantity oneCan = new Quantity(one, one12ozCan);
			q2 = oneCan.Convert(sixPackCan);
			Assert.IsTrue(IsCloseTo(q2.Amount, 0.1666666666666667, DELTA6));

			// A beer bottling line is rated at 2000 12 ounce cans/hour (US) at the
			// filler. The case packer packs four 6-packs of cans into a case.
			// Assuming no losses, what should be the rating of the case packer in
			// cases per hour? And, what is the draw-down rate on the holding tank
			// in gallons/minute?
			UnitOfMeasure canph = sys.CreateQuotientUOM(one12ozCan, sys.GetHour());
			UnitOfMeasure caseph = sys.CreateQuotientUOM(fourPackCase, sys.GetHour());
			UnitOfMeasure gpm = sys.CreateQuotientUOM(sys.GetUOM(Unit.US_GALLON), sys.GetMinute());
			Quantity filler = new Quantity(2000, canph);

			// draw-down
			Quantity draw = filler.Convert(gpm);
			Assert.IsTrue(IsCloseTo(draw.Amount, 3.125, DELTA6));

			// case production
			Quantity packer = filler.Convert(caseph);
			Assert.IsTrue(IsCloseTo(packer.Amount, 83.333333, DELTA6));
		}

		[TestMethod]
		public void TestGenericQuantity()
		{

			UnitOfMeasure a = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "a", "aUOM", "A");

			UnitOfMeasure b = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "b", "b", "B");
			b.SetConversion(10, a);

			double four = 4;

			double bd = Quantity.CreateAmount("4");
			Assert.IsTrue(bd.Equals(four));

			bd = Quantity.CreateAmount(4);
			Assert.IsTrue(bd.Equals(four));

			bd = Quantity.CreateAmount((decimal)4.0);
			Assert.IsTrue(bd.Equals(four));

			bd = Quantity.CreateAmount((decimal)4.0f);
			Assert.IsTrue(bd.Equals(four));

			bd = Quantity.CreateAmount(4L);
			Assert.IsTrue(bd.Equals(four));

			bd = Quantity.CreateAmount((short)4);
			Assert.IsTrue(bd.Equals(four));

			// Add
			Quantity q1 = new Quantity(four, a);

			Assert.IsFalse(q1.Equals(null));

			Quantity q2 = new Quantity(four, b);
			Quantity q3 = q1.Add(q2);

			Assert.IsTrue(IsCloseTo(q3.UOM.ScalingFactor, 1, DELTA6));
			Assert.IsTrue(q3.UOM.AbscissaUnit.Equals(a));
			Assert.IsTrue(IsCloseTo(q3.UOM.Offset, 0, DELTA6));
			Assert.IsTrue(IsCloseTo(q3.Amount, 44, DELTA6));

			// Subtract
			q3 = q1.Subtract(q2);
			Assert.IsTrue(IsCloseTo(q3.UOM.ScalingFactor, 1, DELTA6));
			Assert.IsTrue(q3.UOM.AbscissaUnit.Equals(a));
			Assert.IsTrue(IsCloseTo(q3.UOM.Offset, 0, DELTA6));
			Assert.IsTrue(IsCloseTo(q3.Amount, -36, DELTA6));

			// Multiply
			q3 = q1.Multiply(q2);
			Assert.IsTrue(IsCloseTo(q3.Amount, 16, DELTA6));
			Assert.IsTrue(IsCloseTo(q3.UOM.ScalingFactor, 1, DELTA6));
			Assert.IsTrue(IsCloseTo(q3.UOM.Offset, 0, DELTA6));

			UnitOfMeasure a2 = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "a*2", "a*2", "A squared", a, 2);
			Quantity q4 = q3.Convert(a2);
			Assert.IsTrue(IsCloseTo(q4.Amount, 160, DELTA6));
			Assert.IsTrue(q4.UOM.Equals(a2));

			q4 = q3.Divide(q2);
			Assert.IsTrue(q4.Equals(q1));
			Assert.IsTrue(IsCloseTo(q4.Amount, 4, DELTA6));

			// Divide
			q3 = q1.Divide(q2);
			Assert.IsTrue(IsCloseTo(q3.Amount, 1, DELTA6));
			Assert.IsTrue(IsCloseTo(q3.UOM.ScalingFactor, 0.1, DELTA6));

			q4 = q3.Multiply(q2);
			Assert.IsTrue(q4.Equals(q1));
		}

		[TestMethod]
		public void TestExceptions()
		{

			UnitOfMeasure floz = sys.GetUOM(Unit.BR_FLUID_OUNCE);

			Quantity q1 = new Quantity(10, sys.GetDay());
			Quantity q2 = new Quantity(10, sys.GetUOM(Unit.BR_FLUID_OUNCE));

			try
			{
				String amount = null;
				Quantity.CreateAmount(amount);
				Assert.Fail("create");
			}
			catch (Exception)
			{
			}

			try
			{
				q1.Convert(floz);
				Assert.Fail("Convert");
			}
			catch (Exception)
			{
			}

			try
			{
				q1.Add(q2);
				Assert.Fail("Add");
			}
			catch (Exception)
			{
			}

			try
			{
				q1.Subtract(q2);
				Assert.Fail("Subtract");
			}
			catch (Exception)
			{
			}

			// OK
			q1.Multiply(q2);

			// OK
			q1.Divide(q2);
		}

		[TestMethod]
		public void TestEquality()
		{
			UnitOfMeasure newton = sys.GetUOM(Unit.NEWTON);
			UnitOfMeasure metre = sys.GetUOM(Unit.METRE);
			UnitOfMeasure nm = sys.GetUOM(Unit.NEWTON_METRE);
			UnitOfMeasure m2 = sys.GetUOM(Unit.SQUARE_METRE);
			UnitOfMeasure J = sys.GetUOM(Unit.JOULE);
			double amount = 10;

			Quantity q1 = new Quantity(amount, newton);
			Quantity q2 = new Quantity(amount, metre);
			Quantity q3 = new Quantity(amount, nm);
			Quantity q5 = new Quantity(100, nm);

			// unity
			Quantity q4 = q5.Divide(q3);
			Assert.IsTrue(q4.UOM.GetBaseSymbol().Equals(sys.GetOne().Symbol));
			Assert.IsTrue(q4.Amount.Equals(amount));

			// Newton-metre (Joules)
			q4 = q1.Multiply(q2);
			Assert.IsTrue(q5.UOM.GetBaseSymbol().Equals(q4.UOM.GetBaseSymbol()));
			Quantity q6 = q5.Convert(J);
			Assert.IsTrue(q6.Amount.Equals(q4.Amount));

			// Newton
			q5 = q4.Divide(q2);
			Assert.IsTrue(q5.UOM.GetBaseSymbol().Equals(q1.UOM.GetBaseSymbol()));
			Assert.IsTrue(q5.Equals(q1));

			// metre
			q5 = q4.Divide(q1);
			Assert.IsTrue(q5.UOM.GetBaseSymbol().Equals(q2.UOM.GetBaseSymbol()));
			Assert.IsTrue(q5.Equals(q2));

			// square metre
			q4 = q2.Multiply(q2);
			q5 = new Quantity(100, m2);
			string q4s = q4.UOM.GetBaseSymbol();
			string q5s = q5.UOM.GetBaseSymbol();
			Assert.IsTrue(q4s.Equals(q5s));
			Assert.IsTrue(q5.Equals(q4));

			// metre
			q4 = q5.Divide(q2);
			Assert.IsTrue(q4.UOM.GetBaseSymbol().Equals(q2.UOM.GetBaseSymbol()));
			Assert.IsTrue(q4.Equals(q2));

		}

		[TestMethod]
		public void TestComparison()
		{
			UnitOfMeasure newton = sys.GetUOM(Unit.NEWTON);
			UnitOfMeasure metre = sys.GetUOM(Unit.METRE);
			UnitOfMeasure cm = sys.GetUOM(Prefix.CENTI, metre);

			double amount = 10;

			Quantity qN = new Quantity(amount, newton);
			Quantity qm10 = new Quantity(amount, metre);
			Quantity qm1 = new Quantity(1, metre);
			Quantity qcm = new Quantity(amount, cm);

			Assert.IsTrue(qN.Compare(qN) == 0);
			Assert.IsTrue(qm10.Compare(qm1) == 1);
			Assert.IsTrue(qm1.Compare(qm10) == -1);
			Assert.IsTrue(qcm.Compare(qm1) == -1);
			Assert.IsTrue(qm1.Compare(qcm) == 1);

			try
			{
				qN.Compare(qm10);
				Assert.Fail("not comparable)");
			}
			catch (Exception)
			{
			}

			Quantity conc = new Quantity(0.0025, sys.GetUOM(Unit.MOLARITY));
			double pH = -Math.Log10(conc.Amount);
			Assert.IsTrue(IsCloseTo(pH, 2.60, DELTA2));
		}

		[TestMethod]
		public void TestArithmetic()
		{
			UnitOfMeasure inch = sys.GetUOM(Unit.INCH);
			UnitOfMeasure cm = sys.GetUOM(Prefix.CENTI, sys.GetUOM(Unit.METRE));
			Quantity qcm = new Quantity(1, cm);
			Quantity qin = new Quantity(1, inch);
			double bd = 2.54;
			Quantity q1 = qcm.Multiply(bd).Convert(inch);
			Assert.IsTrue(IsCloseTo(q1.Amount, qin.Amount, DELTA6));
			Quantity q2 = q1.Convert(cm);
			Assert.IsTrue(IsCloseTo(q2.Amount, bd, DELTA6));
		}

		[TestMethod]
		public void TestFinancial()
		{
			Quantity q1 = new Quantity(10, Unit.US_DOLLAR);
			Quantity q2 = new Quantity(12, Unit.US_DOLLAR);
			Quantity q3 = q2.Subtract(q1).Divide(q1).Convert(Unit.PERCENT);
			Assert.IsTrue(IsCloseTo(q3.Amount, 20, DELTA6));
		}

		[TestMethod]
		public void TestPowerProductConversions()
		{

			UnitOfMeasure one = sys.GetOne();
			UnitOfMeasure mps = sys.GetUOM(Unit.METRE_PER_SEC);
			UnitOfMeasure fps = sys.GetUOM(Unit.FEET_PER_SEC);
			UnitOfMeasure nm = sys.GetUOM(Unit.NEWTON_METRE);
			UnitOfMeasure ft = sys.GetUOM(Unit.FOOT);
			UnitOfMeasure inch = sys.GetUOM(Unit.INCH);
			UnitOfMeasure mi = sys.GetUOM(Unit.MILE);
			UnitOfMeasure hr = sys.GetUOM(Unit.HOUR);
			UnitOfMeasure m = sys.GetUOM(Unit.METRE);
			UnitOfMeasure s = sys.GetUOM(Unit.SECOND);
			UnitOfMeasure n = sys.GetUOM(Unit.NEWTON);
			UnitOfMeasure lbf = sys.GetUOM(Unit.POUND_FORCE);
			UnitOfMeasure m2 = sys.GetUOM(Unit.SQUARE_METRE);
			UnitOfMeasure m3 = sys.GetUOM(Unit.CUBIC_METRE);
			UnitOfMeasure ft2 = sys.GetUOM(Unit.SQUARE_FOOT);

			// test products and quotients
			sys.UnregisterUnit(sys.GetUOM(Unit.FOOT_POUND_FORCE));
			Quantity nmQ = new Quantity(1, nm);
			Quantity lbfinQ = nmQ.ConvertToPowerProduct(lbf, inch);
			Assert.IsTrue(IsCloseTo(lbfinQ.Amount, 8.850745791327183, DELTA6));
			Quantity mpsQ = new Quantity(1, mps);

			Quantity fphQ = mpsQ.ConvertToPowerProduct(ft, hr);
			Assert.IsTrue(IsCloseTo(fphQ.Amount, 11811.02362204724, DELTA6));

			Quantity mps2Q = fphQ.ConvertToPowerProduct(m, s);
			Assert.IsTrue(IsCloseTo(mps2Q.Amount, 1, DELTA6));

			Quantity mps3Q = mpsQ.ConvertToPowerProduct(m, s);
			Assert.IsTrue(IsCloseTo(mps3Q.Amount, 1, DELTA6));

			Quantity inlbfQ = nmQ.ConvertToPowerProduct(inch, lbf);
			Assert.IsTrue(IsCloseTo(inlbfQ.Amount, 8.850745791327183, DELTA6));

			Quantity nm2Q = lbfinQ.ConvertToPowerProduct(n, m);
			Assert.IsTrue(IsCloseTo(nm2Q.Amount, 1, DELTA6));

			nm2Q = lbfinQ.ConvertToPowerProduct(m, n);
			Assert.IsTrue(IsCloseTo(nm2Q.Amount, 1, DELTA6));

			Quantity mQ = new Quantity(1, m);
			try
			{
				mQ.ConvertToPowerProduct(ft, hr);
				Assert.Fail();
			}
			catch (Exception)
			{
			}

			sys.UnregisterUnit(sys.GetUOM(Unit.SQUARE_FOOT));
			Quantity m2Q = new Quantity(1, m2);
			Quantity ft2Q = m2Q.ConvertToPowerProduct(ft, ft);
			Assert.IsTrue(IsCloseTo(ft2Q.Amount, 10.76391041670972, DELTA6));

			Quantity mmQ = ft2Q.ConvertToPowerProduct(m, m);
			Assert.IsTrue(IsCloseTo(mmQ.Amount, 1, DELTA6));

			try
			{
				m2Q.ConvertToPowerProduct(m, one);
				Assert.Fail();
			}
			catch (Exception)
			{
			}

			sys.UnregisterUnit(sys.GetUOM(Unit.CUBIC_FOOT));
			Quantity m3Q = new Quantity(1, m3);
			Quantity ft3Q = m3Q.ConvertToPowerProduct(ft2, ft);
			Assert.IsTrue(IsCloseTo(ft3Q.Amount, 35.31466672148858, DELTA6));

			Quantity m3Q2 = m3Q.ConvertToPowerProduct(m2, m);
			Assert.IsTrue(IsCloseTo(m3Q2.Amount, 1, DELTA6));

			ft3Q = m3Q.ConvertToPowerProduct(ft, ft2);
			Assert.IsTrue(IsCloseTo(ft3Q.Amount, 35.31466672148858, DELTA6));

			UnitOfMeasure perM = sys.GetUOM(Unit.DIOPTER);
			Quantity perMQ = new Quantity(1, perM);
			Quantity perInQ = perMQ.ConvertToPowerProduct(one, inch);
			Assert.IsTrue(IsCloseTo(perInQ.Amount, 0.0254, DELTA6));

			try
			{
				perMQ.ConvertToPowerProduct(inch, one);
				Assert.Fail();
			}
			catch (Exception)
			{
			}

			try
			{
				perMQ.ConvertToPowerProduct(inch, inch);
				Assert.Fail();
			}
			catch (Exception)
			{
			}

			Quantity fpsQ = new Quantity(1, fps);
			Quantity mphQ = fpsQ.ConvertToPowerProduct(mi, hr);
			Assert.IsTrue(IsCloseTo(mphQ.Amount, 0.6818181818181818, DELTA6));

			// test powers
			Quantity in2Q = m2Q.ConvertToPower(inch);
			Assert.IsTrue(IsCloseTo(in2Q.Amount, 1550.003100006200, DELTA6));

			Quantity m2Q2 = in2Q.ConvertToPower(m);
			Assert.IsTrue(IsCloseTo(m2Q2.Amount, 1, DELTA6));

			Quantity perInQ2 = perMQ.ConvertToPower(inch);
			Assert.IsTrue(IsCloseTo(perInQ2.Amount, 0.0254, DELTA6));

			Quantity q1 = perInQ2.ConvertToPower(m);
			Assert.IsTrue(IsCloseTo(q1.Amount, 1, DELTA6));

			Quantity inQ2 = mQ.ConvertToPower(inch);
			Assert.IsTrue(IsCloseTo(inQ2.Amount, 39.37007874015748, DELTA6));

			q1 = inQ2.ConvertToPower(m);
			Assert.IsTrue(IsCloseTo(q1.Amount, 1, DELTA6));

			Quantity one1 = new Quantity(1, sys.GetOne());
			q1 = one1.ConvertToPower(sys.GetOne());
			Assert.IsTrue(IsCloseTo(q1.Amount, 1, DELTA6));
		}

	}
}
