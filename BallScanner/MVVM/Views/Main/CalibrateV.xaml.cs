using BallScanner.MVVM.ViewModels.Main;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BallScanner.MVVM.Views.Main
{
    public partial class CalibrateV : UserControl
    {
        private DataTemplate DefaultState;
        private DataTemplate MinState;

        private bool isMinState;

        public CalibrateV()
        {
            InitializeComponent();

            DefaultState = FindResource("DefaultState") as DataTemplate;
            MinState = FindResource("MinState") as DataTemplate;
        }

        private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (CalibrateVM.PerformAction.CanExecute("Slider_DragStarted"))
                CalibrateVM.PerformAction.Execute("Slider_DragStarted");
        }

        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (CalibrateVM.PerformAction.CanExecute("Slider_DragCompeled"))
                CalibrateVM.PerformAction.Execute("Slider_DragCompleted");
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (isMinState)
            {
                foreach (var sv in FindVisualChildren<ScrollViewer>(this))
                {
                    if (sv.Name == "sv")
                    {
                        sv.ScrollToVerticalOffset(sv.ContentVerticalOffset - e.Delta);
                    }
                }
            }
        }

        private void Border_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DefaultState == null || MinState == null) return;

            if (e.NewSize.Width < 1007)
            {
                if (MyContentControl.ContentTemplate == MinState) return;
                MyContentControl.ContentTemplate = MinState;

                isMinState = true;
            }
            else
            {
                if (MyContentControl.ContentTemplate == DefaultState) return;
                MyContentControl.ContentTemplate = DefaultState;

                isMinState = false;
            }
        }

        // Поиск элемента в DataTemplate
        public IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    if (child != null && child is T)
                        yield return (T)child;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }
    }
}
