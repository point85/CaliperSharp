using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.point85.uom;

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

			u = sys.CreateQuotientUOM(UnitType.UNCLASSIFIED, "1/1", "1/1", "", sys.GetOne(), sys.GetOne());
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
	} // end TestUnits
} // end namespace
