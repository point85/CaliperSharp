using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Point85.Caliper.UnitOfMeasure;
using System.Collections.Generic;
using System.Diagnostics;

namespace CaliperSharpTests
{
	[TestClass]
	public class TestPerformance : BaseTest
	{
		// unit map
		private Dictionary<UnitType, List<UnitOfMeasure>> unitListMap = new Dictionary<UnitType, List<UnitOfMeasure>>();

		[TestMethod]
		public void TestPerformance1()
		{
			try
			{
				MeasurementSystem sys = MeasurementSystem.GetSystem();

				foreach (Unit u in Enum.GetValues(typeof(Unit)))
				{
					UnitOfMeasure uom = sys.GetUOM(u);
					this.AddUnit(uom);
				}

				RunSingleTest();
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
				Assert.Fail();
			}
		}

		private void AddUnit(UnitOfMeasure uom)
		{
			if (!unitListMap.TryGetValue(uom.GetUnitType(), out List<UnitOfMeasure> unitList))
			{
				unitList = new List<UnitOfMeasure>();
				unitListMap[uom.GetUnitType()] = unitList;
			}
			unitList.Add(uom);
		}

		private void RunSingleTest()
		{
			// for each unit type, execute the quantity operations
			foreach (KeyValuePair<UnitType, List<UnitOfMeasure>> entry in unitListMap)
			{
				// run the matrix
				foreach (UnitOfMeasure rowUOM in entry.Value)
				{
					// row quantity
					Quantity rowQty = new Quantity(10, rowUOM);

					foreach (UnitOfMeasure colUOM in entry.Value)
					{
						// column qty
						Quantity colQty = new Quantity(10, colUOM);

						// arithmetic operations
						rowQty.Add(colQty);
						rowQty.Subtract(colQty);

						// offsets are not supported
						if (rowUOM.GetOffset().CompareTo(0) == 0
								&& colUOM.GetOffset().CompareTo(0) == 0)
						{
							rowQty.Multiply(colQty);
							rowQty.Divide(colQty);
							rowQty.Invert();
						}

						rowQty.Convert(colUOM);
						rowQty.Equals(colQty);
						rowQty.GetAmount();
						rowQty.GetUOM();
						rowQty.ToString();
					}
				}
			}
		}
	}
}
