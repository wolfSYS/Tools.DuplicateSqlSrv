using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace Tools.DuplicateSqlSrv.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "DuplicateSqlSrv";

        [ObservableProperty]
        private ObservableCollection<object> _menuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Duplicate",
                Icon = new SymbolIcon { Symbol = SymbolRegular.ServerSurfaceMultiple16 },
                TargetPageType = typeof(Views.Pages.DashboardPage)
            },
            new NavigationViewItem()
            {
                Content = "DB Logons",
                Icon = new SymbolIcon { Symbol = SymbolRegular.WindowDatabase24 },
                TargetPageType = typeof(Views.Pages.DataPage)
            },
            // adding this page & setting Visibility.Collapsed as a WORKAROUND for displaying the title
            // when navigated manually to this page
			new NavigationViewItem()
			{
				Content = "Edit Logon",
				Icon = new SymbolIcon { Symbol = SymbolRegular.ServerSurface16 },
				TargetPageType = typeof(Views.Pages.SingleLogonPage),
                Visibility = Visibility.Collapsed
			}
		};

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };
    }
}
