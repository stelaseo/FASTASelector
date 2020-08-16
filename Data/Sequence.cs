using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FASTASelector.Data
{
    internal sealed class Sequence : INotifyPropertyChanged
    {
        internal const char HEADER_DELIMITER = '|';
        private bool _checked = false;
        private Metadata _metadata = null;
        private string _rawHeader = string.Empty;
        private string _value = string.Empty;


        public Sequence( )
        {
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                NotifyPropertyChanged( );
            }
        }


        public Metadata Header
        {
            get;
            private set;
        } = new Metadata( );


        /// <summary>
        /// Positions to highlight portions of the sequence data in SequenceViewer.
        /// </summary>
        /// <remarks>
        /// The list has to be in the ascending order without overlap.
        /// </remarks>
        public List<SequencePosition> Highlights
        {
            get;
            private set;
        } = new List<SequencePosition>( );


        public Metadata Metadata
        {
            get { return _metadata; }
            set
            {
                _metadata = value;
                NotifyPropertyChanged( );
            }
        }


        public string RawHeader
        {
            get { return _rawHeader; }
            set
            {
                _rawHeader = value ?? string.Empty;
                NotifyPropertyChanged( );
                ParseHeader( );
            }
        }


        public string Value
        {
            get { return _value; }
            set
            {
                _value = value ?? string.Empty;
                NotifyPropertyChanged( );
            }
        }


        public bool Highlight( string text, int beginOffset, int endOffset )
        {
            Highlights.Clear( );
            bool found = false;
            int index = Value.IndexOf( text, beginOffset, StringComparison.CurrentCultureIgnoreCase );
            while( index >= 0 && index + text.Length < Value.Length - endOffset )
            {
                found = true;
                Highlights.Add( new SequencePosition( index, index + text.Length ) );
                index = Value.IndexOf( text, index + text.Length, StringComparison.CurrentCultureIgnoreCase );
            }
            return found;
        }


        public void ParseHeader( )
        {
            string[] keys = App.Controller.SequenceHeaderKeys;
            Header.Clear( );
            if( keys != null && keys.Length > 0 )
            {
                string[] values = RawHeader.Split( new char[] { HEADER_DELIMITER }, keys.Length );
                for( int i = 0; i < keys.Length && i < values.Length; ++i )
                {
                    Header[keys[i]] = values[i];
                }
            }
            else
            {
                Header[string.Empty] = RawHeader;
            }
        }


        private void NotifyPropertyChanged( [CallerMemberName] string name = "" )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( name ) );
        }

    }
}
