using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DansWorld.Common.Net
{
    public class Packet
    {
        public PacketFamily Family { get; private set; }
        public PacketAction Action { get; private set; }
        public byte[] RawData { get; private set; }
        public int ReadPosition { get; private set; }

        public int Length { get { return RawData.Length; } }

        public Packet(byte[] data) 
        {
            Family = (PacketFamily)data[0];
            Action = (PacketAction)data[1];
            RawData = data;
            ReadPosition = 2;
        }

        private void ThrowIfOutOfBounds(int bytes)
        {
            if (ReadPosition + bytes > Length)
                throw new InvalidOperationException("Operation is out of bounds of the packet");
        }

        public byte PeekByte()
        {
            ThrowIfOutOfBounds(0);
            return RawData[ReadPosition];
        }
        public IEnumerable<byte> PeekBytes(int length)
        {
            ThrowIfOutOfBounds(length);
            List<byte> bytes = new List<byte>(length);
            for (var i = ReadPosition; i < ReadPosition + length; ++i)
                bytes.Add(RawData[i]);
            return bytes;
        }

        public string PeekString(int length)
        {
            return Encoding.ASCII.GetString(PeekBytes(length).ToArray());
        }

        public byte ReadByte()
        {
            byte ret = PeekByte();
            ReadPosition += 1;
            return ret;
        }

        public string ReadString(int length)
        {
            var ret = PeekString(length);
            ReadPosition += length;
            return ret;
        }
    }
}
