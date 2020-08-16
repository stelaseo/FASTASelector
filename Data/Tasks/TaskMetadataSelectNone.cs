﻿using System.Collections.Generic;

namespace FASTASelector.Data
{
    partial class Controller
    {
        private sealed class TaskMetadataSelectNone : Task
        {
            private HashSet<Metadata> _checked = new HashSet<Metadata>( );


            public override bool Initialize( Controller controller )
            {
                _checked.Clear( );
                if( controller.Metadata.Count > 0 )
                {
                    foreach( Metadata item in controller.Metadata )
                    {
                        if( item.Checked )
                        {
                            _checked.Add( item );
                        }
                        item.Checked = false;
                    }
                    App.Log( App.DEFAULT, "Deselected all metadata entries" );
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
                foreach( Metadata item in controller.Metadata )
                {
                    item.Checked = _checked.Contains( item );
                }
                App.Log( App.DEFAULT, "Reverted selections in the metadata list to the state prior to deselecting all metadata entries" );
            }

        }
    }
}
