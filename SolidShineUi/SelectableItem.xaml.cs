using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.ComponentModel;

namespace SolidShineUi
{
    /// <summary>
    /// A standard selectable control that can be used with a <see cref="SelectPanel"/>. This has a number of customization options out of the box,
    /// including being able to set left-aligned and right-aligned text, an image, and also indent the contents to mimic the appearance of a tree view.
    /// If <see cref="AllowTextEditing"/> is set to true, users can even edit the text within the item.
    /// </summary>
    public partial class SelectableItem : SelectableUserControl
    {
        /// <summary>
        /// Create a SelectableItem to use with a SelectPanel.
        /// </summary>
        public SelectableItem()
        {
            InitializeComponent();
            Text = "";

            IsEnabledChanged += SelectableItem_IsEnabledChanged;
            chkSel.CheckChanged += ChkSel_CheckChanged;
            IsSelectedChanged += SelectableItem_SelectionChanged;
        }

        /// <summary>
        /// Create a SelectableItem to use with a SelectPanel, with the text preset.
        /// </summary>
        /// <param name="text">The text to display in the item.</param>
#if NETCOREAPP
        public SelectableItem(string? text)
#else
        public SelectableItem(string text)
#endif
        {
            InitializeComponent();
            Text = text ?? "";

            IsEnabledChanged += SelectableItem_IsEnabledChanged;
            chkSel.CheckChanged += ChkSel_CheckChanged;
            IsSelectedChanged += SelectableItem_SelectionChanged;
        }

        /// <summary>
        /// Create a SelectableItem to use with a SelectPanel, with certain properties preset.
        /// </summary>
        /// <param name="text">The text to display in the item.</param>
        /// <param name="image">The image to display in the item. (If the image is wider than 16 pixels, you may need to update the <c>ImageWidth</c> property.) Set to <c>null</c> to not show an image.</param>
        /// <param name="indent">The left indent to apply to the item's content. The indent can be used to make an improvised tree view.</param>
#if NETCOREAPP
        public SelectableItem(string? text, ImageSource? image, double indent = 0)
#else
        public SelectableItem(string text, ImageSource image, double indent = 0)
#endif
        {
            InitializeComponent();
            Text = text ?? "";
            ImageSource = image;
            Indent = indent;

            IsEnabledChanged += SelectableItem_IsEnabledChanged;
            chkSel.CheckChanged += ChkSel_CheckChanged;
            IsSelectedChanged += SelectableItem_SelectionChanged;
        }

        /// <summary>
        /// Apply a color scheme to this control. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply.</param>
        public override void ApplyColorScheme(ColorScheme cs)
        {
            base.ApplyColorScheme(cs);

            lblText.DisabledBrush = cs.DarkDisabledColor.ToBrush();
            lblText.TextBrush = cs.ForegroundColor.ToBrush();
            lblText.HighlightBrush = cs.ForegroundColor.ToBrush();

            chkSel.ApplyColorScheme(cs);
        }

        /// <inheritdoc/>
        protected override void OnApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
        {
            base.OnApplySsuiTheme(ssuiTheme, useLightBorder, useAccentTheme);

            lblText.DisabledBrush = ssuiTheme.DisabledForeground;
            lblText.TextBrush = ssuiTheme.Foreground;
            lblText.HighlightBrush = ssuiTheme.HighlightForeground;

            chkSel.SsuiTheme = ssuiTheme;
        }

        private void SelectableItem_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            lblText.IsEnabled = IsEnabled;
            chkSel.IsEnabled = IsEnabled;
        }

        #region UI Elements / Layout

        /// <summary>
        /// Get or set the left indent to apply to the item's content. This can be used to make an improvised tree view.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the left indent to apply to the item's content. This can be used to make an improvised tree view.")]
        public double Indent
        {
            get
            {
                return colIndent.Width.Value;
            }
            set
            {
                colIndent.Width = new GridLength(value);
            }
        }

        #region Checkbox

