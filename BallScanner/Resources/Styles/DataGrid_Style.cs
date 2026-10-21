using System.Windows;
using System.Windows.Controls;

namespace BallScanner.Resources.Styles
{
    public partial class DataGrid_Style
    {
        public DataGrid_Style()
        {
            InitializeComponent();
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                // Создаем стиль для TextBlock
                var centeredTrimmingStyle = new Style(typeof(TextBlock));
                centeredTrimmingStyle.Setters.Add(new Setter(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis));
                centeredTrimmingStyle.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.NoWrap));
                centeredTrimmingStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
                centeredTrimmingStyle.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
                centeredTrimmingStyle.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Stretch));

                // Применяем ТОЛЬКО к колонкам
                foreach (var column in dataGrid.Columns)
                {
                    if (column is DataGridTextColumn textColumn)
                    {
                        var newStyle = new Style(typeof(TextBlock), textColumn.ElementStyle);

                        foreach (var setter in centeredTrimmingStyle.Setters)
                        {
                            newStyle.Setters.Add(setter);
                        }

                        textColumn.ElementStyle = newStyle;
                    }
                }
            }
        }
    }
}
