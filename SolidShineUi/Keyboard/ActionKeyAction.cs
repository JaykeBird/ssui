using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if AVALONIA
using Avalonia.Controls;
using Avalonia.Input;
#else
using System.Windows;
#endif

namespace SolidShineUi.KeyboardShortcuts
{

    /// <summary>
    /// A key action that executes an <see cref="Action"/> delegate when activated.
    /// </summary>
    public class ActionKeyAction : IKeyAction
    {
        Action a;

#if AVALONIA
        /// <summary>
        /// Gets the UI element that this action is related to, if any.
        /// </summary>
        public InputElement? SourceElement { get; }

        /// <summary>
        /// Create a new ActionKeyAction.
        /// </summary>
        /// <param name="action">The action to execute when this key action is activated.</param>
        /// <param name="methodId">The unique ID of this key action.</param>
        /// <param name="sourceElement">The UI element, if any, this action is related to. For example, it could be a menu item or button that would alternatively trigger this action.</param>
        public ActionKeyAction(Action action, string methodId, Control? sourceElement = null)
        {
            a = action;
            ID = methodId;
            SourceElement = sourceElement;
        }
#elif NETCOREAPP
        /// <summary>
        /// Gets the UI element that this action is related to, if any.
        /// </summary>
        public UIElement? SourceElement { get; }

        /// <summary>
        /// Create a new ActionKeyAction.
        /// </summary>
        /// <param name="action">The action to execute when this key action is activated.</param>
        /// <param name="methodId">The unique ID of this key action.</param>
        /// <param name="sourceElement">The UI element, if any, this action is related to. For example, it could be a menu item or button that would alternatively trigger this action.</param>
        public ActionKeyAction(Action action, string methodId, UIElement? sourceElement = null)
        {
            a = action;
            ID = methodId;
            SourceElement = sourceElement;
        }

#else
        /// <summary>
        /// Gets the UI element that this action is related to, if any.
        /// </summary>
        public UIElement SourceElement { get; }

        /// <summary>
        /// Create a new ActionKeyAction.
        /// </summary>
        /// <param name="action">The action to execute when this key action is activated.</param>
        /// <param name="methodId">The unique ID of this key action.</param>
        /// <param name="sourceElement">The UI element, if any, this action is related to. For example, it could be a menu item or button that would alternatively trigger this action.</param>
        public ActionKeyAction(Action action, string methodId, UIElement sourceElement = null)
        {
            a = action;
            ID = methodId;
            SourceElement = sourceElement;
        }
#endif
        /// <summary>
        /// Get or set the unique ID associated with this key action.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Activate this key action.
        /// </summary>
        public void Execute()
        {
            a.Invoke();
        }
    }
}
