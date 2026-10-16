using System;
using System.Globalization;
using System.Windows.Data;

namespace aLeeTheY.Utils.Converters
{
    public class NotePreviewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string note = value as string;

            if (string.IsNullOrEmpty(note)) return string.Empty;

            // Если заметка в одну строку — вернуть как есть
            if (!note.Contains("\n") && !note.Contains("\r"))
                return note;

            // Иначе — первая строка + "..."
            int firstNewLine = note.IndexOfAny(new[] { '\r', '\n' });
            string firstLine = note.Substring(0, firstNewLine).TrimEnd();

            return firstLine + "...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
