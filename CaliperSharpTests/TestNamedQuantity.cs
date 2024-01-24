using Microsoft.VisualStudio.TestTools.UnitTesting;
using Point85.Caliper.UnitOfMeasure;
using System;

namespace CaliperSharpTests
{
	[TestClass]
	public class TestNamedQuantity : BaseTest
	{
		[TestMethod]
		public void TestConstant()
		{
			foreach (Constant value in Enum.GetValues(typeof(Constant)))
			{
				Quantity q = sys.GetQuantity(value);
				Assert.IsTrue(q.Name != null);
				Assert.IsTrue(q.Symbol != null);
				Assert.IsTrue(q.Description != null);
				Assert.IsTrue(q.Amount != double.MinValue);
				Assert.IsTrue(q.UOM != null);
				Assert.IsTrue(q.ToString() != null);
			}
		}
	}
}
