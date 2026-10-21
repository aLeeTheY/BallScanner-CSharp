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
    public class CalibrateVM : PageVM
    {
        // Команды
        public static RelayCommand PerformAction { get; set; }

        public RelayCommand ChangeImageCommand { get; set; }

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
        private byte _thresholdValue;
        public byte ThresholdValue
        {
            get => _thresholdValue;
            set
            {
                if (_thresholdValue == value) return;

                _thresholdValue = value;
                OnPropertyChanged(nameof(ThresholdValue));

                Properties.Settings.Default.Threshold = ThresholdValue;
                Properties.Settings.Default.Save();

                if (Data != null && Data.Length != 0 && !isDragging) UpdateImage(100);
            }
        }

        private bool isDragging;

        // Первое и второе пороговые значения
        private int _firstThreshold;
        public int FirstThreshold
        {
            get => _firstThreshold;
            set
            {
                if (_firstThreshold == value) return;

                _firstThreshold = value;
                OnPropertyChanged(nameof(FirstThreshold));

                Properties.Settings.Default.FirstThreshold = FirstThreshold;
                Properties.Settings.Default.Save();
            }
        }

        private int _secondThreshold;
        public int SecondThreshold
        {
            get => _secondThreshold;
            set
            {
                if (_secondThreshold == value) return;

                _secondThreshold = value;
                OnPropertyChanged(nameof(SecondThreshold));

                Properties.Settings.Default.SecondThreshold = SecondThreshold;
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
        public CalibrateVM()
        {
            PerformAction = new RelayCommand(OnPerformAction);

            // Подгрузить настройки
            ThresholdValue = Properties.Settings.Default.Threshold;

            FirstThreshold = Properties.Settings.Default.FirstThreshold;
            SecondThreshold = Properties.Settings.Default.SecondThreshold;

            ChangeImageCommand = new RelayCommand(ChangeImage);
        }

        // ТЕПЕРЬ ЭТОТ МЕТОД БЕЗОПАСНО ОБНОВЛЯЕТ UI ИЗ ФОНОВОГО ПОТОКА
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
                    App.WriteMsg2Log("Нажатие на кнопку \"Загрузка изображений\" на странице \"Калибровка\"", LoggerTypes.INFO);

                    if (worker_DownloadExecute == null || (worker_DownloadExecute != null && !worker_DownloadExecute.IsBusy))
                    {
                        OpenFileDialog openFile = new OpenFileDialog();
                        openFile.Filter = "JPEG/JPG - JPG Files|*.jpeg;*.jpg|BMP - Windows Bitmap|*.bmp|PNG - Portable Network Graphics|*.png";
                        openFile.Multiselect = true;

                        if (openFile.ShowDialog() != true) break;

                        string[] fileNames = openFile.FileNames;
                        string[] safeFileNames = openFile.SafeFileNames;

                        worker_DownloadExecute = new BackgroundWorker();
                        worker_DownloadExecute.WorkerReportsProgress = false;
                        worker_DownloadExecute.WorkerSupportsCancellation = false;

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
                    App.WriteMsg2Log("Нажатие на кнопку \"Предыдущее изображение\" на странице \"Калибровка\"", LoggerTypes.INFO);

                    if (Data != null && Data.Length > 0 && currentImageIndex > 0)
                    {
                        Interlocked.Decrement(ref currentImageIndex);
                        OnPropertyChanged(nameof(CurrentData));
                        UpdateImage(100);
                    }
                    break;
                case "Button_NextImage":
                    App.WriteMsg2Log("Нажатие на кнопку \"Следующее изображение\" на странице \"Калибровка\"", LoggerTypes.INFO);

                    if (Data != null && Data.Length > 0 && currentImageIndex < Data.Length - 1)
                    {
                        Interlocked.Increment(ref currentImageIndex);
                        OnPropertyChanged(nameof(CurrentData));
                        UpdateImage(100);
                    }
                    break;
                case "Button_Execute":
                    App.WriteMsg2Log("Нажатие на кнопку \"Выполнить\" на странице \"Калибровка\"", LoggerTypes.INFO);

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

                                Parallel.For(0, data.Length, new ParallelOptions
                                {
                                    MaxDegreeOfParallelism = cores
                                }, i =>
                                {
                                    int blackPixels = CountBlackPixels(data[i].ImagePath, threshold);

                                    BallGrade grade;
                                    if (blackPixels <= first) grade = BallGrade.FIRST;
                                    else if (blackPixels <= second) grade = BallGrade.SECOND;
                                    else grade = BallGrade.DEFECTIVE;

                                    Interlocked.Add(ref sum, blackPixels);
                                    int currentProgress = Interlocked.Increment(ref progress);

                                    // БЕЗОПАСНО отправляем данные в UI-поток
                                    worker_DownloadExecute.ReportProgress(currentProgress, new object[] { i, blackPixels, grade });
                                });

                                // Обновляем среднее значение ТОЛЬКО в UI-потоке
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    AvgNumBlackPixels = sum / total;
                                });

                                App.WriteMsg2Log($"Завершено сканирование {total} изображений на странице \"Калибровка\"! Среднее число чёрных пикселей = {AvgNumBlackPixels}. Пороговое значение ползунка = {threshold}. Первое пороговое значение = {first}. Второе пороговое значение = {second}.", LoggerTypes.INFO);
                            }
                        };
                        worker_DownloadExecute.RunWorkerAsync();
                    }
                    break;
                case "Slider_DragStarted":
                    isDragging = true;
                    break;
                case "Slider_DragCompleted":
                    isDragging = false;
                    App.WriteMsg2Log("Изменено положение ползунка \"Пороговое значение\" на странице \"Калибровка\"! Новое значение = " + ThresholdValue, LoggerTypes.INFO);

                    if (Data != null && Data.Length > 0)
                        UpdateImage(0);
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

            // Запоминаем путь локально, чтобы избежать ошибок при изменении массива Data во время работы
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
                        App.WriteMsg2Log("Ошибка \"" + nameof(ArgumentException) + "\" в методе \"" + nameof(UpdateImage) + "\". Активная страница \"Калибровка\". Текст ошибки: " + App.GetFullExceptionMessage(e), LoggerTypes.ERROR);
                        Application.Current.Dispatcher.Invoke(() => MessageBox.Show(App.L("LangRes_Msg_ErrorPrefix") + App.GetFullExceptionMessage(e), App.L("LangRes_Msg_TitleArgument"), MessageBoxButton.OK, MessageBoxImage.Error));
                    }
                    catch (Exception e)
                    {
                        App.WriteMsg2Log("Непредвиденная ошибка в методе \"" + nameof(UpdateImage) + "\". Активная страница \"Калибровка\". Текст ошибки: " + App.GetFullExceptionMessage(e), LoggerTypes.FATAL);
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
                app.ChangeTheme(new Uri("Resources/Palettes/Dark/Yellow.xaml", UriKind.Relative));
            else
                app.ChangeTheme(new Uri("Resources/Palettes/Light/Yellow.xaml", UriKind.Relative));

            Properties.Settings.Default.SelectedPage = 3;
            Properties.Settings.Default.Save();
        }
    }
}
