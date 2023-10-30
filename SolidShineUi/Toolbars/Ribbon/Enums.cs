using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi.Toolbars.Ribbon
{
    /// <summary>
    /// The display options for a <see cref="RibbonGroup"/>.
    /// </summary>
    public enum GroupSizeMode
    {
        /// <summary>
        /// Display the commands in this group at the largest size that they can fit.
        /// </summary>
        Standard = 0,

        /// <summary>
        /// Compact down the commands in this group as small as they can.
        /// </summary>
        Compact = 1,

        /// <summary>
        /// Display the group only as an icon/menu, which will display the commands in a pop-up when clicked.
        /// </summary>
        IconOnly = 2,
    }

    /// <summary>
    /// The size of a Ribbon element within its group.
    /// </summary>
    public enum RibbonElementSize
    {
        /// <summary>
        /// Display the control in its content form (such as displaying a text box or a full <see cref="Gallery"/>). Depending upon the control,
        /// this may be a small height that allows stacking with multiple elements, or a large height that takes up the full size of the Ribbon,
        /// or this value may not do anything at all (such as with <see cref="RibbonButton"/> or <see cref="RibbonCheckBox"/>).
        /// </summary>
        Content = 3,
        /// <summary>
        /// Large size and large icon. This will take up the entire height of the Ribbon's control area.
        /// </summary>
        Large = 1,
        /// <summary>
        /// Small size and small icon. Multiple small elements can be displayed stacked together in the Ribbon's control area.
        /// </summary>
        Small = 2,
        /// <summary>
        /// Small icon and display no text. Multiple small elements can be displayed stacked together in the Ribbon's control area.
        /// </summary>
        IconOnly = 0,
    }

    /// <summary>
    /// Determine how a <see cref="GalleryItem"/> is displayed within a <see cref="Gallery"/>.
    /// </summary>
    public enum GalleryItemLayout
    {
        /// <summary>
        /// Display custom content within the <see cref="GalleryItem"/> at full height, with no text underneath.
        /// An example in Microsoft Office is the Quick Styles gallery on the Shape Format tab.
        /// </summary>
        LargeContentOnly = 0,
        /// <summary>
        /// Display custom content within the <see cref="GalleryItem"/> at a small height, allowing multiple controls to be stacked together.
        /// An example in Microsoft Office is the Cell Styles gallery on the Home tab in Microsoft Excel.
        /// </summary>
        SmallContentOnly = 1,
        /// <summary>
        /// Display a large icon within the <see cref="GalleryItem"/> at full height, with no text underneath.
        /// An example in Microsoft Office is the various Chart options on the Insert tab in Microsoft Excel.
        /// </summary>
        LargeIconOnly = 2,
        /// <summary>
        /// Display a small icon within the <see cref="GalleryItem"/> at a small height, allowing multiple controls to be stacked together.
        /// This option is best meant for when there is a large number of items to display within a <see cref="Gallery"/>, and for which seeing the titles/labels of these items
        /// isn't as important. An example in Microsoft Office is the Shapes gallery on the Insert tab.
        /// </summary>
        SmallIconOnly = 3,
        /// <summary>
        /// Display a large icon within the <see cref="GalleryItem"/> at full height, with the item's title underneath.
        /// An example in Microsoft Office is the Data Types gallery on the Data tab in Microsoft Excel.
        /// </summary>
        LargeIconAndText = 4,
        /// <summary>
        /// Display a small icon within the <see cref="GalleryItem"/> at a small height, allowing multiple controls to be stacked together.
        /// The title is displayed to the right of the icon. An example in Microsoft Office is the Borders menu on the Home tab.
        /// </summary>
        SmallIconAndText = 5,
        /// <summary>
        /// Display custom content within the <see cref="GalleryItem"/> at full height, with the item's title underneath.
        /// This option is best meant for when you want to display custom content, which users may also associate with the title used with it.
        /// An example in Microsoft Office is the Text Box gallery on the Insert tab in Microsoft Word.
        /// </summary>
        LargeContentAndText = 6,
        /// <summary>
        /// Display custom content within the <see cref="GalleryItem"/> at a small height, allowing multiple controls to be stacked together.
        /// The title is displayed to the right of the content.
        /// An example in Microsoft Office is the Colors gallery on the Design tab in Microsoft Word.
        /// </summary>
        SmallContentAndText = 7,
        /// <summary>
        /// Display only the title within the <see cref="GalleryItem"/> at a small height, allowing multiple controls to be stacked together.
        /// </summary>
        TextOnly = 8,
        /// <summary>
        /// Display a small icon, custom content, and the title within the <see cref="GalleryItem"/> at a small height, allowing multiple controls to be stacked together.
        /// The icon is displayed on the furthest left, followed by the custom content, and the title to the right of both.
        /// </summary>
        SmallIconAndContent = 9
    }
}
