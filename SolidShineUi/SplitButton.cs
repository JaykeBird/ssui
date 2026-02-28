using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace SolidShineUi
{
    /// <summary>
    /// A control that displays two buttons: a main button on the left which activates like a normal button, 
    /// and a secondary button on the right that displays a menu of additional options.
    /// </summary>
    [Localizability(LocalizationCategory.Button)]
    public class SplitButton : ThemedContentControl, IClickSelectableControl
    {
        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
        }

        /// <summary>
        /// Create a new SplitButton.
        /// </summary>
        public SplitButton()
        {
            ColorSchemeChanged += OnColorSchemeChanged;

            DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(Control)).AddValueChanged(this, (s, e) => ApplyValueToButtons(BackgroundProperty, Background));
            DependencyPropertyDescriptor.FromProperty(BorderBrushProperty, typeof(Control)).AddValueChanged(this, (s, e) => ApplyValueToButtons(BorderBrushProperty, BorderBrush));
            DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(Control)).AddValueChanged(this, (s, e) => ApplyValueToButtons(ForegroundProperty, Foreground));
            DependencyPropertyDescriptor.FromProperty(UseAccentThemeProperty, typeof(ThemedContentControl)).AddValueChanged(this, (s, e) => UpdateFromUseAccentTheme(UseAccentTheme));
            DependencyPropertyDescriptor.FromProperty(BorderThicknessProperty, typeof(Control)).AddValueChanged(this, (s, e) => ApplyBorderThicknessToButtons(BorderThickness));

        }

        private void OnColorSchemeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            ColorScheme cs = (e.NewValue as ColorScheme)!;
#else
            ColorScheme cs = e.NewValue as ColorScheme;
#endif

            Menu?.ApplyColorScheme(cs);
        }

        #region Template IO
        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            LoadTemplateItems();

            if (itemsLoaded)
            {
                // the itemsLoaded value will only be true if both controls are not null
#if NETCOREAPP
                btnMain!.Click += btnMain_Click;
                btnMenu!.Click += btnMenu_Click;

                btnMain.RightClick += btnMain_RightClick;
                btnMenu.RightClick += btnMenu_RightClick;
#else
                btnMain.Click += btnMain_Click;
                btnMenu.Click += btnMenu_Click;

                btnMain.RightClick += btnMain_RightClick;
                btnMenu.RightClick += btnMenu_RightClick;
#endif
            }
        }

        bool itemsLoaded = false;

#if NETCOREAPP
        FlatButton? btnMain = null;
        FlatButton? btnMenu = null;
#else
        FlatButton btnMain = null;
        FlatButton btnMenu = null;
