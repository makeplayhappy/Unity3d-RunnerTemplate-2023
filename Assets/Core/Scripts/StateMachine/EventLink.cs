using UnityEngine;

namespace HyperCasual.Core
{
    /// <summary>
    /// A link that listens for a specific event and becomes open for transition if the event is raised.
    /// If the current state is linked to next step by this link type,
    /// The state machine waits for the event to be triggered and then moves to the next step.
    /// </summary>
    public class EventLink : ILink, IGameEventListener
    {
        IState m_NextState;
        AbstractGameEvent m_GameEvent;
        bool m_EventRaised;
        
        /// <param name="gameEvent">the event this link listens to</param>
        /// <param name="nextState">the next state</param>
        public EventLink(AbstractGameEvent gameEvent, IState nextState)
        {
            m_GameEvent = gameEvent;
            m_NextState = nextState;
        }

        public bool Validate(out IState nextState)
        {
            nextState = null;
            bool result = false;
            
            if (m_EventRaised)
            {
                nextState = m_NextState;
                result = true;
            }
            
            return result;
        }

        public void OnEventRaised()
        {
            m_EventRaised = true;
        }

        public void Enable()
        {
            m_GameEvent.AddListener(this);
            m_EventRaised = false;
        }
        
        public void Disable()
        {
            m_GameEvent.RemoveListener(this);
            m_EventRaised = false;
        }
    }
}