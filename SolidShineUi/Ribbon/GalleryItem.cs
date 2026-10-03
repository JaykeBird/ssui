using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SolidShineUi.Utils;

namespace SolidShineUi.Ribbon
{

    /// <summary>
    /// A control to display content, for usage within a <see cref="Gallery"/>.
    /// </summary>
    public class GalleryItem : ThemedContentControl, IClickSelectableControl
    {
        static GalleryItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryItem), new FrameworkPropertyMetadata(typeof(GalleryItem)));
        }

        /// <summary>
        /// Create a GalleryItem.
        /// </summary>
        public GalleryItem()
        {
            MouseEnter += UserControl_MouseEnter;
            MouseLeave += UserControl_MouseLeave;
            TouchEnter += UserControl_TouchEnter;
            TouchLeave += UserControl_TouchLeave;
            StylusEnter += UserControl_StylusEnter;
            StylusLeave += UserControl_StylusLeave;

            GotKeyboardFocus += UserControl_GotKeyboardFocus;
            LostKeyboardFocus += UserControl_LostKeyboardFocus;

            MouseDown += UserControl_MouseDown;
            MouseUp += UserControl_MouseUp;
            TouchDown += UserControl_TouchDown;
            TouchUp += UserControl_TouchUp;
            StylusDown += UserControl_StylusDown;
            StylusUp += UserControl_StylusUp;

            KeyDown += UserControl_KeyDown;
        }

        #region Color Scheme
        /// <summary>
        /// Raised when the ColorScheme property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

        /// <summary>
        /// A dependency property object backing the related ColorScheme property. See <see cref="ColorScheme"/> for more details.
        /// </summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(GalleryItem),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            ColorScheme cs = (e.NewValue as ColorScheme)!;
#else
            ColorScheme cs = e.NewValue as ColorScheme;
