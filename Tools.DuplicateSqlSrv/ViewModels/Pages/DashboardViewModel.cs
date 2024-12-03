using System.Windows.Navigation;
using Tools.DuplicateSqlSrv.ExtensionMethods;
using Tools.DuplicateSqlSrv.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Tools.DuplicateSqlSrv.ViewModels.Pages
{
    public partial class DashboardViewModel(INavigationService navigationService) : ObservableObject
	{
        [ObservableProperty]
        private int _counter = 0;

        [RelayCommand]
        private void OnCounterIncrement()
        {
            Counter++;


			navigationService.NavigateWithHierarchy(typeof(SingleLogonPage));
			//navigationService.GoBack(); // YES^^



			//var logonInfo = new Models.LocalDB.DbConnection();

   //         logonInfo.LogonUserPwd = "Hello World".Encrypt();
   //         string whatIsMyPwd = logonInfo.LogonUserPwd.Decrypt();

   //         string x = "";
        }
    }
}
