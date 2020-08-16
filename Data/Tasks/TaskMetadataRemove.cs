using System.Collections.Generic;
using System.Diagnostics;

namespace FASTASelector.Data
{
    partial class Controller
    {
        private sealed class TaskMetadataRemove : Task
        {
            private List<int> _targetIndices = new List<int>( );
            private List<Metadata> _targetItems = new List<Metadata>( );


            public override bool Initialize( Controller controller )
            {
                _targetIndices.Clear( );
                _targetItems.Clear( );
                for( int i = controller.Metadata.Count - 1; i >= 0; --i )
                {
                    if( controller.Metadata[i].Checked )
                    {
                        _targetIndices.Add( i );
                        _targetItems.Add( controller.Metadata[i] );
                        controller.Metadata.RemoveAt( i );
                    }
                }
                if( _targetItems.Count > 0 )
                {
                    App.Log( App.DEFAULT, "Removed {0} metadata", _targetIndices.Count );
                }
                return _targetItems.Count > 0;
            }


            public override void Redo( Controller controller )
            {
                Debug.Assert( _targetIndices.Count == _targetItems.Count );
                for( int i = 0; i < _targetIndices.Count; ++i )
                {
                    controller.Metadata.RemoveAt( _targetIndices[i] );
                }
                App.Log( App.DEFAULT, "Removed {0} metadata", _targetIndices.Count );
            }


            public override void Undo( Controller controller )
            {
                Debug.Assert( _targetIndices.Count == _targetItems.Count );
                for( int i = _targetIndices.Count - 1; i >= 0; --i )
                {
                    controller.Metadata.Insert( _targetIndices[i], _targetItems[i] );
                }
                App.Log( App.DEFAULT, "Restored {0} metadata", _targetIndices.Count );
            }

        }
    }
}
