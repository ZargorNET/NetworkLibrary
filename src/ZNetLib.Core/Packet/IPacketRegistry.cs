using System;

namespace ZNetLib.Core.Packet
{
	public interface IPacketRegistry : IDisposable
	{
		void RegisterPacket<T>(T packet, ushort id) where T : IPacket, new();

		void DeregisterPacket(ushort id);

		IPacket CreateNewPacket(ushort id);

		ushort? GetPacketId(IPacket packet);
	}
}