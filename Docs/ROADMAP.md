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

## Later 2021 / early 2022

- Rewrite SelectPanel to not base from UserControl
  - To help with this, SelectableCollection can be used
- Rebase IntegerSpinner/DoubleSpinner under RangeBase class (breaking change)
- Finish online documentation
- Port to AvaloniaUI (0.10 is out, haven't done any testing or porting yet though)
- PropertyGrid control
- DataTable control
  - use same core bits as PropertyGrid

## 2022

- MAUI Port?
- Toolbar controls
- Ribbon control
- Touch support for ImageColorPicker
  - (Keyboard support may be hard)
