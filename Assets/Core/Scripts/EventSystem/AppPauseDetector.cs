using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyperCasual.Core
{
    /// <summary>
    /// Overrides unity event methods to determine if the game is paused or lost focus by external(OS) sources
    /// and triggers an event.
    /// </summary>
    public class AppPauseDetector : MonoBehaviour
    {
        [SerializeField]
        AbstractGameEvent m_PauseEvent;
        
        /// <summary>
        /// Returns the current pause state of the application
        /// </summary>
        public bool IsPaused { get; private set; }
        
        void OnApplicationFocus(bool hasFocus)
        {
            IsPaused = !hasFocus;

            if (IsPaused)
                m_PauseEvent.Raise();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            IsPaused = pauseStatus;
            
            if (IsPaused)
                m_PauseEvent.Raise();
        }
    }
}