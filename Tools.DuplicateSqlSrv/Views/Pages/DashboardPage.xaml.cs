﻿using Tools.DuplicateSqlSrv.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace Tools.DuplicateSqlSrv.Views.Pages
{
    public partial class DashboardPage : INavigableView<DashboardViewModel>
    {
        public DashboardViewModel ViewModel { get; }

        public DashboardPage(DashboardViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
