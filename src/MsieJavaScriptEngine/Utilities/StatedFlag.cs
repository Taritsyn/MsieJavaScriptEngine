﻿namespace MsieJavaScriptEngine.Utilities
{
	internal struct StatedFlag
	{
		private bool _isSet;


		public bool IsSet()
		{
			return _isSet;
		}

		public bool Set()
		{
			if (!_isSet)
			{
				_isSet = true;
				return true;
			}

			return false;
		}
	}
}