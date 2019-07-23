using System;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using NLog;

namespace Core
{
	public class DebugLoggingHandler : ChannelHandlerAdapter
	{
		private readonly Logger _logger;

		public DebugLoggingHandler(INetworkBootstrap networkBootstrap)
		{
			_logger = networkBootstrap.GetLogger();
		}

		public override void HandlerAdded(IChannelHandlerContext context)
		{
			_logger.Debug($"{context.Channel} HANDLER_ADDED");
		}

		public override void HandlerRemoved(IChannelHandlerContext context)
		{
			_logger.Debug($"{context.Channel} HANDLER_REMOVED");
		}

		public override void ChannelRegistered(IChannelHandlerContext context)
		{
			_logger.Debug($"{context.Channel} REGISTERED");
			context.FireChannelRegistered();
		}

		public override void ChannelUnregistered(IChannelHandlerContext context)
		{
			_logger.Debug($"{context.Channel} UNREGISTERED");
			context.FireChannelUnregistered();
		}

		public override void ChannelActive(IChannelHandlerContext context)
		{
			_logger.Debug($"{context.Channel} ACTIVE");
			context.FireChannelActive();
		}

		public override void ChannelInactive(IChannelHandlerContext context)
		{
			_logger.Debug($"{context.Channel} INACTIVE");
			context.FireChannelInactive();
		}

		public override void ChannelRead(IChannelHandlerContext context, object message)
		{
			_logger.Debug($"{context.Channel} READ");
			context.FireChannelRead(message);
		}

		public override void ChannelReadComplete(IChannelHandlerContext context)
		{
			_logger.Debug($"{context.Channel} READ_COMPLETED");
			context.FireChannelReadComplete();
		}

		public override void ChannelWritabilityChanged(IChannelHandlerContext context)
		{
			_logger.Debug($"{context.Channel} WRITABILITY_CHANGED: {context.Channel.IsWritable}");
			context.FireChannelWritabilityChanged();
		}

		public override void UserEventTriggered(IChannelHandlerContext context, object evt)
		{
			_logger.Debug($"{context.Channel} USER_EVENT: {evt}");
			context.FireUserEventTriggered(evt);
		}

		public override Task WriteAsync(IChannelHandlerContext context, object message)
		{
			_logger.Debug($"{context.Channel} WRITE: {message}");
			return context.WriteAsync(message);
		}

		public override void Flush(IChannelHandlerContext context)
		{
			_logger.Debug($"{context.Channel} FLSUH");
		}

		public override Task BindAsync(IChannelHandlerContext context, EndPoint localAddress)
		{
			_logger.Debug($"{context.Channel} BIND: {localAddress}");
			return context.BindAsync(localAddress);
		}

		public override Task ConnectAsync(IChannelHandlerContext context, EndPoint remoteAddress, EndPoint localAddress)
		{
			_logger.Debug($"{context.Channel} CONNECT: {remoteAddress} ; {localAddress}");
			return context.BindAsync(localAddress);
		}

		public override Task DisconnectAsync(IChannelHandlerContext context)
		{
			_logger.Debug($"{context.Channel} DISCONNECT");
			return context.DisconnectAsync();
		}

		public override Task CloseAsync(IChannelHandlerContext context)
		{
			_logger.Debug($"{context.Channel} CLOSE");
			return context.CloseAsync();
		}

		public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
		{
			_logger.Debug($"{context.Channel} EXCEPTION: {exception}");
			context.FireExceptionCaught(exception);
		}

		public override Task DeregisterAsync(IChannelHandlerContext context)
		{
			_logger.Debug($"{context.Channel} DEREGISTER");
			return context.DeregisterAsync();
		}
	}
}