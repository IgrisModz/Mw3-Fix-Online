namespace Mw3_Fix_Online.Core
{
    public static class Addresses
    {
        #region LocalInfo
        public static uint RefreshProfile_a => 0x1BBBC2B;
        public static uint RefreshProfile2_a => 0x1BBBC29;

        public static uint[] LocalName_a => new uint[]
        {
            0x731460,
            0x1BBBC2C,
            0x8A3810,
            Core.Grabber.NHG_Client(Functions.IsInParty, Grabber.PlaystationName) + GrabberInterval * Functions.MyIndex,
            Core.Grabber.NHG_Client(Functions.IsInParty, Grabber.PlaystationName2) + GrabberInterval * Functions.MyIndex
        };

        public static uint[] LocalZipcode_a => new uint[]
        {
            0x731474,
            Core.Grabber.NHG_Client(Functions.IsInParty, Grabber.ZIPCode) + GrabberInterval * Functions.MyIndex,
        };

        public static uint[] LocalXUID_a => new uint[]
        {
            0x731E30,
            0x8A37C8,
            Core.Grabber.NHG_Client(Functions.IsInParty, Grabber.XUID) + GrabberInterval * Functions.MyIndex,
            0x1864538,
            0x1BBBC50,
            0x1C12508,
            0x1C488E8,
            0x1D58C28,
            0x8A37F0,
            0x1C15968,
            0x1CF1C30
        };

        public static uint LocalXUIDString_a => 0x1BBBC58;
        #endregion LocalInfo

        #region Lobby Detection
        public static uint IsInParty_a => 0x89D29E;
        #endregion

        #region Server Info
        public static uint[] MaxClients_a => new uint[2] { 0x8AA1E4, 0x89F28C };
        #endregion

        #region Grabber
        public static uint GrabberLength => 0x1A70;

        public static uint[] GrabberEntry => new uint[2] { 0x8A80D8, 0x89D180 };

        public static uint GrabberInterval => 0x178;

        public enum Grabber
        {
            Prestige = 0x10,
            Level = 0x0C,
            ClanTagType = 0x7C,
            ClanTag = 0x70,
            EliteClanTag = 0x9C,
            PlaystationName = 0x2C,
            PlaystationName2 = 0x50,
            ExternalIP = 0x11E,
            InternalIP = 0x10C,
            Port = 0x122,
            NatType = 0x144,
            ZIPCode = 0x40,
            XUID = 0x148,
            PartyID = 0x04,
            Micro = 0x75,
            EmblemIndex = 0x20,
            TitleIndex = 0x24,
            TitleType = 0x7D,
            EliteTitleText = 0x7E,
            EliteTitleIndex = 0x9B,
            EliteTitleLevel = 0xA4,
            EliteTitleBackgroundType = 0x9A,
            PrestigeMW = 0xC0,
            PrestigeWAW = 0xB8,
            PrestigeMW2 = 0xB0,
            PrestigeBO = 0xA8,
            LevelMW = 0xC4,
            LevelWAW = 0xBC,
            LevelMW2 = 0xB4,
            LevelBO = 0xAC
        }
        #endregion

        #region Remote Procedure Call
        public static uint R_SetFrameFog_a => 0x277208;

        public static uint CBuf_AddText_a => 0x1DB240;
        #endregion
    }
}
