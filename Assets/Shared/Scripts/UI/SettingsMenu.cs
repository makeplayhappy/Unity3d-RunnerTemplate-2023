using System.Collections;
using System.Collections.Generic;
using HyperCasual.Core;
using UnityEngine;
using UnityEngine.UI;

namespace HyperCasual.Runner
{
    /// <summary>
    /// This View contains settings menu functionalities
    /// </summary>
    public class SettingsMenu : View
    {
        [SerializeField]
        HyperCasualButton m_Button;
        [SerializeField]
        Toggle m_EnableMusicToggle;
        [SerializeField]
        Toggle m_EnableSfxToggle;
        [SerializeField]
        Slider m_AudioVolumeSlider;
        [SerializeField]
        Slider m_QualitySlider;
        
        void OnEnable()
        {
            m_EnableMusicToggle.isOn = AudioManager.Instance.EnableMusic;
            m_EnableSfxToggle.isOn = AudioManager.Instance.EnableSfx;
            m_AudioVolumeSlider.value = AudioManager.Instance.MasterVolume;
            m_QualitySlider.value = QualityManager.Instance.QualityLevel;
            
            m_Button.AddListener(OnBackButtonClick);
            m_EnableMusicToggle.onValueChanged.AddListener(MusicToggleChanged);
            m_EnableSfxToggle.onValueChanged.AddListener(SfxToggleChanged);
            m_AudioVolumeSlider.onValueChanged.AddListener(VolumeSliderChanged);
            m_QualitySlider.onValueChanged.AddListener(QualitySliderChanged);
        }
        
        void OnDisable()
        {
            m_Button.RemoveListener(OnBackButtonClick);
            m_EnableMusicToggle.onValueChanged.RemoveListener(MusicToggleChanged);
            m_EnableSfxToggle.onValueChanged.RemoveListener(SfxToggleChanged);
            m_AudioVolumeSlider.onValueChanged.RemoveListener(VolumeSliderChanged);
            m_QualitySlider.onValueChanged.RemoveListener(QualitySliderChanged);
        }

        void MusicToggleChanged(bool value)
        {
            AudioManager.Instance.EnableMusic = value;
        }

        void SfxToggleChanged(bool value)
        {
            AudioManager.Instance.EnableSfx = value;
        }

        void VolumeSliderChanged(float value)
        {
            AudioManager.Instance.MasterVolume = value;
        }
        
        void QualitySliderChanged(float value)
        {
            QualityManager.Instance.QualityLevel = (int)value;
        }

        void OnBackButtonClick()
        {
            UIManager.Instance.GoBack();
        }
    }
}
