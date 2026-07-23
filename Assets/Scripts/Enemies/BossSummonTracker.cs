using System;
using UnityEngine;

namespace VampireLike.Enemies
{
    /// <summary>
    /// Lets a summon pattern know when one of its spawned enemies disappears.
    /// </summary>
    public class BossSummonTracker : MonoBehaviour
    {
        private Action<BossSummonTracker> removed;
        private bool hasNotified;

        public void Initialize(Action<BossSummonTracker> onRemoved)
        {
            removed = onRemoved;
            hasNotified = false;
        }

        private void OnDisable()
        {
            NotifyRemoved();
        }

        private void OnDestroy()
        {
            NotifyRemoved();
        }

        private void NotifyRemoved()
        {
            if (hasNotified)
                return;

            hasNotified = true;
            removed?.Invoke(this);
        }
    }
}
