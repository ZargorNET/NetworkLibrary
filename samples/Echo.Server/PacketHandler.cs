using Echo.Server.Packet;
using NLog;
using ZNetLib.Core;
using ZNetLib.Core.Connection;
using ZNetLib.Core.Packet;
using ZNetLib.Core.Packet.Handler;

namespace Echo.Server
{
	public class PacketHandler : ZNetLib.Core.Packet.Handler.PacketHandler
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public PacketHandler(INetworkBootstrap networkBootstrap, IPacketHandlerRegistry packetHandlerRegistry,
			IConnectionRegistry connectionRegistry, IConnection connection) : base(networkBootstrap,
			packetHandlerRegistry, connectionRegistry, connection)
		{
		}

		public override void OnActive()
		{
			_logger.Info("New client connected!");
		}

		public override void OnDisconnect()
		{
			_logger.Info("Client disconnected");
		}

		public override void OnTick(long timestamp)
		{
			// Keep alive will be handled by client
		}

		public override void Fallback(IInPacket packet)
		{
			_logger.Info($"Unknown packet received! {packet.GetType().FullName}");
		}

		[PacketMethod]
		public void OnKeepAlivePacket(KeepAlivePacket packet)
		{
			_logger.Info("KeepAlive!");
			Connection.SendPacketAsync(packet);
		}

		[PacketMethod]
		public void OnTextPacket(TextPacket packet)
		{
			Connection.SendPacketAsync(new TextPacket
			{
				Text = $"You said: {packet.Text}"
			});
		}
	}
}