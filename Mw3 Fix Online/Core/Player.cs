using IgrisLib.Mvvm;

namespace Mw3_Fix_Online.Core
{
    public class Player : ViewModelBase
    {
        public uint Id { get; set; }

        public int Prestige { get; set; }

        public int Level { get; set; }

        public string Name { get; set; }

        public string ExternalIp { get; set; }

        public string InternalIp { get; set; }

        public ushort Port { get; set; }

        public string NatType { get; set; }

        public string Zipcode { get; set; }

        public string XUID { get; set; }

        public string PartyId { get; set; }

        public string Micro { get; set; }

        public int Emblem { get; set; }

        public int TitleIndex { get; set; }

        public string ClanTagType { get; set; }

        public string ClanTag { get; set; }

        public string EliteClanTag { get; set; }

        public string TitleType { get; set; }

        public string EliteTitleText { get; set; }

        public byte EliteTitleIndex { get; set; }

        public int EliteTitleLevel { get; set; }

        public string EliteTitleBackgroundType { get; set; }

        public int PrestigeMW { get; set; }

        public int LevelMW { get; set; }

        public int PrestigeWAW { get; set; }

        public int LevelWAW { get; set; }

        public int PrestigeMW2 { get; set; }

        public int LevelMW2 { get; set; }

        public int PrestigeBO { get; set; }

        public int LevelBO { get; set; }

        public bool IsSelected { get => GetValue(() => IsSelected); set => SetValue(() => IsSelected, value); }
    }
}
