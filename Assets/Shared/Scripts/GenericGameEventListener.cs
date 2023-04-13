using System;

namespace HyperCasual.Core
{
    /// <summary>
    /// A generic event observer class
    /// </summary>
    [Serializable]
    public class GenericGameEventListener : IGameEventListener
    {
        /// <summary>
        /// The event this class is observing
        /// </summary>
        public AbstractGameEvent m_Event;
        
        /// <summary>
        /// The event handler invoked once the event is triggered
        /// </summary>
        public Action EventHandler;

        /// <summary>
        /// Start listening to the event
        /// </summary>
        public void Subscribe()
        {
            m_Event.AddListener(this);
        }

        /// <summary>
        /// Stop listening to the event
        /// </summary>
        public void Unsubscribe()
        {
            m_Event.RemoveListener(this);
        }
        
        /// <summary>
        /// The event handler that is called when the subscribed event is triggered
        /// </summary>
        public void OnEventRaised()
        {
            EventHandler?.Invoke();
        }
    }
}
