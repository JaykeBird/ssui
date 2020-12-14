SSUI Design Principles
=====================

Thus document outlines the design principles for SSUI's controls. This should be the guiding document when wanting to create new controls for the project, as well as wanting to make visual modifications to existing controls.

This document combines a previously-internal design document with additional previously-unwritten rules and practices. Thus far, this is my most comprehensive document about how to design and develop for SSUI.

Please reach out to me if you have any questions.

### Terminology / Goals ###

This project is aimed to help developers create consistent UIs with only small amounts of additional effort. The end result should be a UI that looks pleasing for end users. Developers shouldn't need to create or mess with ResourceDictionary objects or create custom control templates.

SSUI also aims to fill some holes that are present in the standard WPF library, adding controls like the IntegerSpinner, FileSelect, and the ColorPickerDialog. These controls should also be consistent with the SSUI theming, but given that they may be used without building the entire UI using SSUI, these controls should also look decent alongside standard WPF controls.

Developers and contributors following this guide / creating controls for SSUI should be knowledgeable about C# and the design principles of WPF - specifically things like dependency properties, routed events, and control templates.

I will define "developers" to be the engineers/designers/developers who consume the SSUI library and use it in their projects.

I will define "users" or "end users" as the people who ultimately use the program/project that the developer is creating.

### General Rules ###

When creating controls (or dialogs), these general rules should be followed:

- Outline controls in a border (this does apply to controls that are designed to be placed within others, such as SelectableUserControl, although border support should still be present)
- Make sure there is decent spacing/padding - at least 3 px between content and border (although the user should be able to override using Padding or other appropriate property)
- Avoid bold/italics text; use spacing for emphasis
- Do not color text for emphasis or to draw attention - you cannot guarantee this color will be discernable (either due to color scheme setting or disability)
- Controls must respond to keyboard, mouse, touch, and stylus. All primary/essential functionality must be accessible via all methods. Secondary functions should be accessible via most/all.

### Dependency properties / routed events ###

Whenever reasonable, dependency properties should be used. This is especially true for properties that affect a Control's appearance.

If you write an auto property such as
`public string Title { get; set; } = "MyControl's Title"`, it is preferable to instead set up a dependency property with Title, and set the dependency property's default value to be "MyControl's Title" as shown below.

```
        public static DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(MyControl),
            new PropertyMetadata("MyControl's Title"));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
```

Note that there may be situation where calling or setting a property may require other private methods to be called or calculations to be made. These may make creating dependency properties from those hard to do. If it's a property that you don't expect developers to need to change during runtime, or a property that developers primarily won't need to set across the board for all instances of a control, you may be able to get away without making it a dependency property.

Another option could be a special procedure that is used to access private methods and variables after a callback for the property being changed is called. This procedure is used extensively in DoubleSpinner and FlatButton, and so you can use them for examples. This adds to the visual complexity of the code, but allows you to still create a property as a dependency property while still being able to call private methods.

It is also recommended to use routed events for situations where a developer may be expecting a routed event. It may be good to compare against similar controls in the WPF library to see which events are routed.

While dependency properties should be used whenever and wherever possible, routed events can be used primarily for where you deem it best for the control. It is certainly recommended to use routed events as much as possible as well, but it isn't so steadfast a design requirement.

Click events must always be routed events. (For ease of developing, this can be quickly accomplished by reusing the "Click Handling" section of code from other controls such as CheckBox.)

### Colors ###

Controls in the SSUI library should never use hardcoded color values. Instead, the coloring of controls should be taken from the ColorScheme that the developer (or user) has selected.

A Control's ColorScheme should be set via a ColorScheme dependency property. You can set the Control's colors based upon the properties given in the ColorScheme object. See the table below for guidelines about which colors to use for what. Note that these are guidelines, and so differences are okay, as long as the resulting control doesn't look too out of place alongside the other SSUI controls.

It is also customary to have an ApplyColorScheme method in the control that is responsible for actually setting the colors. This is not required though, and may be phased out internally in favor of using XAML binding instead.

For a good and easy-to-follow example, please look to see how IntegerSpinner and DoubleSpinner implement the ColorScheme property. A number of other controls, such as FlatButton and StackPanel, have additional properties that make them not good examples to start with for trying to learn this implementation.

