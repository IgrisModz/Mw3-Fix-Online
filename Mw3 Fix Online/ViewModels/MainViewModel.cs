using IgrisLib;
using IgrisLib.Mvvm;
using MahApps.Metro.Controls.Dialogs;
using Mw3_Fix_Online.Core;
using Mw3_Fix_Online.Properties;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Mw3_Fix_Online.ViewModels
{
    public sealed class MainViewModel : ViewModelBase
    {
        private bool backgroundFunctionsEnabled = true, backgroundFunctionsRefreshEnabled;
        private ResourceDictionary resources;
        private IDialogCoordinator dialog;

        public PS3API PS3 { get; }

        public RemoteProcedureCall RPC { get; }

        public Functions Functions { get; }

        public Grabber Grabber { get; }

        public Thread BackgroundFunctionsThread { get; private set; }

        public bool IsAttached { get => GetValue(() => IsAttached); set => SetValue(() => IsAttached, value); }

        public List<Player> SavedPlayers { get => GetValue(() => SavedPlayers); set => SetValue(() => SavedPlayers, value); }

        public Player SelectedSavedPlayer { get => GetValue(() => SelectedSavedPlayer); set => SetValue(() => SelectedSavedPlayer, value); }

        public ObservableCollection<Player> Players { get => GetValue(() => Players); set => SetValue(() => Players, value); }

        public List<object> SelectedPlayer { get => GetValue(() => SelectedPlayer); set => SetValue(() => SelectedPlayer, value); }

        public FixMethod FixMethods { get => GetValue(() => FixMethods); set => SetValue(() => FixMethods, value); }

        public DelegateCommand SetTMAPICommand { get; }

        public DelegateCommand SetCCAPICommand { get; }

        public DelegateCommand SetPS3MAPICommand { get; }

        public DelegateCommand ConnectionCommand { get; }

        public DelegateCommand RefreshSavedPlayerCommand { get; }

        public DelegateCommand DeleteSavedPlayerCommand { get; }

        public DelegateCommand<bool> FixAccountCommand { get; }

        public DelegateCommand RefreshPlayerCommand { get; }

        public DelegateCommand AddSelectedPlayerCommand { get; }

        public DelegateCommand<bool> FixWithSelectedPlayerCommand { get; }

        public MainViewModel()
        {
            resources = Resources.Language.SetLanguageDictionary();
            dialog = DialogCoordinator.Instance;
            switch (Settings.Default.API)
            {
                case "TMAPI":
                    PS3 = new PS3API(new TMAPI());
                    break;
                case "CCAPI":
                    PS3 = new PS3API(new CCAPI());
                    break;
                case "PS3MAPI":
                    PS3 = new PS3API(new PS3MAPI());
                    break;
                default:
                    PS3 = new PS3API(new TMAPI());
                    break;
            }
            RPC = new RemoteProcedureCall(PS3);
            Grabber = new Grabber(PS3);
            Functions = new Functions(PS3, RPC, Grabber);
            Grabber = new Grabber(PS3);
            SetTMAPICommand = new DelegateCommand(SetTMAPI, CanExecuteSetTMAPI);
            SetCCAPICommand = new DelegateCommand(SetCCAPI, CanExecuteSetCCAPI);
            SetPS3MAPICommand = new DelegateCommand(SetPS3MAPI, CanExecuteSetPS3MAPI);
            ConnectionCommand = new DelegateCommand(Connection, CanExecuteConnection);
            RefreshSavedPlayerCommand = new DelegateCommand(RefreshSavedPlayer);
            DeleteSavedPlayerCommand = new DelegateCommand(DeleteSavedPlayer, CanExecuteDeleteSavedPlayer);
            FixAccountCommand = new DelegateCommand<bool>(keepName => FixAccount(keepName), keepName => CanExecuteAttached());
            RefreshPlayerCommand = new DelegateCommand(RefreshPlayer, CanExecuteAttached);
            AddSelectedPlayerCommand = new DelegateCommand(AddSelectedPlayer, CanExecuteAddSelectedPlayer);
            FixWithSelectedPlayerCommand = new DelegateCommand<bool>(keepName => FixWithSelectedPlayer(keepName), keepName => CanExecuteFixWithSelectedPlayer());
            RefreshSavedPlayer();
        }

        ~MainViewModel()
        {
            backgroundFunctionsEnabled = false;
            BackgroundFunctionsThread?.Abort();
            BackgroundFunctionsThread = null;
        }

        #region CanExecute
        private bool CanExecuteSetTMAPI()
        {
            return Settings.Default.API != "TMAPI";
        }

        private bool CanExecuteSetCCAPI()
        {
            return Settings.Default.API != "CCAPI";
        }

        private bool CanExecuteSetPS3MAPI()
        {
            return Settings.Default.API != "PS3MAPI";
        }

        private bool CanExecuteConnection()
        {
            return PS3 != null && PS3.GetCurrentAPI() != null;
        }

        private bool CanExecuteAttached()
        {
            return IsAttached;
        }

        private bool CanExecuteDeleteSavedPlayer()
        {
            return SelectedSavedPlayer != null;
        }

        private bool VerificationForAdd(string name, string xuid)
        {
            for (uint i = 1; i < 4; i++)
            {
                if (name.EndsWith($"({i})") && xuid.StartsWith($"0{i}") && xuid.EndsWith($"0{i}"))
                    return true;
            }
            return false;
        }

        private bool CanExecuteAddSelectedPlayer()
        {
            if (SelectedPlayer is null)
                return false;
            List<Player> players = new List<Player>();
            foreach (Player player in SelectedPlayer)
            {
                players.Add(player);
            }
            return players.Where(p => VerificationForAdd(p.Name, p.XUID) ||
                   SavedPlayers.Select(p2 => p2.Name).Contains(p.Name) &&
                   SavedPlayers.Select(p2 => p2.XUID).Contains(p.XUID)).Count() == 0;
        }

        private bool CanExecuteFixWithSelectedPlayer()
        {
            return IsAttached && SelectedPlayer?.Count() == 1;
        }
        #endregion CanExecute

        #region Api
        private void SetTMAPI()
        {
            PS3.ChangeAPI(new TMAPI());
            Settings.Default.API = "TMAPI";
            Settings.Default.Save();
        }

        private void SetCCAPI()
        {
            PS3.ChangeAPI(new CCAPI());
            Settings.Default.API = "CCAPI";
            Settings.Default.Save();
        }

        private void SetPS3MAPI()
        {
            PS3.ChangeAPI(new PS3MAPI());
            Settings.Default.API = "PS3MAPI";
            Settings.Default.Save();
        }
        #endregion Api

        private async void Connection()
        {
            backgroundFunctionsRefreshEnabled = false;
            if (PS3.ConnectTarget())
            {
                if (PS3.AttachProcess())
                {
                    if (PS3.GetCurrentGame() == "Modern Warfare® 3")
                    {
                        RPC.Enable();
                        await dialog.ShowMessageAsync(this, resources["success"].ToString(), $"{resources["successAttached"]} \"{PS3.CurrentGame}\".");
                        backgroundFunctionsRefreshEnabled = true;
                        IsAttached = true;
                        BackgroundFunctionsThread = new Thread(() => BackgroundFunctions()) { IsBackground = true };
                        BackgroundFunctionsThread?.Start();
                        return;
                    }
                    else
                    {
                        await dialog.ShowMessageAsync(this, resources["wrongProcess"].ToString(), $"{resources["thisProcess"]} \"{PS3.CurrentGame}\".");
                    }
                }
                else
                {
                    await dialog.ShowMessageAsync(this, resources["attachFailed"].ToString(), resources["unableAttach"].ToString());
                }
            }
            else
            {
                await dialog.ShowMessageAsync(this, resources["connectFailed"].ToString(), resources["unableConnect"].ToString());
            }
            IsAttached = false;
        }

        private void RefreshSavedPlayer()
        {
            PlayerLib.MakeFile();
            SavedPlayers = Functions.GetSavedPlayers();
            SelectedSavedPlayer = SavedPlayers.FirstOrDefault();
        }

        private void DeleteSavedPlayer()
        {
            if (SavedPlayers.Remove(SelectedSavedPlayer))
            {
                PlayerLib.DeleteFile();
                PlayerLib.MakeFile();
                PlayerLib.Log(SavedPlayers);
                SavedPlayers = Functions.GetSavedPlayers();
                SelectedSavedPlayer = SavedPlayers.FirstOrDefault();
            }
        }

        private void FixAccount(bool keepName)
        {
            string name, zipcode, xuid = SelectedSavedPlayer.XUID;
            if (FixMethods == FixMethod.MainPlayer)
            {
                name = SelectedSavedPlayer.Name;
                zipcode = "xxxxps3";
            }
            else
            {
                name = $"{SelectedSavedPlayer.Name}({(uint)FixMethods})";
                zipcode = "";
                xuid = xuid.Remove(0, 2).Insert(0, $"0{(uint)FixMethods}").Remove(14, 2).Insert(14, $"0{(uint)FixMethods}");
            }
            backgroundFunctionsEnabled = false;
            Functions.FixAccount(name, zipcode, xuid, keepName);
            backgroundFunctionsEnabled = true;
        }

        private void RefreshPlayer()
        {
            backgroundFunctionsEnabled = false;
            Functions.InParty();
            Functions.GetMaxClients(Functions.IsInParty);
            Players = Grabber.GetPlayers(Functions.IsInParty);
            backgroundFunctionsEnabled = true;
        }

        private void AddSelectedPlayer()
        {
            foreach (Player player in SelectedPlayer)
                PlayerLib.Log(player);
            RefreshSavedPlayer();
        }

        private void FixWithSelectedPlayer(bool keepName)
        {
            backgroundFunctionsEnabled = false;
            Player player = SelectedPlayer.FirstOrDefault() as Player;
            Functions.FixAccount(player.Name, player.ZipCode, player.XUID, keepName);
            backgroundFunctionsEnabled = true;
        }

        private void BackgroundFunctions()
        {
            PS3.Connect();
            Thread.Sleep(100);
            while (backgroundFunctionsEnabled)
            {
                if (backgroundFunctionsRefreshEnabled)
                {
                    IsAttached = PS3.GetAttached();
                    Thread.Sleep(3000);
                }
            }
        }
    }
}
