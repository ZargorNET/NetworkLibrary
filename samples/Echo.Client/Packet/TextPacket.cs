using System.Text;
using DotNetty.Buffers;
using ZNetLib.Core.Packet;

namespace Echo.Client.Packet
{
	public class TextPacket : IInOutPacket
	{
		public string Text;

		public void Read(IByteBuffer buffer)
		{
			var l = buffer.ReadInt();
			Text = buffer.ReadString(l, Encoding.UTF8);
		}

		public void Write(IByteBuffer buffer)
		{
			buffer.WriteInt(Text.Length);
			buffer.WriteString(Text, Encoding.UTF8);
		}
	}
}