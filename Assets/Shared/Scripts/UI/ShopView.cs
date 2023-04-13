using System.Collections;
using System.Collections.Generic;
using HyperCasual.Core;
using UnityEngine;
using UnityEngine.UI;

namespace HyperCasual.Runner
{
    /// <summary>
    /// This View contains shop menu functionalities
    /// </summary>
    public class ShopView : View
    {
        [SerializeField]
        HyperCasualButton m_Button;

        void OnEnable()
        {
            m_Button.AddListener(OnButtonClick);
        }

        void OnDisable()
        {
            m_Button.RemoveListener(OnButtonClick);
        }

        void OnButtonClick()
        {
            UIManager.Instance.GoBack();
        }
    }
}