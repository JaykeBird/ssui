using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;

#if (NETCOREAPP || NET45_OR_GREATER)
using System.Windows.Shell;
#else
using Microsoft.Windows.Shell;
#endif

namespace SolidShineUi.Utils
{
    /// <summary>
    /// An internal wrapper around a base <c>WindowChrome</c> object. This is built as .NET 4.5+ and .NET Core includes System.Windows.Shell.WindowChrome out of the box,
    /// but .NET 4.0 does not (and thus relies upon an external dependency).
    /// </summary>
    public class WindowChromeWrapper : Freezable
    {

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static Thickness GlassFrameCompleteThickness { get { return new Thickness(-1); } }

        public WindowChrome BaseWindowChromeObject { get; set; } = new WindowChrome();

        #region Attached Properties

        #region WindowChrome
        public WindowChromeWrapper WindowChromeVal { 
            get => (WindowChromeWrapper)GetValue(WindowChromeValProperty); 
            set => SetValue(WindowChromeValProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowChromeVal"/>. See the related property for details.</summary>
        public static DependencyProperty WindowChromeValProperty
            = DependencyProperty.RegisterAttached("WindowChromeVal", 
                typeof(WindowChromeWrapper), typeof(WindowChromeWrapper),
                new FrameworkPropertyMetadata(null, _OnChromeChanged));


        private static void _OnChromeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // The different design tools handle drawing outside their custom window objects differently.
            // Rather than try to support this concept in the design surface let the designer draw its own
            // chrome anyways.
            // There's certainly room for improvement here.
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(d))
            {
                return;
            }

            if (d is Window window)
            {
                if (e.NewValue is WindowChromeWrapper wcw)
                {
#if (NETCOREAPP || NET45_OR_GREATER)
                    System.Windows.Shell.WindowChrome.SetWindowChrome(window, wcw.BaseWindowChromeObject);
#else
                    Microsoft.Windows.Shell.WindowChrome.SetWindowChrome(window, wcw.BaseWindowChromeObject);
#endif
                }
            }
        }

        public static WindowChromeWrapper GetWindowChromeVal(Window window)
        {
            return (WindowChromeWrapper)window.GetValue(WindowChromeValProperty);
        }

        public static void SetWindowChromeVal(Window window, WindowChromeWrapper chrome)
        {
            if (window != null)
            {
                window.SetValue(WindowChromeValProperty, chrome);
            }
        }
        #endregion

        #region IsHitTestVisibleInChrome

        public static readonly DependencyProperty IsHitTestVisibleInChromeProperty
            = DependencyProperty.RegisterAttached("IsHitTestVisibleInChrome",
                typeof(bool),
                typeof(WindowChromeWrapper),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, 
                    _OnHitTestChange));

        public static bool GetIsHitTestVisibleInChrome(IInputElement inputElement)
        {
            VerifyIsNotNull(inputElement, "inputElement");
            var dobj = inputElement as DependencyObject;
            if (dobj == null)
            {
                throw new ArgumentException("The element must be a DependencyObject", "inputElement");
            }
            object htv = dobj.GetValue(IsHitTestVisibleInChromeProperty);
            bool wb = WindowChrome.GetIsHitTestVisibleInChrome(inputElement);
            if (htv is bool b)
            {
                if (b != wb)
                {
                    dobj.SetValue(IsHitTestVisibleInChromeProperty, wb);
                }
            }
            return wb;
        }

        public static void SetIsHitTestVisibleInChrome(IInputElement inputElement, bool hitTestVisible)
        {
            VerifyIsNotNull(inputElement, "inputElement");
            var dobj = inputElement as DependencyObject;
            if (dobj == null)
            {
                throw new ArgumentException("The element must be a DependencyObject", "inputElement");
            }
            dobj.SetValue(IsHitTestVisibleInChromeProperty, hitTestVisible);
            WindowChrome.SetIsHitTestVisibleInChrome(inputElement, hitTestVisible);
        }

        private static void _OnHitTestChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IInputElement i)
            {
                if (e.NewValue is bool b)
                {
                    WindowChrome.SetIsHitTestVisibleInChrome(i, b);
                }
            }
        }

        #endregion

        #endregion

        #region Other Properties

        /// <summary>The extent of the top of the window to treat as the caption.</summary>
        public double CaptionHeight
        {
            get { return BaseWindowChromeObject.CaptionHeight; }
            set { BaseWindowChromeObject.CaptionHeight = value; }
        }

        public Thickness ResizeBorderThickness
        {
            get { return BaseWindowChromeObject.ResizeBorderThickness; }
            set { BaseWindowChromeObject.ResizeBorderThickness = value; }
        }

        public Thickness GlassFrameThickness
        {
            get { return BaseWindowChromeObject.GlassFrameThickness; }
            set { BaseWindowChromeObject.GlassFrameThickness = value; }
        }

#if (NETCOREAPP || NET45_OR_GREATER)
        public bool UseAeroCaptionButtons
        {
            get { return BaseWindowChromeObject.UseAeroCaptionButtons; }
            set { BaseWindowChromeObject.UseAeroCaptionButtons = value; }
        }
        
        public NonClientFrameEdges NonClientFrameEdges
        {
            get { return BaseWindowChromeObject.NonClientFrameEdges; }
            set { BaseWindowChromeObject.NonClientFrameEdges = value; }
        }
#else
        public bool UseAeroCaptionButtons { get; set; }
#endif

        public CornerRadius CornerRadius
        {
            get { return BaseWindowChromeObject.CornerRadius; }
            set { BaseWindowChromeObject.CornerRadius = value; }
        }


        #endregion

        protected override Freezable CreateInstanceCore()
        {
            return new WindowChromeWrapper();
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        static void VerifyIsNotNull(object obj, string name)
        {
            if (obj == null) throw new ArgumentNullException(name);
        }
    }
}
