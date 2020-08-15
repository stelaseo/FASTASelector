using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace FASTASelector.FASTA
{
    internal sealed class SequenceCollection : ObservableCollection<Sequence>
    {
        private string _filename = string.Empty;


        public string FileName
        {
            get { return _filename; }
            private set
            {
                _filename = value;
                OnPropertyChanged( new PropertyChangedEventArgs( "FileName" ) );
            }
        }


        public bool Import( string filename )
        {
            StreamReader sr = null;
            bool success = true;
            try
            {
                sr = new StreamReader( filename, App.Configuration.SequenceFileEncoding );
                int count = ReadProcessLines( sr );
                App.Log( App.DEFAULT, "Total of {0} sequence(s) have been imported from {1}", count, filename );
            }
            catch( Exception ex )
            {
                App.Log( App.ERROR, ex.Message );
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


        public bool Read( string filename )
        {
            StreamReader sr = null;
            bool success = true;
            Clear( );
            try
            {
                sr = new StreamReader( filename, App.Configuration.SequenceFileEncoding );
                int count = ReadProcessLines( sr );
                FileName = filename;
                App.Log( App.DEFAULT, "Total of {0} sequence(s) have been read from {1}", count, filename );
            }
            catch( Exception ex )
            {
                App.Log( App.ERROR, ex.Message );
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


        public bool Write( string filename = null )
        {
            StreamWriter sw = null;
            bool success = true;
            filename = filename ?? FileName;
            try
            {
                sw = new StreamWriter( filename, false, App.Configuration.SequenceFileEncoding );
                foreach( Sequence seq in this )
                {
                    sw.Write( '>' );
                    sw.Write( seq.RawHeader );
                    for( int i = 0; i < seq.Value.Length; ++i )
                    {
                        if( (i % App.Configuration.SequenceViewWidth) == 0 )
                        {
                            sw.WriteLine( );
                        }
                        sw.Write( seq.Value[i] );
                    }
                    sw.WriteLine( );
                }
                App.Log( App.DEFAULT, "Total of {0} sequence(s) have been written to {1}", Count, filename );
                FileName = filename;
            }
            catch( Exception ex )
            {
                App.Log( App.ERROR, ex.Message );
                FileName = string.Empty;
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


        private int ReadProcessLines( StreamReader sr )
        {
            StringBuilder sb = null;
            Sequence seq = null;
            int count = 0;
            int lineNumber = 1;
            string line = sr.ReadLine( );
            while( line != null )
            {
                line = line.Trim( );
                if( !string.IsNullOrWhiteSpace( line ) )
                {
                    if( line.StartsWith( ";" ) )
                    {   // we found the beginning of comment(s)
                        App.Log( App.VERBOSE, "Line {0}: comment block.", lineNumber );
                        if( seq != null )
                        {   // the next sequence
                            seq.Value = sb.ToString( );
                            seq = null;
                            sb = null;
                        }
                    }
                    else if( line.StartsWith( ">" ) )
                    {   // we found the beginning of a sequence
                        App.Log( App.VERBOSE, "Line {0}: found a sequence {1}.", line );
                        if( seq != null )
                        {   // the next sequence
                            seq.Value = sb.ToString( );
                        }
                        count++;
                        sb = new StringBuilder( );
                        seq = new Sequence( );
                        seq.RawHeader = line.Substring( 1 ).Trim( );
                        Add( seq );
                    }
                    else if( sb != null )
                    {   // we are in the middle of appending sequence data
                        sb.Append( line );
                    }
                }
                lineNumber++;
                line = sr.ReadLine( );
            }
            if( seq != null && sb != null )
            {   // the last sequence
                seq.Value = sb.ToString( );
            }
            return count;
        }

    }
}
