SolidShineUi UI Library
=======================

This is a library containing controls and windows for WPF .NET programs (many new, some a replacement of built-in controls). This library also contains helper classes and functions to customize the UI and provide other various bits of additional functionality. I use this for all of my modern software projects, and hopefully it will be of use to you as well!

This library is created and maintained by JaykeBird (Jayke R. Huempfner). While a majority of the code and functionality was built by me, a portion of it was originally created by others. Please see the credits on my website for more details (linked below).

The entire library is available under the MIT License.

View more details about this library at [my website, jaykebird.com](https://jaykebird.com/software/ssui).

## Use it now
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FJaykeBird%2Fssui.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2FJaykeBird%2Fssui?ref=badge_shield)


The library is available [on NuGet](https://www.nuget.org/packages/SolidShineUi/), or you can download the binaries [directly from here on GitHub](https://github.com/JaykeBird/ssui/releases/latest).

### Getting set up

After you get the library from a location listed above, you can add a reference to SolidShineUi in your C# and XAML files.

```XAML
    xmlns:flat="clr-namespace:SolidShineUi;assembly=SolidShineUi"
```

```csharp
using SolidShineUi;
```

After that, you can start using the controls listed below!

You can use just the one or two controls you need, or you can rebuild existing UIs or design all-new UIs with the Solid Shine UI controls. You can use these alongside the standard WPF controls with no issues or changes in their behavior.

Each window and control in Solid Shine UI includes a `ColorScheme` property, but if you don't want to use that, you can modify each control's appearance using the various brush properties (like any other WPF control). The `ColorScheme` property is useful if you're using multiple controls or windows from my library, and you want them all to share a similar appearance. For more details, continue to the next section.

You can use the SsuiSample app included here as an example to get started with or to see the capabilities of the library.

Documentation is available on [the wiki here on GitHub](https://github.com/JaykeBird/ssui/wiki).

### Adding/using a ColorScheme

If you're building your application's UI with many of my controls/windows (or almost entirely from these controls/windows), it's recommended that you set up a `ColorScheme` that all the windows and controls can access. Each window and control class in Solid Shine UI includes a ColorScheme property, which you can use with WPF's binding features.

There could be a number of ways to set this up, but here's the way that I personally use and I recommend to others. You start with placing a static ColorScheme class in your App.xaml.cs file:

```csharp
    public static ColorScheme ColorScheme { get; set; } = new ColorScheme(Colors.Green);
```

You can create a ColorScheme based upon any base color you want to use (such as your app's branding color), or you can use `ColorScheme.CreateLightTheme()` or `ColorScheme.CreateDarkTheme()` for more standard light or dark themes. High-contrast color schemes are also built-in. You can also start from scratch and set each value individually if you'd like, too.

From there, you can use a `FlatWindow` (drop-in replacement of the standard WPF window), which contains a `ColorScheme` property that all the child Solid Shine UI controls can bind to. Set up the UI and binding in the XAML side:

```XAML
<flat:FlatWindow x:Class="MyApp.MyWindow" x:Name="window"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:flat="clr-namespace:SolidShineUi;assembly=SolidShineUi"
        mc:Ignorable="d"
        Title="My Window" >
   <!-- Whatever your UI is... For example: -->
   <flat:IntegerSpinner ColorScheme="{Binding ColorScheme, ElementName=window}" MinValue="0" />
   <!-- for all SolidShineUi controls, you can bind the ColorScheme to the FlatWindow's ColorScheme property if you're using a FlatWindow -->
</flat:FlatWindow>
```

And in the C# side, set the window's `ColorScheme` property, and it'll set it for all the binded controls too.

```csharp
using System;
using System.Windows;
using SolidShineUi;

namespace MyApp
{

    public partial class MyWindow : FlatWindow
    {
    
        public MyWindow()
        {
            InitializeComponent();
            
            ColorScheme = App.ColorScheme; // <--- this sets up the color scheme for the window
        }
        
        // the rest of your code-behind as normal   
    }
}
```

If you want to keep using WPF's standard windows, that's fine too! You'll probably want to create your own ColorScheme property for the window then, which controls can bind to. (Of course, you can also just directly set or change each control's ColorScheme in C# as well.)

Here's how to create your own ColorScheme property that you can add to a standard WPF window (then, you can bind to it just like in the XAML code above):

```csharp
        // in your window's .xaml.cs (code-behind) file

        /// <summary>
        /// A dependency property object backing the related ColorScheme property. See <see cref="ColorScheme"/> for more details.
        /// </summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(MyWindowClassName),
            new FrameworkPropertyMetadata(new ColorScheme()));

        /// <summary>
        /// Get or set the color scheme used for this control/window. This can be used for binding with the combined <see cref="ColorSchemeProperty"/>.
        /// </summary>
        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }
```

Some controls (like the `FlatButton`, `MenuButton`, and `Menu`) will also utilize an accent color, if you set that in the `ColorScheme`. Use this to highlight or bring focus to particular buttons. Be sure to set `UseAccentColors` on the buttons you want to use the accent color on, and `MenusUseAccent` in the `ColorScheme` if you want it with any `Menu` controls in your UI.

From here, you should be on your way!

### Keyboard shortcuts

One part of Solid Shine UI is a keyboard shortcut handling system that I built.

While WPF has its own method of handling keyboard shortcuts (via InputBindings), my system is a bit more flexible and customizable, and these customizations can also be saved to/loaded from a XML file. However, my system is not an easy drop-in replacement, and has the drawback that it mostly needs to be set up in C# (with or without a XML file), rather than being doable in XAML.

To get started with my keyboard shortcut handling system, I recommend looking at the [Adding Keyboard Support](https://github.com/JaykeBird/ssui/wiki/KeyboardSupport) file for a step-by-step process and other notes and remarks.

## Included

### Windows

- **FlatWindow** - a basic WPF window with a flat appearance and additional functionality
- **ColorPickerDialog** - a WPF dialog for selecting colors, either from swatches, using sliders, or from an image or color palette file
- **FontSelectDialog** - a WPF dialog that allows the user to select a font, as well as other properties like size and style
- **MessageDialog** - a MessageBox dialog in the FlatWindow/SSUI style, with additional functions and buttons (similar to the Windows TaskDialog)
- **StringInputDialog** - a WPF dialog to allow users to input a string, similar to the input dialog JOptionPane from Java's Swing library

### Controls

- **ClickSelectTextBox** - a TextBox, that automatically selects all the text within when you click on it
- **CheckBox** - a box that can be checked; it is larger than the standard WPF CheckBox and offers more customization
- **ContextMenu** - a context menu with a flat style to match the rest of the library
- **EnumComboBox** - a ComboBox pre-filled with the values of an enumerator
- **ExperimentalPropertyList** - view and edit the properties of any WPF control or C# object (despite the name, it is now stable, it will be renamed in 2.0)
- **FileSelect** - select one or more files, via the Browse button or drag-and-drop
- **FlatButton** - a flat-styled button with a lot of customization options (including toggle-button functions)
- **DoubleSpinner** - also known as a NumericUpDown; select a number by typing it in, clicking up or down, or entering in a math expression
- **IntegerSpinner** - the same as the DoubleSpinner, but only allows integer values (and can display hex numbers)
- **LinkTextBlock** - a TextBlock that acts as a hyperlink
- **LongSpinner** - the same as the IntegerSpinner, but stores numbers as a `long` (for larger values)
- **Menu** - a menu bar and menu with a flat, colored style
- **MenuButton** - a flat-styled button that opens a menu when you click on it
- **SelectPanel** - a powerful panel to easily manage a list and select items from it
- **TabControl** - a flat-styled tab control, where each tab has a title, icon, and close button

### Other Classes

- **ArithmeticParser** - parse math expressions (`"(5+4)/2"`) quickly and easily. [Available separately](https://github.com/JaykeBird/ArithmeticParser) as well
- **AutoCompleteBehavior** - adds basic auto-complete behavior to text boxes. [Available separately](https://github.com/Nimgoble/WPFTextBoxAutoComplete) as well
- **BindableChild** - allows you to use WPF binding for a control's Child property (such as the WPF Border control)
- **BrushFactory** - easy and convenient way to create various brushes
- **ColorPaletteFileReader** - load a list of colors from various color palette file formats
- **ColorScheme** - a palette/scheme of colors to use throughout your UI. Generate a scheme from a single base color, or customize it piece-by-piece
- **ColorsHelper** - contains various methods for interacting with colors and color spaces
- **ColorToBrushConverter** - a WPF/XAML converter between a Color and a SolidColorBrush
- **ImageColorPicker** - select colors from an image, used in the ColorPickerDialog
- **IKeyAction** - represents an action that can occur when a keyboard shortcut is pressed (such as CommandKeyAction and RoutedEventKeyAction)
- **KeyboardShortcut** - represents a single keyboard shortcut (and the action to take when it is pressed)
- **KeyboardShortcutHandler** - a helper class that can be added to any WPF window to add keyboard shortcut support
- **KeyboardShortcutsIo** - a helper class to load/save keyboard shortcut settings to/from a file
- **KeyRegistry** - represents a list of keyboard shortcuts currently set, and the actions to take when a shortcut is pressed
- **SelectableCollection** - an extension on ObservableCollection, with additional functions/properties for selecting items in the collection
- **SelectableItem** - a type of SelectableUserControl; a basic but powerful control to use to display items in SelectPanel
- **SelectableUserControl** - the base class from which you can create your own controls to use with a SelectPanel
- **TabItem** - a tab to use with the TabControl
- **WildcardMatch** - match file names/paths (or any text) using wildcard characters like `*` and `?`. [Available separately](https://bitbucket.org/hasullivan/fast-wildcard-matching/) as well

## Coming Soon

Version 2.0 will be coming later this year! Don't be surprised if don't see much activity on the Main branch for some periods of time, as I'll be using sub-branches to develop various features, including an upcoming **Ribbon** control!

View the [roadmap](Docs/ROADMAP.md) for more details on my plans.

## Building/Testing

To build this library you will need:

- .NET Framework developer tools (4.7.1 and 4.8)
- latest .NET Core SDK
- (In the future, I will be adding AvaloniaUI support, so this will become a requirement as well.)

If in Visual Studio 2019 or 2022, you can select the ".NET desktop development" workload, and then add on the individual components ".NET Framework 4.7.1 targeting pack" and ".NET Framework 4.8 targeting pack".

I recommend using Visual Studio 2022 (or the latest version) for this library. When opened up, you should be able to just build and run the library without any further action needed.

Included is the SsuiSample program, which is useful for demonstrating the library and its functions. I also use this to test the library and its controls. Feel free to use SsuiSample as a basis for your implementations.

### Notes

If you receive this message (`SourceRoot items must include at least one top-level (not nested) item when DeterministicSourcePaths is true`) when trying to debug, make sure you have Visual Studio set to the Debug configuration, and not the Release configuration (this is the drop-down to the left of the Start button, or you can access it via `Build > Configuration Manager...` and set the active configuration there).

## Credits/Usage

This library is released under the [MIT License](License.md).

For credits/attribution for when you use the library, please use one of the versions below (the first one is preferred):

> Developed/compiled by Jayke R. Huempfner (JaykeBird)
> 
> https://jaykebird.com/software/ssui
> 
> Contains work by Manuel Schweigert, Pete Laker, Steve Lautenschlager, Richard Moss (Cyotek Ltd.), Warren Galyen (Mechanika Design), Yassine (Stopbyte), Hector Pulido, Sebastien Lambla, Nimgoble, and H.A. Sullivan. See website for more details.
>
> Licensed under MIT License


> Developed by Jayke R. Huempfner (JaykeBird), other contributors
> 
> https://jaykebird.com/software/ssui
> 
> Licensed under MIT License. See website for more details.

The full credits is available on [the Credits page on my website](https://jaykebird.com/software/ssui/credits).


## License
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FJaykeBird%2Fssui.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2FJaykeBird%2Fssui?ref=badge_large)