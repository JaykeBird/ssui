using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;

namespace SolidShineUi
{
    // created by Nimgoble, source: https://github.com/Nimgoble/WPFTextBoxAutoComplete

    /// <summary>
    /// Adds basic auto-complete behavior to text boxes via attached properties. Created by Nimgoble.
    /// </summary>
    public class AutoCompleteBehavior : AvaloniaObject
    {
        private static EventHandler<TextChangedEventArgs> onTextChanged = new EventHandler<TextChangedEventArgs>(OnTextChanged);
        private static EventHandler<KeyEventArgs> onKeyDown = new EventHandler<KeyEventArgs>(OnPreviewKeyDown);

        static AutoCompleteBehavior()
        {
            AutoCompleteItemsSource.Changed.AddClassHandler<TextBox>(OnAutoCompleteItemsSource);
        }

        /// <summary>
        /// The collection to search for auto-complete matches from.
        /// </summary>
        public static readonly AttachedProperty<IEnumerable<string>> AutoCompleteItemsSource =
            AvaloniaProperty.RegisterAttached<AutoCompleteBehavior, TextBox, IEnumerable<string>>("AutoCompleteItemsSource");

        /// <summary>
        /// The string comparison method to utilize when supplying auto-completion suggestions.
        /// </summary>
        public static readonly AttachedProperty<StringComparison> AutoCompleteStringComparison =
            AvaloniaProperty.RegisterAttached<AutoCompleteBehavior, TextBox, StringComparison>("AutoCompleteStringComparison", StringComparison.Ordinal);

        /// <summary>
        /// The string that, when typed, activates the auto-completion suggestions, such as "@". If this is null or empty,
        /// auto-complete suggestions will be supplied once any text is typed into the TextBox.
        /// </summary>
		public static readonly AttachedProperty<string> AutoCompleteIndicator =
            AvaloniaProperty.RegisterAttached<AutoCompleteBehavior, TextBox, string>("AutoCompleteIndicator", string.Empty);

        #region Items Source
        /// <summary>
        /// Return the string list/enumerable associated with a specified object; this list is used as the source for the auto complete list.
        /// </summary>
        public static IEnumerable<string> GetAutoCompleteItemsSource(TextBox obj)
        {
            object objRtn = obj.GetValue(AutoCompleteItemsSource);
            if (objRtn is IEnumerable<string> s)
            {
                return s;
            }
            else
            {
#if NETCOREAPP || AVALONIA
                return new List<string>();
#else
                return null;
#endif
            }
        }

        /// <summary>
        /// Set the string list/enumerable used for the auto complete list for the specified object.
        /// </summary>
        public static void SetAutoCompleteItemsSource(TextBox obj, IEnumerable<string> value)
        {
            obj.SetValue(AutoCompleteItemsSource, value);
        }

        private static void OnAutoCompleteItemsSource(TextBox sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (sender == null)
                return;

            //If we're being removed, remove the callbacks
            //Remove our old handler, regardless of if we have a new one.
            sender.TextChanged -= onTextChanged;
            sender.RemoveHandler(InputElement.KeyDownEvent, onKeyDown);
            if (e.NewValue != null)
            {
                //New source.  Add the callbacks
                sender.TextChanged += onTextChanged;
                sender.AddHandler(InputElement.KeyDownEvent, onKeyDown, RoutingStrategies.Tunnel);
            }
        }
        #endregion

        #region String Comparison
        /// <summary>
        /// Get the string comparison method to utilize when supplying auto-completion suggestions.
        /// </summary>
        public static StringComparison GetAutoCompleteStringComparison(TextBox obj)
        {
            return (StringComparison)obj.GetValue(AutoCompleteStringComparison);
        }

        /// <summary>
        /// Set the string comparison method to utilize when supplying auto-completion suggestions.
        /// </summary>
        public static void SetAutoCompleteStringComparison(TextBox obj, StringComparison value)
        {
            obj.SetValue(AutoCompleteStringComparison, value);
        }
        #endregion

