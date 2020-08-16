using FASTASelector.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace FASTASelector.UserInterface
{
    internal partial class MetadataListView : ListView
    {
        private GridViewColumnHeader _lvSortColumn = null;
        private ListViewSortAdorner _lvSortAdorner = null;


        public MetadataListView( )
        {
            InitializeComponent( );
        }


        public event RoutedEventHandler RemoveSelectedItems;


        public void UpdateListViewColumns( )
        {
            if( ItemsSource is MetadataCollection collection )
            {
                GridView gridView = new GridView( );
                gridView.Columns.Add( Factory.CreateCheckBoxColumn( App.Configuration.CoreUI.MetadataColumnSize, "Checked", ColumnHeaderClick ) );
                foreach( KeyValuePair<string, string> kv in collection.Columns )
                {
                    gridView.Columns.Add( Factory.CreateTextBlockColumn( App.Configuration.CoreUI.MetadataColumnSize, kv.Value, "[" + kv.Key + "]", ColumnHeaderClick ) );
                }
                View = gridView;
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
            if( sender is ListView uiMetadataListView )
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

    }
}
