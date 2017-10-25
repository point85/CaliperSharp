using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Point85.Caliper.UnitOfMeasure;

namespace CaliperSharpTests
{
	[TestClass]
	public class TestFinancial : BaseTest
	{
		[TestMethod]
		public void TestStocks()
		{
			// John has 100 shares of Alphabet Class A stock. How much is his
			// portfolio worth in euros when the last trade was $838.96 and a US
			// dollar is worth 0.94 euros?
			UnitOfMeasure euro = sys.GetUOM(Unit.EURO);
			UnitOfMeasure usd = sys.GetUOM(Unit.US_DOLLAR);
			usd.SetConversion("0.94", euro);

			UnitOfMeasure googl = sys.CreateScalarUOM(UnitType.CURRENCY, "Alphabet A", "GOOGL",
					"Alphabet (formerly Google) Class A shares");
			googl.SetConversion("838.96", usd);
			Quantity portfolio = new Quantity(100, googl);
			Quantity value = portfolio.Convert(euro);
			Assert.IsTrue(IsCloseTo(value.Amount, 78862.24, DELTA6));
		}
	}
}
