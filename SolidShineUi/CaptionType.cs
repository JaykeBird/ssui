using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi
{
    /// <summary>
    /// Determines which caption buttons will appear on a window.
    /// </summary>
    public enum CaptionType
    {
        /// <summary>
        /// Display the close, maximize/restore, and minimize buttons.
        /// </summary>
        Full,
        /// <summary>
        /// Display only the close button.
        /// </summary>
        Close,
        /// <summary>
        /// Display only the close and minimize buttons.
        /// </summary>
        MinimizeClose,
        /// <summary>
        /// Display only the close and maximize/restore buttons.
        /// </summary>
        MaximizeClose,
        /// <summary>
        /// Do not display any caption buttons.
        /// (Note that hiding caption buttons alone doesn't prevent users from being able to perform their actions via other methods.)
        /// </summary>
        None
    }

    /// <summary>
    /// Additional methods for <see cref="CaptionType"/>.
    /// </summary>
    public static class CaptionTypeExtensions
    {

        /// <summary>
        /// Return a new <see cref="CaptionType"/> object that would not cause a Minimize button to be rendered.
        /// </summary>
        public static CaptionType RemoveMinimizeButton(this CaptionType captionType)
        {
            switch (captionType)
            {
                case CaptionType.Full:
                    return CaptionType.MaximizeClose;
                case CaptionType.MinimizeClose:
                    return CaptionType.Close;
                default:
                    return captionType;
            }
        }

        /// <summary>
        /// Return a new <see cref="CaptionType"/> object that would not cause a Maximize button to be rendered.
        /// </summary>
        public static CaptionType RemoveMaximizeButton(this CaptionType captionType)
        {
            switch (captionType)
            {
                case CaptionType.Full:
                    return CaptionType.MinimizeClose;
                case CaptionType.MaximizeClose:
                    return CaptionType.Close;
                default:
                    return captionType;
            }
        }

        /// <summary>
        /// Return a new <see cref="CaptionType"/> object that would cause a Minimize button to be rendered.
        /// </summary>
        public static CaptionType AddMinimizeButton(this CaptionType captionType)
        {
            switch (captionType)
            {
                case CaptionType.Close:
                    return CaptionType.MinimizeClose;
                case CaptionType.MaximizeClose:
                    return CaptionType.Full;
                case CaptionType.None:
                    return CaptionType.MinimizeClose;
                default:
                    return captionType;
            }
        }

        /// <summary>
        /// Return a new <see cref="CaptionType"/> object that would cause a Maximize button to be rendered.
        /// </summary>
        public static CaptionType AddMaximizeButton(this CaptionType captionType)
        {
            switch (captionType)
            {
                case CaptionType.Close:
                    return CaptionType.MaximizeClose;
                case CaptionType.MinimizeClose:
                    return CaptionType.Full;
                case CaptionType.None:
                    return CaptionType.MaximizeClose;
                default:
                    return captionType;
            }
        }
    }
}
