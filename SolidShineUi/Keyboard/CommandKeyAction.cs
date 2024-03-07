﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
#if AVALONIA
using Avalonia.Controls;
using Avalonia.Input;
#else
using System.Windows;
#endif

namespace SolidShineUi.KeyboardShortcuts
{

    /// <summary>
    /// A key action that executes a command when activated.
    /// </summary>
    public class CommandKeyAction : IKeyAction, ICommandSource
    {
        // interestingly, Avalonia uses System.Windows.Input.ICommand internally for handling commands, rather than using its own implementation
        // I can only hope/wonder that this means this particular type is also included on non-Windows systems... time will tell, I suppose!

#if AVALONIA
        object? cParam = null;
        //IInputElement? cTarget = null;

        /// <summary>
        /// Gets the UI element that this action is related to, if any.
        /// </summary>
        public InputElement? SourceElement { get; }

        /// <summary>
        /// Get the command parameter associated with this key action. When the command is executed, this parameter is also supplied.
        /// </summary>
        public object? CommandParameter { get => cParam; }
        ///// <summary>
        ///// Get the command target to associated with this key action. When the command is executed, it will be executed in relation to this target.
        ///// </summary>
        //public IInputElement? CommandTarget { get => cTarget; }

        /// <summary>
        /// Create a CommandKeyAction.
        /// </summary>
        /// <param name="command">The command to execute when this key action is activated.</param>
        /// <param name="commandParameter">The parameter to supply with this command. Set to null to not set a parameter.</param>
        /// <param name="methodId">The unique ID to associate with this key action.</param>
        /// <param name="sourceItem">The UI element, if any, associated with this command. For example, it could be a menu item or button that would alternatively execute this command.</param>
        public CommandKeyAction(ICommand command, object? commandParameter, string methodId, Control? sourceItem = null)
        {
            c = command;
            cParam = commandParameter;
            //cTarget = null;
            ID = methodId;
            SourceElement = sourceItem;
        }

        /// <summary>
        /// Create a CommandKeyAction.
        /// </summary>
        /// <param name="command">The command to execute when this key action is activated.</param>
        /// <param name="commandParameter">The parameter to supply with this command. Set to null to not set a parameter.</param>
        /// <param name="commandTarget">The target of the command. Set to null to not set a target.</param>
        /// <param name="methodId">The unique ID to associate with this key action.</param>
        /// <param name="sourceItem">The UI element, if any, associated with this command. For example, it could be a menu item or button that would alternatively execute this command.</param>
        public CommandKeyAction(ICommand command, object? commandParameter, IInputElement? commandTarget, string methodId, Control? sourceItem = null)
        {
            c = command;
            cParam = commandParameter;
            //cTarget = commandTarget;
            ID = methodId;
            SourceElement = sourceItem;
        }

        /// <summary>
        /// Create a CommandKeyAction.
        /// </summary>
        /// <param name="command">The command to execute when this key action is activated.</param>
        /// <param name="methodId">The unique ID to associate with this key action.</param>
        /// <param name="sourceItem">The UI element, if any, associated with this command. For example, it could be a menu item or button that would alternatively execute this command.</param>
        public CommandKeyAction(ICommand command, string methodId, Control? sourceItem = null)
        {
            c = command;
            cParam = null;
            //cTarget = null;
            ID = methodId;
            SourceElement = sourceItem;
        }
#elif NETCOREAPP
        object? cParam = null;
        IInputElement? cTarget = null;

        /// <summary>
        /// Gets the UI element that this action is related to, if any.
        /// </summary>
        public UIElement? SourceElement { get; }

        /// <summary>
        /// Get the command parameter associated with this key action. When the command is executed, this parameter is also supplied. Note: this can be null.
        /// </summary>
        public object CommandParameter { get => cParam!; }
        /// <summary>
        /// Get the command target to associated with this key action. When the command is executed, it will be executed in relation to this target. Note: this can be null.
        /// </summary>
        public IInputElement CommandTarget { get => cTarget!; }

        /// <summary>
        /// Create a CommandKeyAction.
        /// </summary>
        /// <param name="command">The command to execute when this key action is activated.</param>
        /// <param name="commandParameter">The parameter to supply with this command. Set to null to not set a parameter.</param>
        /// <param name="commandTarget">The target of the command. Set to null to not set a target.</param>
        /// <param name="methodId">The unique ID to associate with this key action.</param>
        /// <param name="sourceItem">The UI element, if any, associated with this command. For example, it could be a menu item or button that would alternatively execute this command.</param>
        public CommandKeyAction(ICommand command, object? commandParameter, IInputElement? commandTarget, string methodId, UIElement? sourceItem = null)
        {
            c = command;
            cParam = commandParameter;
            cTarget = commandTarget;
            ID = methodId;
            SourceElement = sourceItem;
        }

