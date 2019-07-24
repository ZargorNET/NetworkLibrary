using System;
using System.Collections.Concurrent;

namespace ZNetLib.Core.Packet
{
	public class DefaultPacketRegistry : IPacketRegistry
	{
		private readonly ConcurrentDictionary<ushort, Type> _registry = new ConcurrentDictionary<ushort, Type>();

		public void RegisterPacket<T>(T packet, ushort id) where T : IPacket, new()
		{
			if (!_registry.TryAdd(id, typeof(T))) throw new InvalidOperationException("Packet id already exists");
		}

		public void DeregisterPacket(ushort id)
		{
			_registry.TryRemove(id, out _);
		}

		public IPacket CreateNewPacket(ushort id)
		{
			if (!_registry.TryGetValue(id, out var packetType)) return null;
			return (IPacket) Activator.CreateInstance(packetType);
		}

		public ushort? GetPacketId(IPacket packet)
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