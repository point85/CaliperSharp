using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Point85.Caliper.UnitOfMeasure;
using System.Text;
using System.Diagnostics;

namespace CaliperSharpTests
{
	[TestClass]
	public class TestUnits : BaseTest
	{
		[TestMethod]
		public void TestGetString()
		{
			string str = MeasurementSystem.GetUnitString("one.name");
			Assert.IsTrue(str.Length > 0);
		}

		[TestMethod]
		public void TestPrefixes()
		{
			foreach (Prefix prefix in Prefix.GetDefinedPrefixes())
			{
				string prefixName = prefix.Name;
				Assert.IsTrue(prefixName.Length > 0);
				Assert.IsTrue(prefix.Symbol.Length > 0);
				Assert.IsTrue(!prefix.Factor.Equals(1));
				Assert.IsTrue(prefix.ToString().Length > 0);
				Assert.IsTrue(Prefix.FromName(prefixName).Equals(prefix));
			}
		}

		[TestMethod]
		public void TestExceptions()
		{
			UnitOfMeasure uom1 = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "uom1", "uom1", "");
			UnitOfMeasure uom2 = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "uom2", "uom2", "");
			UnitOfMeasure uom3 = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "uom3", "uom3", "");

			uom1.SetConversion(1, uom3, 10);
			uom2.SetConversion(1, uom3, 1);
			Assert.IsFalse(uom1.Equals(uom2));

			try
			{
				sys.CreatePowerUOM(null, 0);
				Assert.Fail();
			}
			catch (Exception)
			{
			}

			try
			{
				sys.CreateProductUOM(null, sys.GetOne());
				Assert.Fail();
			}
			catch (Exception)
			{
			}

			try
			{
				sys.CreateProductUOM(sys.GetOne(), null);
				Assert.Fail();
			}
			catch (Exception)
			{
			}

			try
			{
				sys.CreateQuotientUOM(null, sys.GetOne());
				Assert.Fail();
			}
			catch (Exception)
			{
			}

			try
			{
				sys.CreateQuotientUOM(sys.GetOne(), null);
				Assert.Fail();
			}
			catch (Exception)
			{
			}

			try
			{
				sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "uom4", "uom4", "", sys.GetUOM(Unit.METRE), null);
				Assert.Fail();
			}
			catch (Exception)
			{
			}

			try
			{
				sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "uom4", "uom4", "", null, sys.GetUOM(Unit.METRE));
				Assert.Fail();
			}
			catch (Exception)
			{
			}

			try
			{
				sys.CreateProductUOM(UnitType.UNCLASSIFIED, "uom4", "uom4", "", sys.GetUOM(Unit.METRE), null);
				Assert.Fail();
			}
			catch (Exception)
			{
			}

			try
			{
				sys.CreateProductUOM(UnitType.UNCLASSIFIED, "uom4", "uom4", "", null, sys.GetUOM(Unit.METRE));
				Assert.Fail();
			}
			catch (Exception)
			{
			}

			try
			{
				Quantity q = new Quantity(10, Unit.METRE);
				q.Convert(Unit.SECOND);
				Assert.Fail("no conversion");
			}
			catch (Exception)
			{
			}

			sys.UnregisterUnit(null);

			UnitOfMeasure u = null;

			try
			{
				sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "456", null, "description");
				Assert.Fail("no symbol");
			}
			catch (Exception)
			{
			}

			try
			{
				sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "456", "", "description");
				Assert.Fail("no symbol");
			}
			catch (Exception)
			{
			}

			try
			{
				sys.CreateProductUOM(UnitType.UNCLASSIFIED, null, "abcd", "", null, null);
				Assert.Fail("null");
			}
			catch (Exception)
			{
			}

			try
			{
				sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, null, "abcd", "", null, null);
				Assert.Fail("null");
			}
			catch (Exception)
			{
			}

			try
			{
				sys.CreatePowerUOM(UnitType.UNCLASSIFIED, null, "abcd", "", null, 2);
				Assert.Fail("null");
			}
			catch (Exception)
			{
			}

			try
			{
				sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "", null, "");
				sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "", "", "");
				Assert.Fail("already created");
			}
			catch (Exception)
			{
			}

			u = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "1/1", "uno/one", "", sys.GetOne(), sys.GetOne());
			Quantity q1 = new Quantity(10, u);
			Quantity q2 = q1.Convert(sys.GetOne());
			Assert.IsTrue(IsCloseTo(q2.Amount, 10, DELTA6));
			Assert.IsTrue(q2.UOM.Equals(sys.GetOne()));

			u = sys.CreateProductUOM(UnitType.UNCLASSIFIED, "1x1", "1x1", "", sys.GetOne(), sys.GetOne());
			double bd = u.GetConversionFactor(sys.GetOne());
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			u = sys.CreateProductUOM(UnitType.UNCLASSIFIED, "1x1", "1x1", "", sys.GetOne(), sys.GetOne());
			Assert.IsTrue(u.GetBaseSymbol().Equals(sys.GetOne().GetBaseSymbol()));

			u = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "1^2", "1^2", "", sys.GetOne(), 2);
			bd = u.GetConversionFactor(sys.GetOne());
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			u = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "1^2", "1^2", "", sys.GetOne(), 2);
			bd = u.GetConversionFactor(sys.GetOne());
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			u = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "1^0", "1^0", "", sys.GetOne(), 0);
			bd = u.GetConversionFactor(sys.GetOne());
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			u = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "1^0", "1^0", "", sys.GetOne(), 0);
			bd = u.GetConversionFactor(sys.GetOne());
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			UnitOfMeasure uno = sys.GetOne();
			u = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "m^0", "m^0", "", sys.GetUOM(Unit.METRE), 0);
			bd = u.GetConversionFactor(uno);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));
			Assert.IsTrue(u.GetBaseSymbol().Equals(sys.GetOne().GetBaseSymbol()));

			UnitOfMeasure m1 = sys.GetUOM(Unit.METRE);
			u = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "m^1", "m^1", "", sys.GetUOM(Unit.METRE), 1);
			Assert.IsTrue(u.GetBaseSymbol().Equals(m1.GetBaseSymbol()));

			UnitOfMeasure m2 = sys.GetUOM(Unit.SQUARE_METRE);
			u = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "m^2", "m^2", "", sys.GetUOM(Unit.METRE), 2);
			Assert.IsTrue(u.GetBaseSymbol().Equals(m2.GetBaseSymbol()));

			UnitOfMeasure perMetre = m1.Invert();
			UnitOfMeasure diopter = sys.GetUOM(Unit.DIOPTER);
			Assert.IsTrue(perMetre.GetBaseSymbol().Equals(diopter.GetBaseSymbol()));

			u = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "m*-1", "m*-1", "", sys.GetUOM(Unit.METRE), -1);
			UnitOfMeasure mult = u.Multiply(m1);
			Assert.IsTrue(mult.Equals(sys.GetUOM(Unit.ONE)));

			UnitOfMeasure perMetre2 = m2.Invert();
			u = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "m*-2", "m*-2", "", sys.GetUOM(Unit.METRE), -2);
			Assert.IsTrue(u.GetBaseSymbol().Equals(perMetre2.GetBaseSymbol()));

			u = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "m^0", "m^0", "", sys.GetUOM(Unit.METRE), 0);

			try
			{
				UnitOfMeasure abscissaUnit = null;
				uno.SetConversion(abscissaUnit);
				Assert.Fail();
			}
			catch (Exception)
			{
			}
		}

		[TestMethod]
		public void TestOne()
		{
			UnitOfMeasure metre = sys.GetUOM(Unit.METRE);

			UnitOfMeasure u = metre.Multiply(sys.GetOne());
			Assert.IsTrue(u.Equals(metre));

			u = metre.Divide(sys.GetOne());
			Assert.IsTrue(u.Equals(metre));

			UnitOfMeasure oneOverM = metre.Invert();
			u = oneOverM.Invert();
			Assert.IsTrue(u.Equals(metre));

			u = oneOverM.Multiply(metre);
			Assert.IsTrue(u.Equals(sys.GetOne()));

			u = metre.Divide(metre);
			Assert.IsTrue(u.Equals(sys.GetOne()));

			u = sys.GetOne().Divide(metre).Multiply(metre);
			Assert.IsTrue(u.Equals(sys.GetOne()));

			UnitOfMeasure uom = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "1/1", "1/1", "");
			uom.SetConversion(1, sys.GetOne(), 1);

			Assert.IsTrue(IsCloseTo(uom.GetScalingFactor(), 1, DELTA6));
			Assert.IsTrue(uom.GetAbscissaUnit().Equals(sys.GetOne()));
			Assert.IsTrue(IsCloseTo(uom.GetOffset(), 1, DELTA6));

			u = sys.GetOne().Invert();
			Assert.IsTrue(u.GetAbscissaUnit().Equals(sys.GetOne()));

			UnitOfMeasure one = sys.GetOne();
			Assert.IsTrue(one.GetBaseSymbol().Equals("1"));
			Assert.IsTrue(one.Equals(one));

			UnitOfMeasure uno = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "", ".1", "", one, one);
			Assert.IsTrue(uno.GetBaseSymbol().Equals(one.GetBaseSymbol()));

			UnitOfMeasure p = sys.CreateProductUOM(UnitType.UNCLASSIFIED, "", "..1", "", one, one);
			Assert.IsTrue(p.GetBaseSymbol().Equals(one.GetBaseSymbol()));

			UnitOfMeasure p3 = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "", "...1", "", one, 3);
			Assert.IsTrue(p3.GetBaseSymbol().Equals(one.GetBaseSymbol()));

			p3 = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "", "...1", "", one, -1);
			Assert.IsTrue(p3.GetBaseSymbol().Equals(one.GetBaseSymbol()));

			UnitOfMeasure a1 = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "a1", "a1", "A1");
			Assert.IsTrue(a1.GetBaseSymbol().Equals("a1"));

			uno = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "one", "one", "", a1, a1);
			Assert.IsTrue(uno.GetBaseSymbol().Equals(one.GetBaseSymbol()));
		}

		[TestMethod]
		public void TestGeneric()
		{
			UnitOfMeasure b = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "b", "beta", "Beta");
			Assert.IsFalse(b.Equals(null));

			// scalar
			UnitOfMeasure ab1 = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "a=2b+1", "a=2b+1", "custom");
			ab1.SetConversion(2, b, 1);

			Assert.IsTrue(IsCloseTo(ab1.GetScalingFactor(), 2, DELTA6));
			Assert.IsTrue(ab1.GetAbscissaUnit().Equals(b));
			Assert.IsTrue(IsCloseTo(ab1.GetOffset(), 1, DELTA6));

			// quotient
			UnitOfMeasure a = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "a", "alpha", "Alpha");
			Assert.IsTrue(a.GetAbscissaUnit().Equals(a));

			UnitOfMeasure aOverb = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "a/b", "a/b", "", a, b);
			aOverb.SetScalingFactor(2);

			Assert.IsTrue(IsCloseTo(aOverb.GetScalingFactor(), 2, DELTA6));
			Assert.IsTrue(aOverb.GetDividend().Equals(a));
			Assert.IsTrue(aOverb.GetDivisor().Equals(b));
			Assert.IsTrue(IsCloseTo(aOverb.GetOffset(), 0, DELTA6));
			Assert.IsTrue(aOverb.GetAbscissaUnit().Equals(aOverb));

			UnitOfMeasure bOvera = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "b/a", "b/a", "", b, a);
			UnitOfMeasure bOveraI = bOvera.Invert();
			Assert.IsTrue(bOveraI.GetBaseSymbol().Equals(aOverb.GetBaseSymbol()));

			// Multiply2
			UnitOfMeasure uom = aOverb.Multiply(b);
			Assert.IsTrue(uom.GetAbscissaUnit().Equals(a));
			Assert.IsTrue(IsCloseTo(uom.GetScalingFactor(), 2, DELTA6));
			double bd = uom.GetConversionFactor(a);
			Assert.IsTrue(IsCloseTo(bd, 2, DELTA6));

			// Divide2
			UnitOfMeasure uom2 = uom.Divide(b);
			Assert.IsTrue(IsCloseTo(uom2.GetScalingFactor(), 2, DELTA6));
			Assert.IsTrue(IsCloseTo(uom2.GetOffset(), 0, DELTA6));
			Assert.IsTrue(uom2.Equals(aOverb));

			// Invert
			UnitOfMeasure uom3 = uom2.Invert();
			UnitOfMeasure u = uom3.Multiply(uom2);
			Assert.IsTrue(u.GetBaseSymbol().Equals(sys.GetOne().GetBaseSymbol()));

			// product
			UnitOfMeasure ab = sys.CreateProductUOM(UnitType.UNCLASSIFIED, "name", "symbol", "custom", a, b);
			ab.SetOffset(1);

			Assert.IsTrue(IsCloseTo(ab.GetScalingFactor(), 1, DELTA6));
			Assert.IsTrue(ab.GetMultiplier().Equals(a));
			Assert.IsTrue(ab.GetMultiplicand().Equals(b));
			Assert.IsTrue(IsCloseTo(ab.GetOffset(), 1, DELTA6));
			Assert.IsTrue(ab.GetAbscissaUnit().Equals(ab));

			ab.SetOffset(0);

			UnitOfMeasure uom4 = ab.Divide(a);
			Assert.IsTrue(IsCloseTo(uom4.GetScalingFactor(), 1, DELTA6));
			Assert.IsTrue(uom4.GetAbscissaUnit().Equals(b));

			UnitOfMeasure uom5 = uom4.Multiply(a);
			Assert.IsTrue(IsCloseTo(uom5.GetScalingFactor(), 1, DELTA6));
			u = uom5.GetAbscissaUnit();
			Assert.IsTrue(u.GetBaseSymbol().Equals(ab.GetBaseSymbol()));

			// Invert
			UnitOfMeasure uom6 = ab.Invert();
			Assert.IsTrue(IsCloseTo(uom6.GetScalingFactor(), 1, DELTA6));
			Assert.IsTrue(uom6.GetDividend().Equals(sys.GetOne()));
			Assert.IsTrue(uom6.GetDivisor().Equals(ab));
			Assert.IsTrue(IsCloseTo(uom6.GetOffset(), 0, DELTA6));

			// power
			UnitOfMeasure a2 = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "name", "a**2", "custom", a, 2);

			Assert.IsTrue(IsCloseTo(a2.GetScalingFactor(), 1, DELTA6));
			Assert.IsTrue(a2.GetPowerBase().Equals(a));
			Assert.IsTrue(a2.GetPowerExponent() == 2);
			Assert.IsTrue(IsCloseTo(a2.GetOffset(), 0, DELTA6));
			Assert.IsTrue(a2.GetAbscissaUnit().Equals(a2));

			UnitOfMeasure uom8 = a2.Divide(a);
			Assert.IsTrue(IsCloseTo(uom8.GetScalingFactor(), 1, DELTA6));
			Assert.IsTrue(IsCloseTo(uom8.GetOffset(), 0, DELTA6));
			Assert.IsTrue(uom8.GetAbscissaUnit().Equals(a));

			UnitOfMeasure uom9 = uom8.Multiply(a);
			Assert.IsTrue(IsCloseTo(uom9.GetScalingFactor(), 1, DELTA6));
			Assert.IsTrue(IsCloseTo(uom9.GetOffset(), 0, DELTA6));
			u = uom9.GetAbscissaUnit();
			Assert.IsTrue(u.GetBaseSymbol().Equals(a2.GetBaseSymbol()));

			u = sys.GetUOM(a.Symbol);
			Assert.IsFalse(uom == null);

			// again
			UnitOfMeasure c = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "c", "cUnit", "C");
			UnitOfMeasure x = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "x", "xUnit", "X");
			UnitOfMeasure e = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "e", "eUnit", "E");

			UnitOfMeasure aTimesa = sys.CreateProductUOM(UnitType.UNCLASSIFIED, "", "aUnit*2", "", a, a);
			u = aTimesa.Divide(a);
			Assert.IsTrue(u.GetBaseSymbol().Equals(a.GetBaseSymbol()));

			u = aOverb.Multiply(b);
			Assert.IsTrue(u.GetBaseSymbol().Equals(a.GetBaseSymbol()));

			UnitOfMeasure cOverx = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "", "c/x", "", c, x);

			UnitOfMeasure u1 = aOverb.Divide(cOverx).Multiply(cOverx);
			Assert.IsTrue(aOverb.Divide(cOverx).Multiply(cOverx).GetBaseSymbol().Equals(aOverb.GetBaseSymbol()));

			string str0 = aOverb.Multiply(cOverx).GetBaseSymbol();
			string str1 = aOverb.Multiply(cOverx).Divide(cOverx).GetBaseSymbol();
			string str2 = aOverb.GetBaseSymbol();
			Assert.IsTrue(str1.Equals(str2));

			UnitOfMeasure axb = sys.CreateProductUOM(UnitType.UNCLASSIFIED, "", "a.b", "", a, b);
			u = sys.GetUOM(axb.Symbol);
			Assert.IsTrue(u.Equals(axb));
			Assert.IsTrue(axb.Divide(a).Equals(b));

			String symbol = axb.Symbol + "." + axb.Symbol;
			UnitOfMeasure axbsq = sys.CreateProductUOM(UnitType.UNCLASSIFIED, "", symbol, "", axb, axb);
			u = axbsq.Divide(axb);
			Assert.IsTrue(u.GetBaseSymbol().Equals(axb.GetBaseSymbol()));

			UnitOfMeasure b2 = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "b2", "b*2", "", b, 2);

			symbol = axb.GetBaseSymbol();
			u = sys.GetBaseUOM(symbol);
			Assert.IsTrue(u != null);

			UnitOfMeasure axb2 = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "axb2", "(a.b)*2", "", axb, 2);
			u = axb2.Divide(axb);
			Assert.IsTrue(u.GetBaseSymbol().Equals(axb.GetBaseSymbol()));

			UnitOfMeasure aOverb2 = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "aOverb2", "(a/b)*2", "", aOverb, 2);
			u = aOverb2.Multiply(b2);
			Assert.IsTrue(u.GetBaseSymbol().Equals(aTimesa.GetBaseSymbol()));

			symbol = axb.Symbol + "^-2";
			UnitOfMeasure axbm2 = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "", symbol, "", axb, -2);
			uom = axbm2.Multiply(axb2);
			Assert.IsTrue(uom.GetBaseSymbol().Equals(sys.GetOne().Symbol));

			UnitOfMeasure cxd = sys.CreateProductUOM(UnitType.UNCLASSIFIED, "", "c.D", "", c, x);
			const char MULT = (char)(char)0xB7;
			StringBuilder sb = new StringBuilder();
			sb.Append("cUnit").Append(MULT).Append("xUnit");
			String str = sb.ToString();
			Assert.IsTrue(cxd.GetBaseSymbol().IndexOf(str) != -1);

			UnitOfMeasure abdivcd = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "", "(a.b)/(c.D)", "", axb, cxd);
			Assert.IsTrue(abdivcd.GetDividend().Equals(axb));
			Assert.IsTrue(abdivcd.GetDivisor().Equals(cxd));

			UnitOfMeasure cde = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "", "(c.D)/(e)", "", cxd, e);
			sb = new StringBuilder();
			sb.Append("cUnit").Append(MULT).Append("xUnit/eUnit");
			str = sb.ToString();
			Assert.IsTrue(cde.GetBaseSymbol().IndexOf(str) != -1);

			u = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, null, "not null", null);
			Assert.IsTrue(u.ToString() != null);
		}

		[TestMethod]
		public void TestUSUnits()
		{
			UnitOfMeasure foot = sys.GetUOM(Unit.FOOT);
			UnitOfMeasure gal = sys.GetUOM(Unit.US_GALLON);
			UnitOfMeasure flush = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "flush", "flush", "");
			UnitOfMeasure gpf = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "gal per flush", "gpf", "", gal, flush);
			UnitOfMeasure velocity = sys.GetUOM(Unit.FEET_PER_SEC);

			UnitOfMeasure litre = sys.GetUOM(Unit.LITRE);
			UnitOfMeasure lpf = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "litre per flush", "lpf", "", litre, flush);

			double bd = gpf.GetConversionFactor(lpf);
			Assert.IsTrue(IsCloseTo(bd, 3.785411784, DELTA6));

			bd = lpf.GetConversionFactor(gpf);
			Assert.IsTrue(IsCloseTo(bd, 0.2641720523581484, DELTA6));

			// inversions
			UnitOfMeasure u = foot.Invert();
			Assert.IsTrue(u.Symbol.Equals("1/ft"));

			u = u.Multiply(foot);
			Assert.IsTrue(u.GetBaseSymbol().Equals(sys.GetOne().GetBaseSymbol()));

			u = velocity.Invert();
			Assert.IsTrue(u.Symbol.Equals("s/ft"));

			u = u.Multiply(velocity);
			Assert.IsTrue(u.GetBaseSymbol().Equals(sys.GetOne().GetBaseSymbol()));

		}

		[TestMethod]
		public void TestImperialUnits()
		{
			UnitOfMeasure impGal = sys.GetUOM(Unit.BR_GALLON);
			UnitOfMeasure impPint = sys.GetUOM(Unit.BR_PINT);
			UnitOfMeasure impOz = sys.GetUOM(Unit.BR_FLUID_OUNCE);

			UnitOfMeasure usGal = sys.GetUOM(Unit.US_GALLON);
			UnitOfMeasure usPint = sys.GetUOM(Unit.US_PINT);

			UnitOfMeasure litre = sys.GetUOM(Unit.LITRE);
			UnitOfMeasure m3 = sys.GetUOM(Unit.CUBIC_METRE);

			double bd = impGal.GetConversionFactor(litre);
			Assert.IsTrue(IsCloseTo(bd, 4.54609, DELTA6));

			bd = litre.GetConversionFactor(impGal);
			Assert.IsTrue(IsCloseTo(bd, 0.2199692482990878, DELTA6));

			bd = impGal.GetConversionFactor(usGal);
			Assert.IsTrue(IsCloseTo(bd, 1.200949925504855, DELTA6));

			bd = usGal.GetConversionFactor(impGal);
			Assert.IsTrue(IsCloseTo(bd, 0.8326741846289888, DELTA6));

			bd = impGal.GetConversionFactor(impPint);
			Assert.IsTrue(IsCloseTo(bd, 8, DELTA6));

			bd = impPint.GetConversionFactor(impGal);
			Assert.IsTrue(IsCloseTo(bd, 0.125, DELTA6));

			bd = usGal.GetConversionFactor(usPint);
			Assert.IsTrue(IsCloseTo(bd, 8, DELTA6));

			bd = usPint.GetConversionFactor(usGal);
			Assert.IsTrue(IsCloseTo(bd, 0.125, DELTA6));

			bd = impOz.GetConversionFactor(m3);
			Assert.IsTrue(IsCloseTo(bd, 28.4130625E-06, DELTA6));

			bd = m3.GetConversionFactor(impOz);
			Assert.IsTrue(IsCloseTo(bd, 35195.07972785405, DELTA6));

		}

		[TestMethod]
		public void TestOperations()
		{
			UnitOfMeasure u = null;
			UnitOfMeasure hour = sys.GetHour();
			UnitOfMeasure metre = sys.GetUOM(Unit.METRE);
			UnitOfMeasure m2 = sys.GetUOM(Unit.SQUARE_METRE);

			// Multiply2
			UnitOfMeasure velocity = sys.GetUOM("meter/hr");

			if (velocity == null)
			{
				velocity = sys.CreateScalarUOM(UnitType.VELOCITY, "meter/hr", "meter/hr", "");
				velocity.SetConversion((double)1 / (double)3600, sys.GetUOM(Unit.METRE_PER_SEC));
			}

			double sf = (double)1 / (double)3600;
			Assert.IsTrue(IsCloseTo(velocity.GetScalingFactor(), sf, DELTA6));
			Assert.IsTrue(velocity.GetAbscissaUnit().Equals(sys.GetUOM(Unit.METRE_PER_SEC)));
			Assert.IsTrue(IsCloseTo(velocity.GetOffset(), 0, DELTA6));

			u = velocity.Multiply(hour);
			double bd = u.GetConversionFactor(metre);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			u = hour.Multiply(velocity);
			bd = u.GetConversionFactor(metre);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			u = metre.Multiply(metre);
			bd = u.GetConversionFactor(m2);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));
			Assert.IsTrue(u.Equals(m2));

			// Divide2
			u = metre.Divide(hour);

			bd = u.GetConversionFactor(velocity);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			u = u.Multiply(hour);
			Assert.IsTrue(u.Equals(metre));

			// Invert
			UnitOfMeasure vInvert = velocity.Invert();
			Assert.IsTrue(vInvert.GetScalingFactor().Equals(1));

			// max symbol length
			Quantity v = null;
			Quantity h = null;
			UnitOfMeasure mpc = sys.GetUOM(Prefix.MEGA, sys.GetUOM(Unit.PARSEC));
			Quantity d = new Quantity(10, mpc);
			Quantity h0 = sys.GetQuantity(Constant.HUBBLE_CONSTANT);

			for (int i = 0; i < 3; i++)
			{
				v = h0.Multiply(d);
				d = v.Divide(h0);
				h = v.Divide(d);
			}
			Assert.IsTrue(h.UOM.Symbol.Length < 16);

			// conflict with 1/s
			sys.UnregisterUnit(h0.UOM);
		}


		[TestMethod]
		public void TestTime()
		{
			UnitOfMeasure s2 = sys.GetUOM(Unit.SQUARE_SECOND);
			UnitOfMeasure second = sys.GetSecond();
			UnitOfMeasure min = sys.GetMinute();
			UnitOfMeasure hour = sys.GetHour();
			UnitOfMeasure msec = sys.GetUOM(Prefix.MILLI, second);
			UnitOfMeasure min2 = sys.CreatePowerUOM(UnitType.TIME_SQUARED, "sqMin", "min^2", null, min, 2);

			Assert.IsTrue(IsCloseTo(second.GetConversionFactor(msec), 1000, DELTA6));

			Assert.IsTrue(IsCloseTo(second.GetScalingFactor(), 1, DELTA6));
			Assert.IsTrue(second.GetAbscissaUnit().Equals(second));
			Assert.IsTrue(IsCloseTo(second.GetOffset(), 0, DELTA6));

			double bd = hour.GetConversionFactor(second);

			UnitOfMeasure u = second.Multiply(second);

			Assert.IsTrue(IsCloseTo(u.GetScalingFactor(), 1, DELTA6));
			Assert.IsTrue(IsCloseTo(u.GetOffset(), 0, DELTA6));
			Assert.IsTrue(u.Equals(s2));

			u = second.Divide(second);
			Assert.IsTrue(u.GetBaseSymbol().Equals(sys.GetOne().Symbol));

			Quantity q1 = new Quantity(1, u);
			Quantity q2 = q1.Convert(sys.GetOne());
			Assert.IsTrue(IsCloseTo(q2.Amount, 1, DELTA6));
			Assert.IsTrue(q2.UOM.Equals(sys.GetOne()));

			u = second.Invert();

			Assert.IsTrue(u.GetDividend().Equals(sys.GetOne()));
			Assert.IsTrue(u.GetDivisor().Equals(second));

			u = min.Divide(second);
			Assert.IsTrue(IsCloseTo(u.GetScalingFactor(), 60, DELTA6));
			Assert.IsTrue(IsCloseTo(u.GetOffset(), 0, DELTA6));

			UnitOfMeasure uom = u.Multiply(second);
			bd = uom.GetConversionFactor(min);
			Assert.IsTrue(uom.Equals(min));
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			q1 = new Quantity(10, u);
			q2 = q1.Convert(sys.GetOne());
			Assert.IsTrue(IsCloseTo(q2.Amount, 600, DELTA6));
			Assert.IsTrue(q2.UOM.Equals(sys.GetOne()));

			// Multiply2
			u = min.Multiply(min);
			Assert.IsTrue(IsCloseTo(u.GetScalingFactor(), 3600, DELTA6));
			Assert.IsTrue(u.GetAbscissaUnit().Equals(s2));
			Assert.IsTrue(IsCloseTo(u.GetOffset(), 0, DELTA6));

			q1 = new Quantity(10, u);
			q2 = q1.Convert(s2);
			Assert.IsTrue(IsCloseTo(q2.Amount, 36000, DELTA6));
			Assert.IsTrue(q2.UOM.Equals(s2));

			q2 = q2.Convert(min2);
			Assert.IsTrue(IsCloseTo(q2.Amount, 10, DELTA6));

			u = min.Multiply(second);
			Assert.IsTrue(IsCloseTo(u.GetScalingFactor(), 60, DELTA6));
			Assert.IsTrue(u.GetAbscissaUnit().Equals(s2));

			u = second.Multiply(min);
			bd = u.GetConversionFactor(s2);
			Assert.IsTrue(IsCloseTo(bd, 60, DELTA6));

		}

		[TestMethod]
		public void TestSymbolCache()
		{
			UnitOfMeasure uom = sys.GetUOM(Unit.KILOGRAM);
			UnitOfMeasure other = sys.GetUOM(uom.Symbol);
			Assert.IsTrue(uom.Equals(other));

			other = sys.GetUOM(uom.GetBaseSymbol());
			Assert.IsTrue(uom.Equals(other));

			uom = sys.GetUOM(Prefix.CENTI, sys.GetUOM(Unit.METRE));
			other = sys.GetUOM(uom.Symbol);
			Assert.IsTrue(uom.Equals(other));

			other = sys.GetUOM(uom.GetBaseSymbol());
			Assert.IsTrue(uom.GetBaseSymbol().Equals(other.GetBaseSymbol()));

			uom = sys.GetUOM(Unit.NEWTON);
			other = sys.GetUOM(uom.Symbol);
			Assert.IsTrue(uom.Equals(other));

			other = sys.GetBaseUOM(uom.GetBaseSymbol());
			Assert.IsTrue(uom.Equals(other));
		}

		[TestMethod]
		public void TestBaseSymbols()
		{
			char times = (char)0xB7;
			char sq = (char)0xB2;
			char cu = (char)0xB3;

			StringBuilder sb = new StringBuilder();

			UnitOfMeasure metre = sys.GetUOM(Unit.METRE);

			String symbol = sys.GetOne().GetBaseSymbol();
			Assert.IsTrue(symbol.Equals("1"));

			symbol = sys.GetSecond().GetBaseSymbol();
			Assert.IsTrue(symbol.Equals("s"));

			symbol = metre.GetBaseSymbol();
			Assert.IsTrue(symbol.Equals("m"));

			UnitOfMeasure mm = sys.GetUOM(Prefix.MILLI, metre);
			Assert.IsTrue(Prefix.MILLI.Factor > 0.0);

			symbol = mm.Symbol;
			Assert.IsTrue(symbol.Equals("mm"));

			symbol = mm.GetBaseSymbol();
			Assert.IsTrue(symbol.Equals("m"));

			symbol = sys.GetUOM(Unit.SQUARE_METRE).GetBaseSymbol();
			Assert.IsTrue(symbol.Equals("m" + sq));

			symbol = sys.GetUOM(Unit.CUBIC_METRE).GetBaseSymbol();
			Assert.IsTrue(symbol.Equals("m" + cu));

			symbol = sys.GetUOM(Unit.KELVIN).GetBaseSymbol();
			sb.Append("degK");
			Assert.IsTrue(symbol.Equals(sb.ToString()));

			symbol = sys.GetUOM(Unit.CELSIUS).GetBaseSymbol();
			Assert.IsTrue(symbol.Equals(sb.ToString()));

			symbol = sys.GetUOM(Unit.CELSIUS).Symbol;
			sb = new StringBuilder();
			sb.Append("degC");
			Assert.IsTrue(symbol.Equals(sb.ToString()));

			symbol = sys.GetUOM(Unit.GRAM).GetBaseSymbol();
			Assert.IsTrue(symbol.Equals("kg"));

			UnitOfMeasure kg = sys.GetUOM(Unit.KILOGRAM);
			symbol = kg.Symbol;
			Assert.IsTrue(symbol.Equals("kg"));

			symbol = kg.GetBaseSymbol();
			Assert.IsTrue(symbol.Equals("kg"));

			symbol = sys.GetUOM(Unit.CUBIC_METRE).GetBaseSymbol();
			Assert.IsTrue(symbol.Equals("m" + cu));

			symbol = sys.GetUOM(Unit.LITRE).GetBaseSymbol();
			Assert.IsTrue(symbol.Equals("m" + cu));

			symbol = sys.GetUOM(Unit.NEWTON).GetBaseSymbol();
			sb = new StringBuilder();
			sb.Append("kg").Append(times).Append("m/s").Append(sq);
			Assert.IsTrue(symbol.Equals(sb.ToString()));

			symbol = sys.GetUOM(Unit.WATT).GetBaseSymbol();
			sb = new StringBuilder();
			sb.Append("kg").Append(times).Append("m").Append(sq).Append("/s").Append((char)(char)0xB3);
			Assert.IsTrue(symbol.Equals(sb.ToString()));

			symbol = sys.GetUOM(Unit.NEWTON_METRE).GetBaseSymbol();
			sb = new StringBuilder();
			sb.Append("kg").Append(times).Append("m").Append(sq).Append("/s").Append(sq);
			Assert.IsTrue(symbol.Equals(sb.ToString()));

			symbol = sys.GetUOM(Unit.VOLT).GetBaseSymbol();
			sb = new StringBuilder();
			sb.Append("kg").Append(times).Append("m").Append(sq).Append("/(A").Append(times).Append("s").Append(cu)
						.Append(')');
			Assert.IsTrue(symbol.Equals(sb.ToString()));

			symbol = sys.GetUOM(Unit.OHM).GetBaseSymbol();
			sb = new StringBuilder();
			sb.Append("kg").Append(times).Append("m").Append(sq).Append("/(A").Append(sq).Append(times).Append("s")
						.Append(cu).Append(')');
			Assert.IsTrue(symbol.Equals(sb.ToString()));

			symbol = sys.GetUOM(Unit.WEBER).GetBaseSymbol();
			sb = new StringBuilder();
			sb.Append("kg").Append(times).Append("m").Append(sq).Append("/(A").Append(times).Append("s").Append(sq)
						.Append(')');
			Assert.IsTrue(symbol.Equals(sb.ToString()));

			symbol = sys.GetUOM(Unit.MOLE).Symbol;
			Assert.IsTrue(symbol.Equals("mol"));

			symbol = sys.GetUOM(Unit.RADIAN).Symbol;
			Assert.IsTrue(symbol.Equals("rad"));

			symbol = sys.GetUOM(Unit.RADIAN).GetBaseSymbol();
			Assert.IsTrue(symbol.Equals("1"));

			symbol = sys.GetUOM(Unit.STERADIAN).Symbol;
			Assert.IsTrue(symbol.Equals("sr"));

			symbol = sys.GetUOM(Unit.STERADIAN).GetBaseSymbol();
			Assert.IsTrue(symbol.Equals("1"));

			symbol = sys.GetUOM(Unit.CANDELA).GetBaseSymbol();
			Assert.IsTrue(symbol.Equals("cd"));

			symbol = sys.GetUOM(Unit.LUMEN).GetBaseSymbol();
			Assert.IsTrue(symbol.Equals("cd"));

			symbol = sys.GetUOM(Unit.LUMEN).Symbol;
			Assert.IsTrue(symbol.Equals("lm"));

			symbol = sys.GetUOM(Unit.LUX).GetBaseSymbol();
			sb = new StringBuilder();
			sb.Append("cd/m").Append((char)(char)0xB2);
			Assert.IsTrue(symbol.Equals(sb.ToString()));

			symbol = sys.GetUOM(Unit.BECQUEREL).GetBaseSymbol();
			Assert.IsTrue(symbol.Equals("Bq"));

			symbol = sys.GetUOM(Unit.BECQUEREL).Symbol;
			Assert.IsTrue(symbol.Equals("Bq"));

			symbol = sys.GetUOM(Unit.GRAY).GetBaseSymbol();
			sb = new StringBuilder();
			sb.Append("m").Append((char)(char)0xB2).Append("/s").Append((char)(char)0xB2);
			Assert.IsTrue(symbol.Equals(sb.ToString()));

			symbol = sys.GetUOM(Unit.HERTZ).GetBaseSymbol();
			Assert.IsTrue(symbol.Equals("1/s"));

			Assert.IsTrue(sys.GetUOM(Unit.KATAL).GetBaseSymbol().Equals("mol/s"));
		}


		[TestMethod]
		public void TestConversions1()
		{

			UnitOfMeasure m = sys.GetUOM(Unit.METRE);
			UnitOfMeasure cm = sys.GetUOM(Prefix.CENTI, m);
			UnitOfMeasure N = sys.GetUOM(Unit.NEWTON);
			UnitOfMeasure Nm = sys.GetUOM(Unit.NEWTON_METRE);
			UnitOfMeasure mps = sys.GetUOM(Unit.METRE_PER_SEC);
			UnitOfMeasure sqm = sys.GetUOM(Unit.SQUARE_METRE);
			UnitOfMeasure mm = sys.CreateProductUOM(UnitType.AREA, "mxm", "mTimesm", "", m, m);
			UnitOfMeasure mcm = sys.CreateProductUOM(UnitType.AREA, "mxcm", "mxcm", "", m, cm);
			UnitOfMeasure s2 = sys.GetUOM(Unit.SQUARE_SECOND);
			UnitOfMeasure minOverSec = sys.CreateQuotientUOM(UnitType.TIME, "minsec", "min/sec", "", sys.GetMinute(),
					sys.GetSecond());

			UnitOfMeasure minOverSecTimesSec = sys.CreateProductUOM(UnitType.TIME, "minOverSecTimesSec",
					"minOverSecTimesSec", "minOverSecTimesSec", minOverSec, sys.GetSecond());

			UnitOfMeasure inch = sys.GetUOM(Unit.INCH);
			UnitOfMeasure ft = sys.GetUOM(Unit.FOOT);
			UnitOfMeasure lbf = sys.GetUOM(Unit.POUND_FORCE);
			UnitOfMeasure fph = sys.CreateQuotientUOM(UnitType.VELOCITY, "fph", "ft/hr", "feet per hour", ft,
					sys.GetHour());
			UnitOfMeasure ftlb = sys.GetUOM(Unit.FOOT_POUND_FORCE);
			UnitOfMeasure sqft = sys.GetUOM(Unit.SQUARE_FOOT);

			UnitOfMeasure oneDivSec = sys.GetOne().Divide(sys.GetSecond());
			UnitOfMeasure Inverted = oneDivSec.Invert();
			Assert.IsTrue(Inverted.Equals(sys.GetSecond()));

			UnitOfMeasure perSec = sys.CreatePowerUOM(UnitType.TIME, "per second", "perSec", null, sys.GetSecond(), -1);
			UnitOfMeasure mult = perSec.Multiply(sys.GetUOM(Unit.SECOND));
			Assert.IsTrue(mult.GetBaseSymbol().Equals(sys.GetUOM(Unit.ONE).Symbol));

			UnitOfMeasure u = sys.GetSecond().Invert();
			Assert.IsTrue(u.GetScalingFactor().Equals(oneDivSec.GetScalingFactor()));

			Inverted = u.Invert();
			Assert.IsTrue(Inverted.Equals(sys.GetSecond()));

			UnitOfMeasure oneOverSec = sys.GetBaseUOM("1/s");
			Assert.IsTrue(oneOverSec.GetBaseSymbol().Equals(oneDivSec.GetBaseSymbol()));

			Inverted = oneOverSec.Invert();
			Assert.IsTrue(Inverted.GetBaseSymbol().Equals(sys.GetSecond().GetBaseSymbol()));

			UnitOfMeasure minTimesSec = sys.CreateProductUOM(UnitType.TIME_SQUARED, "minsec", "minxsec",
					"minute times a second", sys.GetMinute(), sys.GetSecond());

			UnitOfMeasure sqMin = sys.GetUOM("min^2");
			if (sqMin == null)
			{
				sqMin = sys.CreatePowerUOM(UnitType.TIME_SQUARED, "square minute", "min^2", null, sys.GetUOM(Unit.MINUTE),
						2);
			}

			UnitOfMeasure perMin = sys.CreatePowerUOM(UnitType.TIME, "per minute", "perMin", null, sys.GetMinute(), -1);

			UnitOfMeasure perMin2 = sys.CreatePowerUOM(UnitType.TIME, "per minute squared", "perMin^2", null,
					sys.GetMinute(), -2);

			u = perMin2.Invert();
			Assert.IsTrue(u.GetBaseSymbol().Equals(sqMin.GetBaseSymbol()));

			u = perMin.Invert();
			double bd = u.GetConversionFactor(sys.GetMinute());
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = u.GetConversionFactor(sys.GetSecond());
			Assert.IsTrue(IsCloseTo(bd, 60, DELTA6));

			try
			{
				m.GetConversionFactor(null);
				Assert.Fail("null");
			}
			catch (Exception)
			{
			}

			try
			{
				m.Multiply(null);
				Assert.Fail("null");
			}
			catch (Exception)
			{
			}

			// scalar
			bd = m.GetConversionFactor(m);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));
			Assert.IsTrue(m.Equals(m));

			bd = m.GetConversionFactor(cm);
			Assert.IsTrue(IsCloseTo(bd, 100, DELTA6));
			Assert.IsTrue(m.Equals(m));
			Assert.IsTrue(cm.Equals(cm));
			Assert.IsTrue((!m.Equals(cm)));

			bd = m.GetConversionFactor(cm);
			Assert.IsTrue(IsCloseTo(bd, 100, DELTA6));

			bd = cm.GetConversionFactor(m);
			Assert.IsTrue(IsCloseTo(bd, 0.01, DELTA6));

			bd = m.GetConversionFactor(cm);
			Assert.IsTrue(IsCloseTo(bd, 100, DELTA6));

			bd = m.GetConversionFactor(inch);
			Assert.IsTrue(IsCloseTo(bd, 39.37007874015748, DELTA6));

			bd = inch.GetConversionFactor(m);
			Assert.IsTrue(IsCloseTo(bd, 0.0254, DELTA6));

			bd = m.GetConversionFactor(ft);
			Assert.IsTrue(IsCloseTo(bd, 3.280839895013123, DELTA6));

			bd = ft.GetConversionFactor(m);
			Assert.IsTrue(IsCloseTo(bd, 0.3048, DELTA6));

			Quantity g = sys.GetQuantity(Constant.GRAVITY).Convert(sys.GetUOM(Unit.FEET_PER_SEC_SQUARED));
			bd = g.Amount;
			Assert.IsTrue(IsCloseTo(bd, 32.17404855, DELTA6));

			bd = lbf.GetConversionFactor(N);
			Assert.IsTrue(IsCloseTo(bd, 4.448221615, DELTA6));

			bd = N.GetConversionFactor(lbf);
			Assert.IsTrue(IsCloseTo(bd, 0.2248089430997105, DELTA6));

			// product
			bd = Nm.GetConversionFactor(ftlb);
			Assert.IsTrue(IsCloseTo(bd, 0.7375621492772656, DELTA6));

			bd = ftlb.GetConversionFactor(Nm);
			Assert.IsTrue(IsCloseTo(bd, 1.3558179483314004, DELTA6));

			// quotient
			UnitOfMeasure one = sys.GetOne();
			bd = minOverSec.GetConversionFactor(one);
			Assert.IsTrue(IsCloseTo(bd, 60, DELTA6));

			bd = one.GetConversionFactor(minOverSec);
			Assert.IsTrue(IsCloseTo(bd, 0.0166666666666667, DELTA6));

			bd = mps.GetConversionFactor(fph);
			Assert.IsTrue(IsCloseTo(bd, 11811.02362204724, DELTA6));

			bd = fph.GetConversionFactor(mps);
			Assert.IsTrue(IsCloseTo(bd, 8.46666666666667E-05, DELTA6));

			// power
			bd = sqm.GetConversionFactor(sqft);
			Assert.IsTrue(IsCloseTo(bd, 10.76391041670972, DELTA6));

			bd = sqft.GetConversionFactor(sqm);
			Assert.IsTrue(IsCloseTo(bd, 0.09290304, DELTA6));

			// mixed
			bd = mm.GetConversionFactor(sqm);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = sqm.GetConversionFactor(mm);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = mcm.GetConversionFactor(sqm);
			Assert.IsTrue(IsCloseTo(bd, 0.01, DELTA6));

			bd = sqm.GetConversionFactor(mcm);
			Assert.IsTrue(IsCloseTo(bd, 100, DELTA6));

			bd = minTimesSec.GetConversionFactor(s2);
			Assert.IsTrue(IsCloseTo(bd, 60, DELTA6));

			bd = s2.GetConversionFactor(minTimesSec);
			Assert.IsTrue(IsCloseTo(bd, 0.0166666666666667, DELTA6));

			bd = minTimesSec.GetConversionFactor(sqMin);
			Assert.IsTrue(IsCloseTo(bd, 0.0166666666666667, DELTA6));

			bd = sqMin.GetConversionFactor(minTimesSec);
			Assert.IsTrue(IsCloseTo(bd, 60, DELTA6));

			bd = minOverSecTimesSec.GetConversionFactor(sys.GetSecond());
			Assert.IsTrue(IsCloseTo(bd, 60, DELTA6));

		}

		[TestMethod]
		public void TestConversions2()
		{
			double bd;

			sys.UnregisterUnit(sys.GetUOM(Unit.CUBIC_INCH));
			UnitOfMeasure ft = sys.GetUOM(Unit.FOOT);
			UnitOfMeasure ft2 = sys.GetUOM(Unit.SQUARE_FOOT);
			UnitOfMeasure ft3 = sys.GetUOM(Unit.CUBIC_FOOT);

			UnitOfMeasure cubicFt = ft2.Multiply(ft);
			bd = cubicFt.GetConversionFactor(ft3);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			UnitOfMeasure m3 = sys.GetUOM(Unit.CUBIC_METRE);
			UnitOfMeasure degree = sys.GetUOM(Unit.DEGREE);
			UnitOfMeasure arcsec = sys.GetUOM(Unit.ARC_SECOND);
			UnitOfMeasure radian = sys.GetUOM(Unit.RADIAN);
			UnitOfMeasure kgPerM3 = sys.GetUOM(Unit.KILOGRAM_PER_CU_METRE);
			UnitOfMeasure mps = sys.GetUOM(Unit.METRE_PER_SEC);
			UnitOfMeasure pascal = sys.GetUOM(Unit.PASCAL);
			UnitOfMeasure s2 = sys.GetUOM(Unit.SQUARE_SECOND);
			UnitOfMeasure joule = sys.GetUOM(Unit.JOULE);
			UnitOfMeasure rpm = sys.GetUOM(Unit.REV_PER_MIN);
			UnitOfMeasure rps = sys.GetUOM(Unit.RAD_PER_SEC);
			UnitOfMeasure m3s = sys.GetUOM(Unit.CUBIC_METRE_PER_SEC);
			UnitOfMeasure ms2 = sys.GetUOM(Unit.METRE_PER_SEC_SQUARED);

			UnitOfMeasure lbm = sys.GetUOM(Unit.POUND_MASS);
			UnitOfMeasure acreFoot = sys.CreateProductUOM(UnitType.VOLUME, "acreFoot", "ac-ft", "", sys.GetUOM(Unit.ACRE),
						sys.GetUOM(Unit.FOOT));
			UnitOfMeasure lbmPerFt3 = sys.CreateQuotientUOM(UnitType.DENSITY, "lbmPerFt3", "lbm/ft^3", null, lbm, ft3);
			UnitOfMeasure fps = sys.GetUOM(Unit.FEET_PER_SEC);
			UnitOfMeasure knot = sys.GetUOM(Unit.KNOT);
			UnitOfMeasure btu = sys.GetUOM(Unit.BTU);

			UnitOfMeasure miphs = sys.CreateScalarUOM(UnitType.ACCELERATION, "mph/sec", "mi/hr-sec",
						"mile per hour per second");
			miphs.SetConversion(1.466666666666667, sys.GetUOM(Unit.FEET_PER_SEC_SQUARED));

			UnitOfMeasure inHg = sys.CreateScalarUOM(UnitType.PRESSURE, "inHg", "inHg", "inHg");
			inHg.SetConversion(3386.389, pascal);

			Quantity atm = new Quantity(1, Unit.ATMOSPHERE).Convert(Unit.PASCAL);
			Assert.IsTrue(IsCloseTo(atm.Amount, 101325, DELTA6));

			UnitOfMeasure ft2ft = sys.CreateProductUOM(UnitType.VOLUME, "ft2ft", "ft2ft", null, ft2, ft);

			UnitOfMeasure hrsec = sys.CreateScalarUOM(UnitType.TIME_SQUARED, "", "hr.sec", "");
			hrsec.SetConversion(3600, sys.GetUOM(Unit.SQUARE_SECOND));
			bd = hrsec.GetConversionFactor(s2);
			Assert.IsTrue(IsCloseTo(bd, 3600, DELTA6));

			bd = s2.GetConversionFactor(hrsec);
			Assert.IsTrue(IsCloseTo(bd, 2.777777777777778E-04, DELTA6));

			bd = ft2ft.GetConversionFactor(m3);
			Assert.IsTrue(IsCloseTo(bd, 0.028316846592, DELTA6));

			bd = m3.GetConversionFactor(ft2ft);
			Assert.IsTrue(IsCloseTo(bd, 35.31466672148859, DELTA6));

			bd = acreFoot.GetConversionFactor(m3);
			Assert.IsTrue(IsCloseTo(bd, 1233.48183754752, DELTA6));

			bd = m3.GetConversionFactor(acreFoot);
			Assert.IsTrue(IsCloseTo(bd, 8.107131937899125E-04, DELTA6));

			bd = degree.GetConversionFactor(radian);
			Assert.IsTrue(IsCloseTo(bd, 0.01745329251994329, DELTA6));

			bd = radian.GetConversionFactor(degree);
			Assert.IsTrue(IsCloseTo(bd, 57.29577951308264, DELTA6));

			bd = arcsec.GetConversionFactor(degree);
			Assert.IsTrue(IsCloseTo(bd, 2.777777777777778E-4, DELTA6));

			bd = degree.GetConversionFactor(arcsec);
			Assert.IsTrue(IsCloseTo(bd, 3600, DELTA6));

			bd = lbmPerFt3.GetConversionFactor(kgPerM3);
			Assert.IsTrue(IsCloseTo(bd, 16.01846337, DELTA6));

			bd = kgPerM3.GetConversionFactor(lbmPerFt3);
			Assert.IsTrue(IsCloseTo(bd, 0.0624279605915783, DELTA6));

			bd = rpm.GetConversionFactor(rps);
			Assert.IsTrue(IsCloseTo(bd, 0.104719755, DELTA6));

			bd = rps.GetConversionFactor(rpm);
			Assert.IsTrue(IsCloseTo(bd, 9.549296596425383, DELTA6));

			bd = mps.GetConversionFactor(fps);
			Assert.IsTrue(IsCloseTo(bd, 3.280839895013123, DELTA6));

			bd = fps.GetConversionFactor(mps);
			Assert.IsTrue(IsCloseTo(bd, 0.3048, DELTA6));

			bd = knot.GetConversionFactor(mps);
			Assert.IsTrue(IsCloseTo(bd, 0.5147733333333333, DELTA6));

			bd = mps.GetConversionFactor(knot);
			Assert.IsTrue(IsCloseTo(bd, 1.942602569415665, DELTA6));

			UnitOfMeasure usGal = sys.GetUOM(Unit.US_GALLON);
			UnitOfMeasure gph = sys.CreateQuotientUOM(UnitType.VOLUMETRIC_FLOW, "gph", "gal/hr", "gallons per hour", usGal,
					sys.GetHour());

			bd = gph.GetConversionFactor(m3s);
			Assert.IsTrue(IsCloseTo(bd, 1.051503273E-06, DELTA6));

			bd = m3s.GetConversionFactor(gph);
			Assert.IsTrue(IsCloseTo(bd, 951019.3884893342, DELTA6));

			bd = miphs.GetConversionFactor(ms2);
			Assert.IsTrue(IsCloseTo(bd, 0.44704, DELTA6));

			bd = ms2.GetConversionFactor(miphs);
			Assert.IsTrue(IsCloseTo(bd, 2.236936292054402, DELTA6));

			bd = pascal.GetConversionFactor(inHg);
			Assert.IsTrue(IsCloseTo(bd, 2.952998016471232E-04, DELTA6));

			bd = inHg.GetConversionFactor(pascal);
			Assert.IsTrue(IsCloseTo(bd, 3386.389, DELTA6));

			bd = atm.Convert(inHg).Amount;
			Assert.IsTrue(IsCloseTo(bd, 29.92125240189478, DELTA6));

			bd = inHg.GetConversionFactor(atm.UOM);
			Assert.IsTrue(IsCloseTo(bd, 3386.389, DELTA6));

			bd = btu.GetConversionFactor(joule);
			Assert.IsTrue(IsCloseTo(bd, 1055.05585262, DELTA6));

			bd = joule.GetConversionFactor(btu);
			Assert.IsTrue(IsCloseTo(bd, 9.478171203133172E-04, DELTA6));

		}

		[TestMethod]
		public void TestConversions3()
		{
			UnitOfMeasure weber = sys.GetUOM(Unit.WEBER);
			UnitOfMeasure coulomb = sys.GetUOM(Unit.COULOMB);
			UnitOfMeasure second = sys.GetSecond();
			UnitOfMeasure volt = sys.GetUOM(Unit.VOLT);
			UnitOfMeasure watt = sys.GetUOM(Unit.WATT);
			UnitOfMeasure amp = sys.GetUOM(Unit.AMPERE);
			UnitOfMeasure farad = sys.GetUOM(Unit.FARAD);
			UnitOfMeasure ohm = sys.GetUOM(Unit.OHM);
			UnitOfMeasure henry = sys.GetUOM(Unit.HENRY);
			UnitOfMeasure sr = sys.GetUOM(Unit.STERADIAN);
			UnitOfMeasure cd = sys.GetUOM(Unit.CANDELA);
			UnitOfMeasure lumen = sys.GetUOM(Unit.LUMEN);
			UnitOfMeasure gray = sys.GetUOM(Unit.GRAY);
			UnitOfMeasure sievert = sys.GetUOM(Unit.SIEVERT);

			UnitOfMeasure WeberPerSec = sys.CreateQuotientUOM(UnitType.ELECTROMOTIVE_FORCE, "W/s", "W/s", null, weber,
						second);
			UnitOfMeasure WeberPerAmp = sys.CreateQuotientUOM(UnitType.ELECTRIC_INDUCTANCE, "W/A", "W/A", null, weber, amp);
			UnitOfMeasure fTimesV = sys.CreateProductUOM(UnitType.ELECTRIC_CHARGE, "FxV", "FxV", null, farad, volt);
			UnitOfMeasure WPerAmp = sys.CreateQuotientUOM(UnitType.ELECTROMOTIVE_FORCE, "Watt/A", "Watt/A", null, watt,
						amp);
			UnitOfMeasure VPerA = sys.CreateQuotientUOM(UnitType.ELECTRIC_RESISTANCE, "V/A", "V/A", null, volt, amp);
			UnitOfMeasure CPerV = sys.CreateQuotientUOM(UnitType.ELECTRIC_CAPACITANCE, "C/V", "C/V", null, coulomb, volt);
			UnitOfMeasure VTimesSec = sys.CreateProductUOM(UnitType.MAGNETIC_FLUX, "Vxs", "Vxs", null, volt, second);
			UnitOfMeasure cdTimesSr = sys.CreateProductUOM(UnitType.LUMINOUS_FLUX, "cdxsr", "cdxsr", null, cd, sr);

			double bd = fTimesV.GetConversionFactor(coulomb);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = coulomb.GetConversionFactor(fTimesV);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = WeberPerSec.GetConversionFactor(volt);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = volt.GetConversionFactor(WeberPerSec);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = volt.GetConversionFactor(WPerAmp);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = WPerAmp.GetConversionFactor(volt);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = ohm.GetConversionFactor(VPerA);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = VPerA.GetConversionFactor(ohm);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = farad.GetConversionFactor(CPerV);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = CPerV.GetConversionFactor(farad);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = weber.GetConversionFactor(VTimesSec);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = VTimesSec.GetConversionFactor(weber);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = henry.GetConversionFactor(WeberPerAmp);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = WeberPerAmp.GetConversionFactor(henry);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = lumen.GetConversionFactor(cdTimesSr);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = cdTimesSr.GetConversionFactor(lumen);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			try
			{
				bd = gray.GetConversionFactor(sievert);
				Assert.Fail("No conversion");
			}
			catch (Exception)
			{
			}

			try
			{
				bd = sievert.GetConversionFactor(gray);
				Assert.Fail("No conversion");
			}
			catch (Exception)
			{
			}

		}

		[TestMethod]
		public void TestConversions4()
		{

			UnitOfMeasure K = sys.GetUOM(Unit.KELVIN);
			UnitOfMeasure C = sys.GetUOM(Unit.CELSIUS);

			UnitOfMeasure R = sys.GetUOM(Unit.RANKINE);
			UnitOfMeasure F = sys.GetUOM(Unit.FAHRENHEIT);

			double fiveNinths = (double)5 / (double)9;
			double nineFifths = 1.8;

			// K to C
			double bd = K.GetConversionFactor(C);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = C.GetConversionFactor(K);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			// R to F
			bd = R.GetConversionFactor(F);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = F.GetConversionFactor(R);
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			// C to F
			bd = F.GetConversionFactor(C);
			Assert.IsTrue(IsCloseTo(bd, fiveNinths, DELTA6));

			bd = C.GetConversionFactor(F);
			Assert.IsTrue(IsCloseTo(bd, nineFifths, DELTA6));

			// K to R
			bd = K.GetConversionFactor(R);
			Assert.IsTrue(IsCloseTo(bd, nineFifths, DELTA6));

			bd = F.GetConversionFactor(K);
			Assert.IsTrue(IsCloseTo(bd, fiveNinths, DELTA6));

			// Invert diopters to metre
			Quantity from = new Quantity(10, sys.GetUOM(Unit.DIOPTER));
			Quantity Inverted = from.Invert();
			Assert.IsTrue(IsCloseTo(Inverted.Amount, 0.1, DELTA6));

			UnitOfMeasure u = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "t*4", "t*4", "", K, 4);
			Assert.IsTrue(u != null);

			try
			{
				u = C.Multiply(C);
				Assert.Fail("Can't Multiply Celcius");
			}
			catch (Exception)
			{
				// ignore
			}

			u = K.Divide(K);
			Assert.IsTrue(u.GetBaseSymbol().Equals(sys.GetOne().GetBaseSymbol()));

			// hectare to acre
			UnitOfMeasure ha = sys.GetUOM(Unit.HECTARE);
			from = new Quantity(1, ha);
			Quantity to = from.Convert(Unit.ACRE);
			Assert.IsTrue(IsCloseTo(to.Amount, 2.47105, DELTA5));
		}

		[TestMethod]
		public void TestPerformance()
		{
			int its = 1000;

			UnitOfMeasure metre = sys.GetUOM(Unit.METRE);
			UnitOfMeasure cm = sys.GetUOM(Prefix.CENTI, sys.GetUOM(Unit.METRE));
			UnitOfMeasure ft = sys.GetUOM(Unit.FOOT);

			Quantity q1 = new Quantity(10, metre);
			Quantity q2 = new Quantity(2, cm);

			for (int i = 0; i < its; i++)
			{
				q1.Add(q2);
			}

			for (int i = 0; i < its; i++)
			{
				q1.Subtract(q2);
			}

			for (int i = 0; i < its; i++)
			{
				q1.Multiply(q2);
			}

			for (int i = 0; i < its; i++)
			{
				q1.Divide(q2);
			}

			for (int i = 0; i < its; i++)
			{
				q1.Convert(ft);
			}
		}

		[TestMethod]
		public void TestScaledUnits()
		{
			UnitOfMeasure m = sys.GetUOM(Unit.METRE);

			// mega metre
			UnitOfMeasure mm = sys.GetUOM(Prefix.MEGA, m);

			Quantity qmm = new Quantity(1, mm);
			Quantity qm = qmm.Convert(m);
			Assert.IsTrue(IsCloseTo(qm.Amount, 1.0E+06, DELTA6));

			UnitOfMeasure mm2 = sys.GetUOM(Prefix.MEGA, m);
			Assert.IsTrue(mm.Equals(mm2));

			// centilitre
			UnitOfMeasure litre = sys.GetUOM(Unit.LITRE);
			UnitOfMeasure cL = sys.GetUOM(Prefix.CENTI, litre);
			Quantity qL = new Quantity(1, litre);
			Quantity qcL = qL.Convert(cL);
			Assert.IsTrue(IsCloseTo(qcL.Amount, 100, DELTA6));

			// a mega buck
			UnitOfMeasure buck = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "buck", "$", "one US dollar");
			UnitOfMeasure megabuck = sys.GetUOM(Prefix.MEGA, buck);
			Quantity qmb = new Quantity(10, megabuck);
			Quantity qb = qmb.Convert(buck);
			Assert.IsTrue(IsCloseTo(qb.Amount, 1.0E+07, DELTA6));

			// kilogram vs. scaled gram
			UnitOfMeasure kgm = sys.GetUOM(Prefix.KILO, sys.GetUOM(Unit.GRAM));
			UnitOfMeasure kg = sys.GetUOM(Unit.KILOGRAM);
			Assert.IsTrue(kgm.Equals(kg));

			// kilo and megabytes
			UnitOfMeasure kiB = sys.GetUOM(Prefix.KIBI, sys.GetUOM(Unit.BYTE));
			UnitOfMeasure miB = sys.GetUOM(Prefix.MEBI, sys.GetUOM(Unit.BYTE));
			Quantity qmB = new Quantity(1, miB);
			Quantity qkB = qmB.Convert(kiB);
			Assert.IsTrue(IsCloseTo(qkB.Amount, 1024, DELTA6));
		}

		[TestMethod]
		public void TestPowers()
		{
			sys.ClearCache();

			double bd;

			Quantity q1 = null;
			Quantity q2 = null;

			UnitType t;

			UnitOfMeasure u = null;
			UnitOfMeasure u2 = null;

			UnitOfMeasure min = sys.GetMinute();
			UnitOfMeasure s = sys.GetSecond();
			UnitOfMeasure sm1 = s.Invert();
			UnitOfMeasure s2 = sys.GetUOM(Unit.SQUARE_SECOND);
			UnitOfMeasure min2 = sys.CreatePowerUOM(UnitType.TIME_SQUARED, "sqMin", "min'2", null, min, 2);
			UnitOfMeasure sqs = sys.CreatePowerUOM(UnitType.TIME_SQUARED, "sqSec", "s'2", null, s, 2);
			UnitOfMeasure sminus1 = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "sminus1", "s'-1", null, s, -1);
			UnitOfMeasure minminus1Q = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "minminus1Q", "minQ'-1", null,
						sys.GetOne(), min);
			UnitOfMeasure minminus1 = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "minminus1", "min'-1", null, min, -1);
			UnitOfMeasure newton = sys.GetUOM(Unit.NEWTON);
			UnitOfMeasure newtonm1 = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "Nminus1", "N'-1", null, newton, -1);
			UnitOfMeasure inch = sys.GetUOM(Unit.INCH);
			UnitOfMeasure ft = sys.GetUOM(Unit.FOOT);
			UnitOfMeasure ftm1 = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "ftm1", "ft'-1", null, ft, -1);
			UnitOfMeasure inm1 = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "inm1", "in'-1", null, inch, -1);
			UnitOfMeasure ui = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "ui", "ui", "");
			UnitOfMeasure uj = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "uj", "uj", "");
			UnitOfMeasure ixj = sys.CreateProductUOM(UnitType.UNCLASSIFIED, "ixj", "ixj", "", ui, uj);
			UnitOfMeasure oneOveri = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "oneOveri", "oneOveri", "", sys.GetOne(),
						ui);
			UnitOfMeasure oneOverj = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "oneOverj", "oneOverj", "", sys.GetOne(),
						uj);
			UnitOfMeasure ixjm1 = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "ixjm1", "ixjm1", "", ixj, -1);
			UnitOfMeasure hz = sys.GetUOM(Unit.HERTZ);

			UnitOfMeasure ij = oneOveri.Multiply(oneOverj);
			Assert.IsTrue(ij.Equals(ixjm1));

			bd = min2.GetConversionFactor(s2);
			Assert.IsTrue(IsCloseTo(bd, 3600, DELTA6));

			bd = s2.GetConversionFactor(min2);
			Assert.IsTrue(IsCloseTo(bd, 2.777777777777778e-4, DELTA6));

			u = sys.GetBaseUOM(sm1.Symbol);
			Assert.IsTrue(u != null);
			u = sys.GetUOM(sm1.GetBaseSymbol());

			u = sys.GetOne().Divide(min);
			bd = u.GetScalingFactor();
			Assert.IsTrue(IsCloseTo(bd, 0.0166666666666667, DELTA6));
			bd = u.GetConversionFactor(sm1);
			Assert.IsTrue(IsCloseTo(bd, 0.0166666666666667, DELTA6));

			u = ftm1.Multiply(ft);
			Assert.IsTrue(u.GetBaseSymbol().Equals(sys.GetOne().Symbol));

			u = ft.Multiply(inm1);
			Assert.IsTrue(IsCloseTo(u.GetScalingFactor(), 12, DELTA6));

			u = inm1.Multiply(ft);
			Assert.IsTrue(IsCloseTo(u.GetScalingFactor(), 12, DELTA6));

			u = s.Multiply(minminus1);
			Assert.IsTrue(IsCloseTo(u.GetScalingFactor(), 0.0166666666666667, DELTA6));

			u = minminus1.Multiply(s);
			Assert.IsTrue(IsCloseTo(u.GetScalingFactor(), 0.0166666666666667, DELTA6));

			u = s.Multiply(minminus1Q);
			Assert.IsTrue(IsCloseTo(u.GetScalingFactor(), 0.0166666666666667, DELTA6));

			u = minminus1Q.Multiply(s);
			Assert.IsTrue(IsCloseTo(u.GetScalingFactor(), 0.0166666666666667, DELTA6));

			u = ftm1.Multiply(inch);
			Assert.IsTrue(IsCloseTo(u.GetScalingFactor(), 0.0833333333333333, DELTA6));

			u = inch.Multiply(ftm1);
			Assert.IsTrue(IsCloseTo(u.GetScalingFactor(), 0.0833333333333333, DELTA6));

			u = newtonm1.Multiply(newton);
			Assert.IsTrue(u.GetBaseSymbol().Equals(sys.GetOne().GetBaseSymbol()));

			u = newton.Multiply(newtonm1);
			Assert.IsTrue(u.GetBaseSymbol().Equals(sys.GetOne().GetBaseSymbol()));

			u = minminus1.Multiply(s);
			Assert.IsTrue(IsCloseTo(u.GetScalingFactor(), 0.0166666666666667, DELTA6));

			sys.UnregisterUnit(sys.GetUOM(Unit.HERTZ));
			UnitOfMeasure min1 = min.Invert();
			bd = min1.GetScalingFactor();
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			bd = sqs.GetScalingFactor();
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));

			u = sminus1.Multiply(s);
			Assert.IsTrue(u.GetBaseSymbol().Equals(sys.GetOne().Symbol));

			u = sys.GetOne().Divide(min);
			bd = u.GetScalingFactor();
			Assert.IsTrue(IsCloseTo(bd, 1, DELTA6));
			bd = u.GetConversionFactor(sm1);
			Assert.IsTrue(IsCloseTo(bd, 0.0166666666666667, DELTA6));

			t = s2.UOMType;

			t = min.UOMType;

			u = sys.GetOne().Multiply(min);
			bd = u.GetConversionFactor(s);

			t = min2.UOMType;

			u = min2.Divide(min);
			t = u.UOMType;
			Assert.IsTrue(t.Equals(UnitType.TIME));

			u = min.Multiply(min);
			Assert.IsTrue(IsCloseTo(u.GetScalingFactor(), 3600, DELTA6));
			Assert.IsTrue(u.GetAbscissaUnit().Equals(s2));
			Assert.IsTrue(IsCloseTo(u.GetOffset(), 0, DELTA6));
			t = u.UOMType;
			Assert.IsTrue(t.Equals(UnitType.TIME_SQUARED));

			u2 = sys.GetOne().Divide(min);
			Assert.IsTrue(IsCloseTo(u2.GetScalingFactor(), 1, DELTA6));

			u = u2.Multiply(u2);
			double sf = u.GetScalingFactor();
			Assert.IsTrue(IsCloseTo(sf, 1, DELTA6));

			q1 = new Quantity(1, u2);
			q2 = q1.Convert(hz);
			Assert.IsTrue(IsCloseTo(q2.Amount, 0.0166666666666667, DELTA6));

			q1 = new Quantity(1, u);
			q2 = q1.Convert(s2.Invert());
			Assert.IsTrue(IsCloseTo(q2.Amount, 2.777777777777778e-4, DELTA6));

			u2 = u2.Divide(min);
			q1 = new Quantity(1, u2);
			q2 = q1.Convert(s2.Invert());
			Assert.IsTrue(IsCloseTo(q2.Amount, 2.777777777777778e-4, DELTA6));

			u2 = u2.Invert();
			Assert.IsTrue(u2.GetBaseSymbol().Equals(min2.GetBaseSymbol()));

			q1 = new Quantity(10, u2);
			bd = u2.GetConversionFactor(s2);
			Assert.IsTrue(IsCloseTo(bd, 3600, DELTA6));

			q2 = q1.Convert(s2);
			Assert.IsTrue(q2.UOM.Equals(s2));
			Assert.IsTrue(IsCloseTo(q2.Amount, 36000, DELTA6));

			bd = min.GetConversionFactor(sys.GetSecond());
			Assert.IsTrue(IsCloseTo(bd, 60, DELTA6));

			u = q2.UOM;
			bd = u.GetConversionFactor(min2);
			Assert.IsTrue(IsCloseTo(bd, 2.777777777777778e-4, DELTA6));

			q2 = q2.Convert(min2);
			double amount = q2.Amount;
			Assert.IsTrue(IsCloseTo(amount, 10, DELTA6));
		}

		[TestMethod]
		public void TestInversions()
		{
			UnitOfMeasure uom = null;
			UnitOfMeasure Inverted = null;
			UnitOfMeasure u = null;
			UnitOfMeasure metre = sys.GetUOM(Unit.METRE);

			uom = sys.CreatePowerUOM(metre, -3);
			Inverted = uom.Invert();
			u = uom.Multiply(Inverted);
			Assert.IsTrue(u.GetBaseSymbol().Equals(sys.GetOne().Symbol));

			uom = sys.CreatePowerUOM(metre, 2);
			Inverted = uom.Invert();
			u = uom.Multiply(Inverted);
			Assert.IsTrue(u.GetBaseSymbol().Equals(sys.GetOne().Symbol));

			uom = sys.CreatePowerUOM(metre, -2);
			Inverted = uom.Invert();
			u = uom.Multiply(Inverted);
			Assert.IsTrue(u.GetBaseSymbol().Equals(sys.GetOne().Symbol));

			uom = sys.CreatePowerUOM(metre, 2);
			Inverted = uom.Invert();
			u = uom.Multiply(Inverted);
			Assert.IsTrue(u.GetBaseSymbol().Equals(sys.GetOne().Symbol));

			uom = sys.CreatePowerUOM(metre, 1);
			Inverted = uom.Invert();
			u = uom.Multiply(Inverted);
			Assert.IsTrue(u.GetBaseSymbol().Equals(sys.GetOne().Symbol));

			uom = sys.CreatePowerUOM(metre, -1);
			Inverted = uom.Invert();
			u = uom.Multiply(Inverted);
			Assert.IsTrue(u.GetBaseSymbol().Equals(sys.GetOne().Symbol));

			uom = sys.CreatePowerUOM(metre, -2);
			Inverted = uom.Invert();
			u = uom.Multiply(Inverted);
			Assert.IsTrue(u.GetBaseSymbol().Equals(sys.GetOne().Symbol));

			uom = sys.CreatePowerUOM(metre, -4);
			Inverted = uom.Invert();
			u = uom.Multiply(Inverted);
			Assert.IsTrue(u.GetBaseSymbol().Equals(sys.GetOne().Symbol));
		}

		[TestMethod]
		public void TestMedicalUnits()
		{
			// Equivalent
			UnitOfMeasure eq = sys.GetUOM(Unit.EQUIVALENT);
			UnitOfMeasure litre = sys.GetUOM(Unit.LITRE);
			UnitOfMeasure mEqPerL = sys.CreateQuotientUOM(UnitType.MOLAR_CONCENTRATION, "milliNormal", "mEq/L",
						"solute per litre of solvent ", sys.GetUOM(Prefix.MILLI, eq), litre);
			Quantity testResult = new Quantity(4.9, mEqPerL);
			Assert.IsTrue(testResult.Amount.CompareTo(3.5) == 1);
			Assert.IsTrue(testResult.Amount.CompareTo(5.3) == -1);

			// Unit
			UnitOfMeasure u = sys.GetUOM(Unit.UNIT);
			UnitOfMeasure katal = sys.GetUOM(Unit.KATAL);
			Quantity q1 = new Quantity(1, u);
			Quantity q2 = q1.Convert(sys.GetUOM(Prefix.NANO, katal));
			Assert.IsTrue(IsCloseTo(q2.Amount, 16.666667, DELTA6));

			// blood cell counts
			UnitOfMeasure k = sys.GetUOM(Prefix.KILO, sys.GetOne());
			UnitOfMeasure uL = sys.GetUOM(Prefix.MICRO, Unit.LITRE);
			UnitOfMeasure kul = sys.CreateQuotientUOM(UnitType.MOLAR_CONCENTRATION, "K/uL", "K/uL",
					"thousands per microlitre", k, uL);
			testResult = new Quantity(6.6, kul);
			Assert.IsTrue(testResult.Amount.CompareTo(3.5) == 1);
			Assert.IsTrue(testResult.Amount.CompareTo(12.5) == -1);

			UnitOfMeasure fL = sys.GetUOM(Prefix.FEMTO, Unit.LITRE);
			testResult = new Quantity(90, fL);
			Assert.IsTrue(testResult.Amount.CompareTo(80) == 1);
			Assert.IsTrue(testResult.Amount.CompareTo(100) == -1);

			// TSH
			UnitOfMeasure uIU = sys.GetUOM(Prefix.MICRO, Unit.INTERNATIONAL_UNIT);
			UnitOfMeasure mL = sys.GetUOM(Prefix.MILLI, Unit.LITRE);
			UnitOfMeasure uiuPerml = sys.CreateQuotientUOM(UnitType.MOLAR_CONCENTRATION, "uIU/mL", "uIU/mL",
					"micro IU per millilitre", uIU, mL);
			testResult = new Quantity(2.11, uiuPerml);
			Assert.IsTrue(testResult.Amount.CompareTo(0.40) == 1);
			Assert.IsTrue(testResult.Amount.CompareTo(5.50) == -1);

		}

		[TestMethod]
		public void TestCategory()
		{
			String category = "category";
			UnitOfMeasure m = sys.GetUOM(Unit.METRE);
			m.Category = category;
			Assert.IsTrue(m.Category.Equals(category));
		}

		[TestMethod]
		public void TestMeasurementTypes()
		{
			UnitOfMeasure m = sys.GetUOM(Unit.METRE);
			UnitOfMeasure mps = sys.GetUOM(Unit.METRE_PER_SEC);
			UnitOfMeasure n = sys.GetUOM(Unit.NEWTON_METRE);
			UnitOfMeasure a = sys.GetUOM(Unit.SQUARE_METRE);

			Assert.IsTrue(m.GetMeasurementType().Equals(UnitOfMeasure.MeasurementType.SCALAR));
			Assert.IsTrue(mps.GetMeasurementType().Equals(UnitOfMeasure.MeasurementType.QUOTIENT));
			Assert.IsTrue(n.GetMeasurementType().Equals(UnitOfMeasure.MeasurementType.PRODUCT));
			Assert.IsTrue(a.GetMeasurementType().Equals(UnitOfMeasure.MeasurementType.POWER));
		}

	} // end TestUnits
} // end namespace
