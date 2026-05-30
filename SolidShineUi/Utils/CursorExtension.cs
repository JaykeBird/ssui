using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// A class holding an extension method for the <see cref="CursorType"/> enum.
    /// </summary>
    public static class CursorExtension
    {

        /// <summary>
        /// Return a <see cref="Cursor"/> that is represented by the this <paramref name="cursorType"/>.
        /// </summary>
        /// <param name="cursorType">The cursor type value to get a cursor from.</param>
        /// <returns>A cursor with this value.</returns>
        /// <remarks>
        /// WPF internally has a function to load a Cursor from a CursorType, but the constructor for Cursor to take a CursorType isn't exposed.
        /// This method uses the <see cref="Cursors"/> helper class and a <c>switch</c> block to achieve the same goal in a more roundabout way.
        /// </remarks>
        public static Cursor ToCursor(this CursorType cursorType)
        {
            switch (cursorType)
            {
                case CursorType.None:
                    return Cursors.None;
                case CursorType.No:
                    return Cursors.No;
                case CursorType.Arrow:
                    return Cursors.Arrow;
                case CursorType.AppStarting:
                    return Cursors.AppStarting;
                case CursorType.Cross:
                    return Cursors.Cross;
                case CursorType.Help:
                    return Cursors.Help;
                case CursorType.IBeam:
                    return Cursors.IBeam;
                case CursorType.SizeAll:
                    return Cursors.SizeAll;
                case CursorType.SizeNESW:
                    return Cursors.SizeNESW;
                case CursorType.SizeNS:
                    return Cursors.SizeNS;
                case CursorType.SizeNWSE:
                    return Cursors.SizeNWSE;
                case CursorType.SizeWE:
                    return Cursors.SizeWE;
                case CursorType.UpArrow:
                    return Cursors.UpArrow;
                case CursorType.Wait:
                    return Cursors.Wait;
                case CursorType.Hand:
                    return Cursors.Hand;
                case CursorType.Pen:
                    return Cursors.Pen;
                case CursorType.ScrollNS:
                    return Cursors.ScrollNS;
                case CursorType.ScrollWE:
                    return Cursors.ScrollWE;
                case CursorType.ScrollAll:
                    return Cursors.ScrollAll;
                case CursorType.ScrollN:
                    return Cursors.ScrollN;
                case CursorType.ScrollS:
                    return Cursors.ScrollS;
                case CursorType.ScrollW:
                    return Cursors.ScrollW;
                case CursorType.ScrollE:
                    return Cursors.ScrollE;
                case CursorType.ScrollNW:
                    return Cursors.ScrollNW;
                case CursorType.ScrollNE:
                    return Cursors.ScrollNE;
                case CursorType.ScrollSW:
                    return Cursors.ScrollSW;
                case CursorType.ScrollSE:
                    return Cursors.ScrollSE;
                case CursorType.ArrowCD:
                    return Cursors.ArrowCD;
                default:
                    // just return the base cursor
                    return Cursors.Arrow;
            }
        }

    }
}
