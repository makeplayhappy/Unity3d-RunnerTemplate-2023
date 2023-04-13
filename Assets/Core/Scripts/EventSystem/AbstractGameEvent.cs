using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyperCasual.Core
{
    /// <summary>
    /// A base class which provides basic event functionalities.
    /// Events facilitate the communication between different scopes of the game.
    /// Each event is a scriptable object instance that has a list of listeners that are notified when the event is triggered. 
    /// </summary>
    public abstract class AbstractGameEvent : ScriptableObject
    {
        readonly List<IGameEventListener> m_EventListeners = new();

        /// <summary>
        /// Triggers the current event instance and notifies the subscribers
        /// </summary>
        public void Raise()
        {
            for (int i = m_EventListeners.Count - 1; i >= 0; i--)
                m_EventListeners[i].OnEventRaised();
            Reset();
        }

        /// <summary>
        /// Adds a class to the list of observers for this event
        /// </summary>
        /// <param name="listener">The class that wants to observe this event</param>
        public void AddListener(IGameEventListener listener)
        {
            if (!m_EventListeners.Contains(listener))
            {
                m_EventListeners.Add(listener);
            }
        }

        /// <summary>
        /// Removes a class from the list of observers for this event
        /// </summary>
        /// <param name="listener">The class that doesn't want to observe this event anymore</param>
        public void RemoveListener(IGameEventListener listener)
        {
            if (m_EventListeners.Contains(listener))
            {
                m_EventListeners.Remove(listener);
            }
        }

        /// <summary>
        /// Each event resets immediately after it's triggered.
        /// This method contains the reset logic for the derived classes.
        /// </summary>
        public abstract void Reset();
    }
}