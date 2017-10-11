using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Point85.Caliper.UnitOfMeasure;

namespace CaliperSharpTests
{
	[TestClass]
	public class TestCleanup : BaseTest
	{
		[AssemblyCleanup]
		public static void ClearCaches()
		{
			Assert.IsTrue(sys.GetSymbolCache().Count > 0);
			Assert.IsTrue(sys.GetBaseSymbolCache().Count > 0);
			Assert.IsTrue(sys.GetEnumerationCache().Count > 0);

			foreach (Unit unit in Enum.GetValues(typeof(Unit)))
			{
				sys.GetUOM(unit).ClearCache();
			}

			sys.ClearCache();
		}
	}
}
