---
Title: Events
---

# ModHelper.Events

Used to listen to various events in the game

!!! alert-info "Info"
    Although there are other utilities in this module, they have been found to be unreliable and may not execute, you may be better-off using `GlobalMessenger` and `OWEvent` from the game's code to listen to base-game events.

## ModHelper.Events.Unity

Unity lifecycle-related events

### FireOnNextUpdate

Fires the given action on the next `Update` stage of the lifecycle.

```csharp
public class MyCoolMod : ModBehaviour {
    public void Start() {
        ModHelper.Events.Unity.FireOnNextUpdate(() => {
            ModHelper.Console.WriteLine("This logs 1 update later!");
        });
    }
}
```

### FireInNUpdates

Fires the given action after N `Update`s have passed

```csharp
public class MyCoolMod : ModBehaviour {
    public void Start() {
        ModHelper.Events.Unity.FireInNUpdates(() => {
            ModHelper.Console.WriteLine("This logs 10 updates later!");
        }, 10);
    }
}
```

### RunWhen

Keeps checking the given predicate and fires the given action when the predicate is true.

```csharp
public class MyCoolMod : ModBehaviour {

    public bool Check() {
        return EntitlementsManager.IsDlcOwned() != EntitlementsManager.AsyncOwnershipStatus.NotReady;
    }

    public void Start() {
        ModHelper.Events.Unity.RunWhen(Check, () => {
            var ownsDLC = EntitlementsManager.IsDlcOwned() == EntitlementsManager.AsyncOwnershipStatus.Owned? "does" : "doesn't";
            ModHelper.Console.WriteLine($"The player {ownsDLC} own the DLC!");
        });
    }
}
```

!!! alert-warning "Warning"
    Keep in mind the predicate may be evaluated many times, so make sure to keep performance in mind when writing it.
