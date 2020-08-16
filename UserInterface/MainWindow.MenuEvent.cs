using FASTASelector.Data;
using Microsoft.Win32;
using System.Windows;

namespace FASTASelector.UserInterface
{
    internal partial class MainWindow
    {

        private void MenuLoadMetadata( object sender, RoutedEventArgs e )
        {
            OpenFileDialog dlg = new OpenFileDialog( );
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.DefaultExt = ".tsv";
            dlg.DereferenceLinks = true;
            dlg.Filter = "Metadata files (*.tsv)|*.tsv|All files (*.*)|*.*";
            dlg.Title = "Select a file to load metadata. . .";
            Utility.SetDialogFileName( dlg, Controller.Metadata.FileName );
            if( dlg.ShowDialog( ) == true )
            {
                if( Controller.MetadataRead( dlg.FileName ) )
                {
                    ShowStatus( "Successfully loaded the metadata from " + dlg.FileName );
                    UpdateMetadataListView( );
                }
                else
                {
                    ShowStatus( "Failed to load the metadata from " + dlg.FileName );
                }
            }
        }


        private void MenuMergeMetadata( object sender, RoutedEventArgs e )
        {
            if( Controller.Metadata.Columns.Count <= 0 )
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
                Utility.SetDialogFileName( dlg, Controller.Metadata.FileName );
                if( dlg.ShowDialog( ) == true )
                {
                    if( Controller.MetadataImport( dlg.FileName ) )
                    {
                        ShowStatus( "Successfully merged the metadata from " + dlg.FileName );
                        UpdateMetadataListView( );
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
            if( string.IsNullOrWhiteSpace( Controller.Metadata.FileName ) )
            {
                MenuSaveMetadataAs( sender, e );
            }
            else if( Controller.Metadata.Write( ) )
            {
                App.Log( App.DEFAULT, "Total of {0} metadata have been written to {1}", Controller.Metadata.Count, Controller.Metadata.FileName );
                ShowStatus( "Successfully saved the metadata to " + Controller.Metadata.FileName );
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
            Utility.SetDialogFileName( dlg, Controller.Metadata.FileName );
            if( dlg.ShowDialog( ) == true )
            {
                if( Controller.Metadata.Write( dlg.FileName ) )
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
            Utility.SetDialogFileName( dlg, Controller.Sequences.FileName );
            if( dlg.ShowDialog( ) == true )
            {
                if( Controller.SequenceRead( dlg.FileName ) )
                {
                    ShowStatus( "Successfully loaded the sequence(s) from " + dlg.FileName );
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
            Utility.SetDialogFileName( dlg, Controller.Sequences.FileName );
            if( dlg.ShowDialog( ) == true )
            {
                if( Controller.SequenceImport( dlg.FileName ) )
                {
                    ShowStatus( "Successfully imported the sequence(s) from " + dlg.FileName );
                }
                else
                {
                    ShowStatus( "Failed to import the sequence(s) from " + dlg.FileName );
                }
            }
        }


        private void MenuSaveSequences( object sender, RoutedEventArgs e )
        {
            if( string.IsNullOrWhiteSpace( Controller.Sequences.FileName ) )
            {
                MenuSaveMetadataAs( sender, e );
            }
            else if( Controller.Sequences.Write( ) )
            {
                App.Log( App.DEFAULT, "Total of {0} sequence(s) have been written to {1}", Controller.Sequences.Count, Controller.Sequences.FileName );
                ShowStatus( "Successfully saved the sequence(s) to " + Controller.Sequences.FileName );
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
            Utility.SetDialogFileName( dlg, Controller.Sequences.FileName );
            if( dlg.ShowDialog( ) == true )
            {
                if( Controller.Sequences.Write( dlg.FileName ) )
                {
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


        private void MenuUndo( object sender, RoutedEventArgs e )
        {
            Controller.PerformUndo( );
        }


        private void MenuRedo( object sender, RoutedEventArgs e )
        {
            Controller.PerformRedo( );
        }


        private void MenuSelectAllMetadata( object sender, RoutedEventArgs e )
        {
            Controller.MetadataSelectAll( );
        }


        private void MenuDeselectAllMetadata( object sender, RoutedEventArgs e )
        {
            Controller.MetadataSelectNone( );
        }


        private void MenuRemoveSelectedMetadata( object sender, RoutedEventArgs e )
        {
            Controller.MetadataRemoveSelections( );
        }


        private void MenuSelectAllSequences( object sender, RoutedEventArgs e )
        {
            Controller.SequenceSelectAll( );
        }


        private void MenuDeselectAllSequences( object sender, RoutedEventArgs e )
        {
            Controller.SequenceSelectNone( );
        }


        private void MenuRemoveSelectedSequences( object sender, RoutedEventArgs e )
        {
            Controller.SequenceRemoveSelections( );
        }


        private void MenuViewOptions( object sender, RoutedEventArgs e )
        {
            OptionDialog dialog = new OptionDialog( );
            dialog.DataContext = App.Configuration;
            dialog.Owner = this;
            dialog.ShowDialog( );
            Controller.UpdateSequenceHeader( );
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

    }
}
