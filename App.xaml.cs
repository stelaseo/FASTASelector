using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows;

namespace FASTASelector
{
    public delegate void LogHandler( string format, params object[] args );

    public partial class App : Application
    {
        private static LogHandler _logHandler = DefaultLogHandler;


        public static string Name
        {
            get { return "FASTA Selector"; }
        }


        public static string License
        {
            get
            {
                return "The MIT License (MIT)"
                    + Environment.NewLine + Environment.NewLine
                    + "Copyright (c) Stela H. Seo <stela.seo@cs.umanitoba.ca>"
                    + Environment.NewLine + Environment.NewLine
                    + "Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:"
                    + Environment.NewLine + Environment.NewLine
                    + "The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software."
                    + Environment.NewLine + Environment.NewLine
                    + "THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.";
            }
        }


        public static LogHandler Log
        {
            get { return _logHandler; }
            set { _logHandler = value ?? DefaultLogHandler; }
        }


        public static string Version
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly( );
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo( assembly.Location );
                return fvi.FileVersion;
            }
        }


        private static void DefaultLogHandler( string format, params object[] args )
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
