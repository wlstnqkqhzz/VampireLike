using System.Collections;
using UnityEngine;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 모든 보스 패턴이 상속하는 공통 기반 클래스다.
    /// 보스별 프리팹에 여러 패턴 컴포넌트를 붙여 조합할 수 있다.
    /// </summary>
    public abstract class BossPattern : MonoBehaviour
    {
        [SerializeField]
        private float cooldown = 3f;

        [SerializeField]
        private float initialDelay = 0f;

        [SerializeField]
        private int priority = 1;

        [SerializeField]
        private int minimumPhase = 1;

        [SerializeField]
        private int maximumPhase = 3;

        [SerializeField]
        private bool allowMovementDuringPattern;

        private float nextReadyTime;

        public int Priority => priority;
        public bool AllowMovementDuringPattern => allowMovementDuringPattern;

        protected BossController Boss { get; private set; }
        protected Transform Player => Boss.Player;
        protected Rigidbody2D BossRigidbody => Boss.BossRigidbody;

        public virtual void Initialize(BossController boss)
        {
            Boss = boss;
            nextReadyTime = Time.time + initialDelay;
            ValidateValues();
        }

        public bool CanExecute()
        {
            if (Boss == null || !enabled || Boss.IsDead)
                return false;

            if (Time.time < nextReadyTime)
                return false;

            int phase = Boss.CurrentPhase;
            return phase >= minimumPhase && phase <= maximumPhase && CanExecutePattern();
        }

        public IEnumerator Execute()
        {
            MarkUsed();
            Boss?.PlayAttackAnimation();
            yield return ExecutePattern();
        }

        public void MarkUsed()
        {
            float multiplier = Boss == null ? 1f : Boss.PatternCooldownMultiplier;
            nextReadyTime = Time.time + cooldown * multiplier;
        }

        protected virtual bool CanExecutePattern()
        {
            return true;
        }

        protected abstract IEnumerator ExecutePattern();

        protected virtual void OnValidate()
        {
            ValidateValues();
        }

        private void ValidateValues()
        {
            cooldown = Mathf.Max(0f, cooldown);
            initialDelay = Mathf.Max(0f, initialDelay);
            priority = Mathf.Max(0, priority);
            minimumPhase = Mathf.Clamp(minimumPhase, 1, 3);
            maximumPhase = Mathf.Clamp(maximumPhase, minimumPhase, 3);
        }
    }
}
