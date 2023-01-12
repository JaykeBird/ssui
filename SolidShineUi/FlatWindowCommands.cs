using System.Windows;
using System.Windows.Input;

namespace SolidShineUi
{
    /// <summary>
    /// Contains a collection of commands that can be used with the <see cref="FlatWindow"/> class (and any classes that inherit from it).
    /// </summary>
    public static class FlatWindowCommands
    {

        /// <summary>A WPF command that when executed, will close the specified window.</summary>
        public static RoutedCommand CloseWindow { get; } = new RoutedCommand("CloseWindow", typeof(FlatWindowCommands));

        /// <summary>A WPF command that when executed, will minimize the window (hide in the taskbar).</summary>
        public static RoutedCommand Minimize { get; } = new RoutedCommand("Minimize", typeof(FlatWindowCommands));

        /// <summary>A WPF command that when executed, will maximize the window (take up all space on the screen).</summary>
        public static RoutedCommand Maximize { get; } = new RoutedCommand("Maximize", typeof(FlatWindowCommands));

        /// <summary>A WPF command that when executed, will restore the window to its state prior to being minimized or maximized.</summary>
        public static RoutedCommand Restore { get; } = new RoutedCommand("Restore", typeof(FlatWindowCommands));
    }
}
