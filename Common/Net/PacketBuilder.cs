using System;
using System.Collections.Generic;
using System.Text;

namespace DansWorld.Common.Net
{
    public class PacketBuilder
    {
        private byte[] _data;
        public int Length { get { return _data.Length; } }
        public PacketFamily Family { get { return (PacketFamily)_data[0]; } }
        public PacketAction Action { get { return (PacketAction)_data[1]; } }

        public PacketBuilder()
        {
            _data = new byte[0];
        }

        public PacketBuilder(PacketFamily family, PacketAction action)
        {
            _data = new byte[] { (byte)family, (byte)action };
        }

        private PacketBuilder(byte[] data)
        {
            _data = data;
        }

        public PacketBuilder AddBytes(byte[] bytes)
        {
            List<byte> list = new List<byte>(_data);
            list.AddRange(bytes);
            return new PacketBuilder(list.ToArray());
        }

        public PacketBuilder AddByte(byte b)
        {
            return AddBytes(new byte[] { b });
        }

        public PacketBuilder AddString(string s)
        {
            return AddBytes(Encoding.ASCII.GetBytes(s));
        }

        public Packet Build()
        {
            return new Packet(_data);
        }
    }
}
