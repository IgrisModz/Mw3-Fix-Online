using IgrisLib;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace Mw3_Fix_Online.Core
{
    public class Functions
    {
        public PS3API PS3 { get; }

        public RemoteProcedureCall RPC { get; }

        public Grabber Grabber { get; }

        public static int IsInParty { get; private set; }

        public static uint MaxClients { get; private set; }

        public static uint MyIndex { get; private set; }

        public Functions(PS3API ps3, RemoteProcedureCall rpc, Grabber grabber)
        {
            PS3 = ps3;
            RPC = rpc;
            Grabber = grabber;
        }

        public int InParty()
        {
            IsInParty = PS3.Extension.ReadBool(Addresses.IsInParty_a) ? 1 : 0;
            return IsInParty;
        }

        public uint GetMaxClients(int party)
        {
            MaxClients = PS3.Extension.ReadUInt32(Addresses.MaxClients_a[party]);
            return MaxClients;
        }

        public void ChangeName(string name)
        {
            foreach (uint nameA in Addresses.LocalName_a)
            {
                PS3.SetMemory(nameA, new byte[32]);
                PS3.Extension.WriteString(nameA, name);
            }
            RPC.CBuf_AddText($"name {name}");
        }

        public void ChangeZipcode(string zipcode)
        {
            foreach (uint zipcodeA in Addresses.LocalZipcode_a)
            {
                PS3.Extension.WriteString(zipcodeA, zipcode);
            }
        }

        public void ChangeXUID(string xuid)
        {
            byte[] xuidB = xuid.HexStringToByteArray();
            string xuidL = xuid.ToLower();
            foreach (uint xuidA in Addresses.LocalXUID_a)
            {
                PS3.SetMemory(xuidA, xuidB);
            }
            PS3.Extension.WriteString(Addresses.LocalXUIDString_a, xuidL);
            RPC.CBuf_AddText($"xuid {xuidL}");
        }

        public void RefreshProfile()
        {
            PS3.Extension.WriteByte(Addresses.RefreshProfile_a, 1);
            PS3.Extension.WriteByte(Addresses.RefreshProfile2_a, 1);
        }

        public void FixAccount(string name, string zipcode, string xuid, bool saveName)
        {
            InParty();
            GetMyIndex();
            string savedName = PS3.Extension.ReadString(Addresses.LocalName_a[1]);
            ChangeName(name);
            ChangeZipcode(zipcode);
            ChangeXUID(xuid);
            RefreshProfile();
            Thread.Sleep(500);
            if (saveName)
                PS3.Extension.WriteString(Addresses.LocalName_a[1], savedName);
        }

        public uint GetMyIndex()
        {
            string serverInfo = PS3.Extension.ReadString(Addresses.LocalXUIDString_a);
            foreach (Player player in Grabber.GetPlayers(IsInParty))
            {
                if (serverInfo.Contains(player.XUID))
                {
                    MyIndex = player.Id;
                    return player.Id;
                }
            }
            MyIndex = 18;
            return 18;
        }

        public List<Player> GetSavedPlayers()
        {
            List<Player> players = new List<Player>();
            foreach (string player in PlayerLib.ReadLogs())
            {
                string name = player.Split('|')[0].Replace(" ", "");
                string xuid = player.Split('|')[1].Replace(" ", "");
                string zipcode = player.Split('|')[2].Replace(" ", "");
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name) && name.Length <= 16 && Regex.IsMatch(name, @"^[a-zA-Z]{1}([\w\-]){3,15}$") &&
                    !string.IsNullOrEmpty(xuid) && !string.IsNullOrWhiteSpace(xuid) && xuid.Length <= 16 && Regex.IsMatch(xuid, @"^[a-fA-F0-9]{16}$") &&
                    xuid != "0000000000000000")
                    players.Add(new Player()
                    {
                        Name = name,
                        XUID = xuid,
                        Zipcode = zipcode
                    });
            }
            return players;
        }
    }
}
