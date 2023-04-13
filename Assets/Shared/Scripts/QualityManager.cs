using System;
using System.Collections;
using System.Collections.Generic;
using HyperCasual.Core;
using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    /// This singleton provides functionality to modify quality settings in runtime
    /// </summary>
    public class QualityManager : AbstractSingleton<QualityManager>
    {
        int m_QualityLevel;

        /// <summary>
        /// The current value of quality level
        /// </summary>
        public int QualityLevel
        {
            get => m_QualityLevel;
            set
            {
                m_QualityLevel = value;
                if (QualitySettings.GetQualityLevel() != m_QualityLevel)
                    QualitySettings.SetQualityLevel(m_QualityLevel, true);
            }
        }

        void OnEnable()
        {
            if (SaveManager.Instance.IsQualityLevelSaved)
                QualityLevel = SaveManager.Instance.QualityLevel;
            else
                QualityLevel = 2;
        }

        void OnDisable()
        {
            SaveManager.Instance.QualityLevel = QualityLevel;
        }
    }
}