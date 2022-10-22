using CommunityToolkit.Mvvm.ComponentModel;

namespace ScoopDesktop.ViewModels
{
    public class PageViewModelBase : ObservableObject
    {
        public bool IsCommandBarVisible { get; init; }
    }
}
