using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace FASTASelector.Data
{
    internal sealed class Metadata : INotifyPropertyChanged
    {
        private Dictionary<string, string> _data = null;
        private bool _checked = false;


        public Metadata( )
        {
            _data = new Dictionary<string, string>( );
        }


        public Metadata( Metadata other )
        {
            _data = new Dictionary<string, string>( other._data );
            _checked = other._checked;
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public string this[string key]
        {
            get
            {
                if( _data.ContainsKey( key ) )
                {
                    return _data[key];
                }
                return string.Empty;
            }
            set
            {
                if( _data.ContainsKey( key ) )
                {
                    _data[key] = value ?? string.Empty;
                }
                else
                {
                    _data.Add( key, value ?? string.Empty );
                }
                NotifyPropertyChanged( "[" + key + "]" );
            }
        }


        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                NotifyPropertyChanged( );
            }
        }


        public bool Contains( Metadata other )
        {
            if( other._data.Count > 0 )
            {
                foreach( string key in other._data.Keys )
                {
                    if( !_data.ContainsKey( key ) || !_data[key].Equals( other._data[key], StringComparison.CurrentCultureIgnoreCase ) )
                    {   // key is not found in this metadata or value is not the same
                        return false;
                    }
                }
                return true;
            }
            return false;
        }


        public void Clear( )
        {
            _data.Clear( );
        }


        public override string ToString( )
        {
            StringBuilder sb = new StringBuilder( );
            Dictionary<string, string>.Enumerator enumerator = _data.GetEnumerator( );
            if( enumerator.MoveNext( ) )
            {
                sb.Append( enumerator.Current.Value );
                while( enumerator.MoveNext( ) )
                {
                    sb.Append( Sequence.HEADER_DELIMITER );
                    sb.Append( enumerator.Current.Value );
                }
            }
            return sb.ToString( );
        }


        private void NotifyPropertyChanged( [CallerMemberName] string name = "" )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( name ) );
        }

    }
}
