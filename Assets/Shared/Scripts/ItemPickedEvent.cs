using System.Collections;
using System.Collections.Generic;
using HyperCasual.Core;
using UnityEngine;

namespace HyperCasual.Gameplay
{
    /// <summary>
    /// The event is triggered when the player picks up an item (like coin and keys)
    /// </summary>
    [CreateAssetMenu(fileName = nameof(ItemPickedEvent),
        menuName = "Runner/" + nameof(ItemPickedEvent))]
    public class ItemPickedEvent : AbstractGameEvent
    {
        [HideInInspector]
        public int Count = -1;
        
        public override void Reset()
        {
            Count = -1;
        }
    }
}
