using System.Runtime.Serialization;
using System.Text;

namespace FASTASelector.Configurations
{
    [DataContract]
    internal sealed class MetadataViewConfig : ConfigCategoryBase
    {
        private Encoding _fileEncoding = Encoding.UTF8;


        public MetadataViewConfig( )
        {
        }


        public Encoding FileEncoding
        {
            get { return _fileEncoding; }
            private set
            {
                _fileEncoding = value ?? Encoding.UTF8;
                NotifyPropertyChanged( );
            }
        }


        [DataMember( Name = "FileEncoding" )]
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

    }
}
