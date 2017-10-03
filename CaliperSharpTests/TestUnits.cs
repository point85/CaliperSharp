using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.point85.uom;
using System.Text;

namespace CaliperSharpTests
{
	[TestClass]
	public class TestUnits : BaseTest
	{
		//[TestMethod]
		public void TestGetString()
		{
			string str = MeasurementSystem.GetUnitString("one.name");
			Assert.IsTrue(str.Length > 0);
		}

		//[TestMethod]
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

		//[TestMethod]
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

			u = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "1/1", "1/1", "", sys.GetOne(), sys.GetOne());
			Quantity q1 = new Quantity(10, u);
			Quantity q2 = q1.Convert(sys.GetOne());
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 10, DELTA6));
			Assert.IsTrue(q2.GetUOM().Equals(sys.GetOne()));

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

		//[TestMethod]
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

			u = sys.GetUOM(a.GetSymbol());
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
			u = sys.GetUOM(axb.GetSymbol());
			Assert.IsTrue(u.Equals(axb));
			Assert.IsTrue(axb.Divide(a).Equals(b));

			String symbol = axb.GetSymbol() + "." + axb.GetSymbol();
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

			symbol = axb.GetSymbol() + "^-2";
			UnitOfMeasure axbm2 = sys.CreatePowerUOM(UnitType.UNCLASSIFIED, "", symbol, "", axb, -2);
			uom = axbm2.Multiply(axb2);
			Assert.IsTrue(uom.GetBaseSymbol().Equals(sys.GetOne().GetSymbol()));

			UnitOfMeasure cxd = sys.CreateProductUOM(UnitType.UNCLASSIFIED, "", "c.D", "", c, x);
			const char MULT = (char)0xB7;
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


	} // end TestUnits
} // end namespace
