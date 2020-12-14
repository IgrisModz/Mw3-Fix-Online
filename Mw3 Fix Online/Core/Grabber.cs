using IgrisLib;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Mw3_Fix_Online.Core
{
    public class Grabber
    {
        public PS3API PS3 { get; }

        public Functions Functions { get; }

        internal Extension Extension { get; }

        public Grabber(PS3API ps3)
        {
            PS3 = ps3;
            Extension = new Extension((int)Addresses.GrabberLength);
        }

        #region Grabber
        public static uint NHG_Client(int party, Addresses.Grabber offset)
        {
            return Addresses.GrabberEntry[party] + (uint)offset;
        }

        public ObservableCollection<Player> GetPlayers(int party)
        {
            string name, xuid;
            uint interval;
            ObservableCollection<Player> clients = new ObservableCollection<Player>();
            PS3.GetMemory(Addresses.GrabberEntry[party], Extension.Dump);
            for (uint i = 0; i < Functions.MaxClients; i++)
            {
                interval = Addresses.GrabberInterval * i;
                name = Extension.ReadString((uint)Addresses.Grabber.PlaystationName2 + interval);
                xuid = ReadXUID((uint)Addresses.Grabber.XUID + interval);
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name) && Regex.IsMatch(name, @"^[a-zA-Z]{1}([\w\-]){3,15}((\([1-3]\))?)$") &&
                    !string.IsNullOrEmpty(xuid) && !string.IsNullOrWhiteSpace(xuid) && Regex.IsMatch(xuid, @"^[a-fA-F0-9]{16}$"))
                {
                    clients.Add(new Player()
                    {
                        Id = i,
                        Name = name,
                        ExternalIp = ReadIPAddress((uint)Addresses.Grabber.ExternalIP + interval),
                        XUID = xuid,
                        ZipCode = Extension.ReadString((uint)Addresses.Grabber.ZIPCode + interval)
                    });
                }
            }
            return clients;
        }

        public int ReadPrestige(uint offset)
        {
            if (offset == 0)
                return 0;
            byte[] result = Extension.ReadBytes(offset, 4);
            return BitConverter.IsLittleEndian ? BitConverter.ToInt32(result.Reverse().ToArray(), 0) : BitConverter.ToInt32(result, 0);
        }

        public int ReadLevel(uint offset)
        {
            if (offset == 0)
                return 0;
            return Convert.ToInt32(Extension.ReadUInt32(offset) + 1);
        }

        public string ReadIPAddress(uint offset)
        {
            byte[] ipBytes = Extension.ReadBytes(offset, 4);
            return new IPAddress(ipBytes).ToString();
        }

        public ushort ReadPort(uint offset)
        {
            if (offset == 0)
                return 0;
            byte[] result = Extension.ReadBytes(offset, 2);
            return (ushort)(result[0] << 8 | result[1]);
        }

        public string ReadElite(uint offset)
        {
            switch (Extension.ReadByte(offset))
            {
                case 0:
                    return "Normal";
                case 1:
                    return "Elite";
                default:
                    return "Unknown";
            }
        }

        public string ReadNat(uint offset)
        {
            if (offset == 0)
                return string.Empty;
            int result = Extension.ReadInt32(offset);
            switch (result)
            {
                case 0x0: return "Unknown";
                case 0x1: return "Open";
                case 0x2: return "Moderate";
                case 0x3: return "Strict";
                default: return string.Empty;
            }
        }

        public string ReadSpeed(uint offset)
        {
            if (offset == 0)
                return string.Empty;
            byte[] result = Extension.ReadBytes(offset, 4);
            string Speed = (Math.Truncate((BitConverter.IsLittleEndian ? Convert.ToSingle(BitConverter.ToInt32(result.Reverse().ToArray(), 0)) : Convert.ToSingle(BitConverter.ToInt32(result, 0))) / 1000)).ToString();
            return Speed.Contains("-") || Speed.Equals("0") ? string.Empty : $"{Speed}Kbps";
        }

        public string ReadXUID(uint offset)
        {
            if (offset == 0)
                return string.Empty;
            string xuid = BitConverter.ToString(Extension.ReadBytes(offset, 8)).Replace("-", string.Empty);
            switch (xuid)
            {
                case "0000000000000000": return string.Empty;
                default: return xuid;
            }
        }

        public string ReadMicro(uint offset)
        {
            if (offset == 0)
                return string.Empty;
            byte result = Extension.ReadByte(offset);
            switch (result)
            {
                case 0x0: return "Off";
                case 0x1: return "On";
                default: return string.Empty;
            }
        }

        public string ReadTitleBackgroundType(uint offset)
        {
            if (offset == 0)
                return string.Empty;
            byte result = Extension.ReadByte(offset);
            switch (result)
            {
                case 0x0: return "Challenge";
                case 0x1: return "Weapon";
                case 0x2: return "Checkerboard";
                default: return string.Empty;
            }
        }
        #endregion
    }
}
