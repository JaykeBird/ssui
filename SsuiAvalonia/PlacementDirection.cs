using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi
{
    /// <summary>
    /// Used to indicate the placement of a UI element within a larger parent element/control.
    /// </summary>
    public enum PlacementDirection
    {
        /// <summary>
        /// Hide the UI element.
        /// </summary>
        Hidden = 0,
        /// <summary>
        /// Display the UI element at the top side of the control.
        /// </summary>
        Top = 1,
        /// <summary>
        /// Display the UI element at the left side of the control.
        /// </summary>
        Left = 2,
        /// <summary>
        /// Display the UI element at the right side of the control.
        /// </summary>
        Right = 3,
        /// <summary>
        /// Display the UI element at the bottom side of the control.
        /// </summary>
        Bottom = 4,
    }
}
