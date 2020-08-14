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
                return "Copyright (C) 2020  Stela H. Seo <stela.seo@cs.umanitoba.ca>" +
                    Environment.NewLine + Environment.NewLine +
                    "This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version." +
                    Environment.NewLine + Environment.NewLine +
                    "This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details." +
                    Environment.NewLine + Environment.NewLine +
                    "You should have received a copy of the GNU General Public License along with this program.  If not, see <https://www.gnu.org/licenses/>.";
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
