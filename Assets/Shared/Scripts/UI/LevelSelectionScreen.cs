using System;
using System.Collections;
using System.Collections.Generic;
using HyperCasual.Core;
using HyperCasual.Runner;
using UnityEngine;

namespace HyperCasual.Gameplay
{
    /// <summary>
    /// This View contains level selection screen functionalities
    /// </summary>
    public class LevelSelectionScreen : View
    {
        [SerializeField]
        HyperCasualButton m_QuickPlayButton;
        [SerializeField]
        HyperCasualButton m_BackButton;
        [Space]
        [SerializeField]
        LevelSelectButton m_LevelButtonPrefab;
        [SerializeField]
        RectTransform m_LevelButtonsRoot;
        [SerializeField]
        AbstractGameEvent m_NextLevelEvent;
        [SerializeField]
        AbstractGameEvent m_BackEvent;
#if UNITY_EDITOR
        [SerializeField]
        bool m_UnlockAllLevels;
#endif

        readonly List<LevelSelectButton> m_Buttons = new();

        void Start()
        {
            var levels = SequenceManager.Instance.Levels;
            for (int i = 0; i < levels.Length; i++)
            {
                m_Buttons.Add(Instantiate(m_LevelButtonPrefab, m_LevelButtonsRoot));
            }

            ResetButtonData();
        }

        void OnEnable()
        {
            ResetButtonData();
            
            m_QuickPlayButton.AddListener(OnQuickPlayButtonClicked);
            m_BackButton.AddListener(OnBackButtonClicked);
        }

        void OnDisable()
        {
            m_QuickPlayButton.RemoveListener(OnQuickPlayButtonClicked);
            m_BackButton.RemoveListener(OnBackButtonClicked);
        }

        void ResetButtonData()
        {
            var levelProgress = SaveManager.Instance.LevelProgress;
            for (int i = 0; i < m_Buttons.Count; i++)
            {
                var button = m_Buttons[i];
                var unlocked = i <= levelProgress;
#if UNITY_EDITOR
                unlocked = unlocked || m_UnlockAllLevels;
#endif
                button.SetData(i, unlocked, OnClick);
            }
        }
        
        void OnClick(int startingIndex)
        {
            if (startingIndex < 0)
                throw new Exception("Button is not initialized");

            SequenceManager.Instance.SetStartingLevel(startingIndex);
            m_NextLevelEvent.Raise();
        }
        
        void OnQuickPlayButtonClicked()
        {
            OnClick(SaveManager.Instance.LevelProgress);
        }
        
        void OnBackButtonClicked()
        {
            m_BackEvent.Raise();
        }
    }
}
