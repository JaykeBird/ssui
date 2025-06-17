using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SolidShineUi.Ribbon
{
    /// <summary>
    /// A list of commands for interacting with a <see cref="Ribbon"/> or controls within a Ribbon. Currently, this is blank.
    /// </summary>
    public static class RibbonCommands
    {

        /// <summary>A WPF command that when executed, will trigger the <see cref="RibbonGroup.LauncherClick"/> event.</summary>
        public static RoutedCommand DialogLauncherAction { get; } = new RoutedCommand("DialogLauncherAction", typeof(RibbonCommands));

        ///// <summary>A WPF command that when executed, will open or close a File menu in <see cref="RibbonFileMenu"/>.</summary>
        //public static RoutedCommand OpenCloseFileMenu { get; } = new RoutedCommand("OpenCloseFileMenu", typeof(RibbonCommands));


        /// <summary>
        /// A WPF command that when executed, will scroll the Ribbon's main bar to the left or right.
        /// </summary>
        public static RoutedCommand MainBarScrollCommand { get; } = new RoutedCommand("MainBarScrollCommand", typeof(RibbonCommands));

        /// <summary>
        /// A WPF command that when executed, will scroll the in-Ribbon viewing section of a Gallery.
        /// </summary>
        public static RoutedCommand GalleryScrollUp { get; } = new RoutedCommand("GalleryScrollUp", typeof(RibbonCommands));

        /// <summary>
        /// A WPF command that when executed, will scroll the in-Ribbon viewing section of a Gallery.
        /// </summary>
        public static RoutedCommand GalleryScrollDown { get; } = new RoutedCommand("GalleryScrollDown", typeof(RibbonCommands));

        /// <summary>
        /// A WPF command that when executed, will cause a Gallery to open its menu and display all of the contained options.
        /// </summary>
        public static RoutedCommand GalleryMenuExpand { get; } = new RoutedCommand("GalleryMenuExpand", typeof(RibbonCommands));
    }
}
