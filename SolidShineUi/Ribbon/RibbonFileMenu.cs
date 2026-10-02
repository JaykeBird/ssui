using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    [ContentProperty(nameof(Items))]
    public class RibbonFileMenu : ThemedControl
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
            // Click += RibbonFileMenu_Click;
            //PreviewMouseDown += control_PreviewMouseDown;
            //PreviewMouseUp += control_PreviewMouseUp;
            //MouseLeave += control_MouseLeave;

            SetValue(ItemsPropertyKey, new ObservableCollection<IRibbonItem>());
            Items.CollectionChanged += Items_CollectionChanged;

            IsEnabledChanged += control_IsEnabledChanged;
        }


        #region Template Handling

        bool itemsLoaded = false;

#if NETCOREAPP
        Popup? PART_Popup = null;
        ISsuiButton? mainButton = null;
#else
        Popup PART_Popup = null;
        ISsuiButton mainButton = null;
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
                mainButton = (ISsuiButton)GetTemplateChild("btn_Border");

                if (PART_Popup != null && mainButton != null)
                {
                    itemsLoaded = true;

                    mainButton.Click += RibbonFileMenu_Click;
                }
            }
        }

        #endregion

        #region Button Content

        /// <summary>
        /// Get or set the title to display on the File menu button itself. A shorter title is recommended, such as "File" or "App".
        /// </summary>
        [Category("Common")]
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        /// <summary>The backing dependency property for <see cref="Title"/>. See the related property for details.</summary>
        public static readonly DependencyProperty TitleProperty
            = DependencyProperty.Register(nameof(Title), typeof(string), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata("File"));

        /// <summary>
        /// Get or set if an arrow should be shown to the right of the File button text to indicate the button as a menu button.
        /// </summary>
        [Category("Common")]
        public bool ShowMenuArrow { get => (bool)GetValue(ShowMenuArrowProperty); set => SetValue(ShowMenuArrowProperty, value); }

        /// <summary>The backing dependency property for <see cref="ShowMenuArrow"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ShowMenuArrowProperty 
            = DependencyProperty.Register(nameof(ShowMenuArrow), typeof(bool), typeof(RibbonFileMenu),
            new PropertyMetadata(false));

        #endregion

        #region Items

        /// <summary>
        /// Get or set the list of items in this File menu. This Items property can be used to add and remove items.
        /// </summary>
        [Category("Common")]
        public ObservableCollection<IRibbonItem> Items
        {
            get { return (ObservableCollection<IRibbonItem>)GetValue(ItemsProperty); }
            private set { SetValue(ItemsPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey ItemsPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(Items), typeof(ObservableCollection<IRibbonItem>), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(new ObservableCollection<IRibbonItem>()));

        /// <summary>The backing dependency property for <see cref="Items"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

#if NETCOREAPP
        private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#else
        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#endif
        {
            // throw new NotImplementedException();
        }

#endregion

        #region ColorScheme / SsuiTheme

        /// <summary>
        /// Get or set the color scheme used for this control. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        public ColorScheme ColorScheme { get => (ColorScheme)GetValue(ColorSchemeProperty); set => SetValue(ColorSchemeProperty, value); }

        /// <summary>The backing dependency property for <see cref="ColorScheme"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ColorSchemeProperty
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
                ApplyThemeBinding(ForegroundProperty, SsuiTheme.ForegroundProperty, theme);
                // Border brush already applied in base

                ApplyThemeBinding(BorderHighlightBrushProperty, SsuiTheme.HighlightBorderBrushProperty, theme);
                ApplyThemeBinding(DisabledBrushProperty, SsuiTheme.DisabledBackgroundProperty, theme);
                ApplyThemeBinding(BorderDisabledBrushProperty, SsuiTheme.DisabledBorderBrushProperty, theme);

                ApplyThemeBinding(MenuBackgroundProperty, SsuiTheme.PanelBackgroundProperty, theme);
                ApplyThemeBinding(MenuSecondaryPanelBackgroundProperty, SsuiTheme.ControlBackgroundProperty, theme);

                // we'll leave the button background brushes alone, in case the user has them specially set
                
                if (useLightBorder)
                {
                    ApplyThemeBinding(MenuBorderBrushProperty, SsuiTheme.LightBorderBrushProperty, theme);
                    ApplyThemeBinding(PanelDividerBrushProperty, SsuiTheme.LightBorderBrushProperty, theme);
                }
                else
                {
                    ApplyThemeBinding(MenuBorderBrushProperty, SsuiTheme.BorderBrushProperty, theme);
                    ApplyThemeBinding(PanelDividerBrushProperty, SsuiTheme.BorderBrushProperty, theme);
                }
            }

            // TODO: add ColorButtonWithAccentTheme property to use accent theme colors to set button backgrounds
        }

        #endregion

        #region Brushes

        /// <summary>
        /// Get or set the brush to use for the background of the File menu button.
        /// </summary>
        public Brush ButtonBackgroundBrush { get => (Brush)GetValue(ButtonBackgroundBrushProperty); set => SetValue(ButtonBackgroundBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ButtonBackgroundBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ButtonBackgroundBrushProperty
            = DependencyProperty.Register(nameof(ButtonBackgroundBrush), typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Colors.CornflowerBlue.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the File menu button when it is highlighted (has the mouse/pointer over it).
        /// </summary>
        /// <remarks>
        /// A semi-transparent brush can be used to allow the base color of <see cref="ButtonBackgroundBrush"/> to come through.
        /// </remarks>
        public Brush ButtonHighlightBrush { get => (Brush)GetValue(ButtonHighlightBrushProperty); set => SetValue(ButtonHighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ButtonHighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ButtonHighlightBrushProperty
            = DependencyProperty.Register(nameof(ButtonHighlightBrush), typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(BrushFactory.Create("5077bd"))); 

        /// <summary>
        /// Get or set the brush to use for the File menu button when it is being clicked (the mouse button is being pressed).
        /// </summary>
        /// <remarks>
        /// A semi-transparent brush can be used to allow the base color of <see cref="ButtonBackgroundBrush"/> to come through.
        /// </remarks>
        public Brush ButtonClickBrush { get => (Brush)GetValue(ButtonClickBrushProperty); set => SetValue(ButtonClickBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ButtonClickBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ButtonClickBrushProperty
            = DependencyProperty.Register(nameof(ButtonClickBrush), typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(BrushFactory.Create("4668a5")));

        /// <summary>
        /// Get or set the brush to use for the border around the menu popup of this control.
        /// </summary>
        public Brush MenuBorderBrush { get => (Brush)GetValue(MenuBorderBrushProperty); set => SetValue(MenuBorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuBorderBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuBorderBrushProperty
            = DependencyProperty.Register(nameof(MenuBorderBrush), typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Colors.DarkGray.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of the main area of the menu popup of this control.
        /// </summary>
        public Brush MenuBackground { get => (Brush)GetValue(MenuBackgroundProperty); set => SetValue(MenuBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuBackgroundProperty
            = DependencyProperty.Register(nameof(MenuBackground), typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Colors.White.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of the secondary area of the menu popup of this control.
        /// </summary>
        public Brush MenuSecondaryPanelBackground { get => (Brush)GetValue(MenuSecondaryPanelBackgroundProperty); set => SetValue(MenuSecondaryPanelBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuSecondaryPanelBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuSecondaryPanelBackgroundProperty
            = DependencyProperty.Register(nameof(MenuSecondaryPanelBackground), typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Colors.Gainsboro.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of the button when the control is disabled.
        /// </summary>
        public Brush DisabledBrush { get => (Brush)GetValue(DisabledBrushProperty); set => SetValue(DisabledBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="DisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty DisabledBrushProperty
            = DependencyProperty.Register(nameof(DisabledBrush), typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Colors.Gainsboro.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of the border of the button when this control is disabled.
        /// </summary>
        public Brush BorderDisabledBrush { get => (Brush)GetValue(BorderDisabledBrushProperty); set => SetValue(BorderDisabledBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="BorderDisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderDisabledBrushProperty
            = DependencyProperty.Register(nameof(BorderDisabledBrush), typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Colors.DimGray.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of the border of the button when this control is highlighted (e.g., mouse over).
        /// </summary>
        public Brush BorderHighlightBrush { get => (Brush)GetValue(BorderHighlightBrushProperty); set => SetValue(BorderHighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="BorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderHighlightBrushProperty
            = DependencyProperty.Register(nameof(BorderHighlightBrush), typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Colors.Gray.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the divider between the left main part of the menu and the secondary panel on the right side.
        /// </summary>
        public Brush PanelDividerBrush { get => (Brush)GetValue(PanelDividerBrushProperty); set => SetValue(PanelDividerBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="PanelDividerBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty PanelDividerBrushProperty
            = DependencyProperty.Register(nameof(PanelDividerBrush), typeof(Brush), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(Colors.DimGray.ToBrush()));

        #endregion

        #region Menu Handling

        // from WPF MenuItem

        /// <summary>The backing dependency property for <see cref="IsSubmenuOpenProperty"/>. See the related property for details.</summary>
        public static readonly DependencyProperty IsSubmenuOpenProperty
            = DependencyProperty.Register(nameof(IsSubmenuOpen), typeof(bool), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get or set if a submenu in this control is currently open.
        /// </summary>
        [Browsable(false)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSubmenuOpen
        {
            get => (bool)GetValue(IsSubmenuOpenProperty);
            set => SetValue(IsSubmenuOpenProperty, value);
        }

        /// <summary>
        /// Get or set if the menu of this control should stay open even after an item is selected/clicked.
        /// </summary>
        public bool MenuStaysOpen { get => (bool)GetValue(MenuStaysOpenProperty); set => SetValue(MenuStaysOpenProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuStaysOpen"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuStaysOpenProperty
            = DependencyProperty.Register(nameof(MenuStaysOpen), typeof(bool), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get or set the minimum height that the File menu can be; if there aren't enough items to fill in the full vertical space, 
        /// some blank space will be present at the bottom of the menu.
        /// </summary>
        public double MenuMinHeight { get => (double)GetValue(MenuMinHeightProperty); set => SetValue(MenuMinHeightProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuMinHeight"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuMinHeightProperty
            = DependencyProperty.Register(nameof(MenuMinHeight), typeof(double), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(150.0));

        /// <summary>
        /// Get or set the maximum height that the File menu can be; if there are more items than can fit, a scrollbar will appear.
        /// </summary>
        public double MenuMaxHeight { get => (double)GetValue(MenuMaxHeightProperty); set => SetValue(MenuMaxHeightProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuMaxHeight"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuMaxHeightProperty
            = DependencyProperty.Register(nameof(MenuMaxHeight), typeof(double), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(600.0));

        private void control_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsEnabled && IsSubmenuOpen)
            {
                IsSubmenuOpen = false;
            }
        }

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
            RibbonFileMenu_Click(this, new RoutedEventArgs(ButtonBase.ClickEvent, this));
        }

        #endregion

        #region Secondary Panel

        /// <summary>
        /// Get or set the desired width for the secondary panel on the right side of the File menu.
        /// </summary>
        public GridLength SecondaryPanelWidth { get => (GridLength)GetValue(SecondaryPanelWidthProperty); set => SetValue(SecondaryPanelWidthProperty, value); }

        /// <summary>The backing dependency property for <see cref="SecondaryPanelWidth"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SecondaryPanelWidthProperty
            = DependencyProperty.Register(nameof(SecondaryPanelWidth), typeof(GridLength), typeof(RibbonFileMenu),
            new FrameworkPropertyMetadata(new GridLength(150.0)));

        /// <summary>
        /// Get or set the content to display on the secondary panel on the right side of the File menu.
        /// <para/>
        /// Note that when an item in the File menu has its submenu open, that submenu will cover this content.
        /// </summary>
        /// <remarks>
        /// Suggestions for the content here could be a list of recently opened files, or suggested quick actions (that aren't too context-specific).
        /// </remarks>
        public object SecondaryPanelContent { get => GetValue(SecondaryPanelContentProperty); set => SetValue(SecondaryPanelContentProperty, value); }

        /// <summary>The backing dependency property for <see cref="SecondaryPanelContent"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SecondaryPanelContentProperty
            = DependencyProperty.Register(nameof(SecondaryPanelContent), typeof(object), typeof(RibbonFileMenu));

        #endregion

        #endregion
    }
}
