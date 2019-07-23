using System;
using System.Collections.ObjectModel;
using DotNetty.Transport.Channels;

namespace ZNetLib.Core.Connection
{
	public interface IConnectionRegistry : IDisposable
	{
		void RegisterConnection(IConnection connection);

		void DeregisterConnection(IConnection connection);

		IConnection GetConnectionFromChannel(IChannel channel);

		ReadOnlyCollection<IConnection> CopyList();
	}
}