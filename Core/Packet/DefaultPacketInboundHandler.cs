using Core.Connection;
using Core.Packet.Handler;
using DotNetty.Transport.Channels;

namespace Core.Packet
{
	public class DefaultPacketInboundHandler : PacketInboundHandler
	{
		private bool _registered;

		public DefaultPacketInboundHandler(INetworkBootstrap networkBootstrap, IPacketRegistry packetRegistry,
			IConnectionRegistry connectionRegistry, IPacketHandlerRegistry packetHandlerRegistry) : base(
			networkBootstrap, packetRegistry, connectionRegistry, packetHandlerRegistry)
		{
		}

		protected override void ChannelRead0(IChannelHandlerContext ctx, IPacket msg)
		{
			_connection.GetCurrentPacketHandler().FirePacket(msg as IInPacket);
		}

		public override void ChannelRegistered(IChannelHandlerContext context)
		{
			_connection = new DefaultConnection(context.Channel);
			_connectionRegistry.RegisterConnection(_connection);
			_connection.SetPacketHandler(_packetHandlerRegistry.CreateDefaultPacketHandler(_connection));
			_registered = true;
		}

		public override void ChannelUnregistered(IChannelHandlerContext context)
		{
			if (_connection == null) return;
			_connectionRegistry.DeregisterConnection(_connection);
			_connection.DisconnectAsync();
		}
	}
}