using UnityEngine;

namespace VampireLike.Growth
{
    /// <summary>
    /// 강화가 어떤 능력치를 바꾸는지 구분하는 타입이다.
    /// </summary>
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

    /// <summary>
    /// 레벨업 선택지 하나의 이름, 설명, 최대 레벨, 적용 수치를 담는 ScriptableObject 데이터다.
    /// </summary>
    [CreateAssetMenu(fileName = "UpgradeDefinition", menuName = "VampireLike/Growth/Upgrade Definition")]
    public class UpgradeDefinition : ScriptableObject
    {
        // 이 강화가 실제로 적용할 효과 타입이다.
        [SerializeField]
        private UpgradeType upgradeType;

        // UI에 표시할 강화 이름이다.
        [SerializeField]
        private string displayName;

        // UI에 표시할 강화 설명이다.
        [SerializeField]
        private string description;

        // 제한 있는 강화의 최대 레벨이다.
        [SerializeField]
        private int maxLevel = 1;

        // 공격력, 공격 간격, 이동 속도, 획득 범위처럼 곱연산에 사용하는 값이다.
        [SerializeField]
        private float multiplier = 1f;

        // 체력 증가, 회복량, 투사체 개수처럼 정수 증가에 사용하는 값이다.
        [SerializeField]
        private int flatAmount;

        // 응급 치료처럼 횟수 제한 없이 계속 선택 가능한 강화인지 정한다.
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
