using ModernWpf.Controls;
using System.Windows;
using System.Windows.Controls;

namespace ScoopDesktop
{
    public class ViewLocator
    {
        public FrameworkElement Build(ObservableObject viewModel)
        {
            var name = viewModel
                .GetType()
                .FullName!
                .Replace("ViewModels.", "Pages.")
                .Replace("ViewModel", "View");
            var type = Type.GetType(name);
            if (type != null)
            {
                var view = Activator.CreateInstance(type) as FrameworkElement;
                if (view != null)
                    return view;
            }
            return new TextBlock { Text = $"{name} Not Found." };
        }
    }
}
