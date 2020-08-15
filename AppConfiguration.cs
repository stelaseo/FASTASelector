using FASTASelector.FASTA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Media;

namespace FASTASelector
{
    internal sealed class AppConfiguration : INotifyPropertyChanged
    {
        private const int DEFAULT_SEQUENCE_VIEW_WIDTH = 80;
        private const int MIN_SEQUENCE_VIEW_WIDTH = 50;
        private bool _appConfigUpdated = false;
        private Encoding _metadataFileEncoding = Encoding.UTF8;
        private int _searchBeginOffset = 0;
        private int _searchEndOffset = 0;
        private string _searchText = string.Empty;
        private Encoding _seqFileEncoding = Encoding.ASCII;
        private string _seqHeaderFormat = string.Empty;
        private FontFamily _seqViewFontFamily = new FontFamily( "Consolas" );
        private double _seqViewFontSize = 12.0;
        private double _seqViewLineHeight = 4.0;
        private int _seqViewWidth = DEFAULT_SEQUENCE_VIEW_WIDTH;
        private double _statusTextDuration = 5000.0;
        private double _windowPositionX = 0.0;
        private double _windowPositionY = 0.0;
        private double _windowSizeWidth = 816.0;
        private double _windowSizeHeight = 639.0;


        public AppConfiguration( )
        {
            SequenceHeaderFormat = "Virus name" + Sequence.HEADER_DELIMITER + "Accession ID" + Sequence.HEADER_DELIMITER + "Collection date";
            UIMetadataListColumnSize = new ColumnSizeCollection( );
            UIMetadataListColumnSize.CollectionChanged += ( s, ev ) => { _appConfigUpdated = true; };
            UISequenceListColumnSize = new ColumnSizeCollection( );
            UISequenceListColumnSize.Set( "Checked", 34.0 );
            UISequenceListColumnSize.Set( "Length", 56.0 );
            UISequenceListColumnSize.Set( "Metadata", 120.0 );
            UISequenceListColumnSize.CollectionChanged += ( s, ev ) => { _appConfigUpdated = true; };
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public Encoding MetadataFileEncoding
        {
            get { return _metadataFileEncoding; }
            private set
            {
                _metadataFileEncoding = value ?? Encoding.UTF8;
                NotifyPropertyChanged( );
            }
        }


        public string MetadataFileEncodingName
        {
            get { return Utility.EncodingToString( MetadataFileEncoding ); }
            set
            {
                MetadataFileEncoding = Utility.StringToEncoding( value ?? string.Empty );
                _appConfigUpdated = true;
                NotifyPropertyChanged( );
            }
        }


        public int SearchBeginOffset
        {
            get { return _searchBeginOffset; }
            set
            {
                _searchBeginOffset = Math.Max( 0, value );
                _appConfigUpdated = true;
                NotifyPropertyChanged( );
            }
        }


        public int SearchEndOffset
        {
            get { return _searchEndOffset; }
            set
            {
                _searchEndOffset = Math.Max( 0, value );
                _appConfigUpdated = true;
                NotifyPropertyChanged( );
            }
        }


        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value ?? string.Empty;
                _appConfigUpdated = true;
                NotifyPropertyChanged( );
            }
        }


        public Encoding SequenceFileEncoding
        {
            get { return _seqFileEncoding; }
            private set
            {
                _seqFileEncoding = value ?? Encoding.ASCII;
                NotifyPropertyChanged( );
            }
        }


        public string SequenceFileEncodingName
        {
            get { return Utility.EncodingToString( SequenceFileEncoding ); }
            set
            {
                SequenceFileEncoding = Utility.StringToEncoding( value ?? string.Empty );
                _appConfigUpdated = true;
                NotifyPropertyChanged( );
            }
        }


