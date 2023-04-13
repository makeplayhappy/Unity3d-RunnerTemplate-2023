using HyperCasual.Runner;
using System.Collections.Generic;
using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity.Hypercasual.Tutorials.Editor
{
    /// <summary>
    /// A criterion that checks that N instances of a LevelDefinition have been added to a project
    /// </summary>
    internal class DuplicateLevelCriterion : Criterion
    {
        [SerializeField]
        string levelsFolderPath;

        [SerializeField]
        byte desiredNewInstances = 1;

        int instancesAtStart;

        ///<inheritdoc/>
        public override void StartTesting()
        {
            instancesAtStart = GetInstancesInProject(levelsFolderPath);
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
            return GetInstancesInProject(levelsFolderPath) >= (instancesAtStart + desiredNewInstances);
        }

        ///<inheritdoc/>
        public override bool AutoComplete()
        {
            //nice to have, won't do
            return EvaluateCompletion();
        }

        int GetInstancesInProject(string path)
        {
            var existingLevels = new List<LevelDefinition>();
            FindAllInUnityEditor(path, existingLevels);
            return existingLevels.Count;
        }

        static void FindAllInUnityEditor<T>(string searchPath, IList<T> result) where T : UnityEngine.Object
        {
            result.Clear();
            string typeName = typeof(T).Name;
            foreach (var guid in AssetDatabase.FindAssets($"t:{typeName}", new string[] { searchPath }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                result.Add((T)AssetDatabase.LoadMainAssetAtPath(path));
            }
        }
    }
}