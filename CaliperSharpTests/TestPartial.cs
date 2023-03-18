using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Point85.Caliper.UnitOfMeasure;

namespace CaliperSharpTests
{
	[TestClass]
	public class TestPartial : BaseTest
	{
		[TestMethod]
		public void TestSnippet()
		{
			UnitOfMeasure gUOM = sys.GetUOM(Unit.NEWTON_METRE).Multiply(sys.GetUOM(Unit.METRE)).Divide(sys.CreatePowerUOM(sys.GetUOM(Unit.KILOGRAM), 2));
			Quantity G = new Quantity(6.743015E-11, gUOM);
			Quantity mEarth = new Quantity(5.96E24, sys.GetUOM(Unit.KILOGRAM));
			Quantity mMoon = new Quantity(7.33E22, sys.GetUOM(Unit.KILOGRAM));
			Quantity distance = new Quantity(3.84E08, sys.GetUOM(Unit.METRE));
			Quantity force = G.Multiply(mEarth).Multiply(mMoon).Divide(distance.Multiply(distance));

			Console.WriteLine("Done");


		}
	}
}
