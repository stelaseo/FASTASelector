using System.Windows;
using System.Windows.Input;

namespace FASTASelector.UserInterface
{
    internal partial class AboutDialog : Window
    {
        public AboutDialog( )
        {
            InitializeComponent( );
            Title = App.Name + " v" + App.Version;
        }


        private void ClickClose( object sender, RoutedEventArgs e )
        {
            Close( );
        }


        private void Window_PreviewKeyDown( object sender, KeyEventArgs e )
        {
            switch( e.Key )
            {
            case Key.Enter:
            case Key.Escape:
                Close( );
                e.Handled = true;
                break;
            }
        }

    }
}
