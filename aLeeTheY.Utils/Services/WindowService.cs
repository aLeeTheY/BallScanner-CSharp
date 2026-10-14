using System.Windows;

namespace aLeeTheY.Utils.Services
{
    //interface IWindowService
    //{
    //    static void ShowWindow(object dataContext);
    //}

    public class WindowService
    {
        public static void ShowWindow(object viewModel)
        {
            var win = new Window();
            win.Content = viewModel;
            win.Show();
        }
    }
}
