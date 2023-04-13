using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Unity.Hypercasual.Tutorials.Editor
{
    /// <summary>
    /// A criterion that checks that a specific scene has been loaded
    /// </summary>
    internal class OpenSceneCriterion : Criterion
    {
        [SerializeField, Tooltip("The scene that has to be loaded to match this Criterion")]
        SceneAsset scene;

        ///<inheritdoc/>
        public override void StartTesting()
        {
            base.StartTesting();
            UpdateCompletion();

            EditorApplication.update += UpdateCompletion;
        }

        ///<inheritdoc/>
        public override void StopTesting()
        {
            base.StopTesting();
            EditorApplication.update -= UpdateCompletion;
        }

        ///<inheritdoc/>
        protected override bool EvaluateCompletion()
        {
            int openScenesCount = EditorSceneManager.sceneCount;
            string targetScenePath = AssetDatabase.GetAssetPath(scene);

            for (int i = 0; i < openScenesCount; i++)
            {
                if (EditorSceneManager.GetSceneAt(i).path == targetScenePath)
                {
                    return true;
                }
            }
            return false;
        }

        ///<inheritdoc/>
        public override bool AutoComplete()
        {
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene));
            return EvaluateCompletion();
        }
    }
}