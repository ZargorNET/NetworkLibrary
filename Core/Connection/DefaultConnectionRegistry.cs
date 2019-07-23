using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DotNetty.Transport.Channels;

namespace Core.Connection
{
	public class DefaultConnectionRegistry : IConnectionRegistry
	{
		private readonly List<IConnection> _connections = new List<IConnection>();
		private readonly object _lock = new object();

		public void RegisterConnection(IConnection connection)
		{
			lock (_lock)
			{
				_connections.Add(connection);
			}
		}

		public void DeregisterConnection(IConnection connection)
		{
			lock (_lock)
			{
				_connections.Remove(connection);
			}
		}

		public IConnection GetConnectionFromChannel(IChannel channel)
		{
			lock (_lock)
			{
				return _connections.FirstOrDefault(c => Equals(c.GetChannel(), channel));
			}
		}

		public ReadOnlyCollection<IConnection> CopyList()
		{
			lock (_lock)
			{
				return new ReadOnlyCollection<IConnection>(_connections);
			}
		}

		public void Dispose()
		{
			lock (_lock)
			{
				_connections.Clear();
			}
		}
	}
}