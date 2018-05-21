using System;
using System.Collections.Generic;
using System.Text;

namespace DansWorld.Common.Net
{
    /// <summary>
    /// a utility class that is creates immutable packet data.
    /// </summary>
    public class PacketBuilder
    {
        /// <summary>
        /// packet payload
        /// </summary>
        private byte[] _data;
        public int Length { get { return _data.Length; } }
        public PacketFamily Family { get { return (PacketFamily)_data[0]; } }
        public PacketAction Action { get { return (PacketAction)_data[1]; } }

        public PacketBuilder()
        {
            _data = new byte[0];
        }

        /// <summary>
        /// initiate a packet builder with pre-defined family and actions
        /// </summary>
        /// <param name="family"></param>
        /// <param name="action"></param>
        public PacketBuilder(PacketFamily family, PacketAction action)
        {
            _data = new byte[] { (byte)family, (byte)action };
        }

        private PacketBuilder(byte[] data)
        {
            _data = data;
        }

        /// <summary>
        /// Adds a byte array to the packet payload
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public PacketBuilder AddBytes(byte[] bytes)
        {
            List<byte> list = new List<byte>(_data);
            list.AddRange(bytes);
            return new PacketBuilder(list.ToArray());
        }

        /// <summary>
        /// adds a singular byte to the packet payload
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public PacketBuilder AddByte(byte b)
        {
            return AddBytes(new byte[] { b });
        }

        /// <summary>
        /// adds a string to the packet payload
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public PacketBuilder AddString(string s)
        {
            return AddBytes(Encoding.ASCII.GetBytes(s));
        }

        /// <summary>
        /// adds an integer to the packet payload
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public PacketBuilder AddInt(int i)
        {
            return AddBytes(BitConverter.GetBytes(i));
        }

        /// <summary>
        /// returns a packet constructed by this builder instance
        /// </summary>
        /// <returns></returns>
        public Packet Build()
        {
            return new Packet(_data);
        }
    }
}
