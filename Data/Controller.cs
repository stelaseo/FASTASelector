using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FASTASelector.Data
{
    internal sealed partial class Controller : INotifyPropertyChanged
    {
        private Dictionary<string, string> _sequenceHeaders = new Dictionary<string, string>( );
        private string[] _sequenceHeaderKeys = new string[0];
        private MetadataCollection _metadata = new MetadataCollection( );
        private SequenceCollection _sequences = new SequenceCollection( );
        private List<Task> _taskHistory = new List<Task>( );
        private int _taskIndex = 0;


        public Controller( )
        {
            UpdateSequenceHeader( );
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public bool HasTasksToRedo
        {
            get { return 0 <= _taskIndex && _taskIndex < _taskHistory.Count; }
        }


        public bool HasTasksToUndo
        {
            get { return 0 < _taskIndex && _taskIndex <= _taskHistory.Count; }
        }


        public MetadataCollection Metadata
        {
            get { return _metadata; }
            private set
            {
                _metadata = value;
                NotifyPropertyChanged( );
            }
        }


        public SequenceCollection Sequences
        {
            get { return _sequences; }
            private set
            {
                _sequences = value;
                NotifyPropertyChanged( );
            }
        }


        public string[] SequenceHeaderKeys
        {
            get { return _sequenceHeaderKeys; }
            private set
            {
                _sequenceHeaderKeys = value;
                NotifyPropertyChanged( );
            }
        }


        public string GetSequenceListColumnName( string key )
        {
            if( Metadata.Columns.ContainsKey( key ) )
            {
                return Metadata.Columns[key];
            }
            else if( _sequenceHeaders.ContainsKey( key ) )
            {
                return _sequenceHeaders[key];
            }
            return key;
        }


        public void PerformRedo( )
        {
            if( HasTasksToRedo )
            {
                _taskHistory[_taskIndex++].Redo( this );
                NotifyPropertyChanged( "HasTasksToRedo" );
                NotifyPropertyChanged( "HasTasksToUndo" );
            }
        }


        public void PerformUndo( )
        {
            if( HasTasksToUndo )
            {
                _taskHistory[--_taskIndex].Undo( this );
                NotifyPropertyChanged( "HasTasksToRedo" );
                NotifyPropertyChanged( "HasTasksToUndo" );
            }
        }


        public void UpdateSequenceHeader( )
        {
            string[] keys = App.Configuration.SequenceView.HeaderFormat.Split( Sequence.HEADER_DELIMITER );
            _sequenceHeaders.Clear( );
            for( int i = 0; i < keys.Length; ++i )
            {
                string value = keys[i].Trim( );
                keys[i] = keys[i].ToLower( );
                _sequenceHeaders.Add( keys[i], value );
            }
            SequenceHeaderKeys = keys;
            foreach( Sequence sequence in Sequences )
            {
                sequence.ParseHeader( );
                sequence.Metadata = Metadata.Find( sequence.Header );
            }
        }


        public bool MetadataImport( string filename )
        {
            TaskMetadataImport task = new TaskMetadataImport( );
            task.FileName = filename;
            if( task.Initialize( this ) )
            {
                AddToTaskBuffer( task );
                return true;
            }
            return false;
        }


        public bool MetadataRead( string filename )
        {
            TaskMetadataRead task = new TaskMetadataRead( );
            task.FileName = filename;
            if( task.Initialize( this ) )
            {
                AddToTaskBuffer( task );
                return true;
            }
            return false;
        }


        public bool MetadataRemoveSelections( )
        {
            TaskMetadataRemove task = new TaskMetadataRemove( );
            if( task.Initialize( this ) )
            {
                AddToTaskBuffer( task );
                return true;
            }
            return false;
        }


        public bool MetadataSelectAll( )
        {
            TaskMetadataSelectAll task = new TaskMetadataSelectAll( );
            if( task.Initialize( this ) )
            {
                AddToTaskBuffer( task );
                return true;
            }
            return false;
        }


        public bool MetadataSelectNone( )
        {
            TaskMetadataSelectNone task = new TaskMetadataSelectNone( );
            if( task.Initialize( this ) )
            {
                AddToTaskBuffer( task );
                return true;
            }
            return false;
        }


        public bool SequenceImport( string filename )
        {
            TaskSequenceImport task = new TaskSequenceImport( );
            task.FileName = filename;
            if( task.Initialize( this ) )
            {
                AddToTaskBuffer( task );
                return true;
            }
            return false;
        }


        public bool SequenceRead( string filename )
        {
            TaskSequenceRead task = new TaskSequenceRead( );
            task.FileName = filename;
            if( task.Initialize( this ) )
            {
                AddToTaskBuffer( task );
                return true;
            }
            return false;
        }


        public bool SequenceRemoveSelections( )
        {
            TaskSequenceRemove task = new TaskSequenceRemove( );
            if( task.Initialize( this ) )
            {
                AddToTaskBuffer( task );
                return true;
            }
            return false;
        }


        public bool SequenceSearch( )
        {
            TaskSequenceSearch task = new TaskSequenceSearch( );
            if( task.Initialize( this ) )
            {
                AddToTaskBuffer( task );
                return true;
            }
            return false;
        }


        public bool SequenceSearchClear( )
        {
            TaskSequenceSearchClear task = new TaskSequenceSearchClear( );
            if( task.Initialize( this ) )
            {
                AddToTaskBuffer( task );
                return true;
            }
            return false;
        }


        public bool SequenceSelectAll( )
        {
            TaskSequenceSelectAll task = new TaskSequenceSelectAll( );
            if( task.Initialize( this ) )
            {
                AddToTaskBuffer( task );
                return true;
            }
            return false;
        }


        public bool SequenceSelectNone( )
        {
            TaskSequenceSelectNone task = new TaskSequenceSelectNone( );
            if( task.Initialize( this ) )
            {
                AddToTaskBuffer( task );
                return true;
            }
            return false;
        }


        private void AddToTaskBuffer( Task task )
        {
            if( _taskIndex < _taskHistory.Count )
            {
                _taskHistory.RemoveRange( _taskIndex, _taskHistory.Count - _taskIndex );
            }
            _taskHistory.Add( task );
            _taskIndex = _taskHistory.Count;
            NotifyPropertyChanged( "HasTasksToRedo" );
            NotifyPropertyChanged( "HasTasksToUndo" );
        }


        private void NotifyPropertyChanged( [CallerMemberName] string name = "" )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( name ) );
        }


        private void UpdateMetadataLink( )
        {
            foreach( Sequence sequence in Sequences )
            {
                sequence.Metadata = Metadata.Find( sequence.Header );
            }
        }

    }
}
