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
- Release version 1.9
  - Rewrite SelectPanel to not base from UserControl
    - To help with this, SelectableCollection can be used
  - Add "light theme" and "dark theme" to ColorScheme
  - first pass at PropertyGrid control
- Versions 1.9.2 and 1.9.3
  - Further work on PropertyGrid
  - .NET 7.0 support
  - Bulk of online documentation set up

## 2022

- Add extra documents to describe more things
- Finish "must-have"s for PropertyGrid
- Begin testing with AvaloniaUI support

## Mid 2023

- Ribbon control
- Toolbar controls
- Redesigned ColorScheme system (might be renamed SsuiTheme)
- Work on port to AvaloniaUI (11.0 will be the version I'm targeting)
- DataTable control, DataInputDialog
  - use same core bits as PropertyList
  - will need a static basic list of property editors, rather than it being stored in the PropertyList
- Touch/keyboard support for ImageColorPicker
  - May require yet another rewrite
