using BallScanner.MVVM.Base;

namespace BallScanner.MVVM.Models.Main
{
    public class ImageData : BaseViewModel
    {
        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                if (_id == value) return;

                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;

                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private int _numberOfBlackPixels;
        public int NumberOfBlackPixels
        {
            get => _numberOfBlackPixels;
            set
            {
                if (_numberOfBlackPixels == value) return;

                _numberOfBlackPixels = value;
                OnPropertyChanged(nameof(NumberOfBlackPixels));
            }
        }

        private BallGrade _ballGrade;
        public BallGrade BallGrade
        {
            get => _ballGrade;
            set
            {
                if (_ballGrade == value) return;

                _ballGrade = value;
                OnPropertyChanged(nameof(BallGrade));
                OnPropertyChanged(nameof(BallGrade_Text));
                OnPropertyChanged(nameof(IsDefective));
            }
        }

        public string BallGrade_Text
        {
            get
            {
                switch (BallGrade)
                {
                    case BallGrade.FIRST:
                        return App.L("LangRes_Grade_First");
                    case BallGrade.SECOND:
                        return App.L("LangRes_Grade_Second");
                    case BallGrade.DEFECTIVE:
                        return App.L("LangRes_Grade_NotMatching");
                    case BallGrade.INDEFINED:
                        return "-";
                }
                return null;
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (_imagePath == value) return;

                _imagePath = value;
            }
        }

        public bool IsDefective => BallGrade == BallGrade.DEFECTIVE;
    }
}
