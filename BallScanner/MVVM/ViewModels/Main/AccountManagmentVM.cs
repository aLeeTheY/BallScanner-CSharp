using BallScanner.Data;
using BallScanner.Data.Tables;
using BallScanner.MVVM.Base;
using BallScanner.MVVM.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace BallScanner.MVVM.ViewModels.Main
{
    public class AccountManagmentVM : PageVM
    {
        // DispatcherTimer работает в UI-потоке → краша с потоками не будет
        private readonly DispatcherTimer timer;

        public static RelayCommand RefreshDataGrid { get; set; }
        public RelayCommand OpenDialogWindowCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand AddUser_Command { get; set; }

        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                if (_users == value) return;

                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        private string _search_value;
        public string Search_Value
        {
            get => _search_value;
            set
            {
                if (_search_value == value) return;

                _search_value = value;
                OnPropertyChanged(nameof(Search_Value));

                timer.Stop();
                timer.Start();
            }
        }

        private int _selected_index = 0;
        public int Selected_Index
        {
            get => _selected_index;
            set
            {
                if (_selected_index == value) return;

                _selected_index = value;
                OnPropertyChanged(nameof(Selected_Index));

                timer.Stop();
                timer.Start();
            }
        }

        public AccountManagmentVM()
        {
            // Таймер в UI-потоке
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(400) };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                Search(null);
            };

            OpenDialogWindowCommand = new RelayCommand(OpenDialogWindow);
            SearchCommand = new RelayCommand(Search);
            AddUser_Command = new RelayCommand(AddUser);

            RefreshDataGrid = new RelayCommand(OnRefreshDataGrid);

            try
            {
                AppDbContext dbContext = AppDbContext.GetInstance();
                Users = new ObservableCollection<User>(dbContext.Users.ToList());
            }
            catch (Exception ex)
            {
                App.WriteMsg2Log("Непредвиденная ошибка во время выполнения! Текст ошибки: " + App.GetFullExceptionMessage(ex), LoggerTypes.FATAL);
                MessageBox.Show(App.L("LangRes_Msg_ErrorPrefix") + App.GetFullExceptionMessage(ex), App.L("LangRes_Msg_TitleUnexpected"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void OnRefreshDataGrid(object param)
        {
            // Просто переприменяем фильтр (или показываем всё)
            Search(null);
        }

        private void Search(object param)
        {
            try
            {
                AppDbContext dbContext = AppDbContext.GetInstance();

                // Один запрос, дальше фильтрация в памяти — убирает N+1 и фризы
                var allUsers = dbContext.Users.ToList();

                if (string.IsNullOrEmpty(Search_Value))
                {
                    Users = new ObservableCollection<User>(allUsers);
                    return;
                }

                string search = Search_Value.Trim();
                List<User> search_result = null;

                switch (Selected_Index)
                {
                    case 0: // Логин
                        App.WriteMsg2Log("Поиск пользователей на странице \"Администрирование\" по параметру \"Логин\"", LoggerTypes.INFO);
                        search_result = allUsers
                            .Where(u => string.Equals(u._username, search, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;

                    case 1: // Фамилия
                        App.WriteMsg2Log("Поиск пользователей на странице \"Администрирование\" по параметру \"Фамилия\"", LoggerTypes.INFO);
                        search_result = allUsers
                            .Where(u => string.Equals(u._surname, search, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;

                    case 2: // Имя
                        App.WriteMsg2Log("Поиск пользователей на странице \"Администрирование\" по параметру \"Имя\"", LoggerTypes.INFO);
                        search_result = allUsers
                            .Where(u => string.Equals(u._name, search, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;

                    case 3: // Отчество
                        App.WriteMsg2Log("Поиск пользователей на странице \"Администрирование\" по параметру \"Отчество\"", LoggerTypes.INFO);
                        search_result = allUsers
                            .Where(u => string.Equals(u._lastname, search, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;

                    case 4: // Номер смены
                        App.WriteMsg2Log("Поиск пользователей на странице \"Администрирование\" по параметру \"Номер смены\"", LoggerTypes.INFO);
                        search_result = allUsers
                            .Where(u => u._smena_number.ToString() == search)
                            .ToList();
                        break;

                    case 5: // Уровень доступа
                        App.WriteMsg2Log("Поиск пользователей на странице \"Администрирование\" по параметру \"Уровень доступа\"", LoggerTypes.INFO);

                        search_result = new List<User>();
                        switch (search.ToUpperInvariant())
                        {
                            case "ПОЛЬЗОВАТЕЛЬ":
                            case "USER":
                                search_result = allUsers.Where(u => u._access_level == 0).ToList();
                                break;
                            case "АДМИНИСТРАТОР":
                            case "АДМИН":
                            case "ADMINISTRATOR":
                            case "ADMIN":
                                search_result = allUsers.Where(u => u._access_level == 1).ToList();
                                break;
                        }
                        break;

                    case 6: // Статус аккаунта
                        App.WriteMsg2Log("Поиск пользователей на странице \"Администрирование\" по параметру \"Статус аккаунта\"", LoggerTypes.INFO);

                        search_result = new List<User>();
                        switch (search.ToUpperInvariant())
                        {
                            case "АКТИВЕН":
                            case "ACTIVE":
                                search_result = allUsers.Where(u => u._is_active == 1).ToList();
                                break;
                            case "НЕАКТИВЕН":
                            case "INACTIVE":
                                search_result = allUsers.Where(u => u._is_active == 0).ToList();
                                break;
                        }
                        break;
                }

                Users = new ObservableCollection<User>(search_result ?? new List<User>());
            }
            catch (Exception ex)
            {
                App.WriteMsg2Log("Непредвиденная ошибка во время выполнения! Текст ошибки: " + App.GetFullExceptionMessage(ex), LoggerTypes.FATAL);
                MessageBox.Show(App.L("LangRes_Msg_ErrorPrefix") + App.GetFullExceptionMessage(ex), App.L("LangRes_Msg_TitleUnexpected"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void OpenDialogWindow(object param)
        {
            if (param is User user)
            {
                App.WriteMsg2Log("Открытие окна редактирования пользователя. Пользователь #" + user._id + ". Страница \"Администрирование\".", LoggerTypes.INFO);

                Views.Edit.RootV editWindow = new Views.Edit.RootV(user);
                editWindow.ShowDialog();

                App.WriteMsg2Log("Закрытие окна редактирования пользователя!", LoggerTypes.INFO);

                // Обновляем список после редактирования
                OnRefreshDataGrid(null);
            }
        }

        private void AddUser(object param)
        {
            App.WriteMsg2Log("Открытие окна добавления пользователя на странице \"Администрирование\".", LoggerTypes.INFO);

            Views.Add.RootV addWindow = new Views.Add.RootV();
            addWindow.ShowDialog();

            App.WriteMsg2Log("Закрытие окна добавления пользователя!", LoggerTypes.INFO);

            // Обновляем список после добавления
            OnRefreshDataGrid(null);
        }

        public override void ChangePalette()
        {
            var app = (App)Application.Current;

            if (Properties.Settings.Default.IsDarkTheme)
                app.ChangeTheme(new Uri("Resources/Palettes/Dark/Pink.xaml", UriKind.Relative));
            else
                app.ChangeTheme(new Uri("Resources/Palettes/Light/Pink.xaml", UriKind.Relative));

            Properties.Settings.Default.SelectedPage = 0;
            Properties.Settings.Default.Save();
        }
    }
}
