using FASTASelector.Configurations;
using FASTASelector.Data;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows;

namespace FASTASelector
{
    public delegate void LogHandler( int level, string format, params object[] args );

    public partial class App : Application
    {
        public const int ERROR = 0;
        public const int WARNING = 1;
        public const int DEFAULT = 2;
        public const int VERBOSE = 3;
        private static LogHandler _logHandler = DefaultLogHandler;


        public static LogHandler Log
        {
            get { return _logHandler; }
            set { _logHandler = value ?? DefaultLogHandler; }
        }


        public static int LogLevel
        {
            get;
            set;
        } = DEFAULT;


        internal static AppConfiguration Configuration
        {
            get;
            private set;
        } = new AppConfiguration( );


        internal static Controller Controller
        {
            get;
            private set;
        } = new Controller( );


        internal static string Name
        {
            get { return "FASTA Selector"; }
        }


        internal static string Version
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly( );
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo( assembly.Location );
                return fvi.FileVersion;
            }
        }


        private static void DefaultLogHandler( int level, string format, params object[] args )
        {
            if( level <= LogLevel )
            {
                StringBuilder sb = new StringBuilder( );
                sb.Append( '[' );
                sb.Append( DateTime.Now.ToString( "HH:mm" ) );
                sb.Append( ']' );
                sb.Append( ' ' );
                sb.Append( string.Format( format, args ) );
                Console.WriteLine( sb.ToString( ) );
            }
        }

    }
}
