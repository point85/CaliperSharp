using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.point85.uom;

namespace CaliperSharpTests
{
	[TestClass]
	public class TestBridges : BaseTest
	{
		[TestMethod]
		public void TestBridgeUnits1()
		{
			sys.ClearCache();

			// SI
			UnitOfMeasure kg = sys.GetUOM(Unit.KILOGRAM);
			UnitOfMeasure m = sys.GetUOM(Unit.METRE);
			UnitOfMeasure km = sys.GetUOM(Prefix.KILO, m);
			UnitOfMeasure litre = sys.GetUOM(Unit.LITRE);
			UnitOfMeasure N = sys.GetUOM(Unit.NEWTON);
			UnitOfMeasure m3 = sys.GetUOM(Unit.CUBIC_METRE);
			UnitOfMeasure m2 = sys.GetUOM(Unit.SQUARE_METRE);
			UnitOfMeasure Nm = sys.GetUOM(Unit.NEWTON_METRE);
			UnitOfMeasure kPa = sys.GetUOM(Prefix.KILO, sys.GetUOM(Unit.PASCAL));
			UnitOfMeasure celsius = sys.GetUOM(Unit.CELSIUS);

			// US
			UnitOfMeasure lbm = sys.GetUOM(Unit.POUND_MASS);
			UnitOfMeasure lbf = sys.GetUOM(Unit.POUND_FORCE);
			UnitOfMeasure mi = sys.GetUOM(Unit.MILE);
			UnitOfMeasure ft = sys.GetUOM(Unit.FOOT);
			UnitOfMeasure gal = sys.GetUOM(Unit.US_GALLON);
			UnitOfMeasure ft2 = sys.GetUOM(Unit.SQUARE_FOOT);
			UnitOfMeasure ft3 = sys.GetUOM(Unit.CUBIC_FOOT);
			UnitOfMeasure acre = sys.GetUOM(Unit.ACRE);
			UnitOfMeasure ftlbf = sys.GetUOM(Unit.FOOT_POUND_FORCE);
			UnitOfMeasure psi = sys.GetUOM(Unit.PSI);
			UnitOfMeasure fahrenheit = sys.GetUOM(Unit.FAHRENHEIT);

			Assert.IsTrue(ft.GetBridgeOffset() == double.MinValue);

			Quantity q1 = new Quantity(10, ft);
			Quantity q2 = q1.Convert(m);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 3.048, DELTA6));
			Quantity q3 = q2.Convert(q1.GetUOM());
			Assert.IsTrue(IsCloseTo(q3.GetAmount(), 10, DELTA6));