ColorScheme Item            | Usage
----------------------------|---------------------------------------------------------------------------------------------------------------------------
Main color                  | Emphasis, make things stand out (often too vibrant to be of much use though)
Secondary color             | Use for background of controls that should "pop out", such as buttons. Also usable for emphasis/status (i.e. IsSelected).
Background color            | Background of window; use for background for controls that should blend in with the window (not common)
Light background color      | Background of controls that should "sink in", such as panels.
Border color                | Use for border of control
Highlight color             | Use for border while control is highlighted (mouseover, focus)
Second highlight color      | Use for background while control is highlighted (mouseover, focus)
Third highlight color       | Use for background while control is being clicked/activated, or for emphasis/status (i.e. IsSelected)
Selection color             | Use for border while control is being clicked/activated (often too dark for much other usage, although it is also used in FlatWindow)
Foreground color            | Use for text/content of control, like any other WPF control
Light disabled color        | Use for the background of controls while disabled
Dark disabled color         | Use for the text/foreground/border of controls while disabled
Window title bar color      | **ONLY** for usage in title bar of window (FlatWindow)
Window inactive color       | **ONLY** for usage in title bar of window, when not focused (inactive) (FlatWindow)
Window title bar text color | **ONLY** for usage for text in title bar of window (FlatWindow)

Also include special handling if the color scheme is a High Contrast theme (ColorScheme.IsHighContrast). If so, do not use the MainColor and SecondaryColor properties; replace them with BackgroundColor or HighlightColor as appropriate.

### Templated control vs UserControl ###

Templated controls (ContentControl with a ControlTemplate) offer a lot more flexibility and direct control over the appearance of the control. However, it also does have its caveats, and can make certain actions harder to accomplish than using a UserControl. Thus, depending upon the control, it may be preferable to use one method or another.

ContentControl                                                    | UserControl
------------------------------------------------------------------|---------------------------------------------------------------------------------------------------
Expect developers to set control's content (via Content property) | Control's contents is either statically set or controlled via specific properties or methods
Developers will want to name child elements                       | Developers don't need direct access to child elements
Allow developers to directly extend/overwrite control appearance  | Control appearance only changeable through set properties/methods
Able to use BorderBrush and BorderThickness properties            | Need to create custom properties to modify Control's borders (i.e. PanelBorderBrush and PanelBorderThickness)
Harder to handle events from child elements in control            | Can directly handle events from child elements

If creating a templated control, it's difficult to actually directly access the child controls in the template from code, such as to add handlers for events or to modify the appearance or behavior of the child controls. As is recommended by Microsoft, child controls should be modified via the XAML ControlTemplate whenever possible, using dependency properties and template binding. If absolutely needed, child controls can be accessed from a ControlTemplate by giving the child control a name, and then accessing that control via the FindName function after InitializeComponent() is called; this should be avoided when possible though. If this is the route you decide to take when building a control, make sure to publicly document these named controls, including their name, their type, and the function they play in the control. This way, developers that wish to supply their own ControlTemplate for your control can also be sure to create these named child controls as well.

If a control heavily depends upon accessing and handling events of child controls, it may be preferred to instead use a UserControl. With a UserControl, we can get direct access and handling of child elements, although it comes at the cost of limiting developers' abilities to directly access any child controls or content they may want to set.

### CornerRadius ###

If and when reasonable, a control should have a CornerRadius dependency property.

This can be easy to implement by wrapping the Control's contents in a Border element, and then binding the Border's CornerRadius property to the Control's CornerRadius property.

Note that there are situations, depending upon the controls in the border itself, where this simple solution doesn't work too well. In such situations, another border with a VisualBrush may need to be used. Please see the implementation that IntegerSpinner or DoubleSpinner use for an example of this. How FlatButton implements CornerRadius is the easy ideal solution to use when possible.

Always set the CornerRadius to 0 by default. I want to expose the CornerRadius properties as an option for users that want to use it, but using rounded corners does not match the default style for SSUI. You should test your control against CornerRadius values all the way up to 8, to make sure it looks acceptable at 0 or 8, and all ranges in between. I do not regularly test above the value of 8.