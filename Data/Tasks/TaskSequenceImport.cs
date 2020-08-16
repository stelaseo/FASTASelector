using System.Diagnostics;

namespace FASTASelector.Data
{
    partial class Controller
    {
        private sealed class TaskSequenceImport : Task
        {
            private SequenceCollection _oldList = null;
            private SequenceCollection _newList = null;
            private string _filename = string.Empty;


            public string FileName
            {
                get { return _filename; }
                set { _filename = value ?? string.Empty; }
            }


            public override bool Initialize( Controller controller )
            {
                bool success = false;
                _oldList = controller.Sequences;
                _newList = new SequenceCollection( _oldList );
                if( _newList.Import( FileName ) )
                {
                    App.Log( App.DEFAULT, "Total of {0} sequence(s) have been imported from {1}", _newList.Count - _oldList.Count, FileName );
                    controller.Sequences = _newList;
                    success = true;
                }
                return success;
            }


            public override void Redo( Controller controller )
            {
                Debug.Assert( _oldList != null && _newList != null );
                App.Log( App.DEFAULT, "Total of {0} sequence(s) have been imported from {1}", _newList.Count - _oldList.Count, FileName );
                controller.Sequences = _newList;
            }


            public override void Undo( Controller controller )
            {
                App.Log( App.DEFAULT, "Reverted the sequence list to the state prior to importing {0}", FileName );
                controller.Sequences = _oldList;
            }

        }
    }
}
