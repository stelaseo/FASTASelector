﻿using FASTASelector.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace FASTASelector.UserInterface
{
    internal partial class SequenceViewer : RichTextBox, INotifyPropertyChanged
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register( "Data", typeof( Sequence ), typeof( SequenceViewer ), new FrameworkPropertyMetadata( ) { DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, DefaultValue = null, PropertyChangedCallback = OnPropertyChangedCallback } );
        private enum HIGHLIGHT_STATE
        {
            DEFAULT,
            LOOK_FOR_BEGIN,
            LOOK_FOR_END,
        }
        private delegate void UpdateHandler( );
        private HIGHLIGHT_STATE _highlight = HIGHLIGHT_STATE.DEFAULT;
        private int _highlightBegin = int.MaxValue;
        private int _highlightEnd = int.MinValue;
        private int _highlightIndex = int.MaxValue;
        private Sequence _workData = null;
        private int _workPosPrev = 0;
        private int _workPosCurr = 0;
        private StringBuilder _workString = new StringBuilder( );
        private Dictionary<HIGHLIGHT_STATE, UpdateHandler> _updateHandlers = new Dictionary<HIGHLIGHT_STATE, UpdateHandler>( );


        public SequenceViewer( )
        {
            _updateHandlers.Add( HIGHLIGHT_STATE.DEFAULT, UpdateViewNoHighlight );
            _updateHandlers.Add( HIGHLIGHT_STATE.LOOK_FOR_BEGIN, UpdateViewFindHighlightBegin );
            _updateHandlers.Add( HIGHLIGHT_STATE.LOOK_FOR_END, UpdateViewFindHighlightEnd );
            InitializeComponent( );
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public Sequence Data
        {
            get { return (Sequence)GetValue( DataProperty ); }
            set
            {
                SetValue( DataProperty, value );
                UpdateView( value );
            }
        }


        public void UpdateView( )
        {
            UpdateView( Data );
        }


        private void GrabNextHighlight( )
        {
            if( _highlightIndex < _workData.Highlights.Count )
            {
                _highlightBegin = _workData.Highlights[_highlightIndex].Begin;
                _highlightEnd = _workData.Highlights[_highlightIndex].End;
                _highlightIndex++;
                _highlight = HIGHLIGHT_STATE.LOOK_FOR_BEGIN;
            }
            else
            {
                _highlightBegin = int.MaxValue;
                _highlightEnd = int.MinValue;
                _highlightIndex = int.MaxValue;
                _highlight = HIGHLIGHT_STATE.DEFAULT;
            }
        }


        private void UpdateView( Sequence data )
        {
            FontFamily = App.Configuration.SequenceView.FontFamily;
            FontSize = App.Configuration.SequenceView.FontSize;
            Document.Blocks.Clear( );
            Document.LineHeight = App.Configuration.SequenceView.LineHeight;
            if( data != null )
            {
                _highlightIndex = 0;
                _workData = data;
                _workPosPrev = 0;
                _workPosCurr = App.Configuration.SequenceView.ViewWidth;
                _workString = new StringBuilder( );
                GrabNextHighlight( );
                while( _workPosPrev < _workData.Value.Length )
                {
                    _updateHandlers[_highlight]( );
                }
            }
        }


        private void UpdateViewNoHighlight( )
        {
            int viewWidth = App.Configuration.SequenceView.ViewWidth;
            while( _workPosCurr < _workData.Value.Length )
            {
                _workString.Append( _workData.Value.Substring( _workPosPrev, _workPosCurr - _workPosPrev ) );
                _workString.Append( Environment.NewLine );
                _workPosPrev = _workPosCurr;
                _workPosCurr += viewWidth;
            }
            if( _workPosPrev < _workData.Value.Length )
            {
                _workString.Append( _workData.Value.Substring( _workPosPrev, _workData.Value.Length - _workPosPrev ) );
                _workString.Append( Environment.NewLine );
                _workPosPrev = _workPosCurr;
                _workPosCurr += viewWidth;
            }
            TextRange tr = new TextRange( Document.ContentEnd, Document.ContentEnd );
            tr.Text = _workString.ToString( );
            tr.ApplyPropertyValue( TextElement.BackgroundProperty, Brushes.Transparent );
        }


        private void UpdateViewFindHighlightBegin( )
        {
            int viewWidth = App.Configuration.SequenceView.ViewWidth;
            while( _highlight == HIGHLIGHT_STATE.LOOK_FOR_BEGIN && _workPosPrev < _workData.Value.Length )
            {
                if( _highlightBegin <= _workPosCurr )
                {   // found
                    _workString.Append( _workData.Value.Substring( _workPosPrev, _highlightBegin - _workPosPrev ) );
                    if( _highlightBegin == _workPosCurr )
                    {
                        _workString.Append( Environment.NewLine );
                    }
                    _workPosPrev = _highlightBegin;
                    TextRange tr = new TextRange( Document.ContentEnd, Document.ContentEnd );
                    tr.Text = _workString.ToString( );
                    tr.ApplyPropertyValue( TextElement.BackgroundProperty, Brushes.Transparent );
                    _highlight = HIGHLIGHT_STATE.LOOK_FOR_END;
                    _workString = new StringBuilder( );
                }
                else
                {   // not found
                    _workString.Append( _workData.Value.Substring( _workPosPrev, Math.Min( _workData.Value.Length, _workPosCurr ) - _workPosPrev ) );
                    _workString.Append( Environment.NewLine );
                    _workPosPrev = _workPosCurr;
                    _workPosCurr += viewWidth;
                }
            }
        }


        private void UpdateViewFindHighlightEnd( )
        {
            int viewWidth = App.Configuration.SequenceView.ViewWidth;
            while( _highlight == HIGHLIGHT_STATE.LOOK_FOR_END && _workPosPrev < _workData.Value.Length )
            {
                if( _highlightEnd < _workPosCurr )
                {   // found
                    _workString.Append( _workData.Value.Substring( _workPosPrev, _highlightEnd - _workPosPrev ) );
                    _workPosPrev = _highlightEnd;
                    TextRange tr = new TextRange( Document.ContentEnd, Document.ContentEnd );
                    tr.Text = _workString.ToString( );
                    tr.ApplyPropertyValue( TextElement.BackgroundProperty, Brushes.Yellow );
                    GrabNextHighlight( );
                    _workString = new StringBuilder( );
                }
                else
                {   // not found
                    _workString.Append( _workData.Value.Substring( _workPosPrev, Math.Min( _workData.Value.Length, _workPosCurr ) - _workPosPrev ) );
                    _workString.Append( Environment.NewLine );
                    _workPosPrev = _workPosCurr;
                    _workPosCurr += viewWidth;
                }
            }
        }


        private void NotifyPropertyChanged( string name )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( name ) );
        }


        private static void OnPropertyChangedCallback( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            if( d is SequenceViewer obj )
            {
                if( e.Property == DataProperty )
                {
                    obj.UpdateView( (Sequence)e.NewValue );
                }
                obj.NotifyPropertyChanged( e.Property.Name );
            }
        }

    }
}
