using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JournalEntries
{
    public static Dictionary<string, string> Entries = new();

    public static string DebuffAdd = "Add";
    public static string DebuffRemove = "Remove";

    static JournalEntries()
    {
        // Debuffs
        Entries.Add(nameof(DebuffDamageOverTime) + DebuffAdd, "");
        Entries.Add(nameof(DebuffDamageOverTime) + DebuffRemove, "");
        Entries.Add(nameof(DebuffSlowMovement) + DebuffAdd, "");
        Entries.Add(nameof(DebuffSlowMovement) + DebuffRemove, "");
        Entries.Add(nameof(DebuffSpawnOnKill) + DebuffAdd, "");
        Entries.Add(nameof(DebuffSpawnOnKill) + DebuffRemove, "");

        // Drinking
        Entries.Add(nameof(LiquidPlant), "");
        Entries.Add(nameof(LiquidWater), "");

    }
}
