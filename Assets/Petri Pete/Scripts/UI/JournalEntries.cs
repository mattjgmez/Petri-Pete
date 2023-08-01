using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JournalEntries
{
    // A static dictionary to hold our journal entries.
    private static Dictionary<string, string> entries = new Dictionary<string, string>()
    {
        // An example of a Journal Entry.
        //{"exampleID", "Example log message."},
        {"Virus", "Virus debuff triggered on Pete."},
        {"VirusEnded", "Pete is no longer effected by the virus."},
        {"VirusSlain", "Virus was killed by Pete."},
        {"Oxygen", "Oxygen debuff triggered on Pete."},
        {"OxygenEnded", "Pete can breath easy now."},
        {"SlowMovement", "Slowing debuff triggered on Pete."},
        {"TardigradeSlain", "Pete has killed a Tardigrade."},
        {"PeteSlain", "Pete has died."},
        {"Drink", "Pete has taken a drink."},
    };

    // Static method to retrieve a log message by its identifier.
    public static string GetEntry(string id)
    {
        if (entries.TryGetValue(id, out string logMessage))
        {
            return logMessage;
        }
        else
        {
            Debug.LogWarning($"JournalEntries.GetEntry: Log with identifier [{id}] not found.");

            return null;  // Return null if the ID is not found.
        }
    }
}
