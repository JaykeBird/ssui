using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using TabItem = SolidShineUi.TabItem;

namespace SolidShineUi.Utils
{

    /// <summary>
    /// A visual rendering of a <c>TabItem</c> (see <see cref="TabItem"/>), to display in a <see cref="TabControl"/>.
    /// </summary>
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class TabDisplayItem : TemplatedControl
    {

        /// <summary>
        /// Create a TabDisplayItem.
        /// </summary>
        public TabDisplayItem()
        {

        }

        /// <summary>
        /// Create a TabDisplayItem, representing a specified TabItem.
        /// </summary>
        /// <param name="ti">The TabItem represented by this TabDisplayItem.</param>
        public TabDisplayItem(TabItem ti) : this()
        {
            TabItem = ti;
        }

        private string GetDebuggerDisplay()
        {
            return "TabDisplayItem: {\"" + TabItem.Title + "\"}";
        }

        #region Template IO

        /// <inheritdoc/>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            // get items
            LoadTemplateItems(e);
        }

        FlatButton? btnClose = null;

        bool itemsLoaded = false;

        void LoadTemplateItems(TemplateAppliedEventArgs e)
        {
            if (!itemsLoaded)
            {
                btnClose = e.NameScope.Find<FlatButton>("PART_btnClose");

                if (btnClose != null)
                {
                    btnClose.Click += BtnClose_Click;
                    itemsLoaded = true;
                }
            }
        }

        private void BtnClose_Click(object? sender, RoutedEventArgs e)
        {
            RequestClose?.Invoke(this, e);
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            switch (change.Property.Name)
            {
                case nameof(TabItem):
                    OnTabItemChange(change);
                    break;
                case nameof(ParentTabControl):
                    OnParentTabControlChanged(change);
                    break;
                case nameof(CanSelect):
                    OnCanSelectChanged(change);
                    break;
            }
        }

        #region TabItem

        /// <summary>
        /// The TabItem that this TabDisplayItem is representing. It is not advisable to change this property after the control is loaded; instead, just create a new TabDisplayItem.
        /// </summary>
        public TabItem TabItem { get => GetValue(TabItemProperty); set => SetValue(TabItemProperty, value); }

        /// <summary>The backing styled property for <see cref="TabItem"/>. See the related property for details.</summary>
        public static readonly StyledProperty<TabItem> TabItemProperty
            = AvaloniaProperty.Register<TabDisplayItem, TabItem>(nameof(TabItem), new TabItem());

        void OnTabItemChange(AvaloniaPropertyChangedEventArgs e)
        {
            Bind(CanSelectProperty, new Binding(nameof(TabItem.CanSelect)) { Source = e.NewValue, TargetNullValue = false });
        }

        #endregion

        #region ParentTabControl

        /// <summary>
        /// Get or set the parent TabControl item that holds this tab item.
        /// </summary>
        public TabControl? ParentTabControl { get => GetValue(ParentTabControlProperty); set => SetValue(ParentTabControlProperty, value); }

        /// <summary>The backing styled property for <see cref="ParentTabControl"/>. See the related property for details.</summary>
        public static readonly StyledProperty<TabControl?> ParentTabControlProperty
            = AvaloniaProperty.Register<TabDisplayItem, TabControl?>(nameof(ParentTabControl), null);

