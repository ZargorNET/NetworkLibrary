using System;
using System.Threading.Tasks;
using Echo.Client.Packet;
using NLog;
using ZNetLib.Core;
using ZNetLib.Core.Connection;
using ZNetLib.Core.Packet;
using ZNetLib.Core.Packet.Handler;

namespace Echo.Client
{
	public class PacketHandler : ZNetLib.Core.Packet.Handler.PacketHandler
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private long _lastKeepAliveSent;

		public PacketHandler(INetworkBootstrap networkBootstrap, IPacketHandlerRegistry packetHandlerRegistry,
			IConnectionRegistry connectionRegistry, IConnection connection) : base(networkBootstrap,
			packetHandlerRegistry, connectionRegistry, connection)
		{
		}


		public override void OnActive()
		{
			_logger.Info("Connection established!");
			Task.Run(async () =>
			{
				await Task.Delay(200); // WAIT TO ENSURE CONNECTION IS ESTABLISHED
				await Connection.SendPacketAsync(new TextPacket
				{
					Text = "Hello World!"
				});
			});
		}

		public override void OnDisconnect()
		{
			_logger.Info("Connection disconnected");
		}

		public override void OnTick(long timestamp)
		{
			if (timestamp < _lastKeepAliveSent) return;
			_lastKeepAliveSent = timestamp + 10000; // WE TICK EVERY SECOND, SO THIS IS BASICALLY: NOW + 10 SECONDS
			Connection.SendPacketAsync(new KeepAlivePacket());
		}

		public override void Fallback(IInPacket packet)
		{
			_logger.Info($"Unknown packet received! {packet.GetType().FullName}");
		}

		[PacketMethod]
		public void OnKeepAlive(KeepAlivePacket packet)
		{
			// DO NOTHING
		}

		[PacketMethod]
		public void OnTextPacket(TextPacket packet)
		{
			Console.WriteLine($"Received text packet!: {packet.Text}");
		}
	}
}