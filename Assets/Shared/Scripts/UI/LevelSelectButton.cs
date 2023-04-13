using System;
using System.Collections;
using System.Collections.Generic;
using HyperCasual.Core;
using TMPro;
using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    /// A button used by LevelSelectionScreen to dynamically populate the list of levels to select from
    /// </summary>
    public class LevelSelectButton : HyperCasualButton
    {
        [SerializeField]
        TextMeshProUGUI m_Text;

        int m_Index = -1;
        Action<int> m_OnClick;
        bool m_IsUnlocked;
        
        /// <param name="index">The index of the associated level</param>
        /// <param name="unlocked">Is the associated level locked?</param>
        /// <param name="onClick">callback method for this button</param>
        public void SetData(int index, bool unlocked, Action<int> onClick)
        {
            m_Index = index;
            m_Text.text = (index + 1).ToString();
            m_OnClick = onClick;
            m_IsUnlocked = unlocked;
            m_Button.interactable = m_IsUnlocked;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            AddListener(OnClick);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            RemoveListener(OnClick);
        }

        protected override void OnClick()
        {
            m_OnClick?.Invoke(m_Index);
            PlayButtonSound();
        }
    }
}