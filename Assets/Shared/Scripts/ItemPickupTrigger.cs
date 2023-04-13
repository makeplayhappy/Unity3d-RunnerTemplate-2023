using System.Collections;
using System.Collections.Generic;
using HyperCasual.Core;
using HyperCasual.Runner;
using UnityEngine;

namespace HyperCasual.Gameplay
{
    /// <summary>
    /// Fires an ItemPickedEvent when the player enters the trigger collider attached to this game object.
    /// </summary>
    public class ItemPickupTrigger : Spawnable
    {
        /// <summary>
        /// Player tag
        /// </summary>
        public string m_PlayerTag = "Player";
        
        /// <summary>
        /// The event to raise on trigger
        /// </summary>
        public ItemPickedEvent m_Event;
        
        /// <summary>
        /// Defines how many items(for example coins) are stored in this object.
        /// </summary>
        public int m_Count;
        
        void OnTriggerEnter(Collider col)
        {
            if (!col.CompareTag(m_PlayerTag)) 
                return;
            
            m_Event.Count = m_Count;
            m_Event.Raise();
            gameObject.SetActive(false);
        }
    }
}