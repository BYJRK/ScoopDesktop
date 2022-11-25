using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace ScoopDesktop
{
    /// <summary>
    /// 根据 ViewModel 找到对应的 View
    /// </summary>
    public class ViewLocator
    {
        /// <summary>
        /// 根据 ViewModel 的名字，找到对应的 View 并实例化
        /// </summary>
        public FrameworkElement? Build(INotifyPropertyChanged viewModel)
        {
            var viewModelType = viewModel.GetType();

            var name = viewModelType.FullName!
                .Replace("ViewModels.", "Pages.")
                .Replace("ViewModel", "View");
            var type = Type.GetType(name);
            if (type != null)
                return Activator.CreateInstance(type) as FrameworkElement;

            // 如果在指定命名空间中找不到，则在整个程序集中进行寻找
            type = Assembly.GetAssembly(typeof(ViewLocator))!
                .GetTypes()
                .FirstOrDefault(
                    t =>
                        t == typeof(FrameworkElement)
                        && t.Name == viewModelType.Name.Replace("ViewModel", "View")
                );
            if (type != null)
                return Activator.CreateInstance(type) as FrameworkElement;

            return null;
        }
    }
}
