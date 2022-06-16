using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SolidShineUi
{
    [DefaultEvent("Click")]
    public class LinkTextBlock : TextBlock
    {
        bool isHighlighted = false;

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LinkTextBlock));

        /// <summary>
        /// This event is raised when the control is clicked.
        /// </summary>
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public static readonly DependencyProperty TextBrushProperty = DependencyProperty.Register(
            "TextBrush", typeof(Brush), typeof(LinkTextBlock),
            new PropertyMetadata(Color.FromRgb(0, 102, 204).ToBrush(), new PropertyChangedCallback(OnInternalBrushChanged)));

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(LinkTextBlock),
            new PropertyMetadata(Color.FromRgb(51, 153, 255).ToBrush(), new PropertyChangedCallback(OnInternalBrushChanged)));

        public static readonly DependencyProperty DisabledBrushProperty = DependencyProperty.Register(
            "DisabledBrush", typeof(Brush), typeof(LinkTextBlock),
            new PropertyMetadata(Color.FromRgb(120, 120, 120).ToBrush(), new PropertyChangedCallback(OnInternalBrushChanged)));

        public static readonly DependencyProperty AutoSetLinkFromTextProperty = DependencyProperty.Register(
            "AutoSetLinkFromText", typeof(bool), typeof(LinkTextBlock),
            new PropertyMetadata(false));

        public static readonly DependencyProperty UnderlineOnHighlightProperty = DependencyProperty.Register(
            "UnderlineOnHighlight", typeof(bool), typeof(LinkTextBlock),
            new PropertyMetadata(true));

        #region Brush change hook
        private static void OnInternalBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinkTextBlock t)
            {
                t.InternalBrushChanged?.Invoke(t, e);
            }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalBrushChanged;

        private void LinkTextBlock_InternalBrushChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsEnabled)
            {
                if (isHighlighted)
                {
                    Foreground = HighlightBrush;
                }
                else
                {
                    Foreground = TextBrush;
                }
            }
            else
            {
                Foreground = DisabledBrush;
            }
        }

        #endregion

        /// <summary>
        /// Get or set the standard foreground brush for the text. This overwrites the Foreground property.
        /// </summary>
        [Category("Brushes"), Description("Set brush for text. This overwrites the Foreground property.")]
        public Brush TextBrush
        {
            get => (Brush)GetValue(TextBrushProperty);
            set => SetValue(TextBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush for when the control has mouseover or keyboard focus.
        /// </summary>
        [Category("Brushes"), Description("Set brush for when control has mouseover or keyboard focus.")]
        public Brush HighlightBrush
        {
            get => (Brush)GetValue(HighlightBrushProperty);
            set => SetValue(HighlightBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush for when the control is disabled.
        /// </summary>
        [Category("Brushes"), Description("Set brush for when the control is disabled.")]
        public Brush DisabledBrush
        {
            get => (Brush)GetValue(DisabledBrushProperty);
            set => SetValue(DisabledBrushProperty, value);
        }

        /// <summary>
        /// Get or set if the LinkTextBlock should automatically create a link based upon the Text property. See remarks for more details.
        /// </summary>
        /// <remarks>
        /// When AutoSetLinkFromText is set to <c>true</c>, the LinkTextBlock attempts to open a file or URL based upon the value of the Text property when the user clicks on the control.
        /// For example, if this property is set to <c>true</c> and the Text property is set to <c>"https://microsoft.com"</c>, the LinkTextBlock will open "https://microsoft.com" when the control is clicked,
        /// even if a Click event handler is not attached. Note that for web links, the scheme/protocol should be present (i.e. "http", "https", "ftp", etc.).
        /// <para />
        /// Note that the Click event will still fire even if this property is set to <c>true</c>.
        /// <para/>
        /// If an error occurs while attempting to open the link, nothing happens. If a debugger is attached, the LinkTextBlock attempts to log a message to the debugger.
        /// </remarks>
        [Category("Common"), Description("Set whether to open a link when the control is clicked. Link is set by Text property.")]
        public bool AutoSetLinkFromText
        {
            get => (bool)GetValue(AutoSetLinkFromTextProperty);
            set => SetValue(AutoSetLinkFromTextProperty, value);
        }

        /// <summary>
        /// Get or set if the text should be underlined when the control has mouseover or keyboard focus.
        /// </summary>
        [Category("Appearance"), Description("Set if the text should be underlined when highlighted.")]
        public bool UnderlineOnHighlight
        {
            get => (bool)GetValue(UnderlineOnHighlightProperty);
            set => SetValue(UnderlineOnHighlightProperty, value);
        }

        public LinkTextBlock() : base()
        {
            TextDecorations = null;
            Foreground = TextBrush;
            Focusable = true;

            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;

            MouseEnter += LinkTextBlock_MouseEnter;
            MouseLeave += LinkTextBlock_MouseLeave;
            PreviewMouseUp += LinkTextBlock_PreviewMouseUp;
            PreviewStylusUp += LinkTextBlock_PreviewStylusUp;
            PreviewTouchUp += LinkTextBlock_PreviewTouchUp;
            GotKeyboardFocus += LinkTextBlock_GotKeyboardFocus;
            LostKeyboardFocus += LinkTextBlock_LostKeyboardFocus;
            PreviewKeyUp += LinkTextBlock_PreviewKeyUp;

            InternalBrushChanged += LinkTextBlock_InternalBrushChanged;
        }

        public LinkTextBlock(System.Windows.Documents.Inline inline) : base(inline)
        {
            TextDecorations = null;
            Foreground = TextBrush;
            Focusable = true;

            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;

            MouseEnter += LinkTextBlock_MouseEnter;
            MouseLeave += LinkTextBlock_MouseLeave;
            PreviewMouseUp += LinkTextBlock_PreviewMouseUp;
            PreviewStylusUp += LinkTextBlock_PreviewStylusUp;
            PreviewTouchUp += LinkTextBlock_PreviewTouchUp;
            GotKeyboardFocus += LinkTextBlock_GotKeyboardFocus;
            LostKeyboardFocus += LinkTextBlock_LostKeyboardFocus;
            PreviewKeyUp += LinkTextBlock_PreviewKeyUp;

            InternalBrushChanged += LinkTextBlock_InternalBrushChanged;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this element is enabled in the user interface (UI).
        /// </summary>
        new public bool IsEnabled
        {
            get { return base.IsEnabled; }
            set
            {
                base.IsEnabled = value;

                if (value == false)
                {
                    Foreground = DisabledBrush;
                }
                else
                {
                    Foreground = TextBrush;
                }
            }
        }

        private void Highlight()
        {
            if (IsEnabled)
            {
                if (UnderlineOnHighlight) TextDecorations = System.Windows.TextDecorations.Underline;
                Foreground = HighlightBrush;
                isHighlighted = true;
            }
        }

        private void Unlight()
        {
            if (IsEnabled)
            {
                TextDecorations = null;
                Foreground = TextBrush;
                isHighlighted = false;
            }
        }

        void LinkTextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            Highlight();
        }

        void LinkTextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            Unlight();
        }

        void LinkTextBlock_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Highlight();
        }

        void LinkTextBlock_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Unlight();
        }

        private void RaiseClick()
        {
            if (AutoSetLinkFromText)
            {
                OpenLinkFromText();
            }

            RoutedEventArgs re = new RoutedEventArgs(ClickEvent);
            RaiseEvent(re);
        }

#if NETCOREAPP
        void LinkTextBlock_PreviewTouchUp(object? sender, TouchEventArgs e)
#else
        void LinkTextBlock_PreviewTouchUp(object sender, TouchEventArgs e)
#endif
        {
            RaiseClick();
        }

        void LinkTextBlock_PreviewStylusUp(object sender, StylusEventArgs e)
        {
            RaiseClick();
        }

        void LinkTextBlock_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            RaiseClick();
        }


        void LinkTextBlock_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                RaiseClick();
            }
        }

        void OpenLinkFromText()
        {
            try
            {
                // must use UseShellExecute so that it works on .NET Core
                Process.Start(new ProcessStartInfo { FileName = Text, UseShellExecute = true });
            }
            catch (ArgumentNullException)
            {
                // could not open the link as it is null
                if (Debugger.IsAttached)
                {
                    Debugger.Log(0, "LinkTextBlock", "Link is null, from LinkTextBlock " + Name + ".\n");
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                // could not open the link as it doesn't exist
                if (Debugger.IsAttached)
                {
                    Debugger.Log(0, "LinkTextBlock", "Link \"" + Text + "\" does not exist, from LinkTextBlock " + Name + ".\n");
                }
            }
            catch (InvalidOperationException)
            {
                // could not open the link for some reason
                if (Debugger.IsAttached)
                {
                    Debugger.Log(0, "LinkTextBlock", "Link \"" + Text + "\" was not defined, from LinkTextBlock " + Name + ".\n");
                }
            }
            catch (Win32Exception)
            {
                // could not open the link for some reason
                if (Debugger.IsAttached)
                {
                    Debugger.Log(0, "LinkTextBlock", "Link \"" + Text + "\" could not be opened, from LinkTextBlock " + Name + ".\n");
                }
            }
        }

    }
}
