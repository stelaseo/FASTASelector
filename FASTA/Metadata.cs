using System;
using System.Collections.Generic;
using System.Text;

namespace FASTASelector.FASTA
{
    internal sealed class Metadata
    {
        private Dictionary<string, string> _data = new Dictionary<string, string>( );


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
                    sb.Append( '|' );
                    sb.Append( enumerator.Current.Value );
                }
            }
            return sb.ToString( );
        }

    }
}
