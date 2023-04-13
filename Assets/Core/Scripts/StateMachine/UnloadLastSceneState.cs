using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyperCasual.Core
{
    /// <summary>
    /// Unloads a currently loaded scene
    /// </summary>
    public class UnloadLastSceneState : AbstractState
    {
        readonly SceneController m_SceneController;

        /// <param name="sceneController">The SceneController for the current unloading operation</param>
        public UnloadLastSceneState(SceneController sceneController)
        {
            m_SceneController = sceneController;
        }
        
        public override IEnumerator Execute()
        {
            yield return m_SceneController.UnloadLastScene();
        }
    }
}