        /// <summary>
        /// Create a CommandKeyAction.
        /// </summary>
        /// <param name="command">The command to execute when this key action is activated.</param>
        /// <param name="methodId">The unique ID to associate with this key action.</param>
        /// <param name="sourceItem">The UI element, if any, associated with this command. For example, it could be a menu item or button that would alternatively execute this command.</param>
        public CommandKeyAction(ICommand command, string methodId, UIElement? sourceItem = null)
        {
            c = command;
            cParam = null;
            cTarget = null;
            ID = methodId;
            SourceElement = sourceItem;
        }

#else
        object cParam = null;
        IInputElement cTarget = null;

        /// <summary>
        /// Gets the UI element that this action is related to, if any.
        /// </summary>
        public UIElement SourceElement { get; }
        
        /// <summary>
        /// Get the command parameter associated with this key action. When the command is executed, this parameter is also supplied. Note: this can be null.
        /// </summary>
        public object CommandParameter { get => cParam; }
        /// <summary>
        /// Get the command target to associated with this key action. When the command is executed, it will be executed in relation to this target. Note: this can be null.
        /// </summary>
        public IInputElement CommandTarget { get => cTarget; }

        /// <summary>
        /// Create a CommandKeyAction.
        /// </summary>
        /// <param name="command">The command to execute when this key action is activated.</param>
        /// <param name="commandParameter">The parameter to supply with this command. Set to null to not set a parameter.</param>
        /// <param name="commandTarget">The target of the command. Set to null to not set a target.</param>
        /// <param name="methodId">The unique ID to associate with this key action.</param>
        /// <param name="sourceItem">The UI element, if any, associated with this command. For example, it could be a menu item or button that would alternatively execute this command.</param>
        public CommandKeyAction(ICommand command, object commandParameter, IInputElement commandTarget, string methodId, UIElement sourceItem = null)
        {
            c = command;
            cParam = commandParameter;
            cTarget = commandTarget;
            ID = methodId;
            SourceElement = sourceItem;
        }

        /// <summary>
        /// Create a CommandKeyAction.
        /// </summary>
        /// <param name="command">The command to execute when this key action is activated.</param>
        /// <param name="methodId">The unique ID to associate with this key action.</param>
        /// <param name="sourceItem">The UI element, if any, associated with this command. For example, it could be a menu item or button that would alternatively execute this command.</param>
        public CommandKeyAction(ICommand command, string methodId, UIElement sourceItem = null)
        {
            c = command;
            cParam = null;
            cTarget = null;
            ID = methodId;
            SourceElement = sourceItem;
        }
#endif

        /// <summary>
        /// Creates a collection of CommandKeyAction actions, by loading the static properties of a class (such as <see cref="ApplicationCommands"/> or <see cref="FlatWindowCommands"/>).
        /// </summary>
        /// <param name="commandsClass">The type of the class that contains the commands to load. The commands should be static properties in that type.</param>
        /// <param name="commandParameter">The optional parameter to supply with the commands, to add on to the CommandKeyActions.</param>
        /// <param name="commandTarget">The optional target for the commands, to add on to the CommandKeyActions.</param>
        /// <remarks>
        /// This method uses reflection to load the static properties of a class. 
        /// This only looks at static properties (i.e. <c>public static RoutedCommand CommandName { get; }</c>), so commands stored as a field or a non-static property will not be found.
        /// </remarks>
        /// <returns>A list of CommandKeyActions, one for each static command property in this class.</returns>
#if NETCOREAPP
        public static KeyActionList LoadFromType(Type commandsClass, object? commandParameter = null, IInputElement? commandTarget = null)
#else
        public static KeyActionList LoadFromType(Type commandsClass, object commandParameter = null, IInputElement commandTarget = null)
#endif
        {
            KeyActionList kal = new KeyActionList();

            var props = commandsClass.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            foreach (var prop in props)
            {
                if (typeof(ICommand).IsAssignableFrom(prop.PropertyType))
                {
                    // this is an ICommand
                    if (prop.GetValue(null, null) is ICommand ic)
                    {
                        kal.Add(new CommandKeyAction(ic, commandParameter, commandTarget, $"{commandsClass.Name}.{prop.Name}"));
                    }
                }
            }

            return kal;
        }

        ICommand c;

        /// <summary>
        /// Get or set the unique ID associated with this key action.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Get the command that will be executed when this key action is activated.
        /// </summary>
#if AVALONIA
        public ICommand? Command { get => c; }
#else
        public ICommand Command { get => c; }
#endif

        /// <summary>
        /// Gets a value indicating whether this control and all its parents are enabled.
        /// </summary>
        public bool IsEffectivelyEnabled => true;

        /// <summary>
        /// Activate this key action.
        /// </summary>
        public void Execute()
        {
            if (c.CanExecute(cParam))
            {
#if AVALONIA
                // Avalonia doesn't implement or use RoutedCommands, I guess?
                c.Execute(cParam);
#else
                if (c is RoutedCommand rc)
                {
                    rc.Execute(cParam, cTarget);
                }
                else
                {
                    c.Execute(cParam);
                }
#endif
            }
        }

        /// <summary>
        /// Raised when the underlying <see cref="Command"/> has changed whether it's able to be executed or not.
        /// </summary>
        public void CanExecuteChanged(object sender, EventArgs e)
        {
            // I don't actually have anything to update or change based upon this
            // soooooo I'm not actually going to do anything!
        }
    }
}
