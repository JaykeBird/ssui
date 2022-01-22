SolidShineUI Library Roadmap
============================

This is the general roadmap for the future of this library.

Unlike other roadmaps that I've made (or others that you may have seen online), this roadmap is based upon the year I plan to implement the given items and are not tied to a specific version.

This file will be edited as I make adjustments and changes. Nothing here is set in stone.

## Done

- Release version 1.7
  - NuGet support
  - .NET 5.0 support
- Release version 1.8
  - Keyboard shortcut classes
  - Improvements to FontSelectDialog
  - TabControl is now ready!
- Release version 1.8.1
  - Bug fixes
- Release version 1.8.2
  - Tab drag/drop
  - Improvements to KeyRegistry
  - Additional XML comments for code
- Release version 1.8.3
  - Bug fixes

## Early 2022

- Rewrite SelectPanel to not base from UserControl
  - To help with this, SelectableCollection can be used
- Finish online documentation
- Add "light theme" and "dark theme" to ColorScheme
- first pass at PropertyGrid control

## 2022

- DataTable control
  - use same core bits as PropertyGrid
- Port to AvaloniaUI (0.10 is out, haven't done any testing or porting yet though)
- Toolbar controls
- Ribbon control
- Redesigned ColorScheme system (might be renamed SsuiTheme)
- Touch support for ImageColorPicker
  - (Keyboard support may be hard)