        void OnParentTabControlChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is TabControl tc)
            {
                tc.SetupTabDisplay(this);
            }
        }

        #endregion

        /// <summary>
        /// Get or set if the tab is rendered at the bottom of the <see cref="TabControl"/>. This affects the visuals of the control.
        /// </summary>
        public bool ShowTabOnBottom { get => GetValue(ShowTabOnBottomProperty); set => SetValue(ShowTabOnBottomProperty, value); }

        /// <summary>The backing styled property for <see cref="ShowTabOnBottom"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> ShowTabOnBottomProperty
            = AvaloniaProperty.Register<TabDisplayItem, bool>(nameof(ShowTabOnBottom), false);

        #region DirtyState

        /// <summary>
        /// Get or set if this tab is dirty. This can be used to visually indicate, for example, unsaved changes in the tab's contents. 
        /// </summary>
        public bool IsDirty { get => GetValue(IsDirtyProperty); set => SetValue(IsDirtyProperty, value); }

        /// <summary>The backing styled property for <see cref="IsDirty"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> IsDirtyProperty
            = AvaloniaProperty.Register<TabDisplayItem, bool>(nameof(IsDirty), false);

        //public bool DisplayDirtyState { get => GetValue(DisplayDirtyStateProperty); set => SetValue(DisplayDirtyStateProperty, value); }

        ///// <summary>The backing styled property for <see cref="DisplayDirtyState"/>. See the related property for details.</summary>
        //public static readonly StyledProperty<bool> DisplayDirtyStateProperty
        //    = AvaloniaProperty.Register<TabDisplayItem, bool>(nameof(DisplayDirtyState), false);


        #endregion

        #region Brushes

        /// <summary>
        /// Get or set the brush for the border of this control.
        /// </summary>
        public IBrush TabBorderBrush { get => GetValue(TabBorderBrushProperty); set => SetValue(TabBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="TabBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> TabBorderBrushProperty
            = AvaloniaProperty.Register<TabDisplayItem, IBrush>(nameof(TabBorderBrush), Colors.Black.ToBrush());

        /// <summary>
        /// Get or set the brush used for the close glyph in this control.
        /// </summary>
        public IBrush CloseBrush { get => GetValue(CloseBrushProperty); set => SetValue(CloseBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="CloseBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> CloseBrushProperty
            = AvaloniaProperty.Register<TabDisplayItem, IBrush>(nameof(CloseBrush), Colors.Black.ToBrush());

        /// <summary>
        /// Get or set the background for the tab while it is selected (<see cref="IsSelected"/> is <c>true</c>).
        /// </summary>
        public IBrush? SelectedTabBackground { get => GetValue(SelectedTabBackgroundProperty); set => SetValue(SelectedTabBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="SelectedTabBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> SelectedTabBackgroundProperty
            = AvaloniaProperty.Register<TabDisplayItem, IBrush?>(nameof(SelectedTabBackground), Colors.White.ToBrush());

        /// <summary>
        /// Get or set the brush for the border while this TabDisplayItem is highlighted (i.e. the mouse is over it, or it had keyboard focus).
        /// </summary>
        public IBrush? BorderHighlightBrush { get => GetValue(BorderHighlightBrushProperty); set => SetValue(BorderHighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BorderHighlightBrushProperty
            = AvaloniaProperty.Register<TabDisplayItem, IBrush?>(nameof(BorderHighlightBrush), Colors.Black.ToBrush());

        /// <summary>
        /// Get or set the brush for the background while this TabDisplayItem is highlighted (i.e. the mouse is over it, or it has keyboard focus).
        /// </summary>
        public IBrush? HighlightBrush { get => GetValue(HighlightBrushProperty); set => SetValue(HighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightBrushProperty
            = AvaloniaProperty.Register<TabDisplayItem, IBrush?>(nameof(HighlightBrush), Colors.White.ToBrush());

        /// <summary>
        /// Get or set the brush used for the close button, when it is being clicked (i.e. mouse down, key down).
        /// </summary>
        public IBrush? ButtonClickBrush { get => GetValue(ButtonClickBrushProperty); set => SetValue(ButtonClickBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="ButtonClickBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> ButtonClickBrushProperty
            = AvaloniaProperty.Register<TabDisplayItem, IBrush?>(nameof(ButtonClickBrush), Colors.LightGray.ToBrush());

        /// <summary>
        /// Get or set the brush used for the close button, when it is highlighted (i.e. mouse over).
        /// </summary>
        public IBrush? ButtonHighlightBrush { get => GetValue(ButtonHighlightBrushProperty); set => SetValue(ButtonHighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="ButtonHighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> ButtonHighlightBrushProperty
            = AvaloniaProperty.Register<TabDisplayItem, IBrush?>(nameof(ButtonHighlightBrush), Colors.Gainsboro.ToBrush());

        /// <summary>
        /// Get or set the brush used for the close button, when it is highlighted (i.e. mouse over).
        /// </summary>
        public IBrush? ButtonBorderHighlightBrush { get => GetValue(ButtonBorderHighlightBrushProperty); set => SetValue(ButtonBorderHighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="ButtonBorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> ButtonBorderHighlightBrushProperty
            = AvaloniaProperty.Register<TabDisplayItem, IBrush?>(nameof(ButtonBorderHighlightBrush), Colors.DimGray.ToBrush());


        #endregion

        #endregion

        /// <summary>
        /// Raised when the Close button is clicked, and this tab wants to be closed.
        /// </summary>
        public event EventHandler? RequestClose;
        ///// <summary>
        ///// Raised when a TabItem is dropped onto this TabDisplayItem. Used as part of the TabControl's drag-and-drop system.
        ///// </summary>
        //public event TabItemDropEventHandler? TabItemDrop;

        // just straight up copied all of the click and selection handling from FlatButton, not sure if I actually need all of this

        // in the future, I'll have to look into listening to Avalonia's Tapped and RightTapped events, rather than directly parsing
        // the pointer events myself. For now, though, I'm just focused on replicating the functionality in the WPF version

        #region Click / Selection Handling

        #region Base Variables

        private bool _isPressed = false;
        private bool _isRightPressed = false;
        private bool _isPressedByKey = false;

        #endregion

        #region Selection Properties

        // TODO: look into removing SelectOnClick from this; I believe selecting is handled fully by the TabControl
        // but there may be some use in leaving this here in case someone, for some reason, uses this outside of a TabControl

        /// <summary>
        /// Get or set if this control should change its <see cref="IsSelected"/> value when you click on the control.
        /// </summary>
        /// <remarks>
        /// This allows more fine-tuned control over when and how this control can be selected. If this is <c>false</c>, then the user can only use the checkbox to directly 
        /// select or deselect this control. You can use <see cref="CanSelect"/> to globally disable selecting this control via any method.
        /// </remarks>
        public bool SelectOnClick { get => GetValue(SelectOnClickProperty); set => SetValue(SelectOnClickProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectOnClick"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> SelectOnClickProperty
            = AvaloniaProperty.Register<TabDisplayItem, bool>("SelectOnClick", false);

        /// <summary>
        /// Get or set if this control can be selected.
        /// </summary>
        /// <remarks>
        /// If this is set to <c>false</c>, then this control cannot be selected via any method - even programmatically. Setting this to <c>false</c> will also deselect this control, 
        /// if currently selected. For more fine-tuned control, you can use <see cref="SelectOnClick"/> to limit how the user can select this control, 
        /// while still being able to change the selection status via <see cref="IsSelected"/>.
        /// </remarks>
        public bool CanSelect { get => GetValue(CanSelectProperty); set => SetValue(CanSelectProperty, value); }

        /// <summary>The backing dependency property for <see cref="CanSelect"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> CanSelectProperty
            = AvaloniaProperty.Register<TabDisplayItem, bool>("CanSelect", true);

        private void OnCanSelectChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (((bool?)e.NewValue ?? true) == false)
            {
                if (IsSelected)
                {
                    sel = false;

                    ItemSelectionChangedEventArgs re = new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, true, false, SelectionChangeTrigger.DisableSelecting, e.Sender);
                    RaiseEvent(re);
                }
            }

            RoutedEventArgs cre = new RoutedEventArgs(CanSelectChangedEvent, this);
            RaiseEvent(cre);
        }

        bool sel = false;

        /// <summary>
        /// The backing direct property for <see cref="IsSelected"/>. See the related property for details.
        /// </summary>
        public static readonly DirectProperty<TabDisplayItem, bool> IsSelectedProperty
            = AvaloniaProperty.RegisterDirect<TabDisplayItem, bool>("IsSelected", (c) => c.IsSelected, (c, v) => c.IsSelected = v);

        /// <summary>
        /// Get or set if this control is currently selected.
        /// </summary>
        /// <remarks>
        /// If <see cref="CanSelect"/> is set to <c>false</c>, then this value will not be changed (silent fail).
        /// Use <see cref="SelectOnClick"/> to offer control over whether this can be selected via the user interacting with this, without disabling 
        /// the ability to set this state programmatically.
        /// </remarks>
        public bool IsSelected
        {
            get
            {
                return sel;
            }
            set
            {
                SetIsSelectedWithSource(value, SelectionChangeTrigger.CodeUnknown);
            }
        }

        #region SetIsSelectedWithSource

        /// <summary>
        /// Set the <see cref="IsSelected"/> value of this control, while also defining how the selection was changed.
        /// </summary>
        /// <param name="value">The value to set <see cref="IsSelected"/> to.</param>
        /// <param name="triggerMethod">The source or method used to trigger the change in selection.</param>
        /// <param name="triggerSource">The object that triggered the change.</param>
        /// <remarks>If <see cref="CanSelect"/> is set to <c>false</c>, then nothing will occur (silent fail).</remarks>
        public void SetIsSelectedWithSource(bool value, SelectionChangeTrigger triggerMethod, object? triggerSource = null)
        {
            if (CanSelect)
            {
                bool curVal = sel;
                SetAndRaise(IsSelectedProperty, ref sel, value);
                //PseudoClasses.Set(pcSel, value);

                if (curVal != sel)
                {
                    ItemSelectionChangedEventArgs e = new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, curVal, sel, triggerMethod, triggerSource);
                    RaiseEvent(e);
                }
            }
        }
        #endregion

        #endregion

        #region Selection Events

        /// <summary>
        /// Defines the <see cref="IsSelectedChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent<ItemSelectionChangedEventArgs> IsSelectedChangedEvent =
            RoutedEvent.Register<TabDisplayItem, ItemSelectionChangedEventArgs>(nameof(IsSelectedChanged), RoutingStrategies.Bubble);

        /// <summary>
        /// Defines the <see cref="CanSelectChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> CanSelectChangedEvent =
            RoutedEvent.Register<TabDisplayItem, RoutedEventArgs>(nameof(CanSelectChanged), RoutingStrategies.Bubble);

        /// <summary>
        /// Raised when the user clicks the button with the right mouse.
        /// </summary>
        public event EventHandler<ItemSelectionChangedEventArgs> IsSelectedChanged
        {
            add => AddHandler(IsSelectedChangedEvent, value);
            remove => RemoveHandler(IsSelectedChangedEvent, value);
        }

        /// <summary>
        /// Raised when the <see cref="CanSelect"/> property has changed.
        /// </summary>
        public event EventHandler<RoutedEventArgs> CanSelectChanged
        {
            add => AddHandler(CanSelectChangedEvent, value);
            remove => RemoveHandler(CanSelectChangedEvent, value);
        }

        #endregion

        #region Click Events

        /// <summary>
        /// Defines the <see cref="Click"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
            RoutedEvent.Register<TabDisplayItem, RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble);

        /// <summary>
        /// Defines the <see cref="RightClick"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> RightClickEvent =
            RoutedEvent.Register<TabDisplayItem, RoutedEventArgs>(nameof(RightClick), RoutingStrategies.Bubble);

        /// <summary>
        /// Raised when the user clicks the button.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        /// <summary>
        /// Raised when the user clicks the button with the right mouse.
        /// </summary>
        public event EventHandler<RoutedEventArgs> RightClick
        {
            add => AddHandler(RightClickEvent, value);
            remove => RemoveHandler(RightClickEvent, value);
        }

        #endregion

        #region Click Properties

        /// <summary>
        /// The backing styled property for <see cref="ClickMode"/>. See the related property for details.
        /// </summary>
        public static readonly StyledProperty<ClickMode> ClickModeProperty
            = AvaloniaProperty.Register<TabDisplayItem, ClickMode>(nameof(ClickMode), ClickMode.Release);

        /// <summary>
        /// The backing direct property for <see cref="IsPressed"/>. See the related property for details.
        /// </summary>
        public static readonly DirectProperty<TabDisplayItem, bool> IsPressedProperty
            = AvaloniaProperty.RegisterDirect<TabDisplayItem, bool>(nameof(IsPressed), (c) => c.IsPressed, unsetValue: false);

        /// <summary>
        /// The backing direct property for <see cref="IsRightPressed"/>. See the related property for details.
        /// </summary>
        public static readonly DirectProperty<TabDisplayItem, bool> IsRightPressedProperty
            = AvaloniaProperty.RegisterDirect<TabDisplayItem, bool>(nameof(IsRightPressed), (c) => c.IsRightPressed, unsetValue: false);

        /// <summary>
        /// The backing direct property for <see cref="IsPressedByKey"/>. See the related proeprty for details.
        /// </summary>
        public static readonly DirectProperty<TabDisplayItem, bool> IsPressedByKeyProperty
            = AvaloniaProperty.RegisterDirect<TabDisplayItem, bool>(nameof(IsPressedByKey), (c) => c.IsPressedByKey, unsetValue: false);

        /// <summary>
        /// Gets or sets a value indicating how this button should react to clicks.
        /// </summary>
        public ClickMode ClickMode
        {
            get => GetValue(ClickModeProperty);
            set => SetValue(ClickModeProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this button is currently being pressed down via the primary input.
        /// For being pressed by a key press, refer to <see cref="IsPressedByKey"/>.
        /// </summary>
        /// <remarks>
        /// Primary input includes the left mouse button, or a touch occurring with a pen on a tablet or a finger/stylus on a touchpad.
        /// </remarks>
        public bool IsPressed
        {
            get => _isPressed;
            private set => SetAndRaise(IsPressedProperty, ref _isPressed, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this button is currently being pressed down via the secondary input.
        /// </summary>
        /// <remarks>
        /// Secondary input primarily refers to the right mouse button, but other inputs could apply (such as holding a pen immediately above a tablet).
        /// </remarks>
        public bool IsRightPressed
        {
            get => _isRightPressed;
            private set => SetAndRaise(IsRightPressedProperty, ref _isRightPressed, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this button is currently being pressed down via the Enter or Space keys being held down.
        /// For being pressed by other inputs (e.g., mouse, touch, pen), see <see cref="IsPressed"/>.
        /// </summary>
        public bool IsPressedByKey
        {
            get => _isPressedByKey;
            private set => SetAndRaise(IsPressedProperty, ref _isPressedByKey, value);
        }

        #endregion

        #region Base Click Functions

        /// <summary>
        /// Perform a click programmatically. This control responds the same way as if it was clicked by the user.
        /// </summary>
        public void DoClick()
        {
            OnClick();
        }

        /// <summary>
        /// Perform a click on this control, raising the <see cref="Click"/> event.
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

        /// <summary>
        /// Perform a right click on this control, raising the <see cref="RightClick"/> event.
        /// </summary>
        protected void OnRightClick()
        {
            RoutedEventArgs rre = new RoutedEventArgs(RightClickEvent);
            RaiseEvent(rre);
        }

        #endregion

        #region Event Handlers

        /// <inheritdoc/>
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            var pointerProperties = e.GetCurrentPoint(this).Properties;

            if (pointerProperties.IsRightButtonPressed)
            {
                RegisterRightPress();
                // e.Handled = true; // don't want to override base right-clicking functionality
            }
            else if (pointerProperties.IsLeftButtonPressed)
            {
                RegisterPress();
                e.Handled = true;
            }
        }

        void RegisterPress()
        {
            IsPressed = true;
            if (ClickMode == ClickMode.Press)
            {
                OnClick();
            }
        }

        void RegisterRightPress()
        {
            IsRightPressed = true;
            if (ClickMode == ClickMode.Press)
            {
                OnRightClick();
            }
        }

        /// <inheritdoc/>
        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            bool isInThis = this.GetVisualsAt(e.GetPosition(this)).Any(c => this == c || this.IsVisualAncestorOf(c));


            if (IsPressed && e.InitialPressMouseButton == MouseButton.Left)
            {
                IsPressed = false;

                if (ClickMode != ClickMode.Press && isInThis)
                {
                    e.Handled = true;
                    OnClick();
                }
            }
            else if (IsRightPressed && e.InitialPressMouseButton == MouseButton.Right)
            {
                IsRightPressed = false;

                if (ClickMode != ClickMode.Press && isInThis)
                {
                    //e.Handled = true;
                    OnRightClick();
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
        {
            base.OnPointerCaptureLost(e);

            IsPressed = false;
        }

        /// <inheritdoc/>
        protected override void OnLostFocus(FocusChangedEventArgs e)
        {
            base.OnLostFocus(e);

            IsPressed = false;
        }



        /// <inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                _isPressedByKey = true;

                if (ClickMode == ClickMode.Press)
                {
                    OnClick();
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (_isPressedByKey && (e.Key == Key.Enter || e.Key == Key.Space))
            {
                if (ClickMode != ClickMode.Press)
                {
                    OnClick();
                }
            }
            else if (e.Key == Key.Apps)
            {
                OnRightClick();
            }
        }

        #endregion

        #endregion

        #region Highlighting

        private bool _isHighlighting = false;

        /// <summary>
        /// Get if this TabDisplayItem is currently highlighted (i.e. has focus or mouse over).
        /// </summary>
        public bool IsHighlighting { get => _isHighlighting; private set => SetAndRaise(IsHighlightingProperty, ref _isHighlighting, value); }

        /// <summary>The backing direct property for <see cref="IsHighlighting"/>. See the related property for details.</summary>
        public static readonly DirectProperty<TabDisplayItem, bool> IsHighlightingProperty
            = AvaloniaProperty.RegisterDirect<TabDisplayItem, bool>(nameof(IsHighlighting), (s) => s.IsHighlighting, unsetValue: false);

        /// <inheritdoc/>
        protected override void OnGotFocus(FocusChangedEventArgs e)
        {
            base.OnGotFocus(e);

            if (IsEnabled && CanSelect)
            {
                IsHighlighting = true;
            }
        }

        /// <inheritdoc/>
        protected override void OnPointerEntered(PointerEventArgs e)
        {
            base.OnPointerEntered(e);

            if (IsEnabled && CanSelect)
            {
                IsHighlighting = true;
            }
        }

        /// <inheritdoc/>
        protected override void OnLosingFocus(FocusChangingEventArgs e)
        {
            base.OnLosingFocus(e);
            IsHighlighting = false;
        }

        /// <inheritdoc/>
        protected override void OnPointerExited(PointerEventArgs e)
        {
            base.OnPointerExited(e);

            if (!IsFocused)
            {
                IsHighlighting = false;
            }
        }

        #endregion
    }
}
