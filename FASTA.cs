using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FASTASelector
{
    internal static class FASTA
    {

        public static bool Read( string filename, IList<Sequence> dataset )
        {
            StreamReader sr = null;
            bool success = true;
            try
            {
                sr = new StreamReader( filename, Encoding.ASCII );
                StringBuilder sb = null;
                Sequence seq = null;
                int count = 0;
                string line = sr.ReadLine( );
                while( line != null )
                {
                    line = line.Trim( );
                    if( line.StartsWith( ";" ) )
                    {   // we found the beginning of comment(s)
                        if( seq != null )
                        {   // the next sequence
                            seq.Value = sb.ToString( );
                            seq = null;
                            sb = null;
                        }
                    }
                    else if( line.StartsWith( ">" ) )
                    {   // we found the beginning of a sequence
                        if( seq != null )
                        {   // the next sequence
                            seq.Value = sb.ToString( );
                        }
                        count++;
                        sb = new StringBuilder( );
                        seq = new Sequence( );
                        seq.Header = line.Substring( 1 ).Trim( );
                        dataset.Add( seq );
                    }
                    else if( sb != null )
                    {
                        sb.Append( line );
                    }
                    line = sr.ReadLine( );
                }
                if( seq != null )
                {   // the next sequence
                    seq.Value = sb.ToString( );
                }
                App.Log( "Total of {0} sequence(s) have been read from {1}", count, filename );
            }
            catch( Exception ex )
            {
                App.Log( ex.Message );
                success = false;
            }
            finally
            {
                if( sr != null )
                {
                    sr.Close( );
                    sr.Dispose( );
                }
            }
            return success;
        }


        public static bool Write( string filename, IList<Sequence> dataset )
        {
            StreamWriter sw = null;
            bool success = true;
            try
            {
                sw = new StreamWriter( filename, false, Encoding.ASCII );
                foreach( Sequence seq in dataset )
                {
                    sw.Write( '>' );
                    sw.Write( seq.Header );
                    for( int i = 0; i < seq.Value.Length; ++i )
                    {
                        if( i % SequenceViewer.ViewWidth == 0 )
                        {
                            sw.WriteLine( );
                        }
                        sw.Write( seq.Value[i] );
                    }
                    sw.WriteLine( );
                }
                App.Log( "Total of {0} sequence(s) have been written to {1}", dataset.Count, filename );
            }
            catch( Exception ex )
            {
                App.Log( ex.Message );
                success = false;
            }
            finally
            {
                if( sw != null )
                {
                    sw.Close( );
                    sw.Dispose( );
                }
            }
            return success;
        }

    }
}
