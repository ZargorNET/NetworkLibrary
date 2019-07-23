using System;

namespace Core
{
	public class BootstrapConnectionClosed : Exception
	{
		public BootstrapConnectionClosed(string message) : base(message)
		{
		}

		public BootstrapConnectionClosed(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}