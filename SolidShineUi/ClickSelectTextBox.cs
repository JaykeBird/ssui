using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SolidShineUi
{

    // thanks to StackOverflow user Donnelle
    // http://stackoverflow.com/questions/660554/how-to-automatically-select-all-text-on-focus-in-wpf-textbox

    /// <summary>
    /// A TextBox where all the text is automatically selected when the text box gets focus (i.e. mouse click or keyboard focus).
    /// </summary>
    public class ClickSelectTextBox : TextBox
    {
        /// <summary>
        /// Creates a new ClickSelectTextBox and defines the default event handlers.
        /// </summary>
        public ClickSelectTextBox()
        {
            AddHandler(PreviewMouseLeftButtonDownEvent,
              new MouseButtonEventHandler(MouseButtonDown), true);
            AddHandler(GotKeyboardFocusEvent,
              new RoutedEventHandler(SelectAllText), true);
            AddHandler(MouseDoubleClickEvent,
              new RoutedEventHandler(SelectAllText), true);
        }

        /// <summary>
        /// Get or set whether all text will be selected when the text box receives focus.
        /// </summary>
        [Category("Common")]
        public bool SelectOnFocus { get => (bool)GetValue(SelectOnFocusProperty); set => SetValue(SelectOnFocusProperty, value); }

        /// <summary>
        /// The dependency property backing its related property. See <see cref="SelectOnFocus"/> for more details.
        /// </summary>
        public static readonly DependencyProperty SelectOnFocusProperty
            = DependencyProperty.Register("SelectOnFocus", typeof(bool), typeof(ClickSelectTextBox),
            new FrameworkPropertyMetadata(true));


        /// <summary>
        /// Handler for when the left mouse button is pressed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event arguments representing the mouse button being pressed.</param>
        private static void MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Find the TextBox
            if (e.OriginalSource is DependencyObject parent)
            //DependencyObject parent = e.OriginalSource as UIElement;
            {
                while (parent != null && !(parent is TextBox))
                    parent = VisualTreeHelper.GetParent(parent);

                if (parent != null)
                {
                    var textBox = (TextBox)parent;
                    if (!textBox.IsKeyboardFocusWithin)
                    {
                        // If the text box is not yet focussed, give it the focus and
                        // stop further processing of this click event.
                        textBox.Focus();
                        e.Handled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Locate the TextBox and have it select all text.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event arguments providing details about the event.</param>
        private static void SelectAllText(object sender, RoutedEventArgs e)
        {
            if (sender is ClickSelectTextBox c)
            {
                if (c.SelectOnFocus)
                {
                    if (e.OriginalSource is TextBox textBox)
                        textBox.SelectAll();
                }
            }
            else
            {
                if (e.OriginalSource is TextBox textBox)
                    textBox.SelectAll();
            }
        }
    }
}
