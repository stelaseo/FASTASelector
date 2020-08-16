using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace FASTASelector.Data
{
    internal sealed class MetadataCollection : ObservableCollection<Metadata>
    {
        private const char DELIMITER = '\t';
        private string _filename = string.Empty;


        public MetadataCollection( )
        {
            Columns = new Dictionary<string, string>( );
        }


        public MetadataCollection( MetadataCollection other )
        {
            Columns = new Dictionary<string, string>( other.Columns );
            FileName = other.FileName;
            foreach( Metadata item in other )
            {
                Add( new Metadata( item ) );
            }
        }


        /// <summary>
        /// Column names in the metadata
        /// </summary>
        /// <remarks>
        /// The key in this dictionary is the lower case version of the value.
        /// This is to speed up our search while importing other metadata files.
        /// </remarks>
        public Dictionary<string, string> Columns
        {
            get;
            private set;
        }


        public string FileName
        {
            get { return _filename; }
            private set
            {
                _filename = value ?? string.Empty;
                OnPropertyChanged( new PropertyChangedEventArgs( "FileName" ) );
            }
        }


        public new void Clear( )
        {
            base.Clear( );
            Columns.Clear( );
        }


        /// <summary>
        /// Find metadata in the list
        /// </summary>
        /// <param name="other">Metadata to compare to items in the list</param>
        /// <returns>Found entry, if found; otherwise, null</returns>
        public Metadata Find( Metadata other )
        {
            foreach( Metadata entry in this )
            {
                if( entry.Contains( other ) )
                {
                    return entry;
                }
            }
            return null;
        }


        public bool Import( string filename )
        {
            StreamReader sr = null;
            bool success = true;
            try
            {
                sr = new StreamReader( filename, App.Configuration.MetadataView.FileEncoding );
                string line = sr.ReadLine( );
                if( line != null )
                {
                    string[] columns = line.Split( DELIMITER );
                    List<string> colsAddToExists = new List<string>( );
                    List<string> colsAddToImport = new List<string>( Columns.Keys );
                    Dictionary<string, int> colsTheSame = new Dictionary<string, int>( );
                    for( int i = 0; i < columns.Length; ++i )
                    {
                        string value = columns[i].Trim( );
                        columns[i] = columns[i].ToLower( );
                        if( Columns.ContainsKey( columns[i] ) )
                        {
                            colsAddToImport.Remove( columns[i] );
                            colsTheSame.Add( columns[i], i );
                        }
                        else
                        {
                            colsAddToExists.Add( columns[i] );
                            Columns.Add( columns[i], value );
                        }
                    }
                    foreach( Metadata entry in this )
                    {   // add new column(s) to the existing entries
                        foreach( string key in colsAddToExists )
                        {
                            entry[key] = string.Empty;
                        }
                    }
                    ReadProcessLines( sr, columns, colsAddToImport.ToArray( ), colsTheSame );
                }
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
                sr = new StreamReader( filename, App.Configuration.MetadataView.FileEncoding );
                string line = sr.ReadLine( );
                if( line != null )
                {
                    string[] columns = line.Split( DELIMITER );
                    for( int i = 0; i < columns.Length; ++i )
                    {
                        string value = columns[i].Trim( );
                        columns[i] = columns[i].ToLower( );
                        if( !Columns.ContainsKey( columns[i] ) )
                        {
                            Columns.Add( columns[i], value );
                        }
                    }
                    ReadProcessLines( sr, columns );
                }
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
                sw = new StreamWriter( filename, false, App.Configuration.MetadataView.FileEncoding );
                StringBuilder line = new StringBuilder( );
                List<string> columns = new List<string>( );
                foreach( KeyValuePair<string, string> kv in Columns )
                {
                    columns.Add( kv.Key );
                    line.Append( kv.Value );
                    line.Append( DELIMITER );
                }
                sw.Write( line.Remove( line.Length - 1, 1 ).ToString( ) );
                foreach( Metadata entry in this )
                {
                    sw.WriteLine( );
                    line = new StringBuilder( );
                    for( int i = 0; i < columns.Count; ++i )
                    {
                        line.Append( entry[columns[i]] );
                        line.Append( DELIMITER );
                    }
                    sw.Write( line.Remove( line.Length - 1, 1 ).ToString( ) );
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


        private void ReadProcessLines( StreamReader sr, string[] columns )
        {
            int lineNumber = 2;
            string line = sr.ReadLine( );
            while( line != null )
            {
                line = line.Trim( );
                if( !string.IsNullOrWhiteSpace( line ) )
                {
                    string[] values = line.Split( DELIMITER );
                    if( columns.Length != values.Length )
                    {
                        App.Log( App.WARNING, "Line {0}: different number of columns found.", lineNumber );
                    }

                    Metadata entry = new Metadata( );
                    int i;
                    for( i = 0; i < columns.Length && i < values.Length; ++i )
                    {
                        entry[columns[i]] = values[i];
                    }
                    for( ; i < columns.Length; ++i )
                    {   // fill the empty columns
                        entry[columns[i]] = string.Empty;
                    }

                    Add( entry );
                }
                lineNumber++;
                line = sr.ReadLine( );
            }
        }


        private void ReadProcessLines( StreamReader sr, string[] columns, string[] emptyColumns, Dictionary<string, int> sameColumns )
        {
            int lineNumber = 2;
            string line = sr.ReadLine( );
            while( line != null )
            {
                line = line.Trim( );
                if( !string.IsNullOrWhiteSpace( line ) )
                {
                    string[] values = line.Split( DELIMITER );
                    if( columns.Length != values.Length )
                    {
                        App.Log( App.WARNING, "Line {0}: different number of columns found.", lineNumber );
                    }

                    Metadata tmp = new Metadata( );
                    foreach( KeyValuePair<string, int> kv in sameColumns )
                    {   // add values with the same keys
                        tmp[kv.Key] = values[kv.Value];
                    }

                    Metadata entry = Find( tmp );
                    int i;
                    if( entry == null )
                    {
                        entry = tmp;
                        for( i = 0; i < emptyColumns.Length; ++i )
                        {   // fill the existing columns
                            entry[emptyColumns[i]] = string.Empty;
                        }
                    }
                    for( i = 0; i < columns.Length && i < values.Length; ++i )
                    {
                        entry[columns[i]] = values[i];
                    }
                    for( ; i < columns.Length; ++i )
                    {   // fill the empty columns
                        entry[columns[i]] = string.Empty;
                    }

                    Add( entry );
                }
                lineNumber++;
                line = sr.ReadLine( );
            }
        }

    }
}
