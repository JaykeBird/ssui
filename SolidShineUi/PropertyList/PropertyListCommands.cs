using System.Windows.Input;

namespace SolidShineUi.PropertyList
{
    /// <summary>
    /// A collection of Commands to use with the <see cref="PropertyList"/> control.
    /// </summary>
    public static class PropertyListCommands
    {
        /// <summary>A WPF command that when executed, will load the parameter object into a PropertyList.</summary>
        /// <remarks>See <see cref="PropertyList.LoadObject(object)"/> for more details.</remarks>
        public static RoutedCommand LoadObject { get; } = new RoutedCommand("LoadObject", typeof(PropertyListCommands));

        /// <summary>A WPF command that when executed, will prompt the PropertyList to reload the currently loaded object</summary>
        public static RoutedCommand Reload { get; } = new RoutedCommand("Reload", typeof(PropertyListCommands));


        /// <summary>A WPF command that when executed, will sort a PropertyList by name.</summary>
        /// <remarks>See also <see cref="PropertyList.SortOption"/> property.</remarks>
        public static RoutedCommand SortByName { get; } = new RoutedCommand("SortByName", typeof(PropertyListCommands));


        /// <summary>A WPF command that when executed, will sort a PropertyList by name.</summary>
        /// <remarks>See also <see cref="PropertyList.SortOption"/> property.</remarks>
        public static RoutedCommand SortByCategory { get; } = new RoutedCommand("SortByCategory", typeof(PropertyListCommands));


        /// <summary>A WPF command that when executed, will turn on or off gridlines for a PropertyList.</summary>
        /// <remarks>See also <see cref="PropertyList.ShowGridlines"/> property.</remarks>
        public static RoutedCommand ToggleGridlines { get; } = new RoutedCommand("ToggleGridlines", typeof(PropertyListCommands));


        /// <summary>A WPF command that when executed, will display a dialog so the end user can change the gridline brush for a PropertyList.</summary>
        /// <remarks>See also <see cref="PropertyList.GridlineBrush"/> property.</remarks>
        public static RoutedCommand ChangeGridlineBrush { get; } = new RoutedCommand("ChangeGridlineBrush", typeof(PropertyListCommands));


        /// <summary>A WPF command that when executed, will show or hide inherited properties for a PropertyList.</summary>
        /// <remarks>See also <see cref="PropertyList.ShowInheritedProperties"/> property.</remarks>
        public static RoutedCommand ToggleInheritedProperties { get; } = new RoutedCommand("ToggleInheritedProperties", typeof(PropertyListCommands));


        /// <summary>A WPF command that when executed, will show or hide read-only properties for a PropertyList.</summary>
        /// <remarks>See also <see cref="PropertyList.ShowReadOnlyProperties"/> property.</remarks>
        public static RoutedCommand ToggleReadOnlyProperties { get; } = new RoutedCommand("ToggleReadOnlyProperties", typeof(PropertyListCommands));

    }
}
