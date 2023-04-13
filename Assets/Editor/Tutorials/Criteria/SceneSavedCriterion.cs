using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Unity.Hypercasual.Tutorials.Editor
{
    internal class SceneSavedCriterion : Criterion
    {
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
            return !EditorSceneManager.GetActiveScene().isDirty;
        }

        ///<inheritdoc/>
        public override bool AutoComplete()
        {
            EditorSceneManager.SaveOpenScenes();
            return EvaluateCompletion();
        }
    }
}