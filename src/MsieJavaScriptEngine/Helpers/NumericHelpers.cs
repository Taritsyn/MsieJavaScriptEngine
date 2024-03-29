﻿using System;

using MsieJavaScriptEngine.Extensions;

namespace MsieJavaScriptEngine.Helpers
{
	/// <summary>
	/// Numeric helpers
	/// </summary>
	internal static class NumericHelpers
	{
		private const double MAX_INTEGER_IN_DOUBLE = (1L << 53) - 1;


		/// <summary>
		/// Casts a double value to the correct type
		/// </summary>
		/// <param name="value">Double value</param>
		/// <returns>Numeric value with the correct type</returns>
		public static object CastDoubleValueToCorrectType(double value)
		{
			if (Math.Round(value) == value)
			{
				if (Math.Abs(value) <= MAX_INTEGER_IN_DOUBLE)
				{
					long longValue = Convert.ToInt64(value);
					if (longValue >= int.MinValue && longValue <= int.MaxValue)
					{
						return (int)longValue;
					}

					return longValue;
				}
			}
			else
			{
				float floatValue = Convert.ToSingle(value);
				if (value == floatValue)
				{
					return floatValue;
				}
			}

			return value;
		}

		internal static int UnsignedAsSigned(uint value)
		{
			return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
		}

		internal static uint SignedAsUnsigned(int value)
		{
			return BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
		}
	}
}