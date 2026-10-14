using aLeeTheY.Utils.Services;
using BallScanner.Data;
using BallScanner.Data.Tables;
using BallScanner.MVVM.Base;
using BallScanner.MVVM.Commands;
using BallScanner.MVVM.ViewModels.Main;
using System;
using System.Linq;
using System.Windows;

namespace BallScanner.MVVM.ViewModels.Edit
{
    public class EditUsersVM : BaseViewModel
    {
        private const int AdminAccessLevel = 1;
        private const int ActiveStatus = 1;

        private bool IsLastActiveAdmin()
        {
            if (user == null)
                return false;

            if (user._is_active != ActiveStatus || user._access_level != AdminAccessLevel)
                return false;

            var dbContext = AppDbContext.GetInstance();

            return !dbContext.Users
                .ToList()
                .Any(u =>
                    u._id != user._id &&
                    u._is_active == ActiveStatus &&
                    u._access_level == AdminAccessLevel);
        }

        private bool CanChangeAccessLevel(int newAccessLevel)
        {
            if (newAccessLevel == AdminAccessLevel)
                return true;

            if (!IsLastActiveAdmin())
                return true;

            App.WriteMsg2Log(
                "Попытка понизить последнего активного администратора. " +
                "Информация о пользователе: номер = " + user._id +
                "; логин = " + user._username + ";",
                LoggerTypes.WARN);

            MessageBox.Show(
                App.L("LangRes_Msg_CannotDowngradeLastAdmin"),
                App.L("LangRes_Msg_TitleWarning"),
                MessageBoxButton.OK,
                MessageBoxImage.Warning,
                MessageBoxResult.OK);

            return false;
        }

        private bool CanChangeActiveStatus(int newActiveStatus)
        {
            if (newActiveStatus == ActiveStatus)
                return true;

            if (!IsLastActiveAdmin())
                return true;

            App.WriteMsg2Log(
                "Попытка деактивировать последнего активного администратора. " +
                "Информация о пользователе: номер = " + user._id +
                "; логин = " + user._username + ";",
                LoggerTypes.WARN);

            MessageBox.Show(
                App.L("LangRes_Msg_CannotDeactivateLastAdmin"),
                App.L("LangRes_Msg_TitleWarning"),
                MessageBoxButton.OK,
                MessageBoxImage.Warning,
                MessageBoxResult.OK);

            return false;
        }

        public RelayCommand DeleteReportCommand { get; set; }
        public RelayCommand ChangePasswordCommand { get; set; }

        private Window currDialogWindow;
        private User user;

        public string Title_Text
        {
            get => App.L("LangRes_EditUsersWindow_Title") + user._id;
        }

        // Fields
        public string Surname
        {
            get => user._surname;
            set
            {
                if (user._surname == value) return;

                user._surname = value;
                OnPropertyChanged(nameof(Surname));

                RefreshDataGrid();
            }
        }

        public string Name
        {
            get => user._name;
            set
            {
                if (user._name == value) return;

                user._name = value;
                OnPropertyChanged(nameof(Name));

                RefreshDataGrid();
            }
        }

        public string Lastname
        {
            get => user._lastname;
            set
            {
                if (user._lastname == value) return;

                user._lastname = value;
                OnPropertyChanged(nameof(Lastname));

                RefreshDataGrid();
            }
        }

        public string Login
        {
            get => user._username;
            set
            {
                if (user._username == value) return;

                user._username = value;
                OnPropertyChanged(nameof(Login));

                RefreshDataGrid();
            }
        }

        public int Smena_Number
        {
            get => user._smena_number;
            set
            {
                if (user._smena_number == value) return;

                user._smena_number = value;
                OnPropertyChanged(nameof(Smena_Number));

                RefreshDataGrid();
            }
        }

        public int Access_Level
        {
            get => user._access_level;
            set
            {
                if (user._access_level == value) return;

                if (!CanChangeAccessLevel(value))
                {
                    int oldValue = user._access_level;

                    // 1) Делаем вид, что приняли значение — ComboBox успокаивается
                    user._access_level = value;
                    OnPropertyChanged(nameof(Access_Level));

                    // 2) На следующем проходе Dispatcher'а возвращаем старое и уведомляем
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        user._access_level = oldValue;
                        OnPropertyChanged(nameof(Access_Level));
                    }), System.Windows.Threading.DispatcherPriority.Background);

                    return;
                }

                user._access_level = value;
                OnPropertyChanged(nameof(Access_Level));