        public string SequenceHeaderFormat
        {
            get { return _seqHeaderFormat; }
            set
            {
                _seqHeaderFormat = value ?? string.Empty;
                string[] tokens = _seqHeaderFormat.Split( Sequence.HEADER_DELIMITER );
                SequenceHeaders = new string[tokens.Length];
                for( int i = 0; i < tokens.Length; ++i )
                {   // make sure internal keys are in lower case
                    SequenceHeaders[i] = tokens[i].ToLower( );
                }
                _appConfigUpdated = true;
                NotifyPropertyChanged( );
                NotifyPropertyChanged( "SequenceHeaders" );
            }
        }


        public string[] SequenceHeaders
        {
            get;
            private set;
        }


        public FontFamily SequenceViewFontFamily
        {
            get { return _seqViewFontFamily; }
        }


        public string SequenceViewFontFamilyName
        {
            get { return _seqViewFontFamily.ToString( ); }
            set
            {
                _seqViewFontFamily = Utility.CreateFontFamily( value );
                _appConfigUpdated = true;
                NotifyPropertyChanged( );
                NotifyPropertyChanged( "SequenceViewFontFamily" );
            }
        }


        public double SequenceViewFontSize
        {
            get { return _seqViewFontSize; }
            set
            {
                _seqViewFontSize = Math.Max( 1.0, value );
                _appConfigUpdated = true;
                NotifyPropertyChanged( );
            }
        }


        public double SequenceViewLineHeight
        {
            get { return _seqViewLineHeight; }
            set
            {
                _seqViewLineHeight = Math.Max( 0.0, value );
                _appConfigUpdated = true;
                NotifyPropertyChanged( );
            }
        }


        public int SequenceViewWidth
        {
            get { return _seqViewWidth; }
            set
            {
                _seqViewWidth = Math.Max( MIN_SEQUENCE_VIEW_WIDTH, value );
                _appConfigUpdated = true;
                NotifyPropertyChanged( );
            }
        }


        public double StatusTextDuration
        {
            get { return _statusTextDuration; }
            set
            {
                _statusTextDuration = Math.Max( 0.0, value );
                _appConfigUpdated = true;
                NotifyPropertyChanged( );
            }
        }


        public ColumnSizeCollection UIMetadataListColumnSize
        {
            get;
            private set;
        }


        public ColumnSizeCollection UISequenceListColumnSize
        {
            get;
            private set;
        }


        public double WindowPositionX
        {
            get { return _windowPositionX; }
            set
            {
                _windowPositionX = value;
                _appConfigUpdated = true;
                NotifyPropertyChanged( );
            }
        }


        public double WindowPositionY
        {
            get { return _windowPositionY; }
            set
            {
                _windowPositionY = value;
                _appConfigUpdated = true;
                NotifyPropertyChanged( );
            }
        }


        public double WindowSizeWidth
        {
            get { return _windowSizeWidth; }
            set
            {
                _windowSizeWidth = Math.Max( 1.0, value );
                _appConfigUpdated = true;
                NotifyPropertyChanged( );
            }
        }


        public double WindowSizeHeight
        {
            get { return _windowSizeHeight; }
            set
            {
                _windowSizeHeight = Math.Max( 1.0, value );
                _appConfigUpdated = true;
                NotifyPropertyChanged( );
            }
        }


        public void Read( string filename )
        {
            FileStream stream = null;
            try
            {
                if( File.Exists( filename ) )
                {
                    stream = File.OpenRead( filename );
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer( typeof( AppConfigJSON ) );
                    AppConfigJSON json = (AppConfigJSON)jsonSerializer.ReadObject( stream );
                    json.CopyTo( this );
                }
                else
                {
                    Write( filename, true );
                }
            }
            catch( Exception ex )
            {
                App.Log( App.ERROR, ex.Message );
            }
            finally
            {
                if( stream != null )
                {
                    stream.Close( );
                    stream.Dispose( );
                }
            }
            _appConfigUpdated = false;
        }


        public void Write( string filename, bool force = false )
        {
            if( force || _appConfigUpdated )
            {
                FileStream stream = null;
                try
                {
                    stream = File.Create( filename );
                    AppConfigJSON json = new AppConfigJSON( );
                    json.CopyFrom( this );
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer( typeof( AppConfigJSON ) );
                    jsonSerializer.WriteObject( stream, json );
                }
                catch( Exception ex )
                {
                    App.Log( App.ERROR, ex.Message );
                }
                finally
                {
                    if( stream != null )
                    {
                        stream.Close( );
                        stream.Dispose( );
                    }
                }
            }
            _appConfigUpdated = false;
        }


