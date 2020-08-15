using System;
using System.Collections.Generic;

namespace FASTASelector
{
    internal sealed class ColumnSizeCollection
    {
        private const double DEFAULT_SIZE = 80.0;
        private Dictionary<string, double> _sizes = new Dictionary<string, double>( );


        public event EventHandler<string> CollectionChanged;


        public double Get( string name )
        {
            name = (name ?? string.Empty).ToLower( );
            if( _sizes.ContainsKey( name ) )
            {
                return _sizes[name];
            }
            return DEFAULT_SIZE;
        }


        public void Set( string name, double value )
        {
            name = (name ?? string.Empty).ToLower( );
            if( _sizes.ContainsKey( name ) )
            {
                _sizes[name] = value;
            }
            else
            {
                _sizes.Add( name, value );
            }
            CollectionChanged?.Invoke( this, name );
        }


        public void FromDictionary( Dictionary<string, double> other )
        {
            _sizes = new Dictionary<string, double>( other );
            CollectionChanged?.Invoke( this, string.Empty );
        }


        public Dictionary<string, double> ToDictionary( )
        {
            return new Dictionary<string, double>( _sizes );
        }

    }
}
