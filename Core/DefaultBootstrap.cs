using System;
using System.Net;
using System.Threading.Tasks;
using Core.Connection;
using Core.Packet;
using Core.Packet.Handler;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using NLog;

namespace Core
{
	public class DefaultBootstrap : INetworkBootstrap
	{
		public enum BootstrapTypeEnum
		{
			Server,
			Client
		}

		private readonly Type _inboundHandlerType;
		private readonly double _packetHandlerTickingInterval;

		public readonly BootstrapTypeEnum BootstrapType;

		public readonly IConnectionRegistry ConnectionRegistry;
		public readonly IPacketHandlerRegistry PacketHandlerRegistry;
		public readonly IPacketRegistry PacketRegistry;

		private bool _active;
		private IEventLoopGroup _bossGroup;
		private Logger _logger = LogManager.GetLogger("NET");
		private IEventLoopGroup _workerGroup;

		public DefaultBootstrap(BootstrapTypeEnum bootstrapType, TimeSpan packetHandlerTickingInterval,
			IConnectionRegistry connectionRegistry = null,
			IPacketRegistry packetRegistry = null,
			IPacketHandlerRegistry packetHandlerRegistry = null, Type inboundHandlerType = null)
		{
			_inboundHandlerType = inboundHandlerType ?? typeof(DefaultPacketInboundHandler);
			if (!typeof(PacketInboundHandler).IsAssignableFrom(_inboundHandlerType))
				throw new ArgumentException(
					$"{_inboundHandlerType.FullName} is not assignable from {typeof(PacketInboundHandler).FullName}");
			BootstrapType = bootstrapType;
			_packetHandlerTickingInterval = packetHandlerTickingInterval.TotalMilliseconds;
			ConnectionRegistry = connectionRegistry ?? new DefaultConnectionRegistry();
			PacketRegistry = packetRegistry ?? new DefaultPacketRegistry();
			PacketHandlerRegistry = packetHandlerRegistry ?? new DefaultPacketHandlerRegistry(this, ConnectionRegistry);
		}

		public async Task Start(IPEndPoint address)
		{
			if (IsActive()) throw new InvalidOperationException("Bootstrap is already active");
			_active = true;

			PacketHandlerRegistry.StartTicking(_packetHandlerTickingInterval);
			_bossGroup = new MultithreadEventLoopGroup(1);
			switch (BootstrapType)
			{
				case BootstrapTypeEnum.Server:
					try
					{
						_workerGroup = new MultithreadEventLoopGroup();
						var bootstrap = new ServerBootstrap();
						bootstrap.Group(_bossGroup, _workerGroup);
						bootstrap.Channel<TcpServerSocketChannel>();

						bootstrap
							.Option(ChannelOption.SoBacklog, 100)
							.Handler(new DebugLoggingHandler(this))
							.ChildHandler(GetChannelInitializer());

						var boundChannel = await bootstrap.BindAsync(address);

						_logger.Info("Server bootstrap started");

						while (_active) await Task.Delay(200);

						await boundChannel.CloseAsync();
					}
					catch (Exception exe)
					{
						throw new BootstrapConnectionClosed("Bootstrap connection was unexpectedly closed", exe);
					}
					finally
					{
						await FinallyStop();
					}

					break;
				case BootstrapTypeEnum.Client:
					try
					{
						var bootstrap = new Bootstrap();
						bootstrap.Group(_bossGroup);
						bootstrap.Channel<TcpSocketChannel>();

						bootstrap
							.Handler(new DebugLoggingHandler(this))
							.Handler(GetChannelInitializer());

						await bootstrap.ConnectAsync(address);

						_logger.Info("Client bootstrap started");

						while (_active) await Task.Delay(200);
					}
					catch (Exception exe)
					{
						throw new BootstrapConnectionClosed("Bootstrap connection was unexpectedly closed", exe);
					}
					finally
					{
						await FinallyStop();
					}

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public Task Stop()
		{
			_active = false;
			return Task.CompletedTask;
		}

		public bool IsActive()
		{
			return _active;
		}

		public void SetLogger(Logger logger)
		{
			_logger = logger;
		}

		public Logger GetLogger()
		{
			return _logger;
		}

		public void Dispose()
		{
			ConnectionRegistry.Dispose();
			PacketRegistry.Dispose();
			PacketHandlerRegistry.Dispose();
		}

		private async Task FinallyStop()
		{
			PacketHandlerRegistry.StopTicking();
			if (BootstrapType == BootstrapTypeEnum.Server)
				await _workerGroup.ShutdownGracefullyAsync();
			await _bossGroup.ShutdownGracefullyAsync();
		}

		private ActionChannelInitializer<IChannel> GetChannelInitializer()
		{
			return new ActionChannelInitializer<IChannel>(channel =>
			{
				var pipeline = channel.Pipeline;
				pipeline.AddLast("timeout", new ReadTimeoutHandler(30));
				pipeline.AddLast("frame_decoder",
					new LengthFieldBasedFrameDecoder(ByteOrder.LittleEndian, int.MaxValue, 0, 4, 0, 4, true));
				pipeline.AddLast("packet_decoder", new PacketDecoder(this, PacketRegistry));
				pipeline.AddLast("length_prepender",
					new LengthFieldPrepender(ByteOrder.LittleEndian, 4, 0, false));
				pipeline.AddLast("packet_encoder", new PacketEncoder(this, PacketRegistry));
				pipeline.AddLast("inbound_handler",
					(PacketInboundHandler) Activator.CreateInstance(_inboundHandlerType,
						this,
						PacketRegistry,
						ConnectionRegistry,
						PacketHandlerRegistry
					));
			});
		}
	}
}