using System.Diagnostics;

namespace FASTASelector.Data
{
    internal struct SequencePosition
    {
        public SequencePosition( int begin, int end )
        {
            Debug.Assert( begin <= end );
            Begin = begin;
            End = end;
        }


        public int Begin
        {
            get;
            set;
        }


        public int End
        {
            get;
            set;
        }

    }
}
