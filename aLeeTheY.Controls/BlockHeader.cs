using System.Windows;
using System.Windows.Controls;

namespace aLeeTheY.Controls
{
    public class BlockHeader : TextBox
    {
        static BlockHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlockHeader),
                new FrameworkPropertyMetadata(typeof(BlockHeader)));
        }
    }
}
