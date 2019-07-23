using DotNetty.Buffers;

namespace ZNetLib.Core.Packet
{
	public interface IInPacket : IPacket
	{
		void Read(IByteBuffer buffer);
	}
}