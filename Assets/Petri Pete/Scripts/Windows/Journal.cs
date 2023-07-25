using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The Journal class provides functionality to log and display messages in a journal-like text format.
/// It utilizes the TextMeshProUGUI component to display the messages.
/// </summary>
public class Journal : MonoBehaviour
{
    protected TextMeshProUGUI _currentJournalText;

    /// <summary>
    /// Initialization method called upon awakening the object.
    /// </summary>
    protected virtual void Awake()
    {
        Initialization();
    }

    /// <summary>
    /// Initializes the Journal by finding the TextMeshProUGUI component and setting it to an empty string.
    /// </summary>
    protected virtual void Initialization()
    {
        _currentJournalText = GetComponentInChildren<TextMeshProUGUI>();
        _currentJournalText.text = string.Empty;
    }

    /// <summary>
    /// Logs a new message to the journal.
    /// </summary>
    /// <param name="messageToAdd">The message to add to the journal.</param>
    public virtual void LogMessage(string messageToAdd)
    {
        _currentJournalText.text += "\n" + messageToAdd;
        _currentJournalText.pageToDisplay = _currentJournalText.textInfo.pageCount;
    }

    // Test code below

    private float messageTimer = 0f;
    private float messageInterval = 0.5f; // Display a new message every half second

    private string[] messages = new string[]
    {
        // ... (a long list of pre-written messages)
        "Hello!",
        "Welcome!",
        "Have a great day!",
        "Keep smiling!",
        "You're awesome!",
        "Enjoy your time!",
        "Stay positive!",
        "Stay curious!",
        "Be kind to others!",
        "You can do it!",
        "Never give up!",
        "Dream big!",
        "Learn something new!",
        "Spread joy!",
        "Believe in yourself!",
        "Embrace challenges!",
        "Make a difference!",
        "Chase your dreams!",
        "Be grateful!",
        "Love and be loved!",
        "Stay determined!",
        "Live in the moment!",
        "Celebrate small victories!",
        "Inspire others!",
        "Take a deep breath!",
        "Practice makes perfect!",
        "Stay humble!",
        "Show compassion!",
        "Strive for greatness!",
        "Forgive and forget!",
        "Count your blessings!",
        "Listen more, talk less!",
        "Life is a journey!",
        "Be true to yourself!",
        "Choose happiness!",
        "Follow your heart!",
        "Never stop learning!",
        "Take risks!",
        "Appreciate the little things!",
        "Stay curious and explore!",
        "Be patient and persistent!",
        "Believe in the power of love!",
        "Celebrate diversity!",
        "Find beauty in simplicity!",
        "Live with passion!",
        "Remember to laugh!",
        "Be the reason someone smiles!",
        "Start where you are!",
        "Success is a mindset!",
        "You are capable!",
        "You are enough!",
        "Inspiration is everywhere!",
        "Be kind to yourself!",
        "The best is yet to come!",
        "Believe in the impossible!",
        "Embrace change!",
        "Create your own path!",
    };

    void Update()
    {
        // Update the timer
        messageTimer += Time.deltaTime;

        // Check if the message interval has passed
        if (messageTimer >= messageInterval)
        {
            // Reset the timer
            messageTimer = 0f;

            // Show a random message
            ShowRandomMessage();
        }
    }

    private void ShowRandomMessage()
    {
        // Get a random index within the range of the messages array
        int randomIndex = Random.Range(0, messages.Length);

        // Display the random message in the journal
        LogMessage(messages[randomIndex]);
    }
}
