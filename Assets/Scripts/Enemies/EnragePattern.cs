using System.Collections;
using UnityEngine;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 지정한 체력 이하에서 한 번만 발동해 보스의 이동 속도와 패턴 빈도를 높이는 패턴이다.
    /// </summary>
    public class EnragePattern : BossPattern
    {
        [SerializeField]
        private float triggerHealthRatio = 0.5f;

        [SerializeField]
        private float moveSpeedMultiplier = 1.25f;

        [SerializeField]
        private float cooldownMultiplier = 0.75f;

        [SerializeField]
        private float pauseDuration = 0.35f;

        [SerializeField]
        private Color enragedColor = new Color(1f, 0.45f, 0.35f, 1f);

        private bool hasTriggered;
        private SpriteRenderer spriteRenderer;

        public override void Initialize(BossController boss)
        {
            base.Initialize(boss);
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        protected override bool CanExecutePattern()
        {
            return !hasTriggered && Boss.ActiveBossHealthRatio() <= triggerHealthRatio;
        }

        protected override IEnumerator ExecutePattern()
        {
            hasTriggered = true;
            Boss.SetState(BossState.PhaseChanging, false);
            Boss.MultiplyMoveSpeed(moveSpeedMultiplier);
            Boss.MultiplyPatternCooldown(cooldownMultiplier);

            if (spriteRenderer != null)
                spriteRenderer.color = enragedColor;

            yield return new WaitForSeconds(pauseDuration);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            triggerHealthRatio = Mathf.Clamp01(triggerHealthRatio);
            moveSpeedMultiplier = Mathf.Max(0f, moveSpeedMultiplier);
            cooldownMultiplier = Mathf.Max(0.1f, cooldownMultiplier);
            pauseDuration = Mathf.Max(0f, pauseDuration);
        }
    }
}
