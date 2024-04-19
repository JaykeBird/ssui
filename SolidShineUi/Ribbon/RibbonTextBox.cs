using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace SolidShineUi.Ribbon
{
    /// <summary>
    /// A <see cref="TextBox"/> to display in a <see cref="RibbonGroup"/>.
    /// </summary>
    [ContentProperty("Text")]
    public class RibbonTextBox : RibbonContentControl
    {
        private TextBox textBox;
        
        /// <summary>
        /// Create a RibbonTextBox.
        /// </summary>
        public RibbonTextBox()
        {
            textBox = new TextBox();
            Content = textBox;
            BaseTextBox = textBox;

            SetupBinding();
        }

        void SetupBinding()
        {
            // TextBox
            SetBinding(CharacterCasingProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(CharacterCasing)) });
            SetBinding(MaxLengthProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(MaxLength)) });
            SetBinding(MaxLinesProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(MaxLines)) });
            SetBinding(MinLinesProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(MinLines)) });
            SetBinding(TextAlignmentProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(TextAlignment)) });
            SetBinding(TextDecorationsProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(TextDecorations)) });
            SetBinding(TextProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(Text)) });
            SetBinding(TextWrappingProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(TextWrapping)) });
            // TextBoxBase
            SetBinding(AcceptsReturnProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(AcceptsReturn)) });
            SetBinding(AcceptsTabProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(AcceptsTab)) });
            SetBinding(AutoWordSelectionProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(AutoWordSelection)) });
            SetBinding(CaretBrushProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(CaretBrush)) });
            SetBinding(HorizontalScrollBarVisibilityProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(HorizontalScrollBarVisibility)) });
            SetBinding(IsInactiveSelectionHighlightEnabledProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(IsInactiveSelectionHighlightEnabled)) });
            SetBinding(IsReadOnlyCaretVisibleProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(IsReadOnlyCaretVisible)) });
            SetBinding(IsReadOnlyProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(IsReadOnly)) });
            SetBinding(IsUndoEnabledProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(IsUndoEnabled)) });
            SetBinding(SelectionBrushProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(SelectionBrush)) });
            SetBinding(SelectionOpacityProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(SelectionOpacity)) });
#if NETCOREAPP || NET48_OR_GREATER
            SetBinding(SelectionTextBrushProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(SelectionTextBrush)) });
