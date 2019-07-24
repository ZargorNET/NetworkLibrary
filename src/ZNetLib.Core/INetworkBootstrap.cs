using System;
using System.Net;
using System.Threading.Tasks;
using NLog;

namespace ZNetLib.Core
{
	public interface INetworkBootstrap : IDisposable
	{
		Task StartAsync(IPEndPoint address);

		Task StopAsync();

		bool IsActive();

		void SetLogger(Logger logger);

		Logger GetLogger();
	}
}