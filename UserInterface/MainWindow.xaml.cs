using FASTASelector.Configurations;
using FASTASelector.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace FASTASelector.UserInterface
{
    internal partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const string CONFIG_FILE_NAME = "FASTASelector.json";
        private DispatcherTimer _statusTextReset = new DispatcherTimer( );


        public MainWindow( )
        {
            Configuration.Read( CONFIG_FILE_NAME );
            InitializeComponent( );
            InitializeCommandBindings( );
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public AppConfiguration Configuration
        {
            get { return App.Configuration; }
        }


        public Controller Controller
        {
            get { return App.Controller; }
        }


        public string StatusText
        {
            get;
            private set;
        }


        public UIConfig UIConfiguration
        {
            get { return App.Configuration.CoreUI; }
        }


        private void InitializeCommandBindings( )
        {
            RoutedCommand command;
            command = new RoutedCommand( "Undo", typeof( Window ) );
            command.InputGestures.Add( new KeyGesture( Key.Z, ModifierKeys.Control ) );
            CommandBindings.Add( new CommandBinding( command, MenuUndo ) );

            command = new RoutedCommand( "Redo", typeof( Window ) );
            command.InputGestures.Add( new KeyGesture( Key.Y, ModifierKeys.Control ) );
            CommandBindings.Add( new CommandBinding( command, MenuRedo ) );

            command = new RoutedCommand( "Options", typeof( Window ) );
            command.InputGestures.Add( new KeyGesture( Key.F10 ) );
            CommandBindings.Add( new CommandBinding( command, MenuViewOptions ) );

            command = new RoutedCommand( "About", typeof( Window ) );
            command.InputGestures.Add( new KeyGesture( Key.F1 ) );
            CommandBindings.Add( new CommandBinding( command, MenuViewAbout ) );
        }


        private void NotifyPropertyChanged( [CallerMemberName] string name = "" )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( name ) );
        }


        private void PrintLog( int level, string format, object[] args )
        {
            if( level <= App.LogLevel )
            {
                StringBuilder sb = new StringBuilder( );
                sb.Append( '[' );
                sb.Append( DateTime.Now.ToString( "HH:mm" ) );
                sb.Append( ']' );
                sb.Append( ' ' );
                sb.Append( string.Format( format, args ) );
                sb.Append( Environment.NewLine );
                string message = sb.ToString( );
                Dispatcher.BeginInvoke( new System.Action( ( ) =>
                {
                    uiLogText.AppendText( message );
                    uiLogText.ScrollToEnd( );
                } ) );
            }
        }


        private void ShowStatus( string text )
        {
            _statusTextReset.Interval = TimeSpan.FromMilliseconds( Configuration.CoreUI.StatusTextDuration );
            _statusTextReset.Start( );
            StatusText = text;
            NotifyPropertyChanged( "StatusText" );
        }


        private void Window_Closing( object sender, CancelEventArgs e )
        {
            App.Log = null;
            Configuration.Write( CONFIG_FILE_NAME );
        }


        private void Window_Loaded( object sender, RoutedEventArgs e )
        {
            _statusTextReset.Interval = TimeSpan.FromMilliseconds( 5000.0 );
            _statusTextReset.Tick += ( s, ev ) =>
            {
                _statusTextReset.Stop( );
                StatusText = "";
                NotifyPropertyChanged( "StatusText" );
            };
            App.Log = PrintLog;
            Title = App.Name + " v" + App.Version;
            uiMetadataList.UpdateListViewColumns( );
            uiSequenceList.UpdateListViewColumns( );
        }

    }
}
