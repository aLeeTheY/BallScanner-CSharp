using BallScanner.MVVM.Base;
using System;
using System.Windows;

namespace BallScanner.MVVM.ViewModels.Main
{
    public class AccountVM : PageVM
    {
        public string Login
        {
            get => App.CurrentUser?._username ?? "-";
        }

        public string Surname
        {
            get => App.CurrentUser?._surname ?? "-";
        }

        public string Name
        {
            get => App.CurrentUser?._name ?? "-";
        }

        public string Lastname
        {
            get => App.CurrentUser?._lastname ?? "-";
        }

        public int Smena_Number
        {
            get => App.CurrentUser?._smena_number ?? 0;
        }

        public string Access_Level
        {
            get
            {
                // superuser
                if (App.CurrentUser == null)
                    return App.L("LangRes_AccessLevel_NotAuthorized");

                switch (App.CurrentUser._access_level)
                {
                    case 0: return App.L("LangRes_AccessLevel_User");  // user
                    case 1: return App.L("LangRes_AccessLevel_Admin"); // admin
                                                                       //case 2: return "Суперпользователь";  // superuser
                }

                return App.L("LangRes_AccessLevel_Unknown");
            }
        }

        public override void ChangePalette()
        {
            var app = (App)Application.Current;

            if (Properties.Settings.Default.IsDarkTheme)
                app.ChangeTheme(new Uri("Resources/Palettes/Dark/Red.xaml", UriKind.Relative));
            else
                app.ChangeTheme(new Uri("Resources/Palettes/Light/Red.xaml", UriKind.Relative));

            Properties.Settings.Default.SelectedPage = 1;
            Properties.Settings.Default.Save();
        }
    }
}
