using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace FASTASelector.Configurations
{
    internal sealed class AppConfiguration : INotifyPropertyChanged
    {
        private UIConfig _coreUI = new UIConfig( );
        private MetadataViewConfig _metadataView = new MetadataViewConfig( );
        private SequenceViewConfig _sequenceView = new SequenceViewConfig( );


        public AppConfiguration( )
        {
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public UIConfig CoreUI
        {
            get { return _coreUI; }
            set
            {
                _coreUI = value ?? new UIConfig( );
                NotifyPropertyChanged( );
            }
        }


        public MetadataViewConfig MetadataView
        {
            get { return _metadataView; }
            private set
            {
                _metadataView = value ?? new MetadataViewConfig( );
                NotifyPropertyChanged( );
            }
        }


        public SequenceViewConfig SequenceView
        {
            get { return _sequenceView; }
            private set
            {
                _sequenceView = value ?? new SequenceViewConfig( );
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
            CoreUI.IsConfigChanged = false;
            MetadataView.IsConfigChanged = false;
            SequenceView.IsConfigChanged = false;
        }


        public void Write( string filename, bool force = false )
        {
            if( force || CoreUI.IsConfigChanged || MetadataView.IsConfigChanged || SequenceView.IsConfigChanged )
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
                CoreUI.IsConfigChanged = false;
                MetadataView.IsConfigChanged = false;
                SequenceView.IsConfigChanged = false;
            }
        }


        private void NotifyPropertyChanged( [CallerMemberName] string name = "" )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( name ) );
        }


        [DataContract]
        private sealed class AppConfigJSON
        {

            [DataMember]
            public UIConfig CoreUI { get; set; }


            [DataMember]
            public MetadataViewConfig MetadataView { get; set; }


            [DataMember]
            public SequenceViewConfig SequenceView { get; set; }


            public void CopyFrom( AppConfiguration other )
            {
                CoreUI = other.CoreUI;
                MetadataView = other.MetadataView;
                SequenceView = other.SequenceView;
            }


            public void CopyTo( AppConfiguration other )
            {
                other.CoreUI = CoreUI;
                other.MetadataView = MetadataView;
                other.SequenceView = SequenceView;
            }

        }

    }
}
