namespace VampireLike.Menu
{
    /// <summary>
    /// 씬이 다시 로드되어도 선택한 캐릭터를 잠깐 보관하는 런타임 선택 상태입니다.
    /// </summary>
    public static class CharacterSelection
    {
        private static readonly CharacterDefinition[] characters =
        {
            new CharacterDefinition(
                "vampire",
                "뱀파이어",
                "균형형",
                "기본 이동과 공격 능력이 안정적인 첫 캐릭터입니다.",
                1f,
                1f,
                0),
            new CharacterDefinition(
                "hunter",
                "헌터",
                "속공형",
                "이동과 공격이 조금 빠른 대신 생존 보너스는 없습니다.",
                1.12f,
                0.9f,
                0)
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
