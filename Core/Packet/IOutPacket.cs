using DotNetty.Buffers;

namespace Core.Packet
{
	public interface IOutPacket : IPacket
	{
		void Write(IByteBuffer buffer);
	}
}