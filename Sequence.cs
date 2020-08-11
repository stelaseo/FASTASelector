using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FASTASelector
{
    internal sealed class Sequence : INotifyPropertyChanged
    {
        private bool _checked = false;
        private string _header = string.Empty;
        private string _value = string.Empty;


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


        public string Header
        {
            get { return _header; }
            set
            {
                _header = value ?? string.Empty;
                NotifyPropertyChanged( );
            }
        }


        public List<Tuple<int, int>> Highlights
        {   // the list has to be in the ascending order without overlap
            get;
            private set;
        } = new List<Tuple<int, int>>( );


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
            Checked = false;
            Highlights.Clear( );
            bool found = false;
            int index = Value.IndexOf( text, beginOffset, StringComparison.CurrentCultureIgnoreCase );
            while( index >= 0 && index + text.Length < Value.Length - endOffset )
            {
                found = true;
                Highlights.Add( new Tuple<int, int>( index, index + text.Length ) );
                index = Value.IndexOf( text, index + text.Length, StringComparison.CurrentCultureIgnoreCase );
            }
            Checked = found;
            return found;
        }


        private void NotifyPropertyChanged( [CallerMemberName] string name = "" )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( name ) );
        }

    }
}
