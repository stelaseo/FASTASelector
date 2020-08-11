using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FASTASelector
{
    internal partial class OptionWindow : Window
    {
        public OptionWindow( )
        {
            InitializeComponent( );
        }


        private void ClickClose( object sender, RoutedEventArgs e )
        {
            Close( );
        }


        private void MouseWheelOnDouble( object sender, MouseWheelEventArgs e )
        {
            if( e.Delta != 0 && DataContext is AppConfiguration configuration && sender is TextBox textBox )
            {
                string propertyName = textBox.Tag.ToString( );
                PropertyInfo propertyInfo = configuration.GetType( ).GetProperty( propertyName );
                double modifier = e.Delta / 120.0;
                double value = (double)propertyInfo.GetValue( configuration );
                if( (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift )
                {
                    modifier *= 5.0;
                }
                propertyInfo.SetValue( configuration, value + modifier );
            }
        }


        private void MouseWheelOnDoubleBigNumber( object sender, MouseWheelEventArgs e )
        {
            if( e.Delta != 0 && DataContext is AppConfiguration configuration && sender is TextBox textBox )
            {
                string propertyName = textBox.Tag.ToString( );
                PropertyInfo propertyInfo = configuration.GetType( ).GetProperty( propertyName );
                double modifier = e.Delta / 120.0 * 100.0;
                double value = (double)propertyInfo.GetValue( configuration );
                if( (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift )
                {
                    modifier *= 10.0;
                }
                propertyInfo.SetValue( configuration, value + modifier );
            }
        }


        private void MouseWheelOnInteger( object sender, MouseWheelEventArgs e )
        {
            if( e.Delta != 0 && DataContext is AppConfiguration configuration && sender is TextBox textBox )
            {
                string propertyName = textBox.Tag.ToString( );
                PropertyInfo propertyInfo = configuration.GetType( ).GetProperty( propertyName );
                int modifier = e.Delta > 0 ? 1 : -1;
                int value = (int)propertyInfo.GetValue( configuration );
                if( (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift )
                {
                    modifier *= 5;
                }
                propertyInfo.SetValue( configuration, value + modifier );
            }
        }

    }
}
