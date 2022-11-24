namespace ScoopDesktop.ViewModels;

public partial class AboutViewModel : PageViewModelBase
{
    public string GitHubRepoUrl { get; } = @"https://github.com/BYJRK/ScoopDesktop";

    public AboutViewModel()
    {
        IsCommandBarVisible = false;
    }
}
