using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation.Peers;

// This comes from the .NET Foundation's WPF repository
// https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/System.Windows.Controls.Ribbon/Microsoft/Windows/Automation/Peers/RibbonTwoLineTextAutomationPeer.cs
// This is licensed under the MIT license and thus is free for me to reuse.

namespace SolidShineUi.Toolbars.Ribbon.Utils
{
    /// <summary>
    ///   An automation peer class which automates the RibbonTwoLineText control.
    /// </summary>
    public class RibbonTwoLineTextAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        ///   Initialize automation peer for RibbonTwoLineText.
        /// </summary>
        public RibbonTwoLineTextAutomationPeer(RibbonTwoLineText owner) : base(owner) { }

        /// <summary>
        ///   Return class name for automation clients to display
        /// </summary> 
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <inheritdoc/>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Text;
        }

        /// <inheritdoc/>
        protected override bool IsControlElementCore()
        {
            // Return false if RibbonTwoLineText is part of a ControlTemplate, otherwise return the base method
            RibbonTwoLineText tlt = (RibbonTwoLineText)Owner;
            DependencyObject templatedParent = tlt.TemplatedParent;
            // If the templatedParent is a ContentPresenter, this RibbonTwoLineText is generated from a DataTemplate
            if (templatedParent == null || templatedParent is ContentPresenter)
            {
                return base.IsControlElementCore();
            }

            return false;
        }

        /// <summary>
        ///   Returns name for automation clients to display
        /// </summary>
        protected override string GetNameCore()
        {
            string name = base.GetNameCore();
            if (string.IsNullOrEmpty(name))
            {
                name = ((RibbonTwoLineText)Owner).Text;
            }

            return name;
        }

    }
}