#endif

        void LoadTemplateItems()
        {
            if (!itemsLoaded)
            {
                btnMain = (FlatButton)GetTemplateChild("PART_Main");
                btnMenu = (FlatButton)GetTemplateChild("PART_Menu");

                if (btnMain != null && btnMenu != null)
                {
                    itemsLoaded = true;
                }
            }
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
        /// Get or set the brush used for the background of this button while it is selected
        /// (i.e. the <c>IsSelected</c> property is true).
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
        public Brush ForegroundHighlightBrush
        {
            get => (Brush)GetValue(ForegroundHighlightBrushProperty);
            set => SetValue(ForegroundHighlightBrushProperty, value);
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
        /// Get or set the brush used for the border while the control is selected
        /// (i.e. the <c>IsSelected</c> property is true).
        /// </summary>
        [Category("Brushes")]
        public Brush BorderSelectedBrush
        {
            get => (Brush)GetValue(BorderSelectedBrushProperty);
            set => SetValue(BorderSelectedBrushProperty, value);
        }

        /// <summary>The backing dependency property for <see cref="ClickBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ClickBrushProperty = FlatButton.ClickBrushProperty.AddOwner(typeof(SplitButton),
            new PropertyMetadata(ApplyPropertyUpdate));

        /// <summary>The backing dependency property for <see cref="SelectedBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectedBrushProperty = FlatButton.SelectedBrushProperty.AddOwner(typeof(SplitButton),
            new PropertyMetadata(ApplyPropertyUpdate));

        /// <summary>The backing dependency property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HighlightBrushProperty = FlatButton.HighlightBrushProperty.AddOwner(typeof(SplitButton),
            new PropertyMetadata(ApplyPropertyUpdate));

        /// <summary>The backing dependency property for <see cref="ForegroundHighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ForegroundHighlightBrushProperty = FlatButton.ForegroundHighlightBrushProperty.AddOwner(typeof(SplitButton),
            new PropertyMetadata(ApplyPropertyUpdate));

        /// <summary>The backing dependency property for <see cref="DisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty DisabledBrushProperty = FlatButton.DisabledBrushProperty.AddOwner(typeof(SplitButton),
            new PropertyMetadata(ApplyPropertyUpdate));

        /// <summary>The backing dependency property for <see cref="BorderDisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderDisabledBrushProperty = FlatButton.BorderDisabledBrushProperty.AddOwner(typeof(SplitButton),
            new PropertyMetadata(ApplyPropertyUpdate));

        /// <summary>The backing dependency property for <see cref="BorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderHighlightBrushProperty = FlatButton.BorderHighlightBrushProperty.AddOwner(typeof(SplitButton),
            new PropertyMetadata(ApplyPropertyUpdate));

        /// <summary>The backing dependency property for <see cref="BorderSelectedBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderSelectedBrushProperty = FlatButton.BorderSelectedBrushProperty.AddOwner(typeof(SplitButton),
            new PropertyMetadata(ApplyPropertyUpdate));

        #endregion

        #region TransparentBack

        /// <summary>
        /// The backing dependency property object for <see cref="TransparentBack"/>. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty TransparentBackProperty = FlatButton.TransparentBackProperty.AddOwner(typeof(SplitButton),
            new PropertyMetadata(false, ApplyPropertyUpdate));

        /// <summary>
        /// Get or set whether the button should have a transparent background when the button is not focused.
        /// </summary>
        [Category("Common")]
        [Description("Get or set whether the button should have a transparent background when the button is not focused.")]
        public bool TransparentBack
        {
            get => (bool)GetValue(TransparentBackProperty);
            set => SetValue(TransparentBackProperty, value);
        }

        #endregion

        #region SsuiTheme

        /// <inheritdoc/>
        protected override void OnApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
        {
            base.OnApplySsuiTheme(ssuiTheme, useLightBorder, useAccentTheme);

            Menu?.SetBinding(ThemedControl.SsuiThemeProperty, new Binding(nameof(SsuiTheme)) { Source = this });

            if (useAccentTheme && ssuiTheme is SsuiAppTheme sat)
            {
                // ApplyThemeBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty, sat.AccentTheme);
                ApplyTheme(sat.AccentTheme);
            }
            else
            {
                // ApplyThemeBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty);
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
                ApplyThemeBinding(ForegroundHighlightBrushProperty, SsuiTheme.HighlightForegroundProperty, theme);
                ApplyThemeBinding(ClickBrushProperty, SsuiTheme.ClickBrushProperty, theme);

                ApplyThemeBinding(CornerRadiusProperty, SsuiTheme.CornerRadiusProperty, theme);
            }
        }

        /// <summary>
        /// Internal function used for debugging the state of the internal buttons.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected string GetMainButtonBackgroundBinding
        {
            get
            {
                if (btnMain != null)
                {
                    Binding b = BindingOperations.GetBinding(btnMain, BackgroundProperty);
                    
                    if (b != null)
                    {
                        return $"{b.Source}: {b.Path.Path} (+ {btnMain?.SsuiTheme?.ToString() ?? "(theme null)"})";
                    }
                    else
                    {
                        return "(no binding)";
                    }
                }

                return "(not found)";
            }
        }

        void UpdateFromUseAccentTheme(bool useAccentTheme)
        {
            if (SsuiTheme != null)
            {
                ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);
            }
            else
            {
                ApplyColorScheme(ColorScheme, useAccentTheme);
            }
        }

        ///// <summary>
        ///// Remove the bindings the split button uses internally for setting the <c>SsuiTheme</c>.
        ///// This may be necessary in certain rare instances where you are setting the theme, but then also need to update other
        ///// properties (such as <see cref="CornerRadius"/>) afterwards.
        ///// However, in most situations, this is not needed and instead may cause other problems.
        ///// </summary>
        //public void ClearButtonThemeBindings()
        //{
        //    if (btnMain != null && btnMenu != null)
        //    {
        //        BindingOperations.ClearBinding(btnMain, ThemedControl.SsuiThemeProperty);
        //        BindingOperations.ClearBinding(btnMenu, ThemedControl.SsuiThemeProperty);

        //        // so as to not leave the buttons' themes undefined, we'll set the themes as unbound values
        //        // (actually, I won't do this, the intention is the brushes will revert to SplitButton's bindings ideally)
        //        //SsuiTheme copy = (SsuiTheme)SsuiTheme.Clone();

        //        //btnMain.SsuiTheme = copy;
        //        //btnMenu.SsuiTheme = copy;
        //    }
        //}

        #endregion

        #region Apply Values to Buttons

        private static void ApplyPropertyUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SplitButton sb)
            {
                sb.ApplyValueToButtons(e.Property, e.NewValue);
            }
        }

        private void ApplyValueToButtons(DependencyProperty property, object value)
        {
            btnMain?.SetValue(property, value);
            btnMenu?.SetValue(property, value);
        }

        void ApplyCornerRadiusToButtons(CornerRadius cr)
        {
            if (btnMenu == null || btnMain == null) return;

            switch (MenuButtonPlacement)
            {
                case PlacementDirection.Hidden:
                    // no menu button
                    // apply all to main button
                    btnMenu.CornerRadius = new CornerRadius(0);
                    btnMain.CornerRadius = new CornerRadius(cr.TopLeft, cr.TopRight, cr.BottomRight, cr.BottomLeft);
                    break;
                case PlacementDirection.Top:
                    // menu button on top (top left, top right)
                    // main button on bottom
                    btnMenu.CornerRadius = new CornerRadius(cr.TopLeft, cr.TopRight, 0, 0);
                    btnMain.CornerRadius = new CornerRadius(0, 0, cr.BottomRight, cr.BottomLeft);
                    break;
                case PlacementDirection.Left:
                    // menu button on left (top left, bottom left)
                    // main button on right
                    btnMenu.CornerRadius = new CornerRadius(cr.TopLeft, 0, 0, cr.BottomLeft);
                    btnMain.CornerRadius = new CornerRadius(0, cr.TopRight, cr.BottomRight, 0);
                    break;
                case PlacementDirection.Right:
                    // menu button on right (top right, bottom right)
                    // main button on left
                    btnMenu.CornerRadius = new CornerRadius(0, cr.TopRight, cr.BottomRight, 0);
                    btnMain.CornerRadius = new CornerRadius(cr.TopLeft, 0, 0, cr.BottomLeft);
                    break;
                case PlacementDirection.Bottom:
                    // menu button on bottom (bottom left, bottom right)
                    // main button on top
                    btnMenu.CornerRadius = new CornerRadius(0, 0, cr.BottomRight, cr.BottomLeft);
                    btnMain.CornerRadius = new CornerRadius(cr.TopLeft, cr.TopRight, 0, 0);
                    break;
                default:
                    // treat it as Right, as that's the default value
                    btnMenu.CornerRadius = new CornerRadius(0, cr.TopRight, cr.BottomRight, 0);
                    btnMain.CornerRadius = new CornerRadius(cr.TopLeft, 0, 0, cr.BottomLeft);
                    break;
            }
        }

        void ApplyBorderThicknessToButtons(Thickness th)
        {
            if (btnMenu == null || btnMain == null) return;

            byte menuVal = 0;
            byte mainVal = 0;

            switch (MenuButtonPlacement)
            {
                case PlacementDirection.Hidden:
                    // no menu button
                    // apply all to main button
                    menuVal = PartialValueHelper.EncodeThicknessPartialValue(false, false, false, false);
                    mainVal = PartialValueHelper.EncodeThicknessPartialValue(true, true, true, true);
                    break;
                case PlacementDirection.Top:
                    // menu button on top (top, left, right)
                    // main button on bottom
                    menuVal = PartialValueHelper.EncodeThicknessPartialValue(left: true, top: true, right: true, bottom: false);
                    mainVal = PartialValueHelper.EncodeThicknessPartialValue(left: true, top: false, right: true, bottom: true);
                    break;
                case PlacementDirection.Left:
                    // menu button on left (left, top, bottom)
                    // main button on right
                    menuVal = PartialValueHelper.EncodeThicknessPartialValue(left: true, top: true, right: false, bottom: true);
                    mainVal = PartialValueHelper.EncodeThicknessPartialValue(left: false, top: true, right: true, bottom: true);
                    break;
                case PlacementDirection.Right:
                    // menu button on right (right, top, bottom)
                    // main button on left
                    menuVal = PartialValueHelper.EncodeThicknessPartialValue(left: false, top: true, right: true, bottom: true);
                    mainVal = PartialValueHelper.EncodeThicknessPartialValue(left: true, top: true, right: false, bottom: true);
                    break;
                case PlacementDirection.Bottom:
                    // menu button on bottom (bottom, left, right)
                    // main button on top
                    menuVal = PartialValueHelper.EncodeThicknessPartialValue(left: true, top: false, right: true, bottom: true);
                    mainVal = PartialValueHelper.EncodeThicknessPartialValue(left: true, top: true, right: true, bottom: false);
                    break;
                default:
                    // treat it as Right, as that's the default value
                    menuVal = PartialValueHelper.EncodeThicknessPartialValue(left: false, top: true, right: true, bottom: true);
                    mainVal = PartialValueHelper.EncodeThicknessPartialValue(left: true, top: true, right: false, bottom: true);
                    break;
            }

            btnMenu.BorderThickness = PartialValueHelper.GetThicknessPartialValue(th, menuVal);
            btnMain.BorderThickness = PartialValueHelper.GetThicknessPartialValue(th, mainVal);
        }

        void ApplyBorderSelectionThicknessToButtons(Thickness th)
        {
            if (btnMenu == null || btnMain == null) return;

            byte menuVal = 0;
            byte mainVal = 0;

            switch (MenuButtonPlacement)
            {
                case PlacementDirection.Hidden:
                    // no menu button
                    // apply all to main button
                    menuVal = PartialValueHelper.EncodeThicknessPartialValue(false, false, false, false);
                    mainVal = PartialValueHelper.EncodeThicknessPartialValue(true, true, true, true);
                    break;
                case PlacementDirection.Top:
                    // menu button on top (top, left, right)
                    // main button on bottom
                    menuVal = PartialValueHelper.EncodeThicknessPartialValue(left: true, top: true, right: true, bottom: false);
                    mainVal = PartialValueHelper.EncodeThicknessPartialValue(left: true, top: false, right: true, bottom: true);
                    break;
                case PlacementDirection.Left:
                    // menu button on left (left, top, bottom)
                    // main button on right
                    menuVal = PartialValueHelper.EncodeThicknessPartialValue(left: true, top: true, right: false, bottom: true);
                    mainVal = PartialValueHelper.EncodeThicknessPartialValue(left: false, top: true, right: true, bottom: true);
                    break;
                case PlacementDirection.Right:
                    // menu button on right (right, top, bottom)
                    // main button on left
                    menuVal = PartialValueHelper.EncodeThicknessPartialValue(left: false, top: true, right: true, bottom: true);
                    mainVal = PartialValueHelper.EncodeThicknessPartialValue(left: true, top: true, right: false, bottom: true);
                    break;
                case PlacementDirection.Bottom:
                    // menu button on bottom (bottom, left, right)
                    // main button on top
                    menuVal = PartialValueHelper.EncodeThicknessPartialValue(left: true, top: false, right: true, bottom: true);
                    mainVal = PartialValueHelper.EncodeThicknessPartialValue(left: true, top: true, right: true, bottom: false);
                    break;
                default:
                    // treat it as Right, as that's the default value
                    menuVal = PartialValueHelper.EncodeThicknessPartialValue(left: false, top: true, right: true, bottom: true);
                    mainVal = PartialValueHelper.EncodeThicknessPartialValue(left: true, top: true, right: false, bottom: true);
                    break;
            }

            btnMenu.BorderSelectionThickness = PartialValueHelper.GetThicknessPartialValue(th, menuVal);
            btnMain.BorderSelectionThickness = PartialValueHelper.GetThicknessPartialValue(th, mainVal);
        }

        #endregion

        #region ColorScheme/UseAccentColors

        /// <summary>
        /// Raised when the ColorScheme property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif
        
        /// <summary>The backing dependency property for <see cref="ColorScheme"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register(nameof(ColorScheme), typeof(ColorScheme), typeof(SplitButton),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        private static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ColorScheme cs)
            {
                if (d is SplitButton f)
                {
                    f.ApplyColorScheme(cs);
                    f.ColorSchemeChanged?.Invoke(d, e);
                }
            }
        }

        /// <summary>
        /// Get or set the color scheme to apply to this button. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the color scheme to apply to this button.")]
        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        bool runApply = true;

        /// <summary>
        /// The dependency property object for <see cref="UseAccentColors"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty UseAccentColorsProperty = FlatButton.UseAccentColorsProperty.AddOwner(typeof(SplitButton),
            new PropertyMetadata(false, new PropertyChangedCallback(OnUseAccentColorsChanged)));

        /// <summary>
        /// Get or set if the button should use the accent colors of the color scheme, rather than the standard colors.
        /// <para/>
        /// This method will be removed in a future version. Please use <c>UseAccentTheme</c> instead.
        /// </summary>
        [Category("Appearance")]
