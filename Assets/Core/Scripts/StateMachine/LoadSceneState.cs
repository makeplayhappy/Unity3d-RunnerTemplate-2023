using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyperCasual.Core
{
    /// <summary>
    /// This state loads a scene 
    /// </summary>
    public class LoadSceneState : AbstractState
    {
        readonly string m_Scene;
        readonly SceneController m_SceneController;
        readonly Action m_OnLoadCompleted;
        
        public override string Name => $"{nameof(LoadSceneState)}: {m_Scene}";
        
        /// <param name="sceneController">The SceneController for the current loading operation</param>
        /// <param name="scene">The path to the scene</param>
        /// <param name="onLoadCompleted">An action that is invoked when scene loading is finished</param>
        public LoadSceneState(SceneController sceneController, string scene, Action onLoadCompleted = null)
        {
            m_Scene = scene;
            m_SceneController = sceneController;
            m_OnLoadCompleted = onLoadCompleted;
        }
        
        public override IEnumerator Execute()
        {
            yield return m_SceneController.LoadScene(m_Scene);

            m_OnLoadCompleted?.Invoke();
        }
    }
}