#endif
            SetBinding(UndoLimitProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(UndoLimit)) });
            SetBinding(VerticalScrollBarVisibilityProperty,
                new Binding() { Source = textBox, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(VerticalScrollBarVisibility)) });
            // Added properties
            textBox.SetBinding(WidthProperty,
                new Binding() { Source = this, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(TextBoxWidth)) });
            textBox.SetBinding(HeightProperty,
                new Binding() { Source = this, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(TextBoxHeight)) });
            textBox.SetBinding(VerticalContentAlignmentProperty,
                new Binding() { Source = this, Mode = BindingMode.TwoWay, Path = new PropertyPath(nameof(VerticalTextAlignment)) });
        }

        #region Properties

        /// <summary>
        /// Get the base text box item in this control.
        /// </summary>
        /// <remarks>
        /// This is the actual <see cref="TextBox"/> that is displayed in this <see cref="RibbonTextBox"/>.
        /// All properties exposed in the RibbonTextBox will be 
        /// </remarks>
        public TextBox BaseTextBox { get => (TextBox)GetValue(BaseTextBoxProperty); private set => SetValue(BaseTextBoxPropertyKey, value); }

        private static readonly DependencyPropertyKey BaseTextBoxPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(BaseTextBox), typeof(TextBox), typeof(RibbonTextBox), new FrameworkPropertyMetadata());

        /// <summary>The backing dependency property for <see cref="BaseTextBox"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BaseTextBoxProperty = BaseTextBoxPropertyKey.DependencyProperty;

        #region TextBox Properties

        /// <summary>
        /// Gets or sets how characters are cased when they are manually entered into the text box.
        /// </summary>
        public CharacterCasing CharacterCasing { get => (CharacterCasing)GetValue(CharacterCasingProperty); set => SetValue(CharacterCasingProperty, value); }

        /// <summary>The backing dependency property for <see cref="CharacterCasing"/>. See the related property for details.</summary>
        public static DependencyProperty CharacterCasingProperty
            = TextBox.CharacterCasingProperty.AddOwner(typeof(RibbonTextBox));

        /// <summary>
        /// Gets or sets the maximum number of characters that can be manually entered into the text box.
        /// </summary>
        public int MaxLength { get => (int)GetValue(MaxLengthProperty); set => SetValue(MaxLengthProperty, value); }

        /// <summary>The backing dependency property for <see cref="MaxLength"/>. See the related property for details.</summary>
        public static DependencyProperty MaxLengthProperty
            = TextBox.MaxLengthProperty.AddOwner(typeof(RibbonTextBox));

        /// <summary>
        /// Gets or sets the maximum number of visible lines.
        /// </summary>
        public int MaxLines { get => (int)GetValue(MaxLinesProperty); set => SetValue(MaxLinesProperty, value); }

        /// <summary>The backing dependency property for <see cref="MaxLines"/>. See the related property for details.</summary>
        public static DependencyProperty MaxLinesProperty
            = TextBox.MaxLinesProperty.AddOwner(typeof(RibbonTextBox));

        /// <summary>
        /// Gets or sets the minimum number of visible lines.
        /// </summary>
        public int MinLines { get => (int)GetValue(MinLinesProperty); set => SetValue(MinLinesProperty, value); }

        /// <summary>The backing dependency property for <see cref="MinLines"/>. See the related property for details.</summary>
        public static DependencyProperty MinLinesProperty
            = TextBox.MinLinesProperty.AddOwner(typeof(RibbonTextBox));

        /// <summary>
        /// Gets or sets the horizontal alignment of the contents of the text box.
        /// </summary>
        public TextAlignment TextAlignment { get => (TextAlignment)GetValue(TextAlignmentProperty); set => SetValue(TextAlignmentProperty, value); }

        /// <summary>The backing dependency property for <see cref="TextAlignment"/>. See the related property for details.</summary>
        public static DependencyProperty TextAlignmentProperty
            = TextBox.TextAlignmentProperty.AddOwner(typeof(RibbonTextBox));

        /// <summary>
        /// Gets the text decorations to apply to the text box.
        /// </summary>
        public TextDecorationCollection TextDecorations { get => (TextDecorationCollection)GetValue(TextDecorationsProperty); set => SetValue(TextDecorationsProperty, value); }

        /// <summary>The backing dependency property for <see cref="TextDecorations"/>. See the related property for details.</summary>
        public static DependencyProperty TextDecorationsProperty
            = TextBox.TextDecorationsProperty.AddOwner(typeof(RibbonTextBox));

        /// <summary>
        /// Gets or sets the text contents of the text box.
        /// </summary>
        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        /// <summary>The backing dependency property for <see cref="Text"/>. See the related property for details.</summary>
        public static DependencyProperty TextProperty
            = TextBox.TextProperty.AddOwner(typeof(RibbonTextBox));

        /// <summary>
        /// Gets or sets how the text box should wrap text.
        /// </summary>
        public TextWrapping TextWrapping { get => (TextWrapping)GetValue(TextWrappingProperty); set => SetValue(TextWrappingProperty, value); }

        /// <summary>The backing dependency property for <see cref="TextWrapping"/>. See the related property for details.</summary>
        public static DependencyProperty TextWrappingProperty
            = TextBox.TextWrappingProperty.AddOwner(typeof(RibbonTextBox));

        #endregion

        #region TextBoxBase Properties

        /// <summary>
        /// Gets or sets a value that indicates how the text editing control responds when the user presses the ENTER key.
        /// </summary>
        public bool AcceptsReturn { get => (bool)GetValue(AcceptsReturnProperty); set => SetValue(AcceptsReturnProperty, value); }

        /// <summary>The backing dependency property for <see cref="AcceptsReturn"/>. See the related property for details.</summary>
        public static DependencyProperty AcceptsReturnProperty
            = TextBoxBase.AcceptsReturnProperty.AddOwner(typeof(RibbonTextBox));

        /// <summary>
        /// Gets or sets a value that indicates how the text editing control responds when the user presses the TAB key.
        /// </summary>
        public bool AcceptsTab { get => (bool)GetValue(AcceptsTabProperty); set => SetValue(AcceptsTabProperty, value); }

        /// <summary>The backing dependency property for <see cref="AcceptsTab"/>. See the related property for details.</summary>
        public static DependencyProperty AcceptsTabProperty
            = TextBoxBase.AcceptsTabProperty.AddOwner(typeof(RibbonTextBox));

        /// <summary>
        /// Gets or sets a value that determines whether when a user selects part of a word by 
        /// dragging across it with the mouse, the rest of the word is selected.
        /// </summary>
        public bool AutoWordSelection { get => (bool)GetValue(AutoWordSelectionProperty); set => SetValue(AutoWordSelectionProperty, value); }

        /// <summary>The backing dependency property for <see cref="AutoWordSelection"/>. See the related property for details.</summary>
        public static DependencyProperty AutoWordSelectionProperty
            = TextBoxBase.AutoWordSelectionProperty.AddOwner(typeof(RibbonTextBox));

        /// <summary>
        /// Gets or sets the brush that is used to paint the caret of the text box.
        /// </summary>
        public Brush CaretBrush { get => (Brush)GetValue(CaretBrushProperty); set => SetValue(CaretBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="CaretBrush"/>. See the related property for details.</summary>
        public static DependencyProperty CaretBrushProperty
            = TextBoxBase.CaretBrushProperty.AddOwner(typeof(RibbonTextBox));

        /// <summary>
        /// Gets or sets a value that indicates whether a horizontal scroll bar is shown.
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility { get => (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); set => SetValue(HorizontalScrollBarVisibilityProperty, value); }

        /// <summary>The backing dependency property for <see cref="HorizontalScrollBarVisibility"/>. See the related property for details.</summary>
        public static DependencyProperty HorizontalScrollBarVisibilityProperty
            = TextBoxBase.HorizontalScrollBarVisibilityProperty.AddOwner(typeof(RibbonTextBox));

        /// <summary>
        /// Gets or sets a value that indicates whether the text box displays selected text when the text box does not have focus.
        /// </summary>
        public bool IsInactiveSelectionHighlightEnabled { get => (bool)GetValue(IsInactiveSelectionHighlightEnabledProperty); set => SetValue(IsInactiveSelectionHighlightEnabledProperty, value); }

        /// <summary>The backing dependency property for <see cref="IsInactiveSelectionHighlightEnabled"/>. See the related property for details.</summary>
        public static DependencyProperty IsInactiveSelectionHighlightEnabledProperty
            = TextBoxBase.IsInactiveSelectionHighlightEnabledProperty.AddOwner(typeof(RibbonTextBox));

        /// <summary>
        /// Gets or sets a value that indicates whether a read-only text box displays a caret.
        /// </summary>
        public bool IsReadOnlyCaretVisible { get => (bool)GetValue(IsReadOnlyCaretVisibleProperty); set => SetValue(IsReadOnlyCaretVisibleProperty, value); }

        /// <summary>The backing dependency property for <see cref="IsReadOnlyCaretVisible"/>. See the related property for details.</summary>
        public static DependencyProperty IsReadOnlyCaretVisibleProperty
            = TextBoxBase.IsReadOnlyCaretVisibleProperty.AddOwner(typeof(RibbonTextBox));

        /// <summary>
        /// Gets or sets a value that indicates whether the text editing control is read-only to a user interacting with the control.
        /// </summary>
        public bool IsReadOnly { get => (bool)GetValue(IsReadOnlyProperty); set => SetValue(IsReadOnlyProperty, value); }

        /// <summary>The backing dependency property for <see cref="IsReadOnly"/>. See the related property for details.</summary>
        public static DependencyProperty IsReadOnlyProperty
            = TextBoxBase.IsReadOnlyProperty.AddOwner(typeof(RibbonTextBox));

        /// <summary>
        /// Gets a value that indicates whether the text box has focus and selected text.
        /// </summary>
        public bool IsSelectionActive { get => textBox.IsSelectionActive; }

        /// <summary>
        /// Gets or sets a value that indicates whether undo support is enabled for the text-editing control.
        /// </summary>
        public bool IsUndoEnabled { get => (bool)GetValue(IsUndoEnabledProperty); set => SetValue(IsUndoEnabledProperty, value); }

        /// <summary>The backing dependency property for <see cref="IsUndoEnabled"/>. See the related property for details.</summary>
        public static DependencyProperty IsUndoEnabledProperty
            = TextBoxBase.IsUndoEnabledProperty.AddOwner(typeof(RibbonTextBox));

        /// <summary>
        /// Gets or sets the brush that highlights selected text.
        /// </summary>
        public Brush SelectionBrush { get => (Brush)GetValue(SelectionBrushProperty); set => SetValue(SelectionBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectionBrush"/>. See the related property for details.</summary>
        public static DependencyProperty SelectionBrushProperty
            = TextBoxBase.SelectionBrushProperty.AddOwner(typeof(RibbonTextBox));

        /// <summary>
        /// Gets or sets the opacity of the <see cref="SelectionBrush"/>.
        /// </summary>
        public Brush SelectionOpacity { get => (Brush)GetValue(SelectionOpacityProperty); set => SetValue(SelectionOpacityProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectionOpacity"/>. See the related property for details.</summary>
        public static DependencyProperty SelectionOpacityProperty
            = TextBoxBase.SelectionOpacityProperty.AddOwner(typeof(RibbonTextBox));

#if NETCOREAPP || NET48_OR_GREATER
        /// <summary>
        /// Gets or sets a value that defines the brush used for selected text.
        /// </summary>
        public Brush SelectionTextBrush { get => (Brush)GetValue(SelectionTextBrushProperty); set => SetValue(SelectionTextBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectionTextBrush"/>. See the related property for details.</summary>
        public static DependencyProperty SelectionTextBrushProperty
            = TextBoxBase.SelectionTextBrushProperty.AddOwner(typeof(RibbonTextBox));
#endif

        /// <summary>
        /// Gets or sets the number of actions stored in the undo queue.
        /// </summary>
        public int UndoLimit { get => (int)GetValue(UndoLimitProperty); set => SetValue(UndoLimitProperty, value); }

        /// <summary>The backing dependency property for <see cref="UndoLimit"/>. See the related property for details.</summary>
        public static DependencyProperty UndoLimitProperty
            = TextBoxBase.UndoLimitProperty.AddOwner(typeof(RibbonTextBox));

        /// <summary>
        /// Gets or sets a value that indicates whether a vertical scroll bar is shown.
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility { get => (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); set => SetValue(VerticalScrollBarVisibilityProperty, value); }

        /// <summary>The backing dependency property for <see cref="VerticalScrollBarVisibility"/>. See the related property for details.</summary>
        public static DependencyProperty VerticalScrollBarVisibilityProperty
            = TextBoxBase.VerticalScrollBarVisibilityProperty.AddOwner(typeof(RibbonTextBox));


        #endregion

        #region Added Properties

        /// <summary>
        /// Get or set the height of the text box.
        /// </summary>
        public double TextBoxHeight { get => (double)GetValue(TextBoxHeightProperty); set => SetValue(TextBoxHeightProperty, value); }

        /// <summary>The backing dependency property for <see cref="TextBoxHeight"/>. See the related property for details.</summary>
        public static DependencyProperty TextBoxHeightProperty
            = DependencyProperty.Register(nameof(TextBoxHeight), typeof(double), typeof(RibbonTextBox),
            new FrameworkPropertyMetadata(24.0));


        /// <summary>
        /// Get or set the width of the text box.
        /// </summary>
        public double TextBoxWidth { get => (double)GetValue(TextBoxWidthProperty); set => SetValue(TextBoxWidthProperty, value); }

        /// <summary>The backing dependency property for <see cref="TextBoxWidth"/>. See the related property for details.</summary>
        public static DependencyProperty TextBoxWidthProperty
            = DependencyProperty.Register(nameof(TextBoxWidth), typeof(double), typeof(RibbonTextBox),
            new FrameworkPropertyMetadata(90.0));

        /// <summary>
        /// Get or set the vertical alignment for the text in the text box.
        /// </summary>
        public VerticalAlignment VerticalTextAlignment { get => (VerticalAlignment)GetValue(VerticalTextAlignmentProperty); set => SetValue(VerticalTextAlignmentProperty, value); }

        /// <summary>The backing dependency property for <see cref="VerticalTextAlignment"/>. See the related property for details.</summary>
        public static DependencyProperty VerticalTextAlignmentProperty
            = DependencyProperty.Register(nameof(VerticalTextAlignment), typeof(VerticalAlignment), typeof(RibbonTextBox),
            new FrameworkPropertyMetadata(VerticalAlignment.Center));


        #endregion

        #endregion
    }
}