#if NET5_0_OR_GREATER
        [Obsolete("This method will be removed in a future version. Please use UseAccentTheme instead.", DiagnosticId = "SSUI001")]
#else
        [Obsolete("This method will be removed in a future version. Please use UseAccentTheme instead.")]
#endif
        public bool UseAccentColors
        {
            get => (bool)GetValue(UseAccentColorsProperty);
            set => SetValue(UseAccentColorsProperty, value);
        }

        /// <summary>
        /// Perform an action when a property of an object has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnUseAccentColorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool ua)
            {
                if (d is SplitButton f)
                {
                    f.ApplyColorScheme(f.ColorScheme, ua);
                }
            }
        }

        /// <summary>
        /// Apply a color scheme to this control, and set some other optional appearance settings. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply</param>
        public void ApplyColorScheme(ColorScheme cs)
        {
#pragma warning disable SSUI001 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
            ApplyColorScheme(cs, UseAccentColors || UseAccentTheme);
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore SSUI001 // Type or member is obsolete
        }

        /// <summary>
        /// Apply a color scheme to this control, and set some other optional appearance settings. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply</param>
        /// <param name="useAccentColors">Set if accent colors should be used for this button, rather than the main color scheme colors.
        /// This can also be achieved with the <c>UseAccentTheme</c> property.
        /// </param>
        public void ApplyColorScheme(ColorScheme cs, bool useAccentColors = false)
        {
            if (runApply == false)
            {
                return;
            }

            if (cs == null)
            {
                return;
            }

            if (cs != ColorScheme)
            {
                runApply = false;
#pragma warning disable SSUI001 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
                UseAccentColors = UseAccentColors || useAccentColors;
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore SSUI001 // Type or member is obsolete
                runApply = true;
                ColorScheme = cs;
                return;
            }

            if (useAccentColors != UseAccentTheme)
            {
                runApply = false;
                UseAccentTheme = useAccentColors;
                runApply = true;
            }

            runApply = false;

            if (cs.IsHighContrast)
            {
                Background = cs.BackgroundColor.ToBrush();
                HighlightBrush = cs.HighlightColor.ToBrush();
                SelectedBrush = cs.HighlightColor.ToBrush();
                BorderHighlightBrush = cs.BorderColor.ToBrush();
                BorderSelectedBrush = cs.BorderColor.ToBrush();
                BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                SeparatorBrush = cs.BorderColor.ToBrush();
                DisabledBrush = cs.BackgroundColor.ToBrush();
                Foreground = cs.ForegroundColor.ToBrush();
                ClickBrush = cs.ThirdHighlightColor.ToBrush();

                //if (transparentBack || TransparentBack)
                //{
                //    BorderBrush = Color.FromArgb(1, 0, 0, 0).ToBrush();
                //    TransparentBack = true;
                //}
                //else
                //{
                //    BorderBrush = cs.BorderColor.ToBrush();
                //}
            }
            else
            {
#pragma warning disable SSUI001 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
                if (UseAccentColors || useAccentColors || UseAccentTheme)
                {
                    Background = cs.AccentSecondaryColor.ToBrush();
                    BorderBrush = cs.AccentBorderColor.ToBrush();
                    HighlightBrush = cs.AccentSecondHighlightColor.ToBrush();
                    DisabledBrush = cs.LightDisabledColor.ToBrush();
                    BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                    SelectedBrush = cs.AccentThirdHighlightColor.ToBrush();
                    BorderHighlightBrush = cs.AccentHighlightColor.ToBrush();
                    BorderSelectedBrush = cs.AccentSelectionColor.ToBrush();
                    SeparatorBrush = cs.AccentBorderColor.ToBrush();
                    Foreground = cs.ForegroundColor.ToBrush();
                    ClickBrush = cs.AccentThirdHighlightColor.ToBrush();

                    UseAccentColors = true;
                }
                else
                {
                    Background = cs.SecondaryColor.ToBrush();
                    BorderBrush = cs.BorderColor.ToBrush();
                    HighlightBrush = cs.SecondHighlightColor.ToBrush();
                    DisabledBrush = cs.LightDisabledColor.ToBrush();
                    BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                    SelectedBrush = cs.ThirdHighlightColor.ToBrush();
                    BorderHighlightBrush = cs.HighlightColor.ToBrush();
                    BorderSelectedBrush = cs.SelectionColor.ToBrush();
                    SeparatorBrush = cs.BorderColor.ToBrush();
                    Foreground = cs.ForegroundColor.ToBrush();
                    ClickBrush = cs.ThirdHighlightColor.ToBrush();
                }
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore SSUI001 // Type or member is obsolete
            }

            runApply = true;
        }

