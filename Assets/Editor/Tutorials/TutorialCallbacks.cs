using UnityEngine;
using UnityEditor;
using Unity.Tutorials.Core.Editor;
using HyperCasual.Runner;

namespace Unity.Hypercasual.Tutorials.Editor
{
    /// <summary>
    /// Implement your Tutorial callbacks here.
    /// </summary>
    [CreateAssetMenu(fileName = k_DefaultFileName, menuName = "Tutorials/" + k_DefaultFileName + " Instance")]
    internal class TutorialCallbacks : ScriptableObject
    {
        [SerializeField] GameObject m_AdsManager;
        
        /// <summary>
        /// The default file name used to create asset of this class type.
        /// </summary>
        public const string k_DefaultFileName = "TutorialCallbacks";

        /// <summary>
        /// Creates a TutorialCallbacks asset and shows it in the Project window.
        /// </summary>
        /// <param name="assetPath">
        /// A relative path to the project's root. If not provided, the Project window's currently active folder path is used.
        /// </param>
        /// <returns>The created asset</returns>
        public static ScriptableObject CreateAndShowAsset(string assetPath = null)
        {
            assetPath = assetPath ?? $"{TutorialEditorUtils.GetActiveFolderPath()}/{k_DefaultFileName}.asset";
            var asset = CreateInstance<TutorialCallbacks>();
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            EditorUtility.FocusProjectWindow(); // needed in order to make the selection of newly created asset to really work
            Selection.activeObject = asset;
            return asset;
        }

        [SerializeField]
        Tutorial m_StartupTutorial;

        public void StartStartupTutorial()
        {
            TutorialWindowUtils.StartTutorial(m_StartupTutorial);
        }

        public void OpenURL(string url)
        {
            TutorialEditorUtils.OpenUrl(url);
        }

        public void FocusGameView()
        {
            /*
             * todo: this solution is a bit weak, but it's the best we can do without accessing internal APIs.
             * Check that it works for Unity 2022 and 2023 as well
             */
            EditorApplication.ExecuteMenuItem("Window/General/Game");
        }

        public void FocusSceneView()
        {
            EditorApplication.ExecuteMenuItem("Window/General/Scene");
        }

        public bool IsLevelAssetSelected()
        {
            if (EditorWindow.HasOpenInstances<RunnerLevelEditorWindow>())
            {
                return EditorWindow.GetWindow<RunnerLevelEditorWindow>().SourceLevelDefinition != null;
            }
            return false;
        }

        public bool HasLoadedLevelAsset()
        {
            if (EditorWindow.HasOpenInstances<RunnerLevelEditorWindow>())
            {
                return EditorWindow.GetWindow<RunnerLevelEditorWindow>().HasLoadedLevel;
            }
            return false;
        }

        public void ResetLastLevelsCount()
        {
            // TODO - Update tutorials!
        }

        public bool HasAddedLevelToManager()
        {
            var gameManager = GameObject.FindObjectOfType<GameManager>();
            if (!gameManager)
            {
                Debug.LogError("Couldn't find a GameManager in the current scene. Please load a scene that includes an active GameManager instance.");
                return false;
            }

            // TODO - Update tutorials!
            return true;
        }

        public bool InstancesOfAssetTypeInFolder()
        {
            var gameManager = GameObject.FindObjectOfType<GameManager>();
            if (!gameManager)
            {
                Debug.LogError("Couldn't find a GameManager in the current scene. Please load a scene that includes an active GameManager instance.");
                return false;
            }

            // TODO - Update tutorials!
            return true;
        }
        
        /// <summary>
        /// Open the Project Settings window, open to the Ads section
        /// </summary>
        public void OpenUnityAdsServiceSettings()
        {
            SettingsService.OpenProjectSettings("Project/Services/Ads");
        }

        /// <summary>
        /// Open the Unity Ads Home page in a web browser
        /// </summary>
        public void OpenAdsHomePageInBrowser()
        {
            Application.OpenURL("https://docs.unity.com/ads/UnityAdsHome.html");
        }

        public void SelectAdsManager()
        {
            if (m_AdsManager != null)
            {
                Selection.activeGameObject = m_AdsManager;
            }
        }
    }
}