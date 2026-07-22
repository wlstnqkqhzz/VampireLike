using UnityEngine;

namespace VampireLike.Growth
{
    public enum UpgradeType
    {
        ProjectileDamage,
        AttackInterval,
        ProjectileCount,
        ProjectilePierce,
        MoveSpeed,
        MaxHealth,
        Heal,
        PickupRadius
    }

    [CreateAssetMenu(fileName = "UpgradeDefinition", menuName = "VampireLike/Growth/Upgrade Definition")]
    public class UpgradeDefinition : ScriptableObject
    {
        [SerializeField]
        private UpgradeType upgradeType;

        [SerializeField]
        private string displayName;

        [SerializeField]
        private string description;

        [SerializeField]
        private int maxLevel = 1;

        [SerializeField]
        private float multiplier = 1f;

        [SerializeField]
        private int flatAmount;

        [SerializeField]
        private bool unlimited;

        public UpgradeType UpgradeType => upgradeType;
        public string DisplayName => displayName;
        public string Description => description;
        public int MaxLevel => maxLevel;
        public float Multiplier => multiplier;
        public int FlatAmount => flatAmount;
        public bool Unlimited => unlimited;

        private void OnValidate()
        {
            maxLevel = Mathf.Max(1, maxLevel);
            multiplier = Mathf.Max(0f, multiplier);
            flatAmount = Mathf.Max(0, flatAmount);
        }
    }
}