                RefreshDataGrid();
            }
        }

        public int Is_Active
        {
            get => user._is_active;
            set
            {
                if (user._is_active == value) return;

                if (!CanChangeActiveStatus(value))
                {
                    int oldValue = user._is_active;

                    // 1) Делаем вид, что приняли значение — ComboBox успокаивается
                    user._is_active = value;
                    OnPropertyChanged(nameof(Is_Active));

                    // 2) На следующем проходе Dispatcher'а возвращаем старое и уведомляем
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        user._is_active = oldValue;
                        OnPropertyChanged(nameof(Is_Active));
                    }), System.Windows.Threading.DispatcherPriority.Background);

                    return;
                }

                user._is_active = value;
                OnPropertyChanged(nameof(Is_Active));

                RefreshDataGrid();
            }
        }

        public string Password
        {
            get => user._password_hash;
            set
            {
                if (user._password_hash == value) return;

                user._password_hash = value;
                OnPropertyChanged(nameof(Password));

                RefreshDataGrid();
            }
        }

        private string _newPassword;
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                if (_newPassword == value) return;

                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));
            }
        }

        private string _newPassword_check;
        public string NewPassword_Check
        {
            get => _newPassword_check;
            set
            {
                if (_newPassword_check == value) return;

                _newPassword_check = value;
                OnPropertyChanged(nameof(NewPassword_Check));
            }
        }

        public EditUsersVM()
        {
            DeleteReportCommand = new RelayCommand(Delete_User);
            ChangePasswordCommand = new RelayCommand(ChangePassword);
        }

        public EditUsersVM(User user, Window currDialogWindow)
        {
            DeleteReportCommand = new RelayCommand(Delete_User);
            ChangePasswordCommand = new RelayCommand(ChangePassword);

            this.user = user;
            this.currDialogWindow = currDialogWindow;
        }

        private void RefreshDataGrid()
        {
            if (AccountManagmentVM.RefreshDataGrid.CanExecute(null))
                AccountManagmentVM.RefreshDataGrid.Execute(null);

            if (DocumentsVM.RefreshDataGrid.CanExecute(null))
                DocumentsVM.RefreshDataGrid.Execute(null);
        }

        private void Delete_User(object param)
        {
            if (user == App.CurrentUser)
            {
                App.WriteMsg2Log("Попытка удалить авторизованного пользователя (самого себя)!", LoggerTypes.ERROR);
                MessageBox.Show(App.L("LangRes_Msg_CannotDeleteSelf"), App.L("LangRes_Msg_TitleError"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }

            if (user.Reports != null && user.Reports.Count != 0)
            {
                App.WriteMsg2Log("Попытка удалить пользователя, у которого есть отчёты в базе данных!", LoggerTypes.ERROR);
                MessageBox.Show(App.L("LangRes_Msg_CannotDeleteUserWithReports"), App.L("LangRes_Msg_TitleError"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }

            App.WriteMsg2Log("Удаление пользователя. Информация о пользователе: номер = " + user._id + "; фамилия = " + user._surname + "; имя = " + user._name + "; отчество = " + user._lastname + "; логин = " + user._username + "; номер смены = " + user._smena_number + "; уровень доступа = " + user.Access_Level + "; статус аккаунта = " + user.Is_Active + ";", LoggerTypes.INFO);
            try
            {

                AppDbContext dbContext = AppDbContext.GetInstance();

                dbContext.Users.Remove(user);
                dbContext.SaveChanges();

                currDialogWindow.Close();
                RefreshDataGrid();
            }
            catch (Exception ex)
            {
                App.WriteMsg2Log("Непредвиденная ошибка во время выполнения! Текст ошибки: " + App.GetFullExceptionMessage(ex), LoggerTypes.FATAL);
                MessageBox.Show(App.L("LangRes_Msg_ErrorPrefix") + App.GetFullExceptionMessage(ex), App.L("LangRes_Msg_TitleUnexpected"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void ChangePassword(object param)
        {
            // Null password
            if (NewPassword == null || NewPassword.Equals(""))
            {
                App.WriteMsg2Log("Предупреждение пользователя! Поле \"Новый пароль\" пустое!", LoggerTypes.WARN);
                MessageBox.Show(App.L("LangRes_Msg_NoNewPassword"), App.L("LangRes_Msg_TitleWarning"), MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return;
            }

            // Null password_check
            if (NewPassword_Check == null || NewPassword_Check.Equals(""))
            {
                App.WriteMsg2Log("Предупреждение пользователя! Поле \"Повторите новый пароль\" пустое!", LoggerTypes.WARN);
                MessageBox.Show(App.L("LangRes_Msg_NoNewPasswordRepeat"), App.L("LangRes_Msg_TitleWarning"), MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return;
            }

            // Bad password length or password_check length
            if (NewPassword.Length < 4 || NewPassword_Check.Length < 4)
            {
                App.WriteMsg2Log("Предупреждение пользователя! Длина пароля или повтора пароля меньше 4-х символов!", LoggerTypes.WARN);
                MessageBox.Show(App.L("LangRes_Msg_PasswordTooShort"), App.L("LangRes_Msg_TitleWarning"), MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return;
            }

            // Password equals password_check
            if (!NewPassword.Equals(NewPassword_Check))
            {
                App.WriteMsg2Log("Предупреждение пользователя! Пароли не совпадают!", LoggerTypes.WARN);
                MessageBox.Show(App.L("LangRes_Msg_PasswordsMismatch"), App.L("LangRes_Msg_TitleWarning"), MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return;
            }

            user._password_hash = SHAService.ComputeSha256Hash(NewPassword);

            App.WriteMsg2Log("Изменение пароля для пользователя. Информация о пользователе: номер = " + user._id + "; фамилия = " + user._surname + "; имя = " + user._name + "; отчество = " + user._lastname + "; логин = " + user._username + "; номер смены = " + user._smena_number + "; уровень доступа = " + user.Access_Level + "; статус аккаунта = " + user.Is_Active + ";", LoggerTypes.INFO);
            MessageBox.Show(App.L("LangRes_Msg_PasswordChanged"), App.L("LangRes_Msg_TitleInfo"), MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }
    }
}
