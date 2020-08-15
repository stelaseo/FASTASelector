using FASTASelector.FASTA;
using FASTASelector.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FASTASelector.UserInterface
{
    internal partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const string CONFIG_FILE_NAME = "FASTASelector.json";
        private DispatcherTimer _statusTextReset = new DispatcherTimer( );
        private List<Task> _taskHistory = new List<Task>( );
        private int _taskIndex = 0;


        public MainWindow( )
        {
            Configuration.Read( CONFIG_FILE_NAME );
            InitializeComponent( );
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public AppConfiguration Configuration
        {
            get { return App.Configuration; }
        }


        public bool HasTasksToRedo
        {
            get { return 0 <= _taskIndex && _taskIndex < _taskHistory.Count; }
        }


        public bool HasTasksToUndo
        {
            get { return 0 < _taskIndex && _taskIndex <= _taskHistory.Count; }
        }


        public MetadataCollection Metadata
        {
            get;
            private set;
        } = new MetadataCollection( );


        public SequenceCollection Sequences
        {
            get;
            private set;
        } = new SequenceCollection( );


        public string StatusText
        {
            get;
            private set;
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
            _statusTextReset.Interval = TimeSpan.FromMilliseconds( Configuration.StatusTextDuration );
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
            UpdateMetadataListView( );
            UpdateSequenceListView( );
        }


        private void UpdateMetadataLinkWithSequences( )
        {
            foreach( Sequence sequence in Sequences )
            {
                sequence.Metadata = Metadata.Find( sequence.Header );
            }
        }


        private void UpdateMetadataListView( )
        {
            GridView gridView = new GridView( );
            foreach( KeyValuePair<string, string> kv in Metadata.Columns )
            {
                gridView.Columns.Add( Factory.CreateTextBlockColumn( App.Configuration.UIMetadataListColumnSize, kv.Value, "[" + kv.Key + "]", ColumnHeaderClick ) );
            }
            uiMetadataList.View = gridView;
        }


        private void UpdateSequenceListView( )
        {
            string[] keys = App.Configuration.SequenceHeaders;
            GridView gridView = new GridView( );
            gridView.Columns.Add( Factory.CreateCheckBoxColumn( App.Configuration.UISequenceListColumnSize, "Checked", ColumnHeaderClick ) );
            for( int i = 0; i < keys.Length; ++i )
            {
                string columnName = Metadata.GetColumnName( keys[i] );
                gridView.Columns.Add( Factory.CreateTextBlockColumn( App.Configuration.UISequenceListColumnSize, columnName, "Header[" + keys[i] + "]", ColumnHeaderClick ) );
            }
            gridView.Columns.Add( Factory.CreateTextBlockColumn( App.Configuration.UISequenceListColumnSize, "Length", "Value.Length", ColumnHeaderClick, HorizontalAlignment.Center ) );
            gridView.Columns.Add( Factory.CreateLinkTextBlockColumn( App.Configuration.UISequenceListColumnSize, "Metadata", "Metadata", ColumnHeaderClick, ClickMetadataLink ) );
            uiSequenceList.View = gridView;
        }

    }
}
