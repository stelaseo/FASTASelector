using System.Collections.Generic;
using System.Diagnostics;

namespace FASTASelector.Data
{
    partial class Controller
    {
        private sealed class TaskSequenceRemove : Task
        {
            private List<int> _targetIndices = new List<int>( );
            private List<Sequence> _targetItems = new List<Sequence>( );


            public override bool Initialize( Controller controller )
            {
                _targetIndices.Clear( );
                _targetItems.Clear( );
                for( int i = controller.Sequences.Count - 1; i >= 0; --i )
                {
                    if( controller.Sequences[i].Checked )
                    {
                        _targetIndices.Add( i );
                        _targetItems.Add( controller.Sequences[i] );
                        controller.Sequences.RemoveAt( i );
                    }
                }
                if( _targetItems.Count > 0 )
                {
                    App.Log( App.DEFAULT, "Removed {0} sequence(s)", _targetIndices.Count );
                }
                return _targetItems.Count > 0;
            }


            public override void Redo( Controller controller )
            {
                Debug.Assert( _targetIndices.Count == _targetItems.Count );
                for( int i = 0; i < _targetIndices.Count; ++i )
                {
                    controller.Sequences.RemoveAt( _targetIndices[i] );
                }
                App.Log( App.DEFAULT, "Removed {0} sequence(s)", _targetIndices.Count );
            }


            public override void Undo( Controller controller )
            {
                Debug.Assert( _targetIndices.Count == _targetItems.Count );
                for( int i = _targetIndices.Count - 1; i >= 0; --i )
                {
                    controller.Sequences.Insert( _targetIndices[i], _targetItems[i] );
                }
                App.Log( App.DEFAULT, "Restored {0} sequence(s)", _targetIndices.Count );
            }

        }
    }
}
