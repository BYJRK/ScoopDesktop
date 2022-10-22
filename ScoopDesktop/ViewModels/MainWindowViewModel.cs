using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScoopDesktop.Pages;
using System;
using System.Windows.Controls;

namespace ScoopDesktop.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    #region Observable Properties

    [ObservableProperty]
    Page currentPage;

    [ObservableProperty]
    bool isCommandBarVisible;

    #endregion

    AppsViewModel appsViewModel;
    SettingsViewModel settingsViewModel;

    public MainWindowViewModel()
    {
        appsViewModel = new AppsViewModel();
        settingsViewModel = new SettingsViewModel();

        NavigateTo("AppList");
    }

    #region Relay Commands

    [RelayCommand]
    void NavigationView(ModernWpf.Controls.NavigationViewItemInvokedEventArgs e)
    {
        NavigateTo((string)e.InvokedItemContainer.Tag);
    }

    #endregion

    private void NavigateTo(string header)
    {
        if (header != null)
        {
            switch (header)
            {
                case "AppList":
                    CurrentPage = new AppsView { DataContext = appsViewModel };
                    IsCommandBarVisible = appsViewModel.IsCommandBarVisible;
                    break;
                case "BucketList":
                    throw new NotImplementedException(header);
                    CurrentPage = new BucketsView();
                    break;
                case "Setting":
                    CurrentPage = new SettingsView { DataContext = settingsViewModel };
                    IsCommandBarVisible = settingsViewModel.IsCommandBarVisible;
                    break;
            }
        }
    }
}
