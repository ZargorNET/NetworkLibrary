using System;
using System.Collections.Concurrent;

namespace Core.Packet
{
	public class DefaultPacketRegistry : IPacketRegistry
	{
		private readonly ConcurrentDictionary<int, Type> _registry = new ConcurrentDictionary<int, Type>();

		public void RegisterPacket<T>(T packet, int id) where T : IPacket, new()
		{
			if (id == 0x0 || id == 0x1)
				throw new InvalidOperationException(
					"Packet id 0x0 (0) & 0x1 (1) are reserved for encryption!"); // TODO Add encryption
			if (!_registry.TryAdd(id, typeof(T))) throw new InvalidOperationException("Packet id already exists");
		}

		public void DeregisterPacket(int id)
		{
			_registry.TryRemove(id, out _);
		}

		public IPacket CreateNewPacket(int id)
		{
			if (!_registry.TryGetValue(id, out var packetType)) return null;
			return (IPacket) Activator.CreateInstance(packetType);
		}

		public int? GetPacketId(IPacket packet)
		{
			foreach (var kvp in _registry)
				if (kvp.Value == packet.GetType())
					return kvp.Key;

			return null;
		}

		public void Dispose()
		{
			_registry.Clear();
		}
	}
}