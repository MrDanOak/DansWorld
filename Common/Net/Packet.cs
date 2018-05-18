using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DansWorld.Common.Net
{
    public class Packet
    {
        /// <summary>
        /// Family of the packet
        /// </summary>
        public PacketFamily Family { get; private set; }
        /// <summary>
        /// Action of the packet
        /// </summary>
        public PacketAction Action { get; private set; }
        /// <summary>
        /// Byte array of the raw data
        /// </summary>
        public byte[] RawData { get; private set; }
        /// <summary>
        /// Position of the read pointer
        /// </summary>
        public int ReadPosition { get; private set; }
        /// <summary>
        /// Length of the packet
        /// </summary>
        public int Length { get { return RawData.Length; } }

        /// <summary>
        /// Constructor for the packet
        /// </summary>
        /// <param name="data">byte array of the data</param>
        public Packet(byte[] data) 
        {
            Family = (PacketFamily)data[0];
            Action = (PacketAction)data[1];
            RawData = data;
            ReadPosition = 2;
        }

        /// <summary>
        /// Throws out of bounds exception if the amount that is about to be read exceeds the length of the packet
        /// </summary>
        /// <param name="bytes">number of bytes to read</param>
        private void ThrowIfOutOfBounds(int bytes)
        {
            if (ReadPosition + bytes > Length)
                throw new InvalidOperationException("Operation is out of bounds of the packet");
        }

        /// <summary>
        /// Seeks 1 byte ahead but does not move the read pointer
        /// </summary>
        /// <returns>the byte to be read</returns>
        public byte PeekByte()
        {
            ThrowIfOutOfBounds(0);
            return RawData[ReadPosition];
        }
        
        /// <summary>
        /// Seeks 1 Integer ahead but does not move the read pointer
        /// </summary>
        /// <returns>the integer to be read</returns>
        public int PeekInt()
        {
            ThrowIfOutOfBounds(3);

            var bytes = new[]
            {
                RawData[ReadPosition],
                RawData[ReadPosition + 1],
                RawData[ReadPosition + 2],
                RawData[ReadPosition + 3]
            };
            return BitConverter.ToInt32(bytes, 0);
        }
        /// <summary>
        /// looks a number of bytes ahead but does not move the read pointer
        /// </summary>
        /// <param name="length">number of bytes to read</param>
        /// <returns>an array of bytes</returns>
        public IEnumerable<byte> PeekBytes(int length)
        {
            ThrowIfOutOfBounds(length);
            List<byte> bytes = new List<byte>(length);
            for (var i = ReadPosition; i < ReadPosition + length; ++i)
                bytes.Add(RawData[i]);
            return bytes;
        }

        /// <summary>
        /// Looks ahead the amount of bytes for the length of the string requested
        /// </summary>
        /// <param name="length">length of the string to peek</param>
        /// <returns>the string that was requested</returns>
        public string PeekString(int length)
        {
            return Encoding.ASCII.GetString(PeekBytes(length).ToArray());
        }

        /// <summary>
        /// Peeks a byte and if no exception is thrown the read pointer is moved 1 byte
        /// </summary>
        /// <returns>a byte in front of the read pointer</returns>
        public byte ReadByte()
        {
            byte ret = PeekByte();
            ReadPosition += 1;
            return ret;
        }

        /// <summary>
        /// Peeks an integer and if no exception is thrown the read pointer is moved 4 bytes
        /// </summary>
        /// <returns>a byte in front of the read pointer</returns>
        public int ReadInt()
        {
            int ret = PeekInt();
            ReadPosition += 4;
            return ret;
        }

        /// <summary>
        /// Peeks a string of a given length and if no exception is thrown the read 
        /// pointer is moved the amount of bytes equivalent of that of the length of the string 
        /// </summary>
        /// <param name="length">Length of the string</param>
        /// <returns>a string of the given length</returns>
        public string ReadString(int length)
        {
            var ret = PeekString(length);
            ReadPosition += length;
            return ret;
        }
    }
}