        #region Indicator
        /// <summary>
        /// Get the string that, when typed, activates the auto-completion suggestions, such as "@". If this is null or empty,
        /// auto-complete suggestions will be supplied once any text is typed into the TextBox.
        /// </summary>
        public static string GetAutoCompleteIndicator(TextBox obj)
        {
            return (string)obj.GetValue(AutoCompleteIndicator);
        }

        /// <summary>
        /// Set the string that, when typed, activates the auto-completion suggestions, such as "@". If this is null or empty,
        /// auto-complete suggestions will be supplied once any text is typed into the TextBox.
        /// </summary>
        public static void SetAutoCompleteIndicator(TextBox obj, string value)
        {
            obj.SetValue(AutoCompleteIndicator, value);
        }
        #endregion

        /// <summary>
        /// Used for moving the caret to the end of the suggested auto-completion text.
        /// </summary>
        static void OnPreviewKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            if (e.Source is TextBox tb)
            {
                if (tb == null) { return; }

                //If we pressed enter and if the selected text goes all the way to the end, move our caret position to the end
                if (tb.SelectionEnd > 0 && (tb.SelectionEnd == tb.Text?.Length))
                {
                    tb.SelectionStart = tb.CaretIndex = tb.Text.Length;
                    tb.ClearSelection();
                }
            }
        }

        /// <summary>
        /// Search for auto-completion suggestions.
        /// </summary>
        static void OnTextChanged(object? sender, TextChangedEventArgs e)
        {
            // this is supposed to be a short-circuit when text is being deleted (removed)
            // but the TextChangedEventArgs object actually doesn't have any properties or values (that help with identifying what changed)
            // the only way I can get what changed on a TextBox is to subscribe to changes to the TextBox's TextProperty,
            // which involves holding an IDisposable, which I'll later have to dispose - which with static methods, it's annoying to do
            // so for now, I'm just going to keep this around, and maybe a later version of Avalonia will add on what's needed to TextChangedEventArgs

            //if ((from change in e.Changes where change.RemovedLength > 0 select change).Any() &&
            //    (from change in e.Changes where change.AddedLength > 0 select change).Any() == false)
            //    return;

            if (sender == null) return;

            if (e.Source is TextBox tb)
            {
                IEnumerable<string> values = GetAutoCompleteItemsSource(tb);
                //No reason to search if we don't have any values.
                if (values == null || !values.Any()) { return; } // values.Count() == 0

                //No reason to search if there's nothing there.
                if (string.IsNullOrEmpty(tb.Text)) { return; }

                string indicator = GetAutoCompleteIndicator(tb);
                int startIndex = 0; //Start from the beginning of the line.
                string matchingString = tb.Text;
                //If we have a trigger string, make sure that it has been typed before
                //giving auto-completion suggestions.
                if (!string.IsNullOrEmpty(indicator))
                {
                    startIndex = tb.Text.LastIndexOf(indicator);
                    //If we haven't typed the trigger string, then don't do anything.
                    if (startIndex == -1) { return; }

                    startIndex += indicator.Length;
                    matchingString = tb.Text.Substring(startIndex, tb.Text.Length - startIndex);
                }

                //If we don't have anything after the trigger string, return.
                if (string.IsNullOrEmpty(matchingString)) { return; }

                int textLength = matchingString.Length;

                StringComparison comparer = GetAutoCompleteStringComparison(tb);
                //Do search and changes here.
                string match =
                (
                    from
                        value
                    in
                    (
                        from subvalue
                        in values
                        where subvalue != null && subvalue.Length >= textLength
                        select subvalue
                    )
                    where value.Substring(0, textLength).Equals(matchingString, comparer)
                    select value.Substring(textLength, value.Length - textLength)/*Only select the last part of the suggestion*/
                ).FirstOrDefault() ?? string.Empty;

                //Nothing.  Leave 'em alone
                if (string.IsNullOrEmpty(match)) { return; }

                int matchStart = (startIndex + matchingString.Length);
                tb.TextChanged -= onTextChanged;
                tb.Text += match;
                tb.CaretIndex = matchStart;
                tb.SelectionStart = matchStart;
                tb.SelectionEnd = (tb.Text.Length - startIndex);
                tb.TextChanged += onTextChanged;
            }
        }
    }
}
