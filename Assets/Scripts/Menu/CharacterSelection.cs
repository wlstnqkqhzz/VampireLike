using System.Collections.Generic;
using VampireLike.Audio;
using VampireLike.Growth;

namespace VampireLike.Menu
{
    /// <summary>
    /// 메인 메뉴에서 고른 플레이어 캐릭터 정보를 보관합니다.
    /// </summary>
    public static class CharacterSelection
    {
        private static readonly CharacterDefinition[] characters =
        {
            new CharacterDefinition(
                "kael",
                "\uCE74\uC5D8",
                "\uD751\uAC80 \uC218\uD638\uC790",
                "\uB290\uB9AC\uC9C0\uB9CC \uB2E8\uB2E8\uD55C \uD751\uAC80 \uC804\uC0AC\uC785\uB2C8\uB2E4. \uB192\uC740 \uCCB4\uB825\uACFC \uAC15\uD55C \uD55C \uBC29\uC73C\uB85C \uC801\uC744 \uBC84\uD301\uB2C8\uB2E4.",
                0.86f,
                1.22f,
                1.55f,
                0,
                20,
                44,
                new Dictionary<UpgradeType, int>
                {
                    { UpgradeType.ProjectileDamage, 6 },
                    { UpgradeType.AttackInterval, 3 },
                    { UpgradeType.ProjectileCount, 1 },
                    { UpgradeType.ProjectilePierce, 3 },
                    { UpgradeType.MoveSpeed, 3 },
                    { UpgradeType.MaxHealth, 6 },
                    { UpgradeType.PickupRadius, 3 }
                },
                "PlayerAnimations/KaelProcessed",
                true,
                "Projectiles/KaelSwordWave",
                1.28f,
                0.2f,
                SfxType.KaelSwordWave),
            new CharacterDefinition(
                "selene",
                "\uC140\uB808\uB124",
                "\uC6D4\uC601 \uC30D\uAC80",
                "\uB2EC\uBE5B\uCC98\uB7FC \uBE60\uB974\uAC8C \uD30C\uACE0\uB4DC\uB294 \uC30D\uAC80 \uC554\uC0B4\uC790\uC785\uB2C8\uB2E4. \uAE30\uB3D9\uC131\uACFC \uC5F0\uC0AC\uB825\uC774 \uB192\uC9C0\uB9CC \uCCB4\uB825\uC740 \uB0AE\uC2B5\uB2C8\uB2E4.",
                1.24f,
                0.74f,
                0.48f,
                1,
                0,
                55,
                new Dictionary<UpgradeType, int>
                {
                    { UpgradeType.ProjectileDamage, 3 },
                    { UpgradeType.AttackInterval, 6 },
                    { UpgradeType.ProjectileCount, 4 },
                    { UpgradeType.ProjectilePierce, 2 },
                    { UpgradeType.MoveSpeed, 6 },
                    { UpgradeType.MaxHealth, 3 },
                    { UpgradeType.PickupRadius, 5 }
                },
                "PlayerAnimations/SeleneProcessed",
                false,
                "Projectiles/SeleneDagger",
                0.75f,
                0.09f,
                SfxType.SeleneDaggerThrow)
        };

        private static int selectedIndex;

        public static CharacterDefinition[] Characters => characters;
        public static int SelectedIndex => selectedIndex;
        public static CharacterDefinition SelectedCharacter => characters[selectedIndex];

        public static void Select(int index)
        {
            if (index < 0 || index >= characters.Length)
                return;

            selectedIndex = index;
        }
    }
}
