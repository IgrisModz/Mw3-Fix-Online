﻿using System;
using System.Linq;
using System.Text;

namespace Mw3_Fix_Online.Core
{
    internal class Extension
    {
        internal byte[] Dump { get; set; }

        internal Extension(int length)
        {
            Dump = new byte[length];
        }

        internal Extension()
        {

        }

        /// <summary>Read a signed byte.</summary>
        internal sbyte ReadSByte(uint offset)
        {
            byte[] buffer = new byte[1];
            Array.Copy(Dump, offset, buffer, 0, 1);
            return (sbyte)buffer[0];
        }

        /// <summary>Read a byte a check if his value. This return a bool according the byte detected.</summary>
        internal bool ReadBool(uint offset)
        {
            byte[] buffer = new byte[1];
            Array.Copy(Dump, offset, buffer, 0, 1);
            return buffer[0] != 0;
        }

        /// <summary>Read and return an integer 16 bits.</summary>
        internal short ReadInt16(uint offset, bool reverse = true)
        {
            byte[] buffer = new byte[2];
            Array.Copy(Dump, offset, buffer, 0, 2);
            if (reverse)
                Array.Reverse(buffer, 0, 2);
            return BitConverter.ToInt16(buffer, 0);
        }

        /// <summary>Read and return an integer 32 bits.</summary>
        internal int ReadInt32(uint offset, bool reverse = true)
        {
            byte[] buffer = new byte[4];
            Array.Copy(Dump, offset, buffer, 0, 4);
            if (reverse)
                Array.Reverse(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        /// <summary>Read and return an integer 64 bits.</summary>
        internal long ReadInt64(uint offset, bool reverse = true)
        {
            byte[] buffer = new byte[8];
            Array.Copy(Dump, offset, buffer, 0, 8);
            if (reverse)
                Array.Reverse(buffer, 0, 8);
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>Read and return a byte.</summary>
        internal byte ReadByte(uint offset)
        {
            byte[] buffer = new byte[1];
            Array.Copy(Dump, offset, buffer, 0, 1);
            return buffer[0];
        }

        /// <summary>Read and return an array of byte.</summary>
        internal byte[] ReadBytes(uint offset, int length)
        {
            byte[] buffer = new byte[length];
            Array.Copy(Dump, offset, buffer, 0, length);
            return buffer;
        }

        /// <summary>Read and return an unsigned integer 16 bits.</summary>
        internal ushort ReadUInt16(uint offset, bool reverse = true)
        {
            byte[] buffer = new byte[2];
            Array.Copy(Dump, offset, buffer, 0, 2);
            if (reverse)
                Array.Reverse(buffer, 0, 2);
            return BitConverter.ToUInt16(buffer, 0);
        }

        /// <summary>Read and return an unsigned integer 32 bits.</summary>
        internal uint ReadUInt32(uint offset, bool reverse = true)
        {
            byte[] buffer = new byte[4];
            Array.Copy(Dump, offset, buffer, 0, 4);
            if (reverse)
                Array.Reverse(buffer, 0, 4);
            return BitConverter.ToUInt32(buffer, 0);
        }

        /// <summary>Read and return an unsigned integer 64 bits.</summary>
        internal ulong ReadUInt64(uint offset, bool reverse = true)
        {
            byte[] buffer = new byte[8];
            Array.Copy(Dump, offset, buffer, 0, 8);
            if (reverse)
                Array.Reverse(buffer, 0, 8);
            return BitConverter.ToUInt64(buffer, 0);
        }

        /// <summary>Read and return a Float.</summary>
        internal float ReadFloat(uint offset, bool reverse = true)
        {
            byte[] buffer = new byte[4];
            Array.Copy(Dump, offset, buffer, 0, 4);
            if (reverse)
                Array.Reverse(buffer, 0, 4);
            return BitConverter.ToSingle(buffer, 0);
        }

        /// <summary>Read and return an array of Floats.</summary>
        internal float[] ReadFloats(uint offset, int arrayLength = 3, bool reverse = true)
        {
            float[] vec = new float[arrayLength];
            for (int i = 0; i < arrayLength; i++)
            {
                byte[] buffer = new byte[4];
                Array.Copy(Dump, offset + ((uint)i * 4), buffer, 0, 4);
                if (reverse)
                    Array.Reverse(buffer, 0, 4);
                vec[i] = BitConverter.ToSingle(buffer, 0);
            }
            return vec;
        }

        /// <summary>Read and return a Double.</summary>
        internal double ReadDouble(uint offset, bool reverse = true)
        {
            byte[] buffer = new byte[8];
            Array.Copy(Dump, offset, buffer, 0, 8);
            if (reverse)
                Array.Reverse(buffer, 0, 8);
            return BitConverter.ToDouble(buffer, 0);
        }

        /// <summary>Read a string very fast by buffer and stop only when a byte null is detected (0x00).</summary>
        internal string ReadString(uint offset)
        {
            int blocksize = 40;
            int scalesize = 0;
            string str = string.Empty;

            while (!str.Contains('\0'))
            {
                byte[] buffer = new byte[blocksize];
                Array.Copy(Dump, offset, buffer, 0, blocksize);
                str += Encoding.UTF8.GetString(buffer);
                scalesize += blocksize;
            }

            return str.Substring(0, str.IndexOf('\0'));
        }

        /// <summary>Write a signed byte.</summary>
        public void WriteSByte(uint offset, sbyte input)
        {
            byte[] buff = new byte[1];
            buff[0] = (byte)input;
            Array.Copy(buff, 0, Dump, offset, 1);
        }

        /// <summary>Write a sbyte array.</summary>
        /// 
        public void WriteSBytes(uint offset, sbyte[] input)
        {
            byte[] buff = (byte[])(Array)input;
            Array.Copy(buff, 0, Dump, offset, buff.Length);
        }

        /// <summary>Write a boolean.</summary>
        public void WriteBool(uint offset, bool input)
        {
            byte[] buff = new byte[1];
            buff[0] = input ? (byte)1 : (byte)0;
            Array.Copy(buff, 0, Dump, offset, 1);
        }

        /// <summary>Write an interger 16 bits.</summary>
        public void WriteInt16(uint offset, short input, bool reverse = true)
        {
            byte[] buff = new byte[2];
            BitConverter.GetBytes(input).CopyTo(buff, 0);
            if (reverse)
                Array.Reverse(buff, 0, 2);
            Array.Copy(buff, 0, Dump, offset, 2);
        }

        /// <summary>Write an array of interger 16 bits.</summary>
        public void WriteInt16(uint offset, short[] input, bool reverse = true)
        {
            int length = input.Length;
            byte[] buff = new byte[length * 2];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(buff, 0);
                if (reverse)
                    Array.Reverse(buff);
                Array.Copy(buff, 0, Dump, offset + ((uint)i * 2), buff.Length);
            }
        }

        /// <summary>Write an integer 32 bits.</summary>
        public void WriteInt32(uint offset, int input, bool reverse = true)
        {
            byte[] buff = new byte[4];
            BitConverter.GetBytes(input).CopyTo(buff, 0);
            if (reverse)
                Array.Reverse(buff, 0, 4);
            Array.Copy(buff, 0, Dump, offset, 4);
        }

        /// <summary>Write an array of interger 32 bits.</summary>
        public void WriteInt32(uint offset, int[] input, bool reverse = true)
        {
            int length = input.Length;
            byte[] buff = new byte[length * 4];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(buff, 0);
                if (reverse)
                    Array.Reverse(buff);
                Array.Copy(buff, 0, Dump, offset + ((uint)i * 4), buff.Length);
            }
        }

        /// <summary>Write an integer 64 bits.</summary>
        public void WriteInt64(uint offset, long input, bool reverse = true)
        {
            byte[] buff = new byte[8];
            BitConverter.GetBytes(input).CopyTo(buff, 0);
            if (reverse)
                Array.Reverse(buff, 0, 8);
            Array.Copy(buff, 0, Dump, offset, 8);
        }

        /// <summary>Write an array of interger 64 bits.</summary>
        public void WriteInt64(uint offset, long[] input, bool reverse = true)
        {
            int length = input.Length;
            byte[] buff = new byte[length * 8];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(buff, 0);
                if (reverse)
                    Array.Reverse(buff);
                Array.Copy(buff, 0, Dump, offset + ((uint)i * 8), buff.Length);
            }
        }

