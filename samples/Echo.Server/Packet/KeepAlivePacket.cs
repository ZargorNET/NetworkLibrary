using DotNetty.Buffers;
using ZNetLib.Core.Packet;

namespace Echo.Server.Packet
{
	public class KeepAlivePacket : IInOutPacket
	{
		public void Read(IByteBuffer buffer)
		{
		}

		public void Write(IByteBuffer buffer)
		{
		}
	}
}