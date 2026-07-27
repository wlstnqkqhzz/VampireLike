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
                "카엘",
                "흑검 수호자",
                "검은 갑주와 보랏빛 마력을 두른 전사입니다. 느리지만 단단하고 한 발의 피해가 강합니다.",
                0.9f,
                1.18f,
                1.35f,
                0,
                15,
                45,
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
                1.25f,
                0.2f,
                SfxType.KaelSwordWave),
            new CharacterDefinition(
                "selene",
                "셀레네",
                "월영 쌍검",
                "달빛처럼 빠르게 파고드는 쌍검 암살자입니다. 기동성과 연사력이 높지만 한 발 피해와 체력은 낮습니다.",
                1.18f,
                0.68f,
                0.75f,
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
