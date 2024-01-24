using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Point85.Caliper.UnitOfMeasure;
using System;
using System.Diagnostics;
using System.Resources;

namespace CaliperSharpTests
{
	[TestClass]
	public class TestPartial : BaseTest
	{
		[TestMethod]
		public void TestSnippet()
		{
			string s = MeasurementSystem.GetUnitString("sec.desc");
			Debug.WriteLine(s);
		}
	}
}
