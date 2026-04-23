---
Title: Rebinding
---

# Using Key Rebinds

The rebinding system lets mods create inputs with default keyboard/controller buttons assigned, and lets players change and save their bindings.

## Creating A Rebind

Rebinds must be created in Start(), and are created through ModHelper.RebindingHelper.
When registering a rebindable input you are given back an InputCommandType enum value, which you will use later to retrieve the input's value.

```csharp
public class ExampleMod : ModBehaviour {
    public InputConsts.InputCommandType rebindButton;

    public void Start() {
        rebindButton = ModHelper.RebindingHelper.RegisterRebindable("Test Button", "Tooltip", Key.LeftArrow, GamepadBinding.DPadLeft, false);
    }
}
```

Composite inputs are created with two InputCommandType values. Composite rebinds do not appear in the rebinding menu, and are instead purely for retrieving values.

```csharp
public class ExampleMod : ModBehaviour {
    public InputConsts.InputCommandType rebindX;
    public InputConsts.InputCommandType rebindY;
    public InputConsts.InputCommandType rebindComposite;

    public void Start() {
        rebindX = ModHelper.RebindingHelper.RegisterRebindable("Test X", "X Tooltip", Key.A, GamepadBinding.LeftStickRight, Key.D, GamepadBinding.LeftStickLeft, true);
        rebindY = ModHelper.RebindingHelper.RegisterRebindable("Test Y", "Y Tooltip", Key.W, GamepadBinding.LeftStickUp, Key.S, GamepadBinding.LeftStickDown, true);
        rebindComposite = ModHelper.RebindingHelper.RegisterComposite("Test Composite Axis", rebindX, rebindY);
    }
}
```

## Getting An Input's Value

To retrieve an input value you first retrieve the command associated with the input, then get the value from the command.

For button or axis inputs:
```csharp
public class ExampleMod : ModBehaviour {
    public InputConsts.InputCommandType rebindButton;

    public void Update() {
        var command = InputLibrary.GetInputCommand(rebindButton);
        var val = OWInput.GetValue(command);
    }
}
```

For composite inputs:
```csharp
public class ExampleMod : ModBehaviour {
    public InputConsts.InputCommandType rebindComposite;

    public void Update() {
        var command = InputLibrary.GetInputCommand(rebindComposite);
        var val = OWInput.GetAxisValue(command);
    }
}
```

## Types of Input

| Axis/Button | Single/Dual | Example Binding | Pressed Threshold | GetValue Behaviour | OWInput.IsPressed Behaviour | 
| ----------- | ----------- | --------------- | ------------------| ------------------ | --------------------------- |
| Button      | Single      | Z                  | Default (0.4) | 0 when not pressed, 1 pressed.                                                                           | Same as GetValue. |
| Button      | Single      | RT                 | Default (0.4) | 0 when trigger is not pressed, and remaining 0 until roughly half pressed, depending on deadzone config. | Returns false until the value exceeds the deadzone value, which is roughly 0.5. |
| Button      | Single      | RT                 | 0.7           | 0 when trigger is not pressed, and remaining 0 until roughly half pressed, depending on deadzone config. | Returns false until the value exceeds 0.7. |
| Axis        | Single      | Z                  | Default (0.4) | 0 when not pressed, 1 pressed.                                                                           | Same as GetValue. |
| Axis        | Single      | RT                 | Default (0.4) | 0 when trigger is not pressed, increases as soon as trigger is pressed until 1.                          | Returns false if value is under 0.4, true if 0.4 or above. |
| Axis        | Single      | RT                 | 0.7           | 0 when trigger is not pressed, increases as soon as trigger is pressed until 1.                          | Returns false if value is under 0.7, true if 0.7 or above. |
| Button      | Dual        | Z, X               | Default (0.4) | 0 when neither or both pressed, 1 when Z pressed, -1 when X pressed.                                     | False when neither or both pressed, true if one pressed. |
| Button      | Dual        | LT, RT             | Default (0.4) | 0 when trigger is not pressed, and remaining 0 until roughly half pressed, depending on deadzone config. | Returns false until the value exceeds the deadzone value, which is roughly 0.5. |
| Button      | Dual        | LT, RT             | 0.7           | 0 when trigger is not pressed, and remaining 0 until roughly half pressed, depending on deadzone config. | Returns false until the value exceeds 0.7. |
| Axis        | Dual        | Z, X               | Default (0.4) | 0 when neither or both pressed, 1 when Z pressed, -1 when X pressed.                                     | False when neither or both pressed, true if one pressed. |
| Axis        | Dual        | LT, RT             | Default (0.4) | 0 when trigger is not pressed. LT increases to 1, RT decreases to -1.                                    | Returns false if value is under 0.4, true if 0.4 or above. |
| Axis        | Dual        | LT, RT             | 0.7           | 0 when trigger is not pressed. LT increases to 1, RT decreases to -1.                                    | Returns false if value is under 0.7, true if 0.7 or above. |

### Axis vs Button

A button input is for inputs that are either on or off, and need no precision inbetween. This would include jumping, tool usage, interaction, etc. Button inputs are usually not bound to triggers or joysticks, but can be - their behaviour in these situations is complicated.

Axis inputs are designed for inputs that need precision, rather than just on or off. This would include zooming in or out, movement, or any other control usually bound to triggers or joysticks. Axis inputs can also be bound to normal buttons however.

### Single vs Dual

### Composite