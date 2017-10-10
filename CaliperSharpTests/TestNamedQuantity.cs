using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.point85.uom;

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
				Assert.IsTrue(q.GetName() != null);
				Assert.IsTrue(q.GetSymbol() != null);
				Assert.IsTrue(q.GetDescription() != null);
				Assert.IsTrue(q.GetAmount() != double.MinValue);
				Assert.IsTrue(q.GetUOM() != null);
				Assert.IsTrue(q.ToString() != null);
			}
		}
	}
}
