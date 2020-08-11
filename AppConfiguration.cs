using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace FASTASelector
{
    [DataContract]
    internal sealed class AppConfiguration : INotifyPropertyChanged
    {
        private bool _appConfigUpdated = false;
        private int _searchBeginOffset = 0;
        private int _searchEndOffset = 0;
        private string _searchText = string.Empty;
        private double _statusTextDuration = 5000.0;
        private double _windowPositionX = 0.0;
        private double _windowPositionY = 0.0;
        private double _windowSizeWidth = 816.0;
        private double _windowSizeHeight = 519.0;


        public AppConfiguration( )
        {
        }


        public event PropertyChangedEventHandler PropertyChanged;


        [DataMember]
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


        [DataMember]
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


        [DataMember]
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


        [DataMember]
        public string SequenceViewFontFamily
        {
            get { return SequenceViewer.ViewFontFamily?.ToString( ); }
            set
            {
                SequenceViewer.ViewFontFamily = value;
                _appConfigUpdated = true;
                NotifyPropertyChanged( );
            }
        }


        [DataMember]
        public double SequenceViewFontSize
        {
            get { return SequenceViewer.ViewFontSize; }
            set
            {
                SequenceViewer.ViewFontSize = value;
                _appConfigUpdated = true;
                NotifyPropertyChanged( );
            }
        }


        [DataMember]
        public double SequenceViewLineHeight
        {
            get { return SequenceViewer.ViewLineHeight; }
            set
            {
                SequenceViewer.ViewLineHeight = value;
                _appConfigUpdated = true;
                NotifyPropertyChanged( );
            }
        }


        [DataMember]
        public int SequenceViewWidth
        {
            get { return SequenceViewer.ViewWidth; }
            set
            {
                SequenceViewer.ViewWidth = value;
                _appConfigUpdated = true;
                NotifyPropertyChanged( );
            }
        }


        [DataMember]
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


        [DataMember]
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


        [DataMember]
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


        [DataMember]
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


        [DataMember]
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
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer( typeof( AppConfiguration ) );
                    AppConfiguration config = (AppConfiguration)jsonSerializer.ReadObject( stream );
                    config.CopyTo( this );
                }
                else
                {
                    Write( filename, true );
                }
            }
            catch( Exception ex )
            {
                App.Log( ex.Message );
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
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer( typeof( AppConfiguration ) );
                    jsonSerializer.WriteObject( stream, this );
                }
                catch( Exception ex )
                {
                    App.Log( ex.Message );
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


        private void CopyTo( AppConfiguration other )
        {
            other.SearchBeginOffset = SearchBeginOffset;
            other.SearchEndOffset = SearchEndOffset;
            other.SearchText = SearchText;
            other.SequenceViewFontFamily = SequenceViewFontFamily;
            other.SequenceViewFontSize = SequenceViewFontSize;
            other.SequenceViewLineHeight = SequenceViewLineHeight;
            other.SequenceViewWidth = SequenceViewWidth;
            other.StatusTextDuration = StatusTextDuration;
            other.WindowPositionX = WindowPositionX;
            other.WindowPositionY = WindowPositionY;
            other.WindowSizeWidth = WindowSizeWidth;
            other.WindowSizeHeight = WindowSizeHeight;
        }


        private void NotifyPropertyChanged( [CallerMemberName] string name = "" )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( name ) );
        }

    }
}
