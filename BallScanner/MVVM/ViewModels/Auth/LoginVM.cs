using BallScanner.MVVM.Base;
using BallScanner.MVVM.Commands;
using System.Windows;
using aLeeTheY.Utils.Services;
using System.Linq;
using BallScanner.Data;
using BallScanner.Data.Tables;
using System;
using NLog;

namespace BallScanner.MVVM.ViewModels.Auth
{
    public class LoginVM : BaseViewModel
    {
        //private static readonly Logger Log = LogManager.GetLogger("Неавторизованный пользователь");

        private string _login;
        public string Login
        {
            get => _login;
            set
            {
                if (_login == value) return;

                _login = value;
                OnPropertyChanged(nameof(Login));

                //Log.Info("Изменение данных в поле \"Логин\"");
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (_password == value) return;

                _password = value;
                OnPropertyChanged(nameof(Password));

                //Log.Info("Изменение данных в поле \"Пароль\"");
            }
        }

        public RelayCommand AuthUser_Command { get; set; }

        public LoginVM()
        {
            AuthUser_Command = new RelayCommand(Authenticate);
        }

        private void Authenticate(object param)
        {
            Logger Log = LogManager.GetLogger("Неавторизованный пользователь");

            if ((Login == null || Login.Length == 0 || Login.Equals("")) || (Password == null || Password.Length == 0 || Password.Equals("")))
            {
                Log.Warn("Предупреждение пользователя! Не были указаны данные (логин или пароль) для авторизации!");
                MessageBox.Show(App.L("LangRes_Msg_EnterCredentials"), App.L("LangRes_Msg_TitleWarning"), MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return;
            }

            // superuser rights
            if (Login == "admin" && SHAService.ComputeSha256Hash(Password) == "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918")
            {
                try
                {
                    AppDbContext dbContext = AppDbContext.GetInstance();

                    // Ищем суперюзера в БД
                    User superuser = dbContext.Users
                        .FirstOrDefault(u => u._username == "admin");

                    // Если нет — создаём
                    if (superuser == null)
                    {
                        superuser = new User(
                            surname: "Silverhand",
                            name: "Johnny",
                            lastname: "-",
                            smena_number: 0,
                            username: "admin",
                            password_hash: "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918"
                        );

                        // User-конструктор ставит _access_level = 0, а нам нужен админ
                        superuser._access_level = 1;

                        dbContext.Users.Add(superuser);
                        dbContext.SaveChanges();

                        Log.Info("Создан суперпользователь admin в БД");
                    }

                    // ВАЖНО: ставим текущего юзера, чтобы _user_id для Reports был валидным
                    App.CurrentUser = superuser;

                    // open work window
                    Window old = Application.Current.MainWindow;
                    Window future = new Views.Main.RootV();
                    Application.Current.MainWindow = future;

                    // Clear fields
                    Login = Password = null;

                    future.Show();
                    old.Close();

                    return;

                }
                catch (Exception ex)
                {
                    Log.Fatal("Ошибка при создании/поиске суперюзера: " + App.GetFullExceptionMessage(ex));
                    MessageBox.Show(App.L("LangRes_Msg_ErrorPrefix") + App.GetFullExceptionMessage(ex), App.L("LangRes_Msg_TitleUnexpected"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    return;
                }
            }
            else
            {
                string username = Login;
                string password_hash = SHAService.ComputeSha256Hash(Password);

                try
                {
                    AppDbContext dbContext = AppDbContext.GetInstance();
                    //dbContext.Users.Load();

                    User user = dbContext.Users
                        .Where(u => u._username.Equals(username) && u._password_hash.Equals(password_hash) && u._is_active == 1)
                        .FirstOrDefault();

                    if (user == null)
                    {
                        Log.Error("Ошибка авторизации! Неправильный логин или пароль!");
                        MessageBox.Show(App.L("LangRes_Msg_InvalidCredentials"), App.L("LangRes_Msg_TitleError"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                        return;
                    }

                    App.CurrentUser = user;
                    //Console.WriteLine(user._surname);

                    // open work window
                    Window old = Application.Current.MainWindow;
                    Window future = new Views.Main.RootV();

                    Application.Current.MainWindow = future;

                    // Clear fields
                    Login = Password = null;

                    future.Show();
                    old.Close();
                }
                catch (Exception ex)
                {
                    Log.Fatal("Непредвиденная ошибка во время выполнения! Текст ошибки: " + App.GetFullExceptionMessage(ex));
                    MessageBox.Show(App.L("LangRes_Msg_ErrorPrefix") + App.GetFullExceptionMessage(ex), App.L("LangRes_Msg_TitleUnexpected"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
            }
        }
    }
}

