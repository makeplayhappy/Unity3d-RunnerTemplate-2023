using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace HyperCasual.Runner
{
    /// <summary>
    /// A level editor window that allows the user to 
    /// load levels in the level editor scene, modify level 
    /// parameters, and save that level to be loaded in
    /// a Runner game.
    /// </summary>
    public class RunnerLevelEditorWindow : EditorWindow
    {
        internal bool HasLoadedLevel { get; private set; }
        internal LevelDefinition SourceLevelDefinition => m_SourceLevelDefinition;
        LevelDefinition m_SourceLevelDefinition;
        LevelDefinition m_LoadedLevelDefinition;

        GameObject m_LevelParentGO;
        GameObject m_LoadedLevelGO;
        GameObject m_TerrainGO;
        GameObject m_LevelMarkersGO;

        List<Spawnable> m_SelectedSpawnables = new List<Spawnable>();
        Color m_ActiveColor;
        bool m_CurrentLevelNotLoaded;
        bool m_AutoSaveShowSettings;
        bool m_AutoSaveLevel;
        bool m_AutoSavePlayer;
        bool m_AutoSaveCamera;

        bool m_AutoSaveSettingsLoaded;
        bool m_AttemptedToLoadPreviousLevel;

        const string k_EditorPrefsPreviouslyLoadedLevelPath = "PreviouslyLoadedLevelPath";

        const string k_AutoSaveSettingsInitializedKey = "AutoSaveInitialized";
        const string k_AutoSaveLevelKey = "AutoSaveLevel";
        const string k_AutoSavePlayerKey = "AutoSavePlayer";
        const string k_AutoSaveCameraKey = "AutoSaveCamera";
        const string k_AutoSaveShowSettingsKey = "AutoSaveShowSettings";

        const string k_LevelParentGameObjectName = "LevelParent";
        const string k_LevelEditorSceneName = "RunnerLevelEditor";
        const string k_LevelEditorScenePath = "Assets/Runner/Scenes/RunnerLevelEditor.unity";

        /// <summary>
        /// Returns the loaded LevelDefinition.
        /// </summary>
        public LevelDefinition LoadedLevelDefinition => m_LoadedLevelDefinition;

        static readonly Color s_Blue = new Color(0.0f, 0.0f, 1.0f, 1.0f);
        static readonly string s_LevelParentTag = "LevelParent";

        [MenuItem("Window/Runner Level Editor")]
        static void Init()
        {
            RunnerLevelEditorWindow window = (RunnerLevelEditorWindow)EditorWindow.GetWindow(typeof(RunnerLevelEditorWindow), false, "Level Editor");
            window.Show();

            // Load auto-save settings
            window.LoadAutoSaveSettings();
        }

        void OnFocus() 
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;

            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;

            EditorSceneManager.sceneSaved -= OnSceneSaved;
            EditorSceneManager.sceneSaved += OnSceneSaved;
        }

        /// <summary>
        /// Load the auto-save settings from EditorPrefs.
        /// </summary>
        public void LoadAutoSaveSettings()
        {
            bool autoSaveSettingsInitialized = EditorPrefs.GetBool(k_AutoSaveSettingsInitializedKey);

            if (!autoSaveSettingsInitialized)
            {
                // Default all auto-save values to true and save them to Editor Prefs
                // the first time the user opens the window

                m_AutoSaveLevel = true;
                m_AutoSavePlayer = true;
                m_AutoSaveCamera = true;

                EditorPrefs.SetBool(k_AutoSaveLevelKey, m_AutoSaveLevel);
                EditorPrefs.SetBool(k_AutoSavePlayerKey, m_AutoSavePlayer);
                EditorPrefs.SetBool(k_AutoSaveCameraKey, m_AutoSaveCamera);

                EditorPrefs.SetBool(k_AutoSaveSettingsInitializedKey, true);
                return;
            }

            m_AutoSaveShowSettings = EditorPrefs.GetBool(k_AutoSaveShowSettingsKey);
            m_AutoSaveLevel = EditorPrefs.GetBool(k_AutoSaveLevelKey);
            m_AutoSavePlayer = EditorPrefs.GetBool(k_AutoSavePlayerKey);
            m_AutoSaveCamera = EditorPrefs.GetBool(k_AutoSaveCameraKey);

            m_AutoSaveSettingsLoaded = true;
        }

        void OnPlayModeChanged(PlayModeStateChange state)
        {
            if ((state == PlayModeStateChange.EnteredEditMode || state == PlayModeStateChange.EnteredPlayMode) && m_SourceLevelDefinition != null)
            {
                Scene scene = EditorSceneManager.GetActiveScene();
                if (scene.name.Equals(k_LevelEditorSceneName))
                {
                    // Reload the scene automatically
                    LoadLevel(m_SourceLevelDefinition);
                }
            }
            else if (state == PlayModeStateChange.ExitingEditMode && m_SourceLevelDefinition != null && !LevelNotLoaded())
            {
                Scene scene = EditorSceneManager.GetActiveScene();
                if (scene.name.Equals(k_LevelEditorSceneName))
                {
                    // Save the scene automatically before testing
                    SaveLevel(m_LoadedLevelDefinition);
                }
            }
        }

        void OnSceneSaved(Scene scene)
        {
            if (m_SourceLevelDefinition != null && !LevelNotLoaded())
            {
                if (scene.name.Equals(k_LevelEditorSceneName))
                {
                    SaveLevel(m_LoadedLevelDefinition);
                }
            }
        }

        void OnSelectionChange()
        {
            // Needed to update color options when a Spawnable is selected
            Repaint();
        }

        void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorSceneManager.sceneSaved -= OnSceneSaved;
        }

        void OnSceneGUI(SceneView sceneView)
        {
            if (m_LoadedLevelDefinition == null)
            {
                string levelPath = EditorPrefs.GetString(k_EditorPrefsPreviouslyLoadedLevelPath);
                bool levelPathExists = !string.IsNullOrEmpty(levelPath);

                // Attempt to load previously loaded level
                if (!m_AttemptedToLoadPreviousLevel && levelPathExists)
                {
                    m_SourceLevelDefinition = AssetDatabase.LoadAssetAtPath<LevelDefinition>(levelPath);

                    if (m_SourceLevelDefinition != null)
                    {
                        LoadLevel(m_SourceLevelDefinition);
                    }
                    else
                    {
                        Debug.LogError($"Could not load level with path {levelPath}. Specify a valid level to continue.");
                        m_AttemptedToLoadPreviousLevel = true;
                    }
                }
                else if (levelPathExists)
                {
                    Debug.LogError($"Could not load level with path {levelPath}. Specify a valid level to continue.");
                    m_AttemptedToLoadPreviousLevel = true;
                }
                
                return;
            }

            if (m_LoadedLevelDefinition.SnapToGrid)
            {
                float nearestGridPositionToLevelWidth = m_LoadedLevelDefinition.LevelWidth + m_LoadedLevelDefinition.LevelWidth % m_LoadedLevelDefinition.GridSize;
                float nearestGridPositionToLevelLength = m_LoadedLevelDefinition.LevelLength + m_LoadedLevelDefinition.LevelLength % m_LoadedLevelDefinition.GridSize;

                int numberOfGridLinesWide = (int)Mathf.Ceil(nearestGridPositionToLevelWidth / m_LoadedLevelDefinition.GridSize);
                int numberOfGridLinesLong = (int)Mathf.Ceil(nearestGridPositionToLevelLength / m_LoadedLevelDefinition.GridSize);

                Handles.BeginGUI();
                Handles.color = s_Blue;

                // Empty label is needed to draw lines below
                Handles.Label(Vector3.zero, "");

                float gridWidth = numberOfGridLinesWide * m_LoadedLevelDefinition.GridSize;
                float gridLength = numberOfGridLinesLong * m_LoadedLevelDefinition.GridSize;

                // Draw horizontal grid lines (parallel to X axis) from the start 
                // of the level to the end of the level
                for (int z = 0; z <= numberOfGridLinesLong; z++)
                {
                    float zPosition = z * m_LoadedLevelDefinition.GridSize;
                    Handles.DrawLine(new Vector3(-gridWidth, 0.0f, zPosition), new Vector3(gridWidth, 0.0f, zPosition));
                }

                // Draw vertical grid lines (parallel to Z axis) from the center out
                for (int x = 0; x <= numberOfGridLinesWide; x++)
                {
                    float xPosition = x * m_LoadedLevelDefinition.GridSize;
                    Handles.DrawLine(new Vector3(-xPosition, 0.0f, 0.0f), new Vector3(-xPosition, 0.0f, gridLength));

                    // Only draw one grid line at the center of the level
                    if (x > 0)
                    {
                        Handles.DrawLine(new Vector3(xPosition, 0.0f, 0.0f), new Vector3(xPosition, 0.0f, gridLength));
                    }
                }
                Handles.EndGUI();
            }
        }

        void OnGUI()
        {
            if (!m_AutoSaveSettingsLoaded)
            {
                // Load auto-save settings
                LoadAutoSaveSettings();
            }

            if (Application.isPlaying)
            {
                GUILayout.Label("Exit play mode to continue editing level.");
                return;
            }

            Scene scene = SceneManager.GetActiveScene();
            if (!scene.name.Equals(k_LevelEditorSceneName))
            {
                if (GUILayout.Button("Open Level Editor Scene"))
                {
                    EditorSceneManager.OpenScene(k_LevelEditorScenePath);
                    if (m_SourceLevelDefinition != null)
                    {
                        LoadLevel(m_SourceLevelDefinition);
                    }
                }
                return;
            }

            m_SourceLevelDefinition = (LevelDefinition)EditorGUILayout.ObjectField("Level Definition", m_SourceLevelDefinition, typeof(LevelDefinition), false, null);

            if (m_SourceLevelDefinition == null)
            {
                GUILayout.Label("Select a LevelDefinition ScriptableObject to begin.");
                HasLoadedLevel = false;
                return;
            }

            if (m_LoadedLevelDefinition != null && !m_SourceLevelDefinition.name.Equals(m_LoadedLevelDefinition.name))
            {
                // Automatically load the new source level if it has changed.
                LoadLevel(m_SourceLevelDefinition);
                return;
            }

            if (Event.current.type == EventType.Layout)
            {
                m_CurrentLevelNotLoaded = LevelNotLoaded();
            }

            if (m_LoadedLevelGO != null && !m_CurrentLevelNotLoaded)
            {
                if (GUILayout.Button("Reload Level"))
                {
                    LoadLevel(m_SourceLevelDefinition);
                }
            }
            else
            {
                LoadLevel(m_SourceLevelDefinition);
            }

            if (m_LoadedLevelDefinition == null || m_CurrentLevelNotLoaded)
            {
                GUILayout.Label("No level loaded.");
                return;
            }

            if (GUILayout.Button("Save Level"))
            {
                SaveLevel(m_LoadedLevelDefinition);
            }

            // Auto-save

            m_AutoSaveShowSettings = EditorGUILayout.BeginFoldoutHeaderGroup(m_AutoSaveShowSettings, "Auto-Save Settings");

            if (m_AutoSaveShowSettings)
            {
                EditorGUI.BeginChangeCheck();
                m_AutoSaveLevel = EditorGUILayout.Toggle(new GUIContent("Save Level on Play", "Any changes made to the level being edited will be automatically saved when entering play mode."), m_AutoSaveLevel);
                m_AutoSavePlayer = EditorGUILayout.Toggle(new GUIContent("Save Player on Play", "Any changes made to the Player prefab will be automatically saved when entering play mode and reflected when playing the game via the Boot scene."), m_AutoSavePlayer);
                m_AutoSaveCamera = EditorGUILayout.Toggle(new GUIContent("Save Camera on Play", "Any changes made to the GameplayCamera prefab will be automatically saved when entering play mode and reflected when playing the game via the Boot scene."), m_AutoSaveCamera);
                if (EditorGUI.EndChangeCheck())
                {
                    SaveAutoSaveSettings();
                }
            }
            EditorGUILayout.Space();

            // Level Size Parameters

            GUILayout.Label("Terrain", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            m_LoadedLevelDefinition.LevelLength = Mathf.Max(0.0f, EditorGUILayout.FloatField("Length", m_LoadedLevelDefinition.LevelLength));
            m_LoadedLevelDefinition.LevelWidth = Mathf.Max(0.0f, EditorGUILayout.FloatField("Width", m_LoadedLevelDefinition.LevelWidth));
            m_LoadedLevelDefinition.LevelLengthBufferStart = Mathf.Max(0.0f, EditorGUILayout.FloatField("Start Buffer", m_LoadedLevelDefinition.LevelLengthBufferStart));
            m_LoadedLevelDefinition.LevelLengthBufferEnd = Mathf.Max(0.0f, EditorGUILayout.FloatField("End Buffer", m_LoadedLevelDefinition.LevelLengthBufferEnd));
            m_LoadedLevelDefinition.LevelThickness = Mathf.Max(EditorGUILayout.FloatField("Level Thickness", m_LoadedLevelDefinition.LevelThickness));
            m_LoadedLevelDefinition.TerrainMaterial = (Material)EditorGUILayout.ObjectField("Terrain Material", m_LoadedLevelDefinition.TerrainMaterial, typeof(Material), false, null);
            if (EditorGUI.EndChangeCheck() && m_TerrainGO != null && m_LevelParentGO != null)
            {
                GameManager.CreateTerrain(m_LoadedLevelDefinition, ref m_TerrainGO);
                m_TerrainGO.transform.SetParent(m_LevelParentGO.transform);
            }
            EditorGUILayout.Space();

            // Spawnable Snapping

            GUILayout.Label("Snapping Options", EditorStyles.boldLabel);
            m_LoadedLevelDefinition.SnapToGrid = EditorGUILayout.Toggle("Snap to Grid", m_LoadedLevelDefinition.SnapToGrid);
            if (m_LoadedLevelDefinition.SnapToGrid)
            {
                // Ensure the grid size is never too small, zero, or negative
                m_LoadedLevelDefinition.GridSize = Mathf.Max(0.1f, EditorGUILayout.FloatField("Grid Size", m_LoadedLevelDefinition.GridSize));
            }
            EditorGUILayout.Space();

            // Necessary Prefabs

            GUILayout.Label("Prefabs", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            m_LoadedLevelDefinition.StartPrefab = (GameObject)EditorGUILayout.ObjectField("Start Prefab", m_LoadedLevelDefinition.StartPrefab, typeof(GameObject), false, null);
            m_LoadedLevelDefinition.EndPrefab = (GameObject)EditorGUILayout.ObjectField("End Prefab", m_LoadedLevelDefinition.EndPrefab, typeof(GameObject), false, null);
            if (EditorGUI.EndChangeCheck())
            {
                GameManager.PlaceLevelMarkers(m_LoadedLevelDefinition, ref m_LevelMarkersGO);
                m_LevelMarkersGO.transform.SetParent(m_LevelParentGO.transform);
            }
            EditorGUILayout.Space();

            // Spawnable Coloring
            if (Selection.gameObjects != null && Selection.gameObjects.Length > 0)
            {
                m_SelectedSpawnables.Clear();
                for (int i = 0; i < Selection.gameObjects.Length; i++)
                {
                    Spawnable spawnable = Selection.gameObjects[i].GetComponent<Spawnable>();
                    if (spawnable != null && PrefabUtility.IsPartOfNonAssetPrefabInstance(Selection.gameObjects[i]))
                    {
                        m_SelectedSpawnables.Add(spawnable);
                    }
                }

                if (m_SelectedSpawnables.Count > 0)
                {
                    GUILayout.Label("Selected Spawnable Options", EditorStyles.boldLabel);
                    m_ActiveColor = EditorGUILayout.ColorField("Base Color", m_ActiveColor);
                    if (GUILayout.Button("Apply Base Color to Selected Spawnables"))
                    {
                        for (int i = 0; i < m_SelectedSpawnables.Count; i++)
                        {
                            m_SelectedSpawnables[i].SetBaseColor(m_ActiveColor);
                        }
                    }
                }
                EditorGUILayout.Space();
            }

            EditorGUILayout.HelpBox($"New objects added to the level require a {nameof(Spawnable)} type component added to the GameObject", MessageType.Info);
        }

        bool LevelNotLoaded()
        {
            return m_LoadedLevelDefinition == null || m_LevelParentGO == null || m_LoadedLevelGO == null || m_TerrainGO == null || m_LevelMarkersGO == null;
        }

        void LoadLevel(LevelDefinition levelDefinition)
        {
            UnloadOpenLevels();

            if (!EditorSceneManager.GetActiveScene().path.Equals(k_LevelEditorScenePath))
                return;

            m_LoadedLevelDefinition = Instantiate(levelDefinition);
            m_LoadedLevelDefinition.name = levelDefinition.name;

            m_LevelParentGO = new GameObject(k_LevelParentGameObjectName);
            m_LevelParentGO.tag = s_LevelParentTag;

            GameManager.LoadLevel(m_LoadedLevelDefinition, ref m_LoadedLevelGO);
            GameManager.CreateTerrain(m_LoadedLevelDefinition, ref m_TerrainGO);
            GameManager.PlaceLevelMarkers(m_LoadedLevelDefinition, ref m_LevelMarkersGO);

            m_LoadedLevelGO.transform.SetParent(m_LevelParentGO.transform);
            m_TerrainGO.transform.SetParent(m_LevelParentGO.transform);
            m_LevelMarkersGO.transform.SetParent(m_LevelParentGO.transform);
            HasLoadedLevel = true;

            string levelPath = AssetDatabase.GetAssetPath(levelDefinition);
            EditorPrefs.SetString(k_EditorPrefsPreviouslyLoadedLevelPath, levelPath);

            m_AttemptedToLoadPreviousLevel = false;

            Repaint();
        }

        void UnloadOpenLevels()
        {
            GameObject[] levelParents = GameObject.FindGameObjectsWithTag(s_LevelParentTag);
            for (int i = 0; i < levelParents.Length; i++)
            {
                DestroyImmediate(levelParents[i]);
            }

            m_LevelParentGO = null;
        }

        void SaveLevel(LevelDefinition levelDefinition)
        {
            if (m_AutoSaveLevel)
            {
                // Update array of spawnables based on what is currently in the scene
                Spawnable[] spawnables = (Spawnable[])Object.FindObjectsOfType(typeof(Spawnable));
                levelDefinition.Spawnables = new LevelDefinition.SpawnableObject[spawnables.Length];
                for (int i = 0; i < spawnables.Length; i++)
                {
                    try
                    {
                        levelDefinition.Spawnables[i] = new LevelDefinition.SpawnableObject()
                        {
                            SpawnablePrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(spawnables[i].gameObject),
                            Position = spawnables[i].SavedPosition,
                            EulerAngles = spawnables[i].transform.eulerAngles,
                            Scale = spawnables[i].transform.lossyScale,
                            BaseColor = spawnables[i].BaseColor
                        };
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e.ToString());
                    }
                }

                // Overwrite source level definition with current version
                m_SourceLevelDefinition.SaveValues(levelDefinition);
            }

            if (m_AutoSavePlayer)
            {
                PlayerController[] players = (PlayerController[])Object.FindObjectsOfType(typeof(PlayerController));
                if (players.Length == 1)
                {
                    GameObject playerPrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(players[0].gameObject);
                    if (playerPrefab != null)
                    {
                        PrefabUtility.ApplyPrefabInstance(players[0].gameObject, InteractionMode.UserAction);
                    }
                    else
                    {
                        Debug.LogError("PlayerController could not be found on a prefab instance. Changes could not be saved.");
                    }
                }
                else
                {
                    if (players.Length == 0)
                    {
                        Debug.LogWarning("No instance of PlayerController found in the scene. No changes saved!");
                    }
                    else 
                    {
                        Debug.LogWarning("More than two instances of PlayerController found in the scene. No changes saved!");
                    }
                }
            }

            if (m_AutoSaveCamera)
            {
                CameraManager[] cameraManagers = (CameraManager[])Object.FindObjectsOfType(typeof(CameraManager));
                if (cameraManagers.Length == 1)
                {
                    GameObject cameraManagerPrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(cameraManagers[0].gameObject);
                    if (cameraManagerPrefab != null)
                    {
                        PrefabUtility.ApplyPrefabInstance(cameraManagers[0].gameObject, InteractionMode.UserAction);
                    }
                    else
                    {
                        Debug.LogError("CameraManager could not be found on a prefab instance. Changes could not be saved.");
                    }
                }
                else
                {
                    if (cameraManagers.Length == 0)
                    {
                        Debug.LogWarning("No instance of CameraManager found in the scene. No changes saved!");
                    }
                    else 
                    {
                        Debug.LogWarning("More than two instances of CameraManager found in the scene. No changes saved!");
                    }
                }
            }

            // Set level definition dirty so the changes will be written to disk
            EditorUtility.SetDirty(m_SourceLevelDefinition);

            // Write changes to disk
            AssetDatabase.SaveAssets();
        }

        void SaveAutoSaveSettings()
        {
            // Write auto-save settings to EditorPrefs
            EditorPrefs.SetBool(k_AutoSaveLevelKey, m_AutoSaveLevel);
            EditorPrefs.SetBool(k_AutoSavePlayerKey, m_AutoSavePlayer);
            EditorPrefs.SetBool(k_AutoSaveCameraKey, m_AutoSaveCamera);
            EditorPrefs.SetBool(k_AutoSaveShowSettingsKey, m_AutoSaveShowSettings);
        }
    }
}