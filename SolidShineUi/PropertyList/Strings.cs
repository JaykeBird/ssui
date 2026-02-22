using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi.PropertyList
{
    /// <summary>
    /// A collection of string resources used in the built-in property editor controls for <see cref="PropertyList"/>.
    /// <para/>
    /// The editor controls load these strings when they are created. They are provided publicly here to be changed for localization purposes.
    /// Note that changes will only apply to editors created after the change is made; existing editor controls will not be updated
    /// (if you have a property list already loaded, you can call <see cref="PropertyList.ReloadObject()"/> to recreate all of its editors).
    /// </summary>
    public static class Strings
    {
        /// <summary>A string with the value <c>"View"</c>.</summary>
        public static string View { get; set; } = "View";

        /// <summary>A string with the value <c>"View / Edit"</c>.</summary>
        public static string ViewEdit { get; set; } = "View / Edit";

        /// <summary>A string with the value <c>"(null)"</c>.</summary>
        public static string Null { get; set; } = "(null)";

        /// <summary>A string with the value <c>"False"</c>.</summary>
        public static string False { get; set; } = "False";

        /// <summary>A string with the value <c>"True"</c>.</summary>
        public static string True { get; set; } = "True";

        /// <summary>A string with the value <c>"Unknown"</c>.</summary>
        public static string Unknown { get; set; } = "Unknown";

        /// <summary>A string with the value <c>"(unknown)"</c>.</summary>
        public static string Unknown2 { get; set; } = "(unknown)";

        /// <summary>A string with the value <c>"(value unset)"</c>.</summary>
        public static string ValueUnset { get; set; } = "(value unset)";

        /// <summary>A string with the value <c>"(null brush)"</c>.</summary>
        public static string NullBrush { get; set; } = "(null brush)";

        /// <summary>A string with the value <c>"Current value:"</c>.</summary>
        public static string CurrentValue { get; set; } = "Current value:";

        /// <summary>A string with the value <c>"Edit..."</c>.</summary>
        public static string Edit { get; set; } = "Edit...";

        /// <summary>A string with the value <c>"Edit"</c>.</summary>
        public static string Edit2 { get; set; } = "Edit";

        /// <summary>A string with the value <c>"Edit Transform..."</c>.</summary>
        public static string EditTransform { get; set; } = "Edit Transform...";

        /// <summary>A string with the value <c>"Edit Relative Transform..."</c>.</summary>
        public static string EditRelativeTransform { get; set; } = "Edit Relative Transform...";

        /// <summary>A string with the value <c>"Edit Color..."</c>.</summary>
        public static string EditColor { get; set; } = "Edit Color...";

        /// <summary>A string with the value <c>"Edit Gradient..."</c>.</summary>
        public static string EditGradient { get; set; } = "Edit Gradient...";

        /// <summary>A string with the value <c>"Edit Brush..."</c>.</summary>
        public static string EditBrush { get; set; } = "Edit Brush...";

        /// <summary>A string with the value <c>"Change to:"</c>.</summary>
        public static string ChangeTo { get; set; } = "Change to:";

        /// <summary>A string with the value <c>"Nothing (null)"</c>.</summary>
        public static string NothingNull { get; set; } = "Nothing (null)";

        /// <summary>A string with the value <c>"Solid Color Brush"</c>.</summary>
        public static string SolidColorBrush { get; set; } = "Solid Color Brush";

        /// <summary>A string with the value <c>"Linear Gradient Brush"</c>.</summary>
        public static string LinearGradientBrush { get; set; } = "Linear Gradient Brush";

        /// <summary>A string with the value <c>"Radial Gradient Brush"</c>.</summary>
        public static string RadialGradientBrush { get; set; } = "Radial Gradient Brush";

        /// <summary>A string with the value <c>"Bitmap Cache Brush"</c>.</summary>
        public static string BitmapCacheBrush { get; set; } = "Bitmap Cache Brush";

        /// <summary>A string with the value <c>"Drawing Brush"</c>.</summary>
        public static string DrawingBrush { get; set; } = "Drawing Brush";

        /// <summary>A string with the value <c>"Image Brush"</c>.</summary>
        public static string ImageBrush { get; set; } = "Image Brush";

        /// <summary>A string with the value <c>"(null)"</c>.</summary>
        public static string BrushTypeNull {get; set;} = "(null)";

        /// <summary>A string with the value <c>"Unknown"</c>.</summary>
        public static string BrushTypeUnknown {get; set;} = "Unknown";

        /// <summary>A string with the value <c>"Solid Color"</c>.</summary>
        public static string BrushTypeSolidColor {get; set;} = "Solid Color";

        /// <summary>A string with the value <c>"Gradient"</c>.</summary>
        public static string BrushTypeGradient {get; set;} = "Gradient";

        /// <summary>A string with the value <c>"Image"</c>.</summary>
        public static string BrushTypeImage {get; set;} = "Image";

        /// <summary>A string with the value <c>"Bitmap Cache"</c>.</summary>
        public static string BrushTypeBitmapCache {get; set;} = "Bitmap Cache";

        /// <summary>A string with the value <c>"Drawing"</c>.</summary>
        public static string BrushTypeDrawing { get; set; } = "Drawing";

        /// <summary>A string with the value <c>"Angle"</c>.</summary>
        public static string Angle { get; set; } = "Angle";

        /// <summary>A string with the value <c>"Radial"</c>.</summary>
        public static string Radial { get; set; } = "Radial";

        /// <summary>A string with the value <c>"(image from stream)"</c>.</summary>
        public static string ImageFromStream { get; set; } = "(image from stream)";

        /// <summary>A string with the value <c>"(image from bitmap source)"</c>.</summary>
        public static string ImageFromBitmapSource { get; set; } = "(image from bitmap source)";

        /// <summary>A string with the value <c>"(image from drawing)"</c>.</summary>
        public static string ImageFromDrawing { get; set; } = "(image from drawing)";

        /// <summary>A string with the value <c>"(image from unknown source)"</c>.</summary>
        public static string ImageFromUnknown { get; set; } = "(image from unknown source)";

        /// <summary>A string with the value <c>"(surrogate)"</c>.</summary>
        public static string Surrogate { get; set; } = "(surrogate)";

        /// <summary>A string with the value <c>"(not a char)"</c>.</summary>
        public static string NotAChar { get; set; } = "(not a char)";

        /// <summary>A string with the value <c>"Set as Null"</c>.</summary>
        public static string SetAsNull { get; set; } = "Set as Null";

        /// <summary>A string with the value <c>"Set as NaN"</c>.</summary>
        public static string SetAsNaN { get; set; } = "Set as NaN";

        /// <summary>A string with the value <c>"Enter in Unicode Value..."</c>.</summary>
        public static string EnterInUnicodeValue { get; set; } = "Enter in Unicode Value...";

        /// <summary>A string with the value <c>"Select Color..."</c>.</summary>
        public static string SelectColor { get; set; } = "Select Color...";

        /// <summary>A string with the value <c>"Set All To Zero (0)"</c>.</summary>
        public static string SetAllToZero { get; set; } = "Set All to Zero (0)";

        /// <summary>A string with the value <c>"Set All To One (1)"</c>.</summary>
        public static string SetAllToOne { get; set; } = "Set All to One (1)";

        /// <summary>A string with the value <c>"Set All To Five (5)"</c>.</summary>
        public static string SetAllToFive { get; set; } = "Set All to Five (5)";

        /// <summary>A string with the value <c>"Set to Empty"</c>.</summary>
        public static string SetToEmpty { get; set; } = "Set to Empty";

        /// <summary>A string with the value <c>"Set to 1,1 Rect"</c>.</summary>
        public static string SetTo11Rect { get; set; } = "Set to 1,1 Rect";

        /// <summary>A string with the value <c>"Top Left"</c>.</summary>
        public static string TopLeft { get; set; } = "Top Left";

        /// <summary>A string with the value <c>"Top Right"</c>.</summary>
        public static string TopRight { get; set; } = "Top Right";

        /// <summary>A string with the value <c>"Bottom Left"</c>.</summary>
        public static string BottomLeft { get; set; } = "Bottom Left";

        /// <summary>A string with the value <c>"Bottom Right"</c>.</summary>
        public static string BottomRight { get; set; } = "Bottom Right";

        /// <summary>A string with the value <c>"Select System Cursor"</c>.</summary>
        public static string SelectSystemCursor { get; set; } = "Select System Cursor";

        /// <summary>A string with the value <c>"Load Cursor from File"</c>.</summary>
        public static string LoadCursorFromFile { get; set; } = "Load Cursor from File";

        /// <summary>A string with the value <c>"From File:"</c>.</summary>
        public static string FromFile { get; set; } = "From File:";

        /// <summary>A string with the value <c>"(no files selected)"</c>.</summary>
        public static string NoFilesSelected { get; set; } = "(no files selected)";

        /// <summary>A string with the value <c>"Browse..."</c>.</summary>
        public static string Browse { get; set; } = "Browse...";

        /// <summary>A string with the value <c>"Multiline Editor..."</c>.</summary>
        public static string MultilineEditor { get; set; } = "Multiline Editor...";

        /// <summary>A string with the value <c>"Display as 12-Hour clock"</c>.</summary>
        public static string DisplayAs12HourClock { get; set; } = "Display as 12-Hour clock";

        /// <summary>A string with the value <c>"Set to Noon"</c>.</summary>
        public static string SetToNoon { get; set; } = "Set to Noon";

        /// <summary>A string with the value <c>"Set to Midnight"</c>.</summary>
        public static string SetToMidnight { get; set; } = "Set to Midnight";

        /// <summary>A string with the value <c>"Set to Current Time"</c>.</summary>
        public static string SetToCurrentTime { get; set; } = "Set to Current Time";

        /// <summary>A string with the value <c>"Set to Current Time (no milliseconds)"</c>.</summary>
        public static string SetToCurrentTimeNoMilliseconds { get; set; } = "Set to Current Time (no milliseconds)";

        /// <summary>A string with the value <c>"Copy Full Value"</c>.</summary>
        public static string CopyFullValue { get; set; } = "Copy Full Value";

        /// <summary>A string with the value <c>"Set Exact Value..."</c>.</summary>
        public static string SetExactValue { get; set; } = "Set Exact Value...";

        /// <summary>A string with the value <c>"No value is set for this property (it is null)."</c>.</summary>
        public static string NoValueIsSet { get; set; } = "No value is set for this property (it is null).";

        /// <summary>A string with the value <c>"collection"</c>.</summary>
        public static string Collection { get; set; } = "collection";

        /// <summary>A string with the value <c>"items"</c>.</summary>
        public static string Items { get; set; } = "items";

        /// <summary>A string with the value <c>"View Items..."</c>.</summary>
        public static string ViewItems { get; set; } = "View Items...";

        /// <summary>A string with the value <c>"View / Edit List..."</c>.</summary>
        public static string ViewEditList { get; set; } = "View / Edit List...";

        /// <summary>A string with the value <c>"Set to New Empty List..."</c>.</summary>
        public static string SetToNewEmptyList { get; set; } = "Set to New Empty List...";

        /// <summary>A string with the value <c>"Are you sure you want to replace the current list with a new empty list?"</c>.</summary>
        public static string AreYouSureNewEmptyList { get; set; } = "Are you sure you want to replace the current list with a new empty list?";

        /// <summary>A string with the value <c>"Confirm Empty List"</c>.</summary>
        public static string ConfirmEmptyList { get; set; } = "Confirm Empty List";

        /// <summary>A string with the value <c>"Enter a string value to use for creating a new item:"</c>.</summary>
        public static string EnterAStringValue { get; set; } = "Enter a string value to use for creating a new item:";

        /// <summary>A string with the value <c>"(no font selected)"</c>.</summary>
        public static string NoFontSelected { get; set; } = "(no font selected)";

        /// <summary>A string with the value <c>"Normal"</c>.</summary>
        public static string Normal { get; set; } = "Normal";

        /// <summary>A string with the value <c>"Italic"</c>.</summary>
        public static string Italic { get; set; } = "Italic";

        /// <summary>A string with the value <c>"Oblique"</c>.</summary>
        public static string Oblique { get; set; } = "Oblique";

        //<ComboBoxItem Content="Thin" /> <!-- 100 -->
        //<ComboBoxItem Content="ExtraLight" />  <!-- 200 -->
        //<ComboBoxItem Content="Light" /> <!-- 300 -->
        //<ComboBoxItem Content="Normal" /> <!-- 400 -->
        //<ComboBoxItem Content="Medium" /> <!-- 500 -->
        //<ComboBoxItem Content="SemiBold" /> <!-- 600 -->
        //<ComboBoxItem Content="Bold" /> <!-- 700 -->
        //<ComboBoxItem Content="ExtraBold" /> <!-- 800 -->
        //<ComboBoxItem Content="Black" /> <!-- 900 -->
        //<ComboBoxItem Content="ExtraBlack" /> <!-- 950 -->

        /// <summary>A string with the value <c>"Thin"</c>.</summary>
        public static string Thin { get; set; } = "Thin";
        /// <summary>A string with the value <c>"ExtraLight"</c>.</summary>
        public static string ExtraLight { get; set; } = "ExtraLight";
        /// <summary>A string with the value <c>"Light"</c>.</summary>
        public static string Light { get; set; } = "Light";
        /// <summary>A string with the value <c>"Medium"</c>.</summary>
        public static string Medium { get; set; } = "Medium";
        /// <summary>A string with the value <c>"SemiBold"</c>.</summary>
        public static string SemiBold { get; set; } = "SemiBold";
        /// <summary>A string with the value <c>"Bold"</c>.</summary>
        public static string Bold { get; set; } = "Bold";
        /// <summary>A string with the value <c>"ExtraBold"</c>.</summary>
        public static string ExtraBold { get; set; } = "ExtraBold";
        /// <summary>A string with the value <c>"Black"</c>.</summary>
        public static string Black { get; set; } = "Black";
        /// <summary>A string with the value <c>"ExtraBlack"</c>.</summary>
        public static string ExtraBlack { get; set; } = "ExtraBlack";

        /// <summary>A string with the value <c>"Select from List"</c>.</summary>
        public static string SelectFromList { get; set; } = "Select from List";

        /// <summary>A string with the value <c>"Enter Weight Class"</c>.</summary>
        public static string EnterWeightClass { get; set; } = "Enter Weight Class";

        /// <summary>A string with the value <c>"Generate New Guid"</c>.</summary>
        public static string GenerateNewGuid { get; set; } = "Generate New Guid";

        /// <summary>A string with the value <c>"Set Guid..."</c>.</summary>
        public static string SetGuid { get; set; } = "Set Guid...";

        /// <summary>A string with the value <c>"Enter Guid"</c>.</summary>
        public static string EnterGuid { get; set; } = "Enter Guid";

        /// <summary>A string with the value <c>"Enter in a valid Guid:"</c>.</summary>
        public static string EnterInAValidGuid { get; set; } = "Enter in a valid Guid:";

        /// <summary>A string with the value <c>"Not a valid Guid"</c>.</summary>
        public static string NotAValidGuid { get; set; } = "Not a valid Guid";

        /// <summary>A string with the value <c>"Display as Hex"</c>.</summary>
        public static string DisplayAsHex { get; set; } = "Display as Hex";

        /// <summary>A string with the value <c>"Decrease and Edit"</c>.</summary>
        public static string DecreaseAndEdit { get; set; } = "Decrease and Edit";

        /// <summary>A string with the value <c>"Left"</c>.</summary>
        public static string Left { get; set; } = "Left";

        /// <summary>A string with the value <c>"Top"</c>.</summary>
        public static string Top { get; set; } = "Top";

        /// <summary>A string with the value <c>"Right"</c>.</summary>
        public static string Right { get; set; } = "Right";

        /// <summary>A string with the value <c>"Bottom"</c>.</summary>
        public static string Bottom { get; set; } = "Bottom";

        /// <summary>A string with the value <c>"Increase All by One (+1)"</c>.</summary>
        public static string IncreaseAllByOne { get; set; } = "Increase All by One (+1)";

        /// <summary>A string with the value <c>"Decrease All by One (-1)"</c>.</summary>
        public static string DecreaseAllByOne { get; set; } = "Decrease All by One (-1)";

        /// <summary>A string with the value <c>"Reset"</c>.</summary>
        public static string Reset { get; set; } = "Reset";

        /// <summary>A string with the value <c>"group"</c>.</summary>
        public static string Group { get; set; } = "group";
        /// <summary>A string with the value <c>"rotate"</c>.</summary>
        public static string Rotate { get; set; } = "rotate";
        /// <summary>A string with the value <c>"skew"</c>.</summary>
        public static string Skew { get; set; } = "skew";
        /// <summary>A string with the value <c>"scale"</c>.</summary>
        public static string Scale { get; set; } = "scale";
        /// <summary>A string with the value <c>"translate"</c>.</summary>
        public static string Translate { get; set; } = "translate";
        /// <summary>A string with the value <c>"matrix"</c>.</summary>
        public static string Matrix { get; set; } = "matrix";
        /// <summary>A string with the value <c>"default (identity matrix)"</c>.</summary>
        public static string Identity { get; set; } = "default (identity matrix)";

        /// <summary>A string with the value <c>"no transforms"</c>.</summary>
        public static string NoTransforms { get; set; } = "no transforms";

        /// <summary>A string with the value <c>"Select a File"</c>.</summary>
        public static string SelectAFile { get; set; } = "Select a File";

        /// <summary>A string with the value <c>"Open URL"</c>.</summary>
        public static string OpenUrl { get; set; } = "Open URL";

        /// <summary>A string with the value <c>"Enter / Paste Value.."</c>.</summary>
        public static string EnterPasteValue { get; set; } = "Enter / Paste Value..";

        /// <summary>A string with the value <c>"Major"</c>.</summary>
        public static string Major { get; set; } = "Major";

        /// <summary>A string with the value <c>"Minor"</c>.</summary>
        public static string Minor { get; set; } = "Minor";

        /// <summary>A string with the value <c>"Build"</c>.</summary>
        public static string Build { get; set; } = "Build";

        /// <summary>A string with the value <c>"Revision"</c>.</summary>
        public static string Revision { get; set; } = "Revision";

        /// <summary>A string with the value <c>"Parse Version from String"</c>.</summary>
        public static string ParseVersionFromString { get; set; } = "Parse Version from String";

        /// <summary>A string with the value <c>"Enter a Version string (numbers and periods only):"</c>.</summary>
        public static string EnterAVersionString { get; set; } = "Enter a Version string (numbers and periods only):";

        /// <summary>
        /// Retrieves the names of all of the properties in this class.
        /// </summary>
        /// <returns>An array of strings containing the names of each public static string property defined in this class.</returns>
        public static string[] GetListOfStringNames()
        {
            return typeof(Strings).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(string)) // they should all be of type string, but I guess this is good for future proofing this
                .Select(p => p.Name)
                .ToArray();
        }

        /// <summary>
        /// Set the values of the string properties in this class from a dictionary.
        /// <para/>
        /// The keys of the dictionary should be the property names and the values are the string values to set for those properties.
        /// </summary>
        /// <param name="values">the dictionary of strings to use for setting values</param>
        public static void SetStrings(Dictionary<string, string> values)
        {
            // this searches through the keys in the dictionary
            // probably is less resource intensive than iterating through the property names instead and then finding dictionary keys from there
            //
            // I guess the question is since it's a fixed number of properties, is it more efficient to iterate through the properties or iterate through the dictionary keys?
            // probably doing the dictionary in most cases (I'd expect that a dictionary passed in here may be tailor made for this function),
            // but if the dictionary has a looot of keys that aren't relevant to this function, then it may end up being slower
            // I guess it doesn't super matter

            foreach (var kvp in values)
            {
                var prop = typeof(Strings).GetProperty(kvp.Key, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (prop != null && prop.PropertyType == typeof(string))
                {
                    prop.SetValue(null, kvp.Value);
                }
            }
        }

    }
}
