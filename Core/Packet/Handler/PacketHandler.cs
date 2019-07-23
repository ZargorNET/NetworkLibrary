using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.Connection;

namespace Core.Packet.Handler
{
	/// <summary>
	///     The heart of every network connection.
	///     It's main job is to handle all incoming packets but it fits because of the <see cref="OnTick" /> method also well
	///     for keep-alive packets.
	///     Can be switched out via <see cref="IPacketHandlerRegistry.SetNextPacketHandler" /> for handling efficiently
	///     multiple stats (for example login state & play state)
	/// </summary>
	/// <example>
	///     To listen for specific packets just add these methods to your class
	///     <code>
	/// 	[PacketMethod]
	///  public void OnYourPacket(YourPacket packet) {
	/// 		// HANDLE PACKET
	///  	// FOR EXMAPLE:
	///  	Connection.SendPacketAsync(new YourOutPacket("hello world"));
	///  }
	/// 
	///  [PacketMethod]
	///  public void OnYourSecondPacketType(YourSecondPacket packet) {
	/// 		// HANDLE PACKET
	///  }
	///  </code>
	/// </example>
	public abstract class PacketHandler
	{
		protected IConnection Connection;
		protected IConnectionRegistry ConnectionRegistry;
		protected INetworkBootstrap NetworkBootstrap;
		protected IPacketHandlerRegistry PacketHandlerRegistry;

		protected PacketHandler(INetworkBootstrap networkBootstrap, IPacketHandlerRegistry packetHandlerRegistry,
			IConnectionRegistry connectionRegistry, IConnection connection)
		{
			NetworkBootstrap = networkBootstrap;
			PacketHandlerRegistry = packetHandlerRegistry;
			ConnectionRegistry = connectionRegistry;
			Connection = connection;
		}

		/// <summary>
		///     Called when the PacketHandler gets set
		/// </summary>
		public abstract void OnActive();

		/// <summary>
		///     If server: Called when the client disconnects.
		///     If client: Called when the connection closed.
		/// </summary>
		public abstract void OnDisconnect();

		/// <summary>
		///     Gets called every X seconds with the current timestamp.
		///     Useful for keep-alive packets
		/// </summary>
		/// See
		/// <see cref="DefaultBootstrap" />
		/// how the default way is to define the interval
		/// <param name="timestamp">The current UTC timestamp in milliseconds</param>
		public abstract void OnTick(long timestamp);

		/// <summary>
		///     Gets called when no specific packet method has been specified.
		///     Usually useful for discarding all packets except X
		/// </summary>
		/// <param name="packet">The incoming packet</param>
		[PacketMethod]
		public abstract void Fallback(IInPacket packet);

		public void FirePacket(IInPacket packet)
		{
			var methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(m => m.GetCustomAttributes(typeof(PacketMethodAttribute), false).Length > 0)
				.ToImmutableArray();
			var packetMethod =
				methods.FirstOrDefault(m => m.GetParameters().All(p => p.ParameterType == packet.GetType()));
			if (packetMethod == null)
				packetMethod = GetType().GetMethod(nameof(Fallback));

			if (packetMethod == null) throw new NullReferenceException("Fallback method is null");

			var attr = packetMethod.GetCustomAttribute<PacketMethodAttribute>(true);
			if (attr.RunAsync)
				Task.Run(() => packetMethod.Invoke(this, new object[] {packet}));
			else
				packetMethod.Invoke(this, new object[] {packet});
		}
	}
}