        /// <summary>
        /// Get or set whether a checkbox should be displayed on the item. Checkboxes can make it easy to select mutliple items.
        /// </summary>
        [Category("Common")]
        [Description("Get or set whether a checkbox should be displayed on the item. Checkboxes can make it easy to select mutliple items.")]
        public bool ShowCheckbox
        {
            get
            {
                return colCheck.Width.Value > 0;
            }
            set
            {
                if (value)
                {
                    colCheck.Width = new GridLength(24);
                    chkSel.IsEnabled = true;
                }
                else
                {
                    colCheck.Width = new GridLength(0);
                    chkSel.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Get or set the state of the checkbox. The checkbox is only shown if <see cref="ShowCheckbox"/> is set to true.
        /// </summary>
        [Category("Common")]
        [Description("Get or set the state of the checkbox. The checkbox is only shown if ShowCheckbox is set to true.")]
        public CheckState CheckboxState
        {
            get
            {
                return chkSel.CheckState;
            }
            set
            {
                chkSel.CheckState = value;
            }
        }

        /// <summary>
        /// Get or set if the checkbox is checked. The checkbox is only shown if <see cref="ShowCheckbox"/> is set to true.
        /// </summary>
        [Category("Common")]
        [Description("Get or set if the checkbox is checked. The checkbox is only shown if ShowCheckbox is set to true.")]
        public bool IsCheckboxChecked
        {
            get
            {
                return chkSel.IsChecked;
            }
            set
            {
                chkSel.IsChecked = value;
            }
        }

        /// <summary>
        /// Get or set if the checkbox's value should be changed if this control is selected (and vice-versa). By default, this value is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, changing the checkstate will also change whether this control is selected.
        /// When <c>false</c>, the checkstate can be changed without affecting whether this control is selected or not.
        /// </remarks>
        [Category("Common")]
        [Description("Get or set if the checkbox's value should be changed if this control is selected (and vice-versa). By default, this value is true.")]
        public bool MatchCheckboxValueToSelect { get; set; } = true;

        bool updatingCheck = false;

        private void ChkSel_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (MatchCheckboxValueToSelect && !updatingCheck)
            {
                updatingCheck = true;
                SetIsSelectedWithSource(chkSel.IsChecked, SelectionChangeTrigger.CheckBox, this);
                updatingCheck = false;
            }
        }

        private void SelectableItem_SelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            if (MatchCheckboxValueToSelect && !updatingCheck)
            {
                updatingCheck = true;
                chkSel.IsChecked = IsSelected;
                updatingCheck = false;
            }
        }

        #endregion

        #region Image

        double imgWidth = 16;

        /// <summary>
        /// Get or set the width to afford to the image. By default, the width is set to 16, but larger images will require the width to be set higher.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the width to afford to the image. By default, the width is set to 16, but larger images will require the width to be set higher.")]
        public double ImageWidth
        {
            get
            {
                return imgWidth;
            }
            set
            {
                if (double.IsNaN(value))
                {
                    ShowImage = false;
                    imgWidth = 16;
                    return;
                }

                if (ShowImage)
                {
                    colImage.Width = new GridLength(value + 8);
                }
                imgWidth = value;
            }
        }

        /// <summary>
        /// Get or set whether to display an image on the left side of the item. Set the image via the <c>ImageSource</c> property.
        /// </summary>
        [Category("Common")]
        [Description("Get or set whether to display an image on the left side of the item. Set the image via the ImageSource property.")]
        public bool ShowImage
        {
            get
            {
                return colImage.Width.Value > 0;
            }
            set
            {
                if (value)
                {
                    colImage.Width = new GridLength(imgWidth + 8);
                }
                else
                {
                    colImage.Width = new GridLength(0);
                }
            }
        }

        /// <summary>
        /// Get or set the source for the image to show on the left side of the item.
        /// </summary>
        /// <remarks>
        /// If this is updated to a value other than null, then <c>ShowImage</c> is automatically set to <c>true</c> to display the image
        /// (unless <c>AutoShowImageOnSourceSet</c> is set to false).
        /// </remarks>
        [Category("Common")]
        [Description("Get or set the source for the image to show on the left side of the item.")]
#if NETCOREAPP
        public ImageSource? ImageSource
#else
        public ImageSource ImageSource
#endif
        {
            get
            {
                return image.Source;
            }
            set
            {
                image.BeginInit();
                image.Source = value;
                image.EndInit();

                if (AutoShowImageOnSourceSet)
                {
                    ShowImage = value != null;
                }

                if (!double.IsNaN(image.ActualWidth) && image.ActualWidth != 0d)
                {
                    ImageWidth = image.ActualWidth;
                }
            }
        }

        /// <summary>
        /// Get or set whether the <c>ShowImage</c> property should be updated when the <c>ImageSource</c> property is set. Default is true.
        /// </summary>
        [Description("Get or set whether the ShowImage property should be updated when the ImageSource property is set. Default is true.")]
        public bool AutoShowImageOnSourceSet { get; set; } = true;

        #endregion

        #endregion

        #region Text

        /// <summary>
        /// Get or set the text to display within the item.
        /// <para/>
        /// This text can be edited by setting <see cref="AllowTextEditing"/> to true, or by calling <see cref="DisplayEditText()"/>.
        /// </summary>
        [Category("Common")]
        [Description("Get or set the text to display within the item.")]
        public string Text
        {
            get
            {
                return lblText.Text;
            }
            set
            {
                string old = lblText.Text;
                lblText.Text = value;

                RoutedPropertyChangedEventArgs<string> re = new RoutedPropertyChangedEventArgs<string>(old, value, TextChangedEvent);
                RaiseEvent(re);
            }
        }

        /// <summary>
        /// Get or set the width of the main text section of the control.
        /// <para/>
        /// This can be used to limit the width of the main text section, or make it as wide as needed for the full text to fit.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the width of the main text section of the control.")]
        public GridLength TextColumnWidth
        {
            get
            {
                return colText.Width;
            }
            set
            {
                colText.Width = value;
            }
        }

        /// <summary>
        /// Get or set the text trimming behavior to use when the text overflows the visible area.
        /// </summary>
        [Category("Text")]
        [Description("Get or set the text trimming behavior to use when the text overflows the visible area.")]
        public TextTrimming TextTrimming
        {
            get
            {
                return lblText.TextTrimming;
            }
            set
            {
                lblText.TextTrimming = value;
            }
        }

        #endregion

        #region RightText

        /// <summary>
        /// Get or set the text to display on the far-right side of the item. This text cannot be edited directly by the user.
        /// </summary>
        [Category("Common")]
        [Description("Get or set the text to display on the far-right side of the item. This text cannot be edited directly by the user.")]
        public string RightText
        {
            get
            {
                return lblRightText.Text;
            }
            set
            {
                lblRightText.Text = value;
            }
        }

        /// <summary>
        /// Get or set the FontFamily to use for the text on the right side of the control.
        /// </summary>
        [Category("Text")]
        [Description("Get or set the FontFamily to use for the text on the right side of the control.")]
        public FontFamily RightFontFamily
        {
            get
            {
                return lblRightText.FontFamily;
            }
            set
            {
                lblRightText.FontFamily = value;
            }
        }

        /// <summary>
        /// Get or set the font size to use for the text on the right side of the control.
        /// </summary>
        [Category("Text")]
        [Description("Get or set the font size to use for the text on the right side of the control.")]
        public double RightFontSize
        {
            get
            {
                return lblRightText.FontSize;
            }
            set
            {
                lblRightText.FontSize = value;
            }
        }

        /// <summary>
        /// Get or set the font weight to use for the text on the right side of the control.
        /// </summary>
        [Category("Text")]
        [Description("Get or set the font weight to use for the text on the right side of the control.")]
        public FontWeight RightFontWeight
        {
            get
            {
                return lblRightText.FontWeight;
            }
            set
            {
                lblRightText.FontWeight = value;
            }
        }

        /// <summary>
        /// Get or set the font stretch to use for the text on the right side of the control.
        /// </summary>
        [Category("Text")]
        [Description("Get or set the font stretch to use for the text on the right side of the control.")]
        public FontStretch RightFontStretch
        {
            get
            {
                return lblRightText.FontStretch;
            }
            set
            {
                lblRightText.FontStretch = value;
            }
        }

        /// <summary>
        /// Get or set the FontStyle to use for the text on the right side of the control.
        /// </summary>
        [Category("Text")]
        [Description("Get or set the FontStyle to use for the text on the right side of the control.")]
        public FontStyle RightFontStyle
        {
            get
            {
                return lblRightText.FontStyle;
            }
            set
            {
                lblRightText.FontStyle = value;
            }
        }

        /// <summary>
        /// Get or set the width of the right text section of the control.
        /// <para/>
        /// This can be used to limit the width of the right text section, or make it as wide as needed for the right text to fit fully.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the width of the right text section of the control.")]
        public GridLength RightTextWidth
        {
            get
            {
                return colRightText.Width;
            }
            set
            {
                colRightText.Width = value;
            }
        }
        #endregion

        #region Text Editing

        /// <summary>
        /// Get or set if the text should have an underline effect when the mouse is over the text. This is enabled by default when <c>AllowTextEditing</c> is set to "true", but otherwise is disabled.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set if the text should have an underline effect when the mouse is over the text. This is enabled by default when AllowTextEditing is set to true, but otherwise is disabled.")]
        public bool TextUnderlineOnMouseOver
        {
            get
            {
                return lblText.UnderlineOnHighlight;
            }
            set
            {
                lblText.UnderlineOnHighlight = value;
            }
        }

        /// <summary>
        /// The routed event object backing the related event. See <see cref="TextChanged"/> for more details.
        /// </summary>
        public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(TextChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<string>), typeof(SelectableItem));

