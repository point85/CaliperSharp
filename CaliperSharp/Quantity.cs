using System;
using System.Text;

/*
MIT License

Copyright (c) 2016 Kent Randall

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
namespace org.point85.uom
{
	/**
 * The Quantity class represents an amount and {@link UnitOfMeasure}. A constant
 * quantity can be named and given a symbol, e.g. the speed of light.
 * <p>
 * The amount is expressed as a double in order to control the precision of
 * floating point arithmetic. A MathContext is used with a precision setting
 * matching the IEEE 754R double64 format, 16 digits, and a rounding mode of
 * HALF_EVEN, the IEEE 754R default.
 * </p>
 * 
 * @author Kent Randall
 *
 */
	public class Quantity : Symbolic
	{
		// the amount
		public double Amount { get; set; }

		// and its unit of measure
		public UnitOfMeasure UOM { get; set; }

		/**
		 * Default constructor
		 */
		public Quantity() : base()
		{
		}

		/**
		 * Create a quantity with an amount and unit of measure
		 * 
		 * @param amount
		 *            Amount
		 * @param uom
		 *            {@link UnitOfMeasure}
		 */
		public Quantity(double amount, UnitOfMeasure uom)
		{
			Amount = amount;
			UOM = uom;
		}

		/**
		 * Create a quantity with an amount, prefix and unit
		 * 
		 * @param amount
		 *            Amount
		 * @param prefix
		 *            {@link Prefix}
		 * @param unit
		 *            {@link Unit}
		 * @ Exception
		 */
		public Quantity(double amount, Prefix prefix, Unit unit) : this(amount, MeasurementSystem.GetSystem().GetUOM(prefix, unit))
		{
		}

		/**
		 * Create a quantity with a string amount and unit of measure
		 * 
		 * @param amount
		 *            Amount
		 * @param uom
		 *            {@link UnitOfMeasure}
		 * @
		 *             Exception
		 */
		 /*
		public Quantity(string amount, UnitOfMeasure uom)
		{
			Amount = CreateAmount(amount);
			UOM = uom;
		}
		*/

		/**
		 * Create a quantity with an amount and unit
		 * 
		 * @param amount
		 *            Amount
		 * @param unit
		 *            {@link Unit}
		 * @
		 *             Exception
		 */
		public Quantity(double amount, Unit unit) : this(amount, MeasurementSystem.GetSystem().GetUOM(unit))
		{
		}

		/**
		 * Create a quantity with a string amount and unit
		 * 
		 * @param amount
		 *            Amount
		 * @param unit
		 *            {@link Unit}
		 * @
		 *             Exception
		 */
		 /*
		public Quantity(string amount, Unit unit) : this(CreateAmount(amount), MeasurementSystem.GetSystem().GetUOM(unit))
		{
		}
		*/

		/**
		 * Create a hash code
		 * 
		 * @return hash code
		 */
		public override int GetHashCode()
		{
			return Amount.GetHashCode() ^ UOM.GetHashCode();
		}

		/**
		 * Compare this Quantity to another one
		 * 
		 * @param other
		 *            Quantity
		 * @return true if equal
		 */
		public override bool Equals(Object other)
		{
			bool answer = false;

			if (other == null || GetType() != other.GetType())
			{
				return answer;
			}

			Quantity otherQuantity = (Quantity)other;

			// same amount and same unit of measure
			if (Amount.CompareTo(otherQuantity.Amount) == 0 && UOM.Equals(otherQuantity.UOM))
			{
				answer = true;
			}
			return answer;
		}

		/**
		 * Create an amount of a quantity that adheres to precision and rounding
		 * settings from a string
		 * 
		 * @param value
		 *            Text value of amount
		 * @return Amount
		 * @
		 *             Exception
		 */
		public static double CreateAmount(string value)
		{
			// use string constructor for exact precision with rounding mode in math
			// context
			if (value == null)
			{
				throw new Exception(MeasurementSystem.GetMessage("amount.cannot.be.null"));
			}
			return System.Convert.ToDouble(value);
		}

		/**
		 * Create an amount of a quantity that adheres to precision and rounding
		 * settings from a Number
		 * 
		 * @param number
		 *            Value
		 * @return Amount
		 */
		public static double CreateAmount(decimal number)
		{
			return decimal.ToDouble(number);
		}

		/**
		 * Create an amount by dividing two amounts represented by strings
		 * 
		 * @param dividendAmount
		 *            Dividend
		 * @param divisorAmount
		 *            Divisor
		 * @return Ratio of two amounts
		 * @
		 *             Exception
		 */
		 /*
		static public double DivideAmounts(string dividendAmount, string divisorAmount)
		{
			double dividend = Quantity.CreateAmount(dividendAmount);
			double divisor = Quantity.CreateAmount(divisorAmount);
			return dividend / divisor;
		}
		*/

		/**
		 * Create an amount by multiplying two amounts represented by strings
		 * 
		 * @param multiplierAmount
		 *            Multiplier
		 * @param multiplicandAmount
		 *            Multiplicand
		 * @return Product of two amounts
		 * @
		 *             Exception
		 */
		 /*
		static public double MultiplyAmounts(string multiplierAmount, string multiplicandAmount)
		{
			double multiplier = Quantity.CreateAmount(multiplierAmount);
			double multiplicand = Quantity.CreateAmount(multiplicandAmount);
			return multiplier * multiplicand;
		}
		*/

		/**
		 * Subtract a quantity from this quantity
		 * 
		 * @param other
		 *            quantity
		 * @return New quantity
		 * @
		 *             Exception
		 */
		public Quantity Subtract(Quantity other)
		{
			Quantity toSubtract = other.Convert(UOM);
			double amount = Amount - toSubtract.Amount;
			Quantity quantity = new Quantity(amount, this.UOM);
			return quantity;
		}

		/**
		 * Add two quantities
		 * 
		 * @param other
		 *            {@link Quantity}
		 * @return Sum {@link Quantity}
		 * @
		 *             Exception
		 */
		public Quantity Add(Quantity other)
		{
			Quantity toAdd = other.Convert(UOM);
			double amount = Amount + toAdd.Amount;
			Quantity quantity = new Quantity(amount, this.UOM);
			return quantity;
		}

		/**
		 * Divide two quantities to create a third quantity
		 * 
		 * @param other
		 *            {@link Quantity}
		 * @return Quotient {@link Quantity}
		 * @
		 *             Exception
		 */
		public Quantity Divide(Quantity other)
		{
			Quantity toDivide = other;

			double amount = Amount / toDivide.Amount;
			UnitOfMeasure newUOM = UOM.Divide(toDivide.UOM);

			Quantity quantity = new Quantity(amount, newUOM);
			return quantity;
		}

		/**
		 * Divide this quantity by the specified amount
		 * 
		 * @param divisor
		 *            Amount
		 * @return Quantity {@link Quantity}
		 * @
		 *             Exception
		 */
		public Quantity Divide(double divisor)
		{
			double amount = Amount / divisor;
			Quantity quantity = new Quantity(amount, UOM);
			return quantity;
		}

		/**
		 * Multiply this quantity by another quantity to create a third quantity
		 * 
		 * @param other
		 *            Quantity
		 * @return Multiplied quantity
		 * @
		 *             Exception
		 */
		public Quantity Multiply(Quantity other)
		{
			Quantity toMultiply = other;

			double amount = Amount * toMultiply.Amount;
			UnitOfMeasure newUOM = UOM.Multiply(toMultiply.UOM);

			Quantity quantity = new Quantity(amount, newUOM);
			return quantity;
		}

		/**
		 * Raise this quantity to the specified power
		 * 
		 * @param exponent
		 *            Exponent
		 * @return new Quantity
		 * @
		 *             Exception
		 */
		public Quantity Power(int exponent)
		{
			double amount = Math.Pow(Amount, exponent);
			UnitOfMeasure newUOM = MeasurementSystem.GetSystem().CreatePowerUOM(UOM, exponent);

			Quantity quantity = new Quantity(amount, newUOM);
			return quantity;
		}

		/**
		 * Multiply this quantity by the specified amount
		 * 
		 * @param multiplier
		 *            Amount
		 * @return new Quantity
		 * @
		 *             Exception
		 */
		public Quantity Multiply(double multiplier)
		{
			double amount = Amount * multiplier;
			Quantity quantity = new Quantity(amount, UOM);
			return quantity;
		}

		/**
		 * Invert this quantity, i.e. 1 divided by this quantity to create another
		 * quantity
		 * 
		 * @return {@link Quantity}
		 * @
		 *             Exception
		 */
		public Quantity Invert()
		{
			double amount = 1 / Amount;
			UnitOfMeasure uom = UOM.Invert();

			Quantity quantity = new Quantity(amount, uom);
			return quantity;
		}

		/**
		 * Convert this quantity to the target UOM
		 * 
		 * @param toUOM
		 *            {@link UnitOfMeasure}
		 * @return Converted quantity
		 * @
		 *             Exception
		 */
		public Quantity Convert(UnitOfMeasure toUOM)
		{
			double multiplier = UOM.GetConversionFactor(toUOM);
			double thisOffset = UOM.GetOffset();
			double targetOffset = toUOM.GetOffset();

			// adjust for a non-zero "this" offset
			double offsetAmount = Amount + thisOffset;

			// new path amount
			double newAmount = offsetAmount * multiplier;

			// adjust for non-zero target offset
			newAmount = newAmount - targetOffset;

			// create the quantity now
			return new Quantity(newAmount, toUOM);
		}

		/**
		 * Convert this quantity with a product or quotient unit of measure to the
		 * specified units of measure.
		 * 
		 * @param uom1
		 *            Multiplier or dividend {@link UnitOfMeasure}
		 * @param uom2
		 *            Multiplicand or divisor {@link UnitOfMeasure}
		 * @return Converted quantity
		 * @
		 *             Exception
		 */
		public Quantity ConvertToPowerProduct(UnitOfMeasure uom1, UnitOfMeasure uom2)
		{
			UnitOfMeasure newUOM = UOM.ClonePowerProduct(uom1, uom2);

			return Convert(newUOM);
		}

		/**
		 * Convert this quantity of a power unit using the specified base unit of
		 * measure.
		 * 
		 * @param uom
		 *            Base {@link UnitOfMeasure}
		 * @return Converted quantity
		 * @
		 *             exception
		 */
		public Quantity ConvertToPower(UnitOfMeasure uom)
		{
			UnitOfMeasure newUOM = UOM.ClonePower(uom);

			return Convert(newUOM);
		}

		/**
		 * Convert this quantity to the target unit
		 * 
		 * @param unit
		 *            {@link Unit}
		 * @return {@link Quantity}
		 * @
		 *             Exception
		 */
		public Quantity Convert(Unit unit)
		{
			return Convert(MeasurementSystem.GetSystem().GetUOM(unit));
		}

		/**
		 * Convert this quantity to the target unit with the specified prefix
		 * 
		 * @param prefix
		 *            {@link Prefix}
		 * @param unit
		 *            {@link Unit}
		 * @return {@link Quantity}
		 * @
		 *             Exception
		 */
		public Quantity Convert(Prefix prefix, Unit unit)
		{
			return Convert(MeasurementSystem.GetSystem().GetUOM(prefix, unit));
		}

		/**
		 * Create a string representation of this Quantity
		 */
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(Amount.ToString()).Append(", [").Append(UOM.ToString()).Append("] ");
			sb.Append(base.ToString());
			return sb.ToString();
		}

		/**
		 * Compare this quantity to the other quantity
		 * 
		 * @param other
		 *            Quantity
		 * @return -1 if less than, 0 if equal and 1 if greater than
		 * @
		 *             If the quantities cannot be compared.
		 */
		public int Compare(Quantity other)
		{
			Quantity toCompare = other;

			if (!UOM.Equals(other.UOM))
			{
				// first try converting the units
				toCompare = other.Convert(this.UOM);
			}

			return Amount.CompareTo(toCompare.Amount);
		}

	}
}
