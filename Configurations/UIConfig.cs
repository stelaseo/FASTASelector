using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FASTASelector.Configurations
{
    [DataContract]
    internal sealed class UIConfig : ConfigCategoryBase
    {
        private int _searchBeginOffset = 0;
        private int _searchEndOffset = 0;
        private string _searchText = string.Empty;
        private double _statusTextDuration = 5000.0;
        private double _windowPositionX = 0.0;
        private double _windowPositionY = 0.0;
        private double _windowSizeWidth = 816.0;
        private double _windowSizeHeight = 639.0;


        public UIConfig( )
        {
            MetadataColumnSizeBase = null;
            SequenceColumnSizeBase = null;
        }


        public ColumnSizeCollection MetadataColumnSize
        {
            get;
            private set;
        }


        [DataMember( Name = "MetadataColumnSize" )]
        private Dictionary<string, double> MetadataColumnSizeBase
        {
            get { return MetadataColumnSize.ToDictionary( ); }
            set
            {
                if( MetadataColumnSize == null )
                {
                    MetadataColumnSize = new ColumnSizeCollection( );
                    MetadataColumnSize.Set( "Checked", 34.0 );
                    MetadataColumnSize.CollectionChanged += ( s, ev ) => { IsConfigChanged = true; };
                }
                MetadataColumnSize.FromDictionary( value );
            }
        }


        public ColumnSizeCollection SequenceColumnSize
        {
            get;
            private set;
        }


        [DataMember( Name = "SequenceColumnSize" )]
        private Dictionary<string, double> SequenceColumnSizeBase
        {
            get { return SequenceColumnSize.ToDictionary( ); }
            set
            {
                if( SequenceColumnSize == null )
                {
                    SequenceColumnSize = new ColumnSizeCollection( );
                    SequenceColumnSize.Set( "Checked", 34.0 );
                    SequenceColumnSize.Set( "Length", 56.0 );
                    SequenceColumnSize.Set( "Metadata", 120.0 );
                    SequenceColumnSize.CollectionChanged += ( s, ev ) => { IsConfigChanged = true; };
                }
                SequenceColumnSize.FromDictionary( value );
            }
        }


        [DataMember]
        public int SearchBeginOffset
        {
            get { return _searchBeginOffset; }
            set
            {
                _searchBeginOffset = Math.Max( 0, value );
                IsConfigChanged = true;
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
                IsConfigChanged = true;
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
                IsConfigChanged = true;
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
                IsConfigChanged = true;
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
                IsConfigChanged = true;
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
                IsConfigChanged = true;
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
                IsConfigChanged = true;
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
                IsConfigChanged = true;
                NotifyPropertyChanged( );
            }
        }

    }
}