			q1 = new Quantity(10, kg);
			q2 = q1.Convert(lbm);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 22.0462, DELTA4));
			q3 = q2.Convert(q1.GetUOM());
			Assert.IsTrue(IsCloseTo(q3.GetAmount(), 10, DELTA6));

			q1 = new Quantity(212, fahrenheit);
			q2 = q1.Convert(celsius);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 100, DELTA6));
			q3 = q2.Convert(q1.GetUOM());
			Assert.IsTrue(IsCloseTo(q3.GetAmount(), 212, DELTA6));

			UnitOfMeasure mm = sys.CreateProductUOM(UnitType.AREA, "name", "mxm", "", m, m);

			q1 = new Quantity(10, mm);
			q2 = q1.Convert(ft2);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 107.639104167, DELTA6));
			q2 = q2.Convert(m2);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 10, DELTA6));

			UnitOfMeasure mhr = sys.GetUOM("m/hr");

			if (mhr == null)
			{
				mhr = sys.CreateScalarUOM(UnitType.VELOCITY, "m/hr", "m/hr", "");
				mhr.SetConversion((double)1 / (double)3600, sys.GetUOM(Unit.METRE_PER_SEC));
			}

			q1 = new Quantity(10, psi);
			q2 = q1.Convert(kPa);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 68.94757280343134, DELTA6));
			q2 = q2.Convert(psi);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 10, DELTA6));

			q1 = new Quantity(10, mhr);
			q2 = q1.Convert(sys.GetUOM(Unit.FEET_PER_SEC));
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 0.009113444152814231, DELTA6));
			q2 = q2.Convert(mhr);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 10, DELTA6));

			q1 = new Quantity(10, gal);
			q2 = q1.Convert(litre);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 37.8541178, DELTA6));
			q2 = q2.Convert(gal);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 10, DELTA6));

			q1 = new Quantity(10, m3);
			q2 = q1.Convert(ft3);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 353.1466672398284, DELTA6));
			q2 = q2.Convert(m3);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 10, DELTA6));

			q1 = new Quantity(10, N);
			q2 = q1.Convert(lbf);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 2.24809, DELTA6));
			q2 = q2.Convert(N);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 10, DELTA6));

			q1 = new Quantity(10, ftlbf);
			q2 = q1.Convert(Nm);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 13.558179483314004, DELTA6));
			q2 = q2.Convert(ftlbf);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 10, DELTA6));

			q1 = new Quantity(10, lbm);
			q2 = q1.Convert(kg);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 4.5359237, DELTA6));
			q2 = q2.Convert(lbm);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 10, DELTA6));

			q1 = new Quantity(10, km);
			q2 = q1.Convert(mi);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 6.21371192237, DELTA6));
			q2 = q2.Convert(km);
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 10, DELTA6));

			// length
			q1 = new Quantity(10, sys.GetUOM(Unit.METRE));
			q2 = q1.Convert(sys.GetUOM(Unit.INCH));
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 393.7007874015748, DELTA6));
			q2 = q2.Convert(sys.GetUOM(Unit.METRE));
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 10, DELTA6));

			q2 = q1.Convert(sys.GetUOM(Unit.FOOT));
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 32.80839895013123, DELTA6));
			q2 = q2.Convert(sys.GetUOM(Unit.METRE));
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 10, DELTA6));

			// area
			q1 = new Quantity(10, sys.GetUOM(Unit.SQUARE_METRE));
			q2 = q1.Convert(sys.GetUOM(Unit.SQUARE_INCH));
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 15500.031000062, DELTA6));
			q2 = q2.Convert(sys.GetUOM(Unit.SQUARE_METRE));
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 10, DELTA6));

			q2 = q1.Convert(sys.GetUOM(Unit.SQUARE_FOOT));
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 107.6391041670972, DELTA6));
			q2 = q2.Convert(sys.GetUOM(Unit.SQUARE_METRE));
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 10, DELTA6));

			// volume
			q1 = new Quantity(10, sys.GetUOM(Unit.LITRE));
			q2 = q1.Convert(sys.GetUOM(Unit.US_GALLON));
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 2.641720523581484, DELTA6));
			q2 = q2.Convert(sys.GetUOM(Unit.LITRE));
			Assert.IsTrue(IsCloseTo(q2.GetAmount(), 10, DELTA6));

			q1 = new Quantity(4.0468564224, m);
			q2 = new Quantity(1000, m);
			q3 = q1.Multiply(q2);
			Assert.IsTrue(IsCloseTo(q3.GetAmount(), 4046.8564224, DELTA6));

			UnitOfMeasure uom = q3.GetUOM();
			UnitOfMeasure powerBase = uom.GetPowerBase();
			double sf = uom.GetScalingFactor();

			Assert.IsTrue(uom.GetAbscissaUnit().Equals(m2));
			Assert.IsTrue(powerBase.Equals(m));
			Assert.IsTrue(IsCloseTo(sf, 1, DELTA6));

			Quantity q4 = q3.Convert(acre);
			Assert.IsTrue(IsCloseTo(q4.GetAmount(), 1, DELTA6));
			Assert.IsTrue(q4.GetUOM().Equals(acre));

			UnitOfMeasure usSec = sys.GetSecond();

			UnitOfMeasure v1 = sys.GetUOM("m/hr");

			UnitOfMeasure v2 = sys.GetUOM(Unit.METRE_PER_SEC);
			UnitOfMeasure v3 = sys.CreateQuotientUOM(UnitType.VELOCITY, "", "ft/usec", "", ft, usSec);

			UnitOfMeasure d1 = sys.GetUOM(Unit.KILOGRAM_PER_CU_METRE);
			UnitOfMeasure d2 = sys.CreateQuotientUOM(UnitType.DENSITY, "density", "lbm/gal", "", lbm, gal);

			q1 = new Quantity(10, v1);
			q2 = q1.Convert(v3);

			q1 = new Quantity(10, v1);
			q2 = q1.Convert(v2);

			q1 = new Quantity(10, d1);
			q2 = q1.Convert(d2);

		}

		[TestMethod]
		public void TestBridgeUnits2()
		{
			UnitOfMeasure bridge1 = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "Bridge1", "B1", "description");
			UnitOfMeasure bridge2 = sys.CreateScalarUOM(UnitType.UNCLASSIFIED, "Bridge2", "B2", "description");

			bridge1.SetBridgeConversion(1, bridge2, 0);
			Assert.IsTrue(bridge1.GetBridgeScalingFactor().Equals(1));
			Assert.IsTrue(bridge1.GetBridgeAbscissaUnit().Equals(bridge2));
			Assert.IsTrue(bridge1.GetBridgeOffset().Equals(0));

			try
			{
				bridge1.SetConversion(10, bridge1, 0);
				Assert.Fail();
			}
			catch (Exception)
			{

			}

			try
			{
				bridge1.SetConversion(1, bridge1, 10);
				Assert.Fail();
			}
			catch (Exception)
			{

			}
		}
	}
}