        /// <summary>Write a byte.</summary>
        public void WriteByte(uint offset, byte input)
        {
            byte[] buff = new byte[1];
            buff[0] = input;
            Array.Copy(buff, 0, Dump, offset, 1);
        }

        /// <summary>Write a byte array.</summary>
        public void WriteBytes(uint offset, byte[] input)
        {
            byte[] buff = input;
            Array.Copy(buff, 0, Dump, offset, buff.Length);
        }

        /// <summary>Write a string.</summary>
        public void WriteString(uint offset, string input)
        {
            byte[] buff = Encoding.Default.GetBytes(input);
            Array.Resize(ref buff, buff.Length + 1);
            Array.Copy(buff, 0, Dump, offset, buff.Length);
        }

        /// <summary>Write an unsigned integer 16 bits.</summary>
        public void WriteUInt16(uint offset, ushort input, bool reverse = true)
        {
            byte[] buff = new byte[2];
            BitConverter.GetBytes(input).CopyTo(buff, 0);
            if (reverse)
                Array.Reverse(buff, 0, 2);
            Array.Copy(buff, 0, Dump, offset, 2);
        }

        /// <summary>Write an array of unsigned integer 16 bits.</summary>
        public void WriteUInt16(uint offset, ushort[] input, bool reverse = true)
        {
            int length = input.Length;
            byte[] buff = new byte[length * 2];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(buff, 0);
                if (reverse)
                    Array.Reverse(buff);
                Array.Copy(buff, 0, Dump, offset + ((uint)i * 2), buff.Length);
            }
        }

