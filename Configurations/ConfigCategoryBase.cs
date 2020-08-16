using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace FASTASelector.Configurations
{
    [DataContract]
    internal abstract class ConfigCategoryBase : INotifyPropertyChanged
    {
        private bool _isConfigChanged = false;


        public ConfigCategoryBase( )
        {
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public bool IsConfigChanged
        {
            get { return _isConfigChanged; }
            set
            {
                _isConfigChanged = value;
                NotifyPropertyChanged( );
            }
        }


        protected void NotifyPropertyChanged( [CallerMemberName] string name = "" )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( name ) );
        }

    }
}
