using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FASTASelector.UserInterface
{
    internal static class Factory
    {

        public static GridViewColumn CreateCheckBoxColumn( ColumnSizeCollection sizeCollection, string propertyPath, RoutedEventHandler clickHandler, HorizontalAlignment alignment = HorizontalAlignment.Center )
        {
            FrameworkElementFactory checkBox = new FrameworkElementFactory( typeof( CheckBox ) );
            checkBox.SetValue( CheckBox.MarginProperty, new Thickness( 2.0 ) );
            checkBox.SetValue( CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Stretch );
            checkBox.SetValue( CheckBox.HorizontalContentAlignmentProperty, alignment );
            checkBox.SetValue( CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center );
            checkBox.SetValue( CheckBox.IsCheckedProperty, new Binding( )
            {
                Path = new PropertyPath( propertyPath ),
                Mode = BindingMode.TwoWay,
            } );

            GridViewColumnHeader header = new GridViewColumnHeader( );
            header.Content = string.Empty;
            header.HorizontalAlignment = HorizontalAlignment.Stretch;
            header.HorizontalContentAlignment = alignment;
            header.Tag = propertyPath;
            if( clickHandler != null )
            {
                header.Click += clickHandler;
            }
            header.SizeChanged += ( s, ev ) => { sizeCollection.Set( propertyPath, ev.NewSize.Width ); };

            GridViewColumn column = new GridViewColumn( );
            column.CellTemplate = new DataTemplate( );
            column.CellTemplate.VisualTree = checkBox;
            column.Header = header;
            column.Width = sizeCollection.Get( propertyPath );
            return column;
        }


        public static GridViewColumn CreateLinkTextBlockColumn( ColumnSizeCollection sizeCollection, string headerContent, string propertyPath, RoutedEventHandler clickHandler, RoutedEventHandler linkClickHandler, HorizontalAlignment alignment = HorizontalAlignment.Left )
        {
            FrameworkElementFactory textBlock = new FrameworkElementFactory( typeof( TextBlock ) );
            textBlock.SetValue( TextBlock.MarginProperty, new Thickness( 2.0 ) );
            textBlock.SetValue( TextBlock.TextAlignmentProperty, ToTextAlignment( alignment ) );
            textBlock.SetValue( TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Stretch );
            textBlock.SetValue( TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center );
            textBlock.SetValue( TextBlock.TextProperty, new Binding( )
            {
                Path = new PropertyPath( propertyPath ),
                Mode = BindingMode.OneWay,
            } );
            textBlock.SetValue( TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis );

            FrameworkElementFactory button = new FrameworkElementFactory( typeof( Button ) );
            button.SetValue( Button.ContentProperty, "↗" );
            button.SetValue( Button.MarginProperty, new Thickness( 2.0 ) );
            button.SetValue( Button.HorizontalAlignmentProperty, HorizontalAlignment.Right );
            button.SetValue( Button.VerticalAlignmentProperty, VerticalAlignment.Center );
            button.SetValue( Button.TagProperty, new Binding( )
            {
                Path = new PropertyPath( propertyPath ),
                Mode = BindingMode.OneWay,
            } );
            button.AddHandler( Button.ClickEvent, linkClickHandler );

            FrameworkElementFactory grid = new FrameworkElementFactory( typeof( Grid ) );
            grid.SetValue( Grid.HorizontalAlignmentProperty, HorizontalAlignment.Stretch );
            grid.SetValue( Grid.VerticalAlignmentProperty, VerticalAlignment.Center );
            grid.AppendChild( textBlock );
            grid.AppendChild( button );

            GridViewColumnHeader header = new GridViewColumnHeader( );
            header.Content = headerContent;
            header.HorizontalAlignment = HorizontalAlignment.Stretch;
            header.HorizontalContentAlignment = alignment;
            header.Padding = new Thickness( 8.0, 0.0, 8.0, 0.0 );
            header.Tag = propertyPath;
            if( clickHandler != null )
            {
                header.Click += clickHandler;
            }
            header.SizeChanged += ( s, ev ) => { sizeCollection.Set( propertyPath, ev.NewSize.Width ); };

            GridViewColumn column = new GridViewColumn( );
            column.CellTemplate = new DataTemplate( );
            column.CellTemplate.VisualTree = grid;
            column.Header = header;
            column.Width = sizeCollection.Get( propertyPath );
            return column;
        }


        public static GridViewColumn CreateTextBlockColumn( ColumnSizeCollection sizeCollection, string headerContent, string propertyPath, RoutedEventHandler clickHandler, HorizontalAlignment alignment = HorizontalAlignment.Left )
        {
            FrameworkElementFactory textBlock = new FrameworkElementFactory( typeof( TextBlock ) );
            textBlock.SetValue( TextBlock.MarginProperty, new Thickness( 2.0 ) );
            textBlock.SetValue( TextBlock.TextAlignmentProperty, ToTextAlignment( alignment ) );
            textBlock.SetValue( TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Stretch );
            textBlock.SetValue( TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center );
            textBlock.SetValue( TextBlock.TextProperty, new Binding( )
            {
                Path = new PropertyPath( propertyPath ),
                Mode = BindingMode.OneWay,
            } );

            GridViewColumnHeader header = new GridViewColumnHeader( );
            header.Content = headerContent;
            header.HorizontalAlignment = HorizontalAlignment.Stretch;
            header.HorizontalContentAlignment = alignment;
            header.Padding = new Thickness( 8.0, 0.0, 8.0, 0.0 );
            header.Tag = propertyPath;
            if( clickHandler != null )
            {
                header.Click += clickHandler;
            }
            header.SizeChanged += ( s, ev ) => { sizeCollection.Set( propertyPath, ev.NewSize.Width ); };

            GridViewColumn column = new GridViewColumn( );
            column.CellTemplate = new DataTemplate( );
            column.CellTemplate.VisualTree = textBlock;
            column.Header = header;
            column.Width = sizeCollection.Get( propertyPath );
            return column;
        }


        private static TextAlignment ToTextAlignment( HorizontalAlignment horizontalAlignment )
        {
            switch( horizontalAlignment )
            {
            case HorizontalAlignment.Left:
            default:
                return TextAlignment.Left;
            case HorizontalAlignment.Right:
                return TextAlignment.Right;
            case HorizontalAlignment.Center:
                return TextAlignment.Center;
            case HorizontalAlignment.Stretch:
                return TextAlignment.Justify;
            }
        }

    }
}