        /// <summary>Write an unsigned integer 32 bits.</summary>
        public void WriteUInt32(uint offset, uint input, bool reverse = true)
        {
            byte[] buff = new byte[4];
            BitConverter.GetBytes(input).CopyTo(buff, 0);
            if (reverse)
                Array.Reverse(buff, 0, 4);
            Array.Copy(buff, 0, Dump, offset, 4);
        }

        /// <summary>Write an array of unsigned integer 32 bits.</summary>
        public void WriteUInt32(uint offset, uint[] input, bool reverse = true)
        {
            int length = input.Length;
            byte[] buff = new byte[length * 4];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(buff, 0);
                if (reverse)
                    Array.Reverse(buff);
                Array.Copy(buff, 0, Dump, offset + ((uint)i * 4), buff.Length);
            }
        }

        /// <summary>Write an unsigned integer 64 bits.</summary>
        public void WriteUInt64(uint offset, ulong input, bool reverse = true)
        {
            byte[] buff = new byte[8];
            BitConverter.GetBytes(input).CopyTo(buff, 0);
            if (reverse)
                Array.Reverse(buff, 0, 8);
            Array.Copy(buff, 0, Dump, offset, 8);
        }

        /// <summary>Write an array of unsigned integer 64 bits.</summary>
        public void WriteUInt64(uint offset, ulong[] input, bool reverse = true)
        {
            int length = input.Length;
            byte[] buff = new byte[length * 8];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(buff, 0);
                if (reverse)
                    Array.Reverse(buff);
                Array.Copy(buff, 0, Dump, offset + ((uint)i * 8), buff.Length);
            }
        }

        /// <summary>Write a Float.</summary>
        public void WriteFloat(uint offset, float input, bool reverse = true)
        {
            byte[] buff = new byte[4];
            BitConverter.GetBytes(input).CopyTo(buff, 0);
            if (reverse)
                Array.Reverse(buff, 0, 4);
            Array.Copy(buff, 0, Dump, offset, 4);
        }

        /// <summary>Write an array of Floats.</summary>
        public void WriteFloats(uint offset, float[] input, bool reverse = true)
        {
            byte[] buff = new byte[4];
            for (int i = 0; i < input.Length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(buff, 0);
                if (reverse)
                    Array.Reverse(buff, 0, 4);
                Array.Copy(buff, 0, Dump, offset + ((uint)i * 4), buff.Length);
            }
        }

        /// <summary>Write a double.</summary>
        public void WriteDouble(uint offset, double input, bool reverse = true)
        {
            byte[] buff = new byte[8];
            BitConverter.GetBytes(input).CopyTo(buff, 0);
            if (reverse)
                Array.Reverse(buff, 0, 8);
            Array.Copy(buff, 0, Dump, offset, 8);
        }

        /// <summary>Write an array of doubles.</summary>
        public void WriteDouble(uint offset, double[] input, bool reverse = true)
        {
            int length = input.Length;
            byte[] buff = new byte[length * 8];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(buff, 0);
                if (reverse)
                    Array.Reverse(buff);
                Array.Copy(buff, 0, Dump, offset + ((uint)i * 8), buff.Length);
            }
        }
    }
}
