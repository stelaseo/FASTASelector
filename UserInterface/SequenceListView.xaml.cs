using FASTASelector.Data;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace FASTASelector.UserInterface
{
    internal partial class SequenceListView : ListView
    {
        private GridViewColumnHeader _lvSortColumn = null;
        private ListViewSortAdorner _lvSortAdorner = null;


        public SequenceListView( )
        {
            InitializeComponent( );
        }


        public event RoutedEventHandler RemoveSelectedItems;


        public event EventHandler<Metadata> SelectMetadata;


        public void UpdateListViewColumns( )
        {
            if( DataContext is Controller controller && ItemsSource is SequenceCollection collection )
            {
                string[] keys = App.Controller.SequenceHeaderKeys;
                GridView gridView = new GridView( );
                gridView.Columns.Add( Factory.CreateCheckBoxColumn( App.Configuration.CoreUI.SequenceColumnSize, "Checked", ColumnHeaderClick ) );
                for( int i = 0; i < keys.Length; ++i )
                {
                    string columnName = controller.GetSequenceListColumnName( keys[i] );
                    gridView.Columns.Add( Factory.CreateTextBlockColumn( App.Configuration.CoreUI.SequenceColumnSize, columnName, "Header[" + keys[i] + "]", ColumnHeaderClick ) );
                }
                gridView.Columns.Add( Factory.CreateTextBlockColumn( App.Configuration.CoreUI.SequenceColumnSize, "Length", "Value.Length", ColumnHeaderClick, HorizontalAlignment.Center ) );
                gridView.Columns.Add( Factory.CreateLinkTextBlockColumn( App.Configuration.CoreUI.SequenceColumnSize, "Metadata", "Metadata", ColumnHeaderClick, ClickMetadataLink ) );
                View = gridView;
            }
        }


        private void ClickMetadataLink( object sender, RoutedEventArgs e )
        {
            if( sender is Button button )
            {
                SelectMetadata?.Invoke( this, button.Tag as Metadata );
            }
        }


        private void ColumnHeaderClick( object sender, RoutedEventArgs e )
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;
            if( _lvSortColumn != null )
            {
                AdornerLayer.GetAdornerLayer( _lvSortColumn ).Remove( _lvSortAdorner );
                Items.SortDescriptions.Clear( );
            }
            if( _lvSortColumn == null || _lvSortColumn != column )
            {
                _lvSortColumn = column;
                _lvSortAdorner = new ListViewSortAdorner( _lvSortColumn, ListSortDirection.Ascending );
                AdornerLayer.GetAdornerLayer( _lvSortColumn ).Add( _lvSortAdorner );
                Items.SortDescriptions.Add( new SortDescription( column.Tag.ToString( ), ListSortDirection.Ascending ) );
            }
            else if( _lvSortColumn == column && _lvSortAdorner.Direction == ListSortDirection.Ascending )
            {
                _lvSortColumn = column;
                _lvSortAdorner = new ListViewSortAdorner( _lvSortColumn, ListSortDirection.Descending );
                AdornerLayer.GetAdornerLayer( _lvSortColumn ).Add( _lvSortAdorner );
                Items.SortDescriptions.Add( new SortDescription( column.Tag.ToString( ), ListSortDirection.Descending ) );
            }
            else
            {
                _lvSortColumn = null;
                _lvSortAdorner = null;
            }
        }


        private void ListView_PreviewKeyDown( object sender, KeyEventArgs e )
        {
            if( sender is ListView uiSequenceListView )
            {
                switch( e.Key )
                {
                case Key.Delete:
                case Key.Back:
                    RemoveSelectedItems?.Invoke( this, e );
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
