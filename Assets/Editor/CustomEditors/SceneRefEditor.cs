using System.Collections;
using System.Collections.Generic;
using HyperCasual.Core;
using UnityEditor;
using UnityEngine;

namespace HyperCasual.CustomEditors
{
    /// <summary>
    /// Custom editor for <see cref="SceneRef"/> 
    /// </summary>
    [CustomEditor(typeof(SceneRef), true)]
    public class ScenePickerEditor : Editor
    {
        const string k_PropertyName = "m_ScenePath";
        
        public override void OnInspectorGUI()
        {
            var sceneRef = target as SceneRef;

            if (sceneRef == null)
                return;
            
            var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneRef.m_ScenePath);

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            var newScene = EditorGUILayout.ObjectField("Scene", oldScene, typeof(SceneAsset), false) as SceneAsset;

            if (EditorGUI.EndChangeCheck())
            {
                var newPath = AssetDatabase.GetAssetPath(newScene);
                var scenePathProperty = serializedObject.FindProperty(k_PropertyName);
                scenePathProperty.stringValue = newPath;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}