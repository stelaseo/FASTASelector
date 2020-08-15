using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows.Media;

namespace FASTASelector
{
    internal static class Utility
    {

        public static FontFamily CreateFontFamily( string value )
        {
            FontFamily fontFamily;
            try
            {
                fontFamily = new FontFamily( value ?? "Consolas" );
            }
            catch( Exception ex )
            {
                fontFamily = new FontFamily( );
                App.Log( App.WARNING, ex.Message );
            }
            return fontFamily;
        }


        public static string EncodingToString( Encoding encoding )
        {
            return encoding.EncodingName + " (" + encoding.WebName + ")";
        }


        public static void SetDialogFileName( FileDialog fileDialog, string filename )
        {
            if( !string.IsNullOrWhiteSpace( filename ) )
            {
                FileInfo fileInfo = new FileInfo( filename );
                fileDialog.FileName = fileInfo.Name;
                fileDialog.InitialDirectory = fileInfo.DirectoryName;
            }
            else
            {
                fileDialog.FileName = string.Empty;
                fileDialog.InitialDirectory = Environment.CurrentDirectory;
            }
        }


        public static Encoding StringToEncoding( string value )
        {
            Encoding result = null;
            int beginIndex = value.LastIndexOf( "(" );
            int endIndex = value.LastIndexOf( ")" );
            if( beginIndex >= 0 && endIndex >= 0 && beginIndex < endIndex )
            {
                string webName = value.Substring( beginIndex + 1, endIndex - beginIndex - 1 );
                result = Encoding.GetEncoding( webName );
            }
            return result;
        }

    }
}
