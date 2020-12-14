using IgrisLib;
using System;
using System.Threading;

namespace Mw3_Fix_Online.Core
{
    public class RemoteProcedureCall
    {
        public PS3API PS3 { get; }

        public RemoteProcedureCall(PS3API ps3)
        {
            PS3 = ps3;
        }

        private RemoteProcedureCall()
        {

        }

        public enum Svscmd_type
        {
            SV_CMD_CAN_IGNORE = 0x0,
            SV_CMD_RELIABLE = 0x1,
        }

        public bool Enable()
        {
            byte enabled = PS3.Extension.ReadByte(0x27720C);
            if (enabled == 0x80)
            {
                byte[] WritePPC = new byte[] {
                    0x3F, 0x80, 0x10, 0x05, 0x81, 0x9C, 0x00, 0x48, 0x2C, 0x0C, 0x00, 0x00, 0x41, 0x82, 0x00, 0x78,
                    0x80, 0x7C, 0x00, 0x00, 0x80, 0x9C, 0x00, 0x04, 0x80, 0xBC, 0x00, 0x08, 0x80, 0xDC, 0x00, 0x0C,
                    0x80, 0xFC, 0x00, 0x10, 0x81, 0x1C, 0x00, 0x14, 0x81, 0x3C, 0x00, 0x18, 0x81, 0x5C, 0x00, 0x1C,
                    0x81, 0x7C, 0x00, 0x20, 0xC0, 0x3C, 0x00, 0x24, 0xC0, 0x5C, 0x00, 0x28, 0xC0, 0x7C, 0x00, 0x2C,
                    0xC0, 0x9C, 0x00, 0x30, 0xC0, 0xBC, 0x00, 0x34, 0xC0, 0xDC, 0x00, 0x38, 0xC0, 0xFC, 0x00, 0x3C,
                    0xC1, 0x1C, 0x00, 0x40, 0xC1, 0x3C, 0x00, 0x44, 0x7D, 0x89, 0x03, 0xA6, 0x4E, 0x80, 0x04, 0x21,
                    0x38, 0x80, 0x00, 0x00, 0x90, 0x9C, 0x00, 0x48, 0x90, 0x7C, 0x00, 0x4C, 0xD0, 0x3C, 0x00, 0x50,
                    0x48, 0x00, 0x00, 0x14 };
                PS3.SetMemory(Addresses.R_SetFrameFog_a, new byte[] { 0x41 });
                PS3.SetMemory(Addresses.R_SetFrameFog_a + 4, WritePPC);
                PS3.SetMemory(Addresses.R_SetFrameFog_a, new byte[] { 0x40 });
                Thread.Sleep(10);
                DestroyAll();
                return PS3.Extension.ReadByte(0x27720C) == 0x3F;
            }
            else if (enabled == 0x3F)
            {
                return true;
            }
            return false;
        }

        public void DestroyAll()
        {
            byte[] clear = new byte[0xB4 * 1024];
            PS3.SetMemory(0xF0E10C, clear);
        }

        public int Call(uint address, params object[] parameters)
        {
            int length = parameters.Length;
            int index = 0;
            uint count = 0;
            uint Strings = 0;
            uint Single = 0;
            uint Array = 0;
            while (index < length)
            {
                if (parameters[index] is bool boolean)
                {
                    PS3.Extension.WriteBool(0x10050000 + (count * 4), boolean);
                    count++;
                }
                else if (parameters[index] is int @int)
                {
                    PS3.Extension.WriteInt32(0x10050000 + (count * 4), @int);
                    count++;
                }
                else if (parameters[index] is uint @uint)
                {
                    PS3.Extension.WriteUInt32(0x10050000 + (count * 4), @uint);
                    count++;
                }
                else if (parameters[index] is short @short)
                {
                    PS3.Extension.WriteInt16(0x10050000 + (count * 4), @short);
                    count++;
                }
                else if (parameters[index] is ushort @ushort)
                {
                    PS3.Extension.WriteUInt16(0x10050000 + (count * 4), @ushort);
                    count++;
                }
                else if (parameters[index] is byte @byte)
                {
                    PS3.Extension.WriteByte(0x10050000 + (count * 4), @byte);
                    count++;
                }
                else
                {
                    uint pointer;
                    if (parameters[index] is string)
                    {
                        pointer = 0x10052000 + (Strings * 0x400);
                        PS3.Extension.WriteString(pointer, Convert.ToString(parameters[index]));
                        PS3.Extension.WriteUInt32(0x10050000 + (count * 4), pointer);
                        count++;
                        Strings++;
                    }
                    else if (parameters[index] is float single)
                    {
                        PS3.Extension.WriteFloat(0x10050024 + (Single * 4), single);
                        Single++;
                    }
                    else if (parameters[index] is float[] args)
                    {
                        pointer = 0x10051000 + Array * 4;
                        PS3.Extension.WriteFloats(pointer, args);
                        PS3.Extension.WriteUInt32(0x10050000 + count * 4, pointer);
                        count++;
                        Array += (uint)args.Length;
                    }

                }
                index++;
            }
            PS3.Extension.WriteUInt32(0x10050048, address);
            Thread.Sleep(20);
            return PS3.Extension.ReadInt32(0x1005004C);
        }

        public void CBuf_AddText(int clientIndex, string command)
        {
            Call(Addresses.CBuf_AddText_a, clientIndex, command);
            Thread.Sleep(20);
        }

        public void CBuf_AddText(string command)
        {
            Call(Addresses.CBuf_AddText_a, 0, command);
            Thread.Sleep(20);
        }
    }
}