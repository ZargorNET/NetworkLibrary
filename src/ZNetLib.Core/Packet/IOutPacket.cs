using DotNetty.Buffers;

namespace ZNetLib.Core.Packet
{
	public interface IOutPacket : IPacket
	{
		void Write(IByteBuffer buffer);
	}
}