using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace FASTASelector.Data
{
    internal sealed class SequenceCollection : ObservableCollection<Sequence>
    {
        private string _filename = string.Empty;


        public SequenceCollection( )
        {
        }


        public SequenceCollection( SequenceCollection other )
        {
            FileName = other.FileName;
            foreach( Sequence item in other )
            {
                Add( item );
            }
        }


        public string FileName
        {
            get { return _filename; }
            set
            {
                _filename = value ?? string.Empty;
                OnPropertyChanged( new PropertyChangedEventArgs( "FileName" ) );
            }
        }


        public bool Import( string filename )
        {
            StreamReader sr = null;
            bool success = true;
            try
            {
                sr = new StreamReader( filename, App.Configuration.SequenceView.FileEncoding );
                ReadProcessLines( sr );
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
                sr = new StreamReader( filename, App.Configuration.SequenceView.FileEncoding );
                ReadProcessLines( sr );
                FileName = filename;
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
                sw = new StreamWriter( filename, false, App.Configuration.SequenceView.FileEncoding );
                foreach( Sequence seq in this )
                {
                    sw.Write( '>' );
                    sw.Write( seq.RawHeader );
                    for( int i = 0; i < seq.Value.Length; ++i )
                    {
                        if( (i % App.Configuration.SequenceView.ViewWidth) == 0 )
                        {
                            sw.WriteLine( );
                        }
                        sw.Write( seq.Value[i] );
                    }
                    sw.WriteLine( );
                }
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


        private void ReadProcessLines( StreamReader sr )
        {
            StringBuilder sb = null;
            Sequence seq = null;
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
        }

    }
}
