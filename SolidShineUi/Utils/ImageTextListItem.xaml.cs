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
    /// Interaction logic for ImageTextListItem.xaml
    /// </summary>
    public partial class ImageTextListItem : SelectableUserControl, ICommandSource
    {
        public ImageTextListItem()
        {
            InitializeComponent();
        }

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

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(ImageTextListItem),
            new PropertyMetadata("(no title)"));

        public static readonly DependencyProperty CanRemoveProperty = DependencyProperty.Register(
            "CanRemove", typeof(bool), typeof(ImageTextListItem),
            new PropertyMetadata(true));

        public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register(
            "ShowIcon", typeof(bool), typeof(ImageTextListItem),
            new PropertyMetadata(true));

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(ImageSource), typeof(ImageTextListItem),
            new PropertyMetadata(null));

        [Category("Common")]
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        [Category("Common")]
        public bool CanRemove
        {
            get => (bool)GetValue(CanRemoveProperty);
            set => SetValue(CanRemoveProperty, value);
        }

        [Category("Common")]
        public bool ShowIcon
        {
            get => (bool)GetValue(ShowIconProperty);
            set => SetValue(ShowIconProperty, value);
        }

        [Category("Icon")]
        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        #endregion

        #region Button Brushes

        public static readonly DependencyProperty ButtonBackgroundProperty = DependencyProperty.Register(
            "ButtonBackground", typeof(Brush), typeof(ImageTextListItem),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty ButtonHighlightBrushProperty = DependencyProperty.Register(
            "ButtonHighlightBrush", typeof(Brush), typeof(ImageTextListItem),
            new PropertyMetadata(new SolidColorBrush(Colors.Gainsboro)));

        public static readonly DependencyProperty ButtonClickBrushProperty = DependencyProperty.Register(
            "ButtonClickBrush", typeof(Brush), typeof(ImageTextListItem),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public static readonly DependencyProperty ButtonBackgroundDisabledBrushProperty = DependencyProperty.Register(
            "ButtonBackgroundDisabledBrush", typeof(Brush), typeof(ImageTextListItem),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty ButtonBorderDisabledBrushProperty = DependencyProperty.Register(
            "ButtonBorderDisabledBrush", typeof(Brush), typeof(ImageTextListItem),
            new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));

        [Category("Brushes")]
        public Brush ButtonBackground
        {
            get => (Brush)GetValue(ButtonBackgroundProperty);
            set => SetValue(ButtonBackgroundProperty, value);
        }

        [Category("Brushes")]
        public Brush ButtonHighlightBrush
        {
            get => (Brush)GetValue(ButtonHighlightBrushProperty);
            set => SetValue(ButtonHighlightBrushProperty, value);
        }

        [Category("Brushes")]
        public Brush ButtonClickBrush
        {
            get => (Brush)GetValue(ButtonClickBrushProperty);
            set => SetValue(ButtonClickBrushProperty, value);
        }

        [Category("Brushes")]
        public Brush ButtonBackgroundDisabledBrush
        {
            get => (Brush)GetValue(ButtonBackgroundDisabledBrushProperty);
            set => SetValue(ButtonBackgroundDisabledBrushProperty, value);
        }

        [Category("Brushes")]
        public Brush ButtonBorderDisabledBrush
        {
            get => (Brush)GetValue(ButtonBorderDisabledBrushProperty);
            set => SetValue(ButtonBorderDisabledBrushProperty, value);
        }

        #endregion

        #region Button Command Properties

        public static readonly DependencyProperty ButtonToolTipProperty = DependencyProperty.Register(
            "ButtonToolTip", typeof(object), typeof(ImageTextListItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(ImageTextListItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter", typeof(object), typeof(ImageTextListItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register(
            "CommandTarget", typeof(IInputElement), typeof(ImageTextListItem),
            new PropertyMetadata(null));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => (object)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public IInputElement CommandTarget
        {
            get => (IInputElement)GetValue(CommandTargetProperty);
            set => SetValue(CommandTargetProperty, value);
        }

        public object ButtonToolTip
        {
            get => (object)GetValue(ButtonToolTipProperty);
            set => SetValue(ButtonToolTipProperty, value);
        }

        #endregion

    }
}
