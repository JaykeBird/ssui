using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;

namespace SolidShineUi.Ribbon
{
    /// <summary>
    /// A file menu, built to be displayed in the top-left corner of a <see cref="Ribbon"/>.
    /// </summary>
    [ContentProperty("Items")]
    public class RibbonFileMenu : ButtonBase
    {
        static RibbonFileMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonFileMenu), new FrameworkPropertyMetadata(typeof(RibbonFileMenu)));
        }

        public RibbonFileMenu()
        {
            SetValue(MinWidthProperty, 60.0);
            SetValue(PaddingProperty, new Thickness(10, 2, 10, 2));

            Click += RibbonFileMenu_Click;
            PreviewMouseDown += control_PreviewMouseDown;
            PreviewMouseUp += control_PreviewMouseUp;
            MouseLeave += control_MouseLeave;
        }

#if NETCOREAPP
        Popup? PART_Popup = null;
#else
        Popup PART_Popup = null;
#endif

        public override void OnApplyTemplate()
        {

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Get or set the title to display on the File menu button itself. A shorter title is recommended, such as "File" or "App".
        /// </summary>
        [Category("Common")]
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public static DependencyProperty TitleProperty
            = DependencyProperty.Register("Title", typeof(string), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata("File"));

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        private static readonly DependencyPropertyKey ItemsPropertyKey
            = DependencyProperty.RegisterReadOnly("Items", typeof(ObservableCollection<IRibbonItem>), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(new ObservableCollection<IRibbonItem>()));

        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the list of items in this File menu. This Items property can be used to add and remove items.
        /// </summary>
        [Category("Common")]
        public ObservableCollection<IRibbonItem> Items
        {
            get { return (ObservableCollection<IRibbonItem>)GetValue(ItemsProperty); }
            private set { SetValue(ItemsPropertyKey, value); }
        }

        #region Brushes
        public Brush FileMenuButtonBackgroundBrush { get => (Brush)GetValue(FileMenuButtonBackgroundBrushProperty); set => SetValue(FileMenuButtonBackgroundBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="FileMenuButtonBackgroundBrush"/>. See the related property for details.</summary>
        public static DependencyProperty FileMenuButtonBackgroundBrushProperty
            = DependencyProperty.Register("FileMenuButtonBackgroundBrush", typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Colors.CornflowerBlue.ToBrush()));

        public Brush FileMenuButtonHighlightBrush { get => (Brush)GetValue(FileMenuButtonHighlightBrushProperty); set => SetValue(FileMenuButtonHighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="FileMenuButtonHighlightBrush"/>. See the related property for details.</summary>
        public static DependencyProperty FileMenuButtonHighlightBrushProperty
            = DependencyProperty.Register("FileMenuButtonHighlightBrush", typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(BrushFactory.Create("83aaf0")));

        public Brush FileMenuButtonClickBrush { get => (Brush)GetValue(FileMenuButtonClickBrushProperty); set => SetValue(FileMenuButtonClickBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="FileMenuButtonClickBrush"/>. See the related property for details.</summary>
        public static DependencyProperty FileMenuButtonClickBrushProperty
            = DependencyProperty.Register("FileMenuButtonClickBrush", typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Colors.DarkBlue.ToBrush()));

        public Brush MenuBorderBrush { get => (Brush)GetValue(MenuBorderBrushProperty); set => SetValue(MenuBorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuBorderBrush"/>. See the related property for details.</summary>
        public static DependencyProperty MenuBorderBrushProperty
            = DependencyProperty.Register("MenuBorderBrush", typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Colors.DarkGray.ToBrush()));

        public Brush MenuBackground { get => (Brush)GetValue(MenuBackgroundProperty); set => SetValue(MenuBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuBackground"/>. See the related property for details.</summary>
        public static DependencyProperty MenuBackgroundProperty
            = DependencyProperty.Register("MenuBackground", typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Colors.White.ToBrush()));

        public Brush MenuSecondaryPanelBackground { get => (Brush)GetValue(MenuSecondaryPanelBackgroundProperty); set => SetValue(MenuSecondaryPanelBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuSecondaryPanelBackground"/>. See the related property for details.</summary>
        public static DependencyProperty MenuSecondaryPanelBackgroundProperty
            = DependencyProperty.Register("MenuSecondaryPanelBackground", typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Colors.Gainsboro.ToBrush()));

        public Brush DisabledBrush { get => (Brush)GetValue(DisabledBrushProperty); set => SetValue(DisabledBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="DisabledBrush"/>. See the related property for details.</summary>
        public static DependencyProperty DisabledBrushProperty
            = DependencyProperty.Register("DisabledBrush", typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Colors.Gainsboro.ToBrush()));

        #endregion

        #region IsMouseDown

        // from https://stackoverflow.com/questions/10667545/why-ismouseover-is-recognized-and-mousedown-isnt-wpf-style-trigger

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected static readonly DependencyPropertyKey IsMouseDownPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsMouseDown",
            typeof(bool), typeof(RibbonFileMenu), new FrameworkPropertyMetadata(false));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get if there is a mouse button currently being pressed, while the mouse cursor is over this control.
        /// </summary>
        public static readonly DependencyProperty IsMouseDownProperty = IsMouseDownPropertyKey.DependencyProperty;

        /// <summary>
        /// Set the IsMouseDown property for a FlatButton.
        /// </summary>
        /// <param name="obj">The FlatButton to apply the property change to.</param>
        /// <param name="value">The new value to set for the property.</param>
        protected static void SetIsMouseDown(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMouseDownPropertyKey, value);
        }

        /// <summary>
        /// Get the IsMouseDown property for a FlatButton.
        /// </summary>
        /// <param name="obj">The Flatbutton to get the property value from.</param>
        public static bool GetIsMouseDown(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMouseDownProperty);
        }

        #endregion

        private void RibbonFileMenu_Click(object sender, RoutedEventArgs e)
        {
            if (IsSubmenuOpen)
            {
                SetValue(IsSubmenuOpenProperty, false);
            }
            else
            {
                SetValue(IsSubmenuOpenProperty, true);
            }
        }

        #region Mouse Events
        private void control_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SetIsMouseDown(this, true);
        }

        private void control_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SetIsMouseDown(this, false);
        }

        private void control_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            SetIsMouseDown(this, false);
        }

        /// <summary>
        /// Perform a click programmatically. The button responds the same way as if it was clicked by the user.
        /// </summary>
        public void DoClick()
        {
            OnClick();
        }
        #endregion

        // from WPF MenuItem

        /// <summary>The backing dependency property for <see cref="IsSubmenuOpenProperty"/>. See the related property for details.</summary>
        public static DependencyProperty IsSubmenuOpenProperty
            = DependencyProperty.Register("IsSubmenuOpen", typeof(bool), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(false));

        [Bindable(true)]
        [Browsable(false)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSubmenuOpen
        {
            get => (bool)GetValue(IsSubmenuOpenProperty);
            set => SetValue(IsSubmenuOpenProperty, value);
        }

        public bool MenuStaysOpen { get => (bool)GetValue(MenuStaysOpenProperty); set => SetValue(MenuStaysOpenProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuStaysOpen"/>. See the related property for details.</summary>
        public static DependencyProperty MenuStaysOpenProperty
            = DependencyProperty.Register("MenuStaysOpen", typeof(bool), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(false));


        public double MenuMaxHeight { get => (double)GetValue(MenuMaxHeightProperty); set => SetValue(MenuMaxHeightProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuMaxHeight"/>. See the related property for details.</summary>
        public static DependencyProperty MenuMaxHeightProperty
            = DependencyProperty.Register("MenuMaxHeight", typeof(double), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(500.0));

        public GridLength SecondaryPanelWidth { get => (GridLength)GetValue(SecondaryPanelWidthProperty); set => SetValue(SecondaryPanelWidthProperty, value); }

        /// <summary>The backing dependency property for <see cref="SecondaryPanelWidth"/>. See the related property for details.</summary>
        public static DependencyProperty SecondaryPanelWidthProperty
            = DependencyProperty.Register("SecondaryPanelWidth", typeof(GridLength), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(new GridLength(150.0)));

        public object SecondaryPanelContent { get => GetValue(SecondaryPanelContentProperty); set => SetValue(SecondaryPanelContentProperty, value); }

        /// <summary>The backing dependency property for <see cref="SecondaryPanelContent"/>. See the related property for details.</summary>
        public static DependencyProperty SecondaryPanelContentProperty
            = DependencyProperty.Register("SecondaryPanelContent", typeof(object), typeof(RibbonFileMenu));

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ShowMenuArrowProperty = DependencyProperty.Register(
            "ShowMenuArrow", typeof(bool), typeof(RibbonFileMenu),
            new PropertyMetadata(false));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set if an arrow should be shown to the right of the File button text to indicate the button as a menu button.
        /// </summary>
        [Category("Common")]
        public bool ShowMenuArrow
        {
            get => (bool)GetValue(ShowMenuArrowProperty);
            set => SetValue(ShowMenuArrowProperty, value);
        }
    }
}
