using System.Collections.Generic;

namespace FASTASelector.Data
{
    partial class Controller
    {
        private sealed class TaskSequenceSearch : Task
        {
            private Dictionary<Sequence, List<SequencePosition>> _backup = new Dictionary<Sequence, List<SequencePosition>>( );
            private HashSet<Sequence> _checked = new HashSet<Sequence>( );


            public int SearchBeginOffset
            {
                get { return App.Configuration.CoreUI.SearchBeginOffset; }
            }


            public int SearchEndOffset
            {
                get { return App.Configuration.CoreUI.SearchEndOffset; }
            }


            public string SearchText
            {
                get { return App.Configuration.CoreUI.SearchText; }
            }


            public override bool Initialize( Controller controller )
            {
                _backup.Clear( );
                _checked.Clear( );
                int count = 0;
                if( !string.IsNullOrWhiteSpace( SearchText ) )
                {
                    foreach( Sequence seq in controller.Sequences )
                    {
                        _backup.Add( seq, new List<SequencePosition>( seq.Highlights ) );
                        if( seq.Checked )
                        {
                            _checked.Add( seq );
                        }
                        if( seq.Highlight( SearchText, SearchBeginOffset, SearchEndOffset ) )
                        {
                            seq.Checked = true;
                            count++;
                        }
                        else
                        {
                            seq.Checked = false;
                        }
                    }
                    App.Log( App.DEFAULT, "Found and selected {0} sequence(s) which contain {1}", count, SearchText );
                }
                return count > 0;
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
                App.Log( App.DEFAULT, "Reverted selections in the sequence list to the state prior to searching {0}", SearchText );
            }

        }
    }
}
