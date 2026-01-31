---
Title: Enum Creation
Sort_Priority: 32
---

# EnumUtils

A class with utilities for dynamically creating and getting Enums.

## Getting Enums

Shortcuts for getting enums. There is various functions you can use.
Here are some of the examples.

```csharp
public class MyCoolMod : ModBehaviour
{
    public void Start()
	{
        DeathType impact = EnumUtils.Parse<DeathType>("Impact");

        if (EnumUtils.TryParse("Example", out DeathType example))
        {

        }

        int numOfDeathTypes = EnumUtils.Count<DeathType>();
        DeathType minDeathType = EnumUtils.GetMinValue<DeathType>();
        DeathType maxDeathType = EnumUtils.GetMaxValue<DeathType>();
        DeathType randomDeathType = EnumUtils.GetRandom<DeathType>();
        DeathType unusedDeathTypeValue = EnumUtils.GetFirstFreeValue<DeathType>();
        string[] allDeathNames = EnumUtils.GetNames<DeathType>();
        DeathType[] allDeathValues = EnumUtils.GetValues<DeathType>();
        bool doesExampleDeathExist = EnumUtils.IsDefined<DeathType>("Example");
        Type valueType = EnumUtils.GetUnderlyingType<DeathType>();
        bool hasFlags = EnumUtils.IsPowerOfTwoEnum<DeathType>();
    }
}
```

## Creating Enums

You can use the EnumUtils class to create enums just like this.

```csharp
public class MyCoolMod : ModBehaviour
{
    // Associate an unused enum value with a name of your choosing.
    public static readonly DeathType Example = EnumUtils.Create<DeathType>("Example");
    // Associate a specific enum value with a name of your choosing.
    public static readonly DeathType Example2 = EnumUtils.Create<DeathType>("Example2", -1);
    public static readonly DeathType Example3 = EnumUtils.Create<DeathType>("Example3", 9000);

    public void Start()
    {
    }
}
```

## Enum Holders

Another way to create enums.

Add the `[EnumHolder]` attribute to a class, and any static enum fields will have an enum value created with the name of the field. It'll select any unused enum value to be associated with that name (or if you specify a value it'll use that one).

```csharp
[EnumHolder]
public static class ExampleEnumHolderClass 
{
    public static readonly DeathType Example;
    public static readonly DeathType Example2 = (DeathType)(-1);
    public static readonly DeathType Example3 = (DeathType)9000;
}
```