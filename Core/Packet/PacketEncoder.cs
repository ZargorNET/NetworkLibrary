using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Core.Packet
{
	public class PacketEncoder : MessageToByteEncoder<IPacket>
	{
		private readonly INetworkBootstrap _networkBootstrap;
		private readonly IPacketRegistry _packetRegistry;

		public PacketEncoder(INetworkBootstrap bootstrap, IPacketRegistry registry)
		{
			_networkBootstrap = bootstrap;
			_packetRegistry = registry;
		}

		protected override void Encode(IChannelHandlerContext context, IPacket message, IByteBuffer output)
		{
			var id = _packetRegistry.GetPacketId(message);
			if (id == null)
			{
				_networkBootstrap.GetLogger().Warn("Outgoing packet is not registered");
				return;
			}

			if (!(message is IOutPacket outPacket))
			{
				_networkBootstrap.GetLogger().Warn($"Outgoing packet with id {id} is not declared as outgoing");
				return;
			}

			output.WriteInt((int) id);
			outPacket.Write(output);
		}
	}
}