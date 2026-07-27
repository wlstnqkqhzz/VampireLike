using UnityEngine;
using VampireLike.Combat;
using VampireLike.Growth;

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
            global::PlayerSpriteAnimator spriteAnimator = GetComponent<global::PlayerSpriteAnimator>();
            PlayerAutoAttack autoAttack = GetComponent<PlayerAutoAttack>();
            PlayerHealth playerHealth = GetComponent<PlayerHealth>();
            PlayerExperience playerExperience = GetComponent<PlayerExperience>();

            if (spriteAnimator == null)
                spriteAnimator = gameObject.AddComponent<global::PlayerSpriteAnimator>();

            if (playerController != null)
                playerController.MultiplyMoveSpeed(character.MoveSpeedMultiplier);

            if (spriteAnimator != null)
            {
                spriteAnimator.SetResourceFolder(character.AnimationResourceFolder);
                spriteAnimator.SetInvertHorizontalFacing(character.InvertHorizontalFacing);
            }

            if (autoAttack != null)
            {
                autoAttack.MultiplyAttackInterval(character.AttackIntervalMultiplier);
                autoAttack.MultiplyProjectileDamage(character.ProjectileDamageMultiplier);
                autoAttack.AddProjectileCount(character.BonusProjectileCount);
            }

            if (playerHealth != null && character.BonusMaxHealth > 0)
                playerHealth.IncreaseMaxHealth(character.BonusMaxHealth);

            if (playerExperience != null)
                playerExperience.SetMaxLevel(character.MaxPlayerLevel);
        }
    }
}
