﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
namespace CaliperSharp
{
	/**
 * The Quantity class represents an amount and {@link UnitOfMeasure}. A constant
 * quantity can be named and given a symbol, e.g. the speed of light.
 * <p>
 * The amount is expressed as a decimal in order to control the precision of
 * floating point arithmetic. A MathContext is used with a precision setting
 * matching the IEEE 754R Decimal64 format, 16 digits, and a rounding mode of
 * HALF_EVEN, the IEEE 754R default.
 * </p>
 * 
 * @author Kent Randall
 *
 */
	class Quantity : Symbolic
	{
		// the amount
		private decimal Amount;

		// and its unit of measure
		private UnitOfMeasure UOM;

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
		public Quantity(decimal amount, UnitOfMeasure uom)
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
		public Quantity(decimal amount, Prefix prefix, Unit unit) : this(amount, MeasurementSystem.GetSystem().GetUOM(prefix, unit))
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
		public Quantity(string amount, UnitOfMeasure uom)
		{
			Amount = createAmount(amount);
			UOM = uom;
		}

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
		public Quantity(decimal amount, Unit unit) : this(amount, MeasurementSystem.GetSystem().GetUOM(unit))
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
		public Quantity(string amount, Unit unit) : this(createAmount(amount), MeasurementSystem.GetSystem().GetUOM(unit))
		{
		}

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
		public static decimal CreateAmount(string value)
		{
			// use string constructor for exact precision with rounding mode in math
			// context
			if (value == null)
			{
				throw new Exception(MeasurementSystem.GetMessage("amount.cannot.be.null"));
			}
			return Convert.ToDecimal(value);
		}

		/**
		 * Create an amount of a quantity that adheres to precision and rounding
		 * settings from a Number
		 * 
		 * @param number
		 *            Value
		 * @return Amount
		 */
		public static decimal CreateAmount(ValueType number)
		{
			decimal result = 0;

			if (number is decimal) {
				result = (decimal)number;
			} else if (number is int) {
				result = new decimal((int)number);
			} else if (number is double) {
				result = new decimal((double)number);
			} else if (number is float) {
				result = new decimal((float)number);
			} else if (number is long) {
				result = new decimal((long)number);
			} else if (number is short) {
				result = new decimal((short)number);
			}

			return result;
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
		static public decimal DivideAmounts(string dividendAmount, string divisorAmount)
		{
			decimal dividend = Quantity.CreateAmount(dividendAmount);
			decimal divisor = Quantity.CreateAmount(divisorAmount);
			return UnitOfMeasure.DecimalDivide(dividend, divisor);
		}

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
		static public decimal MultiplyAmounts(string multiplierAmount, string multiplicandAmount)
		{
			decimal multiplier = Quantity.CreateAmount(multiplierAmount);
			decimal multiplicand = Quantity.CreateAmount(multiplicandAmount);
			return UnitOfMeasure.DecimalMultiply(multiplier, multiplicand);
		}

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
			Quantity toSubtract = other.convert(UOM);
			decimal amount = UnitOfMeasure.decimalSubtract(getAmount(), toSubtract.getAmount());
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
		public Quantity add(Quantity other)
		{
			Quantity toAdd = other.convert(UOM);
			decimal amount = UnitOfMeasure.decimalAdd(getAmount(), toAdd.getAmount());
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
		public Quantity divide(Quantity other)
		{
			Quantity toDivide = other;

			decimal amount = UnitOfMeasure.decimalDivide(getAmount(), toDivide.getAmount());
			UnitOfMeasure newUOM = UOM.divide(toDivide.UOM);

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
		public Quantity divide(decimal divisor)
		{
			decimal amount = UnitOfMeasure.decimalDivide(getAmount(), divisor);
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
		public Quantity multiply(Quantity other)
		{
			Quantity toMultiply = other;

			decimal amount = UnitOfMeasure.decimalMultiply(getAmount(), toMultiply.getAmount());
			UnitOfMeasure newUOM = UOM.multiply(toMultiply.UOM);

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
		public Quantity power(int exponent)
		{
			decimal amount = UnitOfMeasure.decimalPower(getAmount(), exponent);
			UnitOfMeasure newUOM = MeasurementSystem.GetSystem().createPowerUOM(UOM, exponent);

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
		public Quantity multiply(decimal multiplier)
		{
			decimal amount = UnitOfMeasure.decimalMultiply(getAmount(), multiplier);
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
		public Quantity invert()
		{
			decimal amount = UnitOfMeasure.decimalDivide(decimal.ONE, getAmount());
			UnitOfMeasure uom = UOM.invert();

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
			decimal multiplier = UOM.GetConversionFactor(toUOM);
			decimal thisOffset = UOM.Offset;
			decimal targetOffset = toUOM.Offset;

			// adjust for a non-zero "this" offset
			decimal offsetAmount = UnitOfMeasure.DecimalAdd(Amount, thisOffset);

			// new path amount
			decimal newAmount = UnitOfMeasure.DecimalMultiply(offsetAmount, multiplier);

			// adjust for non-zero target offset
			newAmount = UnitOfMeasure.DecimalSubtract(newAmount, targetOffset);

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
		public Quantity convertToPower(UnitOfMeasure uom)
		{
			UnitOfMeasure newUOM = UOM.ClonePower(uom);

			return convert(newUOM);
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
		public Quantity convert(Unit unit)
		{
			return convert(MeasurementSystem.GetSystem().GetUOM(unit));
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
		public Quantity convert(Prefix prefix, Unit unit)
		{
			return convert(MeasurementSystem.GetSystem().GetUOM(prefix, unit));
		}

		/**
		 * Create a string representation of this Quantity
		 */
		@Override
	public string tostring()
		{
			stringBuffer sb = new stringBuffer();
			sb.append(this.getAmount()).append(", [").append(UOM.tostring()).append("] ");
			sb.append(super.tostring());
			return sb.tostring();
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
		public int compare(Quantity other)
		{
			Quantity toCompare = other;

			if (!UOM.equals(other.UOM))
			{
				// first try converting the units
				toCompare = other.convert(this.UOM);
			}

			return getAmount().compareTo(toCompare.getAmount());
		}

	}
}