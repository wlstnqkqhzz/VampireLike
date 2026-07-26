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
                "검은 갑주와 보랏빛 마력을 두른 전사입니다. 느리지만 튼튼하고 한 발의 피해가 강합니다.",
                0.9f,
                1.18f,
                1.35f,
                0,
                15,
                "PlayerAnimations/KaelProcessed",
                true),
            new CharacterDefinition(
                "selene",
                "셀레네",
                "월영 쌍검",
                "달빛처럼 빠르게 파고드는 쌍검 암살자입니다. 빠르게 움직이며 처음부터 두 발을 쏩니다.",
                1.18f,
                0.68f,
                0.75f,
                1,
                0,
                "PlayerAnimations/SeleneProcessed",
                false)
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
