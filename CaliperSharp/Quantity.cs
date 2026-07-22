/*
MIT License

Copyright (c) 2016 - 2017 Kent Randall, Point85

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

using System;
using System.Globalization;
using System.Text;

namespace Point85.Caliper.UnitOfMeasure
{
	/// <summary>
	/// The Quantity class represents an amount and UnitOfMeasure. A constant
	/// quantity can be named and given a symbol, e.g. the speed of light.
	/// The amount is expressed internally as a double.
	/// </summary>
	///
	public struct Quantity : ISymbolic, IEquatable<Quantity>, IEquatable<double>, IComparable<Quantity>, IComparable<double>
	{
		/// <summary>
		/// the amount
		/// </summary>
		public double Amount { get; private set; }

		/// <summary>
		/// unit of measure
		/// </summary>
		public UnitOfMeasure UOM { get; private set; }

		/// <summary>Create a quantity with an amount and unit of measure</summary>
		/// 
		/// <param name="amount">Amount</param>           
		/// <param name="uom">UnitOfMeasure</param>
		///   
		public Quantity(double amount, UnitOfMeasure uom)
		{
			Amount = amount;
			UOM = uom;
		}

		/// <summary>
		/// Compute a hash code
		/// </summary>
		/// <returns>Hash code</returns>
		public override int GetHashCode()
		{
			return Amount.GetHashCode() ^ UOM.GetHashCode();
		}

		/// <inheritdoc />
		public int CompareTo(Quantity other) => CompareTo(other.ConvertValue(UOM));

		/// <inheritdoc />
		public int CompareTo(double other)
		{
			double delta = Amount - other;
			if (Math.Abs(delta) < UOM.MeasurementSystem.Epsilon)
				return 0;
			if (delta < 0)
				return -1;
			return 1;
		}

		/// <inheritdoc />
		public bool Equals(Quantity other)
		{
			return Equals(other.Amount) && UOM.Equals(other.UOM); 
		}

		/// <inheritdoc />
		public bool Equals(double other)
		{
			return Math.Abs(Amount - other) < UOM.MeasurementSystem.Epsilon;
		}

		/// <inheritdoc />
		public override bool Equals(object other)
		{
			return other switch
			{
				null => false,
				double dv => Equals(dv),
				Quantity qv => Equals(qv),
				_ => false
			};
		}

		/// <summary>Create an amount of a quantity that adheres to precision and rounding
		/// settings from a string</summary>
		/// 
		/// <param name="value">Text value of amount</param>
		///            
		/// <returns>Amount</returns>
		/// 
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


		/// <summary>Create an amount of a quantity from a decimal number</summary>
		/// 
		/// <param name="number">Value</param>
		///            
		/// <returns>Amount</returns>
		/// 
		public static double CreateAmount(decimal number)
		{
			return decimal.ToDouble(number);
		}

		/// <summary>Subtract a quantity from this quantity</summary>
		/// 
		/// <param name="other">Quantity</param>
		///            
		/// <returns>New quantity</returns>
		/// 
		public Quantity Subtract(Quantity other)
		{
			return Subtract(other.Convert(UOM).Amount);
		}
		
		/// <summary>
		/// Subtracts a value from this quantity.
		/// </summary>
		/// <param name="value">Value to subtract.</param>
		/// <returns>Subtracted quantity.</returns>
		public Quantity Subtract(double value)
		{
			return new Quantity(Amount - value, UOM);
		}

		/// <summary>Add two quantities</summary>
		/// 
		/// <param name="other">Quantity</param>
		/// <returns>Sum Quantity</returns>
		/// 
		public Quantity Add(Quantity other)
		{
			return Add(other.Convert(UOM).Amount);
		}

		/// <summary>
		/// Adds a value to this quantity.
		/// </summary>
		/// <param name="value">Value to add.</param>
		/// <returns>Sum quantity.</returns>
		public Quantity Add(double value)
		{
			return new Quantity(Amount + value, UOM);
		}
		
		/// <summary>Divide two quantities to create a third quantity</summary>
		/// 
		/// <param name="other">Quantity</param>
		///           
		/// <returns>Quotient Quantity</returns>
		/// 
		public Quantity Divide(Quantity other)
		{
			if (other.Amount == 0.0)
				throw new Exception(MeasurementSystem.GetMessage("divisor.cannot.be.zero"));

			double amount = Amount / other.Amount;
			UnitOfMeasure newUOM = UOM.Divide(other.UOM);

			Quantity quantity = new Quantity(amount, newUOM);
			return quantity;
		}


		/// <summary>Divide this quantity by the specified amount</summary>
		/// 
		/// <param name="divisor">Amount</param>
		///            
		/// <returns>Quantity</returns>
		/// 
		public Quantity Divide(double divisor)
		{
			if (divisor == 0.0)
				throw new Exception(MeasurementSystem.GetMessage("divisor.cannot.be.zero"));

			double amount = Amount / divisor;
			Quantity quantity = new Quantity(amount, UOM);
			return quantity;
		}

		/// <summary>Multiply this quantity by another quantity to create a third quantity</summary>
		/// 
		/// <param name="other">Quantity</param>
		///            
		/// <returns>Multiplied quantity</returns>
		/// 
		public Quantity Multiply(Quantity other)
		{
			Quantity toMultiply = other;

			double amount = Amount * toMultiply.Amount;
			UnitOfMeasure newUOM = UOM.Multiply(toMultiply.UOM);

			Quantity quantity = new Quantity(amount, newUOM);
			return quantity;
		}

		/// <summary>Raise this quantity to the specified power</summary>
		/// 
		/// <param name="exponent">Exponent</param>
		///            
		/// <returns>new Quantity</returns>
		/// 
		public Quantity Power(int exponent)
		{
			double amount = Math.Pow(Amount, exponent);
			UnitOfMeasure newUOM = UOM.MeasurementSystem.CreatePowerUOM(UOM, exponent);

			Quantity quantity = new Quantity(amount, newUOM);
			return quantity;
		}

		/// <summary>Multiply this quantity by the specified amount</summary>
		/// 
		/// <param name="multiplier">Amount</param>
		///            
		/// <returns>new Quantity</returns>
		/// 
		public Quantity Multiply(double multiplier)
		{
			double amount = Amount * multiplier;
			Quantity quantity = new Quantity(amount, UOM);
			return quantity;
		}

		/// <summary>Invert this quantity, i.e. 1 divided by this quantity to create another quantity</summary>
		///  
		/// <returns>Quantity</returns>
		/// 
		public Quantity Invert()
		{
			if (Amount == 0.0)
				throw new Exception(MeasurementSystem.GetMessage("divisor.cannot.be.zero"));

			double amount = 1 / Amount;
			UnitOfMeasure uom = UOM.Invert();

			Quantity quantity = new Quantity(amount, uom);
			return quantity;
		}

		/// <summary>Convert this quantity to the target UOM</summary>
		/// 
		/// <param name="toUOM">UnitOfMeasure</param>
		///        
		/// <returns>Converted quantity</returns>
		/// 
		public Quantity Convert(UnitOfMeasure toUOM)
		{
			return new Quantity(ConvertValue(toUOM), toUOM);
		}

		/// <summary>
		/// Converts this quantity to the target UOM.
		/// </summary>
		/// <param name="toUOM">UOM to convert this quantity to.</param>
		/// <returns>Converted quantity as double value.</returns>
		public double ConvertValue(UnitOfMeasure toUOM)
		{
			double multiplier = UOM.GetConversionFactor(toUOM);
			double thisOffset = UOM.Offset;
			double targetOffset = toUOM.Offset;

			// adjust for a non-zero "this" offset
			double offsetAmount = Amount + thisOffset;

			// new path amount
			double newAmount = offsetAmount * multiplier;

			// adjust for non-zero target offset
			return newAmount - targetOffset;
		}

		/// <summary>Convert this quantity with a product or quotient unit of measure to the
		/// specified units of measure.</summary>
		/// 
		/// <param name="uom1">Multiplier or dividend UnitOfMeasure</param>
		/// <param name="uom2">Multiplicand or divisor UnitOfMeasure</param>
		///  
		/// <returns>Converted quantity</returns>
		/// 
		public Quantity ConvertToPowerProduct(UnitOfMeasure uom1, UnitOfMeasure uom2)
		{
			UnitOfMeasure newUOM = UOM.ClonePowerProduct(uom1, uom2);

			return Convert(newUOM);
		}

		/// <summary>Convert this quantity of a power unit using the specified base unit of
		/// measure.</summary>
		/// 
		/// <param name="uom">Base UnitOfMeasure</param>
		///          
		/// <returns>Converted quantity</returns>
		/// 
		public Quantity ConvertToPower(UnitOfMeasure uom)
		{
			UnitOfMeasure newUOM = UOM.ClonePower(uom);

			return Convert(newUOM);
		}

		/// <summary>Convert this quantity to the target unit</summary>
		/// 
		/// <param name="unit">Unit</param>
		///     
		/// <returns>Quantity</returns>
		/// 
		public Quantity Convert(Unit unit)
		{
			return Convert(UOM.MeasurementSystem.GetUOM(unit));
		}

		/// <summary>Convert this quantity to the target unit with the specified prefix</summary>
		/// 
		/// <param name="prefix">Prefix</param>
		/// <param name="unit">Unit</param>
		///           
		/// <returns>Quantity</returns>
		/// 
		public Quantity Convert(Prefix prefix, Unit unit)
		{
			return Convert(UOM.MeasurementSystem.GetUOM(prefix, unit));
		}

		/// <summary>
		/// Build a string representation of a Quantity
		/// </summary>
		/// <returns>String value</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(Amount.ToString(CultureInfo.CurrentCulture)).Append(", [").Append(UOM).Append("] ");
			sb.Append(Symbolic.ToString(Name, Symbol, Description));
			return sb.ToString();
		}

		/// <summary>Compare this quantity to the other quantity</summary>
		/// 
		/// <param name="other">Quantity</param>
		///            
		/// <returns>-1 if less than, 0 if equal and 1 if greater than</returns>
		///             
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

		/// <inheritdoc />
		public string Name { get; set; }

		/// <inheritdoc />
		public string Symbol { get; set; }

		/// <inheritdoc />
		public string Description { get; set; }
		
		
		public static bool operator == (Quantity left, Quantity right) => left.Equals(right);
		public static bool operator !=(Quantity left, Quantity right) => !left.Equals(right);
		public static bool operator <(Quantity left, Quantity right) => left.CompareTo(right) < 0;
		public static bool operator >(Quantity left, Quantity right) => left.CompareTo(right) > 0;
		
		public static bool operator == (Quantity left, double right) => left.Equals(right);
		public static bool operator !=(Quantity left, double right) => !left.Equals(right);
		public static bool operator <(Quantity left, double right) => left.CompareTo(right) < 0;
		public static bool operator >(Quantity left, double right) => left.CompareTo(right) > 0;
		
		public static bool operator == (double left, Quantity right) => right.Equals(left);
		public static bool operator !=(double left, Quantity right) => !right.Equals(left);
		public static bool operator <(double left, Quantity right) => right.CompareTo(left) > 0;
		public static bool operator >(double left, Quantity right) => right.CompareTo(left) < 0;

		public static Quantity operator +(Quantity left, Quantity right) => left.Add(right);
		public static Quantity operator +(Quantity left, double right) => left.Add(right);
		public static Quantity operator +(double left, Quantity right) => right.Add(left);
		public static Quantity operator -(Quantity left, Quantity right) => left.Subtract(right);
		public static Quantity operator -(Quantity left, double right) => left.Subtract(right);
		public static Quantity operator -(double left, Quantity right) => new Quantity(left, right.UOM).Subtract(right);

		public static Quantity operator *(Quantity left, Quantity right) => left.Multiply(right);
		public static Quantity operator *(Quantity left, double right) => left.Multiply(right);
		public static Quantity operator *(double left, Quantity right) => right.Multiply(left);
		public static Quantity operator /(Quantity left, Quantity right) => left.Divide(right);
		public static Quantity operator /(Quantity left, double right) => left.Divide(right);
		public static Quantity operator /(double left, Quantity right) => new Quantity(left, right.UOM).Divide(right);
	}
}
