using System.Collections.Generic;

namespace FASTASelector.Data
{
    partial class Controller
    {
        private sealed class TaskSequenceSelectNone : Task
        {
            private HashSet<Sequence> _checked = new HashSet<Sequence>( );


            public override bool Initialize( Controller controller )
            {
                _checked.Clear( );
                if( controller.Sequences.Count > 0 )
                {
                    foreach( Sequence seq in controller.Sequences )
                    {
                        if( seq.Checked )
                        {
                            _checked.Add( seq );
                        }
                        seq.Checked = false;
                    }
                    App.Log( App.DEFAULT, "Deselected all sequences" );
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
                }
                App.Log( App.DEFAULT, "Reverted selections in the sequence list to the state prior to deselecting all sequences" );
            }

        }
    }
}
