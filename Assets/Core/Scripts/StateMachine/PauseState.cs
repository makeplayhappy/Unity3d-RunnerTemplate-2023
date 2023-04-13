using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyperCasual.Core
{
    /// <summary>
    /// This game loop is paused while this state is active.
    /// </summary>
    public class PauseState : AbstractState
    {
        readonly Action m_OnPause;
        
        public override string Name => $"{nameof(PauseState)}";
        
        /// <param name="onPause">The action that is invoked when the game loop paused</param>
        public PauseState(Action onPause)
        {
            m_OnPause = onPause;
        }

        public override void Enter()
        {
            Time.timeScale = 0f;
            m_OnPause?.Invoke();
        }

        public override IEnumerator Execute()
        {
            yield return null;
        }
        
        public override void Exit()
        {
            Time.timeScale = 1f;
        }
    }
}