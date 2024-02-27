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

    }
}
