using System.Collections;
using System.Collections.Generic;
using HyperCasual.Core;
using UnityEngine;

namespace HyperCasual.Gameplay
{
    /// <summary>
    /// The event is triggered when the player completes a level
    /// </summary>
    [CreateAssetMenu(fileName = nameof(LevelCompletedEvent),
        menuName = "Runner/" + nameof(LevelCompletedEvent))]
    public class LevelCompletedEvent : AbstractGameEvent
    {
        public override void Reset()
        {
        }
    }
}