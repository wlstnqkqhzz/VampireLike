using System.Collections.Generic;
using VampireLike.Audio;
using VampireLike.Growth;

namespace VampireLike.Menu
{
    /// <summary>
    /// 캐릭터 선택창에 표시하고 게임 시작 시 플레이어에게 적용할 캐릭터 데이터입니다.
    /// </summary>
    public readonly struct CharacterDefinition
    {
        private const int UpgradeMaxLevelExpansionMultiplier = 2;

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
            int maxPlayerLevel,
            IReadOnlyDictionary<UpgradeType, int> normalUpgradeMaxLevels,
            string animationResourceFolder,
            bool invertHorizontalFacing,
            bool invertIdleHorizontalFacing,
            bool invertWalkHorizontalFacing,
            string projectileSpriteResourcePath,
            float projectileVisualScale,
            float projectileColliderRadius,
            SfxType attackSfxType,
            IReadOnlyList<SfxType> hitSfxTypes,
            SfxType deathSfxType,
            string battleBgmClipName)
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
            MaxPlayerLevel = maxPlayerLevel;
            NormalUpgradeMaxLevels = normalUpgradeMaxLevels;
            AnimationResourceFolder = animationResourceFolder;
            InvertHorizontalFacing = invertHorizontalFacing;
            InvertIdleHorizontalFacing = invertIdleHorizontalFacing;
            InvertWalkHorizontalFacing = invertWalkHorizontalFacing;
            ProjectileSpriteResourcePath = projectileSpriteResourcePath;
            ProjectileVisualScale = projectileVisualScale;
            ProjectileColliderRadius = projectileColliderRadius;
            AttackSfxType = attackSfxType;
            HitSfxTypes = hitSfxTypes;
            DeathSfxType = deathSfxType;
            BattleBgmClipName = battleBgmClipName;
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
        public int MaxPlayerLevel { get; }
        public IReadOnlyDictionary<UpgradeType, int> NormalUpgradeMaxLevels { get; }
        public string AnimationResourceFolder { get; }
        public bool InvertHorizontalFacing { get; }
        public bool InvertIdleHorizontalFacing { get; }
        public bool InvertWalkHorizontalFacing { get; }
        public string ProjectileSpriteResourcePath { get; }
        public float ProjectileVisualScale { get; }
        public float ProjectileColliderRadius { get; }
        public SfxType AttackSfxType { get; }
        public IReadOnlyList<SfxType> HitSfxTypes { get; }
        public SfxType DeathSfxType { get; }
        public string BattleBgmClipName { get; }

        public int GetMaxLevel(UpgradeDefinition definition)
        {
            if (definition == null)
                return 0;

            if (definition.IsCharacterExclusiveUpgrade
                && (string.Equals(definition.RequiredCharacterId, "selene", System.StringComparison.OrdinalIgnoreCase)
                    || string.Equals(definition.RequiredCharacterId, "hanseorin", System.StringComparison.OrdinalIgnoreCase)))
                return definition.MaxLevel;

            if (definition.Unlimited || definition.IsSpecialUpgrade || definition.IsCharacterExclusiveUpgrade)
                return GetExpandedMaxLevel(definition.MaxLevel);

            if (NormalUpgradeMaxLevels != null
                && NormalUpgradeMaxLevels.TryGetValue(definition.UpgradeType, out int maxLevel))
                return GetExpandedMaxLevel(maxLevel);

            return GetExpandedMaxLevel(definition.MaxLevel);
        }

        private static int GetExpandedMaxLevel(int baseMaxLevel)
        {
            return baseMaxLevel <= 0
                ? 0
                : baseMaxLevel * UpgradeMaxLevelExpansionMultiplier;
        }
    }
}
