using FASTASelector.Data;
using System;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Media;

namespace FASTASelector.Configurations
{
    [DataContract]
    internal sealed class SequenceViewConfig : ConfigCategoryBase
    {
        private const int DEFAULT_VIEW_WIDTH = 80;
        private const int MIN_VIEW_WIDTH = 50;
        private Encoding _fileEncoding = Encoding.ASCII;
        private double _fontSize = 12.0;
        private string _headerFormat = "Virus name" + Sequence.HEADER_DELIMITER + "Accession ID" + Sequence.HEADER_DELIMITER + "Collection date";
        private double _lineHeight = 4.0;
        private int _viewWidth = DEFAULT_VIEW_WIDTH;


        public SequenceViewConfig( )
        {
            FontFamily = new FontFamily( "Consolas" );
        }


        public event EventHandler HeaderFormatChanged;


        public Encoding FileEncoding
        {
            get { return _fileEncoding; }
            private set
            {
                _fileEncoding = value ?? Encoding.ASCII;
                NotifyPropertyChanged( );
            }
        }


        [DataMember(Name = "FileEncoding" )]
        public string FileEncodingName
        {
            get { return Utility.EncodingToString( FileEncoding ); }
            set
            {
                FileEncoding = Utility.StringToEncoding( value ?? string.Empty );
                IsConfigChanged = true;
                NotifyPropertyChanged( );
            }
        }


        public FontFamily FontFamily
        {
            get;
            private set;
        }


        [DataMember(Name = "FontFamily" )]
        public string FontFamilyName
        {
            get { return FontFamily.ToString( ); }
            set
            {
                FontFamily = Utility.CreateFontFamily( value );
                IsConfigChanged = true;
                NotifyPropertyChanged( );
                NotifyPropertyChanged( "FontFamily" );
            }
        }


        [DataMember]
        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = Math.Max( 1.0, value );
                IsConfigChanged = true;
                NotifyPropertyChanged( );
            }
        }


        [DataMember]
        public string HeaderFormat
        {
            get { return _headerFormat; }
            set
            {
                _headerFormat = value ?? string.Empty;
                IsConfigChanged = true;
                NotifyPropertyChanged( );
                HeaderFormatChanged?.Invoke( this, EventArgs.Empty );
            }
        }


        [DataMember]
        public double LineHeight
        {
            get { return _lineHeight; }
            set
            {
                _lineHeight = Math.Max( 0.0, value );
                IsConfigChanged = true;
                NotifyPropertyChanged( );
            }
        }


        [DataMember]
        public int ViewWidth
        {
            get { return _viewWidth; }
            set
            {
                _viewWidth = Math.Max( MIN_VIEW_WIDTH, value );
                IsConfigChanged = true;
                NotifyPropertyChanged( );
            }
        }

    }
}
