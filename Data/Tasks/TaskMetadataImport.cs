using System.Diagnostics;

namespace FASTASelector.Data
{
    partial class Controller
    {
        private sealed class TaskMetadataImport : Task
        {
            private MetadataCollection _oldList = null;
            private MetadataCollection _newList = null;
            private string _filename = string.Empty;


            public string FileName
            {
                get { return _filename; }
                set { _filename = value ?? string.Empty; }
            }


            public override bool Initialize( Controller controller )
            {
                bool success = false;
                _oldList = controller.Metadata;
                _newList = new MetadataCollection( _oldList );
                if( _newList.Import( FileName ) )
                {
                    App.Log( App.DEFAULT, "Metadata have been imported from {0}", FileName );
                    controller.Metadata = _newList;
                    success = true;
                }
                return success;
            }


            public override void Redo( Controller controller )
            {
                Debug.Assert( _oldList != null && _newList != null );
                App.Log( App.DEFAULT, "Metadata have been imported from {0}", FileName );
                controller.Metadata = _newList;
            }


            public override void Undo( Controller controller )
            {
                App.Log( App.DEFAULT, "Reverted the metadata list to the state prior to importing {0}", FileName );
                controller.Metadata = _oldList;
            }

        }
    }
}
