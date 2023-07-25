using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Journal : MonoBehaviour
{
    public enum LogTypes
    {
        Random,
        Event,
        Interaction
    }

    public Dictionary<LogTypes, string> Messages;

    protected string _currentJournalText;

    protected virtual void Awake()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        _currentJournalText = string.Empty;
    }

    public virtual void LogMessage(LogTypes logType)
    {

    }
}
