SolidShineUi UI Library
=======================

This is a library containing controls and windows for WPF .NET programs. This library also contains helper classes and functions to customize the UI and provide additional functionality. I use this for all of my modern software projects, and hopefully it will be of use to you as well! This library is created and maintained by JaykeBird (Jayke R. Huempfner).

While a majority of the code and functionality was built by me, a number of controls or functions were originally created by others. Please see the credits on my website for more details (linked below).

The entire library is available under the MIT License.

View more details about this library at [my website, jaykebird.com](https://jaykebird.com/software/ssui).

## Included

### Windows

- **FlatWindow** - a basic WPF window with a flat appearance and additional functionality
- **ColorPickerDialog** - a WPF dialog for selecting colors, either from swatches, using sliders, or from an image or color palette file
- **MessageDialog** - a MessageBox dialog in the FlatWindow/SSUI style, with additional functions and buttons (similar to the Windows TaskDialog)
- **StringInputDialog** - a WPF dialog to allow users to input a string, similar to the input dialog JOptionPane from Java's Swing library
- **FontSelectDialog** - a WPF dialog that allows the user to select a font, as well as other properties like size and style

### Controls

- **ClickSelectTextBox** - a TextBox, that automatically selects all the text within when you click on it
- **CheckBox** - a box that can be checked; it is larger than the standard WPF CheckBox and offers more customization
- **ContextMenu** - a context menu with a flat style to match the rest of the library
- **EnumComboBox** - a ComboBox pre-filled with the values of an enumerator
- **FileSelect** - select one or more files, via the Browse button or drag-and-drop
- **FlatButton** - a flat-styled button with a lot of customization options (including toggle-button functions)
- **DoubleSpinner** - also known as a NumericUpDown; select a number by typing it in, clicking up or down, or entering in a math expression
- **IntegerSpinner** - the same as the DoubleSpinner, but only allows integer values
- **LinkTextBlock** - a TextBlock that acts as a static hyperlink
- **Menu** - a menu bar and menu with a flat, colored style
- **MenuButton** - a flat-styled button that opens a menu when you click on it
- **SelectPanel** - a powerful panel to easily manage a list and select items from it

### Other Classes

- **ArithmeticParser** - parse math expressions (`"(5+4)/2"`) quickly and easily. Available [separately](https://github.com/JaykeBird/ArithmeticParser) as well
- **AutoCompleteBehavior** - adds basic auto-complete behavior to text boxes. Available [separately](https://github.com/Nimgoble/WPFTextBoxAutoComplete) as well
- **BindableChild** - allows you to use WPF binding for a control's Child property (such as the WPF Border control)
- **BrushFactory** - easy and convenient way to create various brushes
- **ColorPaletteFileReader** - load a list of colors from various color palette file formats
- **ColorScheme** - a palette/scheme of colors to use throughout your UI. Generate a scheme from a single base color, or customize it piece-by-piece
- **ColorsHelper** - contains various methods for interacting with colors and color spaces
- **ColorToBrushConverter** - a WPF/XAML converter between a Color and a SolidColorBrush
- **ImageColorPicker** - select colors from an image, used in the ColorPickerDialog
- **SelectableItem** - a type of SelectableUserControl; a basic but powerful control to use to display items in SelectPanel
- **SelectableUserControl** - the base class from which you can create your own controls to use with a SelectPanel

## Coming Soon

More controls (including **TabControl** and **Toolbar**) are coming soon! View the [roadmap](Docs\Roadmap.md) for more details.

## Building/Testing

To build this library you will need:

- .NET Framework developer tools (4.7.1 and 4.8)
- .NET Core SDK 3.1 and 5.0
- (In the future, I will be adding AvaloniaUI support, so this will become a requirement as well.)

I recommend using Visual Studio 2019 (16.8) for this library. When opened up, you should be able to just build and run the library without any further action needed.

Included is the SsuiSample program, which is useful for demonstrating the library and its functions. I also use this to test the library and its controls. Feel free to use SsuiSample as a basis for your implementations.

## Credits/Usage

This library is released under the [MIT License](License.md).

For credits/attribution for when you use the library, please use one of the versions below (the first one is preferred):

> Developed/compiled by Jayke R. Huempfner (JaykeBird)
> 
> https://jaykebird.com/software/ssui
> 
> Contains work by Manuel Schweigert, Pete Laker, Steve Lautenschlager, Richard Moss (Cyotek Ltd.), Warren Galyen (Mechanika Design), Oleg V. Polikarpotchkin, Yassine (Stopbyte), Hector Pulido, Sebastien Lambla, Nimgoble, and H.A. Sullivan. See website for more details.
>
> Licensed under MIT License


> Developed by Jayke R. Huempfner (JaykeBird), other contributors
> 
> https://jaykebird.com/software/ssui
> 
> Licensed under MIT License. See website for more details.

The full credits is available on [the Credits page on my website](https://jaykebird.com/software/ssui/credits).