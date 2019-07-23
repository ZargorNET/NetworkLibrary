using System;

namespace Core.Packet.Handler
{
	[AttributeUsage(AttributeTargets.Method)]
	public class PacketMethodAttribute : Attribute
	{
		public PacketMethodAttribute(bool runAsync = false)
		{
			RunAsync = runAsync;
		}

		public bool RunAsync { get; set; }
	}
}