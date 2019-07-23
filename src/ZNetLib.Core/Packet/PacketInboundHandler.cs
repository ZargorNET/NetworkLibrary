using System;
using DotNetty.Transport.Channels;
using ZNetLib.Core.Connection;
using ZNetLib.Core.Packet.Handler;

namespace ZNetLib.Core.Packet
{
	public abstract class PacketInboundHandler : SimpleChannelInboundHandler<IPacket>
	{
		protected readonly IConnectionRegistry _connectionRegistry;
		protected readonly INetworkBootstrap _networkBootstrap;
		protected readonly IPacketHandlerRegistry _packetHandlerRegistry;
		protected readonly IPacketRegistry _packetRegistry;
		protected IConnection _connection;

		protected PacketInboundHandler(INetworkBootstrap networkBootstrap, IPacketRegistry packetRegistry,
			IConnectionRegistry connectionRegistry, IPacketHandlerRegistry packetHandlerRegistry)
		{
			_networkBootstrap = networkBootstrap ?? throw new ArgumentNullException(nameof(networkBootstrap));
			_packetRegistry = packetRegistry ?? throw new ArgumentNullException(nameof(packetRegistry));
			_connectionRegistry = connectionRegistry ?? throw new ArgumentNullException(nameof(connectionRegistry));
			_packetHandlerRegistry =
				packetHandlerRegistry ?? throw new ArgumentNullException(nameof(packetHandlerRegistry));
		}
	}
}