---
Title: Menus
---

# ModHelper.Menus

Provides utilities to extend base-game menus and UI.

## Interfaces

These interfaces are used throughout the module

## IModMenu

Represents a menu

### OnInit

The event that's fired when the menu has been initialized.

## IModButton

Represents a button

### OnClick

The event that fires when clicked

### Title

Text displayed on the button

### Duplicate

Duplicates the button with a new label

### Show

Shows the button

### Hide

Hides the button

## IModMessagePopup

### OnConfirm

The event that's fired when the confirm button is pressed

### OnCancel

The event that's fired when the cancel button is pressed

## IModInputMenu

### OnConfirm(string)

The event that's fired when the input is confirmed, it expects an Action&lt;string&gt;

## MainMenu

The main menu of the game. It is represented by `IModMainMenu`.

### IModButtons

- OptionsButton
- QuitButton
- ResumeExpeditionButton
- NewExpeditionButton
- ViewCreditsButton
- SwitchProfileButton

## PauseMenu

The pause menu. It is represented by `IModPauseMenu`.

### IModButtons

- ResumeButton
- OptionsButton
- QuitButton

## Button Example

```csharp
public class MyCoolMod : ModBehaviour {
    public void Start() {
        ModHelper.Menus.MainMenu.OnInit += () => {
            var myButton = ModHelper.Menus.MainMenu.OptionsButton.Duplicate("My Cool Button");
            myButton.OnClick += MyButtonClicked;
        };
    }

    public void MyButtonClicked() {
        ModHelper.Console.WriteLine("My Button Was Clicked!");
    }
}
```

## PopupManager

Allows you to show information and input messages

### CreateMessagePopup

Creates a new message popup

#### Message Parameters

(*italicized* = optional)

- `string message`: The message to show
- *`bool addCancel`*: Whether to add a cancel button to the popup
- *`string okMessage`*: The message to show in the OK button
- *`string cancelMessage`*: The message to show in the cancel button

#### Message Example

```csharp
public class MyCoolMod : ModBehaviour {
    public void Start() {
        var popup = ModHelper.Menus.PopupManager.CreateMessagePopup("What do?", true, "Yes", "What");
        popup.OnConfirm += OnOk;
        popup.OnCancel += OnCancel;
    }

    public void OnOk() {
        ModHelper.Console.WriteLine("You Clicked OK!", MessageType.Success);
    }

    public void OnCancel() {
        ModHelper.Console.WriteLine("You Clicked Cancel!", MessageType.Warning);
    }
}
```

### CreateInputPopup

Creates a new input popup for the player.

#### Input Parameters

- `InputType inputType`: Either `InputType.Text` or `InputType.Number`
- `string value`: The default value for the prompt

#### Input Example

```csharp
public class MyCoolMod : ModBehaviour {
    public void Start() {
        var prompt = ModHelper.Menus.PopupManager.CreateInputPrompt(InputType.Text, "Default");
        prompt.OnConfirm += OnConfirm;
    }

    public void OnConfirm(string value) {
        ModHelper.Console.WriteLine($"You entered: {value}!");
    }
}
```