        /// <summary>
        /// Raised when the Text property is changed, either via updating the property or via the the user's text editing view.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<string> TextChanged
        {
            add { AddHandler(TextChangedEvent, value); }
            remove { RemoveHandler(TextChangedEvent, value); }
        }

        /// <summary>
        /// Get or set whether all of the text should be selected when the text-editing text box recieves focus.
        /// </summary>
        [Category("Common")]
        [Description("Get or set whether all of the text should be selected when the text-editing text box recieves focus.")]
        public bool SelectOnFocusTextEdit
        {
            get
            {
                return txtText.SelectOnFocus;
            }
            set
            {
                txtText.SelectOnFocus = value;
            }
        }

        private bool _canEditText = false;

        /// <summary>
        /// Get or set whether the user should be allowed to edit the text in the Text property. If enabled, a text box will appear when the user clicks on the text.
        /// </summary>
        /// <remarks>
        /// While editing, pressing the Enter key will confirm the edit, while pressing the Escape key will cancel the operation.
        /// Editing can also be confirmed via <see cref="ConfirmEdit"/> or cancelled via <see cref="CancelEdit"/>.
        /// </remarks>
        [Category("Common")]
        [Description("Get or set whether the user should be allowed to edit the text in the Text property. If enabled, a text box will appear when the user clicks on the text.")]
        public bool AllowTextEditing
        {
            get { return _canEditText; }
            set
            {
                _canEditText = value;
                if (value)
                {
                    lblText.UnderlineOnHighlight = true;
                    lblText.Focusable = true;
                }
                else
                {
                    // if currently editing, just kick out of it
                    CancelEdit();
                    lblText.UnderlineOnHighlight = false;
                    lblText.Focusable = false;
                }
            }
        }

