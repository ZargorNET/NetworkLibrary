using System;
using ZNetLib.Core.Connection;

namespace ZNetLib.Core.Packet.Handler
{
	public interface IPacketHandlerRegistry : IDisposable
	{
		void RegisterPacketHandler(params Type[] packetHandler);

		PacketHandler CreatePacketHandler(int index, IConnection connection);

		PacketHandler CreateDefaultPacketHandler(IConnection connection);

		int? GetPacketHandlerIndex(PacketHandler handler);

		bool SetNextPacketHandler(IConnection connection);

		void StartTicking(double interval);

		void StopTicking();
	}
}