        private void NotifyPropertyChanged( [CallerMemberName] string name = "" )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( name ) );
        }


        [DataContract]
        private sealed class AppConfigJSON
        {

            [DataMember]
            public string MetadataFileEncoding { get; set; }


            [DataMember]
            public int SearchBeginOffset { get; set; }


            [DataMember]
            public int SearchEndOffset { get; set; }


            [DataMember]
            public string SearchText { get; set; }


            [DataMember]
            public string SequenceFileEncoding { get; set; }


            [DataMember]
            public string SequenceHeaderFormat { get; set; }


            [DataMember]
            public string SequenceViewFontFamily { get; set; }


            [DataMember]
            public double SequenceViewFontSize { get; set; }


            [DataMember]
            public double SequenceViewLineHeight { get; set; }


            [DataMember]
            public int SequenceViewWidth { get; set; }


            [DataMember]
            public double StatusTextDuration { get; set; }


            [DataMember]
            public Dictionary<string, double> UIMetadataListColumnWidths { get; set; }


            [DataMember]
            public Dictionary<string, double> UISequenceListColumnWidths { get; set; }


            [DataMember]
            public double WindowPositionX { get; set; }


            [DataMember]
            public double WindowPositionY { get; set; }


            [DataMember]
            public double WindowSizeWidth { get; set; }


            [DataMember]
            public double WindowSizeHeight { get; set; }


            public void CopyFrom( AppConfiguration other )
            {
                MetadataFileEncoding = other.MetadataFileEncodingName;
                SearchBeginOffset = other.SearchBeginOffset;
                SearchEndOffset = other.SearchEndOffset;
                SearchText = other.SearchText;
                SequenceFileEncoding = other.SequenceFileEncodingName;
                SequenceHeaderFormat = other.SequenceHeaderFormat;
                SequenceViewFontFamily = other.SequenceViewFontFamilyName;
                SequenceViewFontSize = other.SequenceViewFontSize;
                SequenceViewLineHeight = other.SequenceViewLineHeight;
                SequenceViewWidth = other.SequenceViewWidth;
                StatusTextDuration = other.StatusTextDuration;
                UIMetadataListColumnWidths = other.UIMetadataListColumnSize.ToDictionary( );
                UISequenceListColumnWidths = other.UISequenceListColumnSize.ToDictionary( );
                WindowPositionX = other.WindowPositionX;
                WindowPositionY = other.WindowPositionY;
                WindowSizeWidth = other.WindowSizeWidth;
                WindowSizeHeight = other.WindowSizeHeight;
            }


            public void CopyTo( AppConfiguration other )
            {
                other.MetadataFileEncodingName = MetadataFileEncoding;
                other.SearchBeginOffset = SearchBeginOffset;
                other.SearchEndOffset = SearchEndOffset;
                other.SearchText = SearchText;
                other.SequenceFileEncodingName = SequenceFileEncoding;
                other.SequenceHeaderFormat = SequenceHeaderFormat;
                other.SequenceViewFontFamilyName = SequenceViewFontFamily;
                other.SequenceViewFontSize = SequenceViewFontSize;
                other.SequenceViewLineHeight = SequenceViewLineHeight;
                other.SequenceViewWidth = SequenceViewWidth;
                other.StatusTextDuration = StatusTextDuration;
                other.UIMetadataListColumnSize.FromDictionary( UIMetadataListColumnWidths );
                other.UISequenceListColumnSize.FromDictionary( UISequenceListColumnWidths );
                other.WindowPositionX = WindowPositionX;
                other.WindowPositionY = WindowPositionY;
                other.WindowSizeWidth = WindowSizeWidth;
                other.WindowSizeHeight = WindowSizeHeight;
            }

        }

    }
}
