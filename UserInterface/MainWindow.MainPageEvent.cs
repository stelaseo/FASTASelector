using FASTASelector.Data;
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
            Controller.SequenceSearch( );
            uiSequenceViewer.UpdateView( );
        }


        private void ClickSearchClear( object sender, RoutedEventArgs e )
        {
            Controller.SequenceSearchClear( );
            uiSequenceViewer.UpdateView( );
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


        private void MetadataList_KeyDown( object sender, KeyEventArgs e )
        {
            if( sender is ListView uiMetadataListView )
            {
                switch( e.Key )
                {
                case Key.Delete:
                case Key.Back:
                    MenuRemoveSelectedMetadata( sender, e );
                    e.Handled = true;
                    break;
                case Key.Space:
                    {
                        bool selectAll = false;
                        foreach( Metadata item in uiMetadataListView.SelectedItems )
                        {
                            if( !item.Checked )
                            {
                                selectAll = true;
                            }
                        }
                        foreach( Metadata item in uiMetadataListView.SelectedItems )
                        {
                            item.Checked = selectAll;
                        }
                        e.Handled = true;
                    }
                    break;
                }
            }
        }


        private void SequenceList_KeyDown( object sender, KeyEventArgs e )
        {
            if( sender is ListView uiSequenceListView )
            {
                switch( e.Key )
                {
                case Key.Delete:
                case Key.Back:
                    MenuRemoveSelectedSequences( sender, e );
                    e.Handled = true;
                    break;
                case Key.Space:
                    {
                        bool selectAll = false;
                        foreach( Sequence seq in uiSequenceListView.SelectedItems )
                        {
                            if( !seq.Checked )
                            {
                                selectAll = true;
                            }
                        }
                        foreach( Sequence seq in uiSequenceListView.SelectedItems )
                        {
                            seq.Checked = selectAll;
                        }
                        e.Handled = true;
                    }
                    break;
                }
            }
        }

    }
}
