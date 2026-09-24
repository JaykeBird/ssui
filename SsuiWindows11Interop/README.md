Solid Shine UI WPF Windows 11 Interop
=====================================

This adds support for Windows 11's title bar customization to use with SolidShineUI's FlatWindow,
via way of the Windows App SDK. This will allow you to use Windows 11's caption buttons in place 
of SolidShineUi's custom ones. (This also enables support for the Snap Layouts popup to appear 
when hovering over the Maximize button.)

## Utilizing with FlatWindow

1. Make sure your app's csproj file is targetting .NET 8 or later, and Windows 10 19041 or later 
   (e.g., `<TargetFramework>net8.0-windows10.0.19041.0</TargetFramework>`)
2. Add an event handler for FlatWindow's SourceInitialized event.
3. In that event handler, add in the line `this.UseWindows11CaptionButtons();`

I will also recommend you disable MRT Core in your WPF app if you're not using it. Also keep in
mind how utilizing the Windows App SDK will affect how you package and distribute your app.

The end result should look like this:

In your app's csproj file:

**MyProject.csproj**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0-windows10.0.19041.0</TargetFramework>
    <!-- replace "net8.0" with net9.0 or net10.0 as needed -->

    <EnableCoreMrtTooling>false</EnableCoreMrtTooling>
    <!-- use the above line to disable MRT Core for resource bundling -->

    <!-- remainder of your project's properties and such here -->
  </PropertyGroup>

  <!-- ... further down ... -->

  <ItemGroup>
    <PackageReference Include="SolidShineUi" Version="2.0.0" />
    <!-- or whatever version of SolidShineUi you're using -->
    <PackageReference Include="SolidShineUi.Wpf.Windows11Interop Version="2.0.0" />
    <!-- see the table below to make sure you're using the right version here -->

    <!-- recommended, but optional -->
    <PackageReference Include="Microsoft.WindowsAppSDK" Version="1.8.260804001" />
    <!-- Windows App SDK gives you more access to Windows 10/11 features -->
    <!-- version 1.8.260804001 is what this package is built against, you can use a later one -->
  </ItemGroup>
```

In your window's code-behind:

**MyWindow.xaml.cs**

```csharp
    public MyWindow()
    {
        // other window setup/initialization code

        SourceInitialized += MyWindow_SourceInitialized;
    }

    private void MyWindow_SourceInitialized(object sender, EventArgs e)
    {
        this.UseWindows11CaptionButtons();
    }
```

Repeat the code-behind part for each window that you want to make this change for.
This change is only opt-in for each individual window, not applied project wide.

### Known Issues

**Maximized window**

By default, FlatWindow will add an extra gray frame around its edges when the window is maximized.
This is to counteract some Windows design decision that cuts off the edges of a window when it is
maximized. Usually, this gray frame is hidden as that is what gets cut off, and all is fine.
However, when using this package to utilize Windows 11's title bar customization, this also 
somehow prevents the cutting off from occurring... meaning that the gray frame then becomes visible.

To solve this issue, a new property is being added in SolidShineUi 2.0.3 and 1.9.11 that will
allow the gray frame to be hidden, called `MaximizedWindowFrame`. If you use versions 1.9.11 or
2.0.3 of this package, that fix will be automatically applied.

For earlier versions of SolidShineUi, you will need to create a custom style for FlatWindow or 
find a different way to go about hiding the frame or restoring the cutting-off behavior.

**Resize handles / hit zones**

I'm not sure why, but the hit/hover areas for the mouse to be in for resizing the window end up
being outside the actual window itself. I don't know what the cause of this issue is, so I'm not
quite sure yet how to solve it.

For now, you could add on extra resize grippers at the corners of the window to help make it
easier for users to recognize areas where they can resize the window.

## Which version to use

Different versions of this package (SolidShineUi.Wpf.Windows11Interop) are intended to be used
with different versions of SolidShineUi. See the below table to see which one you should use,
based on which version of SolidShineUi you are using.

| SSUI Version   | Use This Version    |
|----------------|---------------------|
| 1.9.5 - 1.9.10 | 1.9.5.x             |
| 1.9.11 - 1.9.x | 1.9.11.x            |
| 2.0.0 - 2.0.2  | 2.0.0.x             |
| 2.0.3 +        | 2.0.3.x             |

Versions of Solid Shine UI prior to 1.9.5 aren't supported by this, because they are missing
the ability to set the FlatWindow's `CaptionDisplayType` to `None`.

## Avalonia support

Further investigation is being done as to what is needed for the Avalonia version of SSUI. It may
be that this library is not needed for the Avalonia version at all.

## License / About

This code is created by Jayke R. Huempfner (JaykeBird), the maintainer of Solid Shine UI. You can
learn more about Solid Shine UI at [my website, jaykebird.com](https://jaykebird.com/software/ssui),
or view the code on [GitHub](https://github.com/JaykeBird/ssui).

This code is released under the [MIT License](https://github.com/JaykeBird/ssui/blob/main/LICENSE.md).