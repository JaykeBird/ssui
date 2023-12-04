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
    /// An internal wrapper around a base <c>WindowChrome</c> object. This was built because .NET 4.5+ and .NET Core includes <c>System.Windows.Shell.WindowChrome</c> out of the box,
    /// but .NET 4.0 does not (and thus relies upon an external dependency).
    /// </summary>
    /// <remarks>
    /// If you are not targetting multiple .NET Framework versions in your program, it's better to instead just use the standard <c>WindowChrome</c> object.
    /// </remarks>
    public class WindowChromeWrapper : Freezable
    {
        /// <summary>
        /// Get a uniform thickness of -1.
        /// </summary>
        public static Thickness GlassFrameCompleteThickness { get { return new Thickness(-1); } }

        /// <summary>
        /// The underlying WindowChrome object that actually holds the data for a particular window.
        /// </summary>
        public WindowChrome BaseWindowChromeObject { get; set; } = new WindowChrome();

        #region Attached Properties

        #region WindowChrome
        /// <summary>
        /// Get or set the WindowChromeWrapper for a particular window.
        /// </summary>
        /// <remarks>
        /// If this property is attached to an object other than a <see cref="Window"/>, this will fail.
        /// </remarks>
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
        
        /// <summary>
        /// Get a WindowChromeWrapper for a particular window.
        /// </summary>
        /// <param name="window">The window to get the value for.</param>
        public static WindowChromeWrapper GetWindowChromeVal(Window window)
        {
            return (WindowChromeWrapper)window.GetValue(WindowChromeValProperty);
        }

        /// <summary>
        /// Set a WindowChromeWrapper for a particular window.
        /// </summary>
        /// <param name="window">The window to get the value for.</param>
        /// <param name="chrome">The WindowChromeWrapper value to apply.</param>
        public static void SetWindowChromeVal(Window window, WindowChromeWrapper chrome)
        {
            if (window != null)
            {
                window.SetValue(WindowChromeValProperty, chrome);
            }
        }
        #endregion

        #region IsHitTestVisibleInChrome

        /// <summary>
        /// A dependency property for backing the <c>IsHitTestVisibleInChrome</c> attached property.
        /// This determines if the attached object is available for hit-testing. If <c>false</c>, the object cannot be clicked on and is considered part of the window chrome.
        /// </summary>
        /// <remarks>
        /// Use <see cref="GetIsHitTestVisibleInChrome(IInputElement)"/> and <see cref="SetIsHitTestVisibleInChrome(IInputElement, bool)"/> to set this property on an object.
        /// </remarks>
        public static readonly DependencyProperty IsHitTestVisibleInChromeProperty
            = DependencyProperty.RegisterAttached("IsHitTestVisibleInChrome",
                typeof(bool),
                typeof(WindowChromeWrapper),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, 
                    _OnHitTestChange));

        /// <summary>
        /// Get if a particular object is available for hit-testing. If <c>false</c>, the object cannot be clicked on and is considered part of the window chrome.
        /// </summary>
        /// <param name="inputElement">the dependency object to check</param>
        /// <exception cref="ArgumentException">thrown if the inputted object is not a <see cref="DependencyObject"/></exception>
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

        /// <summary>
        /// Set if a particular object is available for hit-testing. If <c>false</c>, the object cannot be clicked on and is considered part of the window chrome.
        /// </summary>
        /// <param name="inputElement">the dependency object to set the value for</param>
        /// <param name="hitTestVisible">the value to set</param>
        /// <exception cref="ArgumentException">thrown if the inputted object is not a <see cref="DependencyObject"/></exception>
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

        /// <summary>Get or set the height of the caption area at the top of the window. This is also known as the title bar.</summary>
        public double CaptionHeight
        {
            get { return BaseWindowChromeObject.CaptionHeight; }
            set { BaseWindowChromeObject.CaptionHeight = value; }
        }

        /// <summary>Get or set the width of the border that can be used for resizing the window. The larger the border, the larger an area that can be clicked on to start resizing.</summary>
        public Thickness ResizeBorderThickness
        {
            get { return BaseWindowChromeObject.ResizeBorderThickness; }
            set { BaseWindowChromeObject.ResizeBorderThickness = value; }
        }

        /// <summary>Get or set the size of the glass frame around the edges of the window. This is more useful on versions of Windows that have the Aero effect, like Windows 7.</summary>
        public Thickness GlassFrameThickness
        {
            get { return BaseWindowChromeObject.GlassFrameThickness; }
            set { BaseWindowChromeObject.GlassFrameThickness = value; }
        }

        /// <summary>
        /// Get or set if hit-testing is enabled on the Windows Aero caption buttons. These are the maximize/minimize/close buttons on the top corner of the window.
        /// </summary>
#if (NETCOREAPP || NET45_OR_GREATER)
        public bool UseAeroCaptionButtons
        {
            get { return BaseWindowChromeObject.UseAeroCaptionButtons; }
            set { BaseWindowChromeObject.UseAeroCaptionButtons = value; }
        }
        
        /// <summary>
        /// Get or set which edges of the window frame are not owned by the client.
        /// </summary>
        public NonClientFrameEdges NonClientFrameEdges
        {
            get { return BaseWindowChromeObject.NonClientFrameEdges; }
            set { BaseWindowChromeObject.NonClientFrameEdges = value; }
        }
#else
        public bool UseAeroCaptionButtons { get; set; }
#endif

        /// <summary>
        /// Get or set the amount of rounding to apply to the corners of the window.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return BaseWindowChromeObject.CornerRadius; }
            set { BaseWindowChromeObject.CornerRadius = value; }
        }


        #endregion

        /// <inheritdoc/>
        protected override Freezable CreateInstanceCore()
        {
            return new WindowChromeWrapper();
        }

        static void VerifyIsNotNull(object obj, string name)
        {
            if (obj == null) throw new ArgumentNullException(name);
        }
    }
}
