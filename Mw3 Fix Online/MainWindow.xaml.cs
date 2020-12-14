using MahApps.Metro.Controls;

namespace Mw3_Fix_Online
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Resources = Mw3_Fix_Online.Resources.Language.SetLanguageDictionary();
        }
    }
}