#endif

            if (d is GalleryItem c)
            {
                c.ColorSchemeChanged?.Invoke(d, e);
                c.ApplyColorScheme(cs);
            }
        }

        /// <summary>
        /// Get or set the color scheme used for this control. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        /// <summary>
        /// Apply a color scheme to this control. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply.</param>
        public void ApplyColorScheme(ColorScheme cs)
        {
            if (cs == null)
            {
                return;
            }
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }

            Background = Color.FromArgb(1, 0, 0, 0).ToBrush();
            BorderBrush = Color.FromArgb(1, 0, 0, 0).ToBrush();

            if (cs.IsHighContrast)
            {
                Background = cs.BackgroundColor.ToBrush();
                HighlightBrush = cs.HighlightColor.ToBrush();
                SelectedBrush = cs.HighlightColor.ToBrush();
                BorderHighlightBrush = cs.BorderColor.ToBrush();
                BorderSelectedBrush = cs.BorderColor.ToBrush();
                BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                DisabledBrush = cs.BackgroundColor.ToBrush();
                Foreground = cs.ForegroundColor.ToBrush();
                ClickBrush = cs.ThirdHighlightColor.ToBrush();

                BorderBrush = Color.FromArgb(1, 0, 0, 0).ToBrush();
            }
            else
            {
                Background = Color.FromArgb(1, 0, 0, 0).ToBrush();
                BorderBrush = Color.FromArgb(1, 0, 0, 0).ToBrush();

                HighlightBrush = cs.SecondHighlightColor.ToBrush();
                DisabledBrush = cs.LightDisabledColor.ToBrush();
                BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                SelectedBrush = cs.ThirdHighlightColor.ToBrush();
                BorderHighlightBrush = cs.HighlightColor.ToBrush();
                BorderSelectedBrush = cs.SelectionColor.ToBrush();
                Foreground = cs.ForegroundColor.ToBrush();
                ClickBrush = cs.ThirdHighlightColor.ToBrush();
            }
        }
        #endregion

        #region SsuiTheme

        /// <inheritdoc/>
        protected override void OnApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
        {
            base.OnApplySsuiTheme(ssuiTheme, useLightBorder, useAccentTheme);

            if (useAccentTheme && ssuiTheme is SsuiAppTheme ssuiAppTheme)
            {
                ApplyTheme(ssuiAppTheme.AccentTheme);
            }
            else
            {
                ApplyTheme(ssuiTheme);
            }

            void ApplyTheme(SsuiTheme theme)
            {
                ApplyThemeBinding(BackgroundProperty, SsuiTheme.ButtonBackgroundProperty, theme);
                ApplyThemeBinding(HighlightBrushProperty, SsuiTheme.HighlightBrushProperty, theme);
                ApplyThemeBinding(DisabledBrushProperty, SsuiTheme.DisabledBackgroundProperty, theme);
                ApplyThemeBinding(BorderDisabledBrushProperty, SsuiTheme.DisabledBorderBrushProperty, theme);
                ApplyThemeBinding(SelectedBrushProperty, SsuiTheme.SelectedBackgroundBrushProperty, theme);
                ApplyThemeBinding(BorderHighlightBrushProperty, SsuiTheme.HighlightBorderBrushProperty, theme);
                ApplyThemeBinding(BorderSelectedBrushProperty, SsuiTheme.SelectedBorderBrushProperty, theme);
                ApplyThemeBinding(ForegroundProperty, SsuiTheme.ForegroundProperty, theme);
                ApplyThemeBinding(HighlightForegroundProperty, SsuiTheme.HighlightForegroundProperty, theme);
                ApplyThemeBinding(SelectedForegroundProperty, SsuiTheme.SelectedForegroundProperty, theme);
                ApplyThemeBinding(ClickBrushProperty, SsuiTheme.ClickBrushProperty, theme);

                ApplyThemeBinding(CornerRadiusProperty, SsuiTheme.CornerRadiusProperty, theme);
            }
        }

        #endregion

        #region Gallery Item Appearance

        // TODO: change LayoutType to an attached property?

        /// <summary>
        /// A property to control the layout and appearance of the GalleryItem within a <see cref="Gallery"/>. Used internally.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GalleryItemLayout LayoutType { get => (GalleryItemLayout)GetValue(LayoutTypeProperty); set => SetValue(LayoutTypeProperty, value); }

        /// <summary>The backing dependency property for <see cref="LayoutType"/>. Used internally.</summary>
        public static readonly DependencyProperty LayoutTypeProperty
            = DependencyProperty.Register(nameof(LayoutType), typeof(GalleryItemLayout), typeof(GalleryItem),
            new FrameworkPropertyMetadata(GalleryItemLayout.LargeIconAndText));

        /// <summary>
        /// Get or set the large icon to use, when using a layout that allows large icons.
        /// </summary>
        public ImageSource LargeIcon { get => (ImageSource)GetValue(LargeIconProperty); set => SetValue(LargeIconProperty, value); }

        /// <summary>The backing dependency property for <see cref="LargeIcon"/>. See the related property for details.</summary>
        public static readonly DependencyProperty LargeIconProperty
            = DependencyProperty.Register(nameof(LargeIcon), typeof(ImageSource), typeof(GalleryItem),
            new FrameworkPropertyMetadata(defaultValue: null));

        /// <summary>
        /// Get or set the small icon to use, when using a layout that allows small icons.
        /// </summary>
        public ImageSource SmallIcon { get => (ImageSource)GetValue(SmallIconProperty); set => SetValue(SmallIconProperty, value); }

        /// <summary>The backing dependency property for <see cref="SmallIcon"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SmallIconProperty
            = DependencyProperty.Register(nameof(SmallIcon), typeof(ImageSource), typeof(GalleryItem),
            new FrameworkPropertyMetadata(defaultValue: null));

        /// <summary>
        /// Get or set the title or label of this item.
        /// </summary>
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        /// <summary>The backing dependency property for <see cref="Title"/>. See the related property for details.</summary>
        public static readonly DependencyProperty TitleProperty
            = DependencyProperty.Register(nameof(Title), typeof(string), typeof(GalleryItem),
            new FrameworkPropertyMetadata("Item"));

        /// <summary>
        /// Get or set the corner radius for the border around the item.
        /// </summary>
        public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }

        /// <summary>The backing dependency property for <see cref="CornerRadius"/>. See the related property for details.</summary>
        public static DependencyProperty CornerRadiusProperty
            = FlatButton.CornerRadiusProperty.AddOwner(typeof(GalleryItem),
            new FrameworkPropertyMetadata(new CornerRadius(0)));

        /// <summary>The backing dependency property for <see cref="BorderSelectionThickness"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderSelectionThicknessProperty = FlatButton.BorderSelectionThicknessProperty.AddOwner(typeof(GalleryItem),
            new PropertyMetadata(new Thickness(1)));

        /// <summary>
        /// Get or set the thickness of the border around the item, while the item is in a selected (<c>IsSelected</c>) state.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the thickness of the border around the item, while the item is in a selected (IsSelected) state.")]
        public Thickness BorderSelectionThickness
        {
            get => (Thickness)GetValue(BorderSelectionThicknessProperty);
            set => SetValue(BorderSelectionThicknessProperty, value);
        }

        #endregion

        #region Brushes

        /// <summary>
        /// Get or set the brush used for the background of the control while the mouse is clicking it.
        /// </summary>
        [Category("Brushes")]
        public Brush ClickBrush
        {
            get => (Brush)GetValue(ClickBrushProperty);
            set => SetValue(ClickBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of this button while it is selected (i.e., <c>IsSelected</c> is <c>true</c>).
        /// </summary>
        [Category("Brushes")]
        public Brush SelectedBrush
        {
            get => (Brush)GetValue(SelectedBrushProperty);
            set => SetValue(SelectedBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of the control while the mouse is over it, or it has keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public Brush HighlightBrush
        {
            get => (Brush)GetValue(HighlightBrushProperty);
            set => SetValue(HighlightBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the foreground of the control while the mouse is over it, or it has keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public Brush HighlightForeground
        {
            get => (Brush)GetValue(HighlightForegroundProperty);
            set => SetValue(HighlightForegroundProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the foreground while the control is selected (i.e., <c>IsSelected</c> is <c>true</c>).
        /// </summary>
        [Category("Brushes")]
        public Brush SelectedForeground
        {
            get => (Brush)GetValue(SelectedForegroundProperty);
            set => SetValue(SelectedForegroundProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of the control when the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush DisabledBrush
        {
            get => (Brush)GetValue(DisabledBrushProperty);
            set => SetValue(DisabledBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the border of the control when the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush BorderDisabledBrush
        {
            get => (Brush)GetValue(BorderDisabledBrushProperty);
            set => SetValue(BorderDisabledBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the border while the control has the mouse over it, or it has keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public Brush BorderHighlightBrush
        {
            get => (Brush)GetValue(BorderHighlightBrushProperty);
            set => SetValue(BorderHighlightBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the border while the control is selected (i.e., <c>IsSelected</c> is <c>true</c>).
        /// </summary>
        [Category("Brushes")]
        public Brush BorderSelectedBrush
        {
            get => (Brush)GetValue(BorderSelectedBrushProperty);
            set => SetValue(BorderSelectedBrushProperty, value);
        }

        // public new static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
        //     "Background", typeof(Brush), typeof(GalleryItem),
        //     new PropertyMetadata(Colors.White.ToBrush()));

        /// <summary>The backing dependency property for <see cref="ClickBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ClickBrushProperty = FlatButton.ClickBrushProperty.AddOwner(typeof(GalleryItem),
            new PropertyMetadata(Colors.Gainsboro.ToBrush()));

        /// <summary>The backing dependency property for <see cref="SelectedBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectedBrushProperty = FlatButton.SelectedBrushProperty.AddOwner(typeof(GalleryItem),
            new PropertyMetadata(Colors.WhiteSmoke.ToBrush()));

        /// <summary>The backing dependency property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HighlightBrushProperty = FlatButton.HighlightBrushProperty.AddOwner(typeof(GalleryItem),
            new PropertyMetadata(Colors.LightGray.ToBrush()));

        /// <summary>The backing dependency property for <see cref="HighlightForeground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HighlightForegroundProperty = FlatButton.HighlightForegroundProperty.AddOwner(typeof(GalleryItem),
            new PropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>The backing dependency property for <see cref="SelectedForeground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectedForegroundProperty = FlatButton.SelectedForegroundProperty.AddOwner(typeof(GalleryItem),
            new PropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>The backing dependency property for <see cref="DisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty DisabledBrushProperty = FlatButton.DisabledBrushProperty.AddOwner(typeof(GalleryItem),
            new PropertyMetadata(Colors.Gray.ToBrush()));

        /// <summary>The backing dependency property for <see cref="BorderDisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderDisabledBrushProperty = FlatButton.BorderDisabledBrushProperty.AddOwner(typeof(GalleryItem),
            new PropertyMetadata(Colors.DarkGray.ToBrush()));

        /// <summary>The backing dependency property for <see cref="BorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderHighlightBrushProperty = FlatButton.BorderHighlightBrushProperty.AddOwner(typeof(GalleryItem),
            new PropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>The backing dependency property for <see cref="BorderSelectedBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderSelectedBrushProperty = FlatButton.BorderSelectedBrushProperty.AddOwner(typeof(GalleryItem),
            new PropertyMetadata(Colors.DimGray.ToBrush()));

        #endregion

        #region Click Handling

        #region Routed Events

        /// <summary>
        /// The backing value for the <see cref="Click"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GalleryItem));

        /// <summary>
        /// Raised when the user clicks on the main button (not the menu button), via a mouse click or via the keyboard.
        /// </summary>
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        /// <summary>
        /// The backing value for the <see cref="RightClick"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent RightClickEvent = EventManager.RegisterRoutedEvent(
            "RightClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GalleryItem));

        /// <summary>
        /// Raised when the user right-clicks on the button, via a mouse click or via the keyboard.
        /// </summary>
        public event RoutedEventHandler RightClick
        {
            add { AddHandler(RightClickEvent, value); }
            remove { RemoveHandler(RightClickEvent, value); }
        }

        #endregion

        #region IsMouseDown

        // from https://stackoverflow.com/questions/10667545/why-ismouseover-is-recognized-and-mousedown-isnt-wpf-style-trigger

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected static readonly DependencyPropertyKey IsMouseDownPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsMouseDown",
            typeof(bool), typeof(GalleryItem), new FrameworkPropertyMetadata(false));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get if there is a mouse button currently being pressed, while the mouse cursor is over this control.
        /// </summary>
        public static readonly DependencyProperty IsMouseDownProperty = IsMouseDownPropertyKey.DependencyProperty;

        /// <summary>
        /// Set the IsMouseDown property for a GalleryItem.
        /// </summary>
        /// <param name="obj">The GalleryItem to apply the property change to.</param>
        /// <param name="value">The new value to set for the property.</param>
        protected static void SetIsMouseDown(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMouseDownPropertyKey, value);
        }

        /// <summary>
        /// Get the IsMouseDown property for a GalleryItem.
        /// </summary>
        /// <param name="obj">The GalleryItem to get the property value from.</param>
        public static bool GetIsMouseDown(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMouseDownProperty);
        }

        #endregion

        #region Variables/Properties

        #region IsSelected / IsSelectedChanged

        bool _runSelChangeEvent = true;

        /// <summary>
        /// The backing dependency property for <see cref="IsSelected"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(GalleryItem),
            new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));

        /// <summary>
        /// Gets or sets whether this GalleryItem is selected. If <see cref="SelectOnClick"/> is <c>true</c>, this state will toggle when this item is clicked.
        /// </summary>
        /// <remarks>
        /// A selected GalleryItem will have a constant highlighted background (set via <see cref="SelectedBrush"/>) to show that it is selected.
        /// This can be used in situations where a Gallery is being used to select one or more options, with the currently active option having <c>IsSelected</c> set to <c>true</c>.
        /// </remarks>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool se)
            {
                bool old = (e.OldValue is bool oval) ? oval : false;

                if (d is GalleryItem f)
                {
                    if (f._runSelChangeEvent)
                    {
                        ItemSelectionChangedEventArgs re = new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, old, se, SelectionChangeTrigger.CodeUnknown, null);
                        f.RaiseEvent(re);
                    }
                }
            }
        }

        /// <summary>
        /// The backing value for the <see cref="IsSelectedChanged"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent IsSelectedChangedEvent = EventManager.RegisterRoutedEvent(
            "IsSelectedChanged", RoutingStrategy.Bubble, typeof(ItemSelectionChangedEventHandler), typeof(GalleryItem));

        /// <summary>
        /// Raised when the user clicks on the main button (not the menu button), via a mouse click or via the keyboard.
        /// </summary>
        public event ItemSelectionChangedEventHandler IsSelectedChanged
        {
            add { AddHandler(IsSelectedChangedEvent, value); }
            remove { RemoveHandler(IsSelectedChangedEvent, value); }
        }

        /// <summary>
        /// Set the <see cref="IsSelected"/> value of this control, while also defining how the selection was changed.
        /// </summary>
        /// <param name="value">The value to set <see cref="IsSelected"/> to.</param>
        /// <param name="trigger">The source or method used to trigger the change in selection.</param>
        /// <param name="triggerSource">The object that triggered the change.</param>
#if NETCOREAPP
        public void SetIsSelectedWithSource(bool value, SelectionChangeTrigger trigger, object? triggerSource = null)
#else
        public void SetIsSelectedWithSource(bool value, SelectionChangeTrigger trigger, object triggerSource = null)
#endif
        {
            bool old = IsSelected;

            _runSelChangeEvent = false;
            IsSelected = value;
            _runSelChangeEvent = true;

            ItemSelectionChangedEventArgs re = new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, old, value, trigger, triggerSource);
            RaiseEvent(re);
        }
        #endregion

        /// <summary>
        /// The backing dependency property for <see cref="SelectOnClick"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty SelectOnClickProperty = DependencyProperty.Register(
            "SelectOnClick", typeof(bool), typeof(GalleryItem),
            new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether the button should change its IsSelected property when a click is performed. With this enabled, this allows the button to take on the functionality of a ToggleButton.
        /// </summary>
        /// <remarks>
        /// While SelectOnClick is true, the button will toggle between <see cref="IsSelected"/> being true and false (similar to a ToggleButton). A selected button will, by default, have some visual
        /// differences to help make it look distinct from unselected buttons. The button's Click event will still be raised while this property is set to <c>true</c>, but the event occurs after the
        /// IsSelected property has already changed. While you could use the Click event to check when the button's IsSelected property is changed, it is better to use the IsSelectedChanged event,
        /// in case of situations where IsSelected is changed via methods other than clicking, such as programmatically or via WPF binding.
        /// </remarks>
        [Category("Common")]
        public bool SelectOnClick
        {
            get => (bool)GetValue(SelectOnClickProperty);
            set => SetValue(SelectOnClickProperty, value);
        }

        #endregion

        #region Base Functions

        /// <summary>
        /// Get if this control is currently in a "highlight" state.
        /// <para/>
        /// Use <see cref="Highlight"/> and <see cref="Unhighlight"/> to change states.
        /// </summary>
        protected bool Highlighting { get; private set; } = false;

        bool performingClick = false;
        bool rightClick = false;

        /// <summary>
        /// Change the control to a highlighted state, while it has focus or has the mouse or stylus over it.
        /// </summary>
        protected virtual void Highlight()
        {
            Highlighting = true;
        }

        /// <summary>
        /// Change the control to an unhighlighted state, when it no longer has focus or has the mouse or stylus over it.
        /// </summary>
        protected virtual void Unhighlight()
        {
            Highlighting = false;
        }

        void InitiateClick()
        {
            performingClick = true;

            Background = ClickBrush;
            Foreground = HighlightForeground;
        }

        void PerformRightClick()
        {
            RoutedEventArgs rre = new RoutedEventArgs(RightClickEvent);
            RaiseEvent(rre);
        }

        /// <summary>
        /// Perform a click on this control programmatically. This responds the same way as if it was clicked by the user.
        /// </summary>
        public void DoClick()
        {
            OnClick();
        }

        /// <summary>
        /// Defines the actions the button performs when it is clicked.
        /// </summary>
        protected void OnClick()
        {
            if (SelectOnClick)
            {
                SetIsSelectedWithSource(true, SelectionChangeTrigger.ControlClick, this);
            }

            RoutedEventArgs rre = new RoutedEventArgs(ClickEvent);
            RaiseEvent(rre);
        }

        void PerformClick()
        {
            if (performingClick)
            {
                if (rightClick)
                {
                    PerformRightClick();
                    rightClick = false;
                }
                else
                {
                    OnClick();
                }

                performingClick = false;
            }
        }

        #endregion

        #region Base Event Handlers

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            InitiateClick();
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                rightClick = true;
            }

            PerformClick();
        }

