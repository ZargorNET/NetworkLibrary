using System;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using ZNetLib.Core.Packet;
using ZNetLib.Core.Packet.Handler;

namespace ZNetLib.Core.Connection
{
	public class DefaultConnection : IConnection
	{
		private readonly IChannel _channel;
		private bool _disconnected;
		private PacketHandler _handler;

		public DefaultConnection(IChannel channel, PacketHandler handler)
		{
			_channel = channel;
			SetPacketHandler(handler);
		}

		internal DefaultConnection(IChannel channel)
		{
			_channel = channel;
		}

		public async Task DisconnectAsync()
		{
			if (_disconnected) return;

			_disconnected = true;
			_handler.OnDisconnect();
			await _channel.CloseAsync();
		}

		public Task SendPacketAsync(IOutPacket packet)
		{
			return _channel.WriteAndFlushAsync(packet);
		}

		public void SetPacketHandler(PacketHandler packetHandler)
		{
			_handler = packetHandler ??
			           throw new ArgumentNullException(nameof(packetHandler), "Packet handler can't be null");
			_handler.OnActive();
		}

		public IChannel GetChannel()
		{
			return _channel;
		}

		public PacketHandler GetCurrentPacketHandler()
		{
			return _handler;
		}
	}
}