using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Net;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Data.Sql;
using Microsoft.SqlServer.Management.Smo;
using Tools.DuplicateSqlSrv.ExtensionMethods;
using Tools.DuplicateSqlSrv.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

// https://stackoverflow.com/questions/8666256/how-to-handle-the-selectionchanged-event-of-combobox-with-mvvm-in-wpf


namespace Tools.DuplicateSqlSrv.ViewModels.Pages
{
    public partial class SingleLogonViewModel(INavigationService navigationService) : ObservableObject, INavigationAware
	{
		private bool _isInitialized = false;

		[ObservableProperty]
		private bool _isProgressVisible = true;

		[ObservableProperty]
		private List<string> _remoteSqlServers = new List<string>();

		[ObservableProperty]
		private string _selectedSqlServer = string.Empty;

		[ObservableProperty]
		private string _selectedDatabase = string.Empty;

		[ObservableProperty]
		private ObservableCollection<string> _databasesOfSelectedSqlServer = new ObservableCollection<string>();


		public void OnNavigatedTo()
		{
			if (!_isInitialized)
				InitializeViewModel();
		}

		public void OnNavigatedFrom() { }

		private void InitializeViewModel()
		{
			IsProgressVisible = true;   // TODO: from AppSettings

			GetSqlServers();

			IsProgressVisible = false;
			_isInitialized = true;
		}

		private void GetSqlServers()
		{
			RemoteSqlServers = App.AllTheSqlServersOnTheNetwork;

			if (App.AllTheSqlServersOnTheNetwork.Count > 0)
				RemoteSqlServers.Insert(0, "(select SQL Server Instance)");
			else
				RemoteSqlServers.Insert(0, "(enter SQL Server Instance)");

			SelectedSqlServer = RemoteSqlServers[0];
		}


		[RelayCommand]
		public async void SqlServerSelectionChanged()
		{
			DatabasesOfSelectedSqlServer.Clear();

			if (!_isInitialized || string.IsNullOrEmpty(SelectedSqlServer) || SelectedSqlServer.StartsWith("(select"))
				return;

			bool exists = false;
			foreach (var srv in App.AllTheSqlServersOnTheNetwork)
			{
				if (string.Equals(srv.ToLower(), SelectedSqlServer.ToLower(), StringComparison.OrdinalIgnoreCase))
				{
					exists = true;
					break;
				}
			}

			if (exists)
			{
				IsProgressVisible = true;
				OnPropertyChanged(nameof(IsProgressVisible));
				await Task.Delay(100);

				DatabasesOfSelectedSqlServer = await GetDatabasesForSqlServer();

				if (DatabasesOfSelectedSqlServer.Count > 0)
					SelectedDatabase = DatabasesOfSelectedSqlServer[0];

				await Task.Delay(100);
				IsProgressVisible = false;
				OnPropertyChanged(nameof(DatabasesOfSelectedSqlServer));
				OnPropertyChanged(nameof(IsProgressVisible));
			}
		}


		private async Task<ObservableCollection<string>> GetDatabasesForSqlServer()
		{
			return
				await Task.Run(() =>
				{
					ObservableCollection<string> dbs = new ObservableCollection<string>();
					Server sqlSrvInstance = new Server(SelectedSqlServer);

					foreach (var db in sqlSrvInstance.Databases)
					{
						string d = db.ToString();

						if (!string.IsNullOrEmpty(d))
							dbs.Add(d.Substring(1, d.Length - 2));
					}
					return dbs;
				});
		}

	}
}
