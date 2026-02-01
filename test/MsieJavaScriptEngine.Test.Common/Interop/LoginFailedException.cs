using System;
#if !NET8_0_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace MsieJavaScriptEngine.Test.Common.Interop
{
	[Serializable]
	public class LoginFailedException : Exception
	{
		private string _userName;

		public string UserName
		{
			get { return _userName; }
			set { _userName = value; }
		}


		public LoginFailedException()
		{ }

		public LoginFailedException(string message)
			: base(message)
		{ }

		public LoginFailedException(string message, Exception innerException)
			: base(message, innerException)
		{ }
#if !NET8_0_OR_GREATER

		protected LoginFailedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info is not null)
			{
				_userName = info.GetString("UserName");
			}
		}


		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info is null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			base.GetObjectData(info, context);
			info.AddValue("UserName", this._userName);
		}
#endif
	}
}