using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace FASTASelector.UserInterface
{
    public class ListViewSortAdorner : Adorner
    {   // http://www.wpf-tutorial.com/listview-control/listview-how-to-column-sorting/
        private static Geometry ASCENDING_GEOMETRY = Geometry.Parse( "M 0 4 L 3.5 0 L 7 4 Z" );
        private static Geometry DESCENDING_GEOMETRY = Geometry.Parse( "M 0 0 L 3.5 4 L 7 0 Z" );


        public ListViewSortAdorner( UIElement element, ListSortDirection direction )
            : base( element )
        {
            Direction = direction;
        }


        public ListSortDirection Direction
        {
            get;
            private set;
        }


        protected override void OnRender( DrawingContext drawingContext )
        {
            base.OnRender( drawingContext );
            if( AdornedElement.RenderSize.Width >= 20 )
            {
                TranslateTransform transform = new TranslateTransform( AdornedElement.RenderSize.Width - 15, (AdornedElement.RenderSize.Height - 5) / 2 );
                Geometry geometry = Direction == ListSortDirection.Descending ? DESCENDING_GEOMETRY : ASCENDING_GEOMETRY;
                drawingContext.PushTransform( transform );
                drawingContext.DrawGeometry( Brushes.Black, null, geometry );
                drawingContext.Pop( );
            }
        }

    }
}
