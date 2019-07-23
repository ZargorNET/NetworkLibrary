using DotNetty.Buffers;
using ZNetLib.Core.Packet;

namespace Echo.Client.Packet
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