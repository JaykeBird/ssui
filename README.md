SolidShineUi UI Library
=======================

This is a library containing controls and windows for WPF .NET programs (many new, some a replacement of built-in controls). This library also 
contains helper classes and functions to customize the UI and provide other various bits of additional functionality. I use this for all of my modern 
software projects, and hopefully it will be of use to you as well!

This library is created and maintained by JaykeBird (Jayke R. Huempfner). While a large majority of the code and functionality was built by me, 
I did reuse some code created by others. Please see the credits on my website for more details (linked below).

The entire library is available under the MIT License.
( [![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FJaykeBird%2Fssui.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2FJaykeBird%2Fssui?ref=badge_shield) )

View more details about this library at [my website, jaykebird.com](https://jaykebird.com/software/ssui).

## Use it now

The library is available [on NuGet](https://www.nuget.org/packages/SolidShineUi/), or you can download the binaries 
[directly from here on GitHub](https://github.com/JaykeBird/ssui/releases/latest).

### Getting set up

After you get the library from a location listed above, you can add a reference to SolidShineUi in your C# and XAML files.

```XAML
    xmlns:flat="clr-namespace:SolidShineUi;assembly=SolidShineUi"
```

```csharp
using SolidShineUi;
```

After that, you can start using the controls listed below!

You can use just the one or two controls you need, or you can rebuild existing UIs or design all-new UIs with the Solid Shine UI controls. 
You can use these alongside the standard WPF controls with no changes in their appearance or behavior.

You can build and run the SsuiSample app included in this repo as an example to get started with or to see the capabilities of the library.

Documentation is available on [the wiki here on GitHub](https://github.com/JaykeBird/ssui/wiki).

### Adding/using SsuiTheme

If you're building your application's UI with many of my controls/windows (or almost entirely from these controls/windows), it's recommended that you set up a
`SsuiTheme` that all the windows can access. Almost all window and control classes in Solid Shine UI includes a SsuiTheme property, which you can use with WPF's binding features.

There could be a number of ways to set this up, but the way I personally use and I recommend to others is to place a static SsuiAppTheme property 
in your App.xaml.cs file:

```csharp
    // the app theme can be based on any color; here, we'll use green
    // there's also pre-made themes available in the SolidShineUi.SsuiThemes class
    public static SsuiAppTheme AppTheme { get; set; } = new SsuiAppTheme(Colors.Green);
```

You can create a SsuiAppTheme based upon any base color you want to use (such as your app's branding color), or you can use 
`SsuiThemes.CreateLightTheme()` or `SsuiThemes.CreateDarkTheme()` for more standard light or dark themes, or even `SsuiThemes.SystemTheme` for a more standard Windows-looking theme.
High-contrast color schemes are also built-in in the `SsuiThemes` class. You can also start from scratch and custom set each value if you'd like, too (including using gradients!).

From there, you can use a `ThemedWindow` (drop-in replacement of the standard WPF window), which contains a `SsuiTheme` property that all the 
child Solid Shine UI controls can bind to. Change the Window to be a ThemedWindow on the XAML side:

```XAML
<flat:ThemedWindow x:Class="MyApp.MyWindow" x:Name="window"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:flat="clr-namespace:SolidShineUi;assembly=SolidShineUi"
        mc:Ignorable="d" Title="My Window" >
   <!-- make sure to include the above xmlns:flat reference -->
   <!-- Whatever your UI is... For example: -->
   <flat:IntegerSpinner MinValue="0" />
   <!-- for all SolidShineUi controls, they'll automatically inherit the SsuiTheme value from the parent ThemedWindow -->
</flat:ThemedWindow>
```

And in the C# side, set the window's `SsuiTheme` property, and it'll set it for all the child SSUI-themed controls too.

```csharp
using System;
using System.Windows;
using SolidShineUi; // <--- don't forget the reference to SolidShineUi

namespace MyApp
{

    public partial class MyWindow : ThemedWindow // <--- make sure to update this to be a ThemedWindow too
    {
    
        public MyWindow()
        {
            InitializeComponent();
            
            SsuiTheme = App.AppTheme; // <--- this sets up the color scheme for the window
        }
        
        // the rest of your code-behind as normal   
    }
}
```

Solid Shine UI also includes another window class called `FlatWindow`, which has all the features of a standard WPF window, but with a custom title bar
and a custom appearance (and with it, a few extra features and options about what to put into the title bar). It is a drop-in replacement if you want to
use `FlatWindow` instead of `ThemedWindow`.

If you're building your own controls, they can also use `SsuiTheme` by having them inherit from `ThemedControl`, `ThemedContentControl`, or `ThemedUserControl`.
Make sure to then override the `OnApplySsuiTheme` method to set your control's appearance based upon the SsuiTheme's values. Use this to get started:

```csharp
    /// <inheritdoc/>
    protected override void OnApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
    {
        base.OnApplySsuiTheme(ssuiTheme, useLightBorder, useAccentTheme);

        if (useAccentTheme && ssuiTheme is SsuiAppTheme ssuiAppTheme)
        {
            ApplyTheme(ssuiAppTheme.AccentTheme);
        }
        else
        {
            ApplyTheme(ssuiTheme);
        }

        void ApplyTheme(SsuiTheme theme)
        {
            // update your appearance and properties in here
                
            // use ApplyThemeBinding() to bind the properties so that changing the value in the SsuiTheme automatically updates this control:
            ApplyThemeBinding(ForegroundProperty, SsuiTheme.ForegroundProperty, theme);
            ApplyThemeBinding(BackgroundProperty, SsuiTheme.PanelBackgroundProperty, theme);

            // note that "theme" can be null; ApplyThemeBinding handles that and also handles the ThemeValueExclude property
        }
    }
```

If you want to customize the appearance of specific controls while still using SsuiTheme, you can either look into using an accent theme (see below) or 
set that control's `ThemeValueExclude` property to make sure that your brushes don't get overwritten by the SsuiTheme:

```xaml
    <flat:FlatButton Content="My Button" x:Name="btn1" Background="Orchid" BorderBrush="Purple" ThemeValueExclude="Background,BorderBrush" />
```

From here, you should be on your way!

### Accent theme

The `SsuiAppTheme` class allows you to set an accent theme that you can use in any SSUI-themed control.

```csharp
    // we'll use Green as the main color, and Yellow as the accent color
    public static SsuiAppTheme AppTheme { get; set; } = new SsuiAppTheme(Colors.Green, Colors.Yellow);
```

From there, you can set `UseAccentTheme` on any control to have them use the accent theme's colors/brushes instead.

```xaml
    <flat:FlatButton Content="My Button 2" x:Name="btn2" UseAccentTheme="true" />
```

There is also a subitem theme that you can set in `SsuiAppTheme`, if you want the `Menu`, `ContextMenu`, and `SelectPanel` controls to use that for its child items instead.
Make sure to also set the `UseSubitemThemeWith...` properties in the SsuiAppTheme to set which types of controls to use the subitem theme with.

### ColorScheme

Versions prior to 2.0 don't have `SsuiTheme`, and instead use `ColorScheme`. The basic idea is the same, but I'd recommend everyone use 2.0 and start using `SsuiTheme` instead.

The old ColorScheme class had names for its properties that didn't have the best descriptions for what they were used for, and was limited to only using solid colors.

### Keyboard shortcuts

One part of Solid Shine UI is a keyboard shortcut handling system that I built.

While WPF has its own method of handling keyboard shortcuts (via InputBindings), my system is a bit more flexible and customizable, and these 
customizations can also be saved to/loaded from a XML file. However, my system is not an easy drop-in replacement, and has the drawback that it mostly 
needs to be set up in C#, rather than being doable in XAML.

To get started with my keyboard shortcut handling system, I recommend looking at the [Adding Keyboard Support](https://github.com/JaykeBird/ssui/wiki/KeyboardSupport) 
file for a step-by-step process and other notes and remarks.

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

Version 2.0 will be coming later this year! Don't be surprised if don't see much activity on the Main branch for some periods of time, as I'll be using 
sub-branches to develop various features, including an upcoming **Ribbon** control!

View the [roadmap](Docs/ROADMAP.md) for more details on my plans.

## Building/Testing

To build this library you will need:

- .NET Framework developer tools/targeting packs (4.6.1 through 4.8.1)
- latest .NET Core SDK
- (for the Avalonia version, I'd also recommend installing the latest Avalonia UI extension for your IDE)

I recommend using Visual Studio 2026 (or the latest version) for this library. When opened up, you should be able to just build and run the library (to build the .NET Framework
versions, you may need to have the individual components for .NET Framework targeting packs installed).

Included is the SsuiSample program, which is useful for demonstrating the library and its functions. I also use this to test the library and its controls. 
Feel free to use SsuiSample as a basis for your implementations.

### Notes

If you receive this message (`SourceRoot items must include at least one top-level (not nested) item when DeterministicSourcePaths is true`) when trying to 
debug, make sure you have Visual Studio set to the Debug configuration, and not the Release configuration (this is the drop-down to the left of the Start 
button, or you can access it via `Build > Configuration Manager...` and set the active configuration there).

## License
This library is released under the [MIT License](License.md).

[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FJaykeBird%2Fssui.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2FJaykeBird%2Fssui?ref=badge_large)

## Credits/Usage

For credits/attribution for when you use the library, please use one of the versions below (the first one is preferred):

> Developed/compiled by Jayke R. Huempfner (JaykeBird)
> 
> https://jaykebird.com/software/ssui
> 
> Contains work by Manuel Schweigert, Pete Laker, Steve Lautenschlager, Richard Moss (Cyotek Ltd.), Warren Galyen (Mechanika Design), Yassine (Stopbyte), 
> Hector Pulido, Sebastien Lambla, Nimgoble, H.A. Sullivan, and ControlzEx (batzen and punker76). See website for more details.
>
> Licensed under MIT License


> Developed by Jayke R. Huempfner (JaykeBird), other contributors
> 
> https://jaykebird.com/software/ssui
> 
> Licensed under MIT License. See website for more details.

The full credits is available on [the Credits page on my website](https://jaykebird.com/software/ssui/credits).

