using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mw3_Fix_Online.Core
{
    public class PlayerLib
    {
        public static string Link => $"{AppDomain.CurrentDomain.BaseDirectory}XUID.txt";

        public static bool CheckFile()
        {
            return File.Exists(Link);
        }

        public static void MakeFile()
        {
            switch (CheckFile())
            {
                case false: File.Create(Link).Dispose(); break;
            }
        }

        public static void DeleteFile()
        {
            switch (CheckFile())
            {
                case true: File.Delete(Link); break;
            }
        }

        public static List<string> ReadLogs()
        {
            return File.ReadAllLines(Link).ToList();
        }

        public static void Log(List<Player> players)
        {
            MakeFile();
            foreach (var player in players)
            {
                if (player.Name != "" && player.XUID != "0000000000000000")
                {
                    if (!File.ReadAllText(Link).Contains($"{player.Name} | {player.XUID} | {player.Zipcode}"))
                    {
                        File.AppendAllText(Link, $"{player.Name} | {player.XUID} | {player.Zipcode}\n");
                    }
                }
            }
        }

        public static void Log(Player player)
        {
            MakeFile();
            if (player.Name != "" && player.XUID != "0000000000000000")
            {
                if (!File.ReadAllText(Link).Contains($"{player.Name} | {player.XUID} | {player.Zipcode}"))
                {
                    File.AppendAllText(Link, $"{player.Name} | {player.XUID} | {player.Zipcode}\n");
                }
            }
        }

        public static void Log(string name, string xuid, string zipcode)
        {
            MakeFile();
            if (name != "" && xuid != "0000000000000000")
            {
                if (!File.ReadAllText(Link).Contains($"{name} | {xuid} | {zipcode}"))
                {
                    File.AppendAllText(Link, $"{name} | {xuid} | {zipcode}\n");
                }
            }
        }
    }
}