#if NETCOREAPP
        private void UserControl_TouchDown(object? sender, TouchEventArgs e)
        {
            InitiateClick();
        }

        private void UserControl_TouchUp(object? sender, TouchEventArgs e)
        {
            PerformClick();
        }
#else
        private void UserControl_TouchDown(object sender, TouchEventArgs e)
        {
            InitiateClick();
        }

        private void UserControl_TouchUp(object sender, TouchEventArgs e)
        {
            PerformClick();
        }
#endif

        private void UserControl_StylusDown(object sender, StylusDownEventArgs e)
        {
            InitiateClick();
        }

        private void UserControl_StylusUp(object sender, StylusEventArgs e)
        {
            PerformClick();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Highlight();
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Unhighlight();
        }

#if NETCOREAPP
        private void UserControl_TouchEnter(object? sender, TouchEventArgs e)
        {
            Highlight();
        }

        private void UserControl_TouchLeave(object? sender, TouchEventArgs e)
        {
            Unhighlight();
        }

#else
        private void UserControl_TouchEnter(object sender, TouchEventArgs e)
        {
            Highlight();
        }

        private void UserControl_TouchLeave(object sender, TouchEventArgs e)
        {
            Unhighlight();
        }
#endif

        private void UserControl_StylusEnter(object sender, StylusEventArgs e)
        {
            Highlight();
        }

        private void UserControl_StylusLeave(object sender, StylusEventArgs e)
        {
            Unhighlight();
        }

        private void UserControl_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Highlight();
        }

        private void UserControl_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Unhighlight();
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                performingClick = true;
                PerformClick();
            }
            else if (e.Key == Key.Apps)
            {
                performingClick = true;
                rightClick = true;
                PerformClick();
            }
        }

        #endregion

        #endregion
    }
}
