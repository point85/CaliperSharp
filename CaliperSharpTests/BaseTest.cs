﻿using Point85.Caliper.UnitOfMeasure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace CaliperSharpTests
{
	public class BaseTest
	{
		protected static double DELTA6 = 0.000001;
		protected static double DELTA5 = 0.00001;
		protected static double DELTA4 = 0.0001;
		protected static double DELTA3 = 0.001;
		protected static double DELTA2 = 0.01;
		protected static double DELTA1 = 0.1;
		protected static double DELTA0 = 1;
		protected static double DELTA_10 = 10;

		protected static MeasurementSystem sys = MeasurementSystem.GetSystem();

		protected BaseTest()
		{
		}

		protected void SnapshotSymbolCache()
		{
			ConcurrentDictionary<string, UnitOfMeasure> cache = sys.GetSymbolCache();

			Debug.WriteLine("Symbol cache ...");
			int count = 0;
			foreach (KeyValuePair<string, UnitOfMeasure> entry in cache)
			{
				count++;
				Debug.WriteLine("(" + count + ") " + entry.Key + ", " + entry.Value);
			}
		}

		protected void SnapshotBaseSymbolCache()
		{
			ConcurrentDictionary<string, UnitOfMeasure> cache = sys.GetBaseSymbolCache();

			Debug.WriteLine("Base symbol cache ...");
			int count = 0;
			foreach (KeyValuePair<string, UnitOfMeasure> entry in cache)
			{
				count++;
				Debug.WriteLine("(" + count + ") " + entry.Key + ", " + entry.Value);
			}
		}

		protected void SnapshotUnitEnumerationCache()
		{
			ConcurrentDictionary<Unit, UnitOfMeasure> cache = sys.GetEnumerationCache();

			Debug.WriteLine("Enumeration cache ...");
			int count = 0;
			foreach (KeyValuePair<Unit, UnitOfMeasure> entry in cache)
			{
				count++;
				Debug.WriteLine("(" + count + ") " + entry.Key + ", " + entry.Value);
			}
		}

		protected bool IsCloseTo(double actualValue, double expectedValue, double delta)
		{
			double diff = Math.Abs(actualValue - expectedValue);
			return (diff <= delta) ? true : false;
		}
	}
}
