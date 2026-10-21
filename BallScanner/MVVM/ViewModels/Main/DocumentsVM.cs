using BallScanner.Data;
using BallScanner.Data.Tables;
using BallScanner.MVVM.Base;
using BallScanner.MVVM.Commands;
using BallScanner.MVVM.Views.Edit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace BallScanner.MVVM.ViewModels.Main
{
    public class DocumentsVM : PageVM
    {
        // DispatcherTimer срабатывает в UI-потоке — краша с потоками не будет.
        private readonly DispatcherTimer timer;

        public RelayCommand OpenDialogWindowCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }

        public static RelayCommand RefreshDataGrid { get; set; }

        private ObservableCollection<Report> _reports;
        public ObservableCollection<Report> Reports
        {
            get => _reports;
            set
            {
                if (_reports == value) return;

                _reports = value;
                OnPropertyChanged(nameof(Reports));
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

        public DocumentsVM()
        {
            // Инициализация таймера — каждое нажатие перезапускает 400-мс паузу
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(400) };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                Search(null);
            };

            OpenDialogWindowCommand = new RelayCommand(OpenDialogWindow);
            SearchCommand = new RelayCommand(Search);

            RefreshDataGrid = new RelayCommand(OnRefreshDataGrid);

            try
            {
                AppDbContext dbContext = AppDbContext.GetInstance();
                Reports = new ObservableCollection<Report>(dbContext.Reports.ToList());
            }
            catch (Exception ex)
            {
                App.WriteMsg2Log("Непредвиденная ошибка во время выполнения! Текст ошибки: " + App.GetFullExceptionMessage(ex), LoggerTypes.FATAL);
                MessageBox.Show(App.L("LangRes_Msg_ErrorPrefix") + App.GetFullExceptionMessage(ex), App.L("LangRes_Msg_TitleUnexpected"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void OnRefreshDataGrid(object param)
        {
            // Просто применяем текущий фильтр (или пустой — вернёт всё)
            Search(null);
        }

        private void Search(object param)
        {
            try
            {
                AppDbContext dbContext = AppDbContext.GetInstance();

                // Загружаем все отчёты ОДИН раз, дальше фильтруем в памяти.
                // Это убирает N+1 запросы и риск NRE от невалидных SQL.
                var allReports = dbContext.Reports.ToList();

                if (string.IsNullOrEmpty(Search_Value))
                {
                    Reports = new ObservableCollection<Report>(allReports);
                    return;
                }

                string search = Search_Value.Trim();
                List<Report> search_result = null;

                switch (Selected_Index)
                {
                    case 0: // Дата
                        App.WriteMsg2Log("Поиск отчётов на странице \"Отчёты\" по параметру \"Дата\"", LoggerTypes.INFO);

                        // Поддерживаем несколько форматов, чтобы поиск не молчал при опечатке
                        DateTime targetDate;
                        if (DateTime.TryParseExact(search, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out targetDate)
                            || DateTime.TryParse(search, CultureInfo.InvariantCulture, DateTimeStyles.None, out targetDate))
                        {
                            targetDate = targetDate.Date;
                            search_result = allReports.Where(r => r._date.Date == targetDate).ToList();
                        }
                        else
                        {
                            App.WriteMsg2Log($"Поиск по дате: не удалось распарсить \"{search}\"", LoggerTypes.INFO);
                        }
                        break;

                    case 1: // Фракция
                        App.WriteMsg2Log("Поиск отчётов на странице \"Отчёты\" по параметру \"Фракция\"", LoggerTypes.INFO);
                        search_result = allReports
                            .Where(r => string.Equals(r._fraction, search, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;

                    case 2: // Номер партии
                        App.WriteMsg2Log("Поиск отчётов на странице \"Отчёты\" по параметру \"Номер партии\"", LoggerTypes.INFO);
                        search_result = allReports
                            .Where(r => string.Equals(r._partia_number, search, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;

                    case 3: // Номер смены
                        App.WriteMsg2Log("Поиск отчётов на странице \"Отчёты\" по параметру \"Номер смены\"", LoggerTypes.INFO);
                        search_result = allReports
                            .Where(r => r.User != null && r.User._smena_number.ToString() == search)
                            .ToList();
                        break;

                    case 4: // Значение
                        App.WriteMsg2Log("Поиск отчётов на странице \"Отчёты\" по параметру \"Значение\"", LoggerTypes.INFO);
                        search_result = allReports
                            .Where(r => r._avg_black_pixels_value.ToString() == search)
                            .ToList();
                        break;

                    case 5: // Примечание
                        App.WriteMsg2Log("Поиск отчётов на странице \"Отчёты\" по параметру \"Примечание\"", LoggerTypes.INFO);
                        search_result = allReports
                            .Where(r => string.Equals(r._note, search, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                }

                // Если switch не сработал или поиск не дал результата — показываем пустую таблицу
                Reports = new ObservableCollection<Report>(search_result ?? new List<Report>());
            }
            catch (Exception ex)
            {
                App.WriteMsg2Log("Непредвиденная ошибка во время выполнения! Текст ошибки: " + App.GetFullExceptionMessage(ex), LoggerTypes.FATAL);
                MessageBox.Show(App.L("LangRes_Msg_ErrorPrefix") + App.GetFullExceptionMessage(ex), App.L("LangRes_Msg_TitleUnexpected"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void OpenDialogWindow(object param)
        {
            if (param != null)
            {
                App.WriteMsg2Log("Открытие окна редактирования отчёта. Отчёт #" + (param as Report)._id + ". Страница \"Отчёты\".", LoggerTypes.INFO);

                RootV editWindow = new RootV(param);
                editWindow.ShowDialog();

                App.WriteMsg2Log("Закрытие окна редактирования отчёта!", LoggerTypes.INFO);
            }
        }

        public override void ChangePalette()
        {
            var app = (App)Application.Current;

            if (Properties.Settings.Default.IsDarkTheme)
                app.ChangeTheme(new Uri("Resources/Palettes/Dark/Green.xaml", UriKind.Relative));
            else
                app.ChangeTheme(new Uri("Resources/Palettes/Light/Green.xaml", UriKind.Relative));

            Properties.Settings.Default.SelectedPage = 4;
            Properties.Settings.Default.Save();
        }
    }
}
