Adding Keyboard Shortcut Support to your app
============================================

With the new SolidShineUi.KeyboardShortcuts classes, you can easily add flexible keyboard shortcut support to your programs, both new and existing ones. This support is not dependent upon anything else in the SolidShineUi library, so you can continue to use your own UI controls and still utilize this system.

WPF comes with the ability to set up keyboard shortcuts via InputBindings and Commands. SSUI's Keyboard system does support Commands as well, but unfortunately it cannot be set up via bindings or XAML. C# must be used.

However, SSUI's system also supports routed event handlers, Actions (including anonymous Actions), and the support can be customized in your live program, with these customizations able to saved or loaded from XML files.

To get started, it takes 3 steps:

## 1. Add the KeyboardShortcutHandler class to your Window

Find the WPF window that you want to add keyboard shortcut support to, and in the backend, create an instance of the `KeyboardShortcutHandler` class as a private field. Set up the `KeyboardShortcutHandler` in the constructor, the Window's Loaded event, or whatever init or setup method you have.

```C#
// in MainWindow.xaml.cs

// add this using statement at the top:
using SolidShineUi.KeyboardShortcuts;

KeyboardShortcutHandler ksh;

public MainWindow()
{
    InitializeComponent();

    // other init code

    ksh = new KeyboardShortcutHandler(this);
}
```

The KeyboardShortcutHandler class connects to your window's KeyDown and KeyUp events to detect when keyboard shortcuts are pressed, and can activate the appropriate action. This class can also handle the ability to load or save your keyboard shortcuts to/from files.

## 2. Create a list of the actions/events you want keyboard shortcut access to

This may be the lengthiest part of this process, but you'll want to create a `KeyActionList` that lists all the events, actions, or commands that you want activatable via keyboard shortcuts.

Once the list is created, you can start assigning keyboard shortcuts to items on the list in step 3.

Keyboard shortcuts can activate actions or code in one of three ways: routed event handlers (with `RoutedEventKeyAction`), commands (with `CommandKeyAction`), and Actions (with `ActionKeyAction`). You can also roll your own by creating a class taking from the `IKeyAction` interface.

``` C#
// still in MainWindow.xaml.cs

private SolidShineUi.KeyboardShortcuts.KeyActionList CreateCommandList()
{
    // commands can be added in a number of ways

    KeyActionList kal = new KeyActionList();

    // from routed event handlers:
    kal.Add(new RoutedEventKeyAction(myMenuItem_Click, "myMenuItem", myMenuItem));

    // from commands
    kal.Add(new CommandKeyAction(ApplicationCommands.Save, null, null, "Save", btnSave));

    // and from actions (including anonymous actions)
    kal.Add(new ActionKeyAction(() => { Console.WriteLine("hello"); }, "Hello", null));

    // once you have the full list ready, you can move to step 3
    return kal;
}
```

Each key action item you add will include an identifier string (such as "Save" or "myMenuItem") that can be used to reference that action later. You can assign multiple keyboard shortcuts to one action. (However, if you have a Command that you want to use with multiple different CommandTargets, you will need to add a key action item for each CommandTarget.)

If you have a menu bar and you handle the Click event for your menu items, a helper method is included that can generate a list for you by iterating through all of your menu items.

``` C#
    kal.AddRange(RoutedEventKeyAction.CreateListFromMenu(myMenuBar));
```

## 3. Start assigning keyboard shortcuts

Once you have your list of commands and actions, you can start assigning keyboard shortcuts to them. Once a shortcut is assigned, you can press that shortcut while your window has focus and that command should run.

Assigning keyboard shortcuts can be done via your C# code, or the system can load a list from an XML file.

Even once keyboard shortcuts are assigned at the beginning, more can be assigned at any time or un-assigned. Also, any changes or assignments can be saved to an XML file at any time to load in later.

### Creating programmatically:
``` C#
// again, in MainWindow.xaml.cs file

// add this using statement at the top:
using SolidShineUi.KeyboardShortcuts;

KeyboardShortcutHandler ksh;

public MainWindow()
{
    InitializeComponent();

    // other init code

    ksh = new KeyboardShortcutHandler(this);
    ksh.LoadShortcutsFromList(CreateKeyboardShortcuts());
}

private List<KeyboardShortcut> CreateKeyboardShortcuts()
{
    // load in the command list from step 2
    KeyActionList kal = CreateCommandList();
    
    return new List<SolidShineUi.KeyboardShortcuts.KeyboardShortcut>()
    {
        new KeyboardShortcut(KeyboardCombination.Ctrl, Key.S, kal["Save"]),
        new KeyboardShortcut(KeyboardCombination.None, Key.F10, kal["Save"]),
        new KeyboardShortcut(KeyboardCombination.CtrlAlt, Key.H, kal["Hello"]),
        new KeyboardShortcut(KeyboardCombination.Shift, Key.PageDown, kal["myMenuItem"])
    };

    // note that no exception handling is done here
    // if you attempt to get an action with an ID that actually isn't in the KeyActionList, an InvalidOperationException is thrown
}
```

