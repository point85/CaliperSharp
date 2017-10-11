using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Point85.Caliper.UnitOfMeasure;
using System.Collections.Generic;

namespace CaliperSharpTests
{
	[TestClass]
	public class TestSystems : BaseTest
	{

		[TestMethod]
		public void TestUnifiedSystem()
		{
			Assert.IsFalse(sys.Equals(null));

			Dictionary<String, UnitOfMeasure> unitMap = new Dictionary<String, UnitOfMeasure>();

			// check the SI units
			foreach (Unit unit in Enum.GetValues(typeof(Unit)))
			{
				UnitOfMeasure uom = sys.GetUOM(unit);

				Assert.IsNotNull(uom);
				Assert.IsNotNull(uom.GetName());
				Assert.IsNotNull(uom.GetSymbol());
				Assert.IsNotNull(uom.GetDescription());
				Assert.IsNotNull(uom.ToString());
				Assert.IsNotNull(uom.GetBaseSymbol());
				Assert.IsNotNull(uom.GetAbscissaUnit());
				Assert.IsNotNull(uom.GetScalingFactor());
				Assert.IsNotNull(uom.GetOffset());

				// symbol uniqueness
				Assert.IsFalse(unitMap.ContainsKey(uom.GetSymbol()));
				unitMap[uom.GetSymbol()] = uom;
			}

			List<Unit> allUnits = new List<Unit>();

			foreach (Unit u in Enum.GetValues(typeof(Unit)))
			{
				allUnits.Add(u);
			}

			foreach (UnitOfMeasure uom in sys.GetRegisteredUnits())
			{
				if (uom.GetEnumeration() != null)
				{
					Assert.IsTrue(allUnits.Contains(uom.GetEnumeration().Value));
				}
			}

			foreach (UnitType unitType in Enum.GetValues(typeof(UnitType)))
			{
				UnitType found;
				bool hasType = false;
				foreach (UnitOfMeasure u in sys.GetRegisteredUnits())
				{
					if (u.GetUnitType().Equals(unitType))
					{
						found = u.GetUnitType();
						hasType = true;
						break;
					}
				}

				if (!hasType && !unitType.Equals(UnitType.UNCLASSIFIED))
				{
					Assert.Fail("No unit found for type " + unitType);
				}
			}

			// constants
			foreach (Constant c in Enum.GetValues(typeof(Constant)))
			{
				Assert.IsTrue(sys.GetQuantity(c) != null);
			}
		}

		[TestMethod]
		public void TestCache()
		{

			// unit cache
			sys.GetOne();

			int before = sys.GetRegisteredUnits().Count;

			for (int i = 0; i < 10; i++)
			{
				sys.CreateScalarUOM(UnitType.UNCLASSIFIED, null, Guid.NewGuid().ToString(), null);
			}

			int after = sys.GetRegisteredUnits().Count;

			Assert.IsTrue(after == (before + 10));

		}

		[TestMethod]
		public void TestGetUnits()
		{
			foreach (UnitType type in Enum.GetValues(typeof(UnitType)))
			{
				List<UnitOfMeasure> uoms = sys.GetUnitsOfMeasure(type);
				if (!type.Equals(UnitType.UNCLASSIFIED))
				{
					Assert.IsTrue(uoms.Count > 0);
				}
			}
		}

	}
}
