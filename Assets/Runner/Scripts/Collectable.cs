using System.Collections;
using System.Collections.Generic;
using HyperCasual.Gameplay;
using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    /// A class representing a Spawnable object.
    /// If a GameObject tagged "Player" collides
    /// with this object, it will be collected, 
    // incrementing the player's amount of this item.
    /// </summary>
    public class Collectable : Spawnable
    {
        [SerializeField]
        SoundID m_Sound = SoundID.None;
        
        const string k_PlayerTag = "Player";

        public ItemPickedEvent m_Event;
        public int m_Count;

        bool m_Collected;
        Renderer[] m_Renderers;

        /// <summary>
        /// Reset the gate to its initial state. Called when a level
        /// is restarted by the GameManager.
        /// </summary>
        public override void ResetSpawnable()
        {
            m_Collected = false;
            
            for (int i = 0; i < m_Renderers.Length; i++)
            {
                m_Renderers[i].enabled = true;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            m_Renderers = gameObject.GetComponentsInChildren<Renderer>();
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag(k_PlayerTag) && !m_Collected)
            {
                Collect();
            }
        }

        void Collect()
        {
            if (m_Event != null)
            {
                m_Event.Count = m_Count;
                m_Event.Raise();
            }

            for (int i = 0; i < m_Renderers.Length; i++)
            {
                m_Renderers[i].enabled = false;
            }

            m_Collected = true;
            AudioManager.Instance.PlayEffect(m_Sound);
        }
    }
}