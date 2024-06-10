using SolidShineUi.Utils;
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
using System.Windows.Input;
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

        /// <summary>
        /// Create a RibbonFileMenu.
        /// </summary>
        public RibbonFileMenu()
        {
            Click += RibbonFileMenu_Click;
            PreviewMouseDown += control_PreviewMouseDown;
            PreviewMouseUp += control_PreviewMouseUp;
            MouseLeave += control_MouseLeave;
        }

        #region Template Handling

        bool itemsLoaded = false;

#if NETCOREAPP
        Popup? PART_Popup = null;
        Border? mainButton = null;
#else
        Popup PART_Popup = null;
        Border mainButton = null;
#endif

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            LoadTemplateItems();
        }

        private void LoadTemplateItems()
        {
            if (!itemsLoaded)
            {
                PART_Popup = (Popup)GetTemplateChild("PART_Popup");
                mainButton = (Border)GetTemplateChild("btn_Border");

                if (PART_Popup != null && mainButton != null)
                {
                    itemsLoaded = true;
                }
            }
        }

        #endregion

        /// <summary>
        /// Get or set the title to display on the File menu button itself. A shorter title is recommended, such as "File" or "App".
        /// </summary>
        [Category("Common")]
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        /// <summary>The backing dependency property for <see cref="Title"/>. See the related property for details.</summary>
        public static DependencyProperty TitleProperty
            = DependencyProperty.Register("Title", typeof(string), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata("File"));

        private static readonly DependencyPropertyKey ItemsPropertyKey
            = DependencyProperty.RegisterReadOnly("Items", typeof(ObservableCollection<IRibbonItem>), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(new ObservableCollection<IRibbonItem>()));

        /// <summary>The backing dependency property for <see cref="Items"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Get or set the list of items in this File menu. This Items property can be used to add and remove items.
        /// </summary>
        [Category("Common")]
        public ObservableCollection<IRibbonItem> Items
        {
            get { return (ObservableCollection<IRibbonItem>)GetValue(ItemsProperty); }
            private set { SetValue(ItemsPropertyKey, value); }
        }

        #region ColorScheme

        /// <summary>
        /// Get or set the color scheme used for this control. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        public ColorScheme ColorScheme { get => (ColorScheme)GetValue(ColorSchemeProperty); set => SetValue(ColorSchemeProperty, value); }

        /// <summary>The backing dependency property for <see cref="ColorScheme"/>. See the related property for details.</summary>
        public static DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register(nameof(ColorScheme), typeof(ColorScheme), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(new ColorScheme(), (d, e) => d.PerformAs<RibbonFileMenu>((o) => o.OnColorSchemeChanged(e))));

        /// <summary>
        /// The backing routed event object for <see cref="ColorSchemeChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent ColorSchemeChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(ColorSchemeChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<ColorScheme>), typeof(RibbonFileMenu));

        /// <summary>
        /// Raised when the <see cref="ColorScheme"/> property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<ColorScheme> ColorSchemeChanged
        {
            add { AddHandler(ColorSchemeChangedEvent, value); }
            remove { RemoveHandler(ColorSchemeChangedEvent, value); }
        }

        private void OnColorSchemeChanged(DependencyPropertyChangedEventArgs e)
        {
            ApplyColorScheme((ColorScheme)e.NewValue);

            RoutedPropertyChangedEventArgs<ColorScheme> re = new RoutedPropertyChangedEventArgs<ColorScheme>
                ((ColorScheme)e.OldValue, (ColorScheme)e.NewValue, ColorSchemeChangedEvent);
            re.Source = this;
            RaiseEvent(re);
        }

        /// <summary>
        /// Apply a color scheme to this control. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply.</param>
        public void ApplyColorScheme(ColorScheme cs)
        {
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }

            BorderBrush = cs.BorderColor.ToBrush();
            MenuBorderBrush = cs.BorderColor.ToBrush();
            MenuBackground = cs.LightBackgroundColor.ToBrush();
            MenuSecondaryPanelBackground = cs.BackgroundColor.ToBrush();
            DisabledBrush = cs.LightDisabledColor.ToBrush();
        }


        #endregion

        #region Brushes

        /// <summary>
        /// Get or set the brush to use for the background of the File menu button.
        /// </summary>
        public Brush ButtonBackgroundBrush { get => (Brush)GetValue(ButtonBackgroundBrushProperty); set => SetValue(ButtonBackgroundBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ButtonBackgroundBrush"/>. See the related property for details.</summary>
        public static DependencyProperty ButtonBackgroundBrushProperty
            = DependencyProperty.Register("ButtonBackgroundBrush", typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Colors.CornflowerBlue.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the File menu button when it is highlighted (has the mouse/pointer over it).
        /// </summary>
        /// <remarks>
        /// A semi-transparent brush can be used to allow the base color of <see cref="ButtonBackgroundBrush"/> to come through.
        /// </remarks>
        public Brush ButtonHighlightBrush { get => (Brush)GetValue(ButtonHighlightBrushProperty); set => SetValue(ButtonHighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ButtonHighlightBrush"/>. See the related property for details.</summary>
        public static DependencyProperty ButtonHighlightBrushProperty
            = DependencyProperty.Register("ButtonHighlightBrush", typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Color.FromArgb(50, 255, 255, 255).ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the File menu button when it is being clicked (the mouse button is being pressed).
        /// </summary>
        /// <remarks>
        /// A semi-transparent brush can be used to allow the base color of <see cref="ButtonBackgroundBrush"/> to come through.
        /// </remarks>
        public Brush ButtonClickBrush { get => (Brush)GetValue(ButtonClickBrushProperty); set => SetValue(ButtonClickBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ButtonClickBrush"/>. See the related property for details.</summary>
        public static DependencyProperty ButtonClickBrushProperty
            = DependencyProperty.Register("ButtonClickBrush", typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Color.FromArgb(70, 120, 120, 120).ToBrush()));

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

        public Brush BorderDisabledBrush { get => (Brush)GetValue(BorderDisabledBrushProperty); set => SetValue(BorderDisabledBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="BorderDisabledBrush"/>. See the related property for details.</summary>
        public static DependencyProperty BorderDisabledBrushProperty
            = DependencyProperty.Register(nameof(BorderDisabledBrush), typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Colors.DimGray.ToBrush()));

        public Brush BorderHighlightBrush { get => (Brush)GetValue(BorderHighlightBrushProperty); set => SetValue(BorderHighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="BorderHighlightBrush"/>. See the related property for details.</summary>
        public static DependencyProperty BorderHighlightBrushProperty
            = DependencyProperty.Register(nameof(BorderHighlightBrush), typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Colors.Gray.ToBrush()));


        #endregion

        #region Mouse and Clicking

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

        /// <summary>
        /// Perform a click programmatically. The button responds the same way as if it was clicked by the user.
        /// </summary>
        public void DoClick()
        {
            OnClick();
        }


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

        #region Mouse Events

        private void control_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (mainButton != null && mainButton.IsMouseOver)
            {
                SetIsMouseDown(this, true);
            }
        }

        private void control_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            SetIsMouseDown(this, false);
        }

        private void control_MouseLeave(object sender, MouseEventArgs e)
        {
            SetIsMouseDown(this, false);
        }

        #endregion

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

        /// <summary>The backing dependency property for <see cref="ShowMenuArrow"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ShowMenuArrowProperty = DependencyProperty.Register(
            "ShowMenuArrow", typeof(bool), typeof(RibbonFileMenu),
            new PropertyMetadata(false));

        /// <summary>
        /// Get or set if an arrow should be shown to the right of the File button text to indicate the button as a menu button.
        /// </summary>
        [Category("Common")]
        public bool ShowMenuArrow
        {
            get => (bool)GetValue(ShowMenuArrowProperty);
            set => SetValue(ShowMenuArrowProperty, value);
        }

        /// <summary>
        /// Get or set the minimum height that the File menu can be; if there aren't enough items to fill in the full vertical space, 
        /// some blank space will be present at the bottom of the menu.
        /// </summary>
        public double MenuMinHeight { get => (double)GetValue(MenuMinHeightProperty); set => SetValue(MenuMinHeightProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuMinHeight"/>. See the related property for details.</summary>
        public static DependencyProperty MenuMinHeightProperty
            = DependencyProperty.Register("MenuMinHeight", typeof(double), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(150.0));

        /// <summary>
        /// Get or set the maximum height that the File menu can be; if there are more items than can fit, a scrollbar will appear.
        /// </summary>
        public double MenuMaxHeight { get => (double)GetValue(MenuMaxHeightProperty); set => SetValue(MenuMaxHeightProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuMaxHeight"/>. See the related property for details.</summary>
        public static DependencyProperty MenuMaxHeightProperty
            = DependencyProperty.Register("MenuMaxHeight", typeof(double), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(600.0));

        #region Secondary Panel

        /// <summary>
        /// Get or set the brush to use for the divider between the left main part of the menu and the secondary panel on the right side.
        /// </summary>
        public Brush PanelDividerBrush { get => (Brush)GetValue(PanelDividerBrushProperty); set => SetValue(PanelDividerBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="PanelDividerBrush"/>. See the related property for details.</summary>
        public static DependencyProperty PanelDividerBrushProperty
            = DependencyProperty.Register(nameof(PanelDividerBrush), typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Colors.DimGray.ToBrush()));

        /// <summary>
        /// Get or set the desired width for the secondary panel on the right side of the File menu.
        /// </summary>
        public GridLength SecondaryPanelWidth { get => (GridLength)GetValue(SecondaryPanelWidthProperty); set => SetValue(SecondaryPanelWidthProperty, value); }

        /// <summary>The backing dependency property for <see cref="SecondaryPanelWidth"/>. See the related property for details.</summary>
        public static DependencyProperty SecondaryPanelWidthProperty
            = DependencyProperty.Register("SecondaryPanelWidth", typeof(GridLength), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(new GridLength(150.0)));

        /// <summary>
        /// Get or set the content to display on the secondary panel on the right side of the File menu.
        /// </summary>
        public object SecondaryPanelContent { get => GetValue(SecondaryPanelContentProperty); set => SetValue(SecondaryPanelContentProperty, value); }

        /// <summary>The backing dependency property for <see cref="SecondaryPanelContent"/>. See the related property for details.</summary>
        public static DependencyProperty SecondaryPanelContentProperty
            = DependencyProperty.Register("SecondaryPanelContent", typeof(object), typeof(RibbonFileMenu));

        #endregion

    }
}
