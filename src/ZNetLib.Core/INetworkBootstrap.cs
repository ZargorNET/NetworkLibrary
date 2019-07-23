using System;
using System.Net;
using System.Threading.Tasks;
using NLog;

namespace ZNetLib.Core
{
	public interface INetworkBootstrap : IDisposable
	{
		Task Start(IPEndPoint address);

		Task Stop();

		bool IsActive();

		void SetLogger(Logger logger);

		Logger GetLogger();
	}
}