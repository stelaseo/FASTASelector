using FASTASelector.FASTA;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace FASTASelector.UserInterface
{
    internal partial class MainWindow
    {
        private GridViewColumnHeader _lvSortColumn = null;
        private ListViewSortAdorner _lvSortAdorner = null;


        private void ClickLogClear( object sender, RoutedEventArgs e )
        {
            ShowStatus( "Clearing the log messages. . ." );
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
                    App.Log( App.ERROR, ex.Message );
                }
            }
        }


        private void ClickMetadataLink( object sender, RoutedEventArgs e )
        {
            if( sender is Button button )
            {
                if( button.Tag is Metadata metadata )
                {
                    uiMetadataList.SelectedItem = metadata;
                    uiMetadataList.ScrollIntoView( metadata );
                }
                else
                {
                    uiMetadataList.SelectedItem = null;
                }
            }
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
                App.Log( App.DEFAULT, "Found {0} sequence(s) which contain {1}", count, Configuration.SearchText );
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


        private void MenuLoadMetadata( object sender, RoutedEventArgs e )
        {
            OpenFileDialog dlg = new OpenFileDialog( );
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.DefaultExt = ".tsv";
            dlg.DereferenceLinks = true;
            dlg.Filter = "Metadata files (*.tsv)|*.tsv|All files (*.*)|*.*";
            dlg.Title = "Select a file to load metadata. . .";
            Utility.SetDialogFileName( dlg, Metadata.FileName );
            if( dlg.ShowDialog( ) == true )
            {
                if( Metadata.Read( dlg.FileName ) )
                {
                    ShowStatus( "Successfully loaded the metadata from " + dlg.FileName );
                    UpdateMetadataLinkWithSequences( );
                    UpdateMetadataListView( );
                    UpdateSequenceListView( );
                }
                else
                {
                    ShowStatus( "Failed to load the metadata from " + dlg.FileName );
                }
            }
        }


        private void MenuMergeMetadata( object sender, RoutedEventArgs e )
        {
            if( Metadata.Columns.Count <= 0 )
            {
                MenuLoadMetadata( sender, e );
            }
            else
            {
                OpenFileDialog dlg = new OpenFileDialog( );
                dlg.CheckFileExists = true;
                dlg.CheckPathExists = true;
                dlg.DefaultExt = ".tsv";
                dlg.DereferenceLinks = true;
                dlg.Filter = "Metadata files (*.tsv)|*.tsv|All files (*.*)|*.*";
                dlg.Title = "Select a file to merge metadata. . .";
                Utility.SetDialogFileName( dlg, Metadata.FileName );
                if( dlg.ShowDialog( ) == true )
                {
                    if( Metadata.Import( dlg.FileName ) )
                    {
                        ShowStatus( "Successfully merged the metadata from " + dlg.FileName );
                        UpdateMetadataLinkWithSequences( );
                        UpdateMetadataListView( );
                        UpdateSequenceListView( );
                    }
                    else
                    {
                        ShowStatus( "Failed to merge the metadata from " + dlg.FileName );
                    }
                }
            }
        }


        private void MenuSaveMetadata( object sender, RoutedEventArgs e )
        {
            if( string.IsNullOrWhiteSpace( Metadata.FileName ) )
            {
                MenuSaveMetadataAs( sender, e );
            }
            else if( Metadata.Write( ) )
            {
                ShowStatus( "Successfully saved the metadata to " + Metadata.FileName );
            }
            else
            {
                ShowStatus( "Failed to save the metadata" );
            }
        }


        private void MenuSaveMetadataAs( object sender, RoutedEventArgs e )
        {
            SaveFileDialog dlg = new SaveFileDialog( );
            dlg.CheckPathExists = true;
            dlg.DefaultExt = ".fasta";
            dlg.DereferenceLinks = true;
            dlg.Filter = "Metadata files (*.tsv)|*.tsv|All files (*.*)|*.*";
            dlg.Title = "Select a file to save the current metadata. . .";
            Utility.SetDialogFileName( dlg, Metadata.FileName );
            if( dlg.ShowDialog( ) == true )
            {
                if( Metadata.Write( dlg.FileName ) )
                {
                    ShowStatus( "Successfully saved the metadata to " + dlg.FileName );
                }
                else
                {
                    ShowStatus( "Failed to save the metadata to " + dlg.FileName );
                }
            }
        }


        private void MenuLoadSequences( object sender, RoutedEventArgs e )
        {
            OpenFileDialog dlg = new OpenFileDialog( );
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.DefaultExt = ".fasta";
            dlg.DereferenceLinks = true;
            dlg.Filter = "FASTA files (*.fasta,*.fna,*.ffn,*.faa,*.frn)|*.fasta;*.fna;*.ffn;*.faa;*.frn|All files (*.*)|*.*";
            dlg.Title = "Select a fasta file to open. . .";
            Utility.SetDialogFileName( dlg, Sequences.FileName );
            if( dlg.ShowDialog( ) == true )
            {
                if( Sequences.Read( dlg.FileName ) )
                {
                    // TODO, modify the task history buffer
                    ShowStatus( "Successfully loaded the sequence(s) from " + dlg.FileName );
                    UpdateMetadataLinkWithSequences( );
                }
                else
                {
                    ShowStatus( "Failed to load the sequence(s) from " + dlg.FileName );
                }
            }
        }


        private void MenuMergeSequences( object sender, RoutedEventArgs e )
        {
            OpenFileDialog dlg = new OpenFileDialog( );
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.DefaultExt = ".fasta";
            dlg.DereferenceLinks = true;
            dlg.Filter = "FASTA files (*.fasta,*.fna,*.ffn,*.faa,*.frn)|*.fasta;*.fna;*.ffn;*.faa;*.frn|All files (*.*)|*.*";
            dlg.Title = "Select a fasta file to open. . .";
            Utility.SetDialogFileName( dlg, Sequences.FileName );
            if( dlg.ShowDialog( ) == true )
            {
                if( Sequences.Import( dlg.FileName ) )
                {
                    // TODO, modify the task history buffer
                    ShowStatus( "Successfully imported the sequence(s) from " + dlg.FileName );
                    UpdateMetadataLinkWithSequences( );
                }
                else
                {
                    ShowStatus( "Failed to import the sequence(s) from " + dlg.FileName );
                }
            }
        }


        private void MenuSaveSequences( object sender, RoutedEventArgs e )
        {
            if( string.IsNullOrWhiteSpace( Sequences.FileName ) )
            {
                MenuSaveMetadataAs( sender, e );
            }
            else if( Sequences.Write( ) )
            {
                // TODO, modify the task history buffer
                ShowStatus( "Successfully saved the sequence(s) to " + Sequences.FileName );
            }
            else
            {
                ShowStatus( "Failed to save the sequence(s)" );
            }
        }


        private void MenuSaveSequencesAs( object sender, RoutedEventArgs e )
        {
            SaveFileDialog dlg = new SaveFileDialog( );
            dlg.CheckPathExists = true;
            dlg.DefaultExt = ".fasta";
            dlg.DereferenceLinks = true;
            dlg.Filter = "FASTA files (*.fasta,*.fna,*.ffn,*.faa,*.frn)|*.fasta;*.fna;*.ffn;*.faa;*.frn|All files (*.*)|*.*";
            dlg.Title = "Select a fasta file to save the current sequence(s). . .";
            Utility.SetDialogFileName( dlg, Sequences.FileName );
            if( dlg.ShowDialog( ) == true )
            {
                if( Sequences.Write( dlg.FileName ) )
                {
                    // TODO, modify the task history buffer
                    ShowStatus( "Successfully saved the sequence(s) to " + dlg.FileName );
                }
                else
                {
                    ShowStatus( "Failed to save the sequence(s) to " + dlg.FileName );
                }
            }
        }


        private void MenuExit( object sender, RoutedEventArgs e )
        {
            Close( );
        }


        private void MenuSequenceUndo( object sender, RoutedEventArgs e )
        {
            // TODO, implement
        }


        private void MenuSequenceRedo( object sender, RoutedEventArgs e )
        {
            // TODO, implement
        }


        private void MenuSelectAllSequences( object sender, RoutedEventArgs e )
        {
            foreach( Sequence seq in Sequences )
            {
                seq.Checked = true;
            }
            App.Log( App.VERBOSE, "Selected all sequences" );
        }


        private void MenuDeselectAllSequences( object sender, RoutedEventArgs e )
        {
            foreach( Sequence seq in Sequences )
            {
                seq.Checked = false;
            }
            App.Log( App.VERBOSE, "Deselected all sequences" );
        }


        private void MenuRemoveSelections( object sender, RoutedEventArgs e )
        {
            int count = 0;
            for( int i = Sequences.Count - 1; i >= 0; --i )
            {
                if( Sequences[i].Checked )
                {
                    App.Log( App.DEFAULT, "Remove sequence {0}", Sequences[i].RawHeader );
                    Sequences.RemoveAt( i );
                    count++;
                }
            }
            if( count > 0 )
            {
                ShowStatus( "Removed " + count + " sequence(s)" );
            }
            else
            {
                ShowStatus( "No sequence is selected" );
            }
            App.Log( App.DEFAULT, "Total of {0} sequence(s) have been removed from the list", count );
        }


        private void MenuViewOptions( object sender, RoutedEventArgs e )
        {
            OptionDialog dialog = new OptionDialog( );
            dialog.DataContext = App.Configuration;
            dialog.Owner = this;
            dialog.ShowDialog( );
            UpdateMetadataLinkWithSequences( );
            UpdateMetadataListView( );
            UpdateSequenceListView( );
            uiSequenceViewer.UpdateView( );
        }


        private void MenuViewAbout( object sender, RoutedEventArgs e )
        {
            AboutDialog dialog = new AboutDialog( );
            dialog.Owner = this;
            dialog.ShowDialog( );
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
                    MenuRemoveSelections( sender, e );
                    e.Handled = true;
                }
            }
        }

    }
}
