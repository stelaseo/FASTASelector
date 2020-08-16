using System.Collections.Generic;

namespace FASTASelector.Data
{
    partial class Controller
    {
        private sealed class TaskSequenceSearchClear : Task
        {
            private Dictionary<Sequence, List<SequencePosition>> _backup = new Dictionary<Sequence, List<SequencePosition>>( );
            private HashSet<Sequence> _checked = new HashSet<Sequence>( );
            private string _searchText = string.Empty;


            public override bool Initialize( Controller controller )
            {
                _backup.Clear( );
                _checked.Clear( );
                _searchText = App.Configuration.CoreUI.SearchText;
                if( !string.IsNullOrWhiteSpace( _searchText ) )
                {
                    foreach( Sequence seq in controller.Sequences )
                    {
                        _backup.Add( seq, new List<SequencePosition>( seq.Highlights ) );
                        if( seq.Checked )
                        {
                            _checked.Add( seq );
                        }
                        seq.Checked = false;
                    }
                    App.Log( App.DEFAULT, "Cleared the search term {0}", _searchText );
                    return true;
                }
                return false;
            }


            public override void Redo( Controller controller )
            {
                Initialize( controller );
            }


            public override void Undo( Controller controller )
            {
                foreach( Sequence seq in controller.Sequences )
                {
                    seq.Checked = _checked.Contains( seq );
                    seq.Highlights.Clear( );
                    foreach( SequencePosition pos in _backup[seq] )
                    {
                        seq.Highlights.Add( pos );
                    }
                }
                App.Configuration.CoreUI.SearchText = _searchText;
                App.Log( App.DEFAULT, "Restored the search term {0}", _searchText );
            }

        }
    }
}
