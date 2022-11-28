using ScoopDesktop.Helpers;
using ScoopDesktop.Utils;
using System.Windows.Controls;

namespace ScoopDesktop.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    #region Observable Properties

    [ObservableProperty]
    Page? currentPage;

    [ObservableProperty]
    bool isCommandBarVisible;

    #endregion

    AppsViewModel appsViewModel = new();
    BucketsViewModel bucketsViewModel = new();
    SettingsViewModel settingsViewModel = new();
    AboutViewModel aboutViewModel = new();

    ViewLocator viewLocator = new();

    public MainWindowViewModel()
    {

    }

    #region Relay Commands

    [RelayCommand]
    void NavigationView(ModernWpf.Controls.NavigationViewItemInvokedEventArgs e)
    {
        NavigateTo((string)e.InvokedItemContainer.Tag);
    }

    [RelayCommand]
    async Task Loaded()
    {
        if (!Directory.Exists(ScoopHelper.ScoopRootDir))
        {
            await DialogHelper.Info("Scoop is not installed. You may type\n\n$ iwr -useb get.scoop.sh | iex\n\nin PowerShell to install.", "Error");
            ApplicationHelper.Exit();
        }

        NavigateTo(appsViewModel);
    }

    #endregion

    void NavigateTo(string header)
    {
        if (header != null)
        {
            switch (header)
            {
                case "AppList":
                    NavigateTo(appsViewModel);
                    break;
                case "BucketList":
                    NavigateTo(bucketsViewModel);
                    break;
                case "Setting":
                    NavigateTo(settingsViewModel);
                    break;
                case "About":
                    NavigateTo(aboutViewModel);
                    break;
                default:
                    throw new NotImplementedException(header);
            }
        }
    }

    void NavigateTo(PageViewModelBase viewModel)
    {
        var page = viewLocator.Build(viewModel) as Page
            ?? throw new ArgumentException(viewModel.ToString());

        CurrentPage = page;
        CurrentPage.DataContext = viewModel;
        IsCommandBarVisible = viewModel.IsCommandBarVisible;
    }
}
