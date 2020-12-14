using System.Windows.Controls;

namespace Mw3_Fix_Online.Views
{
    /// <summary>
    /// Logique d'interaction pour MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            Resources = Mw3_Fix_Online.Resources.Language.SetLanguageDictionary();
            InitializeComponent();
        }
    }
}
