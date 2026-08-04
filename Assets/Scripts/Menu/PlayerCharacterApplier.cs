using UnityEngine;
using UnityEngine.SceneManagement;
using VampireLike.Combat;
using VampireLike.Growth;

namespace VampireLike.Menu
{
    /// <summary>
    /// 선택한 캐릭터의 시작 능력치를 Player 컴포넌트에 한 번만 적용합니다.
    /// </summary>
    public class PlayerCharacterApplier : MonoBehaviour
    {
        private const string MainMenuSceneName = "MainMenuScene";
        private const string PlayerObjectName = "Player";

        private bool hasApplied;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeSceneHook()
        {
            SceneManager.sceneLoaded -= ApplySelectedCharacterToLoadedScene;
            SceneManager.sceneLoaded += ApplySelectedCharacterToLoadedScene;
        }

        /// <summary>
        /// 메인 메뉴에서 선택한 캐릭터가 게임 씬의 Player에 반드시 적용되도록 보장합니다.
        /// </summary>
        private static void ApplySelectedCharacterToLoadedScene(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == MainMenuSceneName)
                return;

            GameObject player = GameObject.Find(PlayerObjectName);

            if (player == null)
                return;

            PlayerCharacterApplier applier = player.GetComponent<PlayerCharacterApplier>();

            if (applier == null)
                applier = player.AddComponent<PlayerCharacterApplier>();

            applier.ApplySelectedCharacter();
        }

        private void Start()
        {
            ApplySelectedCharacter();
        }

        public void ApplySelectedCharacter()
        {
            if (hasApplied)
                return;

            hasApplied = true;
            CharacterDefinition character = CharacterSelection.SelectedCharacter;
            GameSessionStats.RecordCharacter(character.Id, character.DisplayName, character.Role);

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

                Sprite projectileSprite = Resources.Load<Sprite>(character.ProjectileSpriteResourcePath);
                autoAttack.SetProjectileVisual(projectileSprite, character.ProjectileVisualScale, character.ProjectileColliderRadius);
                autoAttack.SetAttackSfx(character.AttackSfxType);
            }

            if (playerHealth != null && character.BonusMaxHealth > 0)
                playerHealth.IncreaseMaxHealth(character.BonusMaxHealth);

            if (playerExperience != null)
                playerExperience.SetMaxLevel(character.MaxPlayerLevel);
        }
    }
}
