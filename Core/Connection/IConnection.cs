using System.Threading.Tasks;
using Core.Packet;
using Core.Packet.Handler;
using DotNetty.Transport.Channels;

namespace Core.Connection
{
	public interface IConnection
	{
		/// <summary>
		///     Closes the connection
		/// </summary>
		/// <returns>A task which completes when the channel successfully closed</returns>
		Task DisconnectAsync();

		/// <summary>
		///     Sends a packet asynchronously
		/// </summary>
		/// <param name="packet">The packet to send</param>
		/// <returns>A task which completes when the packet was send</returns>
		Task SendPacketAsync(IOutPacket packet);

		/// <summary>
		///     Sets the current PacketHandler for the client
		/// </summary>
		/// <param name="packetHandler">The PacketHandler</param>
		/// <b>
		///     Warning: Should not be called directly in order to avoid weird confusion.  Use
		///     <see cref="IPacketHandlerRegistry.SetNextPacketHandler" /> instead!
		/// </b>
		void SetPacketHandler(PacketHandler packetHandler);

		IChannel GetChannel();

		PacketHandler GetCurrentPacketHandler();
	}
}