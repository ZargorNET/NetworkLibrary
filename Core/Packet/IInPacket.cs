using DotNetty.Buffers;

namespace Core.Packet
{
	public interface IInPacket : IPacket
	{
		void Read(IByteBuffer buffer);
	}
}