using System;
using System.Net;
using System.Threading.Tasks;
using Echo.Client.Packet;
using ZNetLib.Core;

namespace Echo.Client
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			new Program().MainAsync().GetAwaiter().GetResult();
		}

		private async Task MainAsync()
		{
			using var bootstrap =
				new DefaultBootstrap(DefaultBootstrap.BootstrapTypeEnum.Client, TimeSpan.FromSeconds(1));

			bootstrap.PacketRegistry.RegisterPacket(new KeepAlivePacket(), 0x2);
			bootstrap.PacketRegistry.RegisterPacket(new TextPacket(), 0x3);

			bootstrap.PacketHandlerRegistry.RegisterPacketHandler(typeof(PacketHandler));

			await bootstrap.Start(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080)); // Keep the main thread running
			Console.WriteLine("Closed!");
		}
	}
}