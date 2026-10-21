using BallScanner.Data;
using BallScanner.Data.Tables;
using BallScanner.MVVM.Base;
using BallScanner.MVVM.Commands;
using BallScanner.MVVM.Models.Main;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BallScanner.MVVM.ViewModels.Main
{
    public class ScanVM : PageVM
    {
        // Команды
        public static RelayCommand PerformAction { get; set; }

        public RelayCommand ChangeImageCommand { get; set; }
        public RelayCommand SaveReportCommand { get; set; }

        // Декодирование изображения, если пикселей на экране меньше разрешения картинки
        private int _decodePixelHeight;
        public int DecodePixelHeight
        {
            get => _decodePixelHeight;
            set
            {
                _decodePixelHeight = value;

                PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);

                double scaleY = 0;
                if (source != null)
                {
                    scaleY = source.CompositionTarget.TransformToDevice.M22;
                }

                _decodePixelHeight = (int)(_decodePixelHeight * scaleY);
                if (Data != null && Data.Length != 0) UpdateImage(100);
            }
        }

        // Массив с изображениями
        private ImageData[] _data;
        public ImageData[] Data
        {
            get => _data;
            set
            {
                _data = value;
                OnPropertyChanged(nameof(Data));
            }
        }

        // Только для чтения в дизайне
        public ImageData CurrentData
        {
            get
            {
                if (Data == null || Data.Length == 0) return null;
                return Data[currentImageIndex];
            }
        }

        // Индекс текущего изображения (ожидаемого)
        private int currentImageIndex;

        // Текущее изображение на экране
        private BitmapImage _currentImage;
        public BitmapImage CurrentImage
        {
            get => _currentImage;
            set
            {
                if (_currentImage == value) return;

                _currentImage = value;
                OnPropertyChanged(nameof(CurrentImage));
            }
        }

        // Текущая чувствительность сканера (порог пикселя)
        public byte ThresholdValue
        {
            get => Properties.Settings.Default.Threshold;
        }

        // Первое и второе пороговые значения
        public int FirstThreshold
        {
            get => Properties.Settings.Default.FirstThreshold;
        }

        public int SecondThreshold
        {
            get => Properties.Settings.Default.SecondThreshold;
        }

        private string _fraction;
        public string Fraction
        {
            get => _fraction;
            set
            {
                if (_fraction == value) return;

                _fraction = value;
                OnPropertyChanged(nameof(Fraction));

                Properties.Settings.Default.Fraction = Fraction;
                Properties.Settings.Default.Save();
            }
        }

        private string _partia_number;
        public string Partia_Number
        {
            get => _partia_number;
            set
            {
                if (_partia_number == value) return;

                _partia_number = value;
                OnPropertyChanged(nameof(Partia_Number));

                Properties.Settings.Default.Partia_Number = Partia_Number;
                Properties.Settings.Default.Save();
            }
        }

        private long _avgNumBlackPixels;
        public long AvgNumBlackPixels
        {
            get => _avgNumBlackPixels;
            set
            {
                if (_avgNumBlackPixels == value) return;

                _avgNumBlackPixels = value;
                OnPropertyChanged(nameof(AvgNumBlackPixels));
            }
        }

        private int _imagesCount;
        public int ImagesCount
        {
            get => _imagesCount;
            set
            {
                if (_imagesCount == value) return;

                _imagesCount = value;
                OnPropertyChanged(nameof(ImagesCount));
            }
        }

        private int _currentProgress;
        public int CurrentProgress
        {
            get => _currentProgress;
            private set
            {
                if (_currentProgress == value) return;

                _currentProgress = value;
                OnPropertyChanged(nameof(CurrentProgress));
            }
        }

        // Фоновые задачи
        private BackgroundWorker worker_DownloadExecute;

        // Конструктор
        public ScanVM()
        {
            Fraction = Properties.Settings.Default.Fraction;
            Partia_Number = Properties.Settings.Default.Partia_Number;

            PerformAction = new RelayCommand(OnPerformAction);

            ChangeImageCommand = new RelayCommand(ChangeImage);
            SaveReportCommand = new RelayCommand(SaveReport);
        }

        private void SaveReport(object param)
        {
            // Сохранение данных в БД
            try
            {
                AppDbContext dbContext = AppDbContext.GetInstance();
                Report newReport;

                if (App.CurrentUser != null)
                    newReport = new Report(App.CurrentUser._id, DateTime.Now.Date, Fraction, Partia_Number, AvgNumBlackPixels); // admin or user
                else
                    newReport = new Report(-1, DateTime.Now.Date, Fraction, Partia_Number, AvgNumBlackPixels);  // superuser

                dbContext.Reports.Add(newReport);
                dbContext.SaveChanges();

                // update datagrid in documents vm
                if (DocumentsVM.RefreshDataGrid.CanExecute(null))
                    DocumentsVM.RefreshDataGrid.Execute(null);

                App.WriteMsg2Log("Сохранение нового отчёта! Сохранённая информация: номер отчёта = " + newReport._id + "; дата = " + newReport._date + "; фракция = " + newReport._fraction + "; номер партии = " + newReport._partia_number + "; среднее число чёрных пикселей = " + newReport._avg_black_pixels_value + "; примечание = " + newReport._note + "; id пользователя, создавшего отчёт = " + newReport._user_id + ";", LoggerTypes.INFO);
                MessageBox.Show(App.L("LangRes_Msg_SaveSuccess"), App.L("LangRes_Msg_TitleInfo"), MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
            catch (Exception ex)
            {
                App.WriteMsg2Log("Непредвиденная ошибка во время выполнения! Текст ошибки: " + App.GetFullExceptionMessage(ex), LoggerTypes.FATAL);
                MessageBox.Show(App.L("LangRes_Msg_ErrorPrefix") + App.GetFullExceptionMessage(ex), App.L("LangRes_Msg_TitleUnexpected"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        // ТЕПЕРЬ БЕЗОПАСНО ОБНОВЛЯЕТ UI ИЗ ФОНОВОГО ПОТОКА
        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CurrentProgress = e.ProgressPercentage;

            // Если пришли данные конкретной строки (для кнопки "Выполнить")
            if (e.UserState is object[] rowData)
            {
                int index = (int)rowData[0];
                int blackPixels = (int)rowData[1];
                BallGrade grade = (BallGrade)rowData[2];

                if (Data != null && index >= 0 && index < Data.Length)
                {
                    Data[index].NumberOfBlackPixels = blackPixels;
                    Data[index].BallGrade = grade;
                }
            }
        }

        private void OnPerformAction(object param)
        {
            switch (param)
            {
                case "Button_LoadImages":
                    App.WriteMsg2Log("Нажатие на кнопку \"Загрузка изображений\" на странице \"Сканирование\"", LoggerTypes.INFO);

                    if (worker_DownloadExecute == null || (worker_DownloadExecute != null && !worker_DownloadExecute.IsBusy))
                    {
                        // 1. Диалог открываем в UI-потоке
                        OpenFileDialog openFile = new OpenFileDialog();
                        openFile.Filter = "JPEG/JPG - JPG Files|*.jpeg;*.jpg|BMP - Windows Bitmap|*.bmp|PNG - Portable Network Graphics|*.png";
                        openFile.Multiselect = true;

                        if (openFile.ShowDialog() != true) break;

                        string[] fileNames = openFile.FileNames;
                        string[] safeFileNames = openFile.SafeFileNames;

                        worker_DownloadExecute = new BackgroundWorker();
                        worker_DownloadExecute.WorkerReportsProgress = false;
                        worker_DownloadExecute.WorkerSupportsCancellation = false;

                        // 2. Только готовим данные в фоне
                        worker_DownloadExecute.DoWork += delegate (object sender, DoWorkEventArgs e)
                        {
                            try
                            {
                                var tempData = new ImageData[fileNames.Length];
                                for (int i = 0; i < tempData.Length; i++)
                                {
                                    tempData[i] = new ImageData()
                                    {
                                        Id = i + 1,
                                        Name = safeFileNames[i],
                                        ImagePath = fileNames[i],
                                        NumberOfBlackPixels = 0,
                                        BallGrade = BallGrade.INDEFINED
                                    };
                                }
                                e.Result = tempData;
                            }
                            catch (Exception ex)
                            {
                                App.WriteMsg2Log("Ошибка при загрузке: " + App.GetFullExceptionMessage(ex), LoggerTypes.FATAL);
                                e.Result = null;
                            }
                        };

                        // 3. Обновляем UI в главном потоке
                        worker_DownloadExecute.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
                        {
                            if (e.Result is ImageData[] tempData && tempData.Length > 0)
                            {
                                ImagesCount = tempData.Length;
                                Data = tempData;
                                currentImageIndex = 0;
                                CurrentProgress = 0;

                                OnPropertyChanged(nameof(CurrentData));
                                UpdateImage(0);
                            }
                            else
                            {
                                MessageBox.Show("Не удалось загрузить данные изображений.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        };

                        worker_DownloadExecute.RunWorkerAsync();
                    }
                    break;
                case "Button_PrevImage":
                    App.WriteMsg2Log("Нажатие на кнопку \"Предыдущее изображение\" на странице \"Сканирование\"", LoggerTypes.INFO);

                    if (Data != null && Data.Length > 0 && currentImageIndex > 0)
                    {
                        Interlocked.Decrement(ref currentImageIndex);
                        OnPropertyChanged(nameof(CurrentData));
                        UpdateImage(100);
                    }
                    break;
                case "Button_NextImage":
                    App.WriteMsg2Log("Нажатие на кнопку \"Следующее изображение\" на странице \"Сканирование\"", LoggerTypes.INFO);

                    if (Data != null && Data.Length > 0 && currentImageIndex < Data.Length - 1)
                    {
                        Interlocked.Increment(ref currentImageIndex);
                        OnPropertyChanged(nameof(CurrentData));
                        UpdateImage(100);
                    }
                    break;
                case "Button_Execute":
                    App.WriteMsg2Log("Нажатие на кнопку \"Выполнить\" на странице \"Сканирование\"", LoggerTypes.INFO);

                    if (worker_DownloadExecute == null || (worker_DownloadExecute != null && !worker_DownloadExecute.IsBusy))
                    {
                        worker_DownloadExecute = new BackgroundWorker();
                        worker_DownloadExecute.WorkerReportsProgress = true;
                        worker_DownloadExecute.WorkerSupportsCancellation = false;
                        worker_DownloadExecute.ProgressChanged += ProgressChanged;

                        worker_DownloadExecute.DoWork += delegate
                        {
                            if (Data != null && Data.Length > 0)
                            {
                                int progress = 0;
                                long sum = 0;

                                byte threshold = ThresholdValue;
                                int first = FirstThreshold;
                                int second = SecondThreshold;

                                ImageData[] data = Data;
                                int total = data.Length;
                                int cores = Environment.ProcessorCount;

                                Parallel.For(0, total, new ParallelOptions
                                {
                                    MaxDegreeOfParallelism = cores
                                }, i =>
                                {
                                    try
                                    {
                                        int blackPixels = CountBlackPixels(data[i].ImagePath, threshold);

                                        BallGrade grade;
                                        if (blackPixels <= first) grade = BallGrade.FIRST;
                                        else if (blackPixels <= second) grade = BallGrade.SECOND;
                                        else grade = BallGrade.DEFECTIVE;

                                        Interlocked.Add(ref sum, blackPixels);

                                        // БЕЗОПАСНО отправляем данные в UI-поток, а не пишем в Data[i] напрямую
                                        int currentProgress = Interlocked.Increment(ref progress);
                                        worker_DownloadExecute.ReportProgress(currentProgress, new object[] { i, blackPixels, grade });
                                    }
                                    catch (Exception ex)
                                    {
                                        App.WriteMsg2Log($"Ошибка при обработке файла \"{data[i].ImagePath}\": {App.GetFullExceptionMessage(ex)}", LoggerTypes.ERROR);

                                        // Всё равно увеличиваем прогресс, чтобы он не завис
                                        Interlocked.Increment(ref progress);
                                        worker_DownloadExecute.ReportProgress(progress, new object[] { i, 0, BallGrade.DEFECTIVE });
                                    }
                                });

                                // Обновляем среднее значение ТОЛЬКО в UI-потоке
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    AvgNumBlackPixels = total > 0 ? sum / total : 0;
                                });

                                App.WriteMsg2Log($"Завершено сканирование {total} изображений на странице \"Сканирование\"! Среднее число чёрных пикселей = {AvgNumBlackPixels}. Пороговое значение ползунка = {threshold}. Первое пороговое значение = {first}. Второе пороговое значение = {second}.", LoggerTypes.INFO);
                            }
                        };
                        worker_DownloadExecute.RunWorkerAsync();
                    }
                    break;
            }
        }

        private unsafe int CountBlackPixels(string path, byte threshold)
        {
            using (Bitmap original = new Bitmap(path))
            {
                Rectangle rect = new Rectangle(0, 0, original.Width, original.Height);
                BitmapData data = original.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                try
                {
                    int stride = data.Stride;
                    int width = original.Width;
                    int height = original.Height;
                    byte* ptr = (byte*)data.Scan0;

                    int count = 0;
                    for (int y = 0; y < height; y++)
                    {
                        byte* row = ptr + y * stride;
                        for (int x = 0; x < width; x++)
                        {
                            int offset = x * 4;
                            int b = row[offset];
                            int g = row[offset + 1];
                            int r = row[offset + 2];

                            int gray = (r * 77 + g * 150 + b * 29) >> 8;
                            if (gray < threshold) count++;
                        }
                    }
                    return count;
                }
                finally { original.UnlockBits(data); }
            }
        }

        private unsafe Bitmap GetThresholdBitmap(string path, byte threshold)
        {
            Bitmap original = new Bitmap(path);
            Rectangle rect = new Rectangle(0, 0, original.Width, original.Height);
            BitmapData data = original.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            try
            {
                int stride = data.Stride;
                int width = original.Width;
                int height = original.Height;
                byte* ptr = (byte*)data.Scan0;

                for (int y = 0; y < height; y++)
                {
                    byte* row = ptr + y * stride;
                    for (int x = 0; x < width; x++)
                    {
                        int offset = x * 4;
                        int b = row[offset];
                        int g = row[offset + 1];
                        int r = row[offset + 2];
                        int gray = (r * 77 + g * 150 + b * 29) >> 8;

                        byte v = gray < threshold ? (byte)0 : (byte)255;
                        row[offset] = v;
                        row[offset + 1] = v;
                        row[offset + 2] = v;
                        row[offset + 3] = 255;
                    }
                }
            }
            finally { original.UnlockBits(data); }

            return original;
        }

        private async void UpdateImage(int delay)
        {
            int myImageIndex = currentImageIndex;
            int myThresholdValue = ThresholdValue;
            int myDecodePixelHeight = DecodePixelHeight;

            await Task.Delay(delay);

            if (Data == null || Data.Length == 0 || currentImageIndex < 0 || currentImageIndex >= Data.Length) return;

            // Запоминаем путь локально, чтобы избежать ошибок при быстром переключении
            string imagePath = Data[currentImageIndex].ImagePath;

            if (delay == 0 || (myImageIndex == currentImageIndex && myDecodePixelHeight == DecodePixelHeight && myThresholdValue == ThresholdValue))
            {
                BitmapImage resultImage = null;

                await Task.Run(() =>
                {
                    try
                    {
                        using (Bitmap original = GetThresholdBitmap(imagePath, (byte)myThresholdValue))
                        {
                            if (myImageIndex != currentImageIndex) return;

                            using (MemoryStream memory = new MemoryStream())
                            {
                                original.Save(memory, ImageFormat.Bmp);
                                int imgHeight = original.Height;
                                memory.Position = 0;

                                BitmapImage bitmapImage = new BitmapImage();
                                bitmapImage.BeginInit();
                                bitmapImage.StreamSource = memory;
                                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;

                                if (imgHeight > myDecodePixelHeight)
                                    bitmapImage.DecodePixelHeight = myDecodePixelHeight;

                                bitmapImage.EndInit();
                                bitmapImage.Freeze();

                                resultImage = bitmapImage;
                            }
                        }
                    }
                    catch (ArgumentException e)
                    {
                        App.WriteMsg2Log("Ошибка \"" + nameof(ArgumentException) + "\" в методе \"" + nameof(UpdateImage) + "\". Активная страница \"Сканирование\". Текст ошибки: " + App.GetFullExceptionMessage(e), LoggerTypes.ERROR);
                        Application.Current.Dispatcher.Invoke(() => MessageBox.Show(App.L("LangRes_Msg_ErrorPrefix") + App.GetFullExceptionMessage(e), App.L("LangRes_Msg_TitleArgument"), MessageBoxButton.OK, MessageBoxImage.Error));
                    }
                    catch (Exception e)
                    {
                        App.WriteMsg2Log("Непредвиденная ошибка в методе \"" + nameof(UpdateImage) + "\". Активная страница \"Сканирование\". Текст ошибки: " + App.GetFullExceptionMessage(e), LoggerTypes.FATAL);
                        Application.Current.Dispatcher.Invoke(() => MessageBox.Show(App.L("LangRes_Msg_ErrorPrefix") + App.GetFullExceptionMessage(e), App.L("LangRes_Msg_TitleUnexpected"), MessageBoxButton.OK, MessageBoxImage.Error));
                    }
                });

                // Присваиваем картинку в UI-потоке
                if (resultImage != null && myImageIndex == currentImageIndex)
                {
                    CurrentImage = resultImage;
                }
            }
        }

        public void ChangeImage(object item)
        {
            if (item != null)
            {
                ImageData data = item as ImageData;
                currentImageIndex = data.Id - 1;

                OnPropertyChanged(nameof(CurrentData));
                UpdateImage(0);

                App.WriteMsg2Log("Изменено текущее изображение. Информация о новом изображении: номер в списке = " + data.Id + "; имя файла = " + data.Name + "; путь к файлу = " + data.ImagePath + "; число чёрных пикселей = " + data.NumberOfBlackPixels + "; cорт шарика = " + data.BallGrade_Text + ";", LoggerTypes.INFO);
            }
        }

        public override void ChangePalette()
        {
            var app = (App)Application.Current;

            if (Properties.Settings.Default.IsDarkTheme)
                app.ChangeTheme(new Uri("Resources/Palettes/Dark/Orange.xaml", UriKind.Relative));
            else
                app.ChangeTheme(new Uri("Resources/Palettes/Light/Orange.xaml", UriKind.Relative));

            Properties.Settings.Default.SelectedPage = 2;
            Properties.Settings.Default.Save();
        }
    }
}
