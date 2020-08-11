using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace FASTASelector
{
    internal partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const string CONFIG_FILE_NAME = "FASTASelector.json";
        private string _filename = string.Empty;
        private GridViewColumnHeader _lvSortColumn = null;
        private ListViewSortAdorner _lvSortAdorner = null;
        private DispatcherTimer _statusTextReset = new DispatcherTimer( );
        private List<Sequence> _tempSave = null;


        public MainWindow( )
        {
            Configuration.Read( CONFIG_FILE_NAME );
            InitializeComponent( );
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public AppConfiguration Configuration
        {
            get;
            private set;
        } = new AppConfiguration( );


        public string FileName
        {
            get { return _filename; }
            private set
            {
                _filename = value ?? string.Empty;
                NotifyPropertyChanged( );
            }
        }


        public bool HasTemporarySave
        {
            get { return _tempSave != null; }
        }


        public ObservableCollection<Sequence> Sequences
        {
            get;
            private set;
        } = new ObservableCollection<Sequence>( );


        public string StatusText
        {
            get;
            private set;
        }


        public List<Sequence> TemporarySave
        {
            get { return _tempSave; }
            private set
            {
                _tempSave = value;
                NotifyPropertyChanged( );
                NotifyPropertyChanged( "HasTemporarySave" );
            }
        }


        private void ClickLogClear( object sender, RoutedEventArgs e )
        {
            uiLogText.Clear( );
        }


        private void ClickLogSave( object sender, RoutedEventArgs e )
        {
            SaveFileDialog dlg = new SaveFileDialog( );
            dlg.CheckPathExists = true;
            dlg.DefaultExt = ".log";
            dlg.DereferenceLinks = true;
            dlg.Filter = "Log files (*.log)|*.log";
            dlg.Title = "Select a file to save the log messages. . .";
            dlg.FileName = string.Empty;
            dlg.InitialDirectory = Environment.CurrentDirectory;
            if( dlg.ShowDialog( ) == true )
            {
                try
                {
                    File.WriteAllText( dlg.FileName, uiLogText.Text );
                    ShowStatus( "Successfully saved the log messages to " + dlg.FileName );
                }
                catch( Exception ex )
                {
                    App.Log( ex.Message );
                }
            }
        }


        private void ClickMenuItemOpen( object sender, RoutedEventArgs e )
        {
            DatasetReadFromFile( true );
        }


        private void ClickMenuItemMerge( object sender, RoutedEventArgs e )
        {
            DatasetReadFromFile( false );
        }


        private void ClickMenuItemSave( object sender, RoutedEventArgs e )
        {
            if( string.IsNullOrWhiteSpace( FileName ) )
            {
                ClickMenuItemSaveAs( sender, e );
            }
            else if( FASTA.Write( FileName, Sequences ) )
            {
                TemporarySave = null;
                UpdateWindowCaption( false );
                ShowStatus( "Successfully saved the dataset to " + FileName );
            }
            else
            {
                ShowStatus( "Failed to save the dataset to " + FileName );
            }
        }


        private void ClickMenuItemSaveAs( object sender, RoutedEventArgs e )
        {
            SaveFileDialog dlg = new SaveFileDialog( );
            dlg.CheckPathExists = true;
            dlg.DefaultExt = ".fasta";
            dlg.DereferenceLinks = true;
            dlg.Filter = "FASTA files (*.fasta,*.fna,*.ffn,*.faa,*.frn)|*.fasta;*.fna;*.ffn;*.faa;*.frn|All files (*.*)|*.*";
            dlg.Title = "Select a fasta file to save current dataset. . .";
            if( !string.IsNullOrWhiteSpace( FileName ) )
            {
                FileInfo fileInfo = new FileInfo( FileName );
                dlg.FileName = fileInfo.Name;
                dlg.InitialDirectory = fileInfo.DirectoryName;
            }
            else
            {
                dlg.FileName = string.Empty;
                dlg.InitialDirectory = Environment.CurrentDirectory;
            }
            if( dlg.ShowDialog( ) == true )
            {
                if( FASTA.Write( dlg.FileName, Sequences ) )
                {
                    FileName = dlg.FileName;
                    TemporarySave = null;
                    UpdateWindowCaption( false );
                    ShowStatus( "Successfully saved the dataset to " + dlg.FileName );
                }
                else
                {
                    FileName = null;
                    ShowStatus( "Failed to save the dataset to " + dlg.FileName );
                }
            }
        }


        private void ClickMenuItemExit( object sender, RoutedEventArgs e )
        {
            Close( );
        }


        private void ClickMenuItemSelectAll( object sender, RoutedEventArgs e )
        {
            foreach( Sequence seq in Sequences )
            {
                seq.Checked = true;
            }
            App.Log( "Selected all sequences" );
        }


        private void ClickMenuItemDeselectAll( object sender, RoutedEventArgs e )
        {
            foreach( Sequence seq in Sequences )
            {
                seq.Checked = false;
            }
            App.Log( "Deselected all sequences" );
        }


        private void ClickMenuItemTmpSave( object sender, RoutedEventArgs e )
        {
            TemporarySave = new List<Sequence>( );
            foreach( Sequence seq in Sequences )
            {
                TemporarySave.Add( seq );
            }
            NotifyPropertyChanged( "HasTemporarySave" );
            ShowStatus( "Saved the current dataset to memory" );
            App.Log( "Saved {0} sequence(s) to the memory", Sequences.Count );
        }


        private void ClickMenuItemTmpRestore( object sender, RoutedEventArgs e )
        {
            Sequences.Clear( );
            foreach( Sequence seq in TemporarySave )
            {
                seq.Checked = false;
                seq.Highlights.Clear( );
                Sequences.Add( seq );
            }
            uiSequenceList.SelectedItem = null;
            UpdateWindowCaption( true );
            ShowStatus( "Reverted to the saved state" );
            App.Log( "Restored {0} sequence(s) from the memory", Sequences.Count );
        }


        private void ClickMenuItemRemove( object sender, RoutedEventArgs e )
        {
            int count = 0;
            for( int i = Sequences.Count - 1; i >= 0; --i )
            {
                if( Sequences[i].Checked )
                {
                    App.Log( "Remove sequence {0}", Sequences[i].Header );
                    Sequences.RemoveAt( i );
                    count++;
                }
            }
            if( count > 0 )
            {
                UpdateWindowCaption( true );
                ShowStatus( "Removed " + count + " sequence(s)" );
            }
            else
            {
                ShowStatus( "No sequence is selected" );
            }
            App.Log( "Total of {0} sequence(s) have been removed from the list", count );
        }


        private void ClickMenuItemViewOptions( object sender, RoutedEventArgs e )
        {
            OptionWindow optionWindow = new OptionWindow( );
            optionWindow.DataContext = Configuration;
            optionWindow.Owner = this;
            optionWindow.ShowDialog( );
            uiSequenceViewer.UpdateView( );
        }


        private void ClickMenuItemViewAbout( object sender, RoutedEventArgs e )
        {
            MessageBox.Show( this, App.License, "About " + App.Name + " v" + App.Version, MessageBoxButton.OK, MessageBoxImage.Information );
        }


        private void ClickSearchButton( object sender, RoutedEventArgs e )
        {
            if( !string.IsNullOrWhiteSpace( Configuration.SearchText ) )
            {
                int count = 0;
                foreach( Sequence seq in Sequences )
                {
                    if( seq.Highlight( Configuration.SearchText, Configuration.SearchBeginOffset, Configuration.SearchEndOffset ) )
                    {
                        count++;
                    }
                }
                App.Log( "Found {0} sequence(s) which contain {1}", count, Configuration.SearchText );
                uiSequenceViewer.UpdateView( );
            }
        }


        private void ClickSearchClear( object sender, RoutedEventArgs e )
        {
            if( !string.IsNullOrWhiteSpace( Configuration.SearchText ) )
            {
                Configuration.SearchText = string.Empty;
                foreach( Sequence seq in Sequences )
                {
                    seq.Highlights.Clear( );
                }
                uiSequenceViewer.UpdateView( );
            }
        }


        private void ColumnHeaderClick( object sender, RoutedEventArgs e )
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;
            if( _lvSortColumn != null )
            {
                AdornerLayer.GetAdornerLayer( _lvSortColumn ).Remove( _lvSortAdorner );
                uiSequenceList.Items.SortDescriptions.Clear( );
            }
            if( _lvSortColumn == null || _lvSortColumn != column )
            {
                _lvSortColumn = column;
                _lvSortAdorner = new ListViewSortAdorner( _lvSortColumn, ListSortDirection.Ascending );
                AdornerLayer.GetAdornerLayer( _lvSortColumn ).Add( _lvSortAdorner );
                uiSequenceList.Items.SortDescriptions.Add( new SortDescription( column.Tag.ToString( ), ListSortDirection.Ascending ) );
            }
            else if( _lvSortColumn == column && _lvSortAdorner.Direction == ListSortDirection.Ascending )
            {
                _lvSortColumn = column;
                _lvSortAdorner = new ListViewSortAdorner( _lvSortColumn, ListSortDirection.Descending );
                AdornerLayer.GetAdornerLayer( _lvSortColumn ).Add( _lvSortAdorner );
                uiSequenceList.Items.SortDescriptions.Add( new SortDescription( column.Tag.ToString( ), ListSortDirection.Descending ) );
            }
            else
            {
                _lvSortColumn = null;
                _lvSortAdorner = null;
            }
        }


        private void DatasetReadFromFile( bool clearCurrent )
        {
            OpenFileDialog dlg = new OpenFileDialog( );
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.DefaultExt = ".fasta";
            dlg.DereferenceLinks = true;
            dlg.Filter = "FASTA files (*.fasta,*.fna,*.ffn,*.faa,*.frn)|*.fasta;*.fna;*.ffn;*.faa;*.frn|All files (*.*)|*.*";
            dlg.InitialDirectory = Environment.CurrentDirectory;
            dlg.Title = "Select a fasta file to open. . .";
            if( dlg.ShowDialog( ) == true )
            {
                if( clearCurrent )
                {
                    Sequences.Clear( );
                }
                if( FASTA.Read( dlg.FileName, Sequences ) )
                {
                    FileName = dlg.FileName;
                    ShowStatus( "Successfully loaded the dataset from " + dlg.FileName );
                }
                else
                {
                    FileName = null;
                    ShowStatus( "Failed to load the dataset from " + dlg.FileName );
                }
                UpdateWindowCaption( false );
            }
        }


        private void MouseWheelOnOffset( object sender, MouseWheelEventArgs e )
        {
            if( e.Delta != 0 && sender is TextBox textBox )
            {
                string propertyName = textBox.Tag.ToString( );
                PropertyInfo propertyInfo = Configuration.GetType( ).GetProperty( propertyName );
                int modifier = e.Delta > 0 ? 1 : -1;
                int value = (int)propertyInfo.GetValue( Configuration );
                if( (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift )
                {
                    modifier *= 5;
                }
                propertyInfo.SetValue( Configuration, value + modifier );
            }
        }


        private void SequenceList_KeyDown( object sender, KeyEventArgs e )
        {
            if( sender is ListView uiSeqList )
            {
                if( e.Key == Key.Space )
                {
                    bool selectAll = false;
                    foreach( Sequence seq in uiSeqList.SelectedItems )
                    {
                        if( !seq.Checked )
                        {
                            selectAll = true;
                        }
                    }
                    foreach( Sequence seq in uiSeqList.SelectedItems )
                    {
                        seq.Checked = selectAll;
                    }
                    e.Handled = true;
                }
                else if( e.Key == Key.Delete || e.Key == Key.Back )
                {
                    ClickMenuItemRemove( sender, e );
                    e.Handled = true;
                }
            }
        }


        private void ShowStatus( string text )
        {
            _statusTextReset.Interval = TimeSpan.FromMilliseconds( Configuration.StatusTextDuration );
            _statusTextReset.Start( );
            StatusText = text;
            NotifyPropertyChanged( "StatusText" );
        }


        private void NotifyPropertyChanged( [CallerMemberName] string name = "" )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( name ) );
        }


        private void PrintLog( string format, object[] args )
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


        private void UpdateWindowCaption( bool edited )
        {
            StringBuilder sb = new StringBuilder( );
            sb.Append( App.Name );
            if( !string.IsNullOrWhiteSpace( FileName ) )
            {
                sb.Append( " - " );
                sb.Append( FileName );
                if( edited )
                {
                    sb.Append( " *" );
                }
            }
            Title = sb.ToString( );
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
            UpdateWindowCaption( false );
        }

    }
}
