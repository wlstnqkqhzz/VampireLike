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
                99,
                new Dictionary<UpgradeType, int>
                {
                    { UpgradeType.ProjectileDamage, 12 },
                    { UpgradeType.AttackInterval, 5 },
                    { UpgradeType.ProjectileCount, 2 },
                    { UpgradeType.ProjectilePierce, 7 },
                    { UpgradeType.MoveSpeed, 5 },
                    { UpgradeType.MaxHealth, 12 },
                    { UpgradeType.PickupRadius, 5 }
                },
                "PlayerAnimations/KaelProcessed",
                true,
                false,
                false,
                "Projectiles/KaelSwordWave",
                1.28f,
                0.2f,
                SfxType.KaelAttack,
                new[] { SfxType.KaelHit1, SfxType.KaelHit2 },
                SfxType.KaelDeath,
                "kael_battle_theme"),
            new CharacterDefinition(
                "selene",
                "\uC140\uB808\uB124",
                "\uC740\uC6D4\uC758 \uC810\uC131\uC220\uC0AC",
                "\uB2EC\uBE5B\uACFC \uBCC4\uBE5B\uC73C\uB85C \uB113\uC740 \uBC94\uC704\uB97C \uC81C\uC5B4\uD558\uB294 \uB9C8\uBC95\uC0AC\uC785\uB2C8\uB2E4. \uBC94\uC704 \uD53C\uD574\uC640 \uAD70\uC911 \uC81C\uC5B4\uC5D0 \uD2B9\uD654\uB418\uC5B4 \uC788\uC9C0\uB9CC \uCCB4\uB825\uC740 \uB0AE\uC2B5\uB2C8\uB2E4.",
                1f,
                1f,
                1f,
                1,
                0,
                99,
                new Dictionary<UpgradeType, int>
                {
                    { UpgradeType.ProjectileDamage, 7 },
                    { UpgradeType.AttackInterval, 9 },
                    { UpgradeType.ProjectileCount, 6 },
                    { UpgradeType.ProjectilePierce, 4 },
                    { UpgradeType.MoveSpeed, 7 },
                    { UpgradeType.MaxHealth, 4 },
                    { UpgradeType.PickupRadius, 9 }
                },
                "PlayerAnimations/SeleneProcessed",
                false,
                true,
                false,
                "Projectiles/SeleneMoonOrb",
                0.58f,
                0.1f,
                SfxType.SeleneAttack,
                new[] { SfxType.SeleneHit1, SfxType.SeleneHit2 },
                SfxType.SeleneDeath,
                "selene_battle_theme"),
            new CharacterDefinition(
                "hanseorin",
                "\uD55C\uC11C\uB9B0",
                "\uC801\uC6D4\uC758 \uC554\uC0B4\uC790",
                "\uBD89\uC740 \uB2EC\uBE5B\uCC98\uB7FC \uBE60\uB974\uAC8C \uB2E8\uAC80\uC744 \uB358\uC838 \uB2E8\uC77C \uC801\uC744 \uCC98\uCE58\uD558\uB294 \uC554\uC0B4\uC790\uC785\uB2C8\uB2E4. \uB192\uC740 \uAE30\uB3D9\uC131\uACFC \uB2E8\uC77C \uD3ED\uB51C\uC5D0 \uD2B9\uD654\uB418\uC5B4 \uC788\uC9C0\uB9CC \uCCB4\uB825\uC740 \uB0AE\uC2B5\uB2C8\uB2E4.",
                1.22f,
                0.68f,
                1.1f,
                0,
                0,
                99,
                new Dictionary<UpgradeType, int>
                {
                    { UpgradeType.ProjectileDamage, 10 },
                    { UpgradeType.AttackInterval, 12 },
                    { UpgradeType.ProjectileCount, 4 },
                    { UpgradeType.ProjectilePierce, 6 },
                    { UpgradeType.MoveSpeed, 12 },
                    { UpgradeType.MaxHealth, 4 },
                    { UpgradeType.PickupRadius, 5 }
                },
                "PlayerAnimations/HanSeorinProcessed",
                false,
                false,
                false,
                "Projectiles/HanSeorinDagger",
                0.72f,
                0.08f,
                SfxType.HanSeorinAttack,
                new[] { SfxType.HanSeorinHit1, SfxType.HanSeorinHit2, SfxType.HanSeorinHit3 },
                SfxType.HanSeorinDeath,
                "hanseorin_battle_theme")
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
