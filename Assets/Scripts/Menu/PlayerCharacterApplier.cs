using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Menu
{
    /// <summary>
    /// 선택한 캐릭터의 시작 능력치를 Player 컴포넌트에 한 번만 적용합니다.
    /// </summary>
    public class PlayerCharacterApplier : MonoBehaviour
    {
        private bool hasApplied;

        public void ApplySelectedCharacter()
        {
            if (hasApplied)
                return;

            hasApplied = true;
            CharacterDefinition character = CharacterSelection.SelectedCharacter;

            global::PlayerController playerController = GetComponent<global::PlayerController>();
            PlayerAutoAttack autoAttack = GetComponent<PlayerAutoAttack>();
            PlayerHealth playerHealth = GetComponent<PlayerHealth>();

            if (playerController != null)
                playerController.MultiplyMoveSpeed(character.MoveSpeedMultiplier);

            if (autoAttack != null)
                autoAttack.MultiplyAttackInterval(character.AttackIntervalMultiplier);

            if (playerHealth != null && character.BonusMaxHealth > 0)
                playerHealth.IncreaseMaxHealth(character.BonusMaxHealth);
        }
    }
}
