using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaliperSharp
{

	// reduce a unit of measure to its most basic scalar units of measure.
	internal class Reducer
	{
		private static readonly int MAX_RECURSIONS = 100;

		// starting level
		private static readonly int STARTING_LEVEL = -1;

		// UOMs and their exponents
		private Dictionary<UnitOfMeasure, int> Terms { get; set; } = new Dictionary<UnitOfMeasure, int>();

		// the overall scaling factor
		private decimal MapScalingFactor { get; set; } = decimal.One;

		// list of exponents down a path to the leaf UOM
		private List<int> PathExponents = new List<int>();

		// recursion counter
		private int Counter = 0;

		internal Reducer()
		{

		}

		internal void Explode(UnitOfMeasure unit)
		{
			ExplodeRecursively(unit, STARTING_LEVEL);
		}

		private void ExplodeRecursively(UnitOfMeasure unit, int level)
		{
			if (++Counter > MAX_RECURSIONS)
			{
				string msg = String.Format(MeasurementSystem.GetMessage("circular.references"),
						unit.Symbol);
				throw new Exception(msg);
			}

			// down a level
			level++;

			// scaling factor to abscissa unit
			decimal scalingFactor = unit.ScalingFactor;

			// explode the abscissa unit
			UnitOfMeasure abscissaUnit = unit.AbscissaUnit;

			UnitOfMeasure uom1 = abscissaUnit.UOM1;
			UnitOfMeasure uom2 = abscissaUnit.UOM2;

			int? exp1 = abscissaUnit.Exponent1;
			int? exp2 = abscissaUnit.Exponent2;

			// scaling
			if (PathExponents.Count > 0)
			{
				int lastExponent = PathExponents[PathExponents.Count - 1];

				// compute the overall scaling factor
				decimal factor = decimal.One;
				for (int i = 0; i < Math.Abs(lastExponent); i++)
				{
					factor = UnitOfMeasure.DecimalMultiply(factor, scalingFactor);
				}

				if (lastExponent < 0)
				{
					MapScalingFactor = UnitOfMeasure.DecimalDivide(MapScalingFactor, factor);
				}
				else
				{
					MapScalingFactor = UnitOfMeasure.DecimalMultiply(MapScalingFactor, factor);
				}
			}
			else
			{
				MapScalingFactor = scalingFactor;
			}

			if (uom1 == null)
			{
				if (!abscissaUnit.IsTerminal())
				{
					// keep exploding down the conversion path
					decimal currentMapFactor = MapScalingFactor;
					MapScalingFactor = decimal.One;
					ExplodeRecursively(abscissaUnit, STARTING_LEVEL);
					MapScalingFactor = UnitOfMeasure.DecimalMultiply(MapScalingFactor, currentMapFactor);
				}
				else
				{

					// multiply out all of the exponents down the path
					int pathExponent = 1;

					foreach (int exp in PathExponents)
					{
						pathExponent = pathExponent * exp;
					}

					bool invert = pathExponent < 0 ? true : false;

					for (int i = 0; i < Math.Abs(pathExponent); i++)
					{
						AddTerm(abscissaUnit, invert);
					}
				}
			}
			else
			{
				// explode UOM #1
				PathExponents.Add(exp1.Value);
				ExplodeRecursively(uom1, level);
				PathExponents.Remove(level);
			}

			if (uom2 != null)
			{
				// explode UOM #2
				PathExponents.Add(exp2.Value);
				ExplodeRecursively(uom2, level);
				PathExponents.Remove(level);
			}

			// up a level
			level--;
		}

		// add a UOM and exponent pair to the map of reduced terms
		private void AddTerm(UnitOfMeasure uom, bool invert)
		{
			int unitPower = 1;
			int power = 0;

			if (!invert)
			{
				// get existing power
				if (!Terms.ContainsKey(uom))
				{
					// add first time
					power = unitPower;
				}
				else
				{
					// increment existing power
					if (!uom.Equals(MeasurementSystem.GetSystem().GetOne()))
					{
						power = Terms[uom] + unitPower;
					}
				}
			}
			else
			{
				// denominator with negative powers
				if (!Terms.ContainsKey(uom))
				{
					// add first time
					power = -unitPower;
				}
				else
				{
					// decrement existing power
					if (!uom.Equals(MeasurementSystem.GetSystem().GetOne()))
					{
						power = Terms[uom] - unitPower;
					}
				}
			}

			if (power == 0)
			{
				Terms.Remove(uom);
			}
			else
			{
				if (!uom.Equals(MeasurementSystem.GetSystem().getOne()))
				{
					Terms[uom] = power;
				}
			}
		}

		// compose the base symbol
		private string BuildBasestring()
		{
			StringBuilder numerator = new StringBuilder();
			StringBuilder denominator = new StringBuilder();

			int numeratorCount = 0;
			int denominatorCount = 0;

			// sort units by symbol (ascending)
			SortedSet<UnitOfMeasure> keys = new SortedSet<UnitOfMeasure>(Terms.Keys);

			foreach (UnitOfMeasure unit in keys)
			{
				int power = Terms[unit];

				if (power < 0)
				{
					// negative, put in denominator
					if (denominator.Length > 0)
					{
						denominator.Append(UnitOfMeasure.MULT);
					}

					if (!unit.Equals(MeasurementSystem.GetSystem().GetOne()))
					{
						denominator.Append(unit.Symbol);
						denominatorCount++;
					}

					if (power < -1)
					{
						if (power == -2)
						{
							denominator.Append(UnitOfMeasure.SQ);
						}
						else if (power == -3)
						{
							denominator.Append(UnitOfMeasure.CUBED);
						}
						else
						{
							denominator.Append(UnitOfMeasure.POW).Append(Math.Abs(power));
						}
					}
				}
				else if (power >= 1 && !unit.Equals(MeasurementSystem.GetSystem().GetOne()))
				{
					// positive, put in numerator
					if (numerator.Length > 0)
					{
						numerator.Append(UnitOfMeasure.MULT);
					}

					numerator.Append(unit.Symbol);
					numeratorCount++;

					if (power > 1)
					{
						if (power == 2)
						{
							numerator.Append(UnitOfMeasure.SQ);
						}
						else if (power == 3)
						{
							numerator.Append(UnitOfMeasure.CUBED);
						}
						else
						{
							numerator.Append(UnitOfMeasure.POW).Append(power);
						}
					}
				}
				else
				{
					// unary, don't add a '1'
				}
			}

			if (numeratorCount == 0)
			{
				numerator.Append(UnitOfMeasure.ONE_CHAR);
			}

			string result = null;

			if (denominatorCount == 0)
			{
				result = numerator.ToString();
			}
			else
			{
				if (denominatorCount == 1)
				{
					result = numerator.Append(UnitOfMeasure.DIV).Append(denominator).ToString();
				}
				else
				{
					result = numerator.Append(UnitOfMeasure.DIV).Append(UnitOfMeasure.LP).Append(denominator).Append(UnitOfMeasure.RP).ToString();
				}
			}

			return result;
		} // end unit of measure iteration
	} // end class
} // end namespace