using System;
using System.Collections.Generic;
using System.Timers;
using ZNetLib.Core.Connection;

namespace ZNetLib.Core.Packet.Handler
{
	public class DefaultPacketHandlerRegistry : IPacketHandlerRegistry
	{
		private readonly IConnectionRegistry _connectionRegistry;
		private readonly object _lock = new object();
		private readonly INetworkBootstrap _networkBootstrap;
		private readonly List<Type> _registry = new List<Type>();
		private Timer _timer;

		public DefaultPacketHandlerRegistry(INetworkBootstrap networkBootstrap, IConnectionRegistry connectionRegistry)
		{
			_networkBootstrap = networkBootstrap;
			_connectionRegistry = connectionRegistry;
		}


		public void RegisterPacketHandler(params Type[] packetHandler)
		{
			foreach (var type in packetHandler)
				if (!typeof(PacketHandler).IsAssignableFrom(type))
					throw new ArgumentException($"Type {type.FullName} is not assignable from {nameof(PacketHandler)}");

			lock (_lock)
			{
				_registry.AddRange(packetHandler);
			}
		}

		public PacketHandler CreatePacketHandler(int index, IConnection connection)
		{
			Type type;
			lock (_lock)
			{
				type = _registry[index];
			}

			if (type == null) throw new InvalidOperationException($"PacketHandler at index {index} is null");
			return (PacketHandler) Activator.CreateInstance(type, _networkBootstrap, this, _connectionRegistry,
				connection);
		}

		public PacketHandler CreateDefaultPacketHandler(IConnection connection)
		{
			return CreatePacketHandler(0, connection);
		}

		public int? GetPacketHandlerIndex(PacketHandler handler)
		{
			lock (_lock)
			{
				var index = _registry.IndexOf(handler.GetType());
				if (index == -1)
					return null;
				return index;
			}
		}

		public bool SetNextPacketHandler(IConnection connection)
		{
			var currentIndex = GetPacketHandlerIndex(connection.GetCurrentPacketHandler());
			if (currentIndex == null)
			{
				_networkBootstrap.GetLogger()
					.Error(
						$"Could not set next PacketHandler for {connection.GetChannel()} because the PacketHandler {connection.GetCurrentPacketHandler().GetType().FullName} is not registered");
				return false;
			}

			var index = (int) currentIndex;

			index++;
			Type nextType;
			lock (_lock)
			{
				nextType = index >= _registry.Count ? null : _registry[index];
			}

			if (nextType == null || nextType == connection.GetCurrentPacketHandler().GetType())
			{
				_networkBootstrap.GetLogger()
					.Warn(
						$"Could not set next PacketHandler for {connection.GetChannel()} because there is no next handler");
				return false;
			}

			var next = CreatePacketHandler(index, connection);
			connection.SetPacketHandler(next);
			return true;
		}

		public void StartTicking(double interval)
		{
			_timer = new Timer(interval);
			_timer.Elapsed += (sender, args) =>
			{
				var time = new DateTimeOffset(DateTime.Now);
				foreach (var connection in _connectionRegistry.CopyList())
					connection.GetCurrentPacketHandler().OnTick(time.ToUnixTimeMilliseconds());
			};
			_timer.Start();
		}

		public void StopTicking()
		{
			_timer?.Stop();
		}

		public void Dispose()
		{
			StopTicking();
			lock (_lock)
			{
				_registry.Clear();
			}
		}
	}
}