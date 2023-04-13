using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyperCasual.Core
{
    /// <summary>
    /// A scriptable object that encapsulates a reference to a scene
    /// </summary>
    [CreateAssetMenu(fileName = nameof(SceneRef),
        menuName = "Runner/" + nameof(SceneRef))]
    public class SceneRef : AbstractLevelData
    {
        [SerializeField]
        public string m_ScenePath;
    }
}
