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

### CreateInfoPopup

### CreateTwoChoicePopup

### CreateThreeChoicePopup

### CreateFourChoicePopup

### CreateInputFieldPopup