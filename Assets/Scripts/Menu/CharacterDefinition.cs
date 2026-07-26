namespace VampireLike.Menu
{
    /// <summary>
    /// 캐릭터 선택창에 표시하고 게임 시작 시 플레이어에게 적용할 간단한 캐릭터 데이터입니다.
    /// </summary>
    public readonly struct CharacterDefinition
    {
        public CharacterDefinition(
            string id,
            string displayName,
            string role,
            string description,
            float moveSpeedMultiplier,
            float attackIntervalMultiplier,
            float projectileDamageMultiplier,
            int bonusProjectileCount,
            int bonusMaxHealth,
            string animationResourceFolder,
            bool invertHorizontalFacing)
        {
            Id = id;
            DisplayName = displayName;
            Role = role;
            Description = description;
            MoveSpeedMultiplier = moveSpeedMultiplier;
            AttackIntervalMultiplier = attackIntervalMultiplier;
            ProjectileDamageMultiplier = projectileDamageMultiplier;
            BonusProjectileCount = bonusProjectileCount;
            BonusMaxHealth = bonusMaxHealth;
            AnimationResourceFolder = animationResourceFolder;
            InvertHorizontalFacing = invertHorizontalFacing;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public string Role { get; }
        public string Description { get; }
        public float MoveSpeedMultiplier { get; }
        public float AttackIntervalMultiplier { get; }
        public float ProjectileDamageMultiplier { get; }
        public int BonusProjectileCount { get; }
        public int BonusMaxHealth { get; }
        public string AnimationResourceFolder { get; }
        public bool InvertHorizontalFacing { get; }
    }
}
