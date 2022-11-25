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

    AppsViewModel appsViewModel;
    SettingsViewModel settingsViewModel;
    AboutViewModel aboutViewModel;

    ViewLocator viewLocator = new();

    public MainWindowViewModel()
    {
        appsViewModel = new();
        settingsViewModel = new();
        aboutViewModel = new();

        NavigateTo(appsViewModel);
    }

    #region Relay Commands

    [RelayCommand]
    void NavigationView(ModernWpf.Controls.NavigationViewItemInvokedEventArgs e)
    {
        NavigateTo((string)e.InvokedItemContainer.Tag);
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
        CurrentPage = (Page)viewLocator.Build(viewModel);
        CurrentPage.DataContext = viewModel;
        IsCommandBarVisible = viewModel.IsCommandBarVisible;
    }
}
