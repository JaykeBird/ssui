using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// A basic control that can be displayed in a SelectPanel, with the ability to set its title, icon, and also display a Remove button.
    /// </summary>
    public partial class ImageTextListItem : SelectableUserControl, ICommandSource
    {
        /// <summary>
        /// Create an ImageTextListItem.
        /// </summary>
        public ImageTextListItem()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Raised when the control requested to be removed, such as when the Remove button is clicked.
        /// </summary>
#if NETCOREAPP
        public event EventHandler? RequestRemove;
#else
        public event EventHandler RequestRemove;
#endif

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            RequestRemove?.Invoke(this, EventArgs.Empty);
        }

        #region Properties

        /// <summary>The backing dependency property for <see cref="Title"/>. See the related property for details.</summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title), typeof(string), typeof(ImageTextListItem),
            new PropertyMetadata("(no title)"));

        /// <summary>The backing dependency property for <see cref="CanRemove"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CanRemoveProperty = DependencyProperty.Register(
            nameof(CanRemove), typeof(bool), typeof(ImageTextListItem),
            new PropertyMetadata(true));

        /// <summary>The backing dependency property for <see cref="ShowIcon"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register(
            nameof(ShowIcon), typeof(bool), typeof(ImageTextListItem),
            new PropertyMetadata(true));

        /// <summary>The backing dependency property for <see cref="Icon"/>. See the related property for details.</summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon), typeof(ImageSource), typeof(ImageTextListItem),
            new PropertyMetadata(null));

        /// <summary>
        /// Get or set the title or text to display within the control.
        /// </summary>
        [Category("Common")]
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Get or set if this item can be removed. If true, the Remove button is visible; if false, the Remove button is hidden.
        /// </summary>
        [Category("Common")]
        public bool CanRemove
        {
            get => (bool)GetValue(CanRemoveProperty);
            set => SetValue(CanRemoveProperty, value);
        }

        /// <summary>
        /// Get or set if the icon on the left side of the control should be visible.
        /// </summary>
        [Category("Common")]
        public bool ShowIcon
        {
            get => (bool)GetValue(ShowIconProperty);
            set => SetValue(ShowIconProperty, value);
        }

        /// <summary>
        /// Get or set the icon displayed on the left side of the control.
        /// </summary>
        [Category("Icon")]
        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        #endregion

        #region Button Brushes

        /// <summary>The backing dependency property for <see cref="ButtonBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ButtonBackgroundProperty = DependencyProperty.Register(
            nameof(ButtonBackground), typeof(Brush), typeof(ImageTextListItem),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>The backing dependency property for <see cref="ButtonHighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ButtonHighlightBrushProperty = DependencyProperty.Register(
            nameof(ButtonHighlightBrush), typeof(Brush), typeof(ImageTextListItem),
            new PropertyMetadata(new SolidColorBrush(Colors.Gainsboro)));

        /// <summary>The backing dependency property for <see cref="ButtonClickBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ButtonClickBrushProperty = DependencyProperty.Register(
            nameof(ButtonClickBrush), typeof(Brush), typeof(ImageTextListItem),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>The backing dependency property for <see cref="ButtonBackgroundDisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ButtonBackgroundDisabledBrushProperty = DependencyProperty.Register(
            nameof(ButtonBackgroundDisabledBrush), typeof(Brush), typeof(ImageTextListItem),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>The backing dependency property for <see cref="ButtonBorderDisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ButtonBorderDisabledBrushProperty = DependencyProperty.Register(
            nameof(ButtonBorderDisabledBrush), typeof(Brush), typeof(ImageTextListItem),
            new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));

        /// <summary>
        /// Get or set the brush used for the background of the Remove button in the control.
        /// </summary>
        [Category("Brushes")]
        public Brush ButtonBackground
        {
            get => (Brush)GetValue(ButtonBackgroundProperty);
            set => SetValue(ButtonBackgroundProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the Remove button when it is highlighted (i.e. has mouse over it, or has keyboard focus).
        /// </summary>
        [Category("Brushes")]
        public Brush ButtonHighlightBrush
        {
            get => (Brush)GetValue(ButtonHighlightBrushProperty);
            set => SetValue(ButtonHighlightBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the Remove button when it is being clicked.
        /// </summary>
        [Category("Brushes")]
        public Brush ButtonClickBrush
        {
            get => (Brush)GetValue(ButtonClickBrushProperty);
            set => SetValue(ButtonClickBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of the Remove button when the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush ButtonBackgroundDisabledBrush
        {
            get => (Brush)GetValue(ButtonBackgroundDisabledBrushProperty);
            set => SetValue(ButtonBackgroundDisabledBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the border of the Remove button when the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush ButtonBorderDisabledBrush
        {
            get => (Brush)GetValue(ButtonBorderDisabledBrushProperty);
            set => SetValue(ButtonBorderDisabledBrushProperty, value);
        }

        #endregion

        #region Button Command Properties

        /// <summary>The backing dependency property for <see cref="ButtonToolTip"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ButtonToolTipProperty = DependencyProperty.Register(
            nameof(ButtonToolTip), typeof(object), typeof(ImageTextListItem),
            new PropertyMetadata(null));

        /// <summary>The backing dependency property for <see cref="Command"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            nameof(Command), typeof(ICommand), typeof(ImageTextListItem),
            new PropertyMetadata(null));

        /// <summary>The backing dependency property for <see cref="CommandParameter"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            nameof(CommandParameter), typeof(object), typeof(ImageTextListItem),
            new PropertyMetadata(null));

        /// <summary>The backing dependency property for <see cref="CommandTarget"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register(
            nameof(CommandTarget), typeof(IInputElement), typeof(ImageTextListItem),
            new PropertyMetadata(null));

        /// <summary>
        /// Get or set the command that is executed when the Remove button is pressed.
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Get or set the parameter sent with the command when it is executed.
        /// </summary>
        public object CommandParameter
        {
            get => (object)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Get or set the target of the command.
        /// </summary>
        public IInputElement CommandTarget
        {
            get => (IInputElement)GetValue(CommandTargetProperty);
            set => SetValue(CommandTargetProperty, value);
        }

        /// <summary>
        /// Get or set the tooltip to display with the Remove button.
        /// </summary>
        public object ButtonToolTip
        {
            get => (object)GetValue(ButtonToolTipProperty);
            set => SetValue(ButtonToolTipProperty, value);
        }

        #endregion

    }
}
