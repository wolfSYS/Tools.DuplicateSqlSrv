using Tools.DuplicateSqlSrv.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace Tools.DuplicateSqlSrv.Views.Pages
{
    public partial class SingleLogonPage : INavigableView<SingleLogonViewModel>
    {
        public SingleLogonViewModel ViewModel { get; }

        public SingleLogonPage(SingleLogonViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
