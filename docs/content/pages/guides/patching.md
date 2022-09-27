---
Title: Patching
Sort_Priority: 50
---

# Patching

Patching allows you to run code before and after base-game methods.  It also allows you to completely replace base-game methods.

## Decompiling The Game

To examine base-game methods and figure out what you need to patch, we recommend you use [DnSpy](https://github.com/dnSpyEx/dnSpy/releases){target="_blank"}, a tool for decompiling and viewing .NET assemblies.

Once installed, select File -> Open. Then, navigate to the game's folder and open the `Assembly-CSharp.dll` file located in `OuterWilds_Data/Managed`.

You should now see the assembly loaded on the left, most classes in Outer Wilds' assembly do not have a namespace, so they'll be located under a `-`

![The - Namespace]({{ "images/patching/dnspy.webp"|static }})

Here are some tips for viewing the base game code:

- If you ever need to find out where a method, class, or variable is used, you can right-click it and select "Analyze".
- You can jump to the definition of a method, class, or variable by clicking on it, you can also control-click to open it in a new tab.
- Sometimes the decompiler treats switch statements strangely, it may look like they're simply chaining else ifs when in reality they're using a switch

## Preparing For Patches

To prepare for adding patches to our mod we'll do two things:

1. Use Harmony.CreateAndPatchAll to make the patches actually *do* something
2. Implement the singleton pattern on our ModBehaviour to make accessing ModHelper outside of it easier.

### CreateAndPatchAll

To make our patches run, we need to execute `Harmony.CreateAndPatchAll` in our `Start` method (technically it doesn't *need* to be in `Start` but you should try to run it as early as possible)

```csharp
public class MyCoolMod : ModBehaviour {
    public void Start() {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }
}
```

And that's it! Now our patches will be applied

### Using A Singleton

Throughout your patches you'll likely want to access `ModHelper`; however, since this is a member of your mod's class and not the patches class you won't be able to.  

To remedy this, you can create a public field on your mod class called `Instance` and assign that field inside of `Awake`.

```csharp
public class MyCoolMod : ModBehaviour {

    public static MyCoolMod Instance;

    public void Awake() {
        Instance = this;
    }

}
```

Now, whenever you want to access your ModHelper you can simply use `MyCoolMod.Instance.ModHelper`.

## Patch Classes

For organization purposes, it's recommended to separate your patches into another class, this class will also need the `HarmonyPatch` attribute on it.

```csharp
[HarmonyPatch]
public class MyPatchClass {

}
```

## Prefixes

A prefix is code that runs before another method.

To create a prefix, add a new method with the `HarmonyPrefix` attribute.

Then, add the `HarmonyPatch` attribute as well, this attribute takes at least two arguments, the first is the type you want to patch, and the second is the name of the method you wish to patch.

Let's say I want to log to the console every time the player dies, to do so I need to prefix `DeathManager.KillPlayer`, so my patch might look something like this:

```csharp
[HarmonyPatch]
public class MyPatchClass {
    [HarmonyPrefix]
    [HarmonyPatch(typeof(DeathManager), nameof(DeathManager.KillPlayer))]
    public static void DeathManager_KillPlayer_Prefix() {
        MyCoolMod.Instance.ModHelper.Console.WriteLine("The player has died! oh no!");
    }
}
```

!!! alert-warning "Warning"
    All patch methods **must** be static!

### Overwriting Methods With Prefixes

Prefixes also allow you to completely stop the original method from running, to do so, make your method return a bool.

- If the boolean returned from the method is `true` the original method will be skipped
- If the boolean returned from the method is `false` the original method is still run

Let's say instead of simply logging when the player dies, we want to prevent it from happening entirely.

```csharp
[HarmonyPatch]
public class MyPatchClass {
    [HarmonyPrefix]
    [HarmonyPatch(typeof(DeathManager), nameof(DeathManager.KillPlayer))]
    public static bool DeathManager_KillPlayer_Prefix() {
        MyCoolMod.Instance.ModHelper.Console.WriteLine("The hatchling would have died, but my cool mod saved them!");
        return false;
    }
}
```

## Postfixes

A postfix is code that runs after another method.

To create a postfix, add a new method with the `HarmonyPostfix` attribute.

Also add the `HarmonyPatch` attribute, it functions the same as it does with prefixes, you need to provide the type to patch and the name of the method to patch.

We'll use `DeathManager.KillPlayer` as an example again:

```csharp
[HarmonyPatch]
public class MyPatchClass {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(DeathManager), nameof(DeathManager.KillPlayer))]
    public static void DeathManager_KillPlayer_Postfix() {
        MyCoolMod.Instance.ModHelper.Console.WriteLine("The player has died! oh no!");
    }
}
```

## Advanced Patching

These next few sections will apply to both Prefixes and Postfixes, so even if the examples use one, know that they will work for the other as well.

### Getting The Object You're Patching

If you want to be able to access the actual object you're patching you can make your patch take an argument named `__instance`.

Let's say I want to log to the console where the Quantum Moon goes to every time it's observed:

```csharp
public class MyPatchClass {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(QuantumMoon), nameof(QuantumMoon.ChangeQuantumState))]
    public static void QuantumMoon_ChangeQuantumState_Postfix(QuantumMoon __instance) {
        MyCoolMod.Instance.ModHelper.Console.WriteLine($"The quantum moon is now at state index: {__instance._stateIndex}!");
    }
}
```

### Getting The Arguments Passed

If you need to get the arguments that were passed to a method inside your patch, you can simply have your method take arguments of the same name.

Going back to our DeathManager example, let's say I want to say the reason the player died as well:

```csharp
[HarmonyPatch]
public class MyPatchClass {
    [HarmonyPrefix]
    [HarmonyPatch(typeof(DeathManager), nameof(DeathManager.KillPlayer))]
    public static void DeathManager_KillPlayer_Prefix(DeathType deathType) {
        MyCoolMod.Instance.ModHelper.Console.WriteLine($"The player died because of: {deathType}!");
    }
}
```

### Overriding The Return Value

If you need to override what value the method returns, you can make your method accept an argument named `__result` by ref.

So if I wanted to make all cards in the ship log green I would do

```csharp
[HarmonyPatch]
public class MyPatchClass {
    [HarmonyPrefix]
    [HarmonyPatch(typeof(UIStyleManager), nameof(UIStyleManager.GetCuriosityColor))]
    public static bool UIStyleManager_GetCuriosityColor(ref Color __result) {
        __result = Color.green;
        return false;
    }
}
```

### Patching Properties

You can patch the getters and setters of properties by passing in a third argument to the `HarmonyPatch` attribute of `MethodType.Getter` and `MethodType.Setter` respectively.

For example, if I wanted to log the repair fraction every time ShipComponent.repairFraction is gotten, I would do:

```csharp
[HarmonyPatch]
public class MyPatchClass {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipComponent), nameof(ShipComponent.repairFraction), MethodType.Getter)]
    public static void ShipComponent_RepairFraction_Get(ref float __result) {
        MyCoolMod.Instance.ModHelper.Console.WriteLine($"This component is at {__result * 100}%!")
    }
}
```

### Patching Overloads

When you want to patch a specific overload of a method, you need to pass the types of the parameters that overload takes as a `Type[]` as the third argument to `HarmonyPatch`.

For example, if I wanted to log `ReferenceFrameTracker.UntargetReferenceFrame` but only the overload that takes a single `bool`, I would do:

```csharp
[HarmonyPatch]
public class MyPatchClass {
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ReferenceFrameTracker), nameof(ReferenceFrameTracker.UntargetReferenceFrame), new System.Type[] { typeof(bool) })]
    public static void ReferenceFrameTracker_UntargetReferenceFrame(bool playAudio)
    {
        MyCoolMod.Instance.ModHelper.Console.WriteLine($"playAudio is {playAudio}!")
    }
}
```

## Further Reading

This about covers all the basics of patching. However, this is just the tip of the iceberg, to learn more, check out both the [Harmony](https://harmony.pardeike.net/articles/intro.html){target="_blank"} and [HarmonyX](https://github.com/BepInEx/HarmonyX/wiki){target="_blank"} docs.
