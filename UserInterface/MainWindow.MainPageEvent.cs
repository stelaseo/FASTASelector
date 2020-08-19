using FASTASelector.Data;
using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FASTASelector.UserInterface
{
    internal partial class MainWindow
    {

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


        private void ClickMetadataLink( object sender, RoutedEventArgs e )
        {
            if( sender is Button button )
            {
                SelectMetadata( this, button.Tag as Metadata );
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


        private void SelectMetadata( object sender, Metadata e )
        {
            if( e != null )
            {
                uiMetadataList.SelectedItem = e;
                uiMetadataList.ScrollIntoView( e );
            }
            else
            {
                uiMetadataList.SelectedItem = null;
            }
        }


        private void SequenceListSelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            if( sender is ListView uiListView && uiListView.SelectedItem is Sequence sequence )
            {
                SelectMetadata( this, sequence.Metadata );
            }
        }

    }
}
