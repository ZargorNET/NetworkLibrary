using System;
using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace ZNetLib.Core.Packet
{
	public class PacketDecoder : ByteToMessageDecoder
	{
		private readonly INetworkBootstrap _networkBootstrap;
		private readonly IPacketRegistry _packetRegistry;

		public PacketDecoder(INetworkBootstrap networkBootstrap, IPacketRegistry packetRegistry)
		{
			_networkBootstrap = networkBootstrap;
			_packetRegistry = packetRegistry;
		}

		protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
		{
			if (input.ReadableBytes == 0)
			{
				_networkBootstrap.GetLogger().Warn("Incoming packet has zero readable bytes");
				return;
			}

			var id = input.ReadUnsignedShort();
			var packet = _packetRegistry.CreateNewPacket(id);
			if (packet == null)
			{
				_networkBootstrap.GetLogger().Warn("Incoming packet did not have an id");
				return;
			}

			if (!(packet is IInPacket inpacket))
			{
				_networkBootstrap.GetLogger().Warn($"Incoming packet with the id {id} is not declared as incoming");
				return;
			}

			try
			{
				inpacket.Read(input);
			}
			catch (IndexOutOfRangeException)
			{
				_networkBootstrap.GetLogger()
					.Warn(
						$"Incoming packet with the id {id} was smaller than expected");
				return;
			}

			if (input.ReadableBytes > 0)
			{
				_networkBootstrap.GetLogger()
					.Warn($"Incoming packet with id {id} was {input.ReadableBytes} bytes larger than expected");
				return;
			}

			output.Add(inpacket);
		}
	}
}