using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JadePhoenix.Tools
{
    public class Timer
    {
        /// The total duration of the timer
        public float Duration { get; private set; }
        /// The elapsed time of the timer
        public float ElapsedTime { get; private set; }
        /// Whether the timer is currently running
        public bool IsRunning { get; private set; }

        private Action OnTimerStarted;
        private Action OnTimerCompleted;

        /// <summary>
        /// A generic class that allows us to attach methods and functions.
        /// </summary>
        /// <param name="duration">The total duration of the timer.</param>
        public Timer(float duration)
        {
            Duration = duration;
            ElapsedTime = 0f;
            IsRunning = false;
            this.OnTimerStarted = OnTimerStarted;
            this.OnTimerCompleted = OnTimerCompleted;
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void StartTimer()
        {
            if (IsRunning) { return; }

            IsRunning = true;
            ElapsedTime = 0f;
            OnTimerStarted?.Invoke();
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void StopTimer()
        {
            IsRunning = false;
        }

        /// <summary>
        /// If the timer is running, updates the elapsed time.
        /// Disables the timer when it has reached the set Duration.
        /// This should be called during the Update method of the relevant class.
        /// </summary>
        public void UpdateTimer()
        {
            if (!IsRunning) { return; }

            ElapsedTime += Time.deltaTime;

            if (ElapsedTime < Duration) { return; }

            ElapsedTime = Duration;
            IsRunning = false;
            OnTimerCompleted?.Invoke();
        }

        /// <summary>
        /// Resets the timer to 0.
        /// </summary>
        public void ResetTimer()
        {
            ElapsedTime = 0f;
            IsRunning = false;
        }

        /// <summary>
        /// Sets the duration of the timer.
        /// </summary>
        /// <param name="duration">Time to set the duration to.</param>
        public void SetDuration(float duration)
        {
            Duration = duration;
        }

        /// <summary>
        /// Returns the normalized value of the timer's progress (0 to 1).
        /// </summary>
        /// <returns></returns>
        public float GetNormalisedTime()
        {
            if (Duration <= 0f) { return 0f; }

            return Mathf.Clamp01(ElapsedTime / Duration);
        }
    }
}
