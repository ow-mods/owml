---
title: Console
description: Write messages to the manager console and OWML log file
---

This module is used to output messages to the console.

## WriteLine

Used to log a message to the console.

### Parameters

(*italicized* = optional)

- `string message`: The message to log
- *`MessageType type`*: The type of message to log, see below; Defaults to `MessageType.Message`
- *`string senderType`*: Usually used internally, the type that logged this message, don't pass this unless you have to

### MessageType Reference

| **Value**   | **Description**                                                                                                   |
|-------------|-------------------------------------------------------------------------------------------------------------------|
| **Debug**   | Logs to the console if and only if OWML Debug Mode is set to on in the manager, always logs to the log file       |
| **Message** | Logs a normal message to the console, appears as white in the manager                                             |
| **Info**    | Logs an information message to the console, appears as blue in the manager                                        |
| **Success** | Logs a successful message to the console, appears as green in the manager                                         |
| **Warning** | Logs a warning to the console, appears as yellow in the manager                                                   |
| **Error**   | Logs an error to the console, appears as red in the manager                                                       |
| **Quit**    | Logs a message to the console, and then quits the game.                                                           |
| **Fatal**   | Logs an error to the console, quits the game, and makes the manager display a message box with the message passed |

### Example

```csharp title="MyCoolMod.cs"
public class MyCoolMod : ModBehaviour {
    public void Start() {
        ModHelper.Console.WriteLine("My Mod Is Started!", MessageType.Success);
        ModHelper.Console.WriteLine("Secret Message For Debug Only", MessageType.Debug);
    }

    public void Update() {
        try {
            var x = Convert.ToInt32("burger");
        } catch (FormatException) {
            ModHelper.Console.WriteLine("My Mod Had A Fatal Error", MessageType.Fatal);
        }
    }
}
```