#endregion

        #region Border

        /// <summary>The backing dependency property for <see cref="BorderSelectionThickness"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderSelectionThicknessProperty = FlatButton.BorderSelectionThicknessProperty.AddOwner(typeof(SplitButton),
            new PropertyMetadata((d, e) => d.PerformAs<SplitButton, Thickness>(e.NewValue, 
                (o, nv) => o.ApplyBorderSelectionThicknessToButtons(nv))));

        /// <summary>
        /// Get or set the thickness of the border around the button, while the button is in a selected (<c>IsSelected</c>) state.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the thickness of the border around the button, while the button is in a selected (IsSelected) state.")]
        public Thickness BorderSelectionThickness
        {
            get => (Thickness)GetValue(BorderSelectionThicknessProperty);
            set => SetValue(BorderSelectionThicknessProperty, value);
        }

        /// <summary>The backing dependency property for <see cref="CornerRadius"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CornerRadiusProperty = FlatButton.CornerRadiusProperty.AddOwner(typeof(SplitButton),
            new PropertyMetadata((o, e) => o.PerformAs<SplitButton, CornerRadius>(e.NewValue, (s, v) => s.ApplyCornerRadiusToButtons(v))));

        /// <summary>
        /// Get or set the corner radius (or radii) to use for the button and its border. Can be used to create a rounded button.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the corner radius (or radii) to use for the button and its border. Can be used to create a rounded button.")]
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        #region Separator Border

        /// <summary>
        /// Get or set if a separator bar should be shown between the main and menu buttons. Without the separator bar, the buttons look more connected, 
        /// but they can be hard to discern as two separate clickable buttons without mousing over them.
        /// </summary>
        public bool ShowSeparator { get => (bool)GetValue(ShowSeparatorProperty); set => SetValue(ShowSeparatorProperty, value); }

        /// <summary>The backing dependency property for <see cref="ShowSeparator"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ShowSeparatorProperty
            = DependencyProperty.Register("ShowSeparator", typeof(bool), typeof(SplitButton), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Get or set the brush to use for the separator bar between the main and menu buttons.
        /// </summary>
        public Brush SeparatorBrush { get => (Brush)GetValue(SeparatorBrushProperty); set => SetValue(SeparatorBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="SeparatorBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SeparatorBrushProperty
            = DependencyProperty.Register("SeparatorBrush", typeof(Brush), typeof(SplitButton), new FrameworkPropertyMetadata(Colors.Gray.ToBrush()));

        /// <summary>
        /// Get or set how distant the separator bar should be from the edges of the button. The higher the number, the bigger the space between
        /// the edges of the button and the ends of the separator bar. A value of 0 will bring the separator bar right up to the edges.
        /// </summary>
        public double SeparatorEdgeMargin { get => (double)GetValue(SeparatorEdgeMarginProperty); set => SetValue(SeparatorEdgeMarginProperty, value); }

        /// <summary>The backing dependency property for <see cref="SeparatorEdgeMargin"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SeparatorEdgeMarginProperty
            = DependencyProperty.Register("SeparatorEdgeMargin", typeof(double), typeof(SplitButton), new FrameworkPropertyMetadata(4.0));

        /// <summary>
        /// Get or set how wide the separator bar should be. A value of 0 will make it invisible.
        /// </summary>
        public double SeparatorThickness { get => (double)GetValue(SeparatorThicknessProperty); set => SetValue(SeparatorThicknessProperty, value); }

        /// <summary>The backing dependency property for <see cref="SeparatorThickness"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SeparatorThicknessProperty
            = DependencyProperty.Register("SeparatorThickness", typeof(double), typeof(SplitButton), new FrameworkPropertyMetadata(1.0));

        #endregion

        #endregion

        #region Menu

        #region Primary Properties / Events

        /// <summary>The backing dependency property for <see cref="Menu"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuProperty = MenuButton.MenuProperty.AddOwner(typeof(SplitButton),
            new FrameworkPropertyMetadata(null, (d, e) => d.PerformAs<SplitButton>((o) => o.OnMenuChanged(e))));

        /// <summary>
        /// Raised when the <see cref="Menu"/> property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? MenuChanged;
#else
        public event DependencyPropertyChangedEventHandler MenuChanged;
#endif

        private void OnMenuChanged(DependencyPropertyChangedEventArgs e)
        {
            MenuChanged?.Invoke(this, e);
        }

#if NETCOREAPP
        /// <summary>
        /// Get or set the menu that appears when the menu button is clicked.
        /// </summary>
        [Category("Common")]
        [Description("Get or set the menu that appears when the menu button is clicked.")]
        public ContextMenu? Menu
        {
            get { return (ContextMenu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// This event is raised when this SplitButton's menu is about to open.
        /// </summary>
        public event CancelEventHandler? MenuOpening;

        /// <summary>
        /// This event is raised when this SplitButton's menu has been opened.
        /// </summary>
        public event EventHandler? MenuOpened;

        /// <summary>
        /// This event is raised when this SplitButton's menu has been closed.
        /// </summary>
        public event EventHandler? MenuClosed;
#else
        /// <summary>
        /// Get or set the menu that appears when the menu button is clicked.
        /// </summary>
        [Category("Common")]
        [Description("Get or set the menu that appears when the menu button is clicked.")]
        public ContextMenu Menu
        {
            get { return (ContextMenu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// This event is raised when this SplitButton's menu is about to open.
        /// </summary>
        public event CancelEventHandler MenuOpening;

        /// <summary>
        /// This event is raised when this SplitButton's menu has been opened.
        /// </summary>
        public event EventHandler MenuOpened;

        /// <summary>
        /// This event is raised when this SplitButton's menu has been closed.
        /// </summary>
        public event EventHandler MenuClosed;
#endif

        /// <summary>
        /// Get or set if the menu should close automatically. Remember to set the <c>StaysOpenOnClick</c> property for applicable child menu items as well.
        /// </summary>
        /// <remarks>
        /// See the remarks for <see cref="MenuButton.StaysOpen"/> for more details on the handling of this property.
        /// </remarks>
        [Category("Common")]
        [Description("Get or set if the menu should close automatically. Remember to set the StaysOpenOnClick property for applicable child menu items as well.")]
        public bool StaysOpen
        {
            get
            {
                if (Menu != null) return Menu.StaysOpen;
                else return false;
            }
            set
            {
                if (Menu != null) Menu.StaysOpen = value;
            }
        }

        private void Menu_Closed(object sender, RoutedEventArgs e)
        {
            MenuClosed?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Placement Properties

        /// <summary>
        /// Get or set the placement mode for the SplitButton's menu. Default is <c>Bottom</c>.
        /// </summary>
        [Category("Common")]
        [Description("Get or set the placement mode for the SplitButton's menu. Default is Bottom.")]
        public PlacementMode MenuPlacement { get => (PlacementMode)GetValue(MenuPlacementProperty); set => SetValue(MenuPlacementProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuPlacement"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuPlacementProperty = MenuButton.MenuPlacementProperty.AddOwner(typeof(SplitButton),
            new FrameworkPropertyMetadata(PlacementMode.Bottom));


        /// <summary>
        /// Get or set the placement target for the SplitButton's menu. Set to <c>null</c> to set the target to this SplitButton. Default is <c>null</c>.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the placement target for the SplitButton's menu. Set to null to set the target to this SplitButton. Default is null.")]
#if NETCOREAPP
        public UIElement? MenuPlacementTarget { get => (UIElement)GetValue(MenuPlacementTargetProperty); set => SetValue(MenuPlacementTargetProperty, value); }
#else
        public UIElement MenuPlacementTarget { get => (UIElement)GetValue(MenuPlacementTargetProperty); set => SetValue(MenuPlacementTargetProperty, value); }
#endif

        /// <summary>The backing dependency property for <see cref="MenuPlacementTarget"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuPlacementTargetProperty = MenuButton.MenuPlacementTargetProperty.AddOwner(typeof(SplitButton));


        /// <summary>
        /// Get or set the placement rectangle for the SplitButton's menu. This sets the area relative to the button that the menu is positioned.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the placement rectangle for the SplitButton's menu. This sets the area relative to the button that the menu is positioned.")]
        public Rect MenuPlacementRectangle { get => (Rect)GetValue(MenuPlacementRectangleProperty); set => SetValue(MenuPlacementRectangleProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuPlacementRectangle"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuPlacementRectangleProperty = MenuButton.MenuPlacementRectangleProperty.AddOwner(typeof(SplitButton));


        /// <summary>
        /// Get or set how far offset the menu is horizontally (left or right) from its placement target/rectangle when it's opened.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set how far offset the menu is horizontally from its placement target/rectangle when it's opened.")]
        public double MenuHorizontalOffset { get => (double)GetValue(MenuHorizontalOffsetProperty); set => SetValue(MenuHorizontalOffsetProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuHorizontalOffset"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuHorizontalOffsetProperty = MenuButton.MenuHorizontalOffsetProperty.AddOwner(typeof(SplitButton));

        /// <summary>
        /// Get or set how far offset the menu is vertically (up or down) from its placement target/rectangle when it's opened.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set how far offset the menu is vertically from its placement target/rectangle when it's opened.")]
        public double MenuVerticalOffset { get => (double)GetValue(MenuVerticalOffsetProperty); set => SetValue(MenuVerticalOffsetProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuVerticalOffset"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuVerticalOffsetProperty = MenuButton.MenuVerticalOffsetProperty.AddOwner(typeof(SplitButton));

        #endregion

        /// <summary>
        /// Display this split button's menu programmatically. This will open the menu at the set placement target and location.
        /// </summary>
        public void OpenMenu()
        {
            if (Menu != null)
            {
                // first, raise MenuOpening event
                CancelEventArgs ce = new CancelEventArgs(false);
                MenuOpening?.Invoke(this, ce);
                if (ce.Cancel) return;

                // then, set up the full menu and show it
                Menu.Placement = MenuPlacement;
                Menu.PlacementTarget = MenuPlacementTarget ?? this;
                Menu.PlacementRectangle = MenuPlacementRectangle;
                Menu.HorizontalOffset = MenuHorizontalOffset;
                Menu.VerticalOffset = MenuVerticalOffset;
                Menu.IsOpen = true;
                Menu.Closed += Menu_Closed;

                // finally, we can raise the MenuOpened event
                MenuOpened?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region HighlightOnKeyboardFocus

        /// <summary>
        /// Get or set if the button should be highlighted (using the <see cref="HighlightBrush"/> and <see cref="BorderHighlightBrush"/>)
        /// when it has keyboard focus. If <c>false</c>, only the keyboard focus outline appears, and highlighting only occurs on mouse/stylus over.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set if the button should be highlighted when it has keyboard focus.")]
        public bool HighlightOnKeyboardFocus { get => (bool)GetValue(HighlightOnKeyboardFocusProperty); set => SetValue(HighlightOnKeyboardFocusProperty, value); }

        /// <summary>The backing dependency property for <see cref="HighlightOnKeyboardFocus"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HighlightOnKeyboardFocusProperty = FlatButton.HighlightOnKeyboardFocusProperty.AddOwner(typeof(SplitButton),
            new PropertyMetadata(ApplyPropertyUpdate));

        #endregion

        #region Button Handling

        private void btnMenu_RightClick(object sender, RoutedEventArgs e)
        {
            PerformRightClick();
        }

        private void btnMain_RightClick(object sender, RoutedEventArgs e)
        {
            PerformRightClick();
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            OpenMenu();

            RoutedEventArgs rre = new RoutedEventArgs(MenuClickEvent);
            RaiseEvent(rre);
        }

        private void btnMain_Click(object sender, RoutedEventArgs e)
        {
            OnClick();
        }

        #region Menu Button Layout

        /// <summary>
        /// Get or set where the menu button should be placed in relation to the main button. Default is <c>Right</c>.
        /// </summary>
        /// <remarks>
        /// If you want to set this value to <c>Top</c> or <c>Bottom</c>, also consider increasing the height of the button or decreasing the value of <see cref="MenuButtonSize"/>.
        /// If you set this value to <c>Hidden</c>, the menu button is entirely hidden (and no longer accessible via the mouse).
        /// </remarks>
        public PlacementDirection MenuButtonPlacement { get => (PlacementDirection)GetValue(MenuButtonPlacementProperty); set => SetValue(MenuButtonPlacementProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuButtonPlacement"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuButtonPlacementProperty
            = DependencyProperty.Register("MenuButtonPlacement", typeof(PlacementDirection), typeof(SplitButton),
            new FrameworkPropertyMetadata(PlacementDirection.Right));

        /// <summary>
        /// Get or set the width or height of the menu button. Larger values make the button wider or taller, and takes up a larger portion of the overall control. Default is <c>20</c>.
        /// </summary>
        /// <remarks>
        /// When <see cref="MenuButtonPlacement"/> is set to <c>Left</c> or <c>Right</c>, this affects the width of the menu button.
        /// When it is set to <c>Top</c> or <c>Bottom</c>, this affects the height of the menu button. When it is set to <c>Hidden</c>, this has no effect.
        /// </remarks>
        public double MenuButtonSize { get => (double)GetValue(MenuButtonSizeProperty); set => SetValue(MenuButtonSizeProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuButtonSize"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuButtonSizeProperty
            = DependencyProperty.Register("MenuButtonSize", typeof(double), typeof(SplitButton),
            new FrameworkPropertyMetadata(20.0));

        #endregion

        #endregion

        #region Click / Selection Handling

        #region Routed Events

        /// <summary>
        /// The backing value for the <see cref="Click"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SplitButton));

        /// <summary>
        /// Raised when the user clicks on the main button (not the menu button), via a mouse click or via the keyboard.
        /// </summary>
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        /// <summary>
        /// The backing value for the <see cref="MenuClick"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent MenuClickEvent = EventManager.RegisterRoutedEvent(
            "MenuClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SplitButton));

        /// <summary>
        /// Raised when the user clicks on the menu button (not the main button), via a mouse click or via the keyboard.
        /// </summary>
        public event RoutedEventHandler MenuClick
        {
            add { AddHandler(MenuClickEvent, value); }
            remove { RemoveHandler(MenuClickEvent, value); }
        }

        /// <summary>
        /// The backing value for the <see cref="RightClick"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent RightClickEvent = EventManager.RegisterRoutedEvent(
            "RightClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SplitButton));

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

        /// <summary>
        /// The DependencyPropertyKey for the attached property <c>IsMouseDown</c>. This indicates to readers if the mouse is currently
        /// pressed while over this control. Use <see cref="SetIsMouseDown(DependencyObject, bool)"/> to update this property.
        /// </summary>
        protected static readonly DependencyPropertyKey IsMouseDownPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsMouseDown",
            typeof(bool), typeof(SplitButton), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get if there is a mouse button currently being pressed, while the mouse cursor is over this control.
        /// </summary>
        public static readonly DependencyProperty IsMouseDownProperty = IsMouseDownPropertyKey.DependencyProperty;

        /// <summary>
        /// Set the IsMouseDown property for a SplitButton.
        /// </summary>
        /// <param name="obj">The SplitButton to apply the property change to.</param>
        /// <param name="value">The new value to set for the property.</param>
        protected static void SetIsMouseDown(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMouseDownPropertyKey, value);
        }

        /// <summary>
        /// Get the IsMouseDown property for a SplitButton.
        /// </summary>
        /// <param name="obj">The SplitButton to get the property value from.</param>
        public static bool GetIsMouseDown(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMouseDownProperty);
        }

        /// <summary>
        /// Get if a mouse button is currently being pressed while the cursor is over this SplitButton.
        /// </summary>
        [ReadOnly(true)]
        public bool IsMouseDown
        {
            get => (bool)GetValue(IsMouseDownProperty);
            protected set => SetValue(IsMouseDownPropertyKey, value);
        }

        #endregion

        #region Variables/Properties

        #region IsSelected / IsSelectedChanged

        bool _runSelChangeEvent = true;

        /// <summary>
        /// The backing dependency property for <see cref="IsSelected"/>. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = FlatButton.IsSelectedProperty.AddOwner(typeof(SplitButton),
            new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool se)
            {
                bool old = (e.OldValue is bool oval) ? oval : false;

                if (d is SplitButton f)
                {
                    if (f._runSelChangeEvent)
                    {
                        ItemSelectionChangedEventArgs re = new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, old, se, IsSelectedProperty, SelectionChangeTrigger.CodeUnknown, null);
                        f.RaiseEvent(re);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets whether this button is selected. This property (combined with <c>SelectOnClick</c>) allows the button to function like a ToggleButton.
        /// </summary>
        /// <remarks>
        /// A selected button will have a slightly different visual appearance to differentiate it as being selected. This will include, by default, the border being a bit thicker.
        /// This can be changed via the <see cref="BorderSelectionThickness"/> property. You can also directly edit the brushes used via the <see cref="SelectedBrush"/> and
        /// <see cref="BorderSelectedBrush"/> properties.
        /// </remarks>
        [Category("Common")]
        [Description("Gets or sets whether this button is selected.")]
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

        /// <summary>
        /// The backing value for the <see cref="IsSelectedChanged"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent IsSelectedChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(IsSelectedChanged), RoutingStrategy.Bubble, typeof(ItemSelectionChangedEventHandler), typeof(SplitButton));

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

            ItemSelectionChangedEventArgs re = new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, old, value, IsSelectedProperty, trigger, triggerSource);
            RaiseEvent(re);
        }

        #endregion

        /// <summary>
        /// The backing dependency property for <see cref="SelectOnClick"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty SelectOnClickProperty = FlatButton.SelectOnClickProperty.AddOwner(typeof(SplitButton));

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
        [Description("Gets or sets whether the button should change its IsSelected property when a click is performed. With this enabled, this allows the button to take on the functionality of a ToggleButton.")]
        public bool SelectOnClick
        {
            get => (bool)GetValue(SelectOnClickProperty);
            set => SetValue(SelectOnClickProperty, value);
        }

        #endregion

        // If the button is prepared by PerformPress, perform the Click actions, including raising the Click event.
        void PerformRightClick()
        {
            RoutedEventArgs rre = new RoutedEventArgs(RightClickEvent);
            RaiseEvent(rre);
        }

        /// <summary>
        /// Perform a click on the main button programmatically. The button responds the same way as if it was clicked by the user.
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
                SetIsSelectedWithSource(!IsSelected, SelectionChangeTrigger.ControlClick, this);
            }

            RoutedEventArgs rre = new RoutedEventArgs(ClickEvent);
            RaiseEvent(rre);
        }

        #endregion

    }
}
