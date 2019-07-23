using System;

namespace ZNetLib.Core.Packet
{
	public interface IPacketRegistry : IDisposable
	{
		void RegisterPacket<T>(T packet, int id) where T : IPacket, new();

		void DeregisterPacket(int id);

		IPacket CreateNewPacket(int id);

		int? GetPacketId(IPacket packet);
	}
}