        private void lblText_Click(object sender, RoutedEventArgs e)
        {
            if (AllowTextEditing)
            {
                DisplayEditText();
            }
        }

        bool _editMode = false;

        /// <summary>
        /// Get if this control is currently in text edit mode.
        /// </summary>
        /// <remarks>
        /// To activate text edit mode, either set <see cref="AllowTextEditing"/> is set to true to make the text editable by clicking on it, 
        /// or call the <see cref="DisplayEditText"/> method.
        /// </remarks>
        [ReadOnly(true)]
        public bool IsCurrentlyEditingText { get { return _editMode; } }

        /// <summary>
        /// Display the text editing view to the user. This is the same as the user clicking on the text while <c>AllowTextEditing</c> is set to true.
        /// </summary>
        /// <remarks>This method will activate the text editing view even if <c>AllowTextEditing</c> is set to false.</remarks>
        public void DisplayEditText()
        {
            if (!_editMode)
            {
                txtText.Text = lblText.Text;
            }

            txtText.Visibility = Visibility.Visible;
            lblText.Visibility = Visibility.Collapsed;
            txtText.Focus();

            _editMode = true;
        }

        /// <summary>
        /// Confirm the user's edit to the text, as changed via the text editing view. Also exits the text editing view. 
        /// This can only be called when in the text editing view (see <see cref="IsCurrentlyEditingText"/>).
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if this item is not in the text editing view.</exception>
        public void ConfirmEdit()
        {
            if (_editMode)
            {
                string old = lblText.Text;
                lblText.Text = txtText.Text;

                txtText.Visibility = Visibility.Collapsed;
                lblText.Visibility = Visibility.Visible;
                _editMode = false;

                RoutedPropertyChangedEventArgs<string> re = new RoutedPropertyChangedEventArgs<string>(old, lblText.Text, TextChangedEvent);
                RaiseEvent(re);
            }
            else
            {
                throw new InvalidOperationException("This item is not in the text editing view.");
            }
        }

        /// <summary>
        /// Cancel the user's edit to the text and exits the text editing view (if in the view). This can be called at any time.
        /// </summary>
        public void CancelEdit()
        {
            txtText.Visibility = Visibility.Collapsed;
            lblText.Visibility = Visibility.Visible;
            _editMode = false;
        }

        private void txtText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ConfirmEdit();
            }
            else if (e.Key == Key.Escape)
            {
                CancelEdit();
            }
        }

        #endregion
    }
}