### Loading from a list:
``` C#
// again, in MainWindow.xaml.cs file

// add this using statement at the top:
using SolidShineUi.KeyboardShortcuts;

KeyboardShortcutHandler ksh;

public MainWindow()
{
    InitializeComponent();

    // other init code

    ksh = new KeyboardShortcutHandler(this);
    // give the path to the file, and the list of actions created in step 2, and it'll handle the rest
    ksh.LoadShortcutsFromFile("C:\MyShortcuts.xml", CreateCommandList());

    // in this situation, if the XML file mentions an action with an ID that's not in the KeyActionList, the file loader skips over it
}
```

In the XML file:
``` XML
<?xml version="1.0" encoding="utf-8"?>
<shortcuts>
  <ks comb="Ctrl" keyid="S" methodid="Save" />
  <ks comb="None" keyid="F10" methodid="Save" />
  <ks comb="CtrlAlt" keyid="H" methodid="Hello" />
  <ks comb="Shift" keyid="PageDown" methodid="myMenuItem" />
</shortcuts>
```

Once shortcuts are loaded in either way, you should be all set!

## More code examples:

### Get the action assigned to a keyboard shortcut
``` C#
// "ksh" is the KeyboardShortcutHandler from step 1 in the walkthrough above

private void GetCtrlSMethod()
{
    // this returns a Tuple; if you're unfamiliar see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-tuples
    (IKeyAction? action, string keyString) shortcut = ksh.KeyRegistry.GetActionForKey(KeyboardCombination.Ctrl, Key.S);

    // from here, you can view the action
    Console.WriteLine(shortcut.action.ID);
    // output:
    // Save

    // if there is no action assigned to the particular shortcut, action will be null

    // you can also get a string that is ready to be displayed to the user to activate this shortcut
    Console.WriteLine(shortcut.keyString);
    // output:
    // Ctrl+S
}
```

### Get all keyboard shortcuts an action is assigned to
``` C#
// "ksh" is the KeyboardShortcutHandler from step 1 in the walkthrough above

private void GetSaveShortcuts()
{
    IEnumerable<KeyboardShortcut> shortcuts = ksh.KeyRegistry.GetShortcutsForMethod("Save");

    foreach (KeyboardShortcut shortcut in shortcuts)
    {
        Console.WriteLine(shortcut.KeyString);
    }
    // output:
    // Ctrl+S
    // F10
}
```

### Another way to assign a keyboard shortcut

``` C#
// "ksh" is the KeyboardShortcutHandler from step 1 in the walkthrough above

private void AssignNewShortcut()
{
    ksh.KeyRegistry.RegisterKeyShortcut(KeyboardCombination.Ctrl, Key.O, new RoutedEventKeyAction(btnOpen_Click, "Open", btnOpen));
}
```

### Un-assigning a keyboard shortcut

``` C#
// "ksh" is the KeyboardShortcutHandler from step 1 in the walkthrough above

private void UnassignHello()
{
    // keyboard shortcuts can be unassigned by the keys needed to press to activate a shortcut
    // (will return false if there is no shortcut with this combination and key)
    ksh.KeyRegistry.UnregisterKeyShortcut(KeyboardCombination.CtrlAlt, Key.H);

    // keyboard shortcuts can also be unassigned by passing a KeyboardShortcut
    IEnumerable<KeyboardShortcut> shortcuts = ksh.KeyRegistry.GetShortcutsForMethod("Hello");

    foreach (KeyboardShortcut shortcut in shortcuts)
    {
        ksh.KeyRegistry.UnregisterKeyShortcut(shortcut);
    }
}
```

### Storing keyboard shortcuts and assignments to a file
``` C#
// "ksh" is the KeyboardShortcutHandler from step 1 in the walkthrough above

private async Task StoreKeyboardShortcutFileAsync()
{
    await ksh.WriteShortcutsToFileAsync("C:\myShortcuts.xml");
}
```

## Other notes / Remarks

The `KeyRegistry` class is the class that actually handles the list of keyboard shortcuts, including registering and unregistering shortcuts and returning querys such as GetActionForKey and GetShortcutsForMethod. When a keyboard shortcut is actually pressed in a window, the `KeyboardShortcutHandler` checks against the `KeyRegistry` for any registered items that are activated for the keys that were pressed. If, for some reason, you do not want to use the `KeyboardShortcutHandler`, you can create your own `KeyRegistry` and handle it more directly. File loading/saving is made available via the static `KeyboardShortcutsIo` class in this instance.

The `KeyRegistry` class includes `ShortcutRegistered` and `ShortcutUnregistered` events that can be listened to; as you'd expect, they are raised when a keyboard shortcut is registered and unregisted respectively.

Note that no UI is provided in SolidShineUi 1.8 to allow the user to customize keyboard shortcuts. This may come in a later version. For now, you'll want to build your own UI.

`KeyActionList.Add`, `KeyActionList.AddRange`, `KeyActionList.Insert`, and `KeyActionList.InsertRange` will all check if an action with an ID exists before allowing a new action in with that ID. If multiple actions with the same ID do end up in the KeyActionList, the first one of those (by index) will be the one that's activated and accessed. You can use `KeyActionList.ContainsId(string id)` to check if a particular ID is already on the list.

If you attempt to register a shortcut with the same `KeyboardCombination` and `Key` as a shortcut already registered/assigned, an ArgumentException will be thrown. If loading from a file, the attempted shortcut to register is skipped. As shown above, you can use `KeyRegistry.GetActionForKey` to determine if there's already an assigned shortcut with that given `KeyboardCombination` and `Key`.