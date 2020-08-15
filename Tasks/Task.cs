using FASTASelector.FASTA;
using System.Collections.Generic;

namespace FASTASelector.Tasks
{
    internal abstract class Task
    {
        public List<Sequence> WorkSequences
        {
            get;
            private set;
        } = new List<Sequence>( );


        public abstract void Redo( IList<Sequence> destination );


        public abstract void Undo( IList<Sequence> destination );

    }
}
