---
Title: Menu Helper
---

# ModHelper.MenuHelper

Used to create and edit menus.

## ModHelper.MenuHelper.TitleMenuManager

Utilities for modifying the title screen menu.

### CreateTitleButton

Adds a button to the title screen, with the given name and position.

```csharp
public class MyCoolMod : ModBehaviour {
    public override void SetupTitleMenu(ITitleMenuManager titleManager) {
        // Create a button called "TEST" and places it one down from the top.
        var testButton = titleManager.CreateTitleButton("TEXT", 1, true);
        testButton.OnSubmitAction += () => ModHelper.Console.WriteLine("Button pressed!", MessageType.Info);
    }
}
```

### SetButtonText

Changes the text of a button that you already created.

```csharp
public class MyCoolMod : ModBehaviour {
    public override void SetupTitleMenu(ITitleMenuManager titleManager) {
        var testButton = titleManager.CreateTitleButton("TEXT");
        titleManager.SetButtonText(testButton, "NEW TEXT!");
    }
}
```

### SetButtonIndex

Changes the position of a button that you already created.

```csharp
public class MyCoolMod : ModBehaviour {
    public override void SetupTitleMenu(ITitleMenuManager titleManager) {
        // Initially one down from the top.
        var testButton = titleManager.CreateTitleButton("TEXT", 1, true);

        // Now three up from the bottom.
        titleManager.SetButtonIndex(testButton, 3, false);
    }
}
```

## ModHelper.MenuHelper.PauseMenuManager

Utilities for modifying the pause menu.

## ModHelper.MenuHelper.OptionsMenuManager

Utilities for modifying the options menu, both in the title screen and in-game.

### CreateStandardTab

### CreateTabWithSubTabs

### AddSubTab

### RemoveTab

### OpenOptionsAtTab

### AddCheckboxInput

### AddToggleInput

### AddSelectorInput

### AddSliderInput

### AddSeparator

### CreateButton

### CreateButtonWithLabel

### AddTextEntryInput

### CreateLabel

### CreateRebinding

## ModHelper.MenuHelper.PopupMenuManager

Utilities for creating popup menus.

### RegisterStartupPopup

Causes a popup to appear when the game opens, in the same way as the Echoes of the Eye popups.

Must be ran in Start().

```csharp
public class MyCoolMod : ModBehaviour {
    public void Start() {
        ModHelper.MenuHelper.PopupMenuManager.RegisterStartupPopup("This is a popup!");
    }
}
```

### CreateInfoPopup

Creates a popup with text and a close button.

```csharp
public class MyCoolMod : ModBehaviour {
    public override void SetupTitleMenu(ITitleMenuManager titleManager) {
        // Create a title menu button
        var button = titleManager.CreateTitleButton("TEXT");
        // Create the popup
        var popup = ModHelper.MenuHelper.PopupMenuManager.CreateInfoPopup("This is an info popup!", "OK");
        // Make the button open the popup
        button.OnSubmitAction += () => popup.EnableMenu(true);
    }
}
```

### CreateTwoChoicePopup

Creates a popup with text, a confirm button, and a cancel button.

```csharp
public class MyCoolMod : ModBehaviour {
    public override void SetupTitleMenu(ITitleMenuManager titleManager) {
        // Create a title menu button
        var button = titleManager.CreateTitleButton("TEXT");
        // Create the popup
        var popup = ModHelper.MenuHelper.PopupMenuManager.CreateTwoChoicePopup("This is a two choice popup!", "OK", "Cancel");
        // Make the button open the popup
        button.OnSubmitAction += () => popup.EnableMenu(true);

        // Called when the player clicks the confirm button
        popup.OnPopupConfirm += () => ModHelper.Console.WriteLine("Confirm");
        // Called when the player clicks the cancel button
        popup.OnPopupCancel += () => ModHelper.Console.WriteLine("Cancel");
    }
}
```

### CreateThreeChoicePopup

Creates a popup with text, two confirm buttons, and a cancel button.

```csharp
public class MyCoolMod : ModBehaviour {
    public override void SetupTitleMenu(ITitleMenuManager titleManager) {
        // Create a title menu button
        var button = titleManager.CreateTitleButton("TEXT");
        // Create the popup
        var popup = ModHelper.MenuHelper.PopupMenuManager.CreateTwoChoicePopup("This is a two choice popup!", "Option A", "Option B", "Cancel");
        // Make the button open the popup
        button.OnSubmitAction += () => popup.EnableMenu(true);

        // Called when the player clicks the first confirm button
        popup.OnPopupConfirm1 += () => ModHelper.Console.WriteLine("Confirm A");
        // Called when the player clicks the second confirm button
        popup.OnPopupConfirm2 += () => ModHelper.Console.WriteLine("Confirm B");
        // Called when the player clicks the cancel button
        popup.OnPopupCancel += () => ModHelper.Console.WriteLine("Cancel");
    }
}
```

### CreateFourChoicePopup

Creates a popup with text, three confirm buttons, and a cancel button.

```csharp
public class MyCoolMod : ModBehaviour {
    public override void SetupTitleMenu(ITitleMenuManager titleManager) {
        // Create a title menu button
        var button = titleManager.CreateTitleButton("TEXT");
        // Create the popup
        var popup = ModHelper.MenuHelper.PopupMenuManager.CreateTwoChoicePopup("This is a two choice popup!", "Option A", "Option B", "Option C", "Cancel");
        // Make the button open the popup
        button.OnSubmitAction += () => popup.EnableMenu(true);

        // Called when the player clicks the first confirm button
        popup.OnPopupConfirm1 += () => ModHelper.Console.WriteLine("Confirm A");
        // Called when the player clicks the second confirm button
        popup.OnPopupConfirm2 += () => ModHelper.Console.WriteLine("Confirm B");
        // Called when the player clicks the third confirm button
        popup.OnPopupConfirm3 += () => ModHelper.Console.WriteLine("Confirm C");
        // Called when the player clicks the cancel button
        popup.OnPopupCancel += () => ModHelper.Console.WriteLine("Cancel");
    }
}
```

### CreateInputFieldPopup

Creates a popup with a text entry field, a confirm button, and a cancel button.

The "Placeholder Message" is what appears in grey in the entry field before anything has been entered.

```csharp
public class MyCoolMod : ModBehaviour {
    public override void SetupTitleMenu(ITitleMenuManager titleManager) {
        // Create a title menu button
        var button = titleManager.CreateTitleButton("TEXT");
        // Create the popup
        var popup = ModHelper.MenuHelper.PopupMenuManager.CreateInputFieldPopup("What's your name?", "Type your name here!", "OK", "Cancel");
        // Make the button open the popup
        button.OnSubmitAction += () => popup.EnableMenu(true);

        // Called when the player clicks the confirm button
        PopupInput.OnPopupConfirm += () => ModHelper.Console.WriteLine($"Your name is {PopupInput.GetInputText()}!");
    }
}
```