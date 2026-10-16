using aLeeTheY.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BallScanner.MVVM.Views.Main
{
    public partial class SettingsV : UserControl
    {
        private DataTemplate DefaultState;
        private DataTemplate MinState;

        public SettingsV()
        {
            InitializeComponent();

            DefaultState = FindResource("DefaultState") as DataTemplate;
            MinState = FindResource("MinState") as DataTemplate;

            Loaded += SettingsV_Loaded;
        }

        private void SettingsV_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyRadioState();
        }

        private void Border_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DefaultState == null || MinState == null) return;

            if (e.NewSize.Width < 1007)
            {
                if (MyContentControl.ContentTemplate == MinState) return;
                MyContentControl.ContentTemplate = MinState;
            }
            else
            {
                if (MyContentControl.ContentTemplate == DefaultState) return;
                MyContentControl.ContentTemplate = DefaultState;
            }

            // После переключения шаблона — применить состояние кнопок
            ApplyRadioState();
        }

        private void ApplyRadioState()
        {
            // Отложить до момента, когда WPF построит визуальное дерево шаблона
            Dispatcher.BeginInvoke(new Action(() =>
            {
                bool isDark = Properties.Settings.Default.IsDarkTheme;
                bool isRussian = Properties.Settings.Default.Language.Name == "ru-RU";

                foreach (var rb in FindVisualChildren<SimpleRadioButton>(this))
                {
                    switch (rb.Name)
                    {
                        case "Button_DarkTheme":
                        case "Button_DarkTheme_Min":
                            rb.IsChecked = isDark;
                            break;
                        case "Button_LightTheme":
                        case "Button_LightTheme_Min":
                            rb.IsChecked = !isDark;
                            break;
                        case "Button_LanguageRussian":
                        case "Button_LanguageRussian_Min":
                            rb.IsChecked = isRussian;
                            break;
                        case "Button_LanguageEnglish":
                        case "Button_LanguageEnglish_Min":
                            rb.IsChecked = !isRussian;
                            break;
                    }
                }
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null)
                yield break;

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typed)
                    yield return typed;

                foreach (var descendant in FindVisualChildren<T>(child))
                    yield return descendant;
            }
        }

        private void Button_DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.IsDarkTheme) return;

            var app = (App)Application.Current;
            switch (Properties.Settings.Default.SelectedPage)
            {
                case 0:
                    app.ChangeTheme(new Uri("Resources/Palettes/Dark/Pink.xaml", UriKind.Relative));
                    break;
                case 1:
                    app.ChangeTheme(new Uri("Resources/Palettes/Dark/Red.xaml", UriKind.Relative));
                    break;
                case 2:
                    app.ChangeTheme(new Uri("Resources/Palettes/Dark/Orange.xaml", UriKind.Relative));
                    break;
                case 3:
                    app.ChangeTheme(new Uri("Resources/Palettes/Dark/Yellow.xaml", UriKind.Relative));
                    break;
                case 4:
                    app.ChangeTheme(new Uri("Resources/Palettes/Dark/Green.xaml", UriKind.Relative));
                    break;
                case 5:
                    app.ChangeTheme(new Uri("Resources/Palettes/Dark/Blue.xaml", UriKind.Relative));
                    break;
                case 6:
                    app.ChangeTheme(new Uri("Resources/Palettes/Dark/Purple.xaml", UriKind.Relative));
                    break;
            }

            Properties.Settings.Default.IsDarkTheme = true;
            Properties.Settings.Default.Save();

            App.WriteMsg2Log("Переключение темы приложения на \"Тёмная\"", LoggerTypes.INFO);
        }

        private void Button_LightTheme_Click(object sender, RoutedEventArgs e)
        {
            if (!Properties.Settings.Default.IsDarkTheme) return;

            var app = (App)Application.Current;
            switch (Properties.Settings.Default.SelectedPage)
            {
                case 0:
                    app.ChangeTheme(new Uri("Resources/Palettes/Light/Pink.xaml", UriKind.Relative));
                    break;
                case 1:
                    app.ChangeTheme(new Uri("Resources/Palettes/Light/Red.xaml", UriKind.Relative));
                    break;
                case 2:
                    app.ChangeTheme(new Uri("Resources/Palettes/Light/Orange.xaml", UriKind.Relative));
                    break;
                case 3:
                    app.ChangeTheme(new Uri("Resources/Palettes/Light/Yellow.xaml", UriKind.Relative));
                    break;
                case 4:
                    app.ChangeTheme(new Uri("Resources/Palettes/Light/Green.xaml", UriKind.Relative));
                    break;
                case 5:
                    app.ChangeTheme(new Uri("Resources/Palettes/Light/Blue.xaml", UriKind.Relative));
                    break;
                case 6:
                    app.ChangeTheme(new Uri("Resources/Palettes/Light/Purple.xaml", UriKind.Relative));
                    break;
            }

            Properties.Settings.Default.IsDarkTheme = false;
            Properties.Settings.Default.Save();

            App.WriteMsg2Log("Переключение темы приложения на \"Светлая\"", LoggerTypes.INFO);
        }

        private void Button_LanguageRussian_Click(object sender, RoutedEventArgs e)
        {
            App.Language = new System.Globalization.CultureInfo("ru-RU");
            App.WriteMsg2Log("Переключение языка приложения на \"Русский, Россия\"", LoggerTypes.INFO);
        }

        private void Button_LanguageEnglish_Click(object sender, RoutedEventArgs e)
        {
            App.Language = new System.Globalization.CultureInfo("en-US");
            App.WriteMsg2Log("Переключение языка приложения на \"Английский, США\"", LoggerTypes.INFO);
        }
